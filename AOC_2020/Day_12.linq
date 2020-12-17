<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day12")
		);

	FirstHalf(inputs);

	SecondHalf(inputs);
}

void FirstHalf(string[] lines)
{
	var s = new Ship();

	foreach(var line in lines){
		s.ProcessDirection(line);	
	}

	new
	{
		Ship = s,
		Result = s.ToString()
	}.Dump("Part 1");
	
}

void SecondHalf(string[] lines)
{
	var s = new Ship();
	
	s.BearingX = 10;
	s.BearingZ = 1;
	"\r\nSecond Half".Dump();
	foreach (var line in lines)
	{
		s.ProcessWaypoint(line);
	}
	
	new
	{
		Ship = s,
		Result = s.ToString()
	}.Dump("Part 2");
}

class Ship {
	public Direction Facing { get; set; }
	public int X { get; set; }
	public int Z { get; set; }
	
	public int BearingX { get; set; }
	public int BearingZ { get; set; }
	
	public int Manhattan { 
		get {
			return Math.Abs(X) + Math.Abs(Z);
		}
	}
	
	public void ProcessDirection(string instr)
	{
		var dir = instr[0];
		var amt = int.Parse(instr.Substring(1));
		
		switch (dir)
		{
			case 'R': TurnRight(amt); return;
			case 'L': TurnLeft(amt); return;
		}
		
		if(dir == 'F'){
			switch(Facing){
				case Direction.NORTH: dir = 'N'; break;
				case Direction.EAST: dir = 'E'; break;
				case Direction.WEST: dir = 'W'; break;
				case Direction.SOUTH: dir = 'S'; break;
			}
		}
		
		switch (dir)
		{
			case 'N': Z += amt; break;
			case 'E': X += amt; break;
			case 'W': X -= amt; break;
			case 'S': Z-= amt; break;
		}
	}

	public void ProcessWaypoint(string instr)
	{
		var dir = instr[0];
		var amt = int.Parse(instr.Substring(1));

		switch (dir)
		{
			case 'R': TurnBearingRight(amt); return;
			case 'L': TurnBearingLeft(amt); return;
		}

		if (dir == 'F')
		{
			for(var f=0;f<amt;f++){
				X += BearingX;
				Z += BearingZ;
			}
			return;
		}

		switch (dir)
		{
			case 'N': BearingZ += amt; break;
			case 'E': BearingX += amt; break;
			case 'W': BearingX -= amt; break;
			case 'S': BearingZ -= amt; break;
		}
	}

	private void TurnRight(int amt) 
	{ 
		Facing = (Direction)(((int)Facing + amt/90)%4);
	}
	private void TurnLeft(int amt) 
	{ 
		Facing = (Direction)(((int)Facing + 4 - amt/90)%4);
	}

	private void TurnBearingRight(int amt)
	{
		int rotation = amt/90;
		
		for(int r =0;r < rotation;r++){
			var t = BearingX;
			BearingX = BearingZ;
			BearingZ = -1*t;
		}
	}

	private void TurnBearingLeft(int amt)
	{
		int rotation = amt / 90;

		for (int r = 0; r < rotation; r++)
		{
			var t = BearingX;
			BearingX = -1 * BearingZ;
			BearingZ = t;
		}
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		
		sb.Append( X >=0 ? "East" : "West" );
		sb.Append($" {Math.Abs(X)}, ");
		sb.Append( Z >=0 ? "North" : "South");
		sb.Append($" {Math.Abs(Z)}");
		
		return sb.ToString();
	}
}

enum Direction {
	EAST,
	SOUTH,
	WEST,
	NORTH
}
