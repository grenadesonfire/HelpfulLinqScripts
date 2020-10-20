<Query Kind="Program">
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	// Deck configuration
	var cardCount = 99;
	var target = 32;
	var handSize = 7;
	
	// Trial configuration
	var trialRuns = 100000;
	
	// Pile shuffle config
	var pileMaxSize = 50;
	
	// Stack shuffle config
	int stackMinSize = 5, stackMaxSize = 25;
	int stackMinIterations = 3, stackMaxIterations = 15;

	// Running the shuffle tests
	var shuffleRunners = new ConcurrentQueue<StackShuffleTestRunner>();
	var shuffleResults = new ConcurrentQueue<StackShuffleResult>();
	
	for (var shuffles = stackMinIterations; shuffles <= stackMaxIterations; shuffles++)
	{
		for (var stackSize = stackMinSize; stackSize <= stackMaxSize; stackSize++)
		{
			shuffleRunners.Enqueue(new StackShuffleTestRunner(cardCount, target, trialRuns, handSize, shuffles, stackSize));
			new Task(
				() => {
					shuffleRunners.TryDequeue(out var runner);
					shuffleResults.Enqueue(runner.StackShuffleTrial());
				}).Start();
		}
	}

	// Running the pile tests
	var pileRunners = new ConcurrentQueue<PileShuffleTestRunner>();
	var pileResults = new ConcurrentQueue<PileShuffleResult>();
	
	for (var pileSize = 2; pileSize <= pileMaxSize; pileSize++)
	{
		pileRunners.Enqueue(new PileShuffleTestRunner(cardCount, target, trialRuns, handSize, pileSize));
		new Task(
			() =>
			{
				pileRunners.TryDequeue(out var runner);
				pileResults.Enqueue(runner.PileShuffleTrial());
			}).Start();
	}

	// Update until finished.
	do
	{
		Util.ClearResults();

		Console.WriteLine($"Stack Shuffle Trials: {shuffleResults.Count()} / {(stackMaxSize - stackMinSize + 1) * (stackMaxIterations - stackMinIterations + 1)}");
		Console.WriteLine($"Pile Shuffle Trials: {pileResults.Count()} / {pileMaxSize - 1}");

		Task.Delay(1 * 1000).GetAwaiter().GetResult();
	} while (shuffleResults.Count() < (stackMaxSize - stackMinSize + 1) * (stackMaxIterations - stackMinIterations + 1) || pileResults.Count() < (pileMaxSize - 1));

	Util.ClearResults();

	Console.WriteLine($"Stack Shuffle Trials: {shuffleResults.Count()} / {(stackMaxSize - stackMinSize + 1) * (stackMaxIterations - stackMinIterations + 1)}");
	Console.WriteLine($"Pile Shuffle Trials: {pileResults.Count()} / {pileMaxSize - 1}");

	// Total time taken
	Console.WriteLine($"Finished stack testing, Total across stack shuffle threads: {TimeSpan.FromMilliseconds(shuffleResults.Sum(r => r.TotalTime.TotalMilliseconds))}");
	Console.WriteLine($"Finished stack testing, Total across stack shuffle threads: {TimeSpan.FromMilliseconds(pileResults.Sum(r => r.TotalTime.TotalMilliseconds))}");

	// And Finally the results.
	Console.WriteLine("Stack Shuffles");
	shuffleResults.OrderByDescending(r => r.Perecentage).Dump();

	Console.WriteLine("Pile Shuffles");
	pileResults.OrderByDescending(r => r.Perecentage).Dump();
}

static int CountTargets(string deckString, int handSize, char target)
{
	return deckString.Substring(0, handSize).Count(s => s == target);
}

static void Swap(char[] array, int swapIdx){
	var temp = array[swapIdx];
	array[swapIdx] = array[0];
	array[0] = temp;
}

public class TestRunner
{
	protected Random rand = new Random((int)DateTime.Now.Ticks);
	protected string GenerateDeck(int targets, int total)
	{
		return new string('b', total - targets) + new string('a', targets);
	}

	protected string GenerateShuffledDeck(int targets, int total, int randomSwaps = 100)
	{
		var deck = GenerateDeck(targets, total).ToArray();

		for (var swap = 0; swap < randomSwaps; swap++) Swap(deck, rand.Next(1, deck.Length));

		return string.Join("", deck);
	}
}

