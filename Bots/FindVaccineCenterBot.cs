using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FindVaccineCenterBot.Clients;
using FindVaccineCenterBot.Helpers;
using FindVaccineCenterBot.Models.ApiResponse;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace FindVaccineCenterBot.Bots
{
    public class FindVaccineCenterBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Equals("about", System.StringComparison.InvariantCultureIgnoreCase)
                || turnContext.Activity.Text.Equals("/start", System.StringComparison.InvariantCultureIgnoreCase))
                await HandleAboutAsync(turnContext, cancellationToken);
            else if (new Regex("\\d{6}").IsMatch(turnContext.Activity.Text))
                await HandleSearchByZipcodeAsync(turnContext, cancellationToken);
            else
                await HandleSearchByDistrictNameAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello! Using this bot you can easily find your nearby vaccine center. You can search by entering your Pin code (For e.g. 110001) or by entering District name (For e.g. New Delhi).";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        #region Private Handlers

        private async Task HandleAboutAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = @"Hi, This bot is created by **Nilay Mehta**. Made with ❤ in *Ahmedabad, India*.

This Bot is created for just learning purposes only.
Code for this bot is open source on [GitHub](https://github.com/mehtanilay10/), feel free to collaborate in that.
This bot uses the [public API of CoWin](https://apisetu.gov.in/public/marketplace/api/cowin/cowin-public-v2) to fetch data.

Get in touch with me @ https://www.mnilay.com/";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

            replyText = "Hello! Using this bot you can easily find your nearby vaccine center. You can search by entering your Pin code (For e.g. 110001) or by entering District name (For e.g. New Delhi).";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        private async Task HandleSearchByZipcodeAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Wait while we are loading centers details for Pin code - {turnContext.Activity.Text}.";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

            var client = new CowinClient();
            var result = await client.FindByZipcode(turnContext.Activity.Text);

            if (result.centers.Any())
            {
                var attachmentHelper = new AttachmentHelper();
                var attachments = result.centers.Select(x => attachmentHelper.CreateAttachment(x)).ToList();
                await turnContext.SendActivityAsync(MessageFactory.Carousel(attachments), cancellationToken);
            }
            else
            {
                replyText = "Not found any center for given Pin code. Try with District search by entering distrinct name (For e.g. New Delhi).";
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
        }

        private async Task HandleSearchByDistrictNameAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Wait while we are loading centers details for District name - {turnContext.Activity.Text}.";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

            var client = new CowinClient();
            List<int> districtIds = DistrictHelper.GetDistrictCodes(turnContext.Activity.Text);

            if (districtIds.Any())
            {
                List<Center> allCenters = new List<Center>();

                foreach (int districtId in districtIds)
                {
                    var result = await client.FindByDistrict(districtId);
                    allCenters.AddRange(result.centers);
                }

                if (allCenters.Any())
                {
                    var attachmentHelper = new AttachmentHelper();
                    var attachments = allCenters.Select(x => attachmentHelper.CreateAttachment(x)).ToList();
                    await turnContext.SendActivityAsync(MessageFactory.Carousel(attachments), cancellationToken);
                }
                else
                {
                    replyText = "Not found any center for given District name. Try with Pin code search by entering Pin code (For e.g. 110001).";
                    await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
                }
            }
            else
            {
                replyText = $"Enter valid Indian pin code (For e.g. 110001) or district name (For e.g. New Delhi). {turnContext.Activity.Text} is invalid pincode or not found any district with same name.";
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
        }

        #endregion
    }
}
