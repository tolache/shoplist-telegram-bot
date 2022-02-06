using System;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ShopListBot
{
    public class Function
    {
        private static readonly AmazonLambdaClient _lambdaClient;
        private static readonly BotManager _botManager;
        
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        static Function()
        {
            _lambdaClient = new AmazonLambdaClient();
            _botManager = new BotManager();
        }

        public async Task<string> FunctionHandler(JObject request, ILambdaContext context)
        {
            LambdaLogger.Log("REQUEST: " + JsonConvert.SerializeObject(request));
            Update? update = request.ToObject<Update>();
            try
            {
                if (update != null)
                {
                    await _botManager.RespondAsync(update);
                }
            }
            catch (Exception e)
            {
                LambdaLogger.Log("exception: " + e.Message);
            }

            return "Hello from AWS Lambda " + DateTimeOffset.Now;
        }
    }
}
