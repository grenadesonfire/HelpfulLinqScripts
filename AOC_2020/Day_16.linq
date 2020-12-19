<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day16")
		);
	
	FirstHalf(inputs);

	SecondHalf(inputs);
}

void FirstHalf(string[] lines)
{
	var rs = new TicketSet(lines);
	
	rs.ValidationErrorRate().Dump("Part 1");
}

void SecondHalf(string[] lines)
{
	var rs = new TicketSet(lines);
	
	rs.ReduceInvalid();
	
	rs.NearbyTickets.Select(nt => nt.Values[15]).OrderBy(v => v).ToList();
	
	var columnsToTotal = rs.ColumnsOrder().Where(r => r.Label.Contains("departure"));
	
	columnsToTotal.Sum(tt => rs.SumColumn(tt.Order));
	
	var total = (long)1;
	
	foreach(var columns in columnsToTotal){
		total *= rs.MyTicket.Values[columns.Dump().Order];
	}
	
	total.Dump("Part 2");
}

class TicketSet
{
	public RuleSet Rules { get; set; }
	public Ticket MyTicket { get; set; }
	public List<Ticket> NearbyTickets { get; set; }
	
	public TicketSet(string[] lines){
		var workingIdx = 0;
		
		Rules = new RuleSet();
		
		workingIdx = Rules.Initialize(lines);
		
		MyTicket = new Ticket();
		
		workingIdx = MyTicket.Initialize(lines, workingIdx);
		
		NearbyTickets = Ticket.ReadTickets(lines, workingIdx);
	}

	public int ValidationErrorRate()
	{
		return NearbyTickets.Sum(nt => Rules.TicketErrorTotal(nt));
	}
	
	public void ReduceInvalid()
	{
		NearbyTickets = NearbyTickets.Where(nt => Rules.HasTicketErrorTotal(nt)).ToList();
	}

	internal List<ColumnOrder> ColumnsOrder()
	{
		return Rules.ColumnsOrder(MyTicket, NearbyTickets);
	}

	internal long SumColumn(int column)
	{
		return NearbyTickets.Sum(nt => nt.Values[column]);
	}
}

class Ticket {
	public List<int> Values { get; set; }
	
	public Ticket()
	{
		Values = new List<int>();
	}
	
	public Ticket(string ticketInfo)
	{
		Values = ticketInfo.Split(',').Select(t => int.Parse(t)).ToList();
	}

	public int Initialize(string[] lines, int workingIdx)
	{
		Values = lines[workingIdx+1].Split(',').Select(t => int.Parse(t)).ToList();
		return workingIdx+3;
	}
	
	public static List<Ticket> ReadTickets(string[] lines, int workingIdx)
	{
		var ret = new List<Ticket>();
		
		for(var lineIdx = workingIdx+1; lineIdx < lines.Length;lineIdx++)
		{
			ret.Add(new Ticket(lines[lineIdx]));
		}
		
		return ret;
	}
}

class RuleSet
{
	public List<Rule> Rules { get; set; }
	
	public RuleSet(){
		Rules = new List<Rule>();
	}
	
	public int Initialize(string[] lines)
	{
		var workingIdx = 0;
		
		while(!string.IsNullOrWhiteSpace(lines[workingIdx++]))
		{
			Rules.Add(new Rule(lines[workingIdx-1]));
		}
		
		return workingIdx;
	}

	public int TicketErrorTotal(Ticket nt)
	{
		return nt.Values.Sum(v => NoValidRanges(v));
	}
	
	public List<ColumnOrder> ColumnsOrder(Ticket starter, List<Ticket> others)
	{
		var rules = new List<List<Rule>>();
		
		for(var columnIdx=0;columnIdx<starter.Values.Count();columnIdx++)
		{
			var columnRules = new List<Rule>();
			
			foreach(var rule in Rules){
				if(rule.ValueIsWitin(starter.Values[columnIdx])){
					columnRules.Add(rule);
				}
			}
			
			rules.Add(columnRules);
		}
		
		foreach(var ticket in others){
			for(int rIdx=0;rIdx<rules.Count();rIdx++)
			{
				//Check if it needs to be reduced
				if(rules[rIdx].Count() > 1){
					rules[rIdx].RemoveAll(r => !r.ValueIsWitin(ticket.Values[rIdx]));
				}
			}
			
			TrimSet(rules);
			
			if(!rules.Any(r => r.Count() > 1)) break;
		}
		
		TrimSet(rules);

		return rules.Select((r, idx) =>
			new ColumnOrder
			{
				Label = r.FirstOrDefault().Label,
				Order = idx
			}).ToList();
	}
	
	private void TrimSet(List<List<Rule>> ruleset)
	{
		var modified = false;
		do{
			modified = false;
			foreach(var rs in ruleset.Where(r => r.Count() == 1))
			{
				foreach(var rs2 in ruleset.Where(r => r.Count() > 1 && r.Contains(rs.FirstOrDefault())))
				{
					modified = rs2.Remove(rs.FirstOrDefault());
				}
			}
		}while(modified);
	}
	
	private int NoValidRanges(int value){
		if(!Rules.Any(r => r.ValueIsWitin(value))) return value;
		return 0;
	}

	internal bool HasTicketErrorTotal(Ticket nt)
	{
		if(nt.Values.Any(v => !Rules.Any(r => r.ValueIsWitin(v)))) return false;
		return true;
	}
}

class ColumnOrder{
	public int Order { get; set; }
	public string Label { get; set; }
}

class Rule
{
	public string Label { get; set; }
	
	public int Range1Start { get; set; }
	public int Range1End { get; set; }
	
	public int Range2Start { get; set; }
	public int Range2End { get; set; }
	
	public Rule(string line){
		var splits = line.Split(new[] { ":", " or ", "-" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);
		try
		{
			Label = splits[0];
			Range1Start = int.Parse(splits[1]);
			Range1End = int.Parse(splits[2]);

			Range2Start = int.Parse(splits[3]);
			Range2End = int.Parse(splits[4]);
		}
		catch
		{
			splits.Dump("Error");
		}
	}

	internal bool ValueIsWitin(int value)
	{
		return (value >= Range1Start && value <= Range1End) || (value >= Range2Start && value <= Range2End);
	}
}