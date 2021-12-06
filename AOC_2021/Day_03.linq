<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2021\Inputs\";
void Main()
{
	var inputFile = Path.Combine(INPUT_FOLDER, "Day3.1.txt");
	
	var lines = File.ReadAllLines(inputFile);
	
	//var max = Math.Pow(2, lines[0].Length);
	
	
	//Part 1
	var hist = new Hist();
	
	foreach(var line in lines) hist.Add(line);
	
	hist.Solve().Dump();
	
	//Pat 2
	var tree = new Tree();
	tree.Init(lines);
	
	tree.Solve().Dump();
}

// You can define other methods, fields, classes and namespaces here

class Hist
{
	private List<int> _track;
	private int _total;
	
	public Hist() {}
	
	public void Add(string line)
	{
		if(_track == null) _track = Enumerable.Repeat(0, line.Length).ToList();
		
		_total++;
		
		for(var idx=0;idx<line.Length;idx++)
		{
			
			_track[idx]+=int.Parse(line[idx]+"");
		}
	}
	
	public int GammaRate()
	{
		var res = new StringBuilder();
		
		foreach(var x in _track)
		{
			res.Append((x > _total / 2 ? "1" : "0"));
		}
		
		return Convert.ToInt32(res.ToString(), 2);
	}

	public int EpisolonRate()
	{
		var res = new StringBuilder();

		foreach (var x in _track)
		{
			res.Append((x < _total / 2 ? "1" : "0"));
		}

		return Convert.ToInt32(res.ToString(), 2);
	}
	
	public int Solve()
	{
		
		return GammaRate() * EpisolonRate();
	}
}

class Tree
{
	public Node _root;
	public Tree()
	{
		_root = new Node();
		_root.Zero = new Node();
		_root.One = new Node();
	}
	
	public void Init(string[] lines)
	{
		foreach (var line in lines)
		{
			Add(line);
		}
	}

	public void Add(string line)
	{
		_root.Children++;
		if(line[0] == '0'){
			_root.Zero.Add(line, 1);
		}	
		else{
			_root.One.Add(line, 1);
		}
	}

	internal int Solve()
	{
		var oxygenrating = _root.OxyRating();
		
		var oxyInt = Convert.ToInt32(oxygenrating, 2);
		
		var c02rating = _root.C02Rating();
		
		var c02Int = Convert.ToInt32(c02rating, 2);
		
		return oxyInt * c02Int;
	}
}

class Node
{
	public Node Zero { get; set; }
	public Node One { get; set; }
	
	public int Children = 0;
	public string Number;

	internal void Add(string line, int v)
	{
		if(v == line.Length){
			Number = line;
			return;
		}
		
		Children++;

		if (line[v] == '0')
		{
			if(Zero == null) Zero = new Node();
			Zero.Add(line, v+1);
		}
		else
		{
			if(One == null) One = new Node();
			One.Add(line, v+1);
		}
	}

	internal string OxyRating()
	{
		//LEAF
		if(Children == 0){
			return Number;
		}
		
		if(Zero == null || One.Children >= Zero.Children) return One.OxyRating();
		else return Zero.OxyRating();
	}

	internal string C02Rating()
	{
		//LEAF
		if (Children == 0)
		{
			return Number;
		}

		if(One == null) return Zero.C02Rating();
		else if(Zero == null) return One.C02Rating();
		else if(Zero.Children <= One.Children) return Zero.C02Rating();
		else return One.C02Rating();
	}
}