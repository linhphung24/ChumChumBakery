using CoffeeManagement.BLL;
using CoffeeManagement.DAL;
using CoffeeManagement.DTO;
using Guna.UI2.WinForms;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace CoffeeManagement.UI
{
    public partial class Customer : Form
    {
        #region Properties
        private bool isAdd = false;
        #endregion
        public Customer()
        {
            InitializeComponent();
        }
        #region Event handler properties
        private event EventHandler insertCustomer;
        public event EventHandler InsertCustomer
        {
            add { insertCustomer += value; }
            remove { insertCustomer -= value; }
        }

        private event EventHandler updateCustomer;
        public event EventHandler UpdateCustomer
        {
            add { updateCustomer += value; }
            remove { updateCustomer -= value; }
        }

        private event EventHandler deleteCustomer;
        public event EventHandler DeleteCustomer
        {
            add { deleteCustomer += value; }
            remove { deleteCustomer -= value; }
        }
        #endregion
        #region Common function
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
        private void ClearInput()
        {
            txtDisplayName.Clear();
            txtStreet.Clear();
            dtpDateOfBirth.Value = DateTime.Now;
            txtMobiphone.Clear();
            txtNote.Clear();
            txtEmail.Clear();
        }
        #endregion
        #region Processing
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearInput();
            btnSave.Enabled = true;
            btnEdit.Enabled = true;
            isAdd = true;
            btnAdd.Enabled = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            btnAdd.Enabled = true;
            isAdd = false;
            btnEdit.Enabled = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (CustomerDAL.Instance.DeleteCustomer(Convert.ToInt32(txtId.Text)))
            {
                MessageBox.Show("Xoá thành công", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Question);
                LoadDataSource();
                if (deleteCustomer != null)
                    deleteCustomer(this, new EventArgs());
            }
            else MessageBox.Show("Vui lòng chọn khách hàng cần xoá", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            HandleCreateOrEdit();
        }
        private void HandleCreateOrEdit()
        {
            if (isAdd)
            {
                HandleAdd();
                btnSave.Enabled = false;
                btnAdd.Enabled = true;
                ClearInput();
            }
            else
            {
                HandleEdit();
                btnSave.Enabled = false;
                btnEdit.Enabled = true;
                ClearInput();
            }
        }
        private void HandleAdd()
        {
            if (CustomerService.Instance.InsertCustomer(txtDisplayName.Text, txtStreet.Text, dtpDateOfBirth.Value, txtEmail.Text, txtMobiphone.Text, txtNote.Text))
            {
                MessageBox.Show("Thêm thành công", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Question);
                LoadDataSource();
                if (insertCustomer != null)
                    insertCustomer(this, new EventArgs());
            }
            else MessageBox.Show("Thêm thất bại", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void HandleEdit()
        {
            if (string.IsNullOrEmpty(txtDisplayName.Text))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần cập nhật", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            if (CustomerService.Instance.EditCustomer(Convert.ToInt32(txtId.Text), txtDisplayName.Text, txtStreet.Text, dtpDateOfBirth.Value, txtEmail.Text, txtMobiphone.Text, txtNote.Text))
            {
                MessageBox.Show("Cập nhật thành công", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Question);
                LoadDataSource();
                if (updateCustomer != null)
                    updateCustomer(this, new EventArgs());
            }
            else MessageBox.Show("Cập nhật thất bại", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }
        private void Customer_Load(object sender, EventArgs e)
        {
            LoadDataSource();
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dgvCustomer.DataSource = CustomerDAL.Instance.SearchCustomer(txtSearch.Text);
            if (dgvCustomer.RowCount == 0)
                MessageBox.Show("Không tìm thấy kết quả phù hợp với từ khoá '" + txtSearch.Text + "' này", "Tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
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
        #endregion

        private void dgvCustomer_DataSourceChanged(object sender, EventArgs e)
        {
            lblStatus.Text = dgvCustomer.RowCount + " khách hàng";
        }
    }
}
