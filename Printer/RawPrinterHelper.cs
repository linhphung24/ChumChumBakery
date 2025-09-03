using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Net;
using System.Windows.Forms;

namespace CoffeeManagement.Printer
{
    public class RawPrinterHelper
    {
        private PrintDocument printDocument;
        private DataTable _data;
        private string _tableId;
        private int _totalAmount;
        private int _shipBill;
        private string _timeIn = "07.14";
        private string _timeOut = "07.14";
        private string _billNumber = "I03480";
        private string _waiterName = Program.context.LoginAccount.DisplayName;
        private string _discount = "";
        private string _note = "";

        public RawPrinterHelper(DataTable data, string tableId)
        {
            _data = data;
            _tableId = tableId;

            // Tính tổng tiền
            _totalAmount = 0;
            foreach (DataRow row in _data.Rows)
            {
                _totalAmount += Convert.ToInt32(row["total_price"]);
                _shipBill = Convert.ToInt32(row["ship_bill"]);
                _note = row["note"].ToString();
            }
        }

        public void ShowPreview()
        {
            printDocument = new PrintDocument();

            // 👉 Tính chiều cao giấy dựa vào số dòng
            int height = EstimatePaperHeight(); // đơn vị: 1/100 inch
            printDocument.DefaultPageSettings.PaperSize = new PaperSize("K80", 315, height);
            //printDocument.PrinterSettings.PrinterName = "ITBI_U";
            printDocument.PrintPage += new PrintPageEventHandler(PrintPage);

            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = printDocument,
                Width = 800,
                Height = EstimatePaperHeight()
            };

            previewDialog.ShowDialog();
        }

        private int EstimatePaperHeight()
        {
            // 8 dòng cố định (tiêu đề + ngày + tổng tiền + cảm ơn)
            // Mỗi món có 2 dòng (tên + chi tiết)
            int totalLines = 8 + (_data.Rows.Count * 2);
            int height = totalLines * 30; // mỗi dòng cao khoảng 30 (1/100 inch)

            return height;
        }
        // Vẽ text căn giữa theo chiều ngang
        // Vẽ text căn giữa, tự xuống dòng khi quá dài
        void DrawCenteredWrappedText(Graphics g, string text, Font font, Brush brush, float paperWidth, ref float y)
        {
            string[] words = text.Split(' ');
            string line = "";
            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;
                SizeF size = g.MeasureString(testLine, font);
                if (size.Width > paperWidth) // nếu quá rộng thì xuống dòng
                {
                    SizeF lineSize = g.MeasureString(line, font);
                    float textX = (paperWidth - lineSize.Width) / 2;
                    g.DrawString(line, font, brush, textX, y);
                    y += font.GetHeight(g) + 2;
                    line = word;
                }
                else
                {
                    line = testLine;
                }
            }

