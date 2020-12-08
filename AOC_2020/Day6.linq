<Query Kind="Program" />

void Main()
{
	var inputs = File.ReadAllLines(
		@"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\Day6A.txt"
		//@"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\Day6Example"
		);

	FirstHalf(
		inputs, 
		"abcdefghijklmnopqrstuvwxyz");

	SecondHalf(
		inputs,
		"abcdefghijklmnopqrstuvwxyz");
}

void FirstHalf(string[] lines, string targets){
	var hits = HistoGram.GetHistos(lines);
	
	hits.Select(h => h.AnyHits(targets)).Sum().Dump();
}

void SecondHalf(string[] lines, string targets){
	var hits = HistoGram.GetHistos(lines);
	
	hits.Select(h => h.EveryoneHits(targets)).Sum().Dump();
}

class HistoGram{
	private	int[] _hits;
	private string[] _lines;
	private int _numPersons;
	
	public HistoGram(string[] lines){
		_lines = lines;
		_hits = new int[26];
		_numPersons = lines.Length;

		foreach (var line in lines)
		{
			foreach (var c in line)
			{
				_hits[c - 'a']++;
			}
		}
	}
	
	public int AnyHits(string targets){
		var ret=0;
		foreach(var c in targets){
			if(_hits[c-'a'] > 0) ret++;
		}
		//new {
		//	Lines = _lines,
		//	Hits = _hits,
		//	Result = ret
		//}.Dump();
		return ret;
	}

	public int EveryoneHits(string targets)
	{
		var ret = 0;
		foreach (var c in _hits)
		{
			if (c == _numPersons) ret++;
			//else return 0;
		}
		
		return ret;
	}

	public static List<HistoGram> GetHistos(string[] lines){
		var ret = new List<HistoGram>();
		var working = new List<string>();
		
		foreach(var line in lines){
			if(string.IsNullOrEmpty(line)){
				ret.Add(new HistoGram(working.ToArray()));
				working.Clear();
			}
			else{
				working.Add(line);
			}
		}
		
		ret.Add(new HistoGram(working.ToArray()));
		
		return ret;
	}
}

