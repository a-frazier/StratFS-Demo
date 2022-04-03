namespace Demo1.Models
{
    internal class Tradeline
    {
        public long TradelineId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal MonthlyPayment { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsMortgage { get; set; }
    }
}
