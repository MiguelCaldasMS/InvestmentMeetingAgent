using Microsoft.Agents.Core;
using Microsoft.Agents.Core.Models;
using Microsoft.Agents.Extensions.Teams;
using System.Text.Json.Nodes;

namespace InvestmentMeeting.WebApp
{
    public class MeetingAgent : TeamsActivityHandler
    {
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Verify the member added is not the agent itself
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await SendAdaptiveCardAsync(turnContext, cancellationToken);
                }
            }
        }

        private async Task SendAdaptiveCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var cardJson = @"
            {
                ""type"": ""AdaptiveCard"",
                ""body"": [
                    { ""type"": ""TextBlock"", ""text"": ""Welcome to the Investment Meeting! Please select an option below:"", ""size"": ""Large"", ""weight"": ""Bolder"" }
                ],
                ""actions"": [
                    { ""type"": ""Action.Submit"", ""title"": ""View Portfolio"", ""data"": { ""choice"": ""portfolio"" } },
                    { ""type"": ""Action.Submit"", ""title"": ""Market Trends"", ""data"": { ""choice"": ""trends"" } },
                    { ""type"": ""Action.Submit"", ""title"": ""Risk Analysis"", ""data"": { ""choice"": ""risk"" } }
                ],
                ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
                ""version"": ""1.5""
            }";

            // Parse the JSON string to a JsonNode
            var cardContent = JsonNode.Parse(cardJson);

            var responseApi = MessageFactory.Attachment(new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = cardContent
            });

            await turnContext.SendActivityAsync(responseApi, cancellationToken);
        }
    }
}