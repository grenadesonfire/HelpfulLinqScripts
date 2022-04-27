<Query Kind="Program">
  <NuGetReference>DotNetSeleniumExtras.WaitHelpers</NuGetReference>
  <NuGetReference>Selenium.Chrome.WebDriver</NuGetReference>
  <NuGetReference>Selenium.Support</NuGetReference>
  <NuGetReference>Selenium.WebDriver</NuGetReference>
  <Namespace>OpenQA.Selenium</Namespace>
  <Namespace>OpenQA.Selenium.Chrome</Namespace>
  <Namespace>OpenQA.Selenium.Support.UI</Namespace>
  <Namespace>SeleniumExtras.WaitHelpers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var deckRepository = @"C:\Users\Nick\AppData\Roaming\Forge\decks\commander";
	var user = @"draconick";
	var pass = @"4KhfUPy@uFRFsm2";
	
	//var decks = Forge.ConvertDirectory(deckRepository).ToList();
	//
	////var scryfall = new ScryFallApi();
	//
	////scryfall.GetTokens(decks);
	//
	//RunStats(decks, "Chromatic Lantern", true);
	
	//decks.SelectMany(d => d.MainDeck).Select(d => d.Name).OrderBy(d => d).Dump();
	
	//var deckStats = new Deckstats(user, pass);
	//
	//deckStats.UploadDecks(decks);
	
	//var moxField = new MoxField("draconick", "QSh6GYDNEmZz9dWoCpLP");
	//
	//moxField.UploadDecks(decks);
	
	//moxField.GetTokens();
	
	//Cockatrice.Convert(decks, @"C:\Users\Nick\AppData\Local\Cockatrice\Cockatrice\decks");
	
	
}

IEnumerable<CommanderDeck> DecksThatRun(List<CommanderDeck> decks, string card)
{
	return decks.Where(d => d.Commanders.Any(c => c.Name.Contains(card)) || d.MainDeck.Any(c => c.Name.Contains(card)));
}

void RunStats(List<CommanderDeck> decks, string specific = null, bool onlySearch = false)
{
	var maindecks = decks
		.SelectMany(d => d.MainDeck)
		.GroupBy(d => d.Name)
		.OrderByDescending(d => d.Count());

	var commanderStats =
		decks
		.SelectMany(d => d.Commanders)
		.GroupBy(d => d.Name)
		.OrderByDescending(d => d);

	var stats =
			maindecks.Select(d =>
			new
			{
				CardName = d.Key,
				Total = d.Sum(c => c.Count),
			});


	stats.Count().Dump("Unique cards");
	if (!onlySearch)
	{
		stats.OrderByDescending(s => s.Total).Take(25).Dump("Top 25 most played cards");

		stats
			.OrderByDescending(s => s.Total)
			.Where(s => !s.CardName.Contains("Forest"))
			.Where(s => !s.CardName.Contains("Island"))
			.Where(s => !s.CardName.Contains("Swamp"))
			.Where(s => !s.CardName.Contains("Plains"))
			.Where(s => !s.CardName.Contains("Mountain"))
			.Take(25).Dump("Top 25 most played minus basic lands");
	}
		
	if(specific != null)
	{
		stats
			.Where(s => s.CardName == specific)
			.Select(s =>
				new
				{
					Decks = decks.Where(d => d.MainDeck.Any(md => md.Name == specific)).Select(d => new { Name = d.Name, Count = d.MainDeck.Count(md => md.Name == specific)})
				}
			)
			.Dump($"All decks with {specific}");
			
	}
}

public class MoxField
{
	ChromeDriver _driver;
	string _username;
	string _password;
	WebDriverWait _wait;
	
	public MoxField(string user, string pass){
		_username = user;
		_password = pass;
		
		_driver = new OpenQA.Selenium.Chrome.ChromeDriver(@"D:\SeleniumDrivers");
		_wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
	}
	
