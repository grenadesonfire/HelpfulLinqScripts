<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
		Path.Combine(INPUT_FOLDER,"Day9")
    	//Path.Combine(INPUT_FOLDER,"Day9.Example")
		);

	FirstHalf(
		inputs,
		25).Dump();

	SecondHalf(
		inputs,
		25).Dump();
}

long FirstHalf(string[] lines, int preambleLength){
	var nums = lines.Select(l => long.Parse(l)).ToList();
	
	for(var idx=preambleLength;idx<lines.Length;idx++){
		var slimList = nums.Skip(idx-preambleLength).Take(preambleLength);
		if(!slimList.Any(l => slimList.Any(s => s!=l && s+l == nums[idx]))) return nums[idx];
	}
	return -1;
}

long SecondHalf(string[] lines, int preambleLength)
{
	var target = FirstHalf(lines, preambleLength);
	var nums = lines.Select(l => long.Parse(l)).ToList();
	
	Tuple<int,int> range = null;
	
	for(var idx=0;idx<nums.Count();idx++)
	{
		for(var idx2=idx+1;idx2<nums.Count();idx2++)
		{
			var sum = nums.Skip(idx).Take(idx2-idx).Sum();

			if (sum > target) break;
			else if (sum == target)
			{
				range = new Tuple<int, int>(idx, idx2);
				break;
			}
		}
		
		if(range != null) break;
	}
	
	var subset = nums.Skip(range.Item1).Take(range.Item2-range.Item1);
	
	return subset.Max() + subset.Min();
}

