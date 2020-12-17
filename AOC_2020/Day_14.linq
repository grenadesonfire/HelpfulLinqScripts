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
	var mem = new MemoryModule();

	mem.ProcessJumpLines(lines);

	mem.TotalMemory().Dump("Part 2");
}

class MemoryModule{
	public Dictionary<long, long> _memory;
	private Mask _mask;
	
	public MemoryModule()
	{
		_memory = new Dictionary<long, long>();
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

	internal void ProcessJumpLines(string[] lines)
	{
		foreach (var line in lines)
		{
			var split = line.Split(new[] { " ", "=", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

			switch (split[0])
			{
				case "mask": SetMask(split[1]); break;
				case "mem": SetFanMemory(split[1], split[2]); break;
				default: Console.WriteLine($"Unkown Instruction [{line}]"); return;
			}
		}
	}

	private void SetMask(string mask)
	{
		_mask = new Mask(mask);
	}
	
	private void SetMemory(string addr, string value)
	{
		var idx = long.Parse(addr);
		
		if(!_memory.Keys.Contains(idx)) _memory.Add(idx, 0);
		
		_memory[idx] = _mask.ApplyInt(int.Parse(value));
	}

	private void SetMemoryNoMask(long addr, string value)
	{
		if (!_memory.Keys.Contains(addr)) _memory.Add(addr, 0);

		_memory[addr] = long.Parse(value);
	}

	private void SetFanMemory(string addr, string value)
	{
		var addresses = _mask.GetAllAddresses(addr);
		
		foreach(var address in addresses){
			SetMemoryNoMask(address, value);
		}
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
	
	public string ApplyAddressMask(string addr)
	{
		var ret = addr.ToList();
		
		for (int bitIdx = 0; bitIdx < MaskString.Length; bitIdx++)
		{
			if (MaskString[bitIdx] != '0')
			{
				ret[bitIdx] = MaskString[bitIdx];
			}
		}
		return string.Join("", ret);
	}

	public List<long> GetAllAddresses(string addr)
	{
		var masked = 
			ApplyAddressMask(
				ConvertTo36BitString(
					Convert.ToInt64(addr)
					));
					
		var binaryDigits = masked.Count(m => m == 'X');
		var variation = Math.Pow(2, binaryDigits);
		
		var ret = new List<long>();
		
		for(int i=0;i<variation;i++)
		{
			try
			{
				var iteration = ApplyVariation(i, masked, binaryDigits);
				ret.Add(Convert.ToInt64(iteration, 2));
			}
			catch(Exception ex)
			{
				$"{i} {masked} {binaryDigits}".Dump();
				"Hmm".Dump();
			}
		}
		
		return ret;
	}
	
	public string ApplyVariation(int num, string masked, int binaryLength)
	{
		var binary = Convert.ToString(num,2).PadLeft(binaryLength, '0');
		var sb = new StringBuilder();
		for(int binaryIdx=0, maskIdx = 0; maskIdx<masked.Length;maskIdx++)
		{
			if(masked[maskIdx] == 'X'){
				sb.Append(binary[binaryIdx++]);
			}
			else{
				sb.Append(masked[maskIdx]);
			}
		}
		return sb.ToString();
	}
	
	private string ConvertTo36BitString(long num)
	{
		return Convert.ToString(num, 2).PadLeft(36, '0');
	}
}