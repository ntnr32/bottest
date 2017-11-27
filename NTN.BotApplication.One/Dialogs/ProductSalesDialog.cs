using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Bot.Connector;

namespace NTN.BotApplication.One.Dialogs
{
    public enum InterestOptions
    {
        Product1,
        Product2,
        Product3
    }

    [Serializable]
    public class ProductSalesDialog : IDialog
    {
        InterestOptions interestOptions;
        string name;
        string description;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hello! How can i help you?");

            context.Wait(MessageRecieveAsync);
        }

        public async Task MessageRecieveAsync(IDialogContext context,IAwaitable<IMessageActivity> argument)
        {
            // get the message
            var message = await argument;

            if (message.Text.Contains("interested"))
            {
                PromptDialog.Choice(
                context: context,
                resume: ResumeGetInterest,
                options: (IEnumerable<InterestOptions>)Enum.GetValues(typeof(InterestOptions)),
                prompt: "Which product are your interested in :",
                retry: "I didn't understand. Please try again.");
            }
        }

        public async Task ResumeGetInterest(IDialogContext context, IAwaitable<InterestOptions> result)
        {
            interestOptions = await result;

            PromptDialog.Text(
            context: context,
            resume: ResumeGetName,
            prompt: "Please provide your name",
            retry: "I didn't understand. Please try again.");
        }

        public async Task ResumeGetName(IDialogContext context, IAwaitable<string> result)
        {
            name = await result;

            PromptDialog.Text(
            context: context,
            resume: ResumeGetDescription,
            prompt: "Please provide a detailed description",
            retry: "I didn't understand. Please try again.");
        }

        public async Task ResumeGetDescription(IDialogContext context, IAwaitable<string> result)
        {
            description = await result;
            PromptDialog.Confirm(
            context: context,
            resume: ResumeAndConfirm,
            prompt: $"You entered Product :- '{interestOptions}', Your Name - '{name}', and Description - '{description}'. Is that correct?",
            retry: "I didn't understand. Please try again.");
        }

        public async Task ResumeAndConfirm(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirm = await result;

            if (confirm)
                await context.PostAsync("Thanks for showing your interest we will contact you shortly.");

            // Create a lead record in CRM
            CreateLeadinCRM();

        }

        private void CreateLeadinCRM()
        {
            Microsoft.Xrm.Sdk.Entity lead = new Microsoft.Xrm.Sdk.Entity("lead");
            lead.Attributes["subject"] = "Interested in product " + interestOptions;
            lead.Attributes["lastname"] = name;
            lead.Attributes["description"] = description;
            GetOrganizationService().Create(lead);
        }

        public static OrganizationServiceProxy GetOrganizationService()
        {
            IServiceManagement<IOrganizationService> orgServiceManagement =
            ServiceConfigurationFactory.CreateManagement<IOrganizationService>(new Uri("https://test9v.crm.dynamics.com/XRMServices/2011/Organization.svc"));

            AuthenticationCredentials authCredentials = new AuthenticationCredentials();
            authCredentials.ClientCredentials.UserName.UserName = "crm.admin@test9v.onmicrosoft.com";
            authCredentials.ClientCredentials.UserName.Password = "Change123";
            AuthenticationCredentials tokenCredentials = orgServiceManagement.Authenticate(authCredentials);
            return new OrganizationServiceProxy(orgServiceManagement, tokenCredentials.SecurityTokenResponse);
        }
    }
}