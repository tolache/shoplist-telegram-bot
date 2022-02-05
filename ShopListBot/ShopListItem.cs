using System;

namespace ShopListBot
{
    public class ShopListItem
    {
        public long Id { get; }
        public string Name { get; }

        public ShopListItem(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}