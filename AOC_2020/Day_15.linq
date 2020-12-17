<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	//	var inputs = File.ReadAllLines(
	//    	Path.Combine(INPUT_FOLDER, "Day14")
	//		);
	var tests =
		new string[]
		{
			"0,3,6",
			"1,3,2",
			"2,1,3",
			"1,2,3",
			"0,5,4,1,10,14,7"
		};

	//foreach(var test in tests) FirstHalf(test);

	SecondHalf(tests.LastOrDefault(), 30000);
}

void FirstHalf(string lines)
{
	var ec = new ElfCounter(lines);
	
	ec.CountUntil(2020);
}

void SecondHalf(string lines, int limit)
{
	var ec = new ElfCounter(lines);
	
	ec.CountUntil(limit, true);
}

class ElfCounter
{
	private List<int> _countHistory;
	private string _start;
	
	public ElfCounter(string start)
	{
		_start = start;
		_countHistory = new List<int>();
		
		_countHistory.AddRange(start.Split(',').Select(s => int.Parse(s)));
	}
	
	public void CountUntil(int limit, bool print = false)
	{
		while(_countHistory.Count() != limit){
			if(_countHistory.Count() % (limit/10) == 0 && print) Console.WriteLine("10% has passed.");
		
			var last = _countHistory.Last();
			
			// Check if last had been spoken before
			if(_countHistory.Count(s => s == last) == 1){
				_countHistory.Add(0);
			}
			// Has been spoken before
			else{
				var count = _countHistory.Count() - 1;
				var prevIndexMinus1 = _countHistory.Take(count).ToList().LastIndexOf(_countHistory.LastOrDefault());
				
				_countHistory.Add(count - prevIndexMinus1);
			}
		}
		
		_countHistory.LastOrDefault().Dump(_start);
	}
}