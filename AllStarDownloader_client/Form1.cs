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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;
using MySql.Data.MySqlClient;

namespace AllStarDownloader_client
{
    public partial class Form1 : Form
    {
        static string constructorString = "server=siriusxiang.xyz;user id=guest;database=allstar;persistsecurityinfo=False;sslmode=None";
        static string select_string = "select id,name,unit,subunit,rarity,update_date,downloadlink,downloadlink_awaken from card ";
        MySqlConnection con = new MySqlConnection(constructorString);
        Filter f = new Filter(select_string);
        DataSet ds = new DataSet();
        preview_form pf = new preview_form();
        batch_download bd = new batch_download();
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
                MySqlDataAdapter sda = new MySqlDataAdapter(new MySqlCommand("select " + (!is_awaken ? "preview_downloadlink,downloadlink" : "preview_downloadlink_awaken,downloadlink_awaken") + " from card where id = " + dataGridViewX1.Rows[e.RowIndex].Cells["id"].Value + ";", con));
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
                Environment.Exit(-1);
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
                dataGridViewX1.Columns.Add(new DataGridViewCheckBoxColumn() { AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, Name = "Choose All", ReadOnly = false, CellTemplate = new DataGridViewCheckBoxCell() { Value = true } } );
                sda.Fill(ds, "card");
                dataGridViewX1.DataSource = ds.Tables[0];
                dataGridViewX1.Columns["name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridViewX1.Columns["id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridViewX1.Columns["id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewX1.Columns.Add(new DataGridViewButtonColumn() { Name = "view_normal_preview", HeaderText = "View normal preview", Text = "View", UseColumnTextForButtonValue = true });
                dataGridViewX1.Columns.Add(new DataGridViewButtonColumn() { Name = "view_awaken_preview", HeaderText = "View awaken preview", Text = "View", UseColumnTextForButtonValue = true });
                for (int i = 0; i < dataGridViewX1.ColumnCount; i++)
                {
                    if (dataGridViewX1.Columns[i].Name != "Choose All")
                        dataGridViewX1.Columns[i].ReadOnly = true;
                }
                dataGridViewX1.Columns["downloadlink"].Visible = false;
                dataGridViewX1.Columns["downloadlink_awaken"].Visible = false;
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

        private void start_download_Click(object sender, EventArgs e)
        {
            if (!bd.IsDisposed && bd.is_downloading)
            {
                bd.ShowDialog();
                return;
            }
            if(bd.IsDisposed)
            {
                bd = new batch_download();
            }
            List<Common.SynFileInfo> info = new List<Common.SynFileInfo>();
            DataTable dx = new DataTable();
            dx.Columns.Add(new DataColumn()
            {
                ColumnName = "Id",
            });
            dx.Columns.Add(new DataColumn()
            {
                ColumnName = "Name",
            });
            dx.Columns.Add(new DataColumn()
            {
                ColumnName = "IsAwaken",
            });
            dx.Columns.Add(new DataColumn()
            {
                ColumnName = "Speed",
            });
            dx.Columns.Add(new DataColumn()
            {
                ColumnName = "url",
            });
            dx.Columns.Add(new DataColumn()
            {
                ColumnName = "path",
            });
            for (int i = 0; i < dataGridViewX1.Rows.Count; i++)
            {
                DataGridViewRow d = dataGridViewX1.Rows[i];
                if (d.Cells["Choose All"].EditedFormattedValue.ToString() == "True")
                {
                    if (d.Cells["downloadlink"].Value == null) continue;
                    System.Diagnostics.Debug.Write(d.Cells["id"].Value + " ");
                    string normallink = d.Cells["downloadlink"].Value.ToString();
                    string awakenlink = d.Cells["downloadlink_awaken"].Value.ToString();
                    info.Add(new Common.SynFileInfo(Convert.ToInt32(d.Cells["id"].Value),
                        d.Cells["id"].Value.ToString() + " " +
                        d.Cells["name"].Value.ToString() + " " +
                        d.Cells["rarity"].Value.ToString() + " normal." +
                        normallink.Substring(normallink.Length - 3),
                        normallink, 2 * i));
                    info.Add(new Common.SynFileInfo(Convert.ToInt32(d.Cells["id"].Value),
                        d.Cells["id"].Value.ToString() + " " +
                        d.Cells["name"].Value.ToString() + " " +
                        d.Cells["rarity"].Value.ToString() + " awaken." +
                        awakenlink.Substring(awakenlink.Length - 3),
                        awakenlink, 2 * i + 1));
                    dx.Rows.Add(d.Cells["id"].Value.ToString(), d.Cells["name"].Value.ToString(),"Normal","0.0",normallink,"hhh");
                    dx.Rows.Add(d.Cells["id"].Value.ToString(), d.Cells["name"].Value.ToString(), "Awaken","0.0", awakenlink, "hhh");
                }
            }
            
            if (info.Count == 0)
            {
                MessageBox.Show("0 files chosen! Please choose at least one file!", "Empty submission!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bd.set_datagridview(dx);
            bd.fileList = info;
            info = null;
            dx = null;
            bd.ShowDialog();
            System.Diagnostics.Debug.Write("\n");
            
        }

        private Applying myProcessBar = new Applying();
        private delegate bool IncreaseHandle(int nValue);
        private IncreaseHandle myIncrease = null;
        private void ShowProcessBar()
        {
            myProcessBar = new Applying();
            myProcessBar.set_max(dataGridViewX1.Rows.Count);
            myIncrease = new IncreaseHandle( myProcessBar.Increase );
            myProcessBar.ShowDialog();
            
        }

        private void dataGridViewX1_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != dataGridViewX1.Columns["Choose All"].Index) return;
            new Thread(new ThreadStart(set_all_items_checked)).Start();
            Filter.Focus();
        }

        private void set_all_items_checked()
        {
            //Filter.Focus();
            MethodInvoker mi = new MethodInvoker(ShowProcessBar);
            BeginInvoke(mi);
            bool blnIncreased = false;
            object objReturn = null;
            for (int i = 0; i < dataGridViewX1.Rows.Count; i++)
            {
                set_item_checked(i, "Choose All");
                objReturn = Invoke(myIncrease, new object[] { i + 1 });
                blnIncreased = (bool)objReturn;
            }
            
        }
        private delegate void delegate_set_item_checked(int rowindex, string columnname);
        private void set_item_checked(int rowindex, string columnname)
        {
            if (InvokeRequired)
            {
                Invoke(new delegate_set_item_checked(set_item_checked),rowindex,columnname);
            }
            else
            {
                bool b = Convert.ToBoolean(((DataGridViewCheckBoxCell)dataGridViewX1.Rows[rowindex].Cells[columnname]).Value);
                ((DataGridViewCheckBoxCell)dataGridViewX1.Rows[rowindex].Cells[columnname]).Value = !b;
                
            }
        }
    }
}
