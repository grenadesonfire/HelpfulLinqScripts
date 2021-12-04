<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2021\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
		Path.Combine(INPUT_FOLDER, "Day1.1.txt")
		);

	FirstHalf(inputs);
	SecondHalf(inputs);
}

void FirstHalf(string[] inputs)
{
	var inc = 0;
	var nums = inputs.Select(i => int.Parse(i)).ToList();
	for(var idx = 1; idx < nums.Count(); idx++)
	{
		if(nums[idx] > nums[idx-1]) inc++;
	}
	inc.Dump("Part 1");
}

void SecondHalf(string[] inputs)
{
	var inc = 0;
	var nums = inputs.Select(i => int.Parse(i)).ToList();
	for (var idx = 3; idx < nums.Count(); idx++)
	{
		var winNow = nums[idx]+nums[idx-1]+nums[idx-2];
		var winPrev = nums[idx-3]+nums[idx-1]+nums[idx-2];
		if (winNow > winPrev) inc++;
	}
	inc.Dump("Part 2");
}

// You can define other methods, fields, classes and namespaces here