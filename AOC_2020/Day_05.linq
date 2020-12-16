<Query Kind="Program" />

void Main()
{
	var inputs = File.ReadAllLines(@"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\Day5A");

	// First Half
	var max = inputs.Select(i => ConvertSeat(i)).Max().Dump();
	
	//Second half
	var second = inputs
		.Select(i => ConvertSeat(i));
	
	for(int idx=second.Min();idx<second.Max();idx++) if(!second.Contains(idx)) idx.Dump();
}

int ConvertSeat(string id){
	return Convert.ToInt32(
		id
			.Replace("B","1")
			.Replace("F","0")
			.Replace("R", "1")
			.Replace("L", "0"), 2);
}
