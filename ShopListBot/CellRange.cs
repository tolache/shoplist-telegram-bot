namespace ShopListBot
{
    public class CellRange
    {
        public string Sheet { get; set; }
        public Cell FromCell { get; set; }
        public Cell ToCell { get; set; }

        public CellRange(string sheet, Cell fromCell, Cell toCell)
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