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
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        // Makes a millisecond property with getter and setter
        public int Milliseconds
        {
            get { return (int)numericUpDownMilli.Value; }
            set { numericUpDownMilli.Value = value; }
        }

        // Makes a Width property with getter and setter
        public int Width
        {
            get { return (int)numericUpDownWidth.Value; }
            set { numericUpDownWidth.Value = value; }
        }

        // Makes a Height property with getter and setter
        public int Height
        {
            get { return (int)numericUpDownHeight.Value; }
            set { numericUpDownHeight.Value = value; }
        }
    }
}
