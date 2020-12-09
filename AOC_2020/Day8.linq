<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
		//Path.Combine(INPUT_FOLDER,"Day8.Example")
    	Path.Combine(INPUT_FOLDER,"Day8")
		);

	FirstHalf(
		inputs);

	SecondHalf(
		inputs);
}

void FirstHalf(string[] lines){
	var vm = new IntCodeVM(lines);
	
	//vm.ExecuteBounded(100);
	vm.ExecuteUnique();
	
	vm.Dump();
}

void SecondHalf(string[] lines)
{
	var vm = new IntCodeVM(lines);
	var lastSwapped = -1;
	do{
		vm.Reset();
		if(lastSwapped != -1){
			vm.SwapInstructionAt(lastSwapped);
		}
		lastSwapped = vm.SwapInstructionAtNext(lastSwapped);
		if(lastSwapped == -1){
			Console.WriteLine("error");
			break;
		}
	}while (!vm.ExecuteUnique());
	
	vm.Accumulator.Dump("Value");
}

enum IntCodeInstructionType{
	nop,
	acc,
	jmp
}

class IntCodeInstruction
{
	public int LineNumber { get; set; }
	public IntCodeInstructionType Type { get; set; }
	public int Argument { get; set; }
}

class IntCodeVMState{
	public int PC { get; set; }
	public int Accumulator { get; set; }
}

class IntCodeVM
{
	// Keep track of what instruction we're on
	public int PC { get; set; }
	public int Accumulator { get; set; }
	public bool Terminated { get; set; }
	
	public IntCodeInstruction[] Program { get; set; }
	public List<IntCodeVMState> InstructionHistory { get; set; }
	
	public IntCodeVM(string[] instructions){
		PC = 0;
		InstructionHistory = new List<IntCodeVMState>();
		Program = Parse(instructions);
	}
	
	private IntCodeInstruction[] Parse(string[] lines){
		var ret = new List<IntCodeInstruction>();
		
		for(int idx=0;idx<lines.Length;idx++){
			var inst = Parse(lines[idx]);
			inst.LineNumber = idx;
			ret.Add(inst);
		}
		
		return ret.ToArray();
	}
	
	private IntCodeInstruction Parse(string line)
	{
		var split = line.Split(' ');
		var ret = new IntCodeInstruction();
		
		ret.Type = (IntCodeInstructionType)Enum.Parse(typeof(IntCodeInstructionType), split[0]);
		
		ret.Argument = int.Parse(split[1].Substring(1));
		
		if(split[1].Contains('-')) ret.Argument *= -1;
		
		return ret;
	}

	private void Execute()
	{
		InstructionHistory.Add(new IntCodeVMState { PC = PC, Accumulator = Accumulator });
		var intst = Program[PC++];
		
		switch(intst.Type){
			case IntCodeInstructionType.nop: break;
			case IntCodeInstructionType.acc: Accumulator += intst.Argument; break;
			case IntCodeInstructionType.jmp: PC += intst.Argument - 1; break;
		}
	}
	
	public void ExecuteBounded(int upperBound){
		while(InstructionHistory.Count() <= upperBound){
			Execute();
		}
	}

	public bool ExecuteUnique(int upperBound = 1000000)
	{
		while (InstructionHistory.Count() <= upperBound && PC < Program.Length)
		{
			if(InstructionHistory.Any(ih => ih.PC == PC)) return false;
			Execute();
		}
		
		return true;
	}
	
	public void Reset()
	{
		PC = 0;
		Accumulator = 0;
		InstructionHistory.Clear();
	}

	internal void SwapInstructionAt(int lastSwapped)
	{
		Program[lastSwapped].Type = 
			Program[lastSwapped].Type == IntCodeInstructionType.nop ? 
			IntCodeInstructionType.jmp :
			IntCodeInstructionType.nop;
	}

	internal int SwapInstructionAtNext(int lastSwapped)
	{
		if(lastSwapped >= Program.Length) return -1;
		for(var idx = lastSwapped+1;idx<Program.Length;idx++){
			if(Program[idx].Type == IntCodeInstructionType.nop || Program[idx].Type == IntCodeInstructionType.jmp){
				SwapInstructionAt(idx);
				return idx;
			}
		}
		return -1;
	}
}