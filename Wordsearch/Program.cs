using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordsearch
{
    class Wordsearch
    {
        static void Main(string[] args)
        {
            //Reading in the file choice.
            Word[] words;
            GridSize grids;
            Board[,] grid;

            string aroundAgain;

            do
            {
                int wordsFound = 0;
                int[] playerChoiceValues = { -1, -1, -1, -1 };

                Console.Clear();

                try
                {
                    //Loading the board the player chooses.  With exceptions for incorect input.
                    GetAFile(args[0], out words, out grids);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("This is an invalid file!");
                    Console.WriteLine(exception.Message);
                    Console.WriteLine("Please press any key to exit");
                    Console.ReadKey();
                    return;
                }

                Console.Clear();

                //Makes row and column = the value of the rows and columns in the choice.
                grid = new Board[grids.rows, grids.columns];

                Random rng = new Random();
                char[] alphabet = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                                 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'y', 'z'};

                //Creates grid of random letters.
                for (int i = 0; i < grids.rows; i++)
                {
                    for (int j = 0; j < grids.columns; j++)
                    {
                        int randomNumber = rng.Next(0, 25);
                        grid[i, j].letter = alphabet[randomNumber];
                        grid[i, j].colour = Colour.white;
                    }
                }

                //Creating the grid.
                for (int i = 0; i < grids.noWords; i++)
                {
                    try
                    {
                        GridWithWordsFoundColour(i, ref grid, words);
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        System.ArgumentException exception = new System.ArgumentException("This word is outside the bounds of the grid (" + words[i].word + ") consider co ordinates and direction");
                        Console.WriteLine("This is an invalid file!");
                        Console.WriteLine(exception.Message);
                        Console.WriteLine("Please press any key to exit");
                        Console.ReadKey();
                        return;
                    }

                }

                do
                {
                    Console.Clear();

                    DrawGrid(grid, playerChoiceValues, words, grids);

                    //Writes the words and number of words that have to be found.
                    Console.WriteLine();
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (i == 0)
                        {
                            if (words.Length - wordsFound == 1)
                            {
                                Console.WriteLine("You have " + (words.Length - wordsFound) + " word to find!");
                            }
                            else if (words.Length - wordsFound > 1)
                            {
                                Console.WriteLine("You have " + (words.Length - wordsFound) + " words to find!");
                            }
                        }
                        if (!words[i].wordFound)
                        {
                            Console.WriteLine((i + 1) + " - " + words[i].word);
                        }
                    }
                    //Writes the instructions for formatting.
                    Console.WriteLine();
                    Console.WriteLine("Please enter the starting row and column followed by the end row and column");
                    Console.WriteLine("in the format (row1,column1,row2,column2)");
                    Console.WriteLine();

                    //Getting the player input for te wordsearch.
                    PlayerChoice(ref wordsFound, out playerChoiceValues, ref grid, words, grids);

                } while (wordsFound != grids.noWords);

                //When the user has found all words it gives them a congratulations.
                Console.Clear();
                DrawGrid(grid, playerChoiceValues, words, grids);
                Console.WriteLine();
                Console.WriteLine("Well done! You have found all the words!");


                //Asks if the player wants to play again.
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Want to play again? (Y or N).");
                    aroundAgain = Console.ReadLine().ToUpper();
                } while (aroundAgain != "Y" && aroundAgain != "N" && aroundAgain != "YES");
            } while (aroundAgain == "Y" || aroundAgain == "YES");
        }

        enum Direction { up, down, left, right, leftup, rightup, leftdown, rightdown }
        enum Colour { red, green, white, overlapping }

        struct GridSize
        {
            public int rows;
            public int columns;
            public int noWords;
        }

        struct Word
        {
            public string word;
            public int coOrdRow;
            public int coOrdColumn;
            public Direction direction;
            public bool wordFound;
            public int coOrdRowEnd;
            public int coOrdColumnEnd;
        }

        struct Board
        {
            public char letter;
            public Colour colour;
        }

        /// <summary>
        /// Gets the direction from the string.
        /// </summary>
        /// <param name="pData">The string that the direction if contained in.</param>
        /// <returns>direction.</returns>
        static Direction GetDirection(string pData)
        {
            switch (pData)
            {
                case "up":
                    return Direction.up;

                case "down":
                    return Direction.down;

                case "left":
                    return Direction.left;

                case "right":
                    return Direction.right;

                case "leftup":
                    return Direction.leftup;

                case "rightup":
                    return Direction.rightup;

                case "leftdown":
                    return Direction.leftdown;

                case "rightdown":
                    return Direction.rightdown;

                default:
                    throw new Exception("This direction is invalid (" + pData + ")");
            }
        }

        /// <summary>
        /// This gets the file from the user.
        /// </summary>
        /// <param name="pArgs">This is the command line arg passed to the program.</param>
        /// <param name="pWords">Allows the use of the Word struct.</param>
        /// <param name="pGrids">Allows the use of the GridSize.</param>
        static void GetAFile(string pArgs, out Word[] pWords, out GridSize pGrids)
        {
            //Aslong as the file isnt a text file the user will be asked to enter a text file.
            while (pArgs.EndsWith(".txt") == false)
            {
                Console.WriteLine("Please enter a txt file.");
                pArgs = Console.ReadLine();
                Console.Clear();
            }

            string[] boardChoice = File.ReadAllLines(pArgs);

            //Creating a new board of 1 less than boardChoice to cater for not having the grid size in the Word struct.
            pWords = new Word[boardChoice.Length - 1];

            //GridSize will always only be 1 in length since the grid size is always only on 1 line.
            pGrids = new GridSize();

            //Putting the information from the selected board into the structs.
            for (int i = 0; i < boardChoice.Length; i++)
            {
                string[] data = boardChoice[i].Split(',');

                //Loading the grid size and number of words.
                switch (i)
                {
                    //This loads the grid.
                    case 0:

                        pGrids.columns = int.Parse(data[0]);
                        pGrids.rows = int.Parse(data[1]);

                        //Checks if the number of words is equal to the actual number of words.
                        if (int.Parse(data[2]) != boardChoice.Length - 1)
                        {
                            throw new Exception("The number of words has to be the same as the ammount of words.");
                        }
                        pGrids.noWords = int.Parse(data[2]);
                        break;

                    //Loading the words etc.
                    default:

                        pWords[i - 1].word = data[0].ToLower();
                        pWords[i - 1].coOrdColumn = int.Parse(data[1]);
                        pWords[i - 1].coOrdRow = int.Parse(data[2]);
                        pWords[i - 1].direction = GetDirection(data[3].ToLower());
                        pWords[i - 1].wordFound = false;
                        break;
                }
            }
        }

        /// <summary>
        /// This creates the grid with the correct words and word found colours.
        /// </summary>
        /// <param name="pI">The value of i.</param>
        /// <param name="pGrid">This is the grid of random numbers itself.</param>
        /// <param name="pWords">Allows the use of the Word struct.</param>
        static void GridWithWordsFoundColour(int pI, ref Board[,] pGrid, Word[] pWords)
        {
            //populating the grid with the correct words and generating the end co ords of all the words.
            switch (pWords[pI].direction)
            {
                case Direction.right:

                    for (int j = 0; j < pWords[pI].word.Length; j++)
                    {
                        pGrid[pWords[pI].coOrdRow, pWords[pI].coOrdColumn + j].letter = pWords[pI].word[j];

                        if (pWords[pI].wordFound)
                        {
                            pGrid[pWords[pI].coOrdRow, pWords[pI].coOrdColumn + j].colour = Colour.green;
                        }
                    }
                    pWords[pI].coOrdRowEnd = pWords[pI].coOrdRow;
                    pWords[pI].coOrdColumnEnd = pWords[pI].coOrdColumn + pWords[pI].word.Length - 1;
                    break;

                case Direction.left:

                    for (int j = 0; j < pWords[pI].word.Length; j++)
                    {
                        pGrid[pWords[pI].coOrdRow, pWords[pI].coOrdColumn - j].letter = pWords[pI].word[j];

                        if (pWords[pI].wordFound)
                        {
                            pGrid[pWords[pI].coOrdRow, pWords[pI].coOrdColumn - j].colour = Colour.green;
                        }
                    }
                    pWords[pI].coOrdRowEnd = pWords[pI].coOrdRow;
                    pWords[pI].coOrdColumnEnd = pWords[pI].coOrdColumn - pWords[pI].word.Length + 1;
                    break;

                case Direction.up:

                    for (int j = 0; j < pWords[pI].word.Length; j++)
                    {
                        pGrid[pWords[pI].coOrdRow - j, pWords[pI].coOrdColumn].letter = pWords[pI].word[j];

                        if (pWords[pI].wordFound)
                        {
                            pGrid[pWords[pI].coOrdRow - j, pWords[pI].coOrdColumn].colour = Colour.green;
                        }
                    }
                    pWords[pI].coOrdRowEnd = pWords[pI].coOrdRow - pWords[pI].word.Length + 1;
                    pWords[pI].coOrdColumnEnd = pWords[pI].coOrdColumn;
                    break;

                case Direction.down:

                    for (int j = 0; j < pWords[pI].word.Length; j++)
                    {
                        pGrid[pWords[pI].coOrdRow + j, pWords[pI].coOrdColumn].letter = pWords[pI].word[j];

                        if (pWords[pI].wordFound)
                        {
                            pGrid[pWords[pI].coOrdRow + j, pWords[pI].coOrdColumn].colour = Colour.green;
                        }
                    }
                    pWords[pI].coOrdRowEnd = pWords[pI].coOrdRow + pWords[pI].word.Length - 1;
                    pWords[pI].coOrdColumnEnd = pWords[pI].coOrdColumn;
                    break;

                case Direction.rightup:

                    for (int j = 0; j < pWords[pI].word.Length; j++)
                    {
                        pGrid[pWords[pI].coOrdRow - j, pWords[pI].coOrdColumn + j].letter = pWords[pI].word[j];

                        if (pWords[pI].wordFound)
                        {
                            pGrid[pWords[pI].coOrdRow - j, pWords[pI].coOrdColumn + j].colour = Colour.green;
                        }
                    }
                    pWords[pI].coOrdRowEnd = pWords[pI].coOrdRow - pWords[pI].word.Length + 1;
                    pWords[pI].coOrdColumnEnd = pWords[pI].coOrdColumn + pWords[pI].word.Length - 1;
                    break;

                case Direction.leftup:

                    for (int j = 0; j < pWords[pI].word.Length; j++)
                    {
                        pGrid[pWords[pI].coOrdRow - j, pWords[pI].coOrdColumn - j].letter = pWords[pI].word[j];

                        if (pWords[pI].wordFound)
                        {
                            pGrid[pWords[pI].coOrdRow - j, pWords[pI].coOrdColumn - j].colour = Colour.green;
                        }
                    }
                    pWords[pI].coOrdRowEnd = pWords[pI].coOrdRow - pWords[pI].word.Length + 1;
                    pWords[pI].coOrdColumnEnd = pWords[pI].coOrdColumn - pWords[pI].word.Length + 1;
                    break;

                case Direction.rightdown:

                    for (int j = 0; j < pWords[pI].word.Length; j++)
                    {
                        pGrid[pWords[pI].coOrdRow + j, pWords[pI].coOrdColumn + j].letter = pWords[pI].word[j];

                        if (pWords[pI].wordFound)
                        {
                            pGrid[pWords[pI].coOrdRow + j, pWords[pI].coOrdColumn + j].colour = Colour.green;
                        }
                    }
                    pWords[pI].coOrdRowEnd = pWords[pI].coOrdRow + pWords[pI].word.Length - 1;
                    pWords[pI].coOrdColumnEnd = pWords[pI].coOrdColumn + pWords[pI].word.Length - 1;
                    break;

                case Direction.leftdown:

                    for (int j = 0; j < pWords[pI].word.Length; j++)
                    {
                        pGrid[pWords[pI].coOrdRow + j, pWords[pI].coOrdColumn - j].letter = pWords[pI].word[j];

                        if (pWords[pI].wordFound)
                        {
                            pGrid[pWords[pI].coOrdRow + j, pWords[pI].coOrdColumn - j].colour = Colour.green;
                        }
                    }
                    pWords[pI].coOrdRowEnd = pWords[pI].coOrdRow + pWords[pI].word.Length - 1;
                    pWords[pI].coOrdColumnEnd = pWords[pI].coOrdColumn - pWords[pI].word.Length + 1;
                    break;
            }
        }

        /// <summary>
        /// This method draws the grid for the wordsearch.
        /// </summary>
        /// <param name="pGrid">This is the grid of random numbers itself.</param>
        /// <param name="pPlayerChoiceValues">These are the players choices.</param>
        /// <param name="pWords">Allows the use of the Word struct.</param>
        /// <param name="pGrids">Allows the use of the GridSize.</param>
        static void DrawGrid(Board[,] pGrid, int[] pPlayerChoiceValues, Word[] pWords, GridSize pGrids)
        {
            //Writing the top numbers abaptable to the number of columns.
            Console.Write("   ");
            Console.ForegroundColor = ConsoleColor.Yellow; //Writing x axis (column) numbers in yellow.
            for (int i = 0; i < pGrids.columns; i++)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            //Writing the grid with correct words.
            for (int i = 0; i < pGrids.rows; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow; //Writing y axis (row) numbers in yellow.
                Console.Write(i + " ");
                Console.ForegroundColor = ConsoleColor.White; //Writing the letters in white.

                for (int j = 0; j < pGrids.columns; j++)
                {
                    if (pGrid[i, j].colour == Colour.red)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (pGrid[i, j].colour == Colour.green)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    if (pGrid[i, j].colour == Colour.overlapping)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(" ");
                    Console.Write(pGrid[i, j].letter);
                    Console.ForegroundColor = ConsoleColor.White;

                    //If the colour is a colour that would give red after the grid is drawn it is switched to either green or null.
                    if (pGrid[i, j].colour == Colour.overlapping)
                    {
                        pGrid[i, j].colour = Colour.green;
                    }
                    else if (pGrid[i, j].colour == Colour.red)
                    {
                        pGrid[i, j].colour = Colour.white;
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Checks the which direction the players choice is and colours the selections red.
        /// </summary>
        /// <param name="pPlayerChoiceValues">These are the players choices.</param>
        /// <param name="pPlayerChoiceDirection">This is the players choice direction.</param>
        /// <param name="pGrid">This is the grid of random numbers itself.</param>
        static void WrongColours(int[] pPlayerChoiceValues, Direction pPlayerChoiceDirection, ref Board[,] pGrid)
        {
            int row1 = pPlayerChoiceValues[0];
            int row2 = pPlayerChoiceValues[2];
            int column1 = pPlayerChoiceValues[1];
            int column2 = pPlayerChoiceValues[3];



            switch (pPlayerChoiceDirection)
            {
                case Direction.left:
                    for (int j = column2; j < column1 + 1; j++)
                    {
                        //If a word is overlapping then change the colour from green to the overlapping colour.
                        if (pGrid[row1, j].colour == Colour.green)
                        {
                            pGrid[row1, j].colour = Colour.overlapping;
                        }
                        else
                        {
                            pGrid[row1, j].colour = Colour.red;
                        }
                    }
                    break;

                case Direction.right:
                    for (int j = column1; j < column2 + 1; j++)
                    {
                        if (pGrid[row1, j].colour == Colour.green)
                        {
                            pGrid[row1, j].colour = Colour.overlapping;
                        }
                        else
                        {
                            pGrid[row1, j].colour = Colour.red;
                        }
                    }
                    break;

                case Direction.up:
                    for (int i = row2; i < row1 + 1; i++)
                    {
                        if (pGrid[i, column1].colour == Colour.green)
                        {
                            pGrid[i, column1].colour = Colour.overlapping;
                        }
                        else
                        {
                            pGrid[i, column1].colour = Colour.red;
                        }
                    }
                    break;

                case Direction.down:
                    for (int i = row1; i < row2 + 1; i++)
                    {
                        if (pGrid[i, column1].colour == Colour.green)
                        {
                            pGrid[i, column1].colour = Colour.overlapping;
                        }
                        else
                        {
                            pGrid[i, column1].colour = Colour.red;
                        }
                    }
                    break;

                case Direction.leftup:
                    for (int j = column1; j >= column2; j--)
                    {

                        if (pGrid[row1, j].colour == Colour.green)
                        {
                            pGrid[row1, j].colour = Colour.overlapping;
                        }
                        else
                        {
                            pGrid[row1, j].colour = Colour.red;
                        }
                        row1--;
                    }
                    break;

                case Direction.rightup:
                    for (int j = column1; j <= column2; j++)
                    {
                        if (pGrid[row1, j].colour == Colour.green)
                        {
                            pGrid[row1, j].colour = Colour.overlapping;
                        }
                        else
                        {
                            pGrid[row1, j].colour = Colour.red;
                        }
                        row1--;
                    }
                    break;

                case Direction.leftdown:
                    for (int j = column1; j <= column2; j++)
                    {
                        if (pGrid[row1, j].colour == Colour.green)
                        {
                            pGrid[row1, j].colour = Colour.overlapping;
                        }
                        else
                        {
                            pGrid[row1, j].colour = Colour.red;
                        }
                        row1--;
                    }
                    break;

                case Direction.rightdown:
                    for (int j = column2; j >= column1; j--)
                    {
                        if (pGrid[row2, j].colour == Colour.green)
                        {
                            pGrid[row2, j].colour = Colour.overlapping;
                        }
                        else
                        {
                            pGrid[row2, j].colour = Colour.red;
                        }
                        row2--;
                    }
                    break;
            }
        }

        /// <summary>
        /// This gets the players choice for the word and the position of the word.
        /// </summary>
        /// <param name="pPlayerChoiceValues">These are the values stored in playerChoice</param>
        /// <param name="pGrids">Allows the use of the GridSize.</param>
        /// <param name="pPlayerChoiceDirection">This outputs the players choice direction.</param>
        static void GetPlayerChoice(out int[] pPlayerChoiceValues, out Direction pPlayerChoiceDirection, GridSize pGrids)
        {
            string playerChoice;
            bool playerChoiceBool = true;
            pPlayerChoiceValues = new int[0];
            pPlayerChoiceDirection = Direction.right;

            while (playerChoiceBool)
            {

                try
                {
                    playerChoice = Console.ReadLine();

                    //This converts the player choice into an array of ints for validation.
                    pPlayerChoiceValues = Array.ConvertAll(playerChoice.Split(','), int.Parse);

                    if (pPlayerChoiceValues.Length > 4 || pPlayerChoiceValues.Length < 4)
                    {
                        throw new Exception("Please enter 4 numbers seperated by commas.");
                    }

                    if (pPlayerChoiceValues[0] < 0 || pPlayerChoiceValues[0] > pGrids.rows || 
                        pPlayerChoiceValues[1] < 0 || pPlayerChoiceValues[1] > pGrids.columns || 
                        pPlayerChoiceValues[2] < 0 || pPlayerChoiceValues[2] > pGrids.rows || 
                        pPlayerChoiceValues[3] < 0 || pPlayerChoiceValues[3] > pGrids.columns)
                    {
                        throw new Exception("Please enter a value within the grid.");
                    }

                    bool direction = true;
                    for (int i = 0; i < pGrids.rows + pGrids.columns; i++)
                    {

                        if (pPlayerChoiceValues[0] == pPlayerChoiceValues[2] && pPlayerChoiceValues[1] == pPlayerChoiceValues[3] + i) //left
                        {
                            pPlayerChoiceDirection = Direction.left;
                            direction = false;
                        }
                        else if (pPlayerChoiceValues[0] == pPlayerChoiceValues[2] && pPlayerChoiceValues[1] == pPlayerChoiceValues[3] - i) //right
                        {
                            pPlayerChoiceDirection = Direction.right;
                            direction = false;
                        }
                        else if (pPlayerChoiceValues[0] == pPlayerChoiceValues[2] + i && pPlayerChoiceValues[1] == pPlayerChoiceValues[3]) //up
                        {
                            pPlayerChoiceDirection = Direction.up;
                            direction = false;
                        }
                        else if (pPlayerChoiceValues[0] == pPlayerChoiceValues[2] - i && pPlayerChoiceValues[1] == pPlayerChoiceValues[3]) //down
                        {
                            pPlayerChoiceDirection = Direction.down;
                            direction = false;
                        }
                        else if (pPlayerChoiceValues[0] == pPlayerChoiceValues[2] + i && pPlayerChoiceValues[1] == pPlayerChoiceValues[3] + i) //left up
                        {
                            pPlayerChoiceDirection = Direction.leftup;
                            direction = false;
                        }
                        else if (pPlayerChoiceValues[0] == pPlayerChoiceValues[2] + i && pPlayerChoiceValues[1] == pPlayerChoiceValues[3] - i) //right up
                        {
                            pPlayerChoiceDirection = Direction.rightup;
                            direction = false;
                        }
                        else if (pPlayerChoiceValues[0] == pPlayerChoiceValues[2] - i && pPlayerChoiceValues[1] == pPlayerChoiceValues[3] + i) //left down
                        {
                            pPlayerChoiceDirection = Direction.leftdown;
                            direction = false;
                        }
                        else if (pPlayerChoiceValues[0] == pPlayerChoiceValues[2] - i && pPlayerChoiceValues[1] == pPlayerChoiceValues[3] - i) //right down
                        {
                            pPlayerChoiceDirection = Direction.rightdown;
                            direction = false;
                        }
                    }
                    if (direction)
                    {
                        throw new Exception("Co ordinates do not follow a correct direction.");
                    }

                    playerChoiceBool = false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine();
                    Console.WriteLine(exception.Message);
                    Console.WriteLine("Please enter valid co ordinates.");
                }
            }
        }

        /// <summary>
        /// This is where the player enters their choice.
        /// </summary>
        /// <param name="wordsFound">This is the number of words that have been found.</param>
        /// <param name="playerChoiceValues">These are the players choice values.</param>
        /// <param name="pGrid">This is the grid of random numbers itself.</param>
        /// <param name="pWords">Allows the use of the Word struct.</param>
        /// <param name="pGrids">Allows the use of the GridSize.</param>
        static void PlayerChoice(ref int wordsFound, out int[] playerChoiceValues, ref Board[,] pGrid, Word[] pWords, GridSize pGrids)
        {
            int wordChoice = 0;
            Direction playerChoiceDirection;
            bool wordFound = false;

            //This gets the players choice.
            GetPlayerChoice(out playerChoiceValues, out playerChoiceDirection, pGrids);

            for (int i = 0; i < pWords.Length; i++)
            {
                if (playerChoiceValues[0] == pWords[i].coOrdRow && playerChoiceValues[1] == pWords[i].coOrdColumn)
                {
                    if (playerChoiceValues[2] == pWords[i].coOrdRowEnd && playerChoiceValues[3] == pWords[i].coOrdColumnEnd)
                    {
                        wordFound = true;
                        wordChoice = i;
                    }
                }
            }

            //This decides if the player chooses the correct word.
            if (wordFound)
            {
                //This tells the user they have already found the word.
                if (pWords[wordChoice].wordFound)
                {
                    for (int i = 0; i < pGrids.noWords; i++)
                    {
                        GridWithWordsFoundColour(i, ref pGrid, pWords);
                    }

                    Console.WriteLine("You have already found " + pWords[wordChoice].word + "!");
                }

                //This tells the user that they have found a correct word.
                if (!pWords[wordChoice].wordFound)
                {
                    pWords[wordChoice].wordFound = true;
                    wordsFound++;

                    for (int i = 0; i < pGrids.noWords; i++)
                    {
                        GridWithWordsFoundColour(i, ref pGrid, pWords);
                    }

                    Console.WriteLine("Congratulations! You have found " + pWords[wordChoice].word);
                }

                //This tells the user how many words there are left to find.
                if (pGrids.noWords - wordsFound == 1)
                {
                    Console.WriteLine("You have " + (pGrids.noWords - wordsFound) + " Word left to find!");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                }
                else if (pGrids.noWords - wordsFound > 1)
                {
                    Console.WriteLine("You have " + (pGrids.noWords - wordsFound) + " Words left to find!");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                }
            }

            //If the user enters an input that doesn't match the if statement then the word is incorrect and so, displays this.
            else
            {
                WrongColours(playerChoiceValues, playerChoiceDirection, ref pGrid);
                Console.WriteLine("That is not a correct word!");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }
    }
}