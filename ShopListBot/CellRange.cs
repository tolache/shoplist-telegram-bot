namespace ShopListBot
{
    public class CellRange
    {
        public string Sheet { get; set; }
        public string FromCell { get; set; }
        public string ToCell { get; set; }

        public CellRange(string sheet, string fromCell, string toCell)
        {
            Sheet = sheet;
            FromCell = fromCell;
            ToCell = toCell;
        }
        
        public override string ToString()
        {
            return $"{Sheet}!{FromCell}:{ToCell}";
        }
    }
}