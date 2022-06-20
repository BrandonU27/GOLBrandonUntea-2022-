using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GOLBrandonUntea
{
    public partial class Form1 : Form
    {

        // Default Width and Height
        int originalWidth = Properties.Settings.Default.GridWidth;
        int originalHeight = Properties.Settings.Default.GridHeight;

        // The universe array and scratchPad array
        bool[,] universe;
        bool[,] scratchPad;

        // Drawing colors
        // Gets the settings and looks to see what to set to
        Color gridColor = Properties.Settings.Default.GridColor;
        Color cellColor = Properties.Settings.Default.CellColor;
        Color grid10Color = Properties.Settings.Default.Gridx10Color;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Living cells count
        int livingCells = 0;

        // hub visible setting
        bool hudOn = true;

        // Current Seed
        int currentSeed = 2003;

        public Form1()
        {
            InitializeComponent();

            // Looks into the settings and sees what size to make the array
            universe = new bool[originalWidth, originalHeight];
            scratchPad = new bool[originalWidth, originalHeight];

            // Sets the background color to the settings one
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;

            // Setup the timer
            // Gets the settings and looks to see what to set to
            timer.Interval = Properties.Settings.Default.Milliseconds; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // int to hold the number of counts for a cell
            int count;

            for(int y = 0; y < universe.GetLength(1); y++)
            {
                for(int x = 0; x <universe.GetLength(0); x++)
                {
                    // int count == CountNeighbor
                    // Checks to see which mode button is selected before choosing which method to use ( depends on mode)
                    if (toroidalToolStripMenuItem.Checked == true)
                    {
                        // Toroidal mode
                        count = CountNeighborsToroidal(x,y);
                    }
                    else
                    {
                        // Finite mode
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
                        else if(count != 3) { scratchPad[x, y] = false; }
                    }

                        
                }   

            }

            // Goes throughout the scratchpad to update the cell count at the end of each gen
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (scratchPad[x, y] == true && universe[x, y] == false) { livingCells++; }
                    if (scratchPad[x,y] == false && universe[x,y] == true) { livingCells--; }
                }
            }


            // Copy from scratchPad to universe
            // Make sure to clear the scratchPad after

            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Clears out the scratchPad after the copy
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
                        // Sets the font and format of the number that is going to be added
                        Font font = new Font("Arial", 20f);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        // gets the neighbor count based on the mode its on
                        int neighbors;
                        if (toroidalToolStripMenuItem.Checked == true) { neighbors = CountNeighborsToroidal(x, y); }
                        else { neighbors = CountNeighborsFinite(x, y); }

                        // checks to see if there are even any neighbors to not waste time putting 0's
                        if(neighbors == 0) { continue; }

                        bool isLive;
                        isLive = universe[x, y];

                        // Checks to see if its alive or dead for diffrent rules
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

            Font hudFont = new Font("Arial", 20F);

            // Draws the hud on the grid
            // Makes a string to hold what type to display on the hud
            string BoundType;
            if(finiteToolStripMenuItem.Checked == true) { BoundType = "Finite"; }
            else { BoundType = "Toroidal"; }

            // Creates a point on the bottom left of the screen
            Point hudPoint = new Point(0,(int)((universe.GetLength(1) - 2) * cellHeight));
            // Goes and prints each line of the hud and then updates the pointer so it doesnt appear on top of each other
            if (hudOn)
            {
                e.Graphics.DrawString($"Universe Size: Width=[{originalWidth}, Height={originalHeight}]", hudFont, Brushes.Pink, hudPoint);
                hudPoint = new Point(0, (int)((universe.GetLength(1) - 3) * cellHeight));
                e.Graphics.DrawString($"Bountary type: {BoundType}", hudFont, Brushes.Pink, hudPoint);
                hudPoint = new Point(0, (int)((universe.GetLength(1) - 4) * cellHeight));
                e.Graphics.DrawString($"Cell Count: {livingCells}", hudFont, Brushes.Pink, hudPoint);
                hudPoint = new Point(0, (int)((universe.GetLength(1) - 5) * cellHeight));
                e.Graphics.DrawString($"Generation: {generations}", hudFont, Brushes.Pink, hudPoint);
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

                if(universe[(int)x, (int)y] == true) { livingCells++; }
                else { livingCells--; }

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void Randomize()
        {

            //Random rand = new Random(); Time
            Random rand = new Random();

            // Loops throughout the universe
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate throught the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Call next (0,2)
                    int num = rand.Next(0, 2);
                    // if random == 0 then turn on other then that turn off
                    if(num == 0) { universe[x, y] = true; }
                    else { universe[x, y] = false; }
                }
            }
            graphicsPanel1.Invalidate();
        }

        // Randomize but enters a seed into the var so that seeds stay the same
        private void RandomizeSeed(int seed)
        {
            //Takes a seed for seed
            Random randSeed = new Random(seed);

            // loops throughout the universe
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Generates random number then puts it into the universe
                    int num = randSeed.Next(0, 2);

                    // if the number is 0 then that means the cell is alive
                    if(num == 0){ universe[x, y] = true; }
                    else { universe[x, y] = false; }
                }
            }
            graphicsPanel1.Invalidate();
        }

        // Count neighbors methods
        #region NeighborsMethods

        // The Finite method basically tells the program that the world doesn't wrap around
        // If the cell hits the wall it don't go to the other side
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


        // Toroidal wrapping the world around
        // If a cell goes to one edge of the map it wraps around the to the other side
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

        //Starts the timer
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }
        //Pause stops the timer
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        //Next generation calls next generation without the timer
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        // Starts the timer
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }
        //Pause stops the timer
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        // Next generation calls the next gerneation without the timer
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        // Exit button quits the program
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
            // Changes the cell count to 0
            livingCells = 0;
            // Refreshes the graphics panel to show the changes
            graphicsPanel1.Invalidate();
        }

        // Mode buttons
        // toroidal sets the array to toroidal mode
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If this is clicked it unchecks the other option and checks this one
            finiteToolStripMenuItem.Checked = false;
            toroidalToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();
        }
        //finite sets the array to finite mode
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If this is clicked it unchecks the other option and checks this one
            toroidalToolStripMenuItem.Checked = false;
            finiteToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();
        }
        // View control in the menu

        // Turns on and off the numbers of the array
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(neighborCountToolStripMenuItem.Checked == true)
            {
                // There is a second button for checking neighbors so it also includes turning it off and on
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

        // Turns off and on the main grid of the array on the screen
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem.Checked == true)
            {
                // There is a second button for showing the grid so it also includes turning it off and on
                gridToolStripMenuItem.Checked = false;
                gridToolStripMenuItem1.Checked = false;
            }
            else
            {
                gridToolStripMenuItem.Checked = true;
                gridToolStripMenuItem1.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        // Color control in the menu

        // Sets the back color of the program by prompting a dialog box
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // creates a new color dialog box with pre set colors to pick from
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;
            
            // basically an if statement that says if the user hits ok then the selected color would be used
            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        // Sets the cell color of the program by promting a dialog box
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // creates a new color dialog box with pre set colors to pick from
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;

            // basically an if statement that says if the user hits ok then the selected color would be used
            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        // Sets the grid color by prompting a dialog box
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // creates a new color dialog box with pre set colors to pick from
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;

            // basically an if statement that says if the user hits ok then the selected color would be used
            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }


        // Sets the grid 10 square color by promting a dialog box
        private void gridX10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // creates a new color dialog box with pre set colors to pick from
            ColorDialog dlg = new ColorDialog();
            dlg.Color = grid10Color;

            // basically an if statment that says if the user hits ok then the selected color would be used
            if (DialogResult.OK == dlg.ShowDialog())
            {
                grid10Color = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        // Reset method that basically starts the file over again
        // By reseting the properties
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            // Reading the Properties
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            grid10Color = Properties.Settings.Default.Gridx10Color;

            timer.Interval = Properties.Settings.Default.Milliseconds;
            originalHeight = Properties.Settings.Default.GridHeight;
            originalWidth = Properties.Settings.Default.GridWidth;

            universe = new bool[originalWidth, originalHeight];
            scratchPad = new bool[originalWidth, originalHeight];

            // Setting any extras back to to the default
            timer.Enabled = false;
            generations = 0;
            livingCells = 0;
            graphicsPanel1.Invalidate();
        }

        private void reloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();

            // Reading the Properties
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            grid10Color = Properties.Settings.Default.Gridx10Color;

            timer.Interval = Properties.Settings.Default.Milliseconds;
            originalHeight = Properties.Settings.Default.GridHeight;
            originalWidth = Properties.Settings.Default.GridWidth;

            universe = new bool[originalWidth, originalHeight];
            scratchPad = new bool[originalWidth, originalHeight];

            // Setting any extras back to normal
            livingCells = 0;
            timer.Enabled = false;
            generations = 0;
            graphicsPanel1.Invalidate();
        }

        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Once clicked on will turn the hud on or off
            // switches the bool to off or on also the check mark
            if (hUDToolStripMenuItem.Checked == true)
            {
                hUDToolStripMenuItem.Checked = false;
                hUDToolStripMenuItem1.Checked = false;
                hudOn = false;
            }
            else
            {
                hUDToolStripMenuItem.Checked = true;
                hUDToolStripMenuItem1.Checked = true;
                hudOn = true;
            }
            graphicsPanel1.Invalidate();
        }

        #endregion

        // Context Menu Strip
        // Methods and buttons

        #region ContextMenuStrip
        // View options of the context menu

        // Same as the other view options in the other buttons but located in the context strip
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

        // Same as the other view options in the other buttons but located in the context strip
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

        // Same as the other color button but this one is located in the context strip
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

        // Same as the other color button but this one is located in the context strip
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

        // Same as the other color button but this one is located in the context strip
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

        // Same as the other color button but this one is located in the context strip
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

        // Opens a dialog box to ask the user which generation to run to before stopping
       //
       //   WORK MORE ON THE TO
       //
        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run_To_Dialog dlg = new Run_To_Dialog();
            timer.Enabled = false;
            int endNumber = generations;

            dlg.Number = generations + 1;

            if(DialogResult.OK == dlg.ShowDialog())
            {
                endNumber = dlg.Number;
            }

            for (int i = generations; i < endNumber; i++)
            {
                NextGeneration();
            }
            
        }

        // Option Button that opens a dialog box to change options
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Makes the options form from the existing form
            Options dlg = new Options();

            // Stops the timer when opened
            timer.Enabled = false;

            // Gets all the options its currently on and setting them in the numberUpDowns in the other form
            dlg.Milliseconds = timer.Interval;

            // Sets the dlg to the original so user knows what the current size is
            dlg.Width = universe.GetLength(0);
            dlg.Height = universe.GetLength(1);

            // Sets it so that when the user presses ok then all the information that is changed is applied to the universe and scratchpad
            // also the timer if the user changed anything
            if(DialogResult.OK == dlg.ShowDialog())
            {
                timer.Interval = dlg.Milliseconds;

                // Checks to see if the user changed anything if so then new array is created with the sizes they asked for
                if(dlg.Width != originalWidth || dlg.Height != originalHeight)
                {
                    universe = new bool[dlg.Width, dlg.Height];
                    scratchPad = new bool[dlg.Width, dlg.Height];

                    // Changes the settings value
                    originalWidth = dlg.Width;
                    originalHeight = dlg.Height;
                }
            }

            // Refreshes the graphicspanel so that if there are any changes it shows
            graphicsPanel1.Invalidate();
        }

        private void hUDToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Same as the other button just diffrent location
            // Once clicked on will turn the hud on or off
            // switches the bool to off or on also the check mark
            if (hUDToolStripMenuItem1.Checked == true)
            {
                hUDToolStripMenuItem1.Checked = false;
                hUDToolStripMenuItem.Checked = false;
                hudOn = false;
            }
            else
            {
                hUDToolStripMenuItem1.Checked = true;
                hUDToolStripMenuItem.Checked = true;
                hudOn = true;
            }

            graphicsPanel1.Invalidate();
        }

        #endregion

        // Gets called when the form is closed in anyway
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Updates all the properties when closing the program
            Properties.Settings.Default.BackColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.Gridx10Color = grid10Color;
            Properties.Settings.Default.GridWidth = originalWidth;
            Properties.Settings.Default.GridHeight = originalHeight;
            Properties.Settings.Default.Milliseconds = timer.Interval;

            // Saves all the properties that have been taken in
            Properties.Settings.Default.Save();
        }

        #region OpenAndSavingButtonsAndTabs
        // Save as file button
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine($"!{DateTime.Now}");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if(universe[x,y] == true) { currentRow += 'O'; }
                        else if(universe[x,y] == false) { currentRow += '.'; }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        // Open file button
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if(row[0] == '!') { continue; }
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    maxHeight++;
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    maxWidth = row.Length;
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.

                originalWidth = maxWidth;
                originalHeight = maxHeight;

                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight]; 

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // keeps track of the y Position for the next while loop
                int yPos = 0;
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row[0] == '!') { continue; }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        if(row[xPos] == 'O')
                        {
                            universe[xPos, yPos] = true;
                        }
                        if(row[xPos] == '.')
                        {
                            universe[xPos, yPos] = false;
                        }
                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.
                    }
                    yPos++;
                }
                graphicsPanel1.Invalidate();

                // Close the file.
                reader.Close();

                graphicsPanel1.Invalidate();
            }
        }

        // Open Tab button
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row[0] == '!') { continue; }
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    maxHeight++;
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    maxWidth = row.Length;
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.

                originalWidth = maxWidth;
                originalHeight = maxHeight;

                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight];

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // keeps track of the y Position for the next while loop
                int yPos = 0;
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row[0] == '!') { continue; }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        if (row[xPos] == 'O')
                        {
                            universe[xPos, yPos] = true;
                        }
                        if (row[xPos] == '.')
                        {
                            universe[xPos, yPos] = false;
                        }
                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.
                    }
                    yPos++;
                }
                graphicsPanel1.Invalidate();

                // Close the file.
                reader.Close();

                graphicsPanel1.Invalidate();
            }
        }

        // Save Tab Button
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine($"!{DateTime.Now}");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y] == true) { currentRow += 'O'; }
                        else if (universe[x, y] == false) { currentRow += '.'; }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }
        #endregion


        // Code for all the buttons that involve randomize
        #region RandomizeButtons

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Makes the options form from the existing form
            Seed dlg = new Seed();

            // Stops the timer when opened
            timer.Enabled = false;

            dlg.SeedNum = currentSeed;

            // checks what the user entered in the box and then setting that as the current seed
            // also randomkizes the file too with that seed
            if (DialogResult.OK == dlg.ShowDialog())
            {

                currentSeed = dlg.SeedNum;
                RandomizeSeed(currentSeed);

            }

            // Refreshes the graphicspanel so that if there are any changes it shows
            graphicsPanel1.Invalidate();
        }

        // Gets the current seed
        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Takes the current seed and places it in a method
            RandomizeSeed(currentSeed);
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Uses the regular randomize method that uses the systems time
            Randomize();
        }

        #endregion
    }

}
