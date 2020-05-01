//**********************************************************************************************
//Austin Klevgaard - CMPE 1600 - Lab01
//**********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDIDrawer;
using System.Drawing;
using UtilityLibrary;
using System.IO;

namespace AKlevgaard_CMPE1600_Lab01
{
    class Program
    {
        static void Main(string[] args)
        {
            int userSelect;                     //char used to hold the value of the user selection
            int[] studentGrades = new int[0];   //array to hold generated student grades  
            Random ranGen = new Random();       //random number generator
            CDrawer canvas = new CDrawer();     //canvas used to draw the histogram of marks
            string fileName = "";               //filename of saved or loaded file


            //loop used to continuously run the console program until the user selects quit
            do
            {
                //clears the consle at the start of each loop
                Console.Clear();

                //prints the title of the program to the console
                Console.WriteLine("\t\t\tAustin Klevgaard - CMPE 1600 Lab 01\n");

                //prints the list of commands to the console.
                Console.WriteLine("Actions available:\n");
                Console.WriteLine("1. Create Random Array");
                Console.WriteLine("2. Array Statistics");
                Console.WriteLine("3. Draw Histogram");
                Console.WriteLine("4. Load array from file");
                Console.WriteLine("5. Save array to file");

                //prompts the user to press a key to select a command
                Console.Write("\nYour selection (1-5): ");
                int.TryParse(Console.ReadLine(), out userSelect);


                //performs an operation based on the user input
                switch (userSelect)
                {
                    case 1:
                        //generates the random array
                        GetArray(out studentGrades, ranGen);
                        //prints array contents to the console
                        Console.WriteLine("The current contents of the array are:\n");
                        for (int i = 0; i <= studentGrades.Length - 1; i++)
                        {
                            Console.Write($"{studentGrades[i]} ");
                        }
                        break;
                    //displays array statistics
                    case 2:
                        DisplayArray(ref studentGrades);
                        break;
                    //draws the array histogram in GDI drawer
                    case 3:
                        DrawHistogram(ref studentGrades, ref canvas);
                        break;
                    //Loads a saved array into the console
                    case 4:
                        LoadFile(ref fileName, ref studentGrades);
                        break;
                    //saves the current array into a text file
                    case 5:
                        SaveFile(out fileName, ref studentGrades);
                        break;
                }
                //Pauses screen for viewing
                Utility.Pause("\n\nPress any key to continue...");

            } while (userSelect != 'q');
        }

        //Method: private static int[] GetArray(out int[] randomMarks, Random ranNumber)
        //Purpose: Creates a specified size array of random grades for the user
        //Parameters:   int[] randomMarks - generated array of random values
        //              ranNumber - passed random number generator used to make random values between 0-100
        private static int[] GetArray(out int[] randomMarks, Random ranNumber)
        {
            int arraySize; //user specified size for the array of student grades

            //gets the user to input the size of the array
            Utility.GetValue(out arraySize, "Enter the size of the Array: ", 1, 10000);

            //creates the array of specified size
            randomMarks = new int[arraySize];

            //populates the array with random marks
            for (int i = 0; i < randomMarks.Length; i++)
            {
                randomMarks[i] = ranNumber.Next(1, 101);
            }
            //returns new array to main
            return randomMarks;
        }

