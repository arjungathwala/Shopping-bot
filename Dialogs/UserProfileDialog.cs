// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class UserProfileDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        public UserProfileDialog(UserState userState)
            : base(nameof(UserProfileDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                CategoryStepAsync,
                CategoryStep2Async,
                CategoryStep3Async,
                cardCategoryStepAsync,
                ConfirmStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), AgePromptValidatorAsync));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt), PicturePromptValidatorAsync));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> CategoryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Here are our products"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Men", "Women", "Kids" }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> CategoryStep2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["category"] = ((FoundChoice)stepContext.Result).Value;
            string category = (string)stepContext.Values["category"];

            var promptList = new List<string> { "T-Shirts", "PhotoFrames", "Bouquets" };

            var attachment = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachment);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            if (category == "Men")
            {
                var cardDisplay1 = new Attachment();
                var cardDisplay2 = new Attachment();
                var cardDisplay3 = new Attachment();


                cardDisplay1.Name = "T-Shirts";
                cardDisplay1.ContentType = "image/png";
                cardDisplay1.ContentUrl = "https://eshopstorage.blob.core.windows.net/res/Men_carousel_tshirts.png";

                cardDisplay2.Name = "PhotoFrame";
                cardDisplay2.ContentType = "image/png";
                cardDisplay2.ContentUrl = "https://eshopstorage.blob.core.windows.net/res/Men_carousel_photo frames.png";

                cardDisplay3.Name = "Bouquets";
                cardDisplay3.ContentType = "image/png";
                cardDisplay3.ContentUrl = "https://eshopstorage.blob.core.windows.net/res/Men_carousel_Bouquets.png";

                reply.Attachments.Add(cardDisplay1);
                reply.Attachments.Add(cardDisplay2);
                reply.Attachments.Add(cardDisplay3);

                promptList = new List<string> { "T-Shirts", "PhotoFrames", "Bouquets" };
            }

            else
            if (category == "Women")
            {
                var cardDisplay1 = new Attachment();
                var cardDisplay2 = new Attachment();
                var cardDisplay3 = new Attachment();


                cardDisplay1.Name = "Cosmetics";
                cardDisplay1.ContentType = "image/png";
                cardDisplay1.ContentUrl = "https://eshopstorage.blob.core.windows.net/reswoman/Women_carousel_cosmetics.png";

                cardDisplay2.Name = "Fashion";
                cardDisplay2.ContentType = "image/png";
                cardDisplay2.ContentUrl = "https://eshopstorage.blob.core.windows.net/reswoman/Women_carousel_fashion.png";

                cardDisplay3.Name = "Cakes";
                cardDisplay3.ContentType = "image/png";
                cardDisplay3.ContentUrl = "https://eshopstorage.blob.core.windows.net/reswoman/Women_carousel_cakes.png";

                reply.Attachments.Add(cardDisplay1);
                reply.Attachments.Add(cardDisplay2);
                reply.Attachments.Add(cardDisplay3);

                promptList = new List<string> { "Cosmetics", "Fashion", "Cakes" };
            }

            else
            if (category == "Kids")
            {
                var cardDisplay1 = new Attachment();
                var cardDisplay2 = new Attachment();
                var cardDisplay3 = new Attachment();


                cardDisplay1.Name = "Toys";
                cardDisplay1.ContentType = "image/png";
                cardDisplay1.ContentUrl = "https://eshopstorage.blob.core.windows.net/reskids/Kids_carousel_toys.png";

                cardDisplay2.Name = "Candies";
                cardDisplay2.ContentType = "image/png";
                cardDisplay2.ContentUrl = "https://eshopstorage.blob.core.windows.net/reskids/Kids_carousel_candy.png";

                cardDisplay3.Name = "Fashion";
                cardDisplay3.ContentType = "image/png";
                cardDisplay3.ContentUrl = "https://eshopstorage.blob.core.windows.net/reskids/Kids_carousel_fashion.png";

                reply.Attachments.Add(cardDisplay1);
                reply.Attachments.Add(cardDisplay2);
                reply.Attachments.Add(cardDisplay3);

                promptList = new List<string> { "Toys", "Candies", "Fashion" };
            }
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            await stepContext.Context.SendActivityAsync($"Here are {stepContext.Values["category"]} collections");
            await stepContext.Context.SendActivityAsync(reply);

            return await stepContext.PromptAsync(nameof(ChoicePrompt),

                new PromptOptions
                {

                    Prompt = MessageFactory.Text(""),
                    Choices = ChoiceFactory.ToChoices(new List<string> { promptList[0], promptList[1], promptList[2] }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> CategoryStep3Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["category2"] = ((FoundChoice)stepContext.Result).Value;
            string category2 = (string)stepContext.Values["category2"];

            var promptList = new List<string> { "T-Shirts", "PhotoFrames", "Bouquets" };

            var attachment = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachment);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            if (category2 == "T-Shirts")
            {
                var cardDisplay1 = new Attachment();
                var cardDisplay2 = new Attachment();
                var cardDisplay3 = new Attachment();


                cardDisplay1.Name = "Men Orange & Black Solid Casual T-shirt";
                cardDisplay1.ContentType = "image/png";
                cardDisplay1.ContentUrl = "https://eshopstorage.blob.core.windows.net/res/Men_tshirt 1.png";

                cardDisplay2.Name = "U.S. Polo Assn Men Fit Casual T-shirt";
                cardDisplay2.ContentType = "image/png";
                cardDisplay2.ContentUrl = "https://eshopstorage.blob.core.windows.net/res/Men_tshirt 2.png";

                cardDisplay3.Name = "Striped Men Hooded Neck Red, Black T-shirt";
                cardDisplay3.ContentType = "image/png";
                cardDisplay3.ContentUrl = "https://eshopstorage.blob.core.windows.net/res/Men_tshirt 3.png";

                reply.Attachments.Add(cardDisplay1);
                reply.Attachments.Add(cardDisplay2);
                reply.Attachments.Add(cardDisplay3);

                promptList = new List<string> { "Orange & Black shirt", "U.S. Polo shirt", "Hooded Neck shirt" };
            }

            else
            if (category2 == "Cakes")
            {
                var cardDisplay1 = new Attachment();
                var cardDisplay2 = new Attachment();
                var cardDisplay3 = new Attachment();


                cardDisplay1.Name = "Mango & Strawberry flavored Cup cake";
                cardDisplay1.ContentType = "image/png";
                cardDisplay1.ContentUrl = "https://eshopstorage.blob.core.windows.net/reswoman/Women_Cake 1.png";
                
                cardDisplay2.Name = "Cream & Vanilla cup cakes";
                cardDisplay2.ContentType = "image/png";
                cardDisplay2.ContentUrl = "https://eshopstorage.blob.core.windows.net/reswoman/Women_Cake 2.png";

                cardDisplay3.Name = "Color full cake";
                cardDisplay3.ContentType = "image/png";
                cardDisplay3.ContentUrl = "https://eshopstorage.blob.core.windows.net/reswoman/Women_Cake 3.png";

                reply.Attachments.Add(cardDisplay1);
                reply.Attachments.Add(cardDisplay2);
                reply.Attachments.Add(cardDisplay3);


                promptList = new List<string> { "Mango & Strawberry cupcake", "Cream & Vanilla cupcake", "Color full cake" };
            }

            else
            if (category2 == "Candies")
            {
                var cardDisplay1 = new Attachment();
                var cardDisplay2 = new Attachment();
                var cardDisplay3 = new Attachment();


                cardDisplay1.Name = "Color full candy";
                cardDisplay1.ContentType = "image/png";
                cardDisplay1.ContentUrl = "https://eshopstorage.blob.core.windows.net/reskids/Kids_Candy 1.png";

                cardDisplay2.Name = "All fruit flavored candies";
                cardDisplay2.ContentType = "image/png";
                cardDisplay2.ContentUrl = "https://eshopstorage.blob.core.windows.net/reskids/Kids_Candy 2.png";

                cardDisplay3.Name = "Candies";
                cardDisplay3.ContentType = "image/png";
                cardDisplay3.ContentUrl = "https://eshopstorage.blob.core.windows.net/reskids/Kids_Candy 3.png";

                reply.Attachments.Add(cardDisplay1);
                reply.Attachments.Add(cardDisplay2);
                reply.Attachments.Add(cardDisplay3);


                promptList = new List<string> { "Color full candy", "All fruit flavored candies", "Candies" };
            }
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            await stepContext.Context.SendActivityAsync($"Here are {stepContext.Values["category"]} collections");
            await stepContext.Context.SendActivityAsync(reply);

            return await stepContext.PromptAsync(nameof(ChoicePrompt),

                new PromptOptions
                {

                    Prompt = MessageFactory.Text("  "),
                    Choices = ChoiceFactory.ToChoices(new List<string> { promptList[0], promptList[1], promptList[2] }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> cardCategoryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["category"] = ((FoundChoice)stepContext.Result).Value;
            string category = (string)stepContext.Values["category"];

            var cardDisplay = new HeroCard();

            if (category == "U.S. Polo shirt")
            {
                cardDisplay.Title = "U.S. Polo Assn Men Fit Casual T-shirt";
                cardDisplay.Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. ";
                cardDisplay.Images = new List<CardImage> { new CardImage("https://eshopstorage.blob.core.windows.net/res/Men_tshirt_details.png") };
               
                
            }

            else
            if (category == "Cream & Vanilla cupcake")
            {
                cardDisplay.Title = "Cream & Vanilla cup cakes";
                cardDisplay.Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. ";
                cardDisplay.Images = new List<CardImage> { new CardImage("https://eshopstorage.blob.core.windows.net/reswoman/Women_cake_details.png") };
                
            }

            else
            if (category == "All fruit flavored candies")
            {
                cardDisplay.Title = "All fruit flavored candies";
                cardDisplay.Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. ";
                cardDisplay.Images = new List<CardImage> { new CardImage("https://eshopstorage.blob.core.windows.net/reskids/Kids_candy_details.png") };
                
            }
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            var reply = MessageFactory.Attachment(cardDisplay.ToAttachment());
            await stepContext.Context.SendActivityAsync(reply);

            return await stepContext.PromptAsync(nameof(ChoicePrompt),

                new PromptOptions
                {

                    Prompt = MessageFactory.Text("  "),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Add to Cart", "Similar Products" }),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            /*var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            userProfile.Category = (string)stepContext.Values["category"];
            userProfile.Category2 = (string)stepContext.Values["category2"];
            userProfile.Name = (string)stepContext.Values["name"];*/

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($" U.S. Polo T-shirt has been added to cart."), cancellationToken);


            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        

        
        

       

        

        private static Task<bool> AgePromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            // This condition is our validation rule. You can also change the value at this point.
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value > 0 && promptContext.Recognized.Value < 150);
        }

        private static async Task<bool> PicturePromptValidatorAsync(PromptValidatorContext<IList<Attachment>> promptContext, CancellationToken cancellationToken)
        {
            if (promptContext.Recognized.Succeeded)
            {
                var attachments = promptContext.Recognized.Value;
                var validImages = new List<Attachment>();

                foreach (var attachment in attachments)
                {
                    if (attachment.ContentType == "image/jpeg" || attachment.ContentType == "image/png")
                    {
                        validImages.Add(attachment);
                    }
                }

                promptContext.Recognized.Value = validImages;

                // If none of the attachments are valid images, the retry prompt should be sent.
                return validImages.Any();
            }
            else
            {
                await promptContext.Context.SendActivityAsync("No attachments received. Proceeding without a profile picture...");

                // We can return true from a validator function even if Recognized.Succeeded is false.
                return true;
            }
        }
    }
}
