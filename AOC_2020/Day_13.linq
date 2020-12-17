<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day13")
		);

	FirstHalf(inputs);

	SecondHalf(inputs);
}

void FirstHalf(string[] lines)
{
	var s = new Schedule(lines);
	
	var id = s.NextArrival();
	
	id.Dump();
	
}

void SecondHalf(string[] lines)
{
	var s = new Schedule(lines);
	
	var t = s.EarliestCascadingStart();
	
	t.Dump("Second Half");
}

struct Departure
{
	public int Id { get; set; }
	public long MinutesWaiting { get; set; }
	public long Solution
	{
		get{
			return Id * MinutesWaiting;
		}
	}
	public long TimeStamp { get; set; }
}

class Bus
{
	public int PositionId { get; set; }
	public int BusId { get; set; }
}

class Schedule 
{
	public long StartingTimeStamp { get; set; }
	public int[] ParsedIds { get; set; }
	public List<Bus> Positions { get; set;}
	
	public Schedule(string[] lines)
	{
		StartingTimeStamp = long.Parse(lines[0]);
		
		ParsedIds = lines[1].Split(',').Where(x => !string.IsNullOrWhiteSpace(x) && x != "x").Select(x => int.Parse(x)).ToArray();
		
		var parsed = lines[1].Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
		var list = new List<Bus>();
		for(int i=0;i<parsed.Length;i++){
			if(parsed[i] != "x") list.Add(new Bus { BusId = int.Parse(parsed[i]), PositionId = i});
		}
		Positions = list;
	}
	
	public Departure NextArrival()
	{
		return ParsedIds
			.Select(pi => 
				new {
					Id = pi,
					Mod = pi - (StartingTimeStamp % pi),
				})
			.OrderBy(pi => pi.Mod)
			.Select(pi =>
				new Departure
				{
					Id = pi.Id,
					MinutesWaiting = pi.Mod,
					TimeStamp = StartingTimeStamp + pi.Mod
				})
			.FirstOrDefault();
	}

	public long EarliestCascadingStart(){
		
		long timeStamp = Positions[0].BusId;
		
		//Crafting a wolframalpha query for CRT
		// (Chinese remainder theorem)
		var sb = new StringBuilder();
		
		sb.Append("{");

		sb.Append($"Mod[t+0,{Positions.First().BusId}]==0");

		foreach (var s in Positions.Skip(1))
		{
			sb.Append($", Mod[t+{s.PositionId},{s.BusId}]==0");	
		}
		
		sb.Append("}");
		
		sb.ToString().Dump("Wolfram query");
		
		return timeStamp;
	}
	
	private bool CascadingDepartures(long timeStamp){
		for(int i=1;i<Positions.Count();i++){
			var mod = (timeStamp + Positions[i].PositionId) % Positions[i].BusId;
			if(mod != 0) return false;
		}
		return true;
	}
}