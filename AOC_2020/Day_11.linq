<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day11")
		);

	FirstHalf(inputs);

	SecondHalf(inputs);
}

void FirstHalf(string[] lines){
	var grid = new Field(lines);
	
	grid.RunSimulation();
	
	grid.Occupied.Dump("Finalized");
}

void SecondHalf(string[] lines)
{
	var grid = new Field(lines);

	grid.RunSimulation(5, true);

	grid.Occupied.Dump("Finalized");
}

class Field{
	private FieldStatus[,] _field;
	private int HEIGHT, WIDTH;
	
	public int Occupied { 
		get {
			var ret = 0;

			for (int zIdx = 0; zIdx < HEIGHT; zIdx++)
			{
				for (int xIdx = 0; xIdx < WIDTH; xIdx++)
				{
					ret += _field[zIdx, xIdx] == FieldStatus.OCCUPIED ? 1 : 0;
				}
			}

			return ret;
		}
	}
	
	public Field(string[] lines){
		HEIGHT = lines.Length;
		WIDTH = lines[0].Trim().Length;
		_field = new FieldStatus[HEIGHT, WIDTH];
		
		for(int zIdx=0;zIdx<HEIGHT;zIdx++){
			for(int xIdx=0;xIdx<WIDTH;xIdx++)
			{
				_field[zIdx, xIdx] = lines[zIdx][xIdx] == 'L' ? FieldStatus.EMPTY : FieldStatus.NOTASEAT;
			}
		}
	}
	
	public void RunSimulation(int threshHold = 4, bool extendedMode = false)
	{
		var lastRun = Occupied;

		while (true)
		{
			ExecuteRules(threshHold, extendedMode);
			
			if(lastRun == Occupied) return;
			else lastRun = Occupied;
		}
	}
	
	private void ExecuteRules(int threshHold, bool extendedMode)
	{
		var counts = new int[HEIGHT,WIDTH];

		//Check before moving on.
		for (int zIdx = 0; zIdx < HEIGHT; zIdx++)
		{
			for (int xIdx = 0; xIdx < WIDTH; xIdx++)
			{
				counts[zIdx, xIdx] = CountOccupied(zIdx, xIdx, extendedMode);
			}
		}

		for (int zIdx = 0; zIdx < HEIGHT; zIdx++)
		{
			for (int xIdx = 0; xIdx < WIDTH; xIdx++)
			{
				if(_field[zIdx,xIdx] == FieldStatus.OCCUPIED
					&& counts[zIdx,xIdx] >= threshHold)
				{
					_field[zIdx,xIdx] = FieldStatus.EMPTY;
				}
				else if(_field[zIdx,xIdx] == FieldStatus.EMPTY
					&& counts[zIdx, xIdx] == 0)
				{
					_field[zIdx,xIdx] = FieldStatus.OCCUPIED;					
				}
			}
		}
	}

	private int CountOccupied(int zIdx, int xIdx, bool extendedMode)
	{
		var dz = new int[] { -1, -1, -1, 0, 0, 1,1,1 };
		var dx = new int[] { -1,0,1,-1,1,-1,0,1 };
		var ret = 0;
		
		for(var change = 0; change < 8; change++){
			var z = dz[change] + zIdx;
			var x = dx[change] + xIdx;
			
			if(z < 0 || z >= HEIGHT) continue;
			if(x < 0 || x >= WIDTH) continue;

			if (extendedMode)
			{
				while (_field[z, x] == FieldStatus.NOTASEAT)
				{
					z = dz[change] + z;
					x = dx[change] + x;

					if (z < 0 || z >= HEIGHT) break;
					if (x < 0 || x >= WIDTH) break;
				}

				if (z < 0 || z >= HEIGHT) continue;
				if (x < 0 || x >= WIDTH) continue;
			}

			ret += _field[z,x] == FieldStatus.OCCUPIED ? 1 : 0;
		}
		
		return ret;
	}
}

enum FieldStatus{
	NOTASEAT,
	EMPTY,
	OCCUPIED
}