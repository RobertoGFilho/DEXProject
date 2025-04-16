namespace DexApi.Models
{
    public class DexLaneMeter
    {
        public string ProductIdentifier { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int NumberOfVends { get; set; }
        public decimal ValueOfPaidSales { get; set; }
    }
}