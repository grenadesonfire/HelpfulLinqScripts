<Query Kind="Program" />

void Main()
{
	var state = DrytronTest1_State();
	var deltas = DrytronTest1_DeltaDescription();
	
	foreach(var d in deltas){
		if(d.Valid(state))
		{
			var s2 = d.Execute(state);
			s2.Dump("After execution");
		}
	}
}

State DrytronTest1_State()
{
	return new State
	{
		Zones = new List<Zone>(){
			new Zone {
				Label = "Hand",
				Cards = new List<Card>
				{
					new Card { Name = "Drytron Alpha Thuban" },
					new Card { Name = "Drytron Al-Zeta" },
				}
			},
			new Zone {
				Label = "Field",
				Cards = new List<Card>()
			},
			new Zone {
				Label = "Deck",
				Cards = new List<Card>
				{
					new Card { Name = "Diviner of the Herald" },
					new Card { Name = "Cyber Angel Benten" },
				}
			},
			new Zone {
				Label = "Graveyard",
				Cards = new List<Card>()
			}
		}	
	};
}

List<DeltaDescription> DrytronTest1_DeltaDescription()
{
	return new List<DeltaDescription>
	{
		new DeltaDescription
		{
			Deltas = new List<Delta>{
				new Delta {
					CardName = "Drytron Alpha Thuban",
					Start = "Hand",
					End = "Field"
				},
				new Delta {
					CardName = "Diviner of the Herald",
					Start = "Deck",
					End = "Hand"
				},
				new Delta {
					CardName = "Cyber Angel Benten",
					Start = "Hand",
					End = "Graveyard"
				}
			}
		},
		new DeltaDescription
		{
			Deltas = new List<Delta>{
				new Delta {
					CardName = "Drytron Alpha Thuban",
					Start = "Hand",
					End = "Field"
				},
				new Delta {
					CardName = "Drytron Al-Zeta",
					Start = "Hand",
					End = "Graveyard"
				},
				new Delta {
					CardName = "Cyber Angel Benten",
					Start = "Deck",
					End = "Hand"
				}
			}
		},
		new DeltaDescription
		{
			Deltas = new List<Delta>{
				new Delta {
					CardName = "Drytron Al-Zeta",
					Start = "Graveyard",
					End = "Field"
				},
				new Delta {
					CardName = "Cyber Angel Benten",
					Start = "Hand",
					End = "Graveyard"
				},
				new Delta {
					CardName = "Diviner of the Herald",
					Start = "Deck",
					End = "Hand"
				}
			}
		},
	};
}

class State
{
	public List<Zone> Zones { get; set; }

	internal State Copy()
	{
		var s = new State();
		s.Zones = new List<Zone>();
		s.Zones.AddRange(Zones.Select(z => z.Copy()));
		return s;
	}
}

class Zone {
	public string Label { get; set; }
	public List<Card> Cards { get; set; }

	internal Zone Copy()
	{
		var z = new Zone { Label = Label, Cards = new List<Card>() };
		z.Cards.AddRange(Cards);
		return z;
	}
}

class DeltaDescription
{
	//public List<Requirement> Requirements { get; set; }
	public List<Delta> Deltas { get; set; }

	internal State Execute(State state)
	{
		var s = state.Copy();
		
		foreach(var d in Deltas)
		{
			var zStart = s.Zones.FirstOrDefault(z => z.Label == d.Start);
			var zEnd = s.Zones.FirstOrDefault(z => z.Label == d.End);
			
			var c = zStart.Cards.FirstOrDefault(ca => ca.Name == d.CardName);
			zStart.Cards.Remove(c);
			zEnd.Cards.Add(c);
		}
		
		return s;
	}

	internal bool Valid(State state)
	{
		foreach(var d in Deltas)
		{
			if(!d.Valid(state)) return false;
		}
		
		return true;
	}
}

class Delta
{
	public string CardName { get; set; }
	//Card to remove
	public string Start { get; set; }
	//Card to add
	public string End { get; set; }

	internal bool Valid(State state)
	{
		var z = state.Zones.FirstOrDefault(z => Start == z.Label);
		
		if(z == null || z.Cards.All(c => c.Name != CardName)) return false;
		
		return true;
	}
}

//class Requirement
//{
//	public string ZoneLabel { get; set; }
//	public string CardName { get; set; }
//}

class Card
{
	public string Name { get; set; }
}