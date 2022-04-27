<Query Kind="Program" />

const string DIR = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\TestData\";
void Main()
{
	var decks = System.Text.Json.JsonSerializer.Deserialize<List<State>>(File.ReadAllText(Path.Combine(DIR, "Decks.json")));
	
	var d = decks.FirstOrDefault(d => d.Deckname == "Drytron");
	var desc = System.Text.Json.JsonSerializer.Deserialize<List<DeltaDescription>>(File.ReadAllText(Path.Combine(DIR, "Drytron.json")));
	
	Simulate(10, d, desc);
}

void Simulate(int simulations, State d, List<DeltaDescription> descs)
{
	var drytronCount = new int[6];
	
	for(var run=0;run<simulations;run++)
	{
		var s = d.Copy();
		var deck = s.Zones.FirstOrDefault(z => z.Label == "Deck");
		var hand = s.Zones.FirstOrDefault(z => z.Label == "Hand");


		deck.Cards = deck.Cards.OrderBy(c => Guid.NewGuid()).ToList();
		
		for(var draw=0;draw<5;draw++)
		{
			hand.Cards.Add(deck.Cards.First());
			deck.Cards.RemoveAt(0);
		}
		
		//Some opening hand stats
		drytronCount[hand.Cards.Distinct().Count(c => c.Name.Contains("Drytron") || c.Name.Contains("Cyber Emergency"))]++;
		
		//Run some simulations
		CreateMap(s, descs);
	}

	drytronCount.Select(c => (c * 1.0m / simulations * 100m)).Dump("Testing the stats");
}

void ConvertTextDocument(string path)
{
	var lines = File.ReadAllLines(path);
	
	var state = new State{
		Zones = new List<Zone>
		{
			new Zone { Label = "Hand", Cards = new List<Card>() },
			new Zone { Label = "Field", Cards = new List<Card>() },
			new Zone { Label = "Graveyard", Cards = new List<Card>() },
			new Zone { Label = "Banish", Cards = new List<Card>() },
			new Zone { Label = "Deck", Cards = new List<Card>() },
			new Zone { Label = "ExtraDeck", Cards = new List<Card>() },
			new Zone { Label = "SideDeckaDeck", Cards = new List<Card>() },
		}
	};
	var idx = 1;
	do
	{
		if (lines[idx] == "---------") { idx++; continue; }
		
		var split = lines[idx].Split("x ");
		
		for(var count=0;count<int.Parse(split[0]);count++)
		{
			state
				.Zones
				.FirstOrDefault(z => z.Label == "Field")
				.Cards
				.Add(new Card { Name = split[1] });
		}
		
		idx++;
	}while(lines[idx] != "Extra Deck:");
	
	idx++;
	
	do
	{
		if (lines[idx] == "---------") { idx++; continue; }

		var split = lines[idx].Split("x ");

		for (var count = 0; count < int.Parse(split[0]); count++)
		{
			state
				.Zones
				.FirstOrDefault(z => z.Label == "ExtraDeck")
				.Cards
				.Add(new Card { Name = split[1] });
		}

		idx++;
	} while (lines[idx] != "Side Deck:");
	
	System.Text.Json.JsonSerializer.Serialize(state).Dump();
}

void DrytronTest()
{
	var state = DrytronTest1_State();
	var deltas = DrytronTest1_DeltaDescription();
	
	CreateMap(state, deltas);
}

void CreateMap(State state, List<DeltaDescription> deltas)
{
	var q = new Queue<PotentialMove>();
	var startingMoves = new List<PotentialMove>();
	//Initial 
	//Build
	foreach (var d in deltas)
	{
		if (d.Valid(state))
		{
			var pmove = new PotentialMove
			{
				Move = d,
				State = state.Copy(),
			};

			q.Enqueue(pmove);

			startingMoves.Add(pmove);
		}
	}

	while (q.Count() != 0)
	{
		var move = q.Dequeue();

		state = move.Move.Execute(move.State);

		foreach (var d in deltas)
		{
			if (d.Valid(state))
			{
				var pmove = new PotentialMove
				{
					Move = d,
					State = state.Copy(),
				};

				move.AddChild(pmove);

				q.Enqueue(pmove);
			}
		}
	}

	if (startingMoves.Count() != 0)
	{
		startingMoves.Dump("Map");
	}
	else{
		state.Dump("No Map");
	}
	"Finished!".Dump();
}

class PotentialMove
{
	public State State { get; set; }
	public DeltaDescription Move { get; set; }
	public List<PotentialMove> ChildMoves { get; set; } = new List<PotentialMove>();

	internal void AddChild(PotentialMove pmove)
	{
		ChildMoves.Add(pmove);
	}
}

class State
{
	public string Deckname { get; set; }
	public List<Zone> Zones { get; set; }
	
	public List<DeltaDescription> History { get; set; } = new List<DeltaDescription>();

	internal State Copy()
	{
		var s = new State();
		s.Zones = new List<Zone>();
		s.Zones.AddRange(Zones.Select(z => z.Copy()));
		s.History.AddRange(History);
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
		s.History.Add(this);
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


///
/// 
/// 
/// 
/// 
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
					new Card { Name = "Meteonis Drytron" }
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
					CardName = "Drytron Alpha Thuban",
					Start = "Hand",
					End = "Graveyard"
				},
				new Delta {
					CardName = "Drytron Al-Zeta",
					Start = "Hand",
					End = "Field"
				},
				new Delta {
					CardName = "Meteonis Drytron",
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
