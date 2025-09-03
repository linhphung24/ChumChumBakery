using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.model
{
    public class Bank
    {
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public int AcqId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        //public string LogoPath { get; set; }

        public static List<Bank> LoadExistingBanks()
        {
            List<Bank> banks = new List<Bank>();

            banks.Add(new Bank
            {
                AccountNo = "MBFDD6154B1CFB64VCB",
                AccountName = "BUI PHUONG LAN",
                AcqId = 970436,
                Code = "VCB",
                Name = "Ngân hàng TMCP Ngoại thương Việt Nam"
                //LogoPath = @"images/banks/VCB.png"
            });

            return banks;
        }
    }
}