        //Method:   private static void DisplayArray(ref int[] studentGrades)
        //Purpose:  Used to calculate and display the statistics of a created array
        //Parameters:   ref int[] studentGrades - array of generated student grades
        private static void DisplayArray(ref int[] studentGrades)
        {
            int minimum;    //minimum value of array
            int maximum;    //maximum value in array
            double average; //calculated average of the array

            //Displays an error message if there is currently no array
            if (studentGrades.Count() == 0)
            {
                Console.WriteLine("Error. No current array values");
                Console.Beep();
            }
            //sorts the array, and gives max, min, and average values
            else
            {
                //sorts the array
                Array.Sort(studentGrades);
                Console.WriteLine("\nThe array has been sorted");
                //displays the sorted array
                Console.WriteLine("\nThe current contents of the array are: ");
                for (int i = 0; i <= studentGrades.Length - 1; i++)
                {
                    Console.Write($"{studentGrades[i]} ");
                }

                //calculates and displays the max min and average of the array
                minimum = studentGrades.Min();
                maximum = studentGrades.Max();
                average = studentGrades.Average();

                Console.WriteLine($"\n\nThe minimum value is {minimum}");
                Console.WriteLine($"\nThe maximum value is {maximum}");
                Console.WriteLine($"\nThe average of the is {average:f2}");
            }
        }

        //Name: private static void DrawHistogram(ref int[] studentGrades, ref CDrawer canvas)
        //Purpose: Uses array data to provide a histogram of student grades for viewing
        //Parameters: studentGrades: referenced array that holds current student grade info
        //canvas: canvas passed from the main program where the histogram will be drawn.
        private static void DrawHistogram(ref int[] studentGrades, ref CDrawer canvas)
        {

            int[] gradeCounter = new int[11];   //array to sort student grades into 11 divisions
            int minGrade = 0;                   //intial minimum conditions used to set up grade category divisions to sort grade array
            int maxGrade = 9;                   //intial maximum conditions used to set up grade category divisions to sort grade array
            int counter = 0;                    //counter used to organize the amount of grades in the student grade array that belong within each division

            //clears canvas of previous data
            canvas.Clear();

            //*********************************************************************************
            //Calulates the values that will be used to draw the histogram
            //*********************************************************************************
            for (int i = 0; i < gradeCounter.Length; i++)
            {
                gradeCounter[i] = 0;
            }

            //outside array used to iterate through the categories of the gradeCounter array divisions
            for (int i = 0; i < gradeCounter.Length; i++)
            {
                counter = 0;    //resets counter to 0 for each loop iteration

                //inside loop will count the amount of grades inside the array that are within the max/min conditions
                for (int i2 = 0; i2 < studentGrades.Length; i2++)
                {
                    if ((studentGrades[i2] >= minGrade) && (studentGrades[i2] <= maxGrade))
                    {
                        counter++;
                    }
                    else if (studentGrades[i2] == 100)
                    {
                        counter++;
                    }
                }
                //increments the conditions that are used to count categories in the student grade array
                gradeCounter[i] = counter;
                minGrade += 10;
                maxGrade += 10;
            }

            //********************************************************************************
            //Creates and Draws the Histogram with GDI drawer
            //********************************************************************************
            double barSize = 0;                                                 //the calculated ratio of bar size compared to the maximum height of 580 pixels.
            int xstart = 0;                                                     //starting x axis pixel to draw the histogram on
            int ystart = 580;                                                   //starting y axis pixel to draw the histogram on
            int[] red = { 255, 255, 255, 0, 180, 100, 128, 70, 20, 225, 100 };  //array to hold RGB red values
            int[] green = { 0, 255, 0, 255, 200, 250, 128, 120, 50, 170, 70 };  //array to hold RGB green values
            int[] blue = { 0, 0, 255, 255, 100, 60, 128, 40, 220, 20, 150 };    //array to hold RGB blue values
            maxGrade = 9;                                                       //maxGrade for current category to display in label                                      
            minGrade = 0;                                                       //minGrade for current category to display in label

            //loop used to interate through the categories of grade distributions and print a histogram of the results
            for (int i = 0; i < gradeCounter.Length; i++)
            {
                //calculates the current histogram barsize to be relative to the highest count possible
                barSize = 580 * ((double)gradeCounter[i] / (double)gradeCounter.Max());

                //prints the labels of the graph and increments the category data if there are no gradeCounts within the category parameters
                    canvas.AddText(gradeCounter[i].ToString(), 15, xstart + 10, ystart - (((int)(barSize / 2)) + 10), 50, 30, Color.Black);
                    //prints the labels of the histogram bars to the canvas
                    if (minGrade < 100)
                    {
                        canvas.AddText($"{minGrade} to {maxGrade}", 10, xstart - 5, 580, 80, 12, Color.White);
                    }
                    else if (minGrade == 100)
                    {
                        canvas.AddText($"100", 10, xstart - 5, 580, 80, 12, Color.White);
                    }

                    //prints the histogram bars
                    if (barSize > 0)
                    {
                        //prints a coloured histogram bar to the canvas. colours are predermined by position in RGB number arrays
                        canvas.AddRectangle(xstart, ystart - ((int)(barSize)), 70, (int)(barSize), Color.FromArgb(red[i], green[i], blue[i]), 1, Color.Black);
                        //prints the count text to the histogram
                        canvas.AddText(gradeCounter[i].ToString(), 15, xstart + 10, ystart - (((int)(barSize / 2)) + 10), 50, 30, Color.Black);
                    }
                    //increments the variables used to control histogram bar and label positioning and information.
                    xstart += 70;
                    maxGrade += 10;
                    minGrade += 10;              
            }
        }
        //Name: private static void SaveFile(out string fileName, ref int[] studentGrades)
        //Purpose: Saves the current contents of the student grades array to a textfile
        //Parameters:   fileName: string value of a file name that is passed used to write a file then passed and held within the main program
        //              studentGrades: passed array of student grades that will be saved in a text file.
        private static void SaveFile(out string fileName, ref int[] studentGrades)
        {
            StreamWriter sw;    //initializes a streamwriter object
            
            //saves the current contents of the studentGrades array into a textfile
            Console.Write("\nPlease enter a name to save the file as: ");
            fileName = Console.ReadLine();
            try
            {
                sw = new StreamWriter(fileName);

                for (int i = 0; i < studentGrades.Length; i++)
                {
                    sw.WriteLine(studentGrades[i]);
                    
                }
                sw.Close();
                Console.WriteLine("File has been saved successfully\n");

            }
            catch (Exception errWrite)
            {

                Console.WriteLine($"Error writing to file. Error Message:{errWrite}");
            }
        }

