<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day14")
		);

	FirstHalf(inputs);

	SecondHalf(inputs);
}

void FirstHalf(string[] lines)
{
	var mem = new MemoryModule();
	
	mem.ProcessLines(lines);
	
	mem.TotalMemory().Dump("Part 1");
}

void SecondHalf(string[] lines)
{
	
}

class MemoryModule{
	public Dictionary<int, long> _memory;
	private Mask _mask;
	
	public MemoryModule()
	{
		_memory = new Dictionary<int, long>();
	}
	
	public void ProcessLines(string[] lines)
	{
		foreach (var line in lines)
		{
			var split = line.Split(new[] { " ", "=", "[", "]"}, StringSplitOptions.RemoveEmptyEntries);
			
			switch(split[0]){
				case "mask": SetMask(split[1]); break;
				case "mem": SetMemory(split[1], split[2]); break;
				default: Console.WriteLine($"Unkown Instruction [{line}]"); return;
			}
		}
	}
	
	public long TotalMemory(){
		return _memory.Values.Sum(v => (long)v);
	}
	
	private void SetMask(string mask)
	{
		_mask = new Mask(mask);
	}
	
	private void SetMemory(string addr, string value)
	{
		var idx = int.Parse(addr);
		
		if(!_memory.Keys.Contains(idx)) _memory.Add(idx, 0);
		
		_memory[idx] = _mask.ApplyInt(int.Parse(value));
	}
}

class Mask
{
	public string MaskString { get; set; }

	public Mask(string str)
	{
		MaskString = str;
	}

	public string Apply(long num)
	{
		var ret = Convert.ToString(num, 2).PadLeft(36, '0').ToList();

		for (int bitIdx = 0; bitIdx < MaskString.Length; bitIdx++)
		{
			if (MaskString[bitIdx] != 'X')
			{
				ret[bitIdx] = MaskString[bitIdx];
			}
		}
		return string.Join("", ret);
	}

	public long ApplyInt(long num)
	{
		return Convert.ToInt64(Apply(num), 2);
	}
}