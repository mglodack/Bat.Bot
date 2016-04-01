using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace Bat.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                var utterance = message.Text ?? string.Empty;

                var luisObj = await Luis.Ask(utterance);
                // return our reply to the user
                return message.CreateReplyMessage(JsonConvert.SerializeObject(luisObj, Formatting.Indented));
            }

            return message.CreateReplyMessage("Unsupported message!");
        }
    }
}