	private IWebElement WaitForId(string id){
		return _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(id)));
	}
	
	private IWebElement WaitForClassName(string className) => _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName(className)));
	
	private IWebElement WaitForName(string name) => _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name(name)));
	
	private void CheckEnvironmentVariable()
	{
		if (!Environment.GetEnvironmentVariable("PATH").Contains("Chrome"))
		{
			Environment.SetEnvironmentVariable(
				"PATH",
				Environment.GetEnvironmentVariable("PATH") + ";" + @"C:\Users\Nick\.nuget\packages\Selenium.Chrome.WebDriver\83.0.0\driver");
		}
	}

	private void Login()
	{
		_driver.Url = @"https://www.moxfield.com/account/signin";
		_driver.Navigate();

		var userElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("username")));

		userElement.SendKeys(_username);

		var passwordElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("password")));

		passwordElement.SendKeys(_password);

		var loginButton = _driver.FindElementByCssSelector("button.btn.btn-custom.btn-primary");

		loginButton.Click();
	}
	
	public void GetTokens()
	{
		try
		{
			Login();

			Task.Delay(5000).GetAwaiter().GetResult();

			_driver.Navigate().GoToUrl("https://moxfield.com/decks/personal");
			WaitForId("maincontent");

			var decksPresent = _driver.FindElementsByXPath("//tr/td/a").ToList();
		}
		catch(Exception ex)
		{
			_driver.Quit();
			_driver.Dispose();
		}
	}

	public void UploadDeckNoLogin(CommanderDeck deck)
	{
		_wait.Until(
			SeleniumExtras
				.WaitHelpers
				.ExpectedConditions
				.ElementIsVisible(
					By.XPath("//header/nav/div/form/div/button"))).Click();
		
		var name = WaitForId("name");
		name.SendKeys(deck.Name);
		
		var cmdr = WaitForId("commander");
		cmdr.SendKeys(deck.Commanders[0].Name);
		name.Click();
		
		if(deck.Commanders.Count() > 1){
			var prtnr = WaitForId("partner");
			prtnr.SendKeys(deck.Commanders[1].Name);
			name.Click();
		}
		
		var privateRadio = WaitForId("visibility-private");
		privateRadio.Click();
		
		var pastList = WaitForId("decklist-paste");
		pastList.Click();
		
		var deckList = WaitForName("importText");
		deckList.SendKeys(string.Join('\n', deck.MainDeck.Select(md => md.Name)));
		
		var submit = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//div[@class = 'modal-footer']/button/span")));
		
		submit.Click();
		WaitForName("deckview");
		
		_driver.Url = "https://moxfield.com";
		_driver.Navigate();
		
		Console.ReadLine();
	}

	public void UploadDecks(List<CommanderDeck> decks)
	{
		try
		{
			Login();
			
			Task.Delay(5000).GetAwaiter().GetResult();
			
			_driver.Navigate().GoToUrl("https://moxfield.com/decks/personal");
			WaitForId("maincontent");
			
			var map = MapExistingDecks(decks);
			
			var decksPresent = _driver.FindElementsByXPath("//tr/td/a").Select(md => md.Text).ToList();
			
			foreach (var deck in decks.Where(d => !decksPresent.Any(p => p.Trim() == d.Name)))
			{
				try
				{
					UploadDeckNoLogin(deck);
				}
				catch (Exception ex)
				{
					ex.Dump(deck.Name);
				}
			}
		}
		catch (Exception ex)
		{
			ex.Dump("Line 121");
		}
		finally
		{
			_driver.Quit();
			_driver.Dispose();
		}
	}

	private DeckMap MapExistingDecks(List<CommanderDeck> decks)
	{
		var file = "moxfield.json";
		var map = new DeckMap();
		if(File.Exists(file)){
			map = System.Text.Json.JsonSerializer.Deserialize<DeckMap>(File.ReadAllText(file));
		}
		
		//foreach(var deck in 
		return null;
	}
}

public class Deckstats
{
	private static string NAMEPATH_USERNAME = @"user";
	private static string NAMEPATH_PASSWORD = @"passwrd";
	
	ChromeDriver _driver;
	string _username;
	string _password;
	WebDriverWait _wait;
	
	public Deckstats(string username, string password)
	{
		CheckEnvironmentVariable();

		_driver = new OpenQA.Selenium.Chrome.ChromeDriver(@"D:\SeleniumDrivers");
		_username = username;
		_password = password;
		_wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
	}

	public void UploadDecks(List<CommanderDeck> decks)
	{
		try
		{
			Login();
			
			foreach (var deck in decks)
			{
				try
				{
					UploadDeckNoLogin(deck);
				}catch(Exception ex)
				{
					ex.Dump(deck.Name);
				}
			}
		}
		catch(Exception ex)
		{
			ex.Dump("Deckstats.UploadDecks -> Line 170");
		}
		finally
		{
			_driver.Quit();
			_driver.Dispose();
		}
	}