        //Name: private static void LoadFile(ref string fileName, ref int[] studentGrades)
        //Purpose: Used to load a text file of student grades into the studentGrades array
        //Parameters:   filename: fileName input by user and then held by the main program
        //              studentGrades: array to be filled with the data from the text file.
        private static void LoadFile(ref string fileName, ref int[] studentGrades)
        {
            StreamReader sr;    //initializes a streamreader object
            string input;       //string of textfile contents used to populate the studentGrades array
            int i = 0;          //integer used to increment through the index of the grades array
            int count = 0;      //count of lines in the text file which is used to set the array to the appropriate size.

            //accepts a filename from the user
            Console.Write("Please enter a file name: ");
            fileName = Console.ReadLine();

            //opens up the file if possible and loads it's contents into the studentGrades array
            try
            {
                //new streamreader object according to the filename
                sr = new StreamReader(fileName);

                try
                {
                    //reads the text file and counts how many lines of data are present to create the array size.
                    while (sr.ReadLine() != null)
                    {
                        count++;
                    }
                    sr.Close();

                    //sets the array size to the lines of text in the file
                    studentGrades = new int[count];

                    //inputs data from the textfile into the array to use in the console
                    sr = new StreamReader(fileName);
                    while ((input = sr.ReadLine()) != null)
                    {
                        int.TryParse(input, out studentGrades[i]);
                        i++;
                    }
                    sr.Close();
                    Console.WriteLine("\nFile has successfully Loaded\n");
                }
                catch (Exception errRead)
                {

                    Console.WriteLine($"Error writing to file. Error Message:{errRead}");
                }
            }
            catch (Exception errOpen)
            {

                Console.WriteLine($"Error writing to file. Error Message:{errOpen}");
            }
        }
    }
}
