<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2021\Inputs\";
void Main()
{
	var inputFile = Path.Combine(INPUT_FOLDER, "Day5.1.txt");
	
	var inputLines = File.ReadAllLines(inputFile);

	var grid = new Grid();

	foreach(var inputLine in inputLines)
	{
		grid.Add(inputLine);
	}
	//grid.DumpGrid();
	grid.Points().Dump("Points");
}

class Grid
{
	private Dictionary<int, Dictionary<int, int>> _grid;
	
	public Grid(){
		_grid = new Dictionary<int, Dictionary<int, int>>();
	}
	
	public int Points(){
		return 
			_grid
				.Values
				.SelectMany(v => v.Values)
				.GroupBy(v => v)
				.OrderByDescending(v => v.Key)
				.Where(v => v.Key >=2)
				.Sum(v => v.Count());
	}
	
	public void DumpGrid()
	{
		_grid.OrderBy(kvp => kvp.Key).Dump();
	}

	public void Add(string inputLine)
	{
		var lines = inputLine.Split(" -> ");
		
		var start = lines[0].Split(',').Select(f => int.Parse(f)).ToArray();
		var end = lines[1].Split(',').Select(f => int.Parse(f)).ToArray();
		var line = new Line(start, end);
		
		// Horizontal
		if(line.Start.X == line.End.X)
		{
			for(
				var idx = Math.Min(line.Start.Y, line.End.Y); 
				idx <= Math.Max(line.Start.Y, line.End.Y); 
				idx++)
			{
				AddPoint(line.Start.X, idx);
			}
		}
		else if(line.Start.Y == line.End.Y)
		{
			for (
				var idx = Math.Min(line.Start.X, line.End.X);
				idx <= Math.Max(line.Start.X, line.End.X);
				idx++)
			{
				AddPoint(idx, line.Start.Y);
			}
		}
		//https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		else
		{
			var x0 = line.Start.X;
			var x1 = line.End.X;
			var y0 = line.Start.Y;
			var y1 = line.End.Y;
			
			Beresenham(x0,x1,y0,y1);
		}
	}

	void Beresenham(int x0, int x1, int y0, int y1)
	{
		if(Math.Abs(y1-y0) < Math.Abs(x1-x0)){
			if(x0 > x1)
			{
				plotLineLow(x1, y1, x0, y0);
			}
			else{
				plotLineLow(x0, y0, x1, y1);
			}
		}
		else
		{
			if(y0 > y1)
			{
				plotLineHigh(x1, y1, x0, y0);
			}
			else
			{
				plotLineHigh(x0, y0, x1, y1);
			}
		}
	}

	void plotLineHigh(int x0, int y0, int x1, int y1)
	{
		var dx = x1 - x0;
		var dy = y1 - y0;
		var xi = 1;
		if(dx < 0)
		{
			xi = -1;
			dx = -dx;
		}
		var D = 2*dx-dy;
		var x = x0;
		
		for(var y=y0;y<=y1;y++)
		{
			AddPoint(x,y);
			if(D > 0)
			{
				x+=xi;
				D+=2*(dx-dy);
			}
			else{
				D+=2*dx;
			}
		}
	}

	void plotLineLow(int x0, int y0, int x1, int y1)
	{
		var dx = x1 - x0;
		var dy = y1 - y0;
		var yi = 1;
		if(dy < 0){
			yi = -1;
			dy = -dy;
		}
		var D = 2*dy-dx;
		var y = y0;
		for(var x = x0;x<=x1;x++){
			AddPoint(x,y);
			if(D > 0)
			{
				y+=yi;
				D+= 2*(dy-dx);
			}
			else{
				D+=2*dy;
			}
		}
	}

	public void AddPoint(int x, int y)
	{
		if(!_grid.ContainsKey(y)) _grid.Add(y, new Dictionary<int, int>());
		
		if(!_grid[y].ContainsKey(x)) _grid[y].Add(x, 0);
		
		_grid[y][x]++;
	}
}

class Line
{
	public Line(int[] start, int[] end)
	{
		Start = new Point(start[0], start[1]);
		End = new Point(end[0], end[1]);
	}

	public Point Start { get; set; }
	public Point End { get; set; }
}

class Point
{
	public Point(int v1, int v2)
	{
		X = v1;
		Y = v2;
	}

	public int X { get; set; }
	public int Y { get; set; }
}