	public void UploadDeck(CommanderDeck deck)
	{
		try{
			Login();
			
			UploadDeckNoLogin(deck);
		}
		finally
		{
			_driver.Close();
			_driver.Quit();
			_driver.Dispose();
		}
	}
	
	private void UploadDeckNoLogin(CommanderDeck deck)
	{
		_driver.Navigate().GoToUrl(@"https://deckstats.net/deckbuilder/en/");

		var typeSelect = "deckbuilder_new_deck_dialog_format";
		_wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(typeSelect))).Click();

		var select = new OpenQA.Selenium.Support.UI.SelectElement(_driver.FindElementById(typeSelect));
		select.SelectByText("EDH / Commander");

		var nameElement = _driver.FindElementById("deckbuilder_new_deck_dialog_name");
		nameElement.Clear();
		nameElement.SendKeys($"{deck.Name}");
		
		
		var isPublicCB = _driver.FindElementById("deckbuilder_new_deck_dialog_is_public");
		isPublicCB.Click();

		_wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(.,'OK')]"))).Click();
		Task.Delay(1500).GetAwaiter().GetResult();

		if (_driver.FindElements(By.CssSelector("body > div > div > a.cc_btn_accept_all")).Count() > 0)
		{
			_wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("body > div > div > a.cc_btn_accept_all"))).Click();
		}
		
		//Enter in cards
		_wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(@"//div[@id='deckbuilder_footer_buttons_other']/button[2]"))).Click();

		_wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("deckbuilder_upload_list_dialog_textarea")));

		foreach (var c in deck.MainDeck)
		{
			var textArea = _driver.FindElementById("deckbuilder_upload_list_dialog_textarea");
			textArea.SendKeys($"{c.Count} {c.Name}");
			textArea.SendKeys("\n");
		}

		foreach (var c in deck.Commanders)
		{
			var textArea = _driver.FindElementById("deckbuilder_upload_list_dialog_textarea");
			textArea.SendKeys(c.Name);
			textArea.SendKeys("\n");
		}

		_driver.FindElements(By.CssSelector("button.button_primary")).ToList().FirstOrDefault(x => x.Displayed).Click();

		//Save
		_driver.FindElementById("deckbuilder_save_button").Click();
		Task.Delay(5000).GetAwaiter().GetResult();
	}

	private void CheckEnvironmentVariable()
	{
		if (!Environment.GetEnvironmentVariable("PATH").Contains("Chrome"))
		{
			Environment.SetEnvironmentVariable(
				"PATH",
				Environment.GetEnvironmentVariable("PATH") + ";" + @"C:\Users\Nick\.nuget\packages\Selenium.Chrome.WebDriver\83.0.0\driver");
		}
	}
	
	private void Login()
	{
		_driver.Url = @"https://www.deckstats.net";
		_driver.Navigate();

		var userElement = _driver.FindElementByName(NAMEPATH_USERNAME);

		userElement.SendKeys(_username);

		var passwordElement = _driver.FindElementByName(NAMEPATH_PASSWORD);

		passwordElement.SendKeys(_password);

		var loginButton = _driver.FindElementById("user_login_segment");

		loginButton.Click();
	}
}

public class ScryFallApi {
	private Root[] _db;
	public ScryFallApi(string databaseJson = @"C:\Users\Nick\Downloads\oracle-cards-20211024090357.json")
	{
		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		_db = System.Text.Json.JsonSerializer.Deserialize<Root[]>(File.ReadAllText(databaseJson));
		sw.Stop();
		
		sw.Elapsed.Dump("Load time");
	}

	internal void GetTokens(List<CommanderDeck> decks)
	{
		var tokensDict = new Dictionary<string,int>();
		foreach(var deck in decks)
		{
			var res = GetTokens(deck);
			foreach(var token in res)
			{
				if (tokensDict.ContainsKey(token)) tokensDict[token]++;
				else tokensDict.Add(token, 1);
			}
		}
		
		tokensDict.ToList().OrderBy(d => d.Key).Dump("Totals");
	}
	
