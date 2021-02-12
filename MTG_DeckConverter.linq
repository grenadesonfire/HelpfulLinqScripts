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
	
	var decks = Forge.ConvertDirectory(deckRepository);
	
	//RunStats(decks);
	
	var deckStats = new Deckstats(user, pass);
	
	deckStats.UploadDecks(decks);
	
	//Cockatrice.Convert(decks, @"C:\Users\Nick\AppData\Local\Cockatrice\Cockatrice\decks");
}

IEnumerable<CommanderDeck> DecksThatRun(List<CommanderDeck> decks, string card)
{
	return decks.Where(d => d.Commanders.Any(c => c.Name.Contains(card)) || d.MainDeck.Any(c => c.Name.Contains(card)));
}

void RunStats(List<CommanderDeck> decks)
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

			foreach (var deck in decks.Take(2))
			{
				try
				{
					UploadDeckNoLogin(deck);
				}catch(Exception ex)
				{
					ex.Dump();
				}
			}
		}
		catch(Exception ex)
		{
			ex.Dump("Line 100");
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
