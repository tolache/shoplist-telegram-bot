namespace ShopListBot
{
    public class CellRange
    {
        public string Sheet => _sheet;
        public string FromCell => _fromCell;
        public string ToCell => _toCell;

        private string _sheet;
        private string _fromCell;
        private string _toCell;

        public CellRange(string sheet, string fromCell, string toCell)
        {
            _sheet = sheet;
            _fromCell = fromCell;
            _toCell = toCell;
        }
        
        public override string ToString()
        {
            return $"{_sheet}!{_fromCell}:{_toCell}";
        }
    }
}