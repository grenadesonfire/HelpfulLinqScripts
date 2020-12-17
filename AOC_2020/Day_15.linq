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

	foreach(var test in tests) FirstHalf(test);

	SecondHalf(tests.LastOrDefault(), 30000000);
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
		
			var last = _countHistory.Last();
			
			
			if(!_numHits.ContainsKey(last) || _numHits[last] == 0){
				_countHistory.Add(0);
				
				//Special case! check 0
				if(_numHits.Keys.Contains(0)) _numHits[0] = 2;
				
				_numHits[last] = 2;
			}
			else
			{
				var spoken = _round - _lastHit[last];

				//$"Round: {_round + 1} \r\n\tLast: {last} \r\n\tSub {_round} {_lastHit[last]} Spoken: {spoken}\r\n".Dump();
				
				_countHistory.Add(spoken);
				
				// See if spoken number is new
				if(!_numHits.TryAdd(spoken, 0)){
					_numHits[spoken] = Math.Max(_numHits[spoken]+1, 2);
				}
			}
			
			if(!_lastHit.TryAdd(last, _round)) _lastHit[last] = _round;
			
			_round++;
		}
		
		_countHistory.LastOrDefault().Dump(_start);
		//_countHistory.Dump("History");
	}
}