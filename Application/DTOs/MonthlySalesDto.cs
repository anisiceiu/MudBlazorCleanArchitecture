using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MonthlySalesDto
    {
        public string MonthName { get; set; }
        public int MonthNumber { get; set; }
        public double TotalSales { get; set; }
    }
}
