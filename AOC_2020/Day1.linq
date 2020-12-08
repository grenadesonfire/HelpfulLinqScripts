<Query Kind="Program" />

void Main()
{
	var inputs = File.ReadAllLines(@"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\Day1A").Select(f => int.Parse(f));

	FirstHalf(inputs);

	var first = inputs.FirstOrDefault(i => inputs.Any(j => inputs.Any(k => i+j+k == 2020)));
	var second = inputs.FirstOrDefault(i => inputs.Any(j => first + i + j == 2020));
	var third = inputs.FirstOrDefault(i => i + first + second == 2020);
	
	(first*second*third).Dump();
}

void FirstHalf(IEnumerable<int> inputs)
{
	var first = inputs.FirstOrDefault(i => inputs.Any(j => j + i == 2020));
	var second = inputs.FirstOrDefault(i => i + first == 2020);

	(first * second).Dump();
}