	internal List<string> GetTokens(CommanderDeck deck)
	{
		var tokenDict = new Dictionary<string, int>();
		foreach(var card in deck.MainDeck)
		{
			var refCard = _db.FirstOrDefault(x => x.name == card.Name || (x.name.Contains("//") && x.name.Contains(card.Name)));
			if (refCard == null) { "Oops".Dump(card.Name); continue; }
			var tokens = refCard.all_parts?.Where(a => a.component == "token");
			
			if(tokens != null && tokens.Count() > 0){
				var tokenAdds = tokens.Select(t => t.name);
				
				foreach(var ta in tokenAdds){
					if(tokenDict.ContainsKey(ta)) tokenDict[ta]++;
					else tokenDict.Add(ta, 1);
				}
			}
		}
		return tokenDict.Keys.ToList();
	}

	#region Helper_Classes
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
	public class ImageUris
	{
		public string small { get; set; }
		public string normal { get; set; }
		public string large { get; set; }
		public string png { get; set; }
		public string art_crop { get; set; }
		public string border_crop { get; set; }
	}

	public class AllPart
	{
		public string @object { get; set; }
		public string id { get; set; }
		public string component { get; set; }
		public string name { get; set; }
		public string type_line { get; set; }
		public string uri { get; set; }
	}

	public class Legalities
	{
		public string standard { get; set; }
		public string future { get; set; }
		public string historic { get; set; }
		public string gladiator { get; set; }
		public string pioneer { get; set; }
		public string modern { get; set; }
		public string legacy { get; set; }
		public string pauper { get; set; }
		public string vintage { get; set; }
		public string penny { get; set; }
		public string commander { get; set; }
		public string brawl { get; set; }
		public string historicbrawl { get; set; }
		public string paupercommander { get; set; }
		public string duel { get; set; }
		public string oldschool { get; set; }
		public string premodern { get; set; }
	}

	public class Prices
	{
		public string usd { get; set; }
		public object usd_foil { get; set; }
		public object usd_etched { get; set; }
		public string eur { get; set; }
		public object eur_foil { get; set; }
		public string tix { get; set; }
	}

	public class RelatedUris
	{
		public string gatherer { get; set; }
		public string tcgplayer_infinite_articles { get; set; }
		public string tcgplayer_infinite_decks { get; set; }
		public string edhrec { get; set; }
		public string mtgtop8 { get; set; }
	}

	public class Root
	{
		public string @object { get; set; }
		public string id { get; set; }
		public string oracle_id { get; set; }
		public List<int> multiverse_ids { get; set; }
		public int mtgo_id { get; set; }
		public int tcgplayer_id { get; set; }
		public int cardmarket_id { get; set; }
		public string name { get; set; }
		public string lang { get; set; }
		public string released_at { get; set; }
		public string uri { get; set; }
		public string scryfall_uri { get; set; }
		public string layout { get; set; }
		public bool highres_image { get; set; }
		public string image_status { get; set; }
		public ImageUris image_uris { get; set; }
		public string mana_cost { get; set; }
		public double cmc { get; set; }
		public string type_line { get; set; }
		public string oracle_text { get; set; }
		public string power { get; set; }
		public string toughness { get; set; }
		public List<string> colors { get; set; }
		public List<string> color_identity { get; set; }
		public List<string> keywords { get; set; }
		public List<AllPart> all_parts { get; set; }
		public Legalities legalities { get; set; }
		public List<string> games { get; set; }
		public bool reserved { get; set; }
		public bool foil { get; set; }
		public bool nonfoil { get; set; }
		public List<string> finishes { get; set; }
		public bool oversized { get; set; }
		public bool promo { get; set; }
		public bool reprint { get; set; }
		public bool variation { get; set; }
		//public string set_id { get; set; }
		//public string set { get; set; }
		//public string set_name { get; set; }
		//public string set_type { get; set; }
		//public string set_uri { get; set; }
		//public string set_search_uri { get; set; }
		public string scryfall_set_uri { get; set; }
		public string rulings_uri { get; set; }
		public string prints_search_uri { get; set; }
		public string collector_number { get; set; }
		public bool digital { get; set; }
		public string rarity { get; set; }
		public string card_back_id { get; set; }
		public string artist { get; set; }
		public List<string> artist_ids { get; set; }
		public string illustration_id { get; set; }
		public string border_color { get; set; }
		public string frame { get; set; }
		public bool full_art { get; set; }
		public bool textless { get; set; }
		public bool booster { get; set; }
		public bool story_spotlight { get; set; }
		public int edhrec_rank { get; set; }
		public Prices prices { get; set; }
		public RelatedUris related_uris { get; set; }
	}
	#endregion
}
// Define other methods, classes and namespaces here
public class Forge
{
	private class NamedGroup
	{
		public string GroupName { get; set; }
		public List<string> Lines { get; set; } = new List<string>();
	}
	
