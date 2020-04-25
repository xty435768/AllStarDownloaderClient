using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AllStarDownloader_client
{
    public partial class Form1 : Form
    {
        static string constructorString = "server=siriusxiang.xyz;user id=guest;database=allstar;persistsecurityinfo=False;sslmode=None";
        static string select_string = "select id,name,unit,subunit,rarity,update_date from card ";
        MySqlConnection con = new MySqlConnection(constructorString);
        Filter f = new Filter(select_string);
        DataSet ds = new DataSet();
        preview_form pf = new preview_form();
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string col_name = dataGridViewX1.Columns[e.ColumnIndex].Name;
            bool is_awaken = !col_name.Contains("normal");
            if ((col_name != "view_normal_preview" && col_name != "view_awaken_preview") || e.RowIndex < 0 || e.RowIndex > ds.Tables[0].Rows.Count - 1)
                return;
            try
            {
                MySqlDataAdapter sda = new MySqlDataAdapter(new MySqlCommand("select " + (!is_awaken ? "preview_downloadlink,downloadlink" : "preview_downloadlink_awaken,downloadlink_awaken") + " from card where id = " + dataGridViewX1.Rows[e.RowIndex].Cells[0].Value + ";", con));
                DataSet temp_ds = new DataSet();
                sda.Fill(temp_ds, "temp_preview_link");
                //System.Diagnostics.Debug.Write();
                pf.url = temp_ds.Tables[0].Rows[0][0].ToString();
                pf.picture_name = ds.Tables[0].Rows[e.RowIndex][0].ToString()+" "+ ds.Tables[0].Rows[e.RowIndex][1].ToString() + (is_awaken ? " awaken" : " normal");
                pf.Text += pf.picture_name;
                pf.original_url = temp_ds.Tables[0].Rows[0][1].ToString();
                pf.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "请求预览失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "网络错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(select_string);
            try
            {
                MySqlDataAdapter sda = new MySqlDataAdapter(new MySqlCommand(select_string, con));
                ds = new DataSet();
                dataGridViewX1.Columns.Clear();
                sda.Fill(ds, "card");
                dataGridViewX1.DataSource = ds.Tables[0];
                dataGridViewX1.Columns.Add(new DataGridViewButtonColumn() { Name = "view_normal_preview", HeaderText = "View normal preview", Text = "View", UseColumnTextForButtonValue = true });
                dataGridViewX1.Columns.Add(new DataGridViewButtonColumn() { Name = "view_awaken_preview", HeaderText = "View awaken preview", Text = "View", UseColumnTextForButtonValue = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误：" + ex.Message, "查询错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        private void Filter_Click(object sender, EventArgs e)
        {
            f.R_command += new return_command(filter_set_command);
            f.ShowDialog();
        }

        void filter_set_command(string c)
        {
            select_string = c;
        }

        private void dataGridViewX1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            MessageBox.Show(e.RowIndex.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit?", "Exit confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Dispose();
                con.Close();
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
