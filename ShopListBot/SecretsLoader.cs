using System;
using System.IO;
using System.Linq;
using Amazon.Lambda.Core;

namespace ShopListBot
{
    public class SecretsLoader
    {
        private const string TelegramBotTokenFile = @"Secrets/telegramBotToken.txt";
        private const string SpreadsheetIdFileName = @"Secrets/spreadsheetId.txt";
        
        public string GetSecret(SecretType secretType)
        {
            string secret;
            string secretFile = secretType switch
            {
                SecretType.TelegramBotToken => TelegramBotTokenFile,
                SecretType.GoogleSpreadsheetId => SpreadsheetIdFileName,
                _ => throw new ArgumentException($"Unknown secret type {secretType}")
            };
            
            try
            {
                secret = File.ReadLines(secretFile).First();
            }
            catch (IOException e)
            {
                LambdaLogger.Log(
                    $"Unable to load secret from file {secretFile}. Caused by: {e.Message}");
                throw;
            }
            
            if (String.IsNullOrWhiteSpace(secret))
            {
                throw new ArgumentException(
                    $"Secret is invalid. Secret must be set through the '{secretFile}' file.",
                    secret);
            }
            return secret;
        }
    }

    public enum SecretType
    {
        TelegramBotToken,
        GoogleSpreadsheetId,
    }
}