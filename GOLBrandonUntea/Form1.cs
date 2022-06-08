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

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color grid10Color = Color.Black;

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
                    if (toroidalToolStripMenuItem.Checked == true)
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

            // A Brush for drawing the grid x 10 lines (color, width)
            Pen grid10Pen = new Pen(grid10Color, 3.5F);

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
                    if (gridToolStripMenuItem.Checked == true)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                        // Draws the grid with 10 cells in each square
                        e.Graphics.DrawRectangle(grid10Pen, (cellRect.X)*10, 10*(cellRect.Y), (cellRect.Width)*10, (cellRect.Height)*10);
                    }

                    // Adds numbers if on 
                    // This is then used to center the text in the middle of the rectangle
                    if(neighborCountToolStripMenuItem.Checked == true)
                    {
                        Font font = new Font("Arial", 20f);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        int neighbors;
                        if (toroidalToolStripMenuItem.Checked == true) { neighbors = CountNeighborsToroidal(x, y); }
                        else { neighbors = CountNeighborsFinite(x, y); }

                        // checks to see if there are even any neighbors to not waste time putting 0's
                        if(neighbors == 0) { continue; }

                        bool isLive;
                        isLive = universe[x, y];

                        if (isLive == true)
                        {
                            if(neighbors < 2 || neighbors > 3) { e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat); }
                            else { e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat); }
                        }

                        if (isLive == false)
                        {
                            if(neighbors != 3) { e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat); }
                            else { e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat); }
                        }
                        
                    }
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        // Clicking
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                float x = (float) e.X / cellWidth;
                float y = (float)e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        // Count neighbors methods
        #region NeighborsMethods
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            // Goes throughout the universe array
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    // These are the rules for the Finite count Neighbors
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

            // Goes throughout the universe array
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    // Rules for the Toroidal count neighbors rule
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
        #endregion

        // Buttons are all below
        // Top Menu Options below too

        #region ButtonsAndMenu
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
        // Basically clears out the array for a new file
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clears the universe array
            for(int y = 0; y < universe.GetLength(1); y++)
            {
                for(int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            
            // Stops the timeer and sets the genreatios to 0
            timer.Enabled = false;
            generations = 0;
            // Refreshes the graphics panel to show the changes
            graphicsPanel1.Invalidate();
        }
        // News the file throught the button
        // Same functions as the button above this one
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            // Stops the timer and sets the generations to 0
            timer.Enabled = false;
            generations = 0;
            // Refreshes the graphics panel to show the changes
            graphicsPanel1.Invalidate();
        }

        // Mode buttons
        // toroidal
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            finiteToolStripMenuItem.Checked = false;
            toroidalToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();
        }
        //finite
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toroidalToolStripMenuItem.Checked = false;
            finiteToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();
        }
        // View control in the menu
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(neighborCountToolStripMenuItem.Checked == true)
            {
                neighborCountToolStripMenuItem.Checked = false;
                neighborToolStripMenuItem.Checked = false;
            }
            else
            {
                neighborCountToolStripMenuItem.Checked = true;
                neighborToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem.Checked == true)
            {
                gridToolStripMenuItem.Checked = false;
                gridToolStripMenuItem1.Checked = false;
            }
            else
            {
                gridToolStripMenuItem.Checked = true;
                gridColorToolStripMenuItem1.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        // Color control in the menu
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }
        
        // Reset method that basically starts the file over again
        // Like a new button but resets the colors too to bring it to the deafault
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            // Stops the timeer and sets the genreatios to 0
            timer.Enabled = false;
            generations = 0;
            // Sets all the colors back to normal
            gridColor = Color.Black;
            cellColor = Color.Gray;
            grid10Color = Color.Black;
            graphicsPanel1.BackColor = Color.White;
            // Sets all the views back to on
            neighborCountToolStripMenuItem.Checked = true;
            gridToolStripMenuItem.Checked = true;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            // Refreshes the graphics panel to show the changes
            graphicsPanel1.Invalidate();
        }

        private void reloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void gridX10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = grid10Color;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                grid10Color = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        private void gridX10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = grid10Color;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                grid10Color = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        #endregion

        // Context Menu Strip
        // Methods and buttons

        #region ContextMenuStrip
        // View options of the context menu
        private void neighborToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(neighborToolStripMenuItem.Checked == true)
            {
                neighborToolStripMenuItem.Checked = false;
                neighborCountToolStripMenuItem.Checked = false;
            }
            else
            {
                neighborToolStripMenuItem.Checked = true;
                neighborCountToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem1.Checked == true)
            {
                gridToolStripMenuItem1.Checked = false;
                gridToolStripMenuItem.Checked = false;
            }
            else
            {
                gridToolStripMenuItem1.Checked = true;
                gridToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }
        // Color settings of the context menu
        private void backColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;
            if(DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        private void cellColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        private void gridColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }


        #endregion

    }

}
