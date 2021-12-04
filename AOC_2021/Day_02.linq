<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2021\Inputs\";
void Main()
{
	var moves = File.ReadAllLines(Path.Combine(INPUT_FOLDER, "Day2.1.txt")).Select(f => Convert(f));
	
	FirstHalf(moves).Dump("First Half");
	SecondHalf(moves).Dump("Second Half");
}

int SecondHalf(IEnumerable<Move> moves)
{
	var horizontal = 0;
	var depth = 0;
	var aim = 0;

	foreach (var move in moves)
	{
		switch (move.Direction)
		{
			case Direction.forward:
				depth += aim * move.Distance;
				horizontal += move.Distance;
				break;
			case Direction.up:
				aim -= move.Distance;
				break;
			case Direction.down:
				aim += move.Distance;
				break;
			default: throw new Exception("Shouldn't be here!");
		}
	}

	return horizontal * depth;
}

int FirstHalf(IEnumerable<Move> moves)
{
	var horizontal = 0;
	var depth = 0;
	
	foreach(var move in moves)
	{
		switch(move.Direction) {
			case Direction.forward:
				horizontal += move.Distance;
				break;
			case Direction.up:
				depth -= move.Distance;
				break;
			case Direction.down:
				depth += move.Distance;
				break;
			default: throw new Exception("Shouldn't be here!");
		}
	}
	
	return horizontal * depth;
}

Move Convert(string line)
{
	var parts = line.Split(' ');
	var move = new Move();
	switch(parts[0]){
		case "forward": move.Direction = Direction.forward; break;
		case "down": move.Direction = Direction.down; break;
		case "up": move.Direction = Direction.up; break;
		default:
			throw new Exception($"Can't parse [{line}]");
	}
	
	move.Distance = int.Parse(parts[1]);
	
	return move;
}

// You can define other methods, fields, classes and namespaces here
struct Move
{
	public Direction Direction;
	public int Distance;
}

enum Direction
{
	forward,
	down,
	up
}