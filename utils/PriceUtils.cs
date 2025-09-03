using CoffeeManagement.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeManagement.utils
{
    public class PriceUtils
    {
        public static decimal ToPriceNumber(string priceStr)
        {
            //MessageBox.Show(priceStr);
            //return Convert.ToDecimal(priceStr.Split(',')[0].Replace(".", ""));
            if (string.IsNullOrWhiteSpace(priceStr))
                return 0;

            try
            {
                // Bỏ ký hiệu tiền và dấu cách
                priceStr = priceStr.Replace("₫", "").Trim();

                // Bỏ phần thập phân sau dấu ','
                if (priceStr.Contains(","))
                    priceStr = priceStr.Substring(0, priceStr.IndexOf(","));

                // Bỏ dấu chấm phân cách hàng nghìn
                priceStr = priceStr.Replace(".", "");

                // Parse thành số
                return decimal.Parse(priceStr);
            }
            catch
            {
                return 0;
            }
        }

        public static string ToPriceString(decimal priceNumber) {
            CultureInfo culture = new CultureInfo("vi-VN");
            return priceNumber.ToString("c", culture);
        }

        public static string ToPercentString(decimal percentNumber)
        {
            return percentNumber.ToString() + "%";
        }
    }
}
