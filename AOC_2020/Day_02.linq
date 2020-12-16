<Query Kind="Program" />

void Main()
{
	var inputs = File.ReadAllLines(@"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\Day2A");

	FirstHalf(inputs);
	SecondHalf(inputs);
	
	ValidPass2("2-9 c: ccccccccc").Dump();
}

void FirstHalf(IEnumerable<string> inputs)
{
	var valid = inputs.Count(i => ValidPass(i));
	
	valid.Dump();
}

void SecondHalf(IEnumerable<string> inputs)
{
	var valid = inputs.Count(i => ValidPass2(i));

	valid.Dump();
}

bool ValidPass(string s){
	var parts = s.Split(':');

	var req = parts[0].Split(new string[] { "-", " "}, System.StringSplitOptions.RemoveEmptyEntries);
	var min = int.Parse(req[0]);
	var max = int.Parse(req[1]);
	var count = parts[1].Count(i => i == req[2][0]);

	var ret = min <= count && count <= max;
	
	return ret;
}

bool ValidPass2(string s)
{
	var parts = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

	var req = parts[0].Split(new string[] { "-", " " }, System.StringSplitOptions.RemoveEmptyEntries);
	var min = int.Parse(req[0]);
	var max = int.Parse(req[1]);
	
	var firstIdx = parts[1][min] == req[2][0];
	var secondIdx = parts[1][max] == req[2][0];
	return (firstIdx || secondIdx) && !(firstIdx && secondIdx);
}
