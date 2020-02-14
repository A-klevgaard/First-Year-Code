//************************************************************************************************
//Author: Austin Klevgaard
//Class: CMPE 1600
//Program: Lab02 - Image Manipulator
//Class A01
//************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab02
{
    public partial class Form1 : Form
    {
        Random _ranGen = new Random();      //Random number generator used to create noise values

        public Form1()
        {
            InitializeComponent();
        }

        //Occurrence: Everytime the scroll bar is moved by the user's input
        //Main Effect: Changes the value associated with the scroll bar
        //Side Effect: The scrollbar display label updates to correspond to the scrollbar value
        private void Trackbar_Effect_Scroll(object sender, EventArgs e)
        {
            //Changes the text of the middle label depending on what radio button is selected when it scrolls
            if (Tint_RadioBtn.Checked)
            {
                //prints the amount that the image will be tinted red
                if (Trackbar_Effect.Value < 50)
                {
                    Middle_Label.Text = ($"To Red: {50 - Trackbar_Effect.Value}");
                }
                //prints the amount that the image will be tinted green
                else if (Trackbar_Effect.Value > 50)
                {
                    Middle_Label.Text = ($"To Green: {Trackbar_Effect.Value - 50}");
                }
                //prints a blank label if there is no tint
                else
                {
                    Middle_Label.Text = "";
                }
            }
            //Prints the numerical value of the trackbar for all other values
            else
            {
                Middle_Label.Text = Trackbar_Effect.Value.ToString();
            }
        }

        //Occurrence: Everytime the use clicks the Load Picture button
        //Main Effect: Executes an OpenFileDialogue that allows the user to select an image to open in the picturebox
        //Side Effect: Imports the image into the picturebox / Enables the transform button
        private void LoadPicture_Btn_Click(object sender, EventArgs e)
        {
            //opens a dialogue box and if the selected file is an image and then the user selects okay, loads the image into the picturebox
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                try
                {
                    //loads the picture into the form
                    pictureBox1.Load(openFileDialog1.FileName);

                    //allows the user to select the transform button
                    Transfrom_Btn.Enabled = true;
                }
                //displays an error messagebox if the file could not be opened.
                catch (Exception)
                {
                    MessageBox.Show("Could not load the file.", "Load File Warning",MessageBoxButtons.OK);                  
                }
            }
        }

        //Occurrence: Everytime the user selects the "Contrast" Radio button
        //Main Effect: Updates the scrollbar and scroll labels to fit the Contrast function 
        //Side Effect:
        private void Contrast_RadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            //sets up the trackbar for contrast operations
            Trackbar_Effect.Minimum = 0;
            Trackbar_Effect.Maximum = 100;
            Trackbar_Effect.TickFrequency = 5;

            //sets up the labels of the trackbar to correspond to the contrast function.
            Left_Label.Text = "Less";
            Right_Label.Text = "More";
            Trackbar_Effect.Value = 50;
            Middle_Label.Text = Trackbar_Effect.Value.ToString();
        }

        //Occurrence: Occurs everytime the Transform button is clicked
        //Main Effect: Executes the pixel functions specified by the radio button, on the image in the pixelbox by generating a raster Bitmap
        //Side Effect:
        private void Transfrom_Btn_Click(object sender, EventArgs e)
        {
            int pixelXVal = 0;          //holds the x value for a pixel in the bitmap
            int pixelYVal = 0;          //holds the y value for a pixel in the bitmap
            Color pixelColor;           //color object to get the original rgb values of a pixel
            Color changedPixelColor;    //color object to hold updated rgb values to be set to a pixel
            int imageWidth = 0;         //width of the image, which is used as an endpoint of x values to iterate through
            int imageHeight = 0;        //height of the image, which is used as an endpoint of the y values to iterage through.
            Bitmap bm;                  //the raster bitmap of pixels generated from the image

            //creates a new bitmap of the loaded impate to work with
            try
            {
                //creates a new bitmap of the loaded impate to work with
                 bm = new Bitmap(pictureBox1.Image);

                //gets the width and height of the image
                imageWidth = bm.Width;
                imageHeight = bm.Height;

                //sets the max of the progress bar to 100
                progressBar1.Maximum = imageWidth / (imageWidth / 100);
                progressBar1.Minimum = 0;
                progressBar1.Step = 1;

                //***********************************************************************
                //Alters the image according to user specified criteria
                //***********************************************************************
                
                //outer loop: iterates through x values in the bitmap
                while (pixelXVal < imageWidth)
                {
                    //inner loop: iterates through y values in the bitmap
                    while (pixelYVal < imageHeight)
                    {
                        //aquires current pixel color value
                        pixelColor = bm.GetPixel(pixelXVal, pixelYVal);

                        //execute the contrast function on the pixel and reassigned it new rgb values
                        if (Contrast_RadioBtn.Checked)
                        {
                            ContrastTransform(pixelColor, out changedPixelColor);
                            bm.SetPixel(pixelXVal, pixelYVal, changedPixelColor);
                        }
                        //execute the Black and White function on the pixel and reassigned it new rgb values
                        if (BlackWhite_RadioBtn.Checked)
                        {
                            BlackWhiteTransform(pixelColor, out changedPixelColor);
                            bm.SetPixel(pixelXVal, pixelYVal, changedPixelColor);
                        }
                        //execute the tint function on the pixel and reassigned it new rgb values
                        if (Tint_RadioBtn.Checked)
                        {
                            TintTransform(pixelColor, out changedPixelColor);
                            bm.SetPixel(pixelXVal, pixelYVal, changedPixelColor);
                        }
                        //execute the Noise function on the pixel and reassigned it new rgb values
                        if (Noise_RadioBtn.Checked)
                        {
                            NoiseTransform(pixelColor, out changedPixelColor, _ranGen);
                            bm.SetPixel(pixelXVal, pixelYVal, changedPixelColor);
                        }

                        //increments the y value pixels
                        pixelYVal++;

                    }
                    //increments the x pixel values
                    pixelXVal++;
                    //resets y values to 0 to start at the top of the next colum
                    pixelYVal = 0;
                    //increments the progress bar
                    progressBar1.PerformStep();
                }
                //prints the transformed bitmap to the picturebox window
                pictureBox1.Image = (Image)bm;
                //resets the progres bar to 0
                progressBar1.Value = 0;
            }
            //Throws an error messagebox if there is an issue converting the image to a bitmap
            catch (Exception)
            {
                MessageBox.Show("Error Converting the Image to bitmap");
            }
        }
        //Name: private static int WrapCheck(int colorValue)
        //Purpose: Controls rgb values so that the max value possible is 255 and the minimum value is 0
        //Parameters: int colorValue - the incoming rgb int value that will be checked and corrected if necessary
        private static int WrapCheck( int colorValue)
        {
            //if color value falls within the acceptable range it is returned immediately
            if ((colorValue >= 0) & (colorValue <= 255))
            {
                return colorValue;
            }

            else
            {
                //if the colour value is greater than 255 it sets it to 255
                if (colorValue > 255)
                {
                    colorValue = 255;
                }
                //if the color value is less than 0, it sets it to 0
                else if (colorValue < 0)
                {
                    colorValue = 0;
                }
            }
            //returns altered color values
            return colorValue;
        }
        //Name: private void ContrastTransform(Color pixelColor, out Color changedPixelColor)
        //Purpose: Increases or decreases the rgb value of a pixel depending on if it is above or below 128, by a factor of 1/5 Trackbar value
        //Parameters:   Color pixelColor - the passed rgb values of the pixel to be altered
        //              Color changedPixelColor - The returned altered rgb values
        private void ContrastTransform(Color pixelColor, out Color changedPixelColor)
        {
            int redColorVal = pixelColor.R;     //Red values (0-255)
            int greenColorVal = pixelColor.G;   //Green values (0-255)
            int blueColorVal = pixelColor.B;    //Blue values (0-255)
            int alphaVal = pixelColor.A;        //Controls transparency, left unaltered currently

            int pause = 0;
            //Performs the contrast operation on red values. 
            //If > 128, increases the value by scrollbar value *(1 / 5)
            if (redColorVal > 128)
            {
                redColorVal = redColorVal + Trackbar_Effect.Value / 5;

            }
            //If > 128, decreases the value by scrollbar value * (1/5)
            if (redColorVal < 128)
            {
                redColorVal = redColorVal - Trackbar_Effect.Value / 5;
            }
            redColorVal = WrapCheck(redColorVal);


            //Performs the contrast operation on green values
            //If > 128, increases the value by scrollbar value * (1/5)
            if (greenColorVal > 128)
            {
                greenColorVal = greenColorVal + Trackbar_Effect.Value / 5;
            }
            //If > 128, decreases the value by scrollbar value * (1/5)
            if (greenColorVal < 128)
            {
                greenColorVal = greenColorVal - Trackbar_Effect.Value / 5;
            }
            greenColorVal = WrapCheck(greenColorVal);
            //Performs the contrast operation on blue values
            //If > 128, increases the value by scrollbar value * (1/5)
            if (blueColorVal > 128)
            {
                blueColorVal = blueColorVal + Trackbar_Effect.Value / 5;
            }
            //If > 128, decreases the value by scrollbar value * (1/5)
            if (blueColorVal < 128)
            {
                blueColorVal = blueColorVal - Trackbar_Effect.Value / 5;
            }
            blueColorVal = WrapCheck(blueColorVal);

            //returns the altered pixel rgb values back to the bitmap
            changedPixelColor = Color.FromArgb(alphaVal, redColorVal, greenColorVal, blueColorVal);
        }

        //Name: private void BlackWhiteTransform(Color pixelColor, out Color changedPixelColor)
        //Purpose: Reduces the color of the image and makes it more Black and White
        //Parameters:   Color pixelColor - the passed rgb values of the pixel to be altered
        //              Color changedPixelColor - The returned altered rgb values
        private void BlackWhiteTransform(Color pixelColor, out Color changedPixelColor)
        {
            int redColorVal = pixelColor.R;     //Red values (0-255)
            int greenColorVal = pixelColor.G;   //Green values (0-255)
            int blueColorVal = pixelColor.B;    //Blue values (0-255)
            int alphaVal = pixelColor.A;        //Controls transparency, left unaltered currently
            int average;

            //calculates the average of the rgb values for a pixel
            average = (redColorVal + greenColorVal + blueColorVal) / 3;

            //causes the red values to become closer to average on a factor of displacment and trackbar value
            redColorVal = redColorVal + ((average - redColorVal) * Trackbar_Effect.Value/100);
            redColorVal = WrapCheck(redColorVal);

            //causes the green values to become closer to average on a factor of displacment and trackbar value
            greenColorVal = greenColorVal + ((average - greenColorVal) * Trackbar_Effect.Value/100);
            greenColorVal = WrapCheck(greenColorVal);
            //causes the blue values to become closer to average on a factor of displacment and trackbar value
            blueColorVal = blueColorVal + ((average - blueColorVal) * Trackbar_Effect.Value/100);
            blueColorVal = WrapCheck(blueColorVal);

            //sends the new colour values out to the bitmap
            changedPixelColor = Color.FromArgb(alphaVal, redColorVal, greenColorVal, blueColorVal);

        }

        //Name: private void TintTransform(Color pixelColor, out Color changedPixelColor)
        //Purpose: Changes the tint of the image so that it is either red dominant or green dominant
        //Parameters:   Color pixelColor - the passed rgb values of the pixel to be altered
        //              Color changedPixelColor - The returned altered rgb values
        private void TintTransform(Color pixelColor, out Color changedPixelColor)
        {
            int redColorVal = pixelColor.R;     //Red values (0-255)
            int greenColorVal = pixelColor.G;   //Green values (0-255)
            int blueColorVal = pixelColor.B;    //Blue values (0-255)
            int alphaVal = pixelColor.A;        //Controls transparency, left unaltered currently

            //if the trackbar is currently less than 50 then the pixels in the image will be made red dominant
            if (Trackbar_Effect.Value < 50)
            {
                redColorVal = redColorVal + (50 - Trackbar_Effect.Value);
                redColorVal = WrapCheck(redColorVal);
            }
            //if the trackbar is currently more than 50 then the pixels in the image will be green dominant.
            if (Trackbar_Effect.Value > 50)
            {
                greenColorVal = greenColorVal + (Trackbar_Effect.Value - 50);
                greenColorVal = WrapCheck(greenColorVal);
            }
            //returns altered rgb pixel values to the bitmap
            changedPixelColor = Color.FromArgb(alphaVal, redColorVal, greenColorVal, blueColorVal);
        }
        //Name: private void NoiseTransform(Color pixelColor, out Color changedPixelColor, Random ranGen)
        //Purpose: Adds random values within an interval to the rgb values to introduce noise
        //Parameters:   Color pixelColor - the passed rgb values of the pixel to be altered
        //              Color changedPixelColor - The returned altered rgb values
        //              Random ranGen - Random number generator
        private void NoiseTransform(Color pixelColor, out Color changedPixelColor, Random ranGen)
        {

            int redColorVal = pixelColor.R;     //Red values (0-255)
            int greenColorVal = pixelColor.G;   //Green values (0-255)
            int blueColorVal = pixelColor.B;    //Blue values (0-255)
            int alphaVal = pixelColor.A;        //Controls transparency, left unaltered currently

            //performs the noise function on the red color
            redColorVal = redColorVal + (ranGen.Next(-Trackbar_Effect.Value, Trackbar_Effect.Value));
            redColorVal = WrapCheck(redColorVal);

            //performs the noise function on the green color
            greenColorVal = greenColorVal + (ranGen.Next(-Trackbar_Effect.Value, Trackbar_Effect.Value));
            greenColorVal = WrapCheck(greenColorVal);

            //performs the noise function on the blue color
            blueColorVal = blueColorVal + (ranGen.Next(-Trackbar_Effect.Value, Trackbar_Effect.Value));
            blueColorVal = WrapCheck(blueColorVal);

            //returns altered rgb values to the bitmap
            changedPixelColor = Color.FromArgb(alphaVal, redColorVal, greenColorVal, blueColorVal);

        }

        //Occurrence: Occurs everytime the Tint radiobutton is checked
        //Main Effect: alters the trackbar and it's labelsto contain data relevant to the tint function
        //Side Effect:
        private void Tint_RadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            //sets up the trackbar for tint operations
            Trackbar_Effect.Minimum = 0;
            Trackbar_Effect.Maximum = 100;
            Trackbar_Effect.TickFrequency = 5;
            Trackbar_Effect.Value = 50;

            //changes the trackbar labels to fit the tint function
            Left_Label.Text = "Red";
            Right_Label.Text = "Green";
            Middle_Label.Text = Trackbar_Effect.Value.ToString();

            //prints the amount that the image will be tinted red
            if (Trackbar_Effect.Value < 50)
            {
                Middle_Label.Text = ($"To Red: {50 - Trackbar_Effect.Value}");
            }
            //prints the amount that the image will be tinted green
            else if (Trackbar_Effect.Value > 50)
            {
                Middle_Label.Text = ($"To Green: {Trackbar_Effect.Value- 50}");
            }
            else 
            {
                Middle_Label.Text = "";
            }


        }

        //Occurrence: Occurs everytime the Black and White radiobutton is checked
        //Main Effect: alters the trackbar and it's labelsto contain data relevant to the black and white function
        //Side Effect:
        private void BlackWhite_RadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            //sets up the trackbar for tint operations
            Trackbar_Effect.Minimum = 0;
            Trackbar_Effect.Maximum = 100;
            Trackbar_Effect.TickFrequency = 5;
            Trackbar_Effect.Value = 50;

            //changes the trackbar label to fit the black and white function
            Left_Label.Text = "Less";
            Right_Label.Text = "More";
            Middle_Label.Text = Trackbar_Effect.Value.ToString();
        }

        //Occurrence: Occurs everytime the Tint Noise is checked
        //Main Effect: alters the trackbar and it's labelsto contain data relevant to the noise function
        //Side Effect:
        private void Noise_RadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            //sets up the trackbar for tint operations
            Trackbar_Effect.Minimum = 0;
            Trackbar_Effect.Maximum = 100;
            Trackbar_Effect.TickFrequency = 5;
            Trackbar_Effect.Value = 50;

            //Changes the trackbar labels to correspond to the noise function.
            Left_Label.Text = "Less";
            Right_Label.Text = "More";
            Middle_Label.Text = Trackbar_Effect.Value.ToString();
        }
    }
    
}

