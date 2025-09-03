using CoffeeManagement.BLL;
using CoffeeManagement.DAL;
using CoffeeManagement.model;
using Guna.UI2.WinForms;
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
    public partial class Product : Form
    {
        #region System
        public Product()
        {
            InitializeComponent();
        }
        #endregion
        #region Common function
        private void LoadDataSource()
        {
            dgvProduct.DataSource = ProductDAL.Instance.GetTable();
            dgvProduct.Columns[0].HeaderText = "ID";
            dgvProduct.Columns[1].HeaderText = "Tên mặt hàng";
            dgvProduct.Columns[2].HeaderText = "Giá";
            dgvProduct.Columns[3].HeaderText = "Ngày sản xuất";
            dgvProduct.Columns[4].HeaderText = "Hạn sử dụng";
            dgvProduct.Columns[5].HeaderText = "Ngày tạo";

            dgvProduct.DefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            dgvProduct.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            // Tạo cột nút
            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
            btnDelete.HeaderText = "";
            btnDelete.Text = "Xóa";
            btnDelete.Name = "btnDelete";
            btnDelete.UseColumnTextForButtonValue = true; // Hiển thị chữ "Xóa" trong nút

            // Thêm vào Guna2DataGridView
            dgvProduct.Columns.Add(btnDelete);

            dgvProduct.Columns[0].FillWeight = 10;
            dgvProduct.Columns[1].FillWeight = 25;
            dgvProduct.Columns[2].FillWeight = 15;
            dgvProduct.Columns[3].FillWeight = 15;
            dgvProduct.Columns[4].FillWeight = 15;
            dgvProduct.Columns[5].FillWeight = 15;
            dgvProduct.Columns[6].FillWeight = 5;

            // set datetime now
            dtpFromDate.Value = DateTime.Now;
            dtpToDate.Value = DateTime.Now;
            dtpNSX.Value = DateTime.Now;
            dtpNSX.MinDate = DateTime.Today;
            dtpHSD.Value = DateTime.Now;
            dtpHSD.MinDate = DateTime.Today;
        }
        #endregion

        private void lblExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Product_Load(object sender, EventArgs e)
        {
            LoadDataSource();
        }

        private void btnStatistical_Click(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                lstSuggestions.Visible = false;
                return;
            }

            DataTable dt = BillInfoDAL.Instance.SP_GetListItemBySearch(keyword);

            if (dt != null && dt.Rows.Count > 0)
            {
                List<DrinkSuggestion> suggestions = new List<DrinkSuggestion>();

                foreach (DataRow row in dt.Rows)
                {
                    suggestions.Add(new DrinkSuggestion
                    {
                        IdDrink = Convert.ToInt32(row["IdDrink"]),
                        NameDrink = row["NameDrink"].ToString(),
                        IdCategory = Convert.ToInt32(row["IdCategory"])
                    });
                }

                lstSuggestions.DataSource = suggestions;
                lstSuggestions.Visible = true;
                lstSuggestions.BringToFront();
                lstSuggestions.Top = txtSearch.Bottom;
                lstSuggestions.Left = txtSearch.Left;
            }
            else
            {
                lstSuggestions.Visible = false;
            }
        }

        private void lstSuggestions_Click(object sender, EventArgs e)
        {
            if (lstSuggestions.SelectedItem != null)
            {
                var selectedItem = (DrinkSuggestion)lstSuggestions.SelectedItem;

                txtSearch.Text = lstSuggestions.SelectedItem.ToString();
                txtName.Text = selectedItem.NameDrink.ToString();
                txtID.Text = selectedItem.IdDrink.ToString();
                cboCategory.SelectedValue = selectedItem.IdCategory;
                lstSuggestions.Visible = false;
            }
        }

        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex >= 0 && dgvProduct.Columns[e.ColumnIndex].Name == "btnDelete")
            //{
            //    // Hỏi xác nhận (tùy chọn)
            //    var confirm = MessageBox.Show("Bạn có chắc muốn xóa dòng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //    if (confirm == DialogResult.Yes)
            //    {
            //        dgvProduct.Rows.RemoveAt(e.RowIndex); // Xóa dòng
            //    }
            //}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Hỏi xác nhận (tùy chọn)
            var confirm = MessageBox.Show("Bạn có chắc muốn nhập thêm mặt hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                ProductService.Instance.InsertProduct(Convert.ToInt32(txtID.Text), 
                    Convert.ToInt32(nudQuantity.Value), 
                    dtpNSX.Value, 
                    dtpHSD.Value);
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (e.Equals(null))
            {
                btnSave.Enabled = false;
            } else
            {
                btnSave.Enabled = true;
            }
        }
    }
}