            if (!string.IsNullOrEmpty(line))
            {
                SizeF lineSize = g.MeasureString(line, font);
                float textX = (paperWidth - lineSize.Width) / 2;
                g.DrawString(line, font, brush, textX, y);
                y += font.GetHeight(g) + 2;
            }
        }


        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            float y = 10;
            float x = 10;
            Font font = new Font("Tahoma", 7);
            Font boldFont = new Font("Tahoma", 8, FontStyle.Bold);
            Font titleFont = new Font("Tahoma", 10, FontStyle.Bold);
            Brush brush = Brushes.Black;
            Pen pen = new Pen(Brushes.Black, 0.5f);

            // Căn giữa text trong ô
            StringFormat centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // ===== Header
            float paperWidth = 315;

            DrawCenteredWrappedText(e.Graphics, "Chum Chum Bakery", new Font("Tahoma", 11, FontStyle.Bold), brush, paperWidth, ref y);
            DrawCenteredWrappedText(e.Graphics, "Đ/C: Chân cầu vượt tự khoát, Đường Ngũ Hiệp, thôn Tự Khoát, Xã Ngũ Hiệp, Thanh Trì", font, brush, paperWidth, ref y);
            DrawCenteredWrappedText(e.Graphics, "Hotline: 0983.112.676 / 0782.393.313", font, brush, paperWidth, ref y);
            DrawCenteredWrappedText(e.Graphics, "Chủ TK: BÙI PHƯƠNG LAN, OCB NH PHƯƠNG ĐÔNG 0983112676", font, brush, paperWidth, ref y);

            // ===== Dòng kẻ bằng text
            DrawDashedLine(e.Graphics, font, brush, paperWidth, ref y);
            y += 5;
            DrawCenteredWrappedText(e.Graphics, "HÓA ĐƠN THANH TOÁN", titleFont, brush, paperWidth, ref y);
            y += 5;


            // ===== Table setup (cột)
            float colTT = x;
            float colName = colTT + 25;
            float colQty = colName + 110;
            float colPrice = colQty + 30;
            float colDiscount = colPrice + 50;
            float colTotal = colDiscount + 30;

            float tableWidth = 25 + 110 + 30 + 50 + 30 + 60; // tổng chiều rộng

            // ===== Header row (cột)
            float headerHeight = 20;
            RectangleF rectTT = new RectangleF(colTT, y, 25, headerHeight);
            RectangleF rectName = new RectangleF(colName, y, 110, headerHeight);
            RectangleF rectQty = new RectangleF(colQty, y, 30, headerHeight);
            RectangleF rectPrice = new RectangleF(colPrice, y, 50, headerHeight);
            RectangleF rectDiscount = new RectangleF(colDiscount, y, 30, headerHeight);
            RectangleF rectTotal = new RectangleF(colTotal, y, 60, headerHeight);

            e.Graphics.DrawRectangle(pen, rectTT.X, rectTT.Y, rectTT.Width, rectTT.Height);
            e.Graphics.DrawString("TT", font, brush, rectTT, centerFormat);

            e.Graphics.DrawRectangle(pen, rectName.X, rectName.Y, rectName.Width, rectName.Height);
            e.Graphics.DrawString("Tên món", font, brush, rectName, centerFormat);

            e.Graphics.DrawRectangle(pen, rectQty.X, rectQty.Y, rectQty.Width, rectQty.Height);
            e.Graphics.DrawString("SL", font, brush, rectQty, centerFormat);

            e.Graphics.DrawRectangle(pen, rectPrice.X, rectPrice.Y, rectPrice.Width, rectPrice.Height);
            e.Graphics.DrawString("Đ.Giá", font, brush, rectPrice, centerFormat);

            e.Graphics.DrawRectangle(pen, rectDiscount.X, rectDiscount.Y, rectDiscount.Width, rectDiscount.Height);
            e.Graphics.DrawString("CK", font, brush, rectDiscount, centerFormat);

            e.Graphics.DrawRectangle(pen, rectTotal.X, rectTotal.Y, rectTotal.Width, rectTotal.Height);
            e.Graphics.DrawString("T.Tiền", font, brush, rectTotal, centerFormat);

            y += headerHeight;

            // ===== Table body
            int index = 1;
            foreach (DataRow row in _data.Rows)
            {
                string name = row["drink_name"].ToString();
                int qty = Convert.ToInt32(row["quantity"]);
                int total = Convert.ToInt32(row["total_price"]);
                int discount = Convert.ToInt32(row["discount"]); 
                int price = Convert.ToInt32(row["drink_price"]);

                // Đo chiều cao tên món
                float nameHeight = DrawWrappedTextMeasure(e.Graphics, name, font, 110);
                float rowHeight = Math.Max(20, nameHeight);

                // STT
                RectangleF cellTT = new RectangleF(colTT, y, 25, rowHeight);
                e.Graphics.DrawRectangle(pen, cellTT.X, cellTT.Y, cellTT.Width, cellTT.Height);
                e.Graphics.DrawString(index.ToString(), font, brush, cellTT, centerFormat);

                // Tên món
                RectangleF cellName = new RectangleF(colName, y, 110, rowHeight);
                e.Graphics.DrawRectangle(pen, cellName.X, cellName.Y, cellName.Width, cellName.Height);
                float tempY = y + 2;
                DrawWrappedText(e.Graphics, name, font, brush, colName + 2, ref tempY, 110);

                // SL
                RectangleF cellQty = new RectangleF(colQty, y, 30, rowHeight);
                e.Graphics.DrawRectangle(pen, cellQty.X, cellQty.Y, cellQty.Width, cellQty.Height);
                e.Graphics.DrawString(qty.ToString(), font, brush, cellQty, centerFormat);

                // Giá
                RectangleF cellPrice = new RectangleF(colPrice, y, 50, rowHeight);
                e.Graphics.DrawRectangle(pen, cellPrice.X, cellPrice.Y, cellPrice.Width, cellPrice.Height);
                e.Graphics.DrawString(price.ToString("#,##0"), font, brush, cellPrice, centerFormat);

                // CK
                RectangleF cellDiscount = new RectangleF(colDiscount, y, 30, rowHeight);
                e.Graphics.DrawRectangle(pen, cellDiscount.X, cellDiscount.Y, cellDiscount.Width, cellDiscount.Height);
                e.Graphics.DrawString((discount > 0 ? "100%" : "0%"), font, brush, cellDiscount, centerFormat);

                // Thành tiền
                RectangleF cellTotal = new RectangleF(colTotal, y, 60, rowHeight);
                e.Graphics.DrawRectangle(pen, cellTotal.X, cellTotal.Y, cellTotal.Width, cellTotal.Height);
                e.Graphics.DrawString(total.ToString("#,##0"), font, brush, cellTotal, centerFormat);

                y += rowHeight;
                index++;
            }

            y += 10;

            // ===== Note
            DrawDashedLine(e.Graphics, font, brush, paperWidth, ref y);
            DrawCenteredWrappedText(e.Graphics, _note, font, brush, paperWidth, ref y);

            // ===== Tổng tiền
            e.Graphics.DrawString("Thành tiền:", boldFont, brush, x, y);
            e.Graphics.DrawString(_totalAmount.ToString("#,##0"), font, brush, x + 180, y);
            y += 14;

            e.Graphics.DrawString("Tiền giảm giá món", font, brush, x, y);
            e.Graphics.DrawString(_discount, font, brush, x + 180, y);
            y += 14;

            e.Graphics.DrawString("Tiền phí vận chuyển:", font, brush, x, y);
            e.Graphics.DrawString(_shipBill.ToString("#,##0"), font, brush, x + 180, y);
            y += 14;

            e.Graphics.DrawString("Tiền Thanh Toán:", boldFont, brush, x, y);
            e.Graphics.DrawString((_totalAmount + _shipBill).ToString("#,##0"), boldFont, brush, x + 160, y);
            y += 16;

            //e.Graphics.DrawString("+ Tiền mặt VND", font, brush, x, y);
            //e.Graphics.DrawString("1 440 000", font, brush, x + 160, y);
            //y += 20;

            // ===== Dòng kẻ bằng text
            DrawDashedLine(e.Graphics, font, brush, paperWidth, ref y);
            // ===== VietQR
            string vietQrUrl = "https://img.vietqr.io/image/970448-0983112676-print.png?amount="
                               + _totalAmount + "&addInfo=ThanhToanHoaDon" + _billNumber;

            Image qrImage = LoadImageFromUrl(vietQrUrl);

            int qrSize = 200;     // kích thước QR in ra
            float pageWidth = e.PageBounds.Width;  // chiều rộng khổ giấy
            float qrX = (pageWidth - qrSize) / 2;  // căn giữa ngang
            e.Graphics.DrawImage(qrImage, new RectangleF(qrX, y, qrSize, qrSize));
            y += qrSize + 10;

            // ===== Footer
            DrawCenteredWrappedText(e.Graphics, "Cảm ơn Quý khách và hẹn gặp lại!", font, brush, paperWidth, ref y);
        }
        void DrawDashedLine(Graphics g, Font font, Brush brush, float paperWidth, ref float y)
        {
            // Tạo một chuỗi dài toàn dấu '-'
            string line = new string('-', 40); // 40 ký tự, bạn có thể tăng/giảm
            SizeF size = g.MeasureString(line, font);

            // Căn giữa chuỗi dấu gạch
            float textX = (paperWidth - size.Width) / 2;
            g.DrawString(line, font, brush, textX, y);

            y += font.GetHeight(g) + 2;
        }

        // Vẽ chữ xuống dòng
        void DrawWrappedText(Graphics g, string text, Font font, Brush brush, float x, ref float y, int maxWidth)
        {
            string[] words = text.Split(' ');
            string line = "";
            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;
                SizeF size = g.MeasureString(testLine, font);
                if (size.Width > maxWidth)
                {
                    g.DrawString(line, font, brush, x, y);
                    y += font.GetHeight(g) + 2;
                    line = word;
                }
                else
                {
                    line = testLine;
                }
            }
            if (!string.IsNullOrEmpty(line))
            {
                g.DrawString(line, font, brush, x, y);
                y += font.GetHeight(g) + 2;
            }
        }

        // Đo chiều cao chữ khi xuống dòng
        float DrawWrappedTextMeasure(Graphics g, string text, Font font, int maxWidth)
        {
            string[] words = text.Split(' ');
            string line = "";
            float totalHeight = 0;
            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;
                SizeF size = g.MeasureString(testLine, font);
                if (size.Width > maxWidth)
                {
                    totalHeight += font.GetHeight(g) + 2;
                    line = word;
                }
                else
                {
                    line = testLine;
                }
            }
            if (!string.IsNullOrEmpty(line))
                totalHeight += font.GetHeight(g) + 2;

            return totalHeight;
        }
        private Image LoadImageFromUrl(string url)
        {
            using (WebClient wc = new WebClient())
            {
                byte[] bytes = wc.DownloadData(url);
                using (var ms = new System.IO.MemoryStream(bytes))
                {
                    return Image.FromStream(ms);
                }
            }
        }
    }
}
