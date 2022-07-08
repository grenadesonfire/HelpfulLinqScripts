<Query Kind="Program" />

const string DIR = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\TestData\";
void Main()
{
	var decks = System.Text.Json.JsonSerializer.Deserialize<List<State>>(File.ReadAllText(Path.Combine(DIR, "Decks.json")));

	//ConvertTextDocument(Path.Combine(DIR, "2022_07_Drytron.txt"));
	//ConvertTextDocument(Path.Combine(DIR, "2022_07_Despia.txt"));
	//DragonMaidTest(decks);

	//DrytronTest(decks);

	var despiaEvaluation = new List<GoalList>
		{
			new GoalList{
				Category = "Aluber / Branded Fusion",
				CardMatches = new List<CardMatches>{
					new CardMatches{
						Options = new List<CardMatch>{
							new CardMatch(){
								CardName = "Aluber the Jester of Despia"
							},
							new CardMatch(){
								CardName = "Branded Fusion"
							},
							new CardMatch(){
								CardName = "Branded Opening"
							}
						}
					}
				},
				MultipleInstancesAllowed = false
			},
			new GoalList{
				Category = "Despia + Fusion",
				CardMatches = new List<CardMatches>{
					new CardMatches{
						Options = new List<CardMatch>{
							new CardMatch(){
								CardName = "Dramaturge of Despia"
							},
							new CardMatch(){
								CardName = "Despian Tragedy"
							}
						}
					},
					new CardMatches{
						Options = new List<UserQuery.CardMatch>{
							new CardMatch(){
								CardName = "Polymerization"
							}
						}
					}
				},
				MultipleInstancesAllowed = false
			},
			new GoalList{
				Category = "Allure",
				CardMatches = new List<CardMatches>{
					new CardMatches{
						Options = new List<CardMatch>{
							new CardMatch(){
								CardName = "Allure of Darkness"
							},
						}
					}
				},
				MultipleInstancesAllowed = false
			},
		};


	EvaluteHands(
			2 * 1000* 1000,
			decks.FirstOrDefault(d => d.Deckname == "DespiaBranded"),
			despiaEvaluation,
			false);

	//	var drytronEvaluation = new List<GoalList>
	//		{
	//			new GoalList{
	//				Category = "Success",
//				CardMatches = new List<CardMatches>{
//					new CardMatches{
//						Options = new List<CardMatch>{
//							new CardMatch(){
//								PropertyMatch = new List<string>(){
//									"Drytron",
//									"LV1"
//								}
//							},
//							new CardMatch(){
//								CardName = "Drytron Nova"
//							},
//							new CardMatch(){
//								CardName = "Foolish Burial"
//							},
//							new CardMatch(){
//								CardName = "Drytron Fafnir"
//							},
//							new CardMatch(){
//								CardName = "Cyber Emergency"
//							},
//							new CardMatch(){
//								CardName = "Jack-In-The-Hand"
//							},
//							new CardMatch(){
//								CardName = "Ritual Sanctuary"
//							},
//							new CardMatch(){
//								CardName = "Diviner of the Herald"
//							},
//							new CardMatch(){
//								CardName = "Cyber Angel Benten"
//							},
//							new CardMatch(){
//								CardName = "Terraforming"
//							}
//						}
//					},
//					new CardMatches{
//						Options = new List<CardMatch>{
//							new CardMatch(){
//								PropertyMatch = new List<string>(){
//									"Drytron",
//									"LV1"
//								}
//							},
//							new CardMatch(){
//								CardName = "Jack-In-The-Hand"
//							},
//							new CardMatch(){
//								CardName = "Drytron Nova"
//							},
//							new CardMatch(){
//								CardName = "Foolish Burial"
//							},
//							new CardMatch(){
//								CardName = "Drytron Fafnir"
//							},
//							new CardMatch(){
//								CardName = "Cyber Emergency"
//							},
//						}
//					}
//				},
//				MultipleInstancesAllowed = false
//			}
//		};
//
//
//	EvaluteHands(
//		2 * 1000 * 1000, //2 * 1000* 1000,
//		decks.FirstOrDefault(d => d.Deckname == "Drytron202207DK"),
//		drytronEvaluation);
}