	private static List<NamedGroup> ConvertGroups(string[] lines)
	{
		var lineIdx = 0;
		var ret = new List<NamedGroup>();
		
		do{
			var group = new NamedGroup();
			group.GroupName = lines[lineIdx++];
			
			while(lineIdx < lines.Length && string.IsNullOrWhiteSpace(lines[lineIdx])) lineIdx++;
			
			while(lineIdx < lines.Length && lines[lineIdx][0] != '[')
			{
				group.Lines.Add(lines[lineIdx++]);
				
				while(lineIdx < lines.Length && string.IsNullOrWhiteSpace(lines[lineIdx])) lineIdx++;
			}
			
			lineIdx--;
			
			ret.Add(group);
		}while(++lineIdx < lines.Length);
		
		return ret;
	}
	
	private static CardEntry GetCardEntry(string line){
		var ret = new CardEntry();
		
		ret.Name = line.Substring(2).Split("|").DefaultIfEmpty($"CANT PARSE {line}").FirstOrDefault().Trim();
		ret.Count = int.Parse(line.Substring(0,2));
		
		return ret;
	}
	
	private static List<CardEntry> GetCardEntries(List<string> lines){
		return lines.Select(l => GetCardEntry(l)).ToList();
	}
	
	public static CommanderDeck ConvertFile(string filepath)
	{
		if(!File.Exists(filepath)) throw new Exception("Invalid filepath");
		
		var lines = File.ReadAllLines(filepath);
		
		var groups = ConvertGroups(lines);
		
		var deck = new CommanderDeck();
		
		deck.Name = new FileInfo(filepath).Name.Split('.').FirstOrDefault();
		
		deck.Commanders = 
			groups
				.Where(g => g.GroupName == "[Commander]")
				.SelectMany(g => GetCardEntries(g.Lines))
				.ToList();
		
		deck.MainDeck = 
			groups
				.Where(g => g.GroupName == "[Main]")
				.SelectMany(g => GetCardEntries(g.Lines))
				.ToList();
				
		deck.Sideboard = 
			groups
				.Where(g => g.GroupName == "[Sideboard]")
				.SelectMany(g => GetCardEntries(g.Lines))
				.ToList();
		
		return deck;
	}

	public static List<CommanderDeck> ConvertDirectory(string deckRepository)
	{
		var files = Directory.GetFiles(deckRepository, "*.dck");
		var ret = new List<CommanderDeck>();

		foreach (var file in files)
		{
			try
			{
				ret.Add(ConvertFile(file));
			}
			catch (Exception ex)
			{
				$"Error while reading {file}".Dump();
				throw ex;
			}
		}
		return ret;
	}
}

public class Cockatrice
{
	public static void Convert(List<CommanderDeck> decks, string cockatriceDirectory)
	{
		foreach(var deck in decks) Convert(deck, cockatriceDirectory);
	}
	
	public static void Convert(CommanderDeck deck, string cockatriceDirectory)
	{
		var xdoc = new XDocument();
		
		var root = new XElement("cockatrice_deck", new XAttribute("version","1"));
		
		xdoc.Add(root);
		
		var main = new XElement("zone", new XAttribute("name", "main"));
		
		foreach(var card in deck.MainDeck){
			var cardNode = new XElement("card");
			cardNode.Add(new XAttribute("number", card.Count));
			cardNode.Add(new XAttribute("name", card.Name));
			main.Add(cardNode);
		}
		
		root.Add(main);
		
		var side = new XElement("zone", new XAttribute("name", "side"));

		foreach (var card in deck.Commanders)
		{
			var cardNode = new XElement("card");
			cardNode.Add(new XAttribute("number", card.Count));
			cardNode.Add(new XAttribute("name", card.Name));
			side.Add(cardNode);
		}
		
		root.Add(side);

		xdoc.Save(Path.Combine(cockatriceDirectory, deck.Name + ".cod"));
	}
}

public class CommanderDeck
{
	public string Name { get; set; }
	public List<CardEntry> Commanders { get; set; }
	public List<CardEntry> MainDeck { get; set; }
	public List<CardEntry> Sideboard { get; set; }
}

public class CardEntry
{
	public string Name { get; set; }
	public int Count { get; set; }
}

public class DeckMap {
	public Dictionary<string, string> Decks;
	
	public DeckMap() {
		Decks = new Dictionary<string, string>();
	}
}