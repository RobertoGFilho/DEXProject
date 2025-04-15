using System;
using System.Collections.Generic;

namespace DexApi.Models
{
    public class DexMeter
    {
        public char Machine { get; set; }
        public DateTime DEXDateTime { get; set; }
        public string MachineSerialNumber { get; set; }
        public decimal ValueOfPaidVends { get; set; }
    }
}