using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NTN.BotApplication.One.Models
{
    public enum InterestOptions { Product1 = 1, Product2 = 2, Product3 = 3 };

    [Serializable]
    public class LeadModel
    {
        public InterestOptions Product;
        public string Name;
        public string Description;
        public static IForm<LeadModel> BuildForm()
        {
            return new FormBuilder<LeadModel>()
            .Message("Welcome to the CRM bot !")
            .OnCompletion(CreateLeadInCRM)
            .Build();
        }
        private static async Task CreateLeadInCRM(IDialogContext context, LeadModel state)
        {
            await context.PostAsync("Thanks for showing your interest we will contact you shortly.");
            Entity lead = new Entity("lead");
            lead.Attributes["subject"] = "Interested in product " + state.Product.ToString();
            lead.Attributes["lastname"] = state.Name;
            lead.Attributes["description"] = state.Description;
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