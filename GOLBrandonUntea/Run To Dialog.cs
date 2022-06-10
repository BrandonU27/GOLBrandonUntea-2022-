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
    public partial class Run_To_Dialog : Form
    {
        public Run_To_Dialog()
        {
            InitializeComponent();
        }

        // Makes a number property with getters and setters

        public int Number
        {
            get { return (int)numericUpDownTo.Value; }
            set { numericUpDownTo.Value = value; }
        }
    }
}