public class StackShuffleTestRunner : TestRunner
{
	private int cardCount, target, trialRuns, handSize, shuffleTimes, stackSize;
	public StackShuffleTestRunner(int cardCount, int target, int trialRuns, int handSize, int shuffleTimes, int stackSize){
		this.cardCount = cardCount;
		this.target = target;
		this.trialRuns = trialRuns;
		this.handSize = handSize;
		this.shuffleTimes = shuffleTimes;
		this.stackSize = stackSize;
	}
	public StackShuffleResult StackShuffleTrial()
	{
		var stopWatch = new Stopwatch();
		var results = new Dictionary<int, int>();
		stopWatch.Start();
		for (var trial = 0; trial < trialRuns; trial++)
		{
			var deck = GenerateShuffledDeck(target, cardCount);

			deck = StackShuffle(deck, shuffleTimes, stackSize);

			var count = CountTargets(deck, handSize, 'a');

			if (!results.ContainsKey(count)) results.Add(count, 1);
			else results[count]++;
		}
		stopWatch.Stop();
		return new StackShuffleResult
		{
			ShuffleCount = shuffleTimes,
			StackSize = stackSize,
			CountOfPlayableHands = results,
			TotalPlayable = results.Where(r => r.Key >= 3 && r.Key <= 5).Sum(r => r.Value),
			TotalRun = trialRuns,
			TotalTime = stopWatch.Elapsed
		};
	}

	string StackShuffle(string deckString, int iterations, int stackSize)
	{
		for (var shuffle = 0; shuffle < iterations; shuffle++)
		{
			deckString = SingleStackShuffle(deckString, stackSize);
		}

		return deckString;
	}

	string SingleStackShuffle(string deck, int pileSize = 10)
	{
		// Shuffling from the very first card is silly. And not shuffling at all.
		var rIdx = rand.Next(1, deck.Length - pileSize);

		return deck.Substring(rIdx, pileSize) + deck.Substring(0, rIdx) + deck.Substring(rIdx + pileSize);
	}
}

class PileShuffleTestRunner : TestRunner
{
	private int cardCount, target, trialRuns, handSize, numberOfPiles;
	
	public PileShuffleTestRunner(int cardCount, int target, int trialRuns, int handSize, int numberOfPiles){
		this.cardCount = cardCount;
		this.target = target;
		this.trialRuns = trialRuns;
		this.handSize = handSize;
		this.numberOfPiles = numberOfPiles;
	}
	
	public PileShuffleResult PileShuffleTrial()
	{
		var results = new Dictionary<int, int>();
		var stopWatch = new Stopwatch();
		stopWatch.Start();
		for (var trial = 0; trial < trialRuns; trial++)
		{
			var deck = GenerateShuffledDeck(target, cardCount);

			deck = PileShuffle(deck, numberOfPiles);

			var count = CountTargets(deck, handSize, 'a');

			if (!results.ContainsKey(count)) results.Add(count, 1);
			else results[count]++;
		}
		stopWatch.Stop();

		return new PileShuffleResult
		{
			NumberOfPiles = numberOfPiles,
			CountOfPlayableHands = results,
			TotalPlayable = results.Where(r => r.Key >= 3 && r.Key <= 5).Sum(r => r.Value),
			TotalRun = trialRuns,
			TotalTime = stopWatch.Elapsed
		};
	}

	string PileShuffle(string deck, int numberOfPiles)
	{
		var piles = new List<List<char>>(numberOfPiles);

		for (var pileIdx = 0; pileIdx < numberOfPiles; pileIdx++) piles.Add(new List<char>());

		for (var cardIdx = 0; cardIdx < deck.Length; cardIdx++)
		{
			piles[cardIdx % piles.Count()].Add(deck[cardIdx]);
		}

		return string.Join("", piles.Select(p => string.Join("", p)));
	}
}

public class PileShuffleResult : TrialResult {
	public int NumberOfPiles { get; set; }
}

public class StackShuffleResult : TrialResult {
	public int StackSize { get; set; }
	public int ShuffleCount { get; set; }
}

public class TrialResult
{
	public Dictionary<int, int> CountOfPlayableHands { get; set; }
	public int TotalPlayable { get; set; }
	public int TotalRun { get; set; }
	public double Perecentage { get => TotalPlayable * 1.0 / TotalRun * 1.0 * 100.0; }
	public TimeSpan TotalTime { get; set; }
}