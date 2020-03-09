using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.Models
{
    public class Order
    {
        public DateTime Date { get; set; }
        public int Number { get; set; }
        public string Company { get; set; }
        public double Amount { get; set; }
        public bool Paid { get; set; }
    }
}
