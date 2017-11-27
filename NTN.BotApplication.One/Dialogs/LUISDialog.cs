using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using NTN.BotApplication.One.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace NTN.BotApplication.One.Dialogs
{
    [LuisModel("1db3c696-205b-4552-8b62-663e759e1b49", "addb70f9ceb04da2a9ad0562fac6be59")]
    [Serializable]
    public class LUISDialog : LuisDialog<LeadModel>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry i do not understand.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("How can I help you ?.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Interest")]
        public async Task Interest(IDialogContext context, LuisResult result)
        {
            LeadModel leadModel = new LeadModel();
            var leadForm = new FormDialog<LeadModel>(leadModel, LeadModel.BuildForm, FormOptions.PromptInStart);
            context.Call<LeadModel>(leadForm, Callback);
        }

        private async Task Callback(IDialogContext context,IAwaitable<LeadModel> result)
        {
            context.Wait(MessageReceived);
        }

        [LuisIntent("QueryProduct")]
        public async Task QueryProduct(IDialogContext context, LuisResult result)
        {
            foreach(var entity in result.Entities)
            {
                var value = entity.Entity.ToUpper();

                if(entity.Type.ToUpper() == "PRODUCT")
                {
                    if(value == "PRODUCT1" || value == "PRODUCT2" || value == "PRODUCT3")
                    {
                        await context.PostAsync("Yes, we have this product");
                        context.Wait(MessageReceived);
                        return;
                    }
                    else
                    {
                        await context.PostAsync("Sorry, we don't have that product.");
                        context.Wait(MessageReceived);
                        return;
                    }
                }

                await context.PostAsync("Please let me know how can we help");
                context.Wait(MessageReceived);
                return;
            }
        }

    }
}