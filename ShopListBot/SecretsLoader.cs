using System;
using System.IO;
using System.Linq;
using Amazon.Lambda.Core;

namespace ShopListBot
{
    public class SecretsLoader
    {
        private const string TelegramBotTokenFile = @"Secrets/telegramBotToken.txt";
        private const string SpreadsheetIdFile = @"Secrets/spreadsheetId.txt";
        private const string GoogleCredentialsFile = @"Secrets/googleSheetsCredentials.json";
        
        public string GetSecret(SecretType secretType)
        {
            string secret;
            string secretFile = secretType switch
            {
                SecretType.TelegramBotToken => TelegramBotTokenFile,
                SecretType.GoogleSpreadsheetId => SpreadsheetIdFile,
                SecretType.GoogleCredentials => GoogleCredentialsFile,
                _ => throw new ArgumentException($"Unknown secret type {secretType}")
            };
            
            try
            {
                secret = File.ReadAllText(secretFile);
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
        GoogleCredentials,
    }
}