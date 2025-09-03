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
    public partial class FormPrintPreview : Form
    {
        private Bitmap _bitmap;

        public FormPrintPreview(Bitmap bitmap)
        {
            InitializeComponent();
            _bitmap = bitmap;
            pictureBoxPreview.Image = _bitmap;
        }

        public bool ShouldPrint { get; private set; } = false;

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ShouldPrint = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ShouldPrint = false;
            this.Close();
        }

    }

}
