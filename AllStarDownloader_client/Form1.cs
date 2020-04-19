using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AllStarDownloader_client
{
    public partial class Form1 : Form
    {
        static string constructorString = "server=siriusxiang.xyz;user id=guest;database=allstar;persistsecurityinfo=False;sslmode=None";
        static string select_string = "select id,name,unit,subunit,rarity from card ";
        MySqlConnection con = new MySqlConnection(constructorString);
        Filter f = new Filter(select_string);
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
                DataSet ds = new DataSet();
                sda.Fill(ds, "card");
                dataGridViewX1.DataSource = ds.Tables[0];

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
    }
}
