using System;
using System.Collections.Generic;

namespace ShopListBot
{
    public class ShoppingList
    {
        public IList<ShopListItem> Items => GetItems();

        private readonly SpreadsheetConnector _spreadsheetConnector = new SpreadsheetConnector();

        private IList<ShopListItem> GetItems()
        {
            IList<ShopListItem> items = new List<ShopListItem>();

            IList<IList<object>> itemValues = _spreadsheetConnector.ReadItems();

            foreach (IList<object> row in itemValues)
            {
                bool idIsValid = Int64.TryParse(row[0].ToString(), out long id);
                if (idIsValid && !String.IsNullOrWhiteSpace(row[1].ToString()))
                {
                    items.Add(new ShopListItem(id, row[1].ToString()));
                }
            }

            return items;
        }
    }
}