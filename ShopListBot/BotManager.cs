using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ShopListBot
{
    public class BotManager
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILogger<BotManager> _logger;

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

            IList<IList<string>> spreadsheetData = new List<IList<string>>();
            try
            {
                spreadsheetData = new SpreadsheetConnector().GetData();
            }
            catch (Exception e)
            {
                spreadsheetData[0][0] = e.ToString();
            }

            _logger?.LogInformation("Received message from {ChatId}", message.Chat.Id);

            IEnumerable<IEnumerable<KeyboardButton>> buttonBoard = new List<IEnumerable<KeyboardButton>>
            {
                new List<KeyboardButton> {new KeyboardButton(spreadsheetData[0][0]), new KeyboardButton(spreadsheetData[0][0])},
            };
            ReplyKeyboardMarkup keyboardMarkup = new ReplyKeyboardMarkup(buttonBoard);

            switch (message.Type)
            {
                case MessageType.Text: ;
                    await _botClient.SendTextMessageAsync(
                        chatId:message.Chat.Id,
                        text: $"I received: {message.Text} from @{user.Username} (id:{user.Id})" + Environment.NewLine + 
                              $"Spreadsheet data: {spreadsheetData}",
                        replyMarkup: keyboardMarkup);
                    break;
            }
        }
    }
}