class CardMatch
{
	public string CardName { get; set; }
	public List<string> PropertyMatch { get; set; }

	internal bool Match(Card c)
	{
		if(!string.IsNullOrEmpty(CardName)) return c.Name == CardName;
		return PropertyMatch.All(p => c.Properties.Contains(p));
	}
}

class CardMatches
{
	public List<CardMatch> Options { get; set; }

	internal List<Card> Valid(Zone hand)
	{
		// Find if there are any cards that have all the properties for this match
		//TODO: send back the list of cards that qualify
		var ret = new List<Card>();
		foreach (var list in Options)
		{
			var tmp = hand.Cards.Where(c => list.Match(c) && !ret.Contains(c));
			if(tmp.Count() > 0) ret.AddRange(tmp);
		}
		return ret;
	}
}

class GoalList
{
	public string Category { get; set; }
	public List<CardMatches> CardMatches { get; set; }
	public bool MultipleInstancesAllowed { get; set; }

	internal bool Valid(Zone hand)
	{
		var chosen = new List<List<Card>>();
		
		foreach(var cm in CardMatches)
		{			
			var tmp = cm.Valid(hand);
			if(tmp.Count() == 0) return false;
			
			chosen.Add(tmp);
		}
		chosen = chosen.OrderBy(c => c.Count()).ToList();
		//var names = hand.Cards.Select(c => c.Name).ToList();
		while(chosen.Count() > 0)
		{
			var f = chosen.FirstOrDefault();
			chosen.Remove(f);
			foreach(var c in chosen)
			{
				c.Remove(f.FirstOrDefault());
				if(c.Count()==0) return false;
			}
		}
		
		return true;
	}
}

void EvaluteHands(int limit, State deck, List<GoalList> validHands, bool debug)
{
	deck.Expand();
	new {
		DeckSize = deck["Deck"].Cards.Count(),
		ExtraDeckSize = deck["ExtraDeck"].Cards.Count()
	}.Dump("Starting Counts");
	
	var sw = new Stopwatch();
	sw.Start();
	var results = new Dictionary<string, int>();
	
	for(var test = 0;test<limit;test++)
	{
		var td = deck.Copy();
		
		var deckZone = td["Deck"];
		var handZone = td["Hand"];
		
		handZone.AddCard(deckZone.RemoveRandomCard());
		handZone.AddCard(deckZone.RemoveRandomCard());
		handZone.AddCard(deckZone.RemoveRandomCard());
		handZone.AddCard(deckZone.RemoveRandomCard());
		handZone.AddCard(deckZone.RemoveRandomCard());
		
		var res = EvaluateHand(td, validHands);

		if (res == string.Empty)
		{
			res = "Fail";
			if(debug) handZone.Cards.Select(c => c.Name).Dump();
		}
		
		if(!results.ContainsKey(res)) results.Add(res, 0);
		
		results[res]++;
	}
	sw.Stop();
	
	var total = results.Keys.Sum(r => results[r]);

	new
	{
		Percentages = results.Keys.Select(k => new { Name = k, Chance = results[k] * 1.0m / total * 1.0m * 100m }).OrderByDescending(k => k.Chance)
	}.Dump("Resulting Hands");
	
	results.Dump("Hands");
	
	sw.Elapsed.TotalSeconds.Dump();
}

string EvaluateHand(State td, List<GoalList> validHands)
{
	var hand = td["Hand"];
	foreach(var vhand in validHands)
	{		
		if(vhand.Valid(hand)) return vhand.Category;
	}
	
	return string.Empty;
}

#region simulator
List<PotentialMove> GetLeafs(List<PotentialMove> potentialMoves)
{
	var ret = new List<PotentialMove>();
	var q = new Queue<PotentialMove>();
	
	potentialMoves.ForEach(m => q.Enqueue(m));
	
	while(q.Count() != 0)
	{
		var dq = q.Dequeue();
		
		// Go through the children
		if(dq.ChildMoves != null && dq.ChildMoves.Count() > 0)
		{
			dq.ChildMoves.ForEach(cm => q.Enqueue(cm));
		}
		else {
			ret.Add(dq);
		}
	}
	
	return ret.OrderBy(r => r.State.History.Count()).ToList();
}

