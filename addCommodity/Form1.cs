using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Telerik.WinControls;
using System.Windows.Forms;
using System.IO;
using addCommodity.Model;
using addCommodity.Service;
using System.Text.RegularExpressions;
using System.Threading;

namespace addCommodity
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Commodity commodity;
        int count = 0;
        void bindGrid()
        {
            radGridView1.DataSource = CommodityService.getall();
            if (radGridView1.Rows.Count() > 0)
            {
                radGridView1.Rows[radGridView1.Rows.Count - 1].IsCurrent = true;
            }

            lblUser.Text = "کالاهای افزوده شده : " + radGridView1.Rows.Count;

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                AppService apps = new AppService();
                var info = apps.get();
                lblMarketName.Text = info.Market;
                count = info.Count;
                bindGrid();
                lblDate.Text = PersianDateTime.Now.DayName + "  " + PersianDateTime.Now.Day + "  " + PersianDateTime.Now.MonthName + "  " + PersianDateTime.Now.Year;
            }
            catch (Exception ex)
            {
                lblMessageBarcode.Text = ex.Message;
            }

        }

        void worker()
        {



            Invoke(new Action(() => txtName.Text = ""));
            Invoke(new Action(() => txtBrand.Text = ""));
            Invoke(new Action(() => txtPrice.Text = ""));
            Invoke(new Action(() => lblBarcode.Text = "- - -"));
            Invoke(new Action(() => lblMessageCommodity.Text = ""));
            grpPrice.BackColor
            = grpName.BackColor
            = grpPrice.BackColor
            = grpBrand.BackColor = Color.WhiteSmoke;

            long barcode = 0;
            long.TryParse(txtBarcode.Text.Trim(), out barcode);
            if (string.IsNullOrEmpty(txtBarcode.Text) || barcode == 0)
            {
                timer1.Enabled = false;
                return;
            }


            try
            {
                commodity = CommodityService.get(barcode);
                if (commodity != null)
                {

                    Invoke(new Action(() => txtName.Text = commodity.Name));
                    Invoke(new Action(() => txtBrand.Text = commodity.Brand));
                    Invoke(new Action(() => txtPrice.Text = commodity.Price == 0 ? "" : commodity.Price.ToString("N0")));

                    Invoke(new Action(() => lblMessageCommodity.Text = ""));
                    Invoke(new Action(() => lblMessageBarcode.Text = ""));

                    Invoke(new Action(() => lblBarcode.Text = txtBarcode.Text));
                    Invoke(new Action(() => txtBarcode.Text = ""));

                    Invoke(new Action(() => txtPrice.Focus()));
                }
                else
                {
                    Invoke(new Action(() => lblMessageCommodity.Text = ""));
                    Invoke(new Action(() => lblMessageBarcode.Text = ""));
                    Invoke(new Action(() => lblMessageCommodity.Text = "هیچ کالایی با این بارکد از پایگاه داده یافت نشد"));

                    Invoke(new Action(() => lblBarcode.Text = txtBarcode.Text));
                    Invoke(new Action(() => txtBrand.Focus()));
                }


            }
            catch (Exception ex)
            {
                Invoke(new Action(() => lblMessageCommodity.Text = ""));
                Invoke(new Action(() => lblMessageBarcode.Text = ""));
                Invoke(new Action(() => lblMessageBarcode.Text = "فرمت بارکد وارد شده اشتباه می باشد"));
            }
            timer1.Enabled = false;
        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {

            timer1.Enabled = true;
        }

        private void txtBarcode_Leave(object sender, EventArgs e)
        {

            txtBarcode.Text = "برنامه در حال حاضر قادر به دریافت بارکد از بارکد خوان نیست";

        }

        private void txtBarcode_Enter(object sender, EventArgs e)
        {

            txtBarcode.Text = "آماده دریافت بار کد ...";

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (validate())
                {
                    if (commodity == null)
                    {
                        Commodity comm = new Commodity();
                        if (string.IsNullOrWhiteSpace(lblBarcode.Text) || lblBarcode.Text == "- - -")
                        {
                            comm.BarCode = 0;
                        }
                        else
                        {
                            comm.BarCode = long.Parse(lblBarcode.Text.Trim());
                        }                        
                        comm.Name = txtName.Text.Trim();
                        comm.Brand = txtBrand.Text;
                        comm.Price = int.Parse(txtPrice.Text.Replace(",", ""));

                        CommodityService.add(comm);

                        lblMessageCommodity.Text = comm.Name + " - با موفقیت دخیره شد";
                    }
                    else
                    {
                        if (ISChangeValue())
                        {
                            commodity.State = 3;
                        }
                        else
                        {
                            commodity.State = 2;
                        }
                        commodity.BarCode = long.Parse(lblBarcode.Text.Trim());
                        commodity.Name = txtName.Text.Trim();
                        commodity.Brand = txtBrand.Text;
                        commodity.Price = int.Parse(txtPrice.Text.Replace(",", ""));

                        CommodityService.update(commodity);

                        lblMessageCommodity.Text = commodity.Name + " - با موفقیت دخیره شد";
                    }
                    bindGrid();
                    txtBarcode.Focus();
                }
                else
                {
                    //lblMessageCommodity.Text = "ورودی اطلاعات دارای اشکال می باشد";
                }
            }
            catch (Exception ex)
            {
                lblMessageCommodity.Text = ex.Message;
            }
        }

        private void txtPrice_KeyUp(object sender, KeyEventArgs e)
        {
            txtPrice.Text = Regex.Replace(txtPrice.Text.Replace(",", ""), @"\d{1,3}(?=(\d{3})+(?!\d))", "$&,");
            txtPrice.SelectionStart = txtPrice.Text.Length;
        }

        bool validate()
        {
            lblMessageCommodity.Text = "";
            bool result = true;
            grpPrice.BackColor
            = grpName.BackColor
            = grpPrice.BackColor
            = grpBrand.BackColor = Color.WhiteSmoke;

            if (!Regex.IsMatch(txtPrice.Text, @"^[-+]?(?:[0-9]+,)*[0-9]+(?:\.[0-9]+)?$")
                /*|| string.IsNullOrEmpty(txtPrice.Text)*/)
            {
                grpPrice.BackColor = Color.Pink;
                lblMessageCommodity.Text = "لطفا قیمت محصول را صحیح وارد کنید";
                result = false;
            }
            if (string.IsNullOrEmpty(txtName.Text))
            {
                grpName.BackColor = Color.Pink;
                lblMessageCommodity.Text = lblMessageCommodity.Text == "" ? "نام محصول وارد نشده است" : lblMessageCommodity.Text + "\n" + "نام محصول وارد نشده است";
                result = false;
            }
            if (!Regex.IsMatch(lblBarcode.Text, @"^[0-9]*$") && !string.IsNullOrEmpty(lblBarcode.Text) && lblBarcode.Text.Trim() != "- - -")
            {
                lblMessageCommodity.Text = lblMessageCommodity.Text == "" ? "فرمت بارکد اشتباه است" : lblMessageCommodity.Text + "\n" + "فرمت بارکد اشتباه است";
                result = false;
            }

            return result;
        }
        bool ISChangeValue()
        {
            if (txtName.Text.Trim() != commodity.Name)
            {
                return true;
            }
            if (txtBrand.Text.Trim() != commodity.Brand)
            {
                return true;
            }
            return false;
        }

        private void txtBarcode_TextChanging(object sender, TextChangingEventArgs e)
        {


        }

        private void btnSave_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void txtBarcode_Click(object sender, EventArgs e)
        {
            txtBarcode.Select(0, txtBarcode.Text.Length);
        }

        private void txtBarcode_DoubleClick(object sender, EventArgs e)
        {
            txtBarcode.Select(0, txtBarcode.Text.Length);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            worker();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            focusKey(e.KeyCode);
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            focusKey(e.KeyCode);
        }

        void focusKey(Keys key)
        {
            switch (key)
            {
                case Keys.F2:
                    {
                        txtBrand.Select(0, txtBrand.Text.Length);
                        txtBrand.Focus();
                        break;
                    }
                case Keys.F3:
                    {
                        txtName.Select(0, txtName.Text.Length);
                        txtName.Focus();
                        break;
                    }
                case Keys.F4:
                    {
                        txtPrice.Select(0, txtPrice.Text.Length);
                        txtPrice.Focus();
                        break;
                    }
                case Keys.F5:
                    {
                        btnSave_Click(null, null);
                        break;
                    }
                case Keys.F1:
                    {
                        txtBarcode.Select(0, txtBarcode.Text.Length);
                        txtBarcode.Focus();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void txtBrand_KeyDown(object sender, KeyEventArgs e)
        {
            focusKey(e.KeyCode);
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                txtBrand.Text = txtName.SelectedText;
                txtName.Text = txtName.Text.Replace(txtName.SelectedText, "");
                return;
            }
            focusKey(e.KeyCode);
        }

        private void txtUnit_KeyDown(object sender, KeyEventArgs e)
        {

            focusKey(e.KeyCode);
        }

        private void txtMultipler_KeyDown(object sender, KeyEventArgs e)
        {
            focusKey(e.KeyCode);
        }

        private void txtPrice_KeyDown(object sender, KeyEventArgs e)
        {
            focusKey(e.KeyCode);
        }

        private void radGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            focusKey(e.KeyCode);
        }

        private void lblMessageBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            focusKey(e.KeyCode);
        }

        private void lblMessageCommodity_KeyDown(object sender, KeyEventArgs e)
        {
            focusKey(e.KeyCode);
        }

        private void lblBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            focusKey(e.KeyCode);
        }

        private void btnSave_KeyDown(object sender, KeyEventArgs e)
        {

            focusKey(e.KeyCode);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var apps = new AppService();
            apps.save(lblDate.Text);
        }

        private void radGridView1_CellClick(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            if (e.Row.Cells[0].Value == null)
            {
                return;
            }
            long barcode = 0;
            long.TryParse(e.Row.Cells[0].Value.ToString(),out barcode);
            Invoke(new Action(() => txtName.Text = ""));
            Invoke(new Action(() => txtBrand.Text = ""));
            Invoke(new Action(() => txtPrice.Text = ""));
            Invoke(new Action(() => lblBarcode.Text = "- - -"));
            Invoke(new Action(() => lblMessageCommodity.Text = ""));
            grpPrice.BackColor
            = grpName.BackColor
            = grpPrice.BackColor
            = grpBrand.BackColor = Color.WhiteSmoke;

            if (e.ColumnIndex == 5)
            {
                if (MessageBox.Show("آیا از حذف این کالا مطمئن هستید ؟","حذف کالا",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    CommodityService.removeCommodity(barcode, e.Row.Cells[1].Value.ToString());
                    bindGrid();
                }
                return;
            }

            commodity = CommodityService.get(barcode, e.Row.Cells[1].Value.ToString());

            if (commodity != null)
            {

                Invoke(new Action(() => txtName.Text = commodity.Name));
                Invoke(new Action(() => txtBrand.Text = commodity.Brand));
                Invoke(new Action(() => txtPrice.Text = commodity.Price == 0 ? "" : commodity.Price.ToString("N0")));

                Invoke(new Action(() => lblMessageCommodity.Text = ""));
                Invoke(new Action(() => lblMessageBarcode.Text = ""));

                Invoke(new Action(() => lblBarcode.Text = commodity.BarCode+""));

                Invoke(new Action(() => txtPrice.Focus()));
            }

        }
    }
}
