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
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[30, 30];
        bool[,] scratchPad = new bool[30, 30];

        // Game mode switch
        int mode = 0;

        // Number views on/off
        int viewNum = 0;

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            int count;

            for(int y = 0; y < universe.GetLength(1); y++)
            {
                for(int x = 0; x <universe.GetLength(0); x++)
                {
                    // int count == CountNeighbor
                    if(mode == 0)
                    {
                        count = CountNeighborsToroidal(x,y);
                    }
                    else
                    {
                        count = CountNeighborsFinite(x, y);
                    }

                    // Apply the rules 
                    // Choose if the cell is off or on
                    // Turn it on/off in the scratchpad

                    if (universe[x, y] == true)
                    {
                        if (count < 2) { scratchPad[x, y] = false; }
                        if (count > 3) { scratchPad[x, y] = false; }
                        if (count == 2 || count == 3) { scratchPad[x, y] = true; }
                    }
                    if (universe[x,y] == false)
                    {
                        if (count == 3) { scratchPad[x, y] = true; }
                    }

                    
                }   

            }

            // Copy from scratchPad to universe
            // Make sure to clear the scratchPad after

            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    scratchPad[x, y] = false;
                }
            }

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);
            
            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    // Adds numbers if on
                    if(viewNum == 0)
                    {
                        Font font = new Font("Arial", 20f);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        int neighbors;
                        if(mode == 0) { neighbors = CountNeighborsToroidal(x, y); }
                        else { neighbors = CountNeighborsFinite(x, y); }

                        if(neighbors == 0) { continue; }

                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Black, cellRect, stringFormat);
                    }
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        //
        //  NOTE TO SELF FIX CLICKING
        //
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        // Count neighbors methods
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if(xOffset == 0 && yOffset == 0) { continue; }

                    if(xCheck < 0) { continue; }

                    if(yCheck < 0) { continue; }

                    if(xCheck >= xLen) { continue; }

                    if(yCheck >= yLen) { continue; }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if(xOffset == 0 && yOffset == 0) { continue; }

                    if(xCheck < 0) { xCheck = xLen - 1; }

                    if(yCheck < 0) { yCheck = yLen - 1; }

                    if(xCheck >= xLen) { xCheck = 0; }

                    if(yCheck >= yLen) { yCheck = 0; }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        // Buttons are all below

        // The start pause and next generation buttons function
        //Start
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }
        //Pause
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        //Next generation
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        // Start
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }
        //Pause
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        // Next generation
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        // Exit button
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        // News the file in the tab
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int y = 0; y < universe.GetLength(1); y++)
            {
                for(int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }
        // News the file throught the button
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // Mode buttons
        // toroidal
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            finiteToolStripMenuItem.Checked = false;
            toroidalToolStripMenuItem.Checked = true;
            mode = 0;
        }
        //finite
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toroidalToolStripMenuItem.Checked = false;
            finiteToolStripMenuItem.Checked = true;
            mode = 1;
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(neighborCountToolStripMenuItem.Checked == true)
            {
                neighborCountToolStripMenuItem.Checked = false;
                viewNum = 1;
            }
            else
            {
                neighborCountToolStripMenuItem.Checked = true;
                viewNum = 0;
            }
        }
    }

}
