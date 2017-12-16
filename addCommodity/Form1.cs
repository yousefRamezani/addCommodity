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

        void bindGrid()
        {
            radGridView1.DataSource = CommodityService.getall();
            if (radGridView1.Rows.Count() > 0)
            {
                radGridView1.Rows[radGridView1.Rows.Count - 1].IsCurrent = true;
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                bindGrid();
                AppService apps = new AppService();
                var info = apps.get();
                lblMarketName.Text = info.Market;
                lblUser.Text = info.User;
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
            Invoke(new Action(() => txtUnit.Text = ""));
            Invoke(new Action(() => txtMultipler.Text = ""));
            Invoke(new Action(() => txtPrice.Text = ""));
            Invoke(new Action(() => lblBarcode.Text = "- - -"));
            
            long barcode = 0;
            long.TryParse(txtBarcode.Text.Trim(), out barcode);
            if (string.IsNullOrEmpty(txtBarcode.Text) || barcode == 0)
            {
                return;
            }
            try
            {
                commodity = CommodityService.get(barcode);
                if (commodity != null)
                {

                    Invoke(new Action(() => txtName.Text = commodity.Name));
                    Invoke(new Action(() => txtBrand.Text = commodity.Brand));
                    Invoke(new Action(() => txtUnit.Text = commodity.Unit));
                    Invoke(new Action(() => txtMultipler.Text = commodity.Multiplier+""));
                    Invoke(new Action(() => txtPrice.Text = commodity.Price == 0 ? "" : commodity.Price + ""));
                    
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
                        comm.BarCode = long.Parse(lblBarcode.Text.Trim());
                        comm.Name = txtName.Text.Trim();
                        comm.Brand = txtBrand.Text;
                        comm.Unit = txtUnit.Text;
                        comm.Multiplier = short.Parse(txtMultipler.Text);
                        comm.Price = int.Parse(txtPrice.Text.Replace(",", ""));

                        CommodityService.add(comm);

                        lblMessageCommodity.Text = comm.Name + " - با موفقیت دخیره شد";
                    }
                    else
                    {
                        if (DontCheckedChangeValue())
                        {
                            commodity.State = 2;
                        }
                        else
                        {
                            commodity.State = 3;
                        }
                        commodity.Name = txtName.Text.Trim();
                        commodity.Brand = txtBrand.Text;
                        commodity.Unit = txtUnit.Text;
                        commodity.Multiplier = short.Parse(txtMultipler.Text);
                        commodity.Price = int.Parse(txtPrice.Text.Replace(",", ""));

                        CommodityService.update(commodity);

                        lblMessageCommodity.Text = commodity.Name + " - با موفقیت دخیره شد";
                    }
                    bindGrid();
                    txtBarcode.Focus();
                }
                else
                {
                    lblMessageCommodity.Text = "ورودی اطلاعات دارای اشکال می باشد";
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
            = grpUnit.BackColor
            = grpBrand.BackColor
            = grpMultipler.BackColor = Color.WhiteSmoke;

            if (!Regex.IsMatch(txtPrice.Text, @"^[-+]?(?:[0-9]+,)*[0-9]+(?:\.[0-9]+)?$")
                || string.IsNullOrEmpty(txtPrice.Text))
            {
                grpPrice.BackColor = Color.Pink;
                result = false;
            }
            if (!Regex.IsMatch(txtMultipler.Text, @"^[0-9]*$") || string.IsNullOrEmpty(txtMultipler.Text))
            {
                grpMultipler.BackColor = Color.Pink;
                result = false;
            }
            if (string.IsNullOrEmpty(txtName.Text))
            {
                grpName.BackColor = Color.Pink;
                result = false;
            }
            if (string.IsNullOrEmpty(txtUnit.Text))
            {
                grpUnit.BackColor = Color.Pink;
                result = false;
            }
            return result;
        }
        bool DontCheckedChangeValue()
        {
            if (txtName.Text.Trim() != commodity.Name)
            {
                return false;
            }
            if (txtBrand.Text.Trim() != commodity.Brand)
            {
                return false;
            }
            if (txtUnit.Text.Trim() != commodity.Unit)
            {
                return false;
            }
            if (txtMultipler.Text.Trim() != commodity.Multiplier.ToString())
            {
                return false;
            }
            return true;
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
                        txtUnit.Select(0, txtUnit.Text.Length);
                        txtUnit.Focus();
                        break;
                    }
                case Keys.F5:
                    {
                        txtMultipler.Select(0, txtMultipler.Text.Length);
                        txtMultipler.Focus();
                        break;
                    }
                case Keys.F6:
                    {
                        txtPrice.Select(0, txtPrice.Text.Length);
                        txtPrice.Focus();
                        break;
                    }
                case Keys.F7:
                    {
                        btnSave_Click(null,null);
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
    }
}