void Simulate(int simulations, State d, List<DeltaDescription> descs)
{
	var drytronCount = new int[6];

	for (var run = 0; run < simulations; run++)
	{
		var s = d.Copy();
		var deck = s.Zones.FirstOrDefault(z => z.Label == "Deck");
		var hand = s.Zones.FirstOrDefault(z => z.Label == "Hand");


		deck.Cards = deck.Cards.OrderBy(c => Guid.NewGuid()).ToList();

		for (var draw = 0; draw < 5; draw++)
		{
			hand.Cards.Add(deck.Cards.First());
			deck.Cards.RemoveAt(0);
		}

		//Some opening hand stats
		//drytronCount[hand.Cards.Distinct().Count(c => c.Name.Contains("Drytron") || c.Name.Contains("Cyber Emergency"))]++;

		//Run some simulations
		
		GetLeafs(CreateMap(s, descs));
	}

	//drytronCount.Select(c => (c * 1.0m / simulations * 100m)).Dump("Testing the stats");
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

List<PotentialMove> CreateMap(State state, List<DeltaDescription> deltas)
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
		var numQueued = 0;

		state = move.Move.Execute(move.State, move.Id);

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
				numQueued++;
			}
		}
		
		if(numQueued == 0){
			var pmove = new PotentialMove
			{
				State = state.Copy()
			};
			
			move.AddChild(pmove);
		}
	}

	if (startingMoves.Count() != 0)
	{
		return startingMoves;
	}
	else
	{
		return new List<PotentialMove>() {
			new PotentialMove{
				ChildMoves = null,
				Move = null,
				State = state
			}};
	}
}
#endregion

#region Deck_Helpers
class PotentialMove
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public State State { get; set; }
	public DeltaDescription Move { get; set; }
	public List<PotentialMove> ChildMoves { get; set; } = new List<PotentialMove>();

	internal void AddChild(PotentialMove pmove)
	{
		ChildMoves.Add(pmove);
	}
}

class HistoryItem {
	public Guid PMId { get; set; }
	public DeltaDescription Delta { get; set; }
}

class State
{
	public string Deckname { get; set; }
	public List<Zone> Zones { get; set; }
	
	public List<HistoryItem> History { get; set; } = new List<HistoryItem>();

	internal State Copy()
	{
		var s = new State();
		s.Zones = new List<Zone>();
		s.Zones.AddRange(Zones.Select(z => z.Copy()));
		s.History.AddRange(History);
		return s;
	}

	internal State Expand()
	{
		foreach(var z in Zones.Where(z => z.Cards.Any(c => c.Count > 1)))
		{
			var multiCards = z.Cards.Where(c => c.Count > 1).ToList();
			foreach(var c in multiCards)
			{
				z.Cards.Remove(c);
				
				z.Cards.AddRange(c.CopyByCount());
			}
		}
		
		return this;
	}

	public Zone this[string ZoneName]
	{
		get {
			return Zones.FirstOrDefault(z => z.Label == ZoneName);
		}
	}
}

class Zone {
	public string Label { get; set; }
	public List<Card> Cards { get; set; }

	internal void AddCard(Card c)
	{
		Cards.Add(c);
	}

	internal Zone Copy()
	{
		var z = new Zone { Label = Label, Cards = new List<Card>() };
		z.Cards.AddRange(Cards);
		return z;
	}

	internal Card RemoveCard(string v)
	{
		var c = Cards.FirstOrDefault(ca => ca.Name == v);
		Cards.Remove(c);
		return c;
	}

	internal Card RemoveRandomCard()
	{
		return Cards.OrderBy(c => Guid.NewGuid()).FirstOrDefault();
	}
}

class DeltaDescription
{
	//public List<Requirement> Requirements { get; set; }
	public bool UniquePerTurn { get; set; }
	public List<Delta> Deltas { get; set; }

