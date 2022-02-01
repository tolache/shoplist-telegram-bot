using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace ShopListBot
{
    public class SpreadsheetConnector
    {
        private readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string _applicationName = "ShopListBot";
        private readonly string _spreadsheetId;
        private SheetsService _sheetsService;

        public SpreadsheetConnector()
        {
            _spreadsheetId = new SecretsLoader().GetSpreadsheetId();
            ConnectToGoogle();
        }

        public IList<IList<string>> GetData()
        {
            string range = "items!A2:B";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);
            
            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                List<IList<string>> result = new List<IList<string>>();

                foreach (var row in values)
                {
                    result.Add(new List<string> {row[0].ToString(), row[1].ToString()});
                }

                return result;
            }

            throw new ArgumentException($"Couldn't get any data from range {range}", range);
        }

        private void ConnectToGoogle()
        {
            GoogleCredential credential;

            using (var stream = new FileStream(@"Secrets/googleSheetsCredentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
            }

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName,
            });
        }
    }
}