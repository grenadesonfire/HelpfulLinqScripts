<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2021\Inputs\";
void Main()
{
	var inputFile = Path.Combine(INPUT_FOLDER, "Day4.1.txt");
	
	var inputLines = File.ReadAllLines(inputFile);
	
	var nums = inputLines[0].Split(',').Select(v => int.Parse(v));
	
	var boards = new List<Board>();
	var lineIdx = 2;
	
	for(;lineIdx < inputLines.Length; lineIdx+=6)
	{
		var board = new Board();
		
		board.Init(inputLines.Skip(lineIdx).Take(5));
		
		boards.Add(board);
	}
	
	//Part1(nums, boards);
	Part2(nums, boards);
}

void Part2(IEnumerable<int> nums, List<Board> boards)
{
	var q = new Queue<int>(nums);
	var lastCalled = 0;
	var possibleBoards = new List<Board>();
	possibleBoards.AddRange(boards);

	do
	{
		lastCalled = q.Dequeue();

		foreach (var b in boards)
		{
			if (!b.Won)
			{
				b.Mark(lastCalled);
				if (b.Won && possibleBoards.Count() > 1)
				{
					possibleBoards.Remove(b);
				}
			}
		}
		//boards.Dump();
	} while (q.Count > 0 && boards.Any(b => !b.Won));

	var winning = possibleBoards.FirstOrDefault(b => b.Won);
	(lastCalled * winning.EmptyTotal()).Dump("Part 2");
}

void Part1(IEnumerable<int> nums, List<Board> boards)
{
	var q = new Queue<int>(nums);
	var lastCalled = 0;

	do
	{
		lastCalled = q.Dequeue();

		foreach (var b in boards)
		{
			b.Mark(lastCalled);
		}
	} while (q.Count > 0 && !boards.Any(b => b.Won));

	var winning = boards.FirstOrDefault(b => b.Won);
	(lastCalled * winning.EmptyTotal()).Dump("Part 1");
}

// You can define other methods, fields, classes and namespaces here
class Board
{
	public bool Won { get; set; }
	
	public List<List<int>> Values;
	public List<List<bool>> Selected;
	
	public Board()
	{
		Values = new List<List<int>>();
		Selected = new List<List<bool>>();
		
		for(var lineIdx=0;lineIdx<5;lineIdx++)
		{
			Values.Add(new List<int>());
			Selected.Add(new List<bool>());
		}
	}

	internal void Init(IEnumerable<string> setup)
	{
		var list = setup.ToList();
		
		for(var lineIdx=0;lineIdx<list.Count();lineIdx++){
			var nums = 
				list[lineIdx]
					.Split(' ', StringSplitOptions.RemoveEmptyEntries)
					.Select(l => int.Parse(l));
			
			foreach(var num in nums)
			{
				Values[lineIdx].Add(num);
				Selected[lineIdx].Add(false);
			}
		}
	}

	public void Mark(int mark)
	{
		for(var rowIdx = 0; rowIdx<Values.Count(); rowIdx++)
		{
			for(var colIdx = 0; colIdx<Values[rowIdx].Count(); colIdx++)
			{
				if(Values[rowIdx][colIdx] == mark){
					Selected[rowIdx][colIdx] = true;
					Evaluate();
					return;
				}
			}
		}
	}
	
	public void Evaluate()
	{
		//Horizontal
		foreach(var line in Selected)
		{
			var lineSolid = true;
			
			foreach(var item in line){
				lineSolid &= item;
			}
			
			if(lineSolid){
				this.Won = true;
				return;
			}
		}
		
		//Vertical
		for(var colIdx=0; colIdx<Selected.FirstOrDefault().Count(); colIdx++)
		{
			var lineSolid = true;
			
			foreach(var line in Selected)
			{
				lineSolid &= line[colIdx];	
			}
			
			if(lineSolid){
				this.Won = true;
				return;
			}
		}

		//Cross-Section
		var cross1 = true;
		var cross2 = true;
		for(var crossIdx=0;crossIdx<Selected.Count();crossIdx++){
			cross1 &= Selected[crossIdx][crossIdx];
			cross2 &= Selected[crossIdx][crossIdx];
		}
		this.Won = cross1 || cross2;
	}

	internal int EmptyTotal()
	{
		var total = 0;
		for(var rowIdx=0;rowIdx<Selected.Count();rowIdx++)
		{
			for(var colIdx=0;colIdx<Selected.Count();colIdx++)
			{
				total += !Selected[rowIdx][colIdx] ? Values[rowIdx][colIdx] : 0;
			}
		}
		
		return total;
	}
}