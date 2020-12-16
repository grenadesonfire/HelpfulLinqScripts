<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
		//Path.Combine(INPUT_FOLDER,"Day7_2.Example")
    	Path.Combine(INPUT_FOLDER,"Day7")
		);

	FirstHalf(
		inputs, 
		"shiny gold");

	SecondHalf(
		inputs,
		"shiny gold");
}

void FirstHalf(string[] lines, string bagName){
	var tree = new BagTree(lines);

	tree.FindPathsTo(bagName).Dump($"This many bags can contain {bagName}");
}

void SecondHalf(string[] lines, string bagName){
	var tree = new BagTree(lines);

	tree.TotalBags(bagName).Dump($"This many child bags are needed for {bagName}:");
}

class BagLink
{
	public string Label { get; set; }
	public int Quantity { get; set; }
	
	public BagLink(string line){
		var splits = line.Replace("bags", "").Replace("bag", "");
		
		Label = splits.Substring(2).Trim();
		Quantity = int.Parse(splits.Substring(0,1));
	}
}

class Bag{
	public string Label { get; set; }
	public List<BagLink> Links { get; set; }
	public int TotalChildBags { get; set; } = -1;
	
	public Bag(string line)
	{
		var pieces = line.Split(new []{ "bags contain", ",", "." }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		
		Label = pieces[0];
		Links = new List<BagLink>();
		
		if(pieces[1] != "no other bags"){
			foreach(var link in pieces.Skip(1)){
				Links.Add(new BagLink(link));
			}
		}
	}
}

class BagTree{
	private Dictionary<string, Bag> _bags;
	
	public BagTree(string[] lines){
		_bags = new Dictionary<string, Bag>();
		
		foreach(var line in lines){
			var bag = new Bag(line);
			
			_bags.Add(bag.Label, bag);
		}
	}

	internal int FindPathsTo(string bagName)
	{
		var processed = new List<string>();
		var toProcess = new Queue<string>();
		var canContain = new List<string>();
		
		// Grab initial
		foreach(var bag in _bags.Values){
			if(bag.Links.Any(bl => bl.Label == bagName)){
				toProcess.Enqueue(bag.Label);
			}
		}
		
		while(toProcess.Count() != 0){
			var bagTarget = toProcess.Dequeue();
			canContain.Add(bagTarget);
			processed.Add(bagTarget);
			
			foreach(var bag in _bags.Values){
				if(bag.Links.Any(l => l.Label == bagTarget)
					&& !processed.Contains(bag.Label)
					&& !toProcess.Contains(bag.Label)){
					toProcess.Enqueue(bag.Label);
				}
			}
		}
		
		return canContain.Count();
	}
	
	internal int TotalBags(string bagName)
	{
		return ChildBags(bagName);
	}
	
	internal int ChildBags(string bagName)
	{
		var bag = _bags[bagName];
		if(bag.TotalChildBags != -1) return bag.TotalChildBags;
		
		var total = 0;
		foreach(var link in bag.Links) total += link.Quantity*ChildBags(link.Label) + link.Quantity;
		bag.TotalChildBags = total;
		return total;
	}
}