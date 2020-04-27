using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllStarDownloader_client
{
    public partial class Filter : Form
    {
        public event return_command R_command;
        public static string sqlcommand_head;
        public string group;
        public Filter()
        {
            InitializeComponent();
        }
        public Filter(string head)
        {
            InitializeComponent();
            sqlcommand_head = head;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int from = range_from.Text.Length == 0 ? 0 : Convert.ToInt32(range_from.Text);
            int to = range_to.Text.Length == 0 ? int.MaxValue : Convert.ToInt32(range_to.Text);
            string c = sqlcommand_head + "where ";
            c += "(" + (rarity_r.Checked ? "rarity='Rare' or " : "") +
                 (rarity_sr.Checked ? "rarity='Super rare' or " : "") +
                 (rarity_ur.Checked ? "rarity='Ultra rare' or " : "") + "false) and (";
            foreach (dynamic item in muse.Controls)
            {
                if (item is CheckBox && item.Checked) c += "name like '%-" + item.Text + "' or ";
            }
            foreach (dynamic item in aqours.Controls)
            {
                if (item is CheckBox && item.Checked) c += "name like '%-" + item.Text + "' or ";
            }
            foreach (dynamic item in Nijigasaki.Controls)
            {
                if (item is CheckBox && item.Checked) c += "name like '%-" + item.Text + "' or ";
            }
            c += "false) and ";
            foreach (dynamic item in Group_filt.Controls)
            {
                if (item.Checked)
                {
                    if (item.Text == "All") { c += "true"; break; }
                    else if (item.Text == "None") { c += "false"; break; }
                    c += "subunit='" + item.Text + "'";
                    break; 
                }
            }
            if(from > to)
            {
                MessageBox.Show("Range is invalid! Please check again.", "Invalid range", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            c += " and id <= " + to + " and id >= " + from + ";";
            System.Diagnostics.Debug.Write(c + "\n");
            R_command(c);
            Hide();
        }

        private void rarity_all_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in rarity.Controls)
            {
                if (item is CheckBox) item.Checked = true;
                else continue;
            }
        }

        private void rarity_none_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in rarity.Controls)
            {
                if (item is CheckBox) item.Checked = false;
                else continue;
            }
        }

        private void rarity_reverse_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in rarity.Controls)
            {
                if (item is CheckBox) item.Checked = !item.Checked;
                else continue;
            }

        }

        private void muse_all_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in muse.Controls)
            {
                if (item is CheckBox) item.Checked = true;
                else continue;
            }
        }

        private void muse_none_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in muse.Controls)
            {
                if (item is CheckBox) item.Checked = false;
                else continue;
            }
        }

        private void muse_reverse_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in muse.Controls)
            {
                if (item is CheckBox) item.Checked = !item.Checked;

                else continue;
            }
        }

        private void aq_all_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in aqours.Controls)
            {
                if (item is CheckBox) item.Checked = true;
                else continue;
            }
        }

        private void aq_none_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in aqours.Controls)
            {
                if (item is CheckBox) item.Checked = false;
                else continue;
            }
        }

        private void aq_reverse_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in aqours.Controls)
            {
                if (item is CheckBox) item.Checked = !item.Checked;
                else continue;
            }

        }

        private void niji_all_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in Nijigasaki.Controls)
            {
                if (item is CheckBox) item.Checked = true;
                else continue;
            }
        }

        private void niji_none_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in Nijigasaki.Controls)
            {
                if (item is CheckBox) item.Checked = false;
                else continue;
            }
        }

        private void niji_reverse_Click(object sender, EventArgs e)
        {
            foreach (dynamic item in Nijigasaki.Controls)
            {
                if (item is CheckBox) item.Checked = !item.Checked;
                else continue;
            }
        }

        private string get_group()
        {
            foreach (dynamic item in Group_filt.Controls)
            {
                if (item.Checked) return item.Text;
            }
            return "null";
        }

        private void range_from_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar) || e.KeyChar == '\b'))
            {
                e.Handled = true;
            }
        }

        private void range_to_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar) || e.KeyChar == '\b'))
            {
                e.Handled = true;
            }
        }
    }

    public delegate void return_command(string command);
}
