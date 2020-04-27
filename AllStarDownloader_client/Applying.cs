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
    public partial class Applying : Form
    {
        public Applying()
        {
            InitializeComponent();
        }
        public void set_max(int max)
        {
            prcBar.Maximum = max;
        }
        public bool Increase(int nValue)
        {
            if (prcBar.Value != prcBar.Maximum - 1)
            {
                prcBar.Value = nValue;
                return true;
            }
            else
            {
                prcBar.Value = prcBar.Maximum;
                Close();
                return false;
            }
            
        }
    }
}
