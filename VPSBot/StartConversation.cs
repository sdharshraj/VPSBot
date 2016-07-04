using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Luis.Models;

namespace VPSBot
{
    [LuisModel("f27beca9-b4a3-4a69-a4be-57988b5cc4ae", "1881d9bae8d64cddab0e57eb02b3d5e5")]
    [Serializable]
    public class StartConversation : LuisDialog<ProductOrder>
    {
        private readonly BuildFormDelegate<ProductOrder> MakeProductForm;
        
        internal StartConversation(BuildFormDelegate<ProductOrder> makeProductForm)
        {
            this.MakeProductForm = makeProductForm;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("greet")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            string message = result.Query + $" sir. \n How can i help you?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        [LuisIntent("ending")]
        public async Task Ending(IDialogContext context, LuisResult result)
        {
            string message = $"Welcome sir. \n Is there anything I can help you with?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        [LuisIntent("show")]
        public async Task Show(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            if (entities.Any((entity) => entity.Type == "Type"))
            {
                // this part to deal ---------------------------------------------------------
                foreach (var entity in result.Entities)
                {
                    string type = null;
                    switch (entity.Type)
                    {
                        case "vps": type = "VPS"; break;
                        default:
                            type = "RDP";
                            break;
                    }
                    if (type != null)
                    {
                        entities.Add(new EntityRecommendation(type: type) { Entity = "Type" });
                        break;
                    }
                }
            }
          
            var productForm = new FormDialog<ProductOrder>(new ProductOrder(), this.MakeProductForm, FormOptions.PromptInStart, entities);
            context.Call<ProductOrder>(productForm, ProductFormComplete);
        }

        private async Task ProductFormComplete(IDialogContext context, IAwaitable<ProductOrder> result)
        {
            ProductOrder order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");
                return;
            }

            if (order != null)
            {
                await context.PostAsync("Your Product Order: " + order.ToString());
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            context.Wait(MessageReceived);
        }

        public static IForm<ProductOrder> BuildForm()
        {
            var builder = new FormBuilder<ProductOrder>();
            ActiveDelegate<ProductOrder> isWindows = (product) => product.os == productOS.Windows;
            ActiveDelegate<ProductOrder> isLinux = (product) => product.os == productOS.Linux;

            return builder
                .Field(nameof(ProductOrder.type))
                .Field(nameof(ProductOrder.os))
                .Field(nameof(ProductOrder.windowsUses), isWindows)
                .Field(nameof(ProductOrder.linuxUses), isLinux)
                .AddRemainingFields()
                .Confirm("Would you like a Type : {type} \n, Operating System : {os}\n, Uses : {windowsUses},\n Processor : {processor}, \n RAM : {ram} \n {type}?", isWindows)
                .Confirm("Would you like a Type : {type} \n, Operating System : {os}\n, Uses : {linuxUses},\n Processor : {processor}, \n RAM : {ram} \n {type}", isLinux)
                .Build()
                ;
        }

    }
}