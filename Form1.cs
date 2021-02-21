// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// PROJECT INFORMATION
// Project :        
// Date:            
// Author:          Austin Klevgaard
// Instructor:      
// Submission Code: 1201_2300_A03
//
// Description:     

// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FILE INFORMATION
// Class:       
// Last Edit:   
// Description: 
//
// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab01_SudukoMatic
{
    public partial class Form1 : Form
    {
        int[,] sudukoArray = new int[9, 9];

        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LoadPuzzleStrip_Click(object sender, EventArgs e)
        {
            
            Directory.GetCurrentDirectory();
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                string filePath = openFileDialog1.FileName;
                string fileName = openFileDialog1.SafeFileName;

                if (LoadPuzzle(filePath))
                {
                    toolStripStatusLabel1.Text = $"Loaded Puzzle: {fileName}";
                }
            }
        }

        private bool LoadPuzzle(string filePath)
        {
            try
            {
                string rawSuduko = System.IO.File.ReadAllText(filePath);

                if (ErrorCheck(rawSuduko))
                {
                    int rowCount = 0;
                    int columnCount = 0;
                    while (rowCount < 9)
                    {
                        foreach (char c in rawSuduko)
                        {
                            if (char.IsDigit(c))
                            {
                                string tempChar = c.ToString();
                                int.TryParse(tempChar, out sudukoArray[rowCount, columnCount]);
                                columnCount++;
                            }
                            if (columnCount == 9)
                            {
                                rowCount++;
                                columnCount = 0;
                            }
                if (ErrorCheck(rawSuduko))
                {
                    int rowCount = 0;
                    int columnCount = 0;
                    while (rowCount < 9)
                    {
                        foreach (char c in rawSuduko)
                        {
                            if (char.IsDigit(c))
                            {
                                string tempChar = c.ToString();
                                int.TryParse(tempChar, out sudukoArray[rowCount, columnCount]);
                                columnCount++;
                            }
                            if (columnCount == 9)
                            {
                                rowCount++;
                                columnCount = 0;
                            }
                        }
                    }

                            
                        }
                    }
                    //prints the puzzle to the form
                    ShowPuzzle(sudukoArray);
                    //enables the form to auto solve the puzzle
                    SolvePuzzleStrip.Enabled = true;
                    return true;
                }

                return false;
            }
                    ShowPuzzle(sudukoArray);
                    return true;
                }

                //puzzle is not correct
                return false;

            }
            catch (Exception errRead)
            {
                toolStripStatusLabel1.Text = $"Error Reading File: {errRead}";
                return false;
            }

        }
        /*
        private bool ErrorCheck(string rawPuzzle)
        {
            char currentChar = '\0';
            char previousChar = '\0';
            int digitCount = 0;
            int commaCount = 0;
            int leftBracketCount = 0;
            int rightBracketCount = 0;
            int rowCount = 0;

            for (int puzzleStrIndex = 0; puzzleStrIndex < rawPuzzle.Length -1; puzzleStrIndex++)
            {
                if (puzzleStrIndex != 0)
                {
                    previousChar = currentChar;
                }
                currentChar = rawPuzzle[puzzleStrIndex];

                //check to ensure proper start conditions are met
                if (puzzleStrIndex == 1)
                {
                    if (!(currentChar.ToString().Equals("[") && previousChar.ToString().Equals("[")))
                    {
                        toolStripStatusLabel1.Text = "Error. Wrong starting brackets."; 
                        return false;
                    }
                }
            }
        }
        */
            
            
        }
        //I can write a better error check I think. The way I want to do it is to set start condtions, and end condtions
        //1.my start conditions will be that current char at index 1 == [ and previous char is also == [
        //2. if that is true, then set row count to 0
        //3.then I will iterate through the string. the sequence should be digit>comma>digit>comma...digit>Rbracket>comma, which will trigger the row count to increment
        //4. from rows index 1 - 7, the sequence will start with just Lbracket>digit>comma>digit>...digit>Rbracket>comma 
        //5.At row 8 I will check to look for the end condition which will be currChar == ] and prevChar == ]
        //6.If it does not follow this sequence it will exit prematurly with an exit code in the tool strip
        //7.If a value is not a valid character then it will exit prematurly
        //8.I need to make sure it ignores escape characters in the string. Perhaps by keeping all allowed characters in a list to compare to?
        private bool ErrorCheck(string rawPuzzle)
        {
            int digitCount = 0; 
            int commaCount= 0;
            int leftBracketCount = 0;
            int rightBracketCount = 0;
            int totalCharCount = 0;
            char previousChar = '\0';
            int digitCount = 0;
            int commaCount = 0;
            int leftBracketCount = 0;
            int rightBracketCount = 0;
            int totalValidCharCount = 0;
            char previousChar = '\0';
            char currentChar = '\0';
            
            //iterate through the string checking for errors
            for (int puzzleStrIndex = 0; puzzleStrIndex < rawPuzzle.Length; puzzleStrIndex++)
            {
                //assigns the last char indexed to the previous char value
                if (puzzleStrIndex != 0) { previousChar = currentChar; }

                //updates currentChar to hold the new index character and adds it to the digit count
                currentChar = rawPuzzle[puzzleStrIndex];
                

                //compare the first two values in the string to ensure that the puzzle template is starte correctly => "[["
                if (puzzleStrIndex == 1)
                {
                    //if the first two characters are not '[' throw an error
                    if (!(rawPuzzle[puzzleStrIndex - 1].Equals('[') && rawPuzzle[puzzleStrIndex].Equals('[')))
                    {
                        toolStripStatusLabel1.Text = "Error: Incorrect row and column designation at template start";
                        return false;
                    }
                }
                if (puzzleStrIndex != rawPuzzle.Length - 1)
                {
                    //checks the middle of the loop to ensure that the template is not ended prematurely. i.e. "]]" doesn't occur before the end
                    if (previousChar.ToString().Equals(']') && currentChar.ToString().Equals(']'))
                    {
                        toolStripStatusLabel1.Text = "Error: Premature end of board template";
                        return false;
                    }
                    /*
                    if (rawPuzzle[puzzleStrIndex - 1].Equals(']') && rawPuzzle[puzzleStrIndex].Equals(']')) //&& (puzzleStrIndex + 1 != rawPuzzle.Length ))
                    {
                        toolStripStatusLabel1.Text = "Error: Premature end of board template";
                        return false;
                    }
                    */
                }
                if (puzzleStrIndex == rawPuzzle.Length - 1)
                {
                    //checks to ensure that the end of loop has two closing brackets => "]]"
                    if (!(rawPuzzle[puzzleStrIndex - 1].Equals(']') && rawPuzzle[puzzleStrIndex].Equals(']')))
                    {
                        toolStripStatusLabel1.Text = "Error: Incorrect row and column designation at template end";
                        return false;
                    }
                }

                if (puzzleStrIndex == rawPuzzle.Length -1)
                {
                    totalCharCount = digitCount + commaCount + leftBracketCount + rightBracketCount;

                    if (digitCount != 81) { toolStripStatusLabel1.Text = "Error: Wrong amount of numbers in template. Is there a invalid char?"; return false; }
                    if (commaCount != 80) { toolStripStatusLabel1.Text = "Error: Wrong amount of commas in template"; return false; }
                    if (leftBracketCount != 10) { toolStripStatusLabel1.Text = "Error: Wrong amount of left brackets in template"; return false; }
                    if (rightBracketCount != 10) { toolStripStatusLabel1.Text = "Error: Wrong amount of right brackets in template"; return false; }
                    if (totalCharCount != 181) { toolStripStatusLabel1.Text = "Error: Wrong amount of chars in template"; return false; }
                }
                //Tallies up the different types of characters that are present in the text file
                if (Char.IsDigit(currentChar)) digitCount++;
                if (currentChar.ToString().Equals(",")) commaCount++;
                if (currentChar.ToString().Equals("[")) leftBracketCount++;
                if (currentChar.ToString().Equals("]")) rightBracketCount++;

            }

            
            //puzzle is good
            return true;
        }
        
                //checks to ensure that the correct number of characters are present in the game template 
                if (puzzleStrIndex == rawPuzzle.Length - 1)
                {
                    //total length should be 181 characters
                    totalValidCharCount = digitCount + commaCount + leftBracketCount + rightBracketCount;
                    if (digitCount != 81) { toolStripStatusLabel1.Text = "Error. Wrong amount of numbers present in template"; return false; }
                    if (commaCount != 80) { toolStripStatusLabel1.Text = "Error. Wrong number of commas present in template"; return false; } 
                    if (leftBracketCount != 10) { toolStripStatusLabel1.Text = "Error. Wrong number of left brackets in template"; return false; }
                    if (rightBracketCount != 10) { toolStripStatusLabel1.Text = "Error. Wrong number of right brackets in template"; return false; }
                    
                    if (totalValidCharCount != 181) { toolStripStatusLabel1.Text = "Error. Wrong number of characters in template. Is there an invalid char?"; return false; }

                    //puzzle is good
                    return true;
                }
                
            }
            //puzzle is not good
            return false;
            
            
        }

        private void ShowPuzzle(int[,] puzzleTemplate)
        {
            //find better way to populate grid
            dataGridView1.Rows.Clear();

            for (int row = 0; row < 1; row++)
            {
                //ataGridView1.Rows.Add();
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(dataGridView1);
                for (int column = 0; column < 9; column++) 
                {                   
                    if (puzzleTemplate[row, column] > 0)

                    {
                        newRow.Cells[column].Value = puzzleTemplate[row, column];
                        dataGridView1.Columns[column].Width = 45;
                        dataGridView1.Columns[column].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                       // dataGridView1.Rows[row].Cells[column].Value = puzzleTemplate[row, column];                      
                    }
                }   
                newRow.Height = dataGridView1.Columns[0].Width;
                dataGridView1.Rows.Add(newRow);
                
            }
            //colorGridSquares();
        }

        private bool SolvePuzzle(int[,] puzzleArray, int row, int column)
        {
            int numberTry = 1;  //number that will be tested in the grid square

            //end condition if the solver has moved through all rows (0-8)
            if (row == 9) return true;

            if (puzzleArray[row, column] > 0)
            {
                //if your are already in the last column, then move to the first column again and the next row
                if (column == 8)
                {
                    if (SolvePuzzle(puzzleArray, row + 1, 0)) return true;
                }
                //if not in the last column then iterate to the next column
                else
                {
                    if (SolvePuzzle(puzzleArray, row, column + 1)) return true;
                }
            }
            else
            {
                //if the current cell has a value of 0, then populate it with a number to try
                for (; numberTry <= 9; numberTry++)
                {
                    //check to see there are no multiple digit errors in the puzzle
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
                        else
                        {
                            if (SolvePuzzle(puzzleArray, row, column + 1)) return true;
                        }

                        //didn't find a valid number to input
                        puzzleArray[row, column] = 0;
                    }
                }
            }
            //exits the recursion branch
            return false;
        }

        private bool CheckValidity(int[,] puzzleArray, int numberTry, int row, int column)
        {
            //checks all numbers in the row and the column for multiples
            for (int irowOrColumn = 0; irowOrColumn < 9; irowOrColumn++)
            {
                if (puzzleArray[row, irowOrColumn] == numberTry) return false;
                if (puzzleArray[irowOrColumn, column] == numberTry) return false;
            }

            //checks the numbers within the grid square for multiples 
            //**after the above row and column check there will only be 4 squares left to check. It is possible to 
            //calculate which squares to check based on the passed in row and column parameters

            //variables to hold the grid coordinates of the 2 calculated positions
            int rowA= 0;
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

        private bool isSolveValid(int[,] puzzleArray)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    if (puzzleArray[row, column] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
            dataGridView1.ColumnCount = 9;  
            dataGridView1.RowCount = 9;
            dataGridView1.RowTemplate.Height = 50;
            dataGridView1.Rows.Clear();
            */
            
            SolvePuzzleStrip.Enabled = false;
            dataGridView1.ColumnCount = 9;
            /*
            dataGridView1.RowTemplate.MinimumHeight = 30;
            dataGridView1.RowTemplate.Resizable = DataGridViewTriState.True;
            dataGridView1.RowTemplate.Height = 100;*/
            dataGridView1.Rows.Add(8);
            colorGridSquares();



        }

        private void SolvePuzzleStrip_Click(object sender, EventArgs e)
        {
            SolvePuzzle(sudukoArray, 0, 0);
            if (isSolveValid(sudukoArray)) toolStripStatusLabel1.Text = "Puzzle solved";
            else toolStripStatusLabel1.Text = "No valid solution to puzzle";
            ShowPuzzle(sudukoArray);

        }

        private void colorGridSquares()
        {

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.LightSkyBlue;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 3; j < 6; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.LightSalmon;
                    dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.LightSalmon;
                }
            }

            for (int i = 3; i < 6; i++)
            {
                for (int j = 3; j < 6; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.LightSkyBlue;
                }
            }
        }
    }
}
