//********************************************************************************
//Austin Klevgaard 
//CMPE 1700 - Lab01
//Program: Bit Run Counter
//Purpose: Counts the average number of "1's" that are present in an amount of bits, up to 31
//********************************************************************************
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

//Name: double RollingAvg(double average, double newCount, double sampleCount)
//Purpose: Calculates a rolling average of given data for bit runs
//Parameters:	double average - passed value of the currently saved average
//				double newCount - passed value of the most recent run count to be added to the data
//				double sampleCount - value of how many bits have been incorporated into the average
double RollingAvg(double average, double newCount, double sampleCount)
{
	//calculates the rolling average of the given runs
	double nextAverage = ((average * sampleCount) + (newCount)) / (sampleCount + 1);
	return nextAverage;
}

int main(int argc, char** argv)
{
	unsigned int byte = 0;			//holds the current value of the bit sequence being analyzed
	unsigned int bitCount = 0;		//holds the user input for the size of the bit sequence will be analyzed
	int previousBit = 0;			//holds the state of the previous bit (0 or 1)
	int currentBit = 0;				//holds the state of the current bit to compare to previous bit
	unsigned int runCount = 0;		//counter to keep track of how many runs are in the bit sequence
	unsigned int maxBitVal = 1;		//holds the end value that will stop the program
	double oldAverage = 0.0;		//double value to carry the previous average
	double newAverage = 0.0;		//double value to hold the most current average

	//handles console errors if the wrong input was used
	if (argc != 2)
	{
		printf("Error: Wrong Inputs. Usage: filename bitSize (4-16)");
		exit(EXIT_FAILURE);
	}

	// for loop used to setup the bitCount to match the user argument. 
	//Would have preferred to have simplified it and just used the atoi(argv[1] in my actual code,
	//but that breaks the program for some reason.
	for (int y = 0; y < atoi(argv[1]); y++)
	{
		bitCount += 1;
	}
	//for loop used to determine the end point value of the bit sequence.
	//Would have preferred to just use pow(2,atoi(argv[1])), but that breaks the program also.
	for (int x = 0; x < atoi(argv[1]); x++)
	{
		maxBitVal = maxBitVal * 2;
	}

	unsigned int mask = (unsigned int)(maxBitVal / 2);	//mask to compare to bit data. set up to be equal to the highest bit value.

	//outer loop iterates through all bit values up to the max value for a given bit size. e.g. 8 bit max value == 255
	for (int j = 0; j < (maxBitVal); j++)
	{
		//innerloop iterates through the current bit arrangments, and counts how many runs of 1 appear.
		for (int i = 0; i < bitCount; i++)
		{
			//if current byte value and mask are both one, then it sets the current bit holder to 1
			if (byte & mask)
			{
				currentBit = 1;;
			}
			//if the current byte value is not one, then it sets the current bit holder to 0
			if (!(byte & mask))
			{
				currentBit = 0;
			}
			//if the condition where a new run is exectuted (0=>1), then the runCount is incremented for the byte size
			if (previousBit == 0)
			{
				if (currentBit == 1)
				{
					runCount++;
				}
			}
			//sets the previous bit value to the current bit for the next loop iteration, and also increments the mask to the next bit
			previousBit = currentBit;
			mask >>= 1;
		}

		//resets the mask for the next bit value to test
		mask = (int)(maxBitVal / 2);
		//increments to the next bit value.
		byte += 1;
		//resets the single bit placeholders for the next bit value.
		currentBit = 0;
		previousBit = 0;

		//calculates the running total
		if (j > 0)
		{
			newAverage = RollingAvg(oldAverage, runCount, j);
		}
		//resets variables for the next loop
		runCount = 0;
		oldAverage = newAverage;
	}
	//prints the calculated average of runs
	printf("\nThe average amount of runs is: %.2f\n", newAverage);

	//exits the program
	exit(EXIT_SUCCESS);
}