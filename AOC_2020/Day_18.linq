<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day18")
		);
		
	//PolishNotation.Solve("2 + 3");
	//PolishNotation.Solve("2 * 3");
	//PolishNotation.Solve("2 * 3 + 4");
	//PolishNotation.Solve("2 * 3 + 4 * 5");
	//PolishNotation.Solve("2 * 3 + (4 * 5)").Dump();
	//PolishNotation.Solve("5 + (8 * 3 + 9 + 3 * 4 * 3)").Dump();
	////PolishNotation.Solve("2 * 3 + (4 * 5)");
	
	//PolishNotation.Solve("2 * 3 + (4 * 5)", true).Dump();
	
	FirstHalf(inputs);

	//PolishNotation.SolveWPresidence("1 + 2 * 3 + 4 * 5 + 6", true).Dump("Hmm");
	//PolishNotation.SolveWPresidence("1 + (2 * 3) + (4 * (5 + 6)) ", true).Dump("Hmm");
	//PolishNotation.SolveWPresidence("2 * 3 + (4 * 5)").Dump();

	SecondHalf(inputs);
}

void FirstHalf(string[] lines)
{
	PolishNotation.Solve(lines).Dump("Part 1");
}

void SecondHalf(string[] lines)
{
	PolishNotation.SolveWPresidence(lines).Dump("Part 2");
}

enum OperatorType
{
	Number,
	ADD,
	MULT,
	OPENPAREN,
	CLOSEPAREN
}

class Notation{
	public long Value { get; set; }
	public OperatorType Operator { get; set; }
}

class PolishNotation
{
	private List<Notation> _notations;
	private string _line;
	
	public PolishNotation(string line)
	{
		_notations = new List<UserQuery.Notation>();
		_line = line;
	}
	
	public void Initialize()
	{
		var splits =
			_line
				.Replace("(", " ( ")
				.Replace(")", " ) ")
				.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Select(l =>
				{
					if (long.TryParse(l, out var num)) return new Notation { Value = num };
					else if (l == "(") return new Notation { Operator = OperatorType.OPENPAREN };
					else if (l == ")") return new Notation { Operator = OperatorType.CLOSEPAREN };
					else if (l == "+") return new Notation { Operator = OperatorType.ADD };
					else return new Notation { Operator = OperatorType.MULT };
				})
				.ToList();

		ConvertSplits(splits, 0);
	}

	public void InitializeAndConvertNewPemdas()
	{
		var splits =
			_line
				.Replace("(", " ( ")
				.Replace(")", " ) ")
				.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Select(l =>
				{
					if (long.TryParse(l, out var num)) return new Notation { Value = num };
					else if (l == "(") return new Notation { Operator = OperatorType.OPENPAREN };
					else if (l == ")") return new Notation { Operator = OperatorType.CLOSEPAREN };
					else if (l == "+") return new Notation { Operator = OperatorType.ADD };
					else return new Notation { Operator = OperatorType.MULT };
				})
				.ToList();

		var operatorStack = new Stack<Notation>();
		
		foreach(var note in splits)
		{
			switch(note.Operator){
				case OperatorType.Number:
					_notations.Add(note);
					break;
				case OperatorType.OPENPAREN:
					operatorStack.Push(note);
					break;
				case OperatorType.CLOSEPAREN:
					{
						var temp = operatorStack.Pop();
						while(temp.Operator != OperatorType.OPENPAREN){
							_notations.Add(temp);
							temp = operatorStack.Pop();
						}
					}
					break;
				case OperatorType.ADD:
					if (operatorStack.Count() > 0 &&
							 operatorStack.Peek().Operator == OperatorType.ADD)
					{
						_notations.Add(operatorStack.Pop());
					}
					operatorStack.Push(note);
					break;
				case OperatorType.MULT:
					if(operatorStack.Count() > 0 && 
						(operatorStack.Peek().Operator == OperatorType.MULT ||
						 operatorStack.Peek().Operator == OperatorType.ADD))
					{
						_notations.Add(operatorStack.Pop());
					}
					operatorStack.Push(note);
					break;
			}
		}
		
		while(operatorStack.Count() > 0) _notations.Add(operatorStack.Pop());
	}

	private int ConvertSplits(List<Notation> splits, int idx)
	{
		while (idx < splits.Count()){
			if (splits[idx].Operator == OperatorType.CLOSEPAREN) return idx + 1;

			if (splits[idx].Operator == OperatorType.OPENPAREN)
			{
				idx = ConvertSplits(splits, idx+1);
			}
			else if(splits[idx].Operator == OperatorType.ADD 
				|| splits[idx].Operator == OperatorType.MULT)
			{
				var notation = splits[idx];
				
				if(splits[idx+1].Operator == OperatorType.Number){
					_notations.Add(splits[idx+1]);
					idx+=2;
				}
				else if(splits[idx+1].Operator == OperatorType.OPENPAREN){
					idx = ConvertSplits(splits, idx+2);
				}
				
				_notations.Add(notation);
				
			}
			else if(splits[idx].Operator == OperatorType.Number)
			{
				_notations.Add(splits[idx++]);
			}
		}
		
		return idx;
	}

	private long Solve()
	{
		var stack = new Stack<Notation>();
		
		foreach(var notation in _notations){
			if(notation.Operator == OperatorType.Number){
				stack.Push(notation);
			}
			else{
				var arg1 = stack.Pop();
				var arg2 = stack.Pop();
				
				if(notation.Operator == OperatorType.ADD){
					stack.Push(new Notation { Value = arg1.Value + arg2.Value });
				}
				else{
					stack.Push(new Notation { Value = arg1.Value * arg2.Value });
				}
			}
		}
		
		return stack.Pop().Value;
	}
	
	public static long Solve(string line, bool debug = false)
	{
		var pn = new PolishNotation(line);
		
		pn.Initialize();
		
		if(debug) pn._notations.Dump();
		
		return pn.Solve();
	}
	
	public static long Solve(string[] lines)
	{
		return lines.Sum(l => Solve(l));
	}

	public static long SolveWPresidence(string[] lines)
	{
		return lines.Sum(l => SolveWPresidence(l));
	}

	internal static long SolveWPresidence(string line, bool debug = false)
	{
		var pn = new PolishNotation(line);
		
		pn.InitializeAndConvertNewPemdas();

		if (debug) pn._notations.Dump();

		return pn.Solve();
	}

	
}
