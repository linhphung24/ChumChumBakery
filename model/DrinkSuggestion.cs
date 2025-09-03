using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.model
{
    public class DrinkSuggestion
    {
        public int IdDrink { get; set; }
        public string NameDrink { get; set; }
        public int IdCategory { get; set; }

        public override string ToString()
        {
            return NameDrink; // để hiển thị trong ListBox
        }
    }
}
