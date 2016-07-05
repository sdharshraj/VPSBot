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
            string message = result.Query + $" sir. \n How can i help you? type anything";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        [LuisIntent("ending")]
        public async Task Ending(IDialogContext context, LuisResult result)
        {
            string message = $"Welcome sir.";
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
                    string os = null;
                    string processor = null;
                    string ram = null;
                    switch (entity.Type)
                    {
                        case "Type":
                                switch (entity.Entity)
                                {
                                    case "vps":
                                    case "vpss":
                                        type = "VPS"; break;

                                    default:
                                        type = "RDP";
                                        break;
                                }
                            break;
                        case "os":
                            switch (entity.Entity)
                            {
                                case "windows":
                                case "window":
                                case "win":
                                    os = "Windows"; break;

                                default:
                                    os = "Linux";
                                    break;
                            }
                            break;
                        case "Processor":
                            switch (entity.Entity)
                            {
                                case "one core":
                                case "single core":
                                case "singlecore":
                                    processor = "One Core"; break;
                                case "two core":
                                case "double core":
                                case "twocore":
                                case "doublecore":
                                    processor = "Two Core"; break;
                                case "eight core":
                                case "eightcore":
                                case "octacore":
                                    processor = "Octa Core"; break;
                                default:
                                    processor = "Quade Core";
                                    break;
                            }
                            break;
                        case "Ram":
                            switch (entity.Entity)
                            {
                                case "1gb":
                                case "one gb":
                                case "onegb":
                                    ram = "One GB"; break;
                                case "2gb":
                                case "two gb":
                                case "twogb":
                                    ram = "Two GB"; break;
                                case "3gb":
                                case "three gb":
                                case "threegb":
                                    ram = "Three GB"; break;
                                default:
                                    ram = "Four GB";
                                    break;
                            }
                            break;
                    }
                    if (type != null)
                    {
                        entities.Add(new EntityRecommendation(type: "type") { Entity = type });
                    }
                    if (os != null)
                    {
                        entities.Add(new EntityRecommendation(type: "os") { Entity = os });
                    }
                    if (processor != null)
                    {
                        entities.Add(new EntityRecommendation(type: "processor") { Entity = processor });
                    }
                    if (ram != null)
                    {
                        entities.Add(new EntityRecommendation(type: "ram") { Entity = ram });
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