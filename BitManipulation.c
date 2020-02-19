0//File BitManipulation.c
//Austin Klevgaard 
//Jan 30 2019
//Description: Program that accepts 4 arguments in the console. arg1 and 3 contains sign values for arg2 and 4
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

unsigned char Compliment(unsigned char twosCompVal) 
{
	unsigned char returnInverse = ~(twosCompVal)+1;
	return returnInverse;
}
//Name: void PrintToBinaryPositive(unsigned char argNum)
//Purpose: takes an unsigned character number as input and prints it in its binary form
//parameters: argNum: passed value of the first argument input into the console.
void PrintToBinaryPositive(unsigned char argNum)
{
	//prints the argument in binary
	unsigned char mask = 0x80;

	for (int i = 0; i < 8; i++)
	{
		printf("%c", (argNum & mask) ? '1' : '0');
		mask >>= 1;
	}
	printf("\n");
}
//Name: void SumBinSum(char* sign1, unsigned char argNum1, char* sign2, unsigned char argNum2)
//Purpose: prints each of the passed arguments as binary as well as the sum of each of them
//parameters:	
//				sign1: sign value of the first argument
//				argNum1: passed value of the first argument input into the console.
//				sign2: sign value of second argument
//				argNum2: passed value of the seond argument input into the console.		
void SumBinSum(char* sign1, unsigned char argNum1, char* sign2, unsigned char argNum2)
{
	unsigned char sumResult = 0;		//the resulting value from summing arg1 and arg2
	unsigned char absVal = 0;

	//prints the sum of the arguments if both arguments are positive
	if ((strcmp(sign1, "+") == 0) & (strcmp(sign2, "+") == 0))
	{
		sumResult = (int)argNum1 + (int)argNum2;

	}

	//prints the sum of the arguments if both are negative
	if ((strcmp(sign1, "-") == 0) & (strcmp(sign2, "-") == 0))
	{

		sumResult = (int)argNum1 + (int)argNum2;
		sumResult = Compliment(sumResult);

	
	}
	//prints the sum of the arguments if only one of them is positive and the other is negative
	if ((((strcmp(sign1, "-") == 0) & (strcmp(sign2, "+") == 0))) | ((strcmp(sign2, "-") == 0) & ((strcmp(sign1, "+") == 0))))
	{
		//converts arg1 to 2's compliment then adds arg1 and arg2 and displays
		if ((strcmp(sign1, "-") == 0))
		{
			sumResult = (int)Compliment(argNum1) + (int)argNum2;

		}
		//converts arg2 to 2's compliment then adds arg1 and arg2 and displays
		if ((strcmp(sign2, "-") == 0))
		{
			sumResult = (int)Compliment(argNum2) + (int)argNum1;

		}

	}

	//these function look to see if the sum is negative or positive and print the proper output based on sign
	unsigned char mask = 0x80;
	//prints sum output if sum is positive
	if ((sumResult & mask) == 0)
	{
		printf("\nThe result of the sum is %d which in binary is: ", sumResult);
		PrintToBinaryPositive(sumResult);
	}
	//prints sum output if sum is negative
	else
	{
		absVal = Compliment(sumResult);
		printf("\nThe result of the sum is -%d which in binary is: ", absVal);
		PrintToBinaryPositive(sumResult);
	}
}

int main(int argc, char** argv)
{
	unsigned char value1;	//unsigned char to hold the value of arg1
	unsigned char value2;	//unisgned char to hold the value of arg2
	unsigned char sumResult = 0;

	//Error with input catching and usage
	if ((argc < 3) | (argc > 5) | (argc == 4))
	{
		printf("Input error. Usage: filename sign1 arg1 sign2 arg2");
		exit(EXIT_FAILURE);
	}

	printf("\n");
	//Performs binary printing in a single integer is passed to the program and sign 1 is negative
	if ((argc == 3) & (strcmp(argv[1], "-") == 0))
	{
		value1 = (unsigned char)atoi(argv[2]);
		printf("%d in binary is: ", (int)value1);
		PrintToBinaryPositive(Compliment(value1));
	}
	//Performs binary printing if a single integer is passed to the program and sign 1 is positive
	if ((argc == 3) & (strcmp(argv[1], "+") == 0))
	{
		value1 = (unsigned char)atoi(argv[2]);
		printf("%d in binary is: ", (int)value1);
		PrintToBinaryPositive(value1);
	}
	//prints in binary and sums if 2 arguments are passed to the program
	if (argc == 5)
	{
		value1 = (unsigned char)atoi(argv[2]);		//value1 holds the numerical value of arg1
		value2 = (unsigned char)atoi(argv[4]);		//value2 holds the numerical value of arg2

		//prints arg1 in binary negative if sign1 is negative
		if ((strcmp(argv[1], "-") == 0))
		{
			printf("-%d in binary is: ", (int)value1);
			PrintToBinaryPositive(Compliment(value1));
		}
		//prints arg1 in binary positive if sign1 is positive
		else if ((strcmp(argv[1], "+") == 0))
		{
			printf("%d in binary is: ", (int)value1);
			PrintToBinaryPositive(value1);
		}
		//prints arg2 in binary negative if sign 2 is negative
		if ((strcmp(argv[3], "-") == 0))
		{
			printf("-%d in binary is: ", (int)value2);
			PrintToBinaryPositive(Compliment(value2));
		}
		//prints arg2 in binary positive if sign 2 is positive
		else if ((strcmp(argv[3], "+") == 0))
		{
			printf("%d in binary is: ", (int)value2);
			PrintToBinaryPositive(value2);
		}
		//sums the 2 input arguments
		SumBinSum(argv[1], value1, argv[3], value2);
	}
	//exits program
	exit(EXIT_SUCCESS);
}
