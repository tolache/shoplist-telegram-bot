using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace ShopListBot
{
    public static class SpreadsheetConnector
    {
        private const string ApplicationName = "ShopListBot";
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string SpreadsheetId;
        private static readonly CellRange ItemsLocation = new CellRange("items", "A2", "B");
        private static readonly CellRange UsersLocation = new CellRange("users", "A2", "A");
        private static SheetsService _sheetsService;

        static SpreadsheetConnector()
        {
            SpreadsheetId = SecretsLoader.GetSecret(SecretsLoader.SecretType.GoogleSpreadsheetId);
            ConnectToGoogle();
        }

        public static IList<IList<object>> ReadItems()
        {
            return ReadRange(ItemsLocation);
        }
        
        
        public static IList<IList<object>> ReadUsers()
        {
            return ReadRange(UsersLocation);
        }

        private static IList<IList<object>> ReadRange(CellRange range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(SpreadsheetId, range.ToString());
            
            ValueRange response = request.Execute();
            if (response == null)
            {
                throw new ArgumentException($"Couldn't get any data from range '{range}'.");
            }
            return response.Values;
        }

        private static void ConnectToGoogle()
        {
            string credentialString = SecretsLoader.GetSecret(SecretsLoader.SecretType.GoogleCredentials);
            GoogleCredential credential = GoogleCredential.FromJson(credentialString).CreateScoped(Scopes);

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
    }
}