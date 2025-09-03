using CoffeeManagement.DAL;
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
    public partial class NoteBill : Form
    {
        private int _idBillInfo;
        public NoteBill(int idBillInfo, string text)
        {
            InitializeComponent();
            _idBillInfo = idBillInfo;
            txtNote.Text = text;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            BillDAL.Instance.UpdateNoteBill(_idBillInfo, txtNote.Text);
            this.Close();
        }
    }
}
