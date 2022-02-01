using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ShopListBot
{
    public class SecretsLoader
    {
        private const string TelegramBotTokenFile = @"Secrets/telegramBotToken.txt";
        private const string SpreadsheetIdFileName = @"Secrets/spreadsheetId.txt";

        private readonly ILogger<SecretsLoader> _logger;

        public string GetTelegramBotToken()
        {
            string token;
            try
            {
                token = File.ReadLines(TelegramBotTokenFile).First();
            }
            catch (IOException e)
            {
                _logger.LogError("Unable to load Telegram bot token from file {File}", TelegramBotTokenFile);
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
                _logger.LogError("Unable to load Spreadsheet ID from file {File}", SpreadsheetIdFileName);
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