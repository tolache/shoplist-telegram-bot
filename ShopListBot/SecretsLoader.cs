using System;
using System.IO;
using System.Linq;

namespace ShopListBot
{
    public class SecretsLoader
    {
        private const string TelegramBotTokenFile = @"Secrets/telegramBotToken.txt";
        private const string SpreadsheetIdFileName = @"Secrets/spreadsheetId.txt";
        
        public string GetTelegramBotToken()
        {
            string token;
            try
            {
                token = File.ReadLines(TelegramBotTokenFile).First();
            }
            catch (IOException e)
            {
                Console.WriteLine(
                    $"Unable to load Telegram bot token from file {TelegramBotTokenFile}. Caused by: {e.Message}");
                throw;
            }
            
            if (String.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(
                    $"Token is invalid. Token must be set through the '{TelegramBotTokenFile}' environment variable.",
                    token);
            }
            return token;
        }
        
        public string GetSpreadsheetId()
        {
            string spreadsheetId;
            try
            {
                spreadsheetId = File.ReadLines(SpreadsheetIdFileName).First();
            }
            catch (IOException e)
            {
                Console.WriteLine($"Unable to load Spreadsheet ID from file {SpreadsheetIdFileName}. Caused by: {e.Message}");
                throw;
            }

            if (String.IsNullOrWhiteSpace(spreadsheetId))
            {
                throw new ArgumentException($"Spreadsheet ID specified in file {SpreadsheetIdFileName} is invalid.",
                    spreadsheetId);
            }
            return spreadsheetId;
        }
    }
}