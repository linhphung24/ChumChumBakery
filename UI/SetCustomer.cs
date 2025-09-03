using CoffeeManagement.BLL;
using CoffeeManagement.DAL;
using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeManagement.UI
{
    public partial class SetCustomer : Form
    {
        private int _id;
        public SetCustomer(int idBill)
        {
            InitializeComponent();
            _id = idBill;
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetCustomer_Load(object sender, EventArgs e)
        {
            LoadDataSource();
        }
        private void LoadDataSource()
        {
            dgvCustomer.DataSource = CustomerService.Instance.GetAllCustomers();

            dgvCustomer.Columns["IdCustomer"].HeaderText = "ID";
            dgvCustomer.Columns["CUSTOMER_NAME"].HeaderText = "Tên khách hàng";
            //dgvAccount.Columns["STREET"].HeaderText = "Địa chỉ";
            //dgvAccount.Columns["MOBIPHONE"].HeaderText = "Số điện thoại";
            //dgvAccount.Columns["BIRTHDAY"].HeaderText = "Ngày sinh";
            //dgvAccount.Columns["BIRTHDAY"].DefaultCellStyle.Format = "dd/MM/yyyy";
            //dgvAccount.Columns["DISCOUNT"].HeaderText = "Giảm giá";
            //dgvAccount.Columns["USER_ID"].HeaderText = "Người tạo";
            //dgvAccount.Columns["NOTE"].HeaderText = "Ghi chú";

            //dtpDateOfBirth.Format = DateTimePickerFormat.Custom;
            //dtpDateOfBirth.CustomFormat = "dd/MM/yyyy";

            dgvCustomer.Columns[0].FillWeight = 10;
            dgvCustomer.Columns[1].FillWeight = 90;
        }

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ClearInput();
            if (dgvCustomer.SelectedRows.Count == 1)
            {
                var selectedAccount = dgvCustomer.CurrentRow.DataBoundItem as CustomerDTO;

                var cusDtl = CustomerDAL.Instance.GetCustomerByID(selectedAccount.IdCustomer);

                txtDisplayName.Text = cusDtl.Rows[0]["CUSTOMER_NAME"].ToString();
                txtStreet.Text = cusDtl.Rows[0]["STREET"].ToString();
                txtMobiphone.Text = cusDtl.Rows[0]["MOBIPHONE"].ToString();
                txtNote.Text = cusDtl.Rows[0]["NOTE"].ToString();
                dtpDateOfBirth.Value = (DateTime)cusDtl.Rows[0]["BIRTHDAY"] != null ? (DateTime)cusDtl.Rows[0]["BIRTHDAY"] : DateTime.Now;
                txtId.Text = cusDtl.Rows[0]["IdCustomer"].ToString();
            }
        }
        private void ClearInput()
        {
            txtDisplayName.Clear();
            txtStreet.Clear();
            dtpDateOfBirth.Value = DateTime.Now;
            txtMobiphone.Clear();
            txtNote.Clear();
            txtEmail.Clear();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtId.Text))
            {
                BillDAL.Instance.UpdateCustomerForBill(_id, Convert.ToInt32(txtId.Text));
                this.Close();
            } else
            {
                MessageBox.Show("Vui lòng chọn khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dgvCustomer.DataSource = CustomerDAL.Instance.SearchCustomer(txtSearch.Text);
            if (dgvCustomer.RowCount == 0)
                MessageBox.Show("Không tìm thấy kết quả phù hợp với từ khoá '" + txtSearch.Text + "' này", "Tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
