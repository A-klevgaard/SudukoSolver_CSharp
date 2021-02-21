// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// PROJECT INFORMATION
// Project :        Lab01 - SudukoMatic   
// Date:            Sept. 26 2020
// Author:          Austin Klevgaard
// Instructor:      Shane Kelemen   
// Submission Code: 1201_2300_L01
//
// Description:     Program to load sukuko files in to either solve yourself or let recursion do the work 

// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FILE INFORMATION
// Class:           CMPE 2300 A01  
// Last Edit:       Sept. 26. 2020
// Description:     Form class file 
//
// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab01_Suduko
{
    public partial class Form1 : Form
    {
        int[,] sudukoArray = new int[9, 9];     //2D array to hold the numbers for the suduko puzzle

        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// When the Load Puzzle button is clicked this activates an open file dialogue which lets the user select a suduko template
        /// file to load. If a correct filepath is supplied this event handler will call the LoadPuzzle() helper function which 
        /// Checks the supplied file for template errors, and if it is a valid file, then it loads the numerical data into the 
        /// suduko array for the user to interact with.
        /// </summary>
        private void LoadPuzzleStrip_Click(object sender, EventArgs e)
        {
            Directory.GetCurrentDirectory();    //sets the directory to the currently chosen one
            //if a valid file has been selected then save the file path and the file name
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                string filePath = openFileDialog1.FileName;
                string fileName = openFileDialog1.SafeFileName;
                
                //prints the chosen file name to the toolstrip for verification
                if (LoadPuzzle(filePath))
                {
                    toolStripStatusLabel1.Text = $"Loaded Puzzle: {fileName}";
                }
            }
        }
        /// <summary>
        /// This function attempts to access a file at a passed location, and then converts the file to one continuous string.
        /// That string will then be subject to a series of error checks to ensure that the puzzle contains a valid template
        /// for the suduko game. If the file passes all the error checks then it's contents will be loaded into the datagridview
        /// </summary>
        /// <param name="filePath">string variable that directs the program to a suduko template to try and load</param>
        /// <returns></returns>
        private bool LoadPuzzle(string filePath)
        {
            //attempts to access the file and throws an error if the file path is not valid
            try
            {
                //converts the contents of the file to a string
                string rawSuduko = System.IO.File.ReadAllText(filePath);

                //checks the supplied file for sukuko template validity
                if (ErrorCheck(rawSuduko))
                {
                    int rowCount = 0;       //row in the puzzle
                    int columnCount = 0;    //column of the puzzle

                    //Iterate through all the rows and columns of the puzzle
                    while (rowCount < 9)
                    {
                        //Each character in the string will be tested one at a time to see if it is a digit
                        foreach (char c in rawSuduko)
                        {
                            //if the character is a digit, it's converted to a string and then an int
                            //I would like to simply cast the char to an int, but the IDE throws me an error when I 
                            //try to do a cast with the iterant, 'c'
                            if (char.IsDigit(c))
                            {
                                string tempChar = c.ToString();
                                int.TryParse(tempChar, out sudukoArray[rowCount, columnCount]);
                                columnCount++;
                            }
                            //if currently passed the end of the puzzle columns then move to the next row
                            if (columnCount == 9)
                            {
                                rowCount++;
                                columnCount = 0;
                            }
                        }
                    }
                    //print the puzzle to the datagridView and enable autosolve 
                    ShowPuzzle(sudukoArray);
                    SolvePuzzleStrip.Enabled = true;
                    return true;
                }

                //puzzle is not correct
                return false;

            }
            //throws an error if the filepath is invalid
            catch (Exception errRead)
            {
                toolStripStatusLabel1.Text = $"Error Reading File: {errRead}";
                return false;
            }

        }
        /// <summary>
        /// Iterates through the string containing the puzzle data and checks it for errors that would break the loading sequence
        /// or would make the puzzle unsolvable. This tries to ensure that the template adhears to a (mostly) consistant format
        /// </summary>
        /// <param name="rawPuzzle">string that holds the entire contents of a puzzle template text file</param>
        /// <returns></returns>
        private bool ErrorCheck(string rawPuzzle)
        {
            int zeroCount = 0;              //count for the amount of digits that are specifically 0 in the string
            int leftBracketCount = 0;       //count for the amount of left brackets in the string
            int rowCharacterCount = 0;      //count for characters in each row
            int rowDigitCount = 0;          //count for digits in each row
            int rowStartCount = 0;          //count for how many row staring characters exist
            int rowEndCount = 0;            //count for how many row ending characters exist

            char previousChar = '\0';       //holds the previously iterated char value
            char currentChar = '\0';        //holds the current char in the string that is being iterated on
            bool templateStartCheck = false;//check value for if template syntax starts correctly
            bool templateEndCheck = false;  //check value for if template syntax ends correctly
            bool correctRowStart = false;   //check value for if template rows starts correctly
            bool correctRowEnd = false;     //check value for if template rows end correctly
            bool correctRowDigits = false;  //check value for if each row contains only correct digits and not other character types


            //iterates through the string to check for errors
            for (int puzzleStrIndex = 0; puzzleStrIndex < rawPuzzle.Length; puzzleStrIndex++)
            {
                //this ensures that we ignore return (\r) and newline (\n) characters,space characters (' ') and commas (',')
                if (!(rawPuzzle[puzzleStrIndex].Equals('\r') || rawPuzzle[puzzleStrIndex].Equals('\n') || rawPuzzle[puzzleStrIndex].Equals(' ') || rawPuzzle[puzzleStrIndex].Equals(','))) 
                {
                    //assigns the last char indexed to the previous char value
                    if (puzzleStrIndex != 0) { previousChar = currentChar; }

                    //updates currentChar to hold the new index character
                    currentChar = rawPuzzle[puzzleStrIndex];

                    //**********************************************************************************************************
                    //Check to ensure correct template start
                    //**********************************************************************************************************
                    //check to ensure that the template starts with the correct format "[["
                    if (templateStartCheck == false)
                    {

                        //checks if the current character isa  '[' to start a row, and checks the next char to ensure it is numerical data
                        if (leftBracketCount == 1)
                        {
                            //checks that the current character is a '[' and the next is a digit
                            if (currentChar.Equals('[') && char.IsDigit(rawPuzzle[puzzleStrIndex + 1]))
                            {
                                leftBracketCount++;
                                templateStartCheck = true;

                            }
                            else { toolStripStatusLabel1.Text = "Error: Incorrect row and column designation at template start"; return false; }
                        }
                        //if leftBacketCount is == 0, then you shouldn't have parsed any data yet
                        if (leftBracketCount == 0)
                        {
                            //checks to ensure the first character in the template is a '['
                            if (currentChar.Equals('[')) { leftBracketCount++; }
                            else toolStripStatusLabel1.Text = "Error: Incorrect row and column designation at template start";
                        }
                    }

                    //**********************************************************************************************************
                    //Check to ensure that each row starts and ends properly, contains the correct amount of digits (including non-digit characters)
                    //********************************************************************************************************** 

                    //checks the beginning of each individual row to ensure that it starts with a '['. If not throws an error message
                    if (correctRowStart == false && puzzleStrIndex > 0)
                    {
                        if (currentChar.Equals('[')) 
                        { 
                            correctRowStart = true;
                            correctRowEnd = false;
                            leftBracketCount++;
                            rowStartCount++;
                            //resets the digitCount for the new row
                            rowDigitCount = 0;
                            rowCharacterCount = 0;
                            correctRowDigits = false;
                        }
                        else { toolStripStatusLabel1.Text = $"Error: Missing '[' at row {rowStartCount} start."; return false; }
                    }
                    //This checks to ensure that each row contains the correct amount of digits and not other characters
                    if (correctRowStart == true && !currentChar.Equals('[') && !currentChar.Equals(']'))
                    {
                        //if the row has not been cleared to have enough digits yet, check the current character
                        if (correctRowDigits == false)
                        {
                            //increment the character count for this row, and check if it is a digit
                            rowCharacterCount++;
                            if (char.IsDigit(currentChar))
                            {
                                //if the character is a digit, then count it as a digit.
                                rowDigitCount++;
                                //check if the digit is a zero. If all the digits in the puzzle are zero, the puzzle is empty and return an error
                                if (currentChar.Equals('0')) 
                                { 
                                    zeroCount++;
                                    if (zeroCount == 81) { toolStripStatusLabel1.Text = $"Error: Puzzle template is empty of suitable values"; return false; }
                                }
                            }

                            //checks for errors with characters and digits within each row
                            if (rowCharacterCount != 9 && rawPuzzle[puzzleStrIndex + 1].Equals(']'))
                            {
                                toolStripStatusLabel1.Text = $"Error: Missing or surplus of column numerical data in row: {rowStartCount} "; return false;
                            }
                            else if (rowCharacterCount == 9 && !rawPuzzle[puzzleStrIndex + 1].Equals(']'))
                            {
                                toolStripStatusLabel1.Text = $"Error: Missing row termination bracket in row: {rowStartCount} "; return false;
                            }
                            else if (rowCharacterCount == 9 && rowDigitCount != 9)
                            {
                                toolStripStatusLabel1.Text = $"Error: Invalid character present in row: {rowStartCount} "; return false;
                            }
                            else if (rowCharacterCount == 9 && rowDigitCount == 9) correctRowDigits = true;

                        }
                    }

                    //**********************************************************************************************************
                    //Check to ensure that the row ends at the correct place
                    //********************************************************************************************************** 
                    if (correctRowEnd == false)
                    {
                        //if at the end of a row, increment the row end count and check for possible errors with template
                        if (currentChar.Equals(']'))
                        { 
                            //increment correct row end count and set correct row end to true
                            rowEndCount++;
                            correctRowEnd = true;

                            //ensures that the puzzle is not missing a row of data
                            if (rowEndCount != 9 && rawPuzzle[puzzleStrIndex +1].Equals(']')) { toolStripStatusLabel1.Text = $"Error: Premature end of template. Error at end of row {rowEndCount}"; return false; }

                            //if not in the last row of the puzzle, reset correctRowStart to false so that it checks the beginning of the next row
                            if (rowEndCount < 9)
                            {
                                correctRowStart = false;

                            }
                        }
                    }
                    //**********************************************************************************************************
                    //Check to ensure that the template ends with the correct characters: ']]'
                    //********************************************************************************************************** 
                    if (templateEndCheck == false)
                    {
                        if (rowEndCount == 9)
                        {
                            //check to ensure that both the current and previous characters are closing brackets
                            if (currentChar.Equals(']') && previousChar.Equals(']'))
                            {
                                //checks to ensure that there is no more character data after the end of the template
                                if (puzzleStrIndex != rawPuzzle.Length - 1)
                                {
                                    toolStripStatusLabel1.Text = "Error: Character data found beyond template terminal characters \"]]\"."; return false;
                                }
                                else
                                {
                                    //there is no more characters beyond the ']]' at the end of the template
                                    templateEndCheck = true;
                                }
                            }
                        }
                        
                    }
                    //if all possible error checks have been cleared, then return true and allow the puzzle to load
                    if (templateStartCheck && templateEndCheck && correctRowStart && correctRowEnd && correctRowDigits)
                    {
                        //puzzle is good to load
                        return true;
                    }               
                }
            }

            //puzzle is not good
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="puzzleTemplate"> passed2D array used to hold the puzzle numbers</param>
        private void ShowPuzzle(int[,] puzzleTemplate)
        {
            //iterates through the rows of the grid
            for (int row = 0; row < 9; row++)
            {
                //iterates through each column in a row
                for (int column = 0; column < 9; column++)
                {
                    //clears any current value that may be in the datagridview cell
                    dataGridView1[column, row].Value = "";

                    //if the suduko template has a valid number for the cell populate it into the grid
                    if (puzzleTemplate[row, column] > 0)
                    {
                         dataGridView1[column,row].Value = puzzleTemplate[row, column];                    
                    }
                }
            }
            //give the grid squares pretty colors
            colorGridSquares();
        }
        /// <summary>
        /// Uses recursive backtracking to find a valid solution to the suduko puzzle
        /// </summary>
        /// <param name="puzzleArray">2D array that is used to hold numerical suduko puzzle data</param>
        /// <param name="row">value for the current row in the 9x9 grid</param>
        /// <param name="column">value for the current column in the 9x9 grid</param>
        /// <returns></returns>
        private bool SolvePuzzle(int[,] puzzleArray, int row, int column)
        {
            int numberTry = 1;  //number that will be tested in the grid square

            //end condition if the solver has moved through all rows (0-8)
            if (row == 9) return true;

            //if the suduko puzzle array already has a value in the current cell that is not 0, then move to another cell
            if (puzzleArray[row, column] > 0)
            {
                //if you are already in the last column, then move to the first column of the next row
                if (column == 8)
                {
                    if (SolvePuzzle(puzzleArray, row + 1, 0)) return true;
                }
                //if not in the last column of the row, then iterate to the next column
                else
                {
                    if (SolvePuzzle(puzzleArray, row, column + 1)) return true;
                }
            }
            else
            {
                //if the current cell in the array has a value of 0, then populate it with a number to try
                for (; numberTry <= 9; numberTry++)
                {
                    //check to ensure that there are no multiple digit errors in the puzzle row, column, and grid square
                    if (CheckValidity(puzzleArray, numberTry, row, column))
                    {
                        //put the number in the array then updates the value in the grid
                        puzzleArray[row, column] = numberTry;
                        dataGridView1.Rows[row].Cells[column].Value = numberTry;
                        

                        //move to either the next column, or if at the end, the next row
                        if (column == 8)
                        {
                            if (SolvePuzzle(puzzleArray, row + 1, 0)) return true;
                        }
                        //if not already in the last column then move one column to the right
                        else
                        {
                            if (SolvePuzzle(puzzleArray, row, column + 1)) return true;
                        }

                        //if the algorithmn did not find a valid number on this recursive branch then put a zero back into the cell
                        puzzleArray[row, column] = 0;
                    }
                }
            }
            //exits the recursion branch
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="puzzleArray">2D array used to hold the template numbers for the suduko puzzle</param>
        /// <param name="numberTry">the current number that is being tested in the cell</param>
        /// <param name="row">the current row that is being tested recursively</param>
        /// <param name="column">the current column that is being tested recursively</param>
        /// <returns></returns>
        private bool CheckValidity(int[,] puzzleArray, int numberTry, int row, int column)
        {
            //checks all numbers in the row and the column for multiples
            for (int irowOrColumn = 0; irowOrColumn < 9; irowOrColumn++)
            {
                //checks the current row and column to see if there are already any instances of the currently tested number present in each
                if (puzzleArray[row, irowOrColumn] == numberTry) return false;
                if (puzzleArray[irowOrColumn, column] == numberTry) return false;
            }

            //checks the numbers within the grid square for multiples 
            //**after the above row and column check there will only be 4 squares left to check. It is possible to 
            //calculate which squares to check based on the passed in row and column parameters

            //variables to hold the grid coordinates of the 2 calculated positions
            int rowA = 0;
            int rowB = 0;
            int columnA = 0;
            int columnB = 0;
            int modColumn = column % 3;
            int modRow = row % 3;
            //calculates which grid rows in the grid square haven't been compared yet
            if (modRow == 0) { rowA = row + 1; rowB = row + 2; }
            if (modRow == 1) { rowA = row - 1; rowB = row + 1; }
            if (modRow == 2) { rowA = row - 2; rowB = row - 1; }
            //calculates which grid columns haven't been compared yet
            if (modColumn == 0) { columnA = column + 1; columnB = column + 2; }
            if (modColumn == 1) { columnA = column - 1; columnB = column + 1; }
            if (modColumn == 2) { columnA = column - 2; columnB = column - 1; }

            //checks the calculated grid squares to see if there are multiple digits
            if (puzzleArray[rowA, columnA] == numberTry) return false;
            if (puzzleArray[rowA, columnB] == numberTry) return false;
            if (puzzleArray[rowB, columnA] == numberTry) return false;
            if (puzzleArray[rowB, columnB] == numberTry) return false;

            //number passed all the checks and it is valid
            return true;
        }
        /// <summary>
        /// Checks the puzzle iteratively to ensure that the recursive solve actually provided a valid solution
        /// </summary>
        /// <param name="puzzleArray">2D array used to hold sukuko puzzle integer values</param>
        /// <returns></returns>
        private bool isSolveValid(int[,] puzzleArray)
        {
            //iterate through all rows and columns in the grid
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    //if there are still any remaining zero's left in the grid then the solve was not valid
                    if (puzzleArray[row, column] == 0)
                    {
                        return false;
                    }
                }
            }
            //solve is valid, be happy
            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //***Citation***//
            /*I will admit that I needed to see an online example for how to setup my datagridview to look like a proper suduko board,
            as I was really struggling to understand which properties I needed to set which way, and there was so many different ways
            that you can work with datagridview that it was hard to know which direction to take without having worked with this control
            before. This solution by user "ddanbe" on the site daniweb.com proved to finally help me set it up how I wanted it to look.
            However, I did not exactly copy his code, but instead I made sure to read through it all to understand what each portion did
            with the datagridview, then I adapted it to suit my own needs and removed the parts of it that were superfluous to for 
            purposes. 

            link to site: https://www.daniweb.com/programming/software-development/code/453445/making-a-datagridview-look-like-a-sudoku-board

            */


            //constant values used to control the setup width and height of the grid. Const values are used here in case the user wants to resize the whole grid
            const int columnWidth = 45;
            const int rowHeight = 45;

            //removes the user's ability to interact with columns and rows. Also cleans up headers so they cannot be seen
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;

            //removes the scrollbar, you don't need it here and it blocks the view of the grid
            dataGridView1.ScrollBars = ScrollBars.None;
            //embiggens the font size so that it fits the cell size better
            dataGridView1.Font = new System.Drawing.Font("Times New Roman", 16F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            //allows the user to select cells, not columns or rows
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;

            //adds 9 columns to the datagridview
            for (int i = 0; i < 9; i++)
            {
                DataGridViewTextBoxColumn newColumn = new DataGridViewTextBoxColumn();
                newColumn.MaxInputLength = 1;   // only allows the user to input 1 digit per cell
                dataGridView1.Columns.Add(newColumn);
                dataGridView1.Columns[i].Name = "Column " + (i + 1).ToString();
                dataGridView1.Columns[i].Width = columnWidth;
                dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                //creates a new datagrid view row and adds it to the datagridview
                DataGridViewRow row = new DataGridViewRow();
                row.Height = rowHeight;
                dataGridView1.Rows.Add(row);
            }

            //this just gives a nice bold divider between each 3x3 grid segment on the suduko board
            dataGridView1.Columns[2].DividerWidth = 2;
            dataGridView1.Columns[5].DividerWidth = 2;
            dataGridView1.Rows[2].DividerHeight = 2;
            dataGridView1.Rows[5].DividerHeight = 2;

            //colors the grid squares properly to make them pretty
            colorGridSquares();
          
        }
        /// <summary>
        /// Calls SolvePuzzle to let backtracking recursion solve the puzzle and updates the toolstrip depending on the result
        /// </summary>
        private void SolvePuzzleStrip_Click(object sender, EventArgs e)
        {
            SolvePuzzle(sudukoArray, 0, 0);
            //if the solve is determined to be valid then update the toolstrip test to let the user know
            if (isSolveValid(sudukoArray)) toolStripStatusLabel1.Text = "Puzzle solved, showing solution";
            //if the solve is not possible then update the user
            else toolStripStatusLabel1.Text = "No valid solution to puzzle";
            //print the puzzle to the grid
            ShowPuzzle(sudukoArray);
        }

        /// <summary>
        /// iterates through each cell in the datagridview and colors the background of the cell
        /// </summary>
        private void colorGridSquares()
        {

            //colors all cells light skyblue
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    dataGridView1.Rows[row].Cells[column].Style.BackColor = Color.LightSkyBlue;
                }
            }
            //colors every "even" 3x3 grid block light salmon
            for (int row = 0; row < 9; row++)
            {
                for (int column = 3; column < 6; column++)
                {
                    dataGridView1.Rows[row].Cells[column].Style.BackColor = Color.LightSalmon;
                    dataGridView1.Rows[column].Cells[row].Style.BackColor = Color.LightSalmon;
                }
            }
            //colors the center3x3 square lightskyblue again
            for (int row = 3; row < 6; row++)
            {
                for (int column = 3; column < 6; column++)
                {
                    dataGridView1.Rows[row].Cells[column].Style.BackColor = Color.LightSkyBlue;
                }
            }
            
        }
    }
}
