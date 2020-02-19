//*******************************************************************************************
//Austin Klevgaard
//Program: Caesar Shift Encryption/Decrytption
//*******************************************************************************************
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int main(int argc, char** argv)
{

	char* string = malloc(strlen(argv[3]) + 1);	//string is the variable that holds the word to be encrypted. Malloc allocates enought data to store the string
	strcpy(string, argv[3]);					//copies the string in arg3 to the char array string
	int rotNumber = atoi(argv[1]);				//integer amount for how much each character will be rotated		
	char operation[1];							//holds the operation to be performed. e/E = encrypt, d/D = decrypt
	strcpy(operation, argv[2]);					//copies arg2 into the operation variable
	unsigned char letter = ' ';					//the current letter of the string char array that is being manipulated

	//Handles input errors and displays error messages
	if (argc != 4)
	{
		printf("Invalid Argument. Usage: <positive integer> <e or d> <string>");
		exit(EXIT_FAILURE);
	}
	if (atoi(argv[1]) <= 0)
	{
		printf("Error. arg1 must be an integer greater than 0");
		exit(EXIT_FAILURE);
	}
	if (strlen(argv[2]) > 1)
	{
		printf("Error. arg2 must be a single character of d or e");
		exit(EXIT_FAILURE);
	}
	if ((operation[0] != 'e') & (operation[0] != 'E'))
	{
		if ((operation[0] != 'd') & (operation[0] != 'D'))
		{
			printf("Error. arg2 must be either d or e.");
			exit(EXIT_FAILURE);
		}
	}

	//*******************************************************************************************
	//encrypts the given string argument
	//*******************************************************************************************
	if ((operation[0] == 'e') | (operation[0] == 'E'))
	{
		//iterates through the string array and perfroms a modular rotation for each character
		int i = 0;
		while (string[i] != '\0')
		{
			//handles the rot n encryption for capital letters
			if ((string[i] >= 65) & (string[i] <= 90))
			{
				//gets the capital letter character from the index of the string
				letter = string[i];

				//rotates the letter by n number of moves through the alphabet, then puts it back in ascii form
				letter = (((letter - 'A' + rotNumber)) % 26) + 'A';
				//puts the new rotated letter back in pace in the string
				string[i] = letter;
			}
			//handles the rot n encryption for lowercase letters
			if ((string[i] >= 97) & (string[i] <= 122))
			{
				//gets the capital letter character from the index of the string
				letter = string[i];

				//rotates the letter by n number of moves through the alphabet, then puts it back in ascii form
				letter = (((letter - 'a' + rotNumber)) % 26) + 'a';
				//puts the new rotated letter back in pace in the string
				string[i] = letter;
			}
			//handles the rot n encryption for numbers
			if ((string[i] >= 48) & (string[i] <= 57))
			{
				//gets the capital letter character from the index of the string
				letter = string[i];

				//rotates the letter by n number of moves through the alphabet, then puts it back in ascii form
				letter = ((letter - '0' + rotNumber) % 10)+ '0';
				//puts the new rotated letter back in pace in the string
				string[i] = letter;
			}
			else
			{
				letter = string[i];
				string[i] = letter;
			}
			i++;
		}
	}
	
	//*******************************************************************************************
	//decrypts the given string argument
	//*******************************************************************************************
	if ((operation[0] == 'd') | (operation[0] == 'D'))
	{
		//iterates through the string array and perfroms a modular rotation for each character
		int i = 0;
		while (string[i] != '\0')
		{
			//handles the rot n encryption for capital letters
			if ((string[i] >= 65) & (string[i] <= 90))
			{
				//gets the capital letter character from the index of the string
				letter = string[i];

				//rotates the letter by n number of moves through the alphabet, then puts it back in ascii form
				letter = (((letter - 'A' + (26- rotNumber))) % 26) + 'A';

				//puts the new rotated letter back in pace in the string
				string[i] = letter;
			}
			//handles the rot n encryption for lowercase letters
			if ((string[i] >= 97) & (string[i] <= 122))
			{
				//gets the capital letter character from the index of the string
				letter = string[i];

				//rotates the letter by n number of moves through the alphabet, then puts it back in ascii form
				letter = ((((letter - 'a') + (26 - rotNumber))) % 26) + 'a';

				//puts the new rotated letter back in pace in the string
				string[i] = letter;
			}
			//handles the rot n encryption for numbers
			if ((string[i] >= 48) & (string[i] <= 57))
			{
				//gets the capital letter character from the index of the string
				letter = string[i];

				//rotates the letter by n number of moves through the alphabet, then puts it back in ascii form
				letter = ((((letter - '0') + (10 - rotNumber))) % 10) + '0';

				//puts the new rotated letter back in pace in the string
				string[i] = letter;
			}
			//Keeps all other special characters unaltered in the string
			else
			{
				letter = string[i];
				string[i] = letter;
			}
			i++;
		}
	}
	//prints the resulting encryption or decryptions back to the console.
	printf("%s", string);
	
	exit(EXIT_SUCCESS);

}
