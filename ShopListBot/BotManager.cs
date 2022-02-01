using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ShopListBot
{
    public class BotManager
    {
        private readonly TelegramBotClient _botClient;

        public BotManager()
        {
            string token = new SecretsLoader().GetTelegramBotToken();
            _botClient = new TelegramBotClient(token);
        }

        public async Task RespondAsync(Update update)
        {
            if (update == null)
            {
                return;
            }

            if (update.Type != UpdateType.Message)
            {
                return;
            }

            Message message = update.Message;
            User user = message.From;
            string replyText = $"I received: {message.Text} from @{user.Username} (id:{user.Id})";
            
            IList<IList<string>> spreadsheetData = new List<IList<string>>();
            try
            {
                spreadsheetData = new SpreadsheetConnector().GetData();
            }
            catch (Exception e)
            {
                replyText = "Failed to get data from Google spreadsheet: " + e;
                spreadsheetData = new List<IList<string>> { new List<string> { "", "" } };
            }

            Console.WriteLine($"Received message from {message.Chat.Id}");

            IEnumerable<IEnumerable<KeyboardButton>> buttonBoard = new List<IEnumerable<KeyboardButton>>
            {
                new List<KeyboardButton> {new KeyboardButton(spreadsheetData[0][0]), new KeyboardButton(spreadsheetData[0][1])},
            };
            ReplyKeyboardMarkup keyboardMarkup = new ReplyKeyboardMarkup(buttonBoard);

            switch (message.Type)
            {
                case MessageType.Text: ;
                    await _botClient.SendTextMessageAsync(
                        chatId:message.Chat.Id,
                        text: replyText,
                        replyMarkup: keyboardMarkup);
                    break;
            }
        }
    }
}