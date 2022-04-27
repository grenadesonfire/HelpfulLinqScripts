<Query Kind="Program" />

void Main()
{
	var PARTS_DIR = @"C:\Users\Nick\AppData\Roaming\Forge\decks\commander\JS_Cube";
	var list = System.Text.Json.JsonSerializer.Deserialize<JSCmdrCubeDefinition>(File.ReadAllText(Path.Combine(PARTS_DIR, "parts.json")));	
	
	//var randDeck = Pairing.Random(list, list.DeckHalves.FirstOrDefault(dh => dh.File == "Sakashima.dck"));
	var randDeck = Pairing.RandomWithRare(list);
	
	new {
		Identity = randDeck.Identity,
		Partner1 = randDeck.Partner1.File,
		Partner2 = randDeck.Partner2.File
	}.Dump();
	
	File.WriteAllLines(@"C:\Users\Nick\AppData\Roaming\Forge\decks\commander\TestJSDeck.dck", randDeck.CombineLists(PARTS_DIR));
}

// You can define other methods, fields, classes and namespaces here

class Pairing {
	public DeckHalf Partner1 { get; set; }
	public DeckHalf Partner2 { get; set; }
	public ColorPack Bridge { get; set; }
	public string Identity { get; set; }

	internal static Pairing Random(JSCmdrCubeDefinition list)
	{
		var partner1 = list.DeckHalves.OrderBy(dh => Guid.NewGuid()).FirstOrDefault();
		var partner2 = list.DeckHalves.OrderBy(dh => Guid.NewGuid()).FirstOrDefault(dh => dh.Identity != partner1.Identity);
		
		return FinishPairing(list, partner1, partner2);
	}

	internal static Pairing Random(JSCmdrCubeDefinition list, DeckHalf deckHalf)
	{
		var partner2 = list.DeckHalves.OrderBy(dh => Guid.NewGuid()).FirstOrDefault(dh => dh.Identity != deckHalf.Identity && dh.Rarity == "U");
		
		return FinishPairing(list, deckHalf, partner2);
	}

	internal static Pairing RandomWithRare(JSCmdrCubeDefinition list)
	{
		var partner1 = list.DeckHalves.OrderBy(dh => Guid.NewGuid()).FirstOrDefault(dh => dh.Rarity == "R");
		var partner2 = list.DeckHalves.OrderBy(dh => Guid.NewGuid()).FirstOrDefault(dh => dh.Identity != partner1.Identity && dh.Rarity == "U");

		return FinishPairing(list, partner1, partner2);
	}

	static Pairing FinishPairing(JSCmdrCubeDefinition list, DeckHalf partner1, DeckHalf partner2)
	{
		var bridge = list.ColorFixPacks.FirstOrDefault(cfp => cfp.Identity.Contains(partner1.Identity) && cfp.Identity.Contains(partner2.Identity));

		return new Pairing
		{
			Partner1 = partner1,
			Partner2 = partner2,
			Bridge = bridge,
			Identity = bridge.Identity
		};
	}

	internal List<string> CombineLists(string partsDir)
	{
		var list = new List<string>();
		list.Add("[metadata]");
		list.Add("Name=TestJSDeck");
		list.Add("[Commander]");
		
		var p1File = File.ReadAllLines(Path.Combine(partsDir, Partner1.File));
		var p2File = File.ReadAllLines(Path.Combine(partsDir, Partner2.File));
		
		list.Add(p1File.Skip(1).FirstOrDefault());
		list.Add(p2File.Skip(1).FirstOrDefault());
		
		list.Add("[Main]");
		
		list.AddRange(p1File.Skip(3));
		list.AddRange(p2File.Skip(3));
		
		var bFile = File.ReadAllLines(Path.Combine(partsDir, Bridge.File));
		
		list.AddRange(bFile.Skip(1));
		
		return list;
	}
}

class JSCmdrCubeDefinition {
	public List<ColorPack> ColorFixPacks { get; set; }
	public List<DeckHalf> DeckHalves { get; set; }
}

class ColorPack : CardList {
	
}

class DeckHalf : CardList {
	public string Rarity { get; set; }
}

abstract class CardList
{
	public string Identity { get; set; }
	public string File { get; set; }
}