using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DTO
{
    public class ProductDTO
    {
        public int IdProduct { get; set; }
        public int IdDrink { get; set; }
        public int Quantity { get; set; }
        // ngay san xuat
        public DateTime DateFrom { get; set; }
        // han su dung
        public DateTime DateTo { get; set; }
        public DateTime CreatedOn { get; set; }
                
    }
}
