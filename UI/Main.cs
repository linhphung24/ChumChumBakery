using CoffeeManagement.BLL;
using CoffeeManagement.DAL;
using CoffeeManagement.DTO;
using CoffeeManagement.management;
using CoffeeManagement.model;
using CoffeeManagement.UI;
using CoffeeManagement.utils;
using Guna.UI2.WinForms;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace CoffeeManagement
{
    public partial class Main : Form
    {
        #region Properties
        private AccountDTO LoginAccount;
        #endregion
        #region System
        public Main()
        {
            InitializeComponent();
        }

        public void PostLogin()
        {
            LoginAccount = Program.context.LoginAccount;
            tsmiManage.Enabled = (LoginAccount.Access == "Admin");
            lbUserName.Text = "Xin chào, " + LoginAccount.DisplayName;

            if(Program.context.LoginAccount.ForceResetPassword)
            {
                var frmReset = new Reset();
                frmReset.ShowDialog();
            }
        }
        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private void AnchorAllControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                // Bỏ qua MenuStrip nếu không muốn giãn
                if (control is MenuStrip) continue;

                control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                // Nếu control chứa con, xử lý tiếp
                if (control.HasChildren)
                {
                    AnchorAllControls(control);
                }
            }
        }
        private void Main_Load(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboBoxDrink();
            LoadComboBoxTable();
            lblMinimize.Font = new Font("Segoe MDL2 Assets", 10);
            lblMinimize.Text = "\uE921";
            txtBarcode.Focus();
            txtBarcode.Visible = false;
            this.ActiveControl = txtBarcode;
        }
        #endregion
        #region Common function
        public void LoadTable()
        {
            var tables = TableService.Instance.GetAllTables();

            fpnlTableList.Controls.Clear();

            tables.ForEach(t =>
            {
                var btn = BuildTableButton(t);
                fpnlTableList.Controls.Add(btn);
            });
        }

        private Guna2Button BuildTableButton(TableDTO table)
        {
            Guna2Button btn = new Guna2Button();
            btn.FillColor = Color.Transparent;
            btn.Font = new Font("Segoe UI", 9.75F);
            btn.ForeColor = Color.Black;
            btn.Image = (table.Status == "Trống") ? Properties.Resources.coffee_null : Properties.Resources.coffee_cup;
            btn.ImageOffset = new Point(0, -15);
            btn.ImageSize = new Size(60, 60);
            btn.Size = new Size(Content.tableWidth, Content.tableHeight);
            btn.Tag = table;
            btn.Text = table.Name + "\n" + table.Status;
            btn.TextAlign = HorizontalAlignment.Left;
            btn.TextOffset = new Point(0, 30);
            btn.Click += Btn_Click;

            return btn;
        }

        private void LoadComboBoxDrink()
        {
            cboCategory.DataSource = CategoryDAL.Instance.GetTable();
            cboCategory.DisplayMember = "NameCategory";
            cboCategory.ValueMember = "IdCategory";
            cboCategory_SelectedIndexChanged(null, null);
        }

        private void LoadComboBoxTable()
        {
            var tables = TableService.Instance.GetAllTables();
            cboSwitchTable.DataSource = tables;
            cboSwitchTable.DisplayMember = "Name";
            cboSwitchTable.ValueMember = "ID";

            cboMergeTable.DataSource = tables;
            cboMergeTable.DisplayMember = "Name";
            cboMergeTable.ValueMember = "ID";
        }

        public void ShowBillInfoOfTable(int id)
        {
            int idBill = BillService.Instance.GetUncheckBillId(id);
            dgvBillInfo.DataSource = BillInfoService.Instance.GetBillInfosById(idBill);

            dgvBillInfo.Columns["ID"].Visible = false;
            dgvBillInfo.Columns["IdTable"].Visible = false;
            dgvBillInfo.Columns["DrinkName"].HeaderText = "Tên món";
            dgvBillInfo.Columns["DrinkPrice"].HeaderText = "Đơn giá";
            dgvBillInfo.Columns["Quantity"].HeaderText = "Số lượng";
            dgvBillInfo.Columns["Discount"].HeaderText = "Giảm giá (%)";
            dgvBillInfo.Columns["TotalPrice"].HeaderText = "Thành tiền";
            dgvBillInfo.Columns["TotalPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvBillInfo.Columns["TotalPrice"].DefaultCellStyle.Padding = new Padding(0, 0, 55, 0);
            
            txtTotalPrice.Text = PriceUtils.ToPriceString(CalculateTotalPrice());
        }

        private decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;

            var billInfos = dgvBillInfo.DataSource as List<BillInfoDTO>;
            billInfos.ForEach(bi =>
            {
                totalPrice += bi.TotalPrice;
            });
            
            return totalPrice;
        }

        private void LoadInsEditDel_Drink()
        {
            int id;
            if (!Int32.TryParse(cboCategory.SelectedValue.ToString(), out id))
                return;
            cboDrink.DataSource = DrinkDAL.Instance.GetDrinkByIdCategory(id);
            if (dgvBillInfo.Tag != null)
                ShowBillInfoOfTable((dgvBillInfo.Tag as TableDTO).ID);
        }
        #endregion
        #region Menu strip
        private void tsmiBill_Click(object sender, EventArgs e)
        {
            Bill bill = new Bill();
            bill.ShowDialog();
        }

        private void tsmiTableDrink_Click(object sender, EventArgs e)
        {
            TableFood table = new TableFood();
            table.InsertTable += Table_InsertTable;
            table.UpdateTable += Table_UpdateTable;
            table.DeleteTable += Table_DeleteTable;
            table.ShowDialog();
        }

        private void tsmiCategory_Click(object sender, EventArgs e)
        {
            Category category = new Category();
            category.InsertCategory += Category_InsertCategory;
            category.UpdateCategory += Category_UpdateCategory;
            category.DeleteCategory += Category_DeleteCategory;
            category.ShowDialog();
        }

        private void tsmiDrink_Click(object sender, EventArgs e)
        {
            Drink drink = new Drink();
            drink.InsertDrink += Drink_InsertDrink;
            drink.UpdateDrink += Drink_UpdateDrink;
            drink.DeleteDrink += Drink_DeleteDrink;
            drink.ShowDialog();
        }

        private void tsmiAccount_Click(object sender, EventArgs e)
        {
            Account account = new Account(LoginAccount);
            account.ShowDialog();
        }

        private void tsmiAccountProfile_Click(object sender, EventArgs e)
        {
            AccountProfile profile = new AccountProfile(LoginAccount);
            profile.UpdateAccount += Profile_UpdateAccount;
            profile.ShowDialog();
        }

        private void tsmiAddNumDrink_Click(object sender, EventArgs e)
        {
            btnAddNumDrink_Click(this, new EventArgs());
        }

        private void tsmiSwitchTable_Click(object sender, EventArgs e)
        {
            btnSwitchTable_Click(this, new EventArgs());
        }

        private void tsmiMergeTable_Click(object sender, EventArgs e)
        {
            btnMergeTable_Click(this, new EventArgs());
        }

        private void tsmiPayment_Click(object sender, EventArgs e)
        {
            btnPayment_Click(this, new EventArgs());
        }
        #endregion
        #region Event Handler
        private void Profile_UpdateAccount(object sender, AccountEvent e)
        {
            LoginAccount = e.LoginAcc;
            tsmiAccountProfile.Text = (LoginAccount.DisplayName == "")
                                            ? "&Thông tin tài khoản (" + LoginAccount.Username + ")"
                                            : "&Thông tin tài khoản (" + LoginAccount.DisplayName + ")";
        }

        private void Table_InsertTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void Table_UpdateTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void Table_DeleteTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void Category_InsertCategory(object sender, EventArgs e)
        {
            LoadComboBoxDrink();
        }

        private void Category_UpdateCategory(object sender, EventArgs e)
        {
            LoadComboBoxDrink();
        }

        private void Category_DeleteCategory(object sender, EventArgs e)
        {
            LoadComboBoxDrink();
        }

        private void Drink_InsertDrink(object sender, EventArgs e)
        {
            LoadInsEditDel_Drink();
        }

        private void Drink_UpdateDrink(object sender, EventArgs e)
        {
            LoadInsEditDel_Drink();
        }

        private void Drink_DeleteDrink(object sender, EventArgs e)
        {
            LoadInsEditDel_Drink();
            LoadTable();
        }
        #endregion
        #region Processing
        private void Btn_Click(object sender, EventArgs e)
        {
            var table = (sender as Guna2Button).Tag as TableDTO;
            dgvBillInfo.Tag = (sender as Guna2Button).Tag;

            ShowBillInfoOfTable(table.ID);
            grbSelectedTable.Text = table.Name;
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            var table = dgvBillInfo.Tag as TableDTO;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn trước khi thanh toán", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idBill = BillService.Instance.GetUncheckBillId(table.ID);
            if (idBill != -1)
            {
                decimal discount = nudDiscount.Value;
                decimal totalPrice = PriceUtils.ToPriceNumber(txtTotalPrice.Text);
                decimal finalTotalPrice = totalPrice - (totalPrice / 100 * discount);

                var billInfos = dgvBillInfo.DataSource as List<BillInfoDTO>;
                var payload = new BillPayload(idBill, table.ID, totalPrice, discount, finalTotalPrice, billInfos);
                var chooseBankFrm = new ChooseBank(payload);
                chooseBankFrm.Show();
                // ITBI_U
            }
        }

        private void btnAddNumDrink_Click(object sender, EventArgs e)
        {
            var table = dgvBillInfo.Tag as TableDTO;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn trước khi thêm mặt hàng", "Quản lý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idBill = BillService.Instance.GetUncheckBillId(table.ID);
            int idDrink = Convert.ToInt32(cboDrink.SelectedValue);
            int amount = (int) nudNumDrink.Value;
            if (amount == 0) return;

            if (idBill == -1) // Bàn này chưa có bill
            {
                BillService.Instance.InsertBill(table.ID);
                BillInfoDAL.Instance.InsertBillInfo(BillDAL.Instance.GetMaxIDBill(), idDrink, amount);
            }
            else
                BillInfoDAL.Instance.InsertBillInfo(idBill, idDrink, amount);

            ShowBillInfoOfTable(table.ID);
            LoadTable();
        }

        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            var table = dgvBillInfo.Tag as TableDTO;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn trước khi chuyển bàn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id1 = table.ID;
            int id2 = Convert.ToInt32(cboSwitchTable.SelectedValue);

            DialogResult result = MessageBox.Show("Bạn có thật sự muốn chuyển từ " + table.Name + " sang " + cboSwitchTable.Text + " không?", "Chuyển bàn", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
                TableService.Instance.SwitchTable(id1, id2);
            LoadTable();
            ShowBillInfoOfTable(id1);
        }

        private void btnMergeTable_Click(object sender, EventArgs e)
        {
            var table = dgvBillInfo.Tag as TableDTO;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn trước khi gộp bàn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id1 = table.ID;
            int id2 = Convert.ToInt32(cboMergeTable.SelectedValue);

            DialogResult result = MessageBox.Show("Bạn có thật sự muốn gộp từ " + table.Name + " sang " + cboMergeTable.Text + " không?", "Gộp bàn", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
                TableService.Instance.MergeTable(id1, id2);
            LoadTable();
            ShowBillInfoOfTable(id1);
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id;
            if (!Int32.TryParse(cboCategory.SelectedValue.ToString(), out id))
                return;

            cboDrink.DataSource = DrinkDAL.Instance.GetDrinkByIdCategory(id);
            cboDrink.DisplayMember = "NameDrink";
            cboDrink.ValueMember = "IdDrink";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dgvBillInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnDeleteNumDrink_Click(object sender, EventArgs e)
        {
            var billInfo = dgvBillInfo.CurrentRow.DataBoundItem as BillInfoDTO;
            BillInfoService.Instance.DeleteBillInfo(billInfo.ID);
            ShowBillInfoOfTable((dgvBillInfo.Tag as TableDTO).ID);
        }

        private void cboDrink_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Program.context.LoginAccount = null;
        }

        private void pnlTitle_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tsmiTopDrink_Click(object sender, EventArgs e)
        {
            return;
        }

        private void thốngKêThứcUốngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrinkAnalytics drinkAnalytics = new DrinkAnalytics();
            drinkAnalytics.ShowDialog();
        }

        private void tsmiMaterial_Click(object sender, EventArgs e)
        {
            Material materialFrm = new Material();
            materialFrm.ShowDialog();
        }

        private void tsmiStorage_Click(object sender, EventArgs e)
        {
            MaterialItem materialItem = new MaterialItem();
            materialItem.ShowDialog();
        }

        private void tsmiSpending_Click(object sender, EventArgs e)
        {
            var spending = new Spending();
            spending.ShowDialog();
        }

        private void thốngKêThuChiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MonthlyStatistical monthlyStatistical = new MonthlyStatistical();
            monthlyStatistical.ShowDialog();
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
                cboCategory.SelectedValue = selectedItem.IdCategory;
                cboDrink.SelectedValue = selectedItem.IdDrink;
                lstSuggestions.Visible = false;
            }
        }

        private void dgvBillInfo_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvBillInfo.Rows[e.RowIndex];
                int id = Convert.ToInt32(row.Cells["ID"].Value);
                int discount = Convert.ToInt32(row.Cells["Discount"].Value);
                int idTable = Convert.ToInt32(row.Cells["IdTable"].Value);
                
                EditDiscount editForm = new EditDiscount(id, discount);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    ShowBillInfoOfTable(idTable);
                }
            }
        }

        private void btnOpenNote_Click(object sender, EventArgs e)
        {
            var table = dgvBillInfo.Tag as TableDTO;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn trước khi tạo ghi chú", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var billDtl = BillDAL.Instance.GetBillByIdTable(table.ID);
            if (billDtl.Rows.Count > 0)
            {
                NoteBill noteBill = new NoteBill(Convert.ToInt32(billDtl.Rows[0]["IdBill"]), billDtl.Rows[0]["Note"].ToString());
                if (noteBill.ShowDialog() == DialogResult.OK)
                {
                    
                }
            }
        }

        private void tsmiCustomer_Click(object sender, EventArgs e)
        {
            Customer customer = new Customer();
            customer.ShowDialog();
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            var table = dgvBillInfo.Tag as TableDTO;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn trước khi thêm khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idBill = BillService.Instance.GetUncheckBillId(table.ID);
            if (idBill != -1)
            {
                SetCustomer setCustomer = new SetCustomer(idBill);
                setCustomer.ShowDialog();
            }
        }

        private void lblMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void tsmiProduct_Click(object sender, EventArgs e)
        {
            Product product = new Product();
            product.ShowDialog();
        }

        private void báoCáoChốtCaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string barcode = txtBarcode.Text.Trim();

                if (!string.IsNullOrEmpty(barcode))
                {
                    DataTable dt = DrinkDAL.Instance.GetDrinkByBarcode(barcode);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        cboCategory.SelectedValue = Convert.ToInt32(dt.Rows[0]["IdCategory"]);
                        cboDrink.SelectedValue = Convert.ToInt32(dt.Rows[0]["IdDrink"]);
                    }

                    txtBarcode.Clear();
                    //
                    var table = dgvBillInfo.Tag as TableDTO;
                    if (table == null)
                    {
                        MessageBox.Show("Hãy chọn bàn trước khi thêm mặt hàng", "Quản lý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int idBill = BillService.Instance.GetUncheckBillId(table.ID);
                    int idDrink = Convert.ToInt32(cboDrink.SelectedValue);
                    int amount = (int)nudNumDrink.Value;
                    if (amount == 0) return;

                    if (idBill == -1) // Bàn này chưa có bill
                    {
                        BillService.Instance.InsertBill(table.ID);
                        BillInfoDAL.Instance.InsertBillInfo(BillDAL.Instance.GetMaxIDBill(), idDrink, amount);
                    }
                    else
                        BillInfoDAL.Instance.InsertBillInfo(idBill, idDrink, amount);

                    ShowBillInfoOfTable(table.ID);
                    LoadTable();
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void btnShipment_Click(object sender, EventArgs e)
        {
            var table = dgvBillInfo.Tag as TableDTO;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn trước khi thêm tiền ship", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var billDtl = BillDAL.Instance.GetBillByIdTable(table.ID);
            if (billDtl.Rows.Count > 0)
            {
                Shipment shipment = new Shipment(Convert.ToInt32(billDtl.Rows[0]["IdBill"]), Convert.ToDecimal(billDtl.Rows[0]["ShipBill"]));
                if (shipment.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }
    }
}
