using CoffeeManagement.BLL;
using CoffeeManagement.DAL;
using CoffeeManagement.DTO;
using CoffeeManagement.management;
using CoffeeManagement.model;
using CoffeeManagement.UI;
using CoffeeManagement.utils;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CoffeeManagement.UI
{
    public partial class EditDiscount : Form
    {
        private int _id;
        public EditDiscount(int id, int discount)
        {
            InitializeComponent();
            nudDiscount.Value = discount;
            _id = id;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            BillInfoDAL.Instance.UpdateDiscountBillInfo(_id, Convert.ToInt32(nudDiscount.Value));
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
