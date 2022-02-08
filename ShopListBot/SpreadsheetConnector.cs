using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;

namespace ShopListBot
{
    public static class SpreadsheetConnector
    {
        private const string ApplicationName = "ShopListBot";
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string SpreadsheetId;
        private static readonly CellRange ItemsLocation;
        private static readonly CellRange UsersLocation;
        private static SheetsService _sheetsService;

        static SpreadsheetConnector()
        {
            SpreadsheetId = SecretsLoader.GetSecret(SecretsLoader.SecretType.GoogleSpreadsheetId);
            ItemsLocation = new CellRange("items", new Cell("A",2), new Cell("B"));
            UsersLocation = new CellRange("users", new Cell("A",2), new Cell("A"));
            InitSheetsService();
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

        public static string UpdateItems(IList<IList<string>> items)
        {
            return WriteRange(items, ItemsLocation);
        }
        
        private static string WriteRange(IList<IList<string>> data, CellRange range)
        {
            IList<IList<object>> objectsData = new List<IList<object>>();
            foreach (IList<string> row in data)
            {
                objectsData.Add(row.Cast<object>().ToList());
            }

            string valueInputOption = "USER_ENTERED";
            
            List<ValueRange> updateData = new List<ValueRange>();
            ValueRange dataValueRange = new ValueRange
            {
                Range = range.ToString(),
                Values = objectsData
            };
            updateData.Add(dataValueRange);

            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = valueInputOption,
                Data = updateData
            };

            SpreadsheetsResource.ValuesResource.BatchUpdateRequest? request =
                _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, SpreadsheetId);

            BatchUpdateValuesResponse response = request.Execute();

            return "Response JSON: " + Environment.NewLine + JsonConvert.SerializeObject(response);
        }

        private static void InitSheetsService()
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