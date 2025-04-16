namespace DexApi.Models
{
    public class DexMeter
    {
        public char Machine { get; set; }
        public DateTime DEXDateTime { get; set; }
        public string MachineSerialNumber { get; set; } = string.Empty;
        public decimal ValueOfPaidVends { get; set; }
    }
}