	internal static List<DeltaDescription> ReadFile(State s, string file)
	{
		var deltas = System.Text.Json.JsonSerializer.Deserialize<List<DeltaDescription>>(File.ReadAllText(Path.Combine(DIR, file)));
		var ret = new List<DeltaDescription>();
		
		foreach(var delta in deltas)
		{
			if(delta.Deltas.Any(d => d.Properties != null && d.Properties.Count() != 0))
			{
				var miniRet = new List<DeltaDescription>();
				foreach(var theta in delta.Deltas)
				{
					if(miniRet.Count() == 0){
						if(theta.Properties == null){
							miniRet.Add(
								new DeltaDescription{
									Deltas = new List<Delta>{
										theta
									}
								});
						}
						else
						{
							var applicable = new List<Card>();

							foreach (var z in s.Zones)
							{
								if (z.Cards.Any(c => c.HasProperties(theta.Properties))) applicable.AddRange(z.Cards.Where(c => c.HasProperties(theta.Properties)));
							}
							
							foreach(var c in applicable)
							{
								miniRet.Add(
									new DeltaDescription
										{
											Deltas = new List<Delta>{
												new Delta{
													CardName = c.Name,
													Start = theta.Start,
													End = theta.End
												}
											}
										});
							}
						}
					}
					//Expand the existing
					else
					{
						if (theta.Properties == null)
						{
							foreach(var t in miniRet){
								if(!t.Deltas.Any(d => d.CardName == theta.CardName)) t.Deltas.Add(theta);
							}
						}
						else
						{
							
							var applicable = new List<Card>();

							foreach(var z in s.Zones){
								if(z.Cards.Any(c => c.HasProperties(theta.Properties))) applicable.AddRange(z.Cards.Where(c => c.HasProperties(theta.Properties)));
							}
							
							var miniRet2 = new List<DeltaDescription>();
							
							foreach(var card in applicable)
							{
								foreach(var dd in miniRet)
								{
									if (!dd.Deltas.Any(d => d.CardName == card.Name)){
										var dd2 = dd.Copy();
										dd2.Deltas.Add(
											new Delta
											{
												CardName = card.Name,
												Start = theta.Start,
												End = theta.End
											});
										miniRet2.Add(dd2);
									}
								}
							}
							
							miniRet = miniRet2;
						}
					}
				}

				ret.AddRange(miniRet);
			}
			else{
				ret.Add(delta);
			}
		}
		
		return ret;
	}

	DeltaDescription Copy()
	{
		return new DeltaDescription{
			UniquePerTurn = UniquePerTurn,
			Deltas = Deltas.Select(d => d).ToList()
		};
	}

	internal State Execute(State state, Guid Id)
	{
		var s = state.Copy();
		
		foreach(var d in Deltas)
		{
			var zStart = s.Zones.FirstOrDefault(z => z.Label == d.Start);
			var zEnd = s.Zones.FirstOrDefault(z => z.Label == d.End);
			Card c = null;
			
			if (d.Properties == null || d.Properties.Count() == 0)
			{
				c = zStart.Cards.FirstOrDefault(ca => ca.Name == d.CardName);
			}
			else
			{
				c = zStart.Cards.FirstOrDefault(ca => d.Properties.All(p => ca.Properties.Contains(p)));
			}
			
			if(c == null) continue;//throw new Exception("oof no card, them properties though");

			zStart.Cards.Remove(c);
			zEnd.Cards.Add(c);
		}
		s.History.Add(
			new HistoryItem
			{
				PMId = Id,
				Delta = this
			});
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
	public List<string> Properties { get; set; }
	public int Count { get; set; }
	//Card to remove
	public string Start { get; set; }
	//Card to add
	public string End { get; set; }

	internal bool Valid(State state)
	{
		var z = state.Zones.FirstOrDefault(z => Start == z.Label);
		try
		{
			if (Properties != null && z.Cards.Any(c => c.Properties.Count() != 0 && ValidProperties(c)))
				return true;
			else if (
				z == null || 
				(Count != 0 && z.Cards.Count(c => c.Name != CardName) == Count) ||
				(Count == 0 && z.Cards.All(c => c.Name != CardName))) return false;

			return true;
		}
		catch(Exception ex)
		{
			Console.WriteLine($"Label: {z?.Label} CardName: {CardName}");
			return false;
		}
	}

	bool ValidProperties(Card c)
	{
		var ret = this.Properties.All(p => c.Properties.Contains(p));
		return ret;
	}
}

class Card
{
	public string Name { get; set; }
	public int Count { get; set; }
	public List<string> Properties { get; set; } = new List<string>();

