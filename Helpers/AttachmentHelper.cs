using System.Collections.Generic;
using AdaptiveCards;
using FindVaccineCenterBot.Models.ApiResponse;
using Microsoft.Bot.Schema;

namespace FindVaccineCenterBot.Helpers
{
    public class AttachmentHelper
    {
        public Attachment CreateAttachment(Center center)
        {
            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = CreateAdaptiveCard(center)
            };
        }

        #region Private Methods

        private AdaptiveCard CreateAdaptiveCard(Center center)
        {
            string isFree = center.fee_type == "Free" ? "Yes" : "No";

            var card = new AdaptiveCard("1.3");
            card.Body.Add(new AdaptiveTextBlock(center.name)
            {
                Size = AdaptiveTextSize.Medium,
                Weight = AdaptiveTextWeight.Bolder
            });

            card.Body.Add(new AdaptiveColumnSet()
            {
                Columns = new List<AdaptiveColumn>() { new AdaptiveColumn {
                    Width="stretch",
                    Items = new List<AdaptiveElement>
                    {
                        new AdaptiveTextBlock($"{center.address} - {center.pincode}")
                        {
                            Weight=AdaptiveTextWeight.Bolder,
                            Wrap=true
                        },
                        new AdaptiveTextBlock($"Is Free - {isFree}")
                        {
                            Wrap=true
                        }
                    }
                } }
            });

            if (center.sessions.Length > 0)
            {
                card.Body.Add(CreateColumnSet("Date", "Min Age", "Vaccine", "Available", true));
                foreach (var session in center.sessions)
                {
                    card.Body.Add(CreateSessionColumnSet(session));
                }
            }

            return card;
        }

        private AdaptiveColumnSet CreateSessionColumnSet(Session session)
            => CreateColumnSet(session.date, session.min_age_limit.ToString(), session.vaccine, session.available_capacity.ToString());

        private AdaptiveColumnSet CreateColumnSet(string value1, string value2, string value3, string value4, bool isBold = false)
        {
            return new AdaptiveColumnSet
            {
                Separator = true,
                Columns = new List<AdaptiveColumn>
                {
                    CreateTextColumn(value1, isBold),
                    CreateTextColumn(value2, isBold),
                    CreateTextColumn(value3, isBold),
                    CreateTextColumn(value4, true), // Last will always bold
                }
            };
        }

        private AdaptiveColumn CreateTextColumn(string text, bool isBold)
        {
            return new AdaptiveColumn
            {
                Width = "stretch",
                Items = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock(text)
                    {
                        Wrap=true,
                        Weight= isBold ? AdaptiveTextWeight.Bolder : AdaptiveTextWeight.Default
                    }
                }
            };
        }

        #endregion
    }
}
