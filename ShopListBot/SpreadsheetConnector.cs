﻿using System;
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
        private readonly CellRange _itemsLocation = new CellRange("items", "A2", "B");
        private SheetsService _sheetsService;

        public SpreadsheetConnector()
        {
            _spreadsheetId = new SecretsLoader().GetSecret(SecretType.GoogleSpreadsheetId);
            ConnectToGoogle();
        }

        public IList<IList<object>> ReadItems()
        {
            
            IList<IList<object>> itemValues = ReadRange(_itemsLocation);
            if (itemValues == null)
            {
                throw new ArgumentException($"Couldn't get any data from range '{_itemsLocation}'.");
            }

            return itemValues;
        }

        private IList<IList<object>> ReadRange(CellRange range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range.ToString());
            
            ValueRange response = request.Execute();
            return response.Values;
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