	internal IEnumerable<Card> CopyByCount()
	{
		var ret = new List<Card>();
		
		for(var idx=0;idx<Count;idx++)
		{
			ret.Add(
				new Card{
					Name = Name,
					Properties = Properties
				});
		}
		
		return ret;
	}

	internal bool HasProperties(List<string> properties)
	{
		return properties.All(p => Properties.Contains(p));
	}
}
#endregion

#region test_cases
void DrytronTest(List<State> decks)
{
	// Drytron
	var d = decks.FirstOrDefault(d => d.Deckname == "DrytronMD");
	var desc = DeltaDescription.ReadFile(d, "DrytronMD.json");
	d = d.Expand();
	//desc.Dump("Moves");
	var deckZone = d["Deck"];
	var handZone = d["Hand"];

	handZone.AddCard(deckZone.RemoveCard("Drytron Alpha Thuban"));
	handZone.AddCard(deckZone.RemoveCard("Drytron Zeta Aldhibah"));
	var leafs = GetLeafs(CreateMap(d, desc));
	leafs.Count().Dump("Total: ");
	leafs
		.Where(d =>
			d.State["Field"]
				.Cards
				.Any(c => c.Name == "Herald Of Perfection") &&
			d.State["Field"]
				.Cards
				.Any(c => c.Name == "Herald Of Perfection")).Dump("Finished");

	//Simulate(10, d, desc);
}

void DragonMaidTest(List<State> decks)
{
	var d = decks.FirstOrDefault(d => d.Deckname == "Dragonmaid").Expand();
	var desc = System.Text.Json.JsonSerializer.Deserialize<List<DeltaDescription>>(File.ReadAllText(Path.Combine(DIR, "Dragonmaid.json")));
	desc.Count().Dump();
	var deckZone = d["Deck"];
	var handZone = d["Hand"];

	// Situation 1
	//handZone.AddCard(deckZone.RemoveCard("Solemn Strike"));
	//handZone.AddCard(deckZone.RemoveCard("Ash Blossom & Joyous Spring"));
	//handZone.AddCard(deckZone.RemoveCard("Gold Sarcophagus"));
	//handZone.AddCard(deckZone.RemoveCard("Dragonmaid Hospitality"));
	//handZone.AddCard(deckZone.RemoveCard("Nurse Dragonmaid"));
	// Situation 2
	//handZone.AddCard(deckZone.RemoveCard("Nurse Dragonmaid"));
	//handZone.AddCard(deckZone.RemoveCard("Dragonmaid Tidying"));
	//handZone.AddCard(deckZone.RemoveCard("Dragonmaid Changeover"));
	//handZone.AddCard(deckZone.RemoveCard("Monster Reborn"));
	//handZone.AddCard(deckZone.RemoveCard("World Legacy Guardragon"));
	// Situation 3
	//handZone.AddCard(deckZone.RemoveCard("Noctovision Dragon"));
	//handZone.AddCard(deckZone.RemoveCard("Noctovision Dragon"));
	//handZone.AddCard(deckZone.RemoveCard("Dragonmaid Changeover"));
	//handZone.AddCard(deckZone.RemoveCard("Dragonmaid Tinkhec"));
	//handZone.AddCard(deckZone.RemoveCard("World Legacy Guardragon"));
	// Situation 4
	handZone.AddCard(deckZone.RemoveCard("Dragonmaid Hospitality"));
	handZone.AddCard(deckZone.RemoveCard("Parlor Dragonmaid"));
	handZone.AddCard(deckZone.RemoveCard("Ash Blossom & Joyous Spring"));
	handZone.AddCard(deckZone.RemoveCard("Dragonmaid Tidying"));
	handZone.AddCard(deckZone.RemoveCard("Red-Eyes Darkness Metal Dragon"));
	GetLeafs(CreateMap(d, desc)).Dump("Finished");
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
#endregion