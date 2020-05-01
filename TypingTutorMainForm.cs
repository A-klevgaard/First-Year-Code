//*************************************************************************
//Name: Austin Klevgaard
//Class: CMPE 1600 
//program: Lab04 - Typing Tutor
//*************************************************************************
using GDIDrawer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AustinKlevgaard_CMPE1600_Lab04b
{
    public partial class Form1 : Form
    {
        NewGameModal newGameDialog;                             //Loads up a newGame modal dialgoue form
        ShowSpeedModeless showAnimationSpeed = null;            //Initializes a new show speed modeless dialogue
        getHighScoreName getNameDialog;                         //Loads up a new getHighScoreName dialogue
        int _difficulty;                                        //difficulty setting imported from modal dialogue
        int _animationSpeed = 70;                               //controls the animation speed of falling letters
        int listSize;                                           //the amount of letters that will be drawn at a time, controlled by difficulty level
        bool pauseGame = false;                                 //boolean used to pause the game
        bool gameOn = false;                                    //bool value used to determine if the player is still playing or lost
        Thread thLetterAnimation;                               //Thread used to control the falling letter animation
        int _playerScore = 0;                                   //holds the current value of the players score for this playthrough
        string saveFileName = "highscores.bin";                 //name of the binary file where highscores are saved into
        highScores dummy;                                       //highscore strucutre used to fill in blank highscore structs in the list
        
        delegate void delVoidListStruct(List<fallingLetter> charList);          //delegate used to pass the char list from worker thread to main
        delegate void delVoidVoidReset();                                       //this delegate will be used to reset the main form after a game has ended
        
        //structure used to hold information for each letter in the game
        private struct fallingLetter
        {
            public char letter;     //char of the falling letter to be drawn
            public int fallSpeed;   //the speed that the letter will fall at
            public int xLocation;   //current x location of the letter
            public int yLocation;   //current y location of the letter
        }

        List<fallingLetter> _currentCharList = new List<fallingLetter>();   //list used to hold falling letter data structures

        //Serializable structure that is used to hold highscore data
        [Serializable]
        struct highScores               
        {
            public string playerName;           //name of the player
            public int difficultyNumeric;       //numeric representation of the difficulty they played on (1,2, or 3)
            public string difficulty;           //string word for the difficulty they played on  (easy, medium, hard)
            public int score;                   //the players score
        }

        List<highScores> _hplayerScores = new List<highScores>();    //list used to hold highscore data for each difficulty level

        public Form1()
        {
            InitializeComponent();

        }

        //Occurrence: Every time the form loads
        //Main Effect: Sets up the form for the game, by initializing values, and creating a highscore file
        //Side Effect: Also populates the listview object with newly created highscore data
        //Sidenote: I chose to create new original highscore data everytime the form loads for ease of moving this prgram to other computers
        //but in a real application, the highscore data should be saved more permanently between form loads
        private void Form1_Load(object sender, EventArgs e)
        {
            PauseButton.Enabled = false;                        //disables pause at startup
            EndGameButton.Enabled = false;                      //disables end game at startup
            CurrentScoreLabel.Text = _playerScore.ToString();   //resets current score label
            FileStream fs;                                      //filestream object for file I/O
            BinaryFormatter bf;                                 //binary formatter object

            //intialllizes a binary high score file if one does not exist
            if (!File.Exists(saveFileName))
            {
                //if the highscore file has not been created, this creates a blank highscore file, then stores it as a serialized bin file
                for (int idifficulty = 1; idifficulty <= 3; idifficulty++)
                {
                    //difficulty display name for the listview object selection
                    if (idifficulty == 1) dummy.difficulty = "Easy";
                    if (idifficulty == 2) dummy.difficulty = "Medium";
                    if (idifficulty == 3) dummy.difficulty = "Hard";

                    dummy.difficultyNumeric = idifficulty;      //difficulty numeric
                    dummy.playerName = "";                      //dummy player name for initial file
                    dummy.score = 0;                            //default score 

                    //adds newly created high scores to the player highScore list
                    _hplayerScores.Add(dummy);
                }
                //saves the highscore list to a binary file through serialization
                try
                {
                    //creates or overwrites the highscore file
                    fs = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
                    bf = new BinaryFormatter();
                    //adds in the data
                    bf.Serialize(fs, _hplayerScores);
                    //closes the filestream
                    fs.Close();
                }
                //displays an error message if the file was not created
                catch (Exception errCreate)
                {

                    MessageBox.Show($"Error creating highscore file, Error Message: {errCreate}", "Error Message", MessageBoxButtons.OK);
                }
            }
            //if there is already a binary file then load the contents of that file into a playerhighscoreslist
            else
            {
                try
                {
                    //deserializes the binary data and imports it into the tempscore list
                    fs = new FileStream(saveFileName, FileMode.Open, FileAccess.Read);
                    bf = new BinaryFormatter();

                    _hplayerScores = (List<highScores>)bf.Deserialize(fs);
                    fs.Close();
                }
                //provides an error message if data is not imported properly
                catch (Exception errRead)
                {
                    MessageBox.Show($"Error opening score file, Error Message: {errRead}", "Error Message", MessageBoxButtons.OK);
                }
            }
                //populates the listview object with highscore data
                SyncListViewScores(_hplayerScores);
            
        }
        //name:private void SyncListViewScores(List<highScores> newScores)
        //purpose: populates the listview object in the main form with current highscore data
        private void SyncListViewScores(List<highScores> newScores)
        {
            //clears out the old items
            listView1.Items.Clear();

            //this adds highscore data to the list view, starting from hardest level to easiest
            for (int i = newScores.Count -1; i > -1; i--)
            {
                ListViewItem lvi = new ListViewItem(newScores[i].playerName);
                lvi.SubItems.Add(newScores[i].difficulty);
                lvi.SubItems.Add(newScores[i].score.ToString());

                listView1.Items.Add(lvi);
            }
        }
        //Occurrence: Everytime the start game button is clicked
        //Main Effect: starts the game by creating a thread to display falling letter data and painting that data to the CDrawer canvas object
        //SideEffect: Also generates a new list of letters to be used in teh game
        private void StartGameButton_Click(object sender, EventArgs e)
        {
            //resets the players score data and score display for a new game
            label1.Text = "Current Score:";
            _playerScore = 0;
            CurrentScoreLabel.Text = _playerScore.ToString();

            //opens up a newGameModal dialogue to select game difficulty
            NewGameModal newGameDialog = new NewGameModal();

            //this just makes it so that the players last selected difficulty will be what is checked in the newGameDialog radiobuttons
            newGameDialog.pDifficulty = _difficulty;

            //if player elects okay the start teh game
            if (DialogResult.OK == newGameDialog.ShowDialog())
            {
                //sets the difficulty for the game according to user selection
                _difficulty = newGameDialog.pDifficulty;

                //changes the max list size to accomodate the players requested difficulty (easy = 5 letters, medium = 10, hard = 15)
                listSize = 5 * _difficulty;

                //sets all the game buttons to the appropriate values to run the game
                StartGameButton.Enabled = false;
                PauseButton.Enabled = true;
                EndGameButton.Enabled = true;
                label1.Text = "Current Score:";

                //fills the games character list with new data
                FillList(_currentCharList);

                //This starts a new background thread that will be used to animate the falling letters
                thLetterAnimation = new Thread(new ParameterizedThreadStart(AnimateFallingLetters));
                thLetterAnimation.Name = "thLetterAnimation";
                thLetterAnimation.IsBackground = true;
                //ensures that the gameOn boolean is set to true to allow the background thread to run
                gameOn = true;
                //starts the thread
                thLetterAnimation.Start(_currentCharList);
            }

        }
        //name: private void AnimateFallingLetters(Object objData)
        //purpose: Animates the movements of the letters as they flow through the CDrawer canvas
        //parameters: Object objData - object that is used to pass in data from main form. objData should be type List<fallingLetter> to work
        private void AnimateFallingLetters(Object objData)
        {
            CDrawer _canvas = new CDrawer();                            //starts up a new canvas
            List<fallingLetter> listCopy = new List<fallingLetter>();   //initializes a fallingLetter list to be used in the background thread

            //ensures that passed in data is List<fallingLetter>
            if (objData is List<fallingLetter>)
            {
                //copies the objData into a new list to be manipulated
                listCopy = (List<fallingLetter>)objData;

                //loop is used to run the game within, and to ensure that the gameover condition has not been breached
                while (gameOn)
                {
                    //loop that is used to pause the game when pause boolean is set to true
                    while (pauseGame)
                    {
                        Thread.Sleep(100);
                    }

                    //clears canvas
                    _canvas.Clear();

                    //draws new letters to the creen
                    for (int i = 0; i < listCopy.Count; i++)
                    {
                        _canvas.AddText(listCopy[i].letter.ToString(), 20, listCopy[i].xLocation, listCopy[i].yLocation, 40, 40, Color.Yellow);
                    }

                    //moves the letters down one
                    MoveLetters(listCopy);

                    //Passes the updated letter positioning back to the main program as the game is running
                    try
                    {
                        Invoke(new delVoidListStruct(UpdateList), listCopy);
                    }
                    //if there is an error passing information then issue an error message box
                    catch (Exception errListUpdate)
                    {

                        MessageBox.Show($"Error Updating List, Error Message: {errListUpdate}", "Error Message", MessageBoxButtons.OK);
                    }
                    //sleep controls the animation speed, for an amount determined by the trackbar in the showspeedmodeless
                    Thread.Sleep(_animationSpeed);
                }
            }
            //if the game has ended this will display game over
            if (!gameOn)
            {
                _canvas.AddText("GAME OVER", 100, Color.Red);
            }
        }

        //name: private void UpdateList(List<fallingLetter> changedList)
        //purpose: updates the list information in the main form with the changes caused to it by the background thread moving letters
        //parameters:   List<fallingLetter> changedList - list of changed letter information
        private void UpdateList(List<fallingLetter> changedList)
        {
            lock(_currentCharList)
            {
                _currentCharList = changedList;
            }    
        }
        //Occurrence: Whenever the change game speed (animation speed) checkbox is clicked
        private void ShowSpeedDialogueChkBox_CheckedChanged(object sender, EventArgs e)
        {
            //if the box is checked
            if (ShowSpeedDialogueChkBox.Checked == true)
            {
                //if the modeless dialogue for choosing animation speed has not yet been created
                if (null == showAnimationSpeed)
                {
                    //create the modeless animation speed dialogue
                    showAnimationSpeed = new ShowSpeedModeless();
                    //use a callback method to update the animation speed in the main form
                    showAnimationSpeed._chooseAnimationSpeed = new delVoidInt(CallBackAnimationSpeedChange);
                    //handles closing the form, by unchecking the box and hiding the form
                    showAnimationSpeed._formClosing = new delVoidVoid(CallBackFormClosing);
                }
                //shows the form
                showAnimationSpeed.Show();
            }
            else
            {   //hides the form
                showAnimationSpeed.Hide();
            }
        }
        //name: private void CallBackAnimationSpeedChange(int speed)
        //purpose: callback method used to send animation speed data from the modless form to the main form
        //parameters:   int speed - integer to represent the trackbar value in the modeless
        private void CallBackAnimationSpeedChange(int speed)
        {
            //calculates animation speed with subtraction so the right side of the trackbar makes for high speed
            _animationSpeed = (150 - speed);                //sets the animation speed)
        }

        //name: private void CallBackFormClosing()
        //purpose: hides the modeless form
        private void CallBackFormClosing()
        {
            ShowSpeedDialogueChkBox.Checked = false;
        }

        //accidentally clicked this, and if i remove it the whole project breaks. Thanks Windows.
        private void ShowSpeedDialogueChkBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        //Occurrence: Everytime a key is pressed
        //Main Effect: Checks to see if the user's key press corresponds to one in the letter list, and if it does
        //it removes that letter from the list and generates a new one in its place
        //Side Effect: Also unpauses the game if it's paused
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            pauseGame = false;  //unpause the game

            
            //checks the user's keypress against the current list
            for (int i = 0; i < listSize; i++)
            {
                //if the pressed key matches a list key then remove that key from the list
                if (e.KeyChar.ToString().ToLower() == _currentCharList[i].letter.ToString())
                {
                    _playerScore++;
                    CurrentScoreLabel.Text = _playerScore.ToString();
                    //locks the _currentCharList so it can't be altered while it's being edited here.
                    lock (_currentCharList)
                    {
                        //removes a letter then generates a new one in its place
                        _currentCharList[i] = RemoveLetter(_currentCharList[i]);
                        FillList(_currentCharList);
                    }
                }
            }
        }

        //name: private fallingLetter MakeNewFallingLetter()
        //purpose: generates a falling letter structure to add to the list
        private fallingLetter MakeNewFallingLetter()
        {
            Random ranGen = new Random();       //random number generator used to generate numbers and characters
            fallingLetter tempStruct;           //temporary structure used to pass on values

            //populates the structure with random values within the parameters
            //ranGen chooses a random number between 'a' and '{' because it is an exclusive method on it's upper end so we need to use 
            //'{' instead of 'z' because '{' is one character above 'z' in the ascii table
            tempStruct.letter = (char)ranGen.Next('a', '{');
            tempStruct.fallSpeed = ranGen.Next(2, 11);
            tempStruct.xLocation = ranGen.Next(50, 750);
            tempStruct.yLocation = ranGen.Next(10, 101);

            //returns the new structure
            return tempStruct;

        }
        //name: private fallingLetter RemoveLetter(fallingLetter letterDestroyed)
        //purpose: neutralizes the values in a fallingLetter structure to be replaced with FillList
        private fallingLetter RemoveLetter(fallingLetter letterDestroyed)
        {
            letterDestroyed.letter = '0';
            letterDestroyed.fallSpeed = 0;
            letterDestroyed.yLocation = 1;
            letterDestroyed.xLocation = 1;

            return letterDestroyed;

        }
        //name: private void FillList(List<fallingLetter> currentCharList)
        //purpose: Generates a list of fallingLetter structures, or fills in list structures that have been destroyed
        //parameters:   List<fallingLetter> currentCharList - retainer for the current fallingLetter structures
        private void FillList(List<fallingLetter> currentCharList)
        {
            Random ranGen = new Random();       //random number generator used to generate numbers and characters
            fallingLetter tempStruct;           //temporary structure used to assign values to the list
            int valueCount = 0;                 //count for how many of each letter are present in the list 
                                                //should always be == 1 if the list is acceptable

            //if the character list is completely empty then generate a new list
            if (currentCharList.Count < listSize)
            {
                for (int i = 0; i < listSize; i++)
                {
                    tempStruct = MakeNewFallingLetter();
                    currentCharList.Add(tempStruct);

                    //ensure there are no duplicates in the list
                    CheckForRepeats(currentCharList, tempStruct);
                }
            }

            //Checks to see if there are any current blank spots in the list, and if there is fills them with a new random letter
            for (int i = 0; i < listSize; i++)
            {
                //if any of the characters in the list are destroyed then get new info for that position
                if (currentCharList[i].letter == '0')
                {
                    tempStruct = MakeNewFallingLetter();
                    currentCharList[i] = tempStruct;
                    CheckForRepeats(currentCharList, tempStruct);
                }
            }
        }
        //name: private void CheckForRepeats(List<fallingLetter> currentCharList, fallingLetter tempStruct)
        //purpose: checks the currentCharList to ensure there are no repeat letters in it
        //parameters:   List<fallingLetter> currentCharList -  current fallingLetter list
        //              fallingLetter tempStruct - the newly created fallingLetter struct to compare to the list
        private void CheckForRepeats(List<fallingLetter> currentCharList, fallingLetter tempStruct)
        {
            int valueCount = 0;                  //count for how many of each letter are present in the list 

            //loop compares the tempStruct to the current list, to ensure there are only unique letters. Repeats if a letter is duplicated in the list
            while (valueCount != 1)
            {
                //resets value count
                valueCount = 0;

                //iterates through the list comparing the temp value to the list values
                for (int j = 0; j < currentCharList.Count; j++)
                {
                    //if there is a match then increment valueCount (if there is only 1 of each letter then value count is 1 and that's what we want)
                    if (tempStruct.letter == currentCharList[j].letter)
                    {
                        valueCount++;
                        
                        //if valueCount is > 1 that means there are duplicates, so generate a new random letter
                        if (valueCount > 1)
                        {
                            //subs a new random character into the tempStruct
                            tempStruct = MakeNewFallingLetter();
                            currentCharList[j] = tempStruct;
                        }
                    }
                }
            }
        }

        //name: private void MoveLetters(List<fallingLetter> currentCharList) 
        //purpose: moves the positions of letters by increasing their y position down by a factor of their structure speed
        //parameters:   List<fallingLetter> currentCharList - current falling letter list
        private void MoveLetters(List<fallingLetter> currentCharList)
        {
            fallingLetter tempStruct;   //temporary falling letter structure that is used to modify the letter position values

            //iterates through the linked list
            for (int i = 0; i < currentCharList.Count; i++)
            {
                //copies list position to the temp struct
                tempStruct = currentCharList[i];
                //modifies the tempStruct y position according to the letters fall speed
                tempStruct.yLocation += tempStruct.fallSpeed;
                //copies the modified tempstruct back to the char list
                currentCharList[i] = tempStruct;

                //if the letter has left the bottom of the screen then intitiate gameover sequence
                if (tempStruct.yLocation >= 600)
                {
                    //invokes a delegate to stop the game from the main form, and reset the game values back to initial
                    try
                    {
                        Invoke(new delVoidVoidReset(ResetMainForm));
                    }
                    //displays a message box if there are errors
                    catch (Exception errReset)
                    {
                        MessageBox.Show($"Error Resetting Game, Error Message: {errReset}", "Error Message", MessageBoxButtons.OK);

                    }
                }
            }
        }
        //name: private void ResetMainForm()
        //purpose: Resets main form values back to initial values and ends the game
        private void ResetMainForm()
        {
            gameOn = false;         //sets the game to not running
            pauseGame = false;      //game can't be paused if it's not running

            StartGameButton.Enabled = true;     //enables start button again
            PauseButton.Enabled = false;        //disables pause button
            EndGameButton.Enabled = false;      //disables endgame button
            label1.Text = "Previous Score:";    //updates score label

            _currentCharList.Clear();           //clears out the old fallingLetter list

            //checks to see if the players high score is higher than the current high score for this level
            CheckHighScore();

        }
        //name: private void CheckHighScore()
        //purpose: checks to see if the players last highscore is worthy to go on the highscore list
        private void CheckHighScore()
        {
            FileStream fs;                                              //filestream object
            List<highScores> tempScoreList = new List<highScores>();    //temp list of highscore structures used to iterate through file information
            highScores tempScore;                                       //temp highscore struct used to modify the highscore data  

            try
            {
                //deserializes the binary data and imports it into the tempscore list
                fs = new FileStream(saveFileName, FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();

                tempScoreList = (List<highScores>)bf.Deserialize(fs);
                fs.Close();
            }
            //provides an error message if data is not imported properly
            catch (Exception errRead)
            {
                MessageBox.Show($"Error opening score file, Error Message: {errRead}", "Error Message", MessageBoxButtons.OK);
            }

            //compares the players score to the highscore for the difficulty they were playing on
            for (int i = 0; i < tempScoreList.Count; i++)
            {
                //checks to see which difficulty the players score will be compared to
                if (_difficulty == tempScoreList[i].difficultyNumeric)
                {
                    //if the player highscore is greater than the old highscore then get their name from the modal dialogue
                    if (_playerScore > tempScoreList[i].score)
                    {
                        //opens up a modal form to get the players name
                        getNameDialog = new getHighScoreName();

                        //retrieves name data from the modal dialogue
                        if (DialogResult.OK == getNameDialog.ShowDialog())
                        {
                            tempScore = tempScoreList[i];
                            tempScore.playerName = getNameDialog.pNameText;
                            tempScore.score = _playerScore;
                            _hplayerScores[i] = tempScore;                             
                        }

                        //redislplays the new highscore data in the listview object
                        SyncListViewScores(_hplayerScores);
                    }
                }
            }
            //serializes the data back into a binary file for storage
            try
            {
                fs = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(fs, _hplayerScores);
                fs.Close();
            }
            //displays an error if data is not saved properly
            catch (Exception errWrite)
            {
                MessageBox.Show($"Error saving scores to file, Error Message: {errWrite}", "Error Message", MessageBoxButtons.OK);
            }
        }
        //Occurrence: When the pause button is clicked
        //Main Effect: Pauses the game if running
        private void PauseButton_Click(object sender, EventArgs e)
        {
            pauseGame = !pauseGame;
        }
        //Occurrence: when the endgame button is clicked
        //Main Effect: ends the game if running.
        private void EndGameButton_Click(object sender, EventArgs e)
        {
            ResetMainForm();
        }

    }
}

