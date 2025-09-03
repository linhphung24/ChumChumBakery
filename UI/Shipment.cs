using CoffeeManagement.DAL;
using iText.StyledXmlParser.Jsoup.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeManagement.UI
{
    public partial class Shipment : Form
    {
        private int _idBillInfo;
        public Shipment(int idBillInfo, decimal feeShip)
        {
            InitializeComponent();
            _idBillInfo = idBillInfo;
            nudFeeShip.Value = feeShip;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            BillDAL.Instance.UpdateFeeShipBill(_idBillInfo, nudFeeShip.Value);
            this.Close();
        }
    }
}
