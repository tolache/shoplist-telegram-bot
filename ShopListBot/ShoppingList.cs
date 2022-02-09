using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopListBot
{
    public class ShoppingList
    {
        public IList<string> Items => GetItems();

        public string AddItems(string message)
        {
            IList<string> newItems = ParseItems(message);
            IList<IList<string>> dataToSubmit = new List<IList<string>>();
            
            foreach (string existingItem in Items)
            {
                dataToSubmit.Add(new List<string> {existingItem});
            }
            
            foreach (string newItem in newItems)
            {
                dataToSubmit.Add(new List<string> {newItem});
            }
            
            return SpreadsheetConnector.UpdateItems(dataToSubmit);
        }

        private IList<string> GetItems()
        {
            IList<string> items = new List<string>();
            IList<IList<object>> itemValues = SpreadsheetConnector.ReadItems();

            // ReSharper disable once ConstantConditionalAccessQualifier
            if (itemValues?.Count > 0)
            {
                foreach (IList<object> row in itemValues)
                {
                    if (!String.IsNullOrWhiteSpace(row[0].ToString()))
                    {
                        items.Add(row[0].ToString());
                    }
                }
            }

            return items;
        }

        private IList<string> ParseItems(string message)
        {
            char[] delimiters = { ',', ';', '\n' };

            IList<string> items = message.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
            return items;
        }
    }
}