<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day19.Example")
		);
		
	FirstHalf(inputs);
	
	SecondHalf(inputs);
}

void FirstHalf(string[] lines)
{
	var rs = new RuleSet();
	
	rs.Initialize(lines);
	
	rs.Dump();
	
	rs.VerifyRule0("aab").Dump();
}

void SecondHalf(string[] lines)
{
	
}

class Rule
{
	public int Label { get; set; }
	
	public List<List<int>> Links { get; set; }
	
	public string Match { get; set; }
	
	public Rule(string input){
		var splits = input.Split(':');
		
		Label = int.Parse(splits[0]);
		Links = new List<List<int>>();
		
		//If it ends in a character
		if(splits[1].Contains('"'))
		{
			Match = splits[1].Trim();
		}
		else
		{
			var subSplits = splits[1].Trim().Split("|");
			
			foreach(var s in subSplits){
				Links.Add(
					s.Trim()
						.Split(" ")
						.Select(x => int.Parse(x))
						.ToList()
				);
			}
		}
	}
}

class RuleSet
{
	public Dictionary<int, Rule> Rules { get; set; }
	
	public RuleSet()
	{
		Rules = new Dictionary<int, Rule>();
	}
	
	public void Initialize(string[] lines){
		foreach(var line in lines)
		{
			var r = new Rule(line);	
			
			Rules.Add(r.Label, r);
		}
	}

	internal bool VerifyRule0(string v)
	{
		return false;
	}
}