<Query Kind="Program" />

void Main()
{
	var inputs = File.ReadAllLines(@"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\Day3A");

	FirstHalf(inputs,5,1);
	Console.WriteLine("Hmm.");
	SecondHalf(
		inputs,
		new List<int[]>{
			new int[]{1,1},
			new int[]{3,1},
			new int[]{5,1},
			new int[]{7,1},
			new int[]{1,2},
		});
}

int FirstHalf(string[] lines, int slopeRight, int slopeDown){
	var lastX = 0;
	var trees = 0;
	var width = lines[0].Trim().Length;

	for(var zIdx =0;zIdx<lines.Length;zIdx+=slopeDown){
		if(lines[zIdx][lastX] == '#') trees++;
		lastX = (lastX + slopeRight) % width; 
	}
	return trees.Dump();
}

void SecondHalf(string[] lines, List<int[]> slopes)
{
	var ret = 1;
	foreach(var slope in slopes){
		ret *= FirstHalf(lines, slope[0], slope[1]);
	}
	ret.Dump("Final");
}
