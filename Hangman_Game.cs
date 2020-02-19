//**********************************************************************************
//Austin Klevgaard - Hangman Game
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GDIDrawer;
using UtilityLibrary;
using System.IO;

namespace AKlevgaard_Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            //loop to run the game within
            do
            {
                //clears the console each loop
                Console.Clear();
                //displays program title
                Console.WriteLine("\t\t\tAustin Klevgaard - Hangman Game");

                CDrawer canvas = new CDrawer();         //canvas used to draw the hangman game
                string guess;                           //user's current guess
                string[] guessResult = new string[0];   //array to hold the current correctly guesses letters
                string[] usedLetters = new string[26];  //array to hold already guessed letters
                string[] secretWord = new string[0];    //array to hold to randomly chosen secret word
                int uniqueGuess = 0;                    //count of current unique guesses
                bool gameOn = true;                     //bool value that is true as long as the current game is in play
                int winCond;                            //integer equal with value of secret word length. Player wins when winCond = 0
                Random ranGen = new Random();           //random number generator to select secret word
                int wrongCounter = 0;                   //counts the number of unique incorrect user guesses
                int xPos = 0;                           //xposition to draw the secret word letters to the canvas
                int xPosition = 0;

                
                //gets the secret word from words file
                GetSecretWord(ref secretWord, ranGen);

                //sets the win condition
                winCond = secretWord.Length;

                //***********************************************************************************
                //Creates the starting screen for the hangman game
                //***********************************************************************************

                //draws the game setup
                DrawScreen(canvas, ref wrongCounter, ref winCond, ref secretWord, ref guessResult,ref usedLetters, ref gameOn, ref xPosition);

                //writes the hidden secretword to the canvas
                guessResult = new string[secretWord.Length];
                for (int index = 0; index < guessResult.Length; index++)
                {
                    guessResult[index] = "?";
                }

                //***********************************************************************************
                //Runs the game
                //***********************************************************************************

                //loop to update game conditions
                while (gameOn)
                {
                    //clears the canvas of old data
                    canvas.Clear();
                    //re-draws the game setup
                    DrawScreen(canvas, ref wrongCounter, ref winCond, ref secretWord, ref guessResult, ref usedLetters, ref gameOn, ref xPosition);

                    //prints the hidden secretword to the canvas with current correct guesses
                    xPos = 225;
                    for (int index = 0; index < guessResult.Length; index++)
                    {
                        canvas.AddText(guessResult[index], 50, xPos, 500, 50, 100, Color.Orange);
                        xPos += 50;                       
                    }

                    //gets and checks user guesses as long as user hasn't won or guessed 6 times
                    if (winCond > 0) 
                    {
                        if (wrongCounter < 6)
                        {
                            //inputs guess from user
                            GetGuess(out guess, ref usedLetters);
                            //checks the user's guess against the secret word
                            CheckGuess(ref secretWord, guess, ref guessResult, ref usedLetters, ref wrongCounter, ref uniqueGuess, ref winCond);
                        }
                    }
                }
                //lets the user play again or quit
            } while (Utility.YesNO("\nPlay again? ") == "yes");
        }
        //***********************************************************************************
        //Method: static private void DrawScreen(CDrawer canvas, ref int wrongCounter, ref int WinCond, ref string[] secretWord, ref string[] guessResult, ref bool gameOn)
        //Purpose: Controls all elements that will appear in GDI drawer and controls game running loop
        //parameters: (described in main)
        //***********************************************************************************
        static private void DrawScreen(CDrawer canvas, ref int wrongCounter, ref int WinCond, ref string[] secretWord, ref string[] guessResult, ref string[] usedLetters, ref bool gameOn, ref int xPosition)
        {
            xPosition = 195;  //incremental x position to draw the letters that have already been used

            //***********************************************************************************
            //Draw the gallows
            //***********************************************************************************

            //draws the rope
            canvas.AddLine(400, 200, 400, 250, Color.Red, 2);

            //main vertical scaffold beam
            canvas.AddLine(300, 175, 300, 450, Color.DarkOrange, 3);

            //scaffold arm and supporting strut
            //arm
            canvas.AddLine(300, 200, 425, 200, Color.DarkOrange, 3);
            //support
            canvas.AddLine(300, 225, 325, 200, Color.DarkOrange, 3);

            //scaffold floor
            canvas.AddLine(300, 400, 450, 400, Color.DarkOrange, 3);

            //front support leg
            canvas.AddLine(425, 400, 425, 450,Color.DarkOrange,3);

            //floor supporting struts
            canvas.AddLine(300, 440, 425, 410, Color.DarkOrange, 3);
            canvas.AddLine(300, 410, 425, 440, Color.DarkOrange, 3);

            //***********************************************************************************
            //Draws the hangman if answers are wrong
            //***********************************************************************************
            if (wrongCounter >= 1)
            {
                //draws the head
                canvas.AddCenteredEllipse(400, 250, 50, 50, Color.White);
            }
            if (wrongCounter >= 2)
            {
                //draws torso
                canvas.AddCenteredEllipse(400, 300, 50, 100, Color.Blue);
            }
            if (wrongCounter >= 3)
            {
                //draws arm
                canvas.AddLine(380, 290, 350, 320, Color.Blue,5);
            }
            if (wrongCounter >= 4)
            {
                //draws arm 2
                canvas.AddLine(420, 290, 450, 320, Color.Blue, 5);
            }
            if (wrongCounter >= 5)
            {
                //draws leg 1
                canvas.AddLine(410, 330, 420, 390, Color.Green,5);
            }
            if (wrongCounter >= 6)
            {
                //draws leg 2
                canvas.AddLine(390, 330, 380, 390, Color.Green, 5);
            }

            //***********************************************************************************
            //Draws currently used letters and win / loss results 
            //***********************************************************************************

            //text to show the currently used letters
            canvas.AddText($"Letters used: ",20,100,50,400,75,Color.White);
           
            for (int i = 0; i <= usedLetters.Length - 1; i++)
            {
                canvas.AddText($"{usedLetters[i]}", 20, xPosition, 50, 400, 75, Color.White);
                xPosition += 20;
            }

            //Displays result if user wins
            if (WinCond == 0)
            {   
                //win text
                canvas.AddText("You Win!", 50, Color.Yellow);
                gameOn = false;
            }

            //Displays result if user loses
            else if ((WinCond > 0) && (wrongCounter == 6))
            {
                //displays the full secret word 
                for (int index = 0; index <= secretWord.Length-1; index++)
                {
                    guessResult[index] = secretWord[index];
                }
                //loss text
                canvas.AddText("You Lose!", 50, Color.Gray);
                gameOn = false;
            }
        }
        //***********************************************************************************
        //Method: private static void GetSecretWord( ref string[] secretWord, Random ranGen)
        //Purpose: Randomly chooses a secret word from the words file
        //Parameters:   ref string[] secretWord - array of all the letters in the secret word
        //              ranGen - random generator to choose word
        //***********************************************************************************
        private static void GetSecretWord( ref string[] secretWord, Random ranGen)
        {
            StreamReader srName; ;                      //streamreader used to select random word for hangman game
            string word = "";                           //randomly selected word from file for game
            string[] fullList;                          //array to contain all the values possible from the words file
            int i = 0;                                  //index value used to turn random word into a string array
            int lineNumber = 0;                         //integer to hold the number of lines in text file

            //***********************************************************************************
            //Chooses a random word from words file and turns it into a string array
            //***********************************************************************************
            
            //tries to open the file, throws and exception if it fails
            try
            {
                //creates a new streamRader to look at the words file
                srName = new StreamReader("words.txt");

                //***********************************************************************************
                //Selects a random word from the file
                //***********************************************************************************
                try
                {
                    while ((srName.ReadLine() )!= null)
                    {
                        lineNumber++;
                    }
                    srName.Close();

                    fullList = new string[lineNumber];

                    //creates a new streamRader to look at the words file
                    srName = new StreamReader("words.txt");

                    //makes an array populated with all the words in the text file                  
                    while ((word = srName.ReadLine()) != null)
                    {
                        fullList[i] = word;                       
                        i++;
                    }
                    srName.Close();

                    //chooses a random word from the array of possible words
                    word = fullList[ranGen.Next(0, lineNumber)];

                    //turns the chosen word into the secret word array
                    secretWord = new string[word.Length];

                    for (int index = 0; index <= word.Length-1; index++)
                    {
                        secretWord[index] = word[index].ToString();                       
                    }
                }
                //catches an errors when reading the file and displays error message
                catch (Exception errRead)
                {
                    Console.WriteLine($"Error reading the file: {errRead.Message}");
                    
                }
            }
            //catches opening exceptions and displays error message
            catch (Exception errOpen)
            {
                Console.WriteLine($"\nError opening the file: {errOpen.Message}");             
            }
        }
        //***********************************************************************************
        //Method: private static string GetGuess(out string guess, ref string[] usedLetters)
        //Purpose: recieve the user's guess of a letter
        //parameters:   string guess - user entered letter to guess
        //              string[] used letter - array of already guesses letters
        //***********************************************************************************
        private static string GetGuess(out string guess, ref string[] usedLetters)
        {   
            int count = 0;                //counter for guess syntax errors

            //***********************************************************************************
            //Loop to enforce that guesses are made in the right syntax
            //***********************************************************************************
            do
            {
                //counts errors in the guess format
                count = 0;

                //asks and recieves guess input from user
                Console.Write("\nPlease enter a letter to guess: ");
                guess = Console.ReadLine().ToLower();

                //tells user in console if they have repeated a guessed letter
                foreach (string c in usedLetters)
                {
                    if (guess == c)
                    {
                        Console.WriteLine("\nYou have already guessed that number. Guess another.");
                        count++;
                    }
                }
                    //checks to ensure that user guess is only one character
                    if ((guess.Length > 1) || (guess.Length < 1))
                {
                    Console.WriteLine("\nYou may guess a single letter at a time.");
                    count++;
                }
                //checks to make sure the guess is a letter
                else
                { 
                    foreach (char c in guess)
                    {
                        //throws error if input is not a letter
                        if (!(char.IsLetter(c)))
                        {
                            Console.WriteLine("\nYour guess must be a letter.");
                            count++;
                        }
                            
                        //throws an error if guess is a number
                        if (char.IsDigit(c))
                        {
                            Console.WriteLine("\nYour guess must be a letter. No numbers.");
                            count++;
                        }
                        //throws an error if guess is a symbol
                        else if (char.IsSymbol(c))
                        {
                            Console.WriteLine("\nYour guess can't be a special character.");
                            count++;
                        }
                        //throws an error if guess is whitespace
                        else if (char.IsWhiteSpace(c))
                        {
                            Console.WriteLine("\nYou must enter a letter.");
                            count++;
                        }                   
                    }                    
                }                               
            } while (count > 0);

            //returns proper guess to main
            return guess;
        }
        //***********************************************************************************
        //Method: static private void CheckGuess(ref string[] secretWord, string guess, ref string[] guessResult, ref string[] usedLetters, ref int wrongCounter, ref int uniqueGuess, ref int winCond) 
        //Purpose: Checks the user's guessed letter against the secret word to see if letters match
        //Parameters: (See main program for descriptions)
        //***********************************************************************************
        static private void CheckGuess(ref string[] secretWord, string guess, ref string[] guessResult, ref string[] usedLetters, ref int wrongCounter, ref int uniqueGuess, ref int winCond) 
        {            
            int correctletter = 0;
            
            //***********************************************************************************
            //Compares the secret word to the user guess, and changes the result display if it matches a letter in secret word
            //***********************************************************************************
            for (int i = 0; i <= secretWord.Length - 1; i++)
            {
                if (secretWord[i] == guess)
                {
                    guessResult[i] = guess;
                    correctletter++;
                    winCond--;
                }
            }

            //***********************************************************************************
            //Makes a array to list all currently guessed letters. If wrong letter increases wrongCounter
            //***********************************************************************************

            //adds the guess into the used letters array and increments 
            if (correctletter > 0)
            { 
                usedLetters[uniqueGuess] = guess;
                uniqueGuess++;
            }

            //adds the guess into the used letters array and increments usedLetter index and wrong guess count
            if (correctletter == 0)
            {
                usedLetters[uniqueGuess] = guess;
                uniqueGuess++;
                wrongCounter++;
            }          
        }
    }
}
