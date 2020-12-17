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
			//"1,3,2",
			//"2,1,3",
			//"1,2,3",
			//"0,5,4,1,10,14,7"
		};

	foreach(var test in tests) FirstHalf(test);

	//SecondHalf(tests.LastOrDefault(), 30000);
}

void FirstHalf(string lines)
{
	var ec = new ElfCounter(lines);
	
	ec.CountUntil(10);
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
	private Dictionary<int,int> _lastHit;
	private Dictionary<int,int> _numHits;
	private int _round;
	
	public ElfCounter(string start)
	{
		_start = start;
		_countHistory = new List<int>();
		_lastHit = new Dictionary<int, int>();
		_numHits = new Dictionary<int, int>();
		
		_countHistory.AddRange(start.Split(',').Select(s => int.Parse(s)));
		
		_round = _countHistory.Count();
		
		for(var idx=0;idx<_countHistory.Count();idx++)
		{
			_numHits.Add(_countHistory[idx], 0);
			_lastHit.Add(_countHistory[idx], idx+1);
		}
	}
	
	public void CountUntil(int limit, bool print = false)
	{
		while(_round != limit){
			if(_round % (limit/10) == 0 && print) Console.WriteLine("10% has passed.");
		
			var last = _countHistory.Last();
			
			if(_numHits[last] == 0){
				_countHistory.Add(0);
				
				//Special case! check 0
				if(_numHits.Keys.Contains(0)) _numHits[0] = 2;
				
				_numHits[last] = 2;
				_lastHit[last] = _round;
			}
			else
			{
				var spoken = _round - _lastHit[last];

				$"Round: {_round + 1} Last: {last} Sub {_round} {_lastHit[last]} Spoken: {spoken}".Dump();
				
				_countHistory.Add(spoken);
				
				// See if spoken number is new
				if(_numHits.TryAdd(spoken, 0)){
					_lastHit.Add(spoken, _round);
				}
				else{
					_numHits[spoken] = Math.Max(_numHits[spoken]+1, 2);
				}
			}
			
			//// Check if last had been spoken before
			//if(_numHits[last] <= 1){
			//	_lastHit[last] = _countHistory.Count();
			//	_numHits[last]++;
			//	_countHistory.Add(0);
			//}
			//// Has been spoken before
			//else{
			//	var count = _countHistory.Count();
			//	var prevIndexMinus1 = _lastHit[last];
			//	
			//	var val = count - prevIndexMinus1;
			//	_countHistory.Add(val);
			//	
			//	if(!_lastHit.TryAdd(val, _countHistory.Count())) _lastHit[val] = _countHistory.Count();
			//}
			_round++;
		}
		
		_countHistory.LastOrDefault().Dump(_start);
		_countHistory.Dump("History");
	}
}