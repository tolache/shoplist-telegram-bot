using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ShopListBot
{
    public class BotManager
    {
        private readonly TelegramBotClient _botClient;
        private ShoppingList _shopList = new ShoppingList();

        public BotManager()
        {
            string token = SecretsLoader.GetSecret(SecretsLoader.SecretType.TelegramBotToken);
            _botClient = new TelegramBotClient(token);
        }

        public async Task RespondAsync(Update update)
        {
            if (update == null) return;
            if (update.Type != UpdateType.Message) return;

            Message message = update.Message;
            User user = message.From;
            LambdaLogger.Log($"Received message from {message.Chat.Id}");

            string replyText = $"I received: {message.Text} from @{user.Username} (id:{user.Id})";

            IList<ShopListItem> items = new List<ShopListItem>();
            try
            {
                items = _shopList.Items;
            }
            catch (Exception e)
            {
                string errorMessage = "Failed to get items data from Google spreadsheet: " + e;
                LambdaLogger.Log(errorMessage);
                replyText += Environment.NewLine + errorMessage;
            }

            ReplyKeyboardMarkup keyboardMarkup = default;
            if (items.Count > 0)
            {
                IList<IList<KeyboardButton>> buttonBoard = new List<IList<KeyboardButton>>();
                foreach (ShopListItem item in items)
                {
                    buttonBoard.Add(new List<KeyboardButton>
                        { new KeyboardButton(item.Id.ToString()), new KeyboardButton(item.Name) });
                }
                keyboardMarkup = new ReplyKeyboardMarkup(buttonBoard);
            }

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