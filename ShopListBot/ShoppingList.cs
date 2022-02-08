using System;
using System.Collections.Generic;

namespace ShopListBot
{
    public class ShoppingList
    {
        public IList<ShopListItem> Items => GetItems();

        private IList<ShopListItem> GetItems()
        {
            IList<ShopListItem> items = new List<ShopListItem>();
            IList<IList<object>> itemValues = SpreadsheetConnector.ReadItems();

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

        public string AddItem(ShopListItem newItem)
        {
            IList<IList<string>> dataToSubmit = new List<IList<string>>();
            
            foreach (ShopListItem existingItem in Items)
            {
                dataToSubmit.Add(new List<string> {existingItem.Id.ToString(), existingItem.Name});
            }
            
            dataToSubmit.Add(new List<string> { newItem.Id.ToString(), newItem.Name });
            
            return SpreadsheetConnector.UpdateItems(dataToSubmit);
        }
    }
}