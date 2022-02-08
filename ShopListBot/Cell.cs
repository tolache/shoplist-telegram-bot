namespace ShopListBot
{
    public class Cell
    {
        public string Column { get; set; }
        public int? Row { get; set; }

        public Cell(string column, int row)
        {
            Column = column;
            Row = row;
        }

        public Cell(string column)
        {
            Column = column;
            Row = null;
        }

        public override string ToString()
        {
            string result = $"{Column}";
            if (Row != null) result += Row;

            return result;
        }
    }
}