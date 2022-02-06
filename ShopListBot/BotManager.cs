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

        private IList<string> AuthorizedUsers => GetAuthorizedUsers();

        public BotManager()
        {
            string token = SecretsLoader.GetSecret(SecretsLoader.SecretType.TelegramBotToken);
            _botClient = new TelegramBotClient(token);
        }

        public async Task RespondAsync(Update update)
        {
            if (update.Type != UpdateType.Message) return;
            
            Message? message = update.Message;
            User? fromUser = message?.From;
            if (message == null || fromUser == null) return;

            LambdaLogger.Log($"Received message from {message.Chat.Id}");
            if (!CheckUserAuthorized(fromUser.Username)) return;

            string replyText = $"I received: {message.Text} from @{fromUser.Username} (id:{fromUser.Id})";

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

        private bool CheckUserAuthorized(string? username)
        {
            if (!String.IsNullOrWhiteSpace(username) && AuthorizedUsers.Contains(username)) return true;
            LambdaLogger.Log($"User '@{username}' is unauthorized");
            return false;
        }

        private IList<string> GetAuthorizedUsers()
        {
            IList<string> authorizedUsers = new List<string>();
            IList<IList<object>> usersValues = SpreadsheetConnector.ReadUsers();

            foreach (IList<object> row in usersValues)
            {
                string? user = row[0].ToString();
                if (!String.IsNullOrWhiteSpace(user))
                {
                    authorizedUsers.Add(user);
                }
            }

            return authorizedUsers;
        }
    }
}