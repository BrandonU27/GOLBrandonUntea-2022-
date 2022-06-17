using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLBrandonUntea
{
    public partial class Seed : Form
    {
        public Seed()
        {
            InitializeComponent();
        }

        // get in the seed typed or returns the seed
        public int SeedNum
        {
            get { return (int)numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }

        private void buttonRand_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            numericUpDown1.Value = rand.Next(-2147483647, 2147483647);
        }
    }
}
