<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day17.Example")
		);
	
	FirstHalf(inputs);

	SecondHalf(inputs);
}

void FirstHalf(string[] lines)
{
	var c3d = new Conway3D(lines);
	
	c3d.TotalVolume().Dump("Counts");
	
	c3d.Simulate();
}

void SecondHalf(string[] lines)
{
	
}

class Conway3D
{
	private List<List<List<short>>> _layers;
	
	public Conway3D(string[] lines)
	{
		_layers = new List<List<List<short>>>();
		
		var initLayer = new List<List<short>>();
		
		foreach(var line in lines){
			initLayer.Add(line.Select(l => l == '#' ? (short)1 : (short)0).ToList());
		}
		
		_layers.Add(initLayer);
	}
	
	public void Simulate() 
	{
		DebugLayers();
		
		Expand();
		
		DebugLayers();
		
		CalculateFlippers();
		
		//Flip();
		
		DebugLayers();
	}
	
	private void Expand()
	{
		//Expand
		var width = _layers[0][0].Count();
		var height = _layers[0].Count();

		if (_layers.Count() == width)
		{
			foreach (var layer in _layers)
			{
				foreach (var line in layer)
				{
					line.Insert(0, 0);
					line.Insert(line.Count(), 0);
				}

				layer.Insert(0, Enumerable.Repeat((short)0, width + 2).ToList());
				layer.Insert(layer.Count(), Enumerable.Repeat((short)0, width + 2).ToList());
			}
		}
		else{
			width-=2;
			height-=2;
		}
		
		var topLayer = new List<List<short>>();
		var bottomLayer = new List<List<short>>();
		
		foreach(var line in _layers[0]){
			topLayer.Insert(0, Enumerable.Repeat((short)0, width+2).ToList());
			bottomLayer.Insert(0, Enumerable.Repeat((short)0, width+2).ToList());
		}
		
		_layers.Insert(0, topLayer);
		_layers.Insert(_layers.Count(), bottomLayer);
	}
	
	private void CalculateFlippers()
	{
		foreach(var (layer, layerIdx) in _layers.WithIndex())
		{
			foreach(var (line, lineIdx) in layer.WithIndex()){
				for(var idx=0;idx<line.Count();idx++)
				{
					// Check if should update counts
					if((line[idx] & 1) == 1){
						UpdateCounts(layerIdx, lineIdx-1, idx, 1);
					}
				}
			}
		}
	}
	
	private void Flip()
	{
		foreach (var layer in _layers)
		{
			foreach (var line in layer)
			{
				for (var idx = 0; idx < line.Count(); idx++)
				{
					// Check if should update counts
					if ((line[idx] & 1) == 1)
					{
						if ((line[idx] >> 2) == 2 || (line[idx] >> 2) == 3)
						{
							line[idx] = 1;
						}
						else{
							line[idx] = 0;
						}
					}
					else if((line[idx] & 1) == 0){
						if((line[idx] >> 2)	== 3){
							line[idx] = 1;
						}
						else
						{
							line[idx] = 0;
						}
					}
					else{
						"How".Dump("134");
						line[idx] = 0;
					}
				}
			}
		}
	}

	private void UpdateCounts(int z, int y, int x, int amtIncrease)
	{
		var dx = new int[] {-1,-1,-1,-1,-1,-1,-1,-1,-1, 0, 0, 0,0,0,0,0,0,1,1,1,1,1,1,1,1,1 };
		var dy = new int[] {-1,-1,-1, 0, 0, 0, 1, 1, 1,-1,-1,-1,0,0,1,1,1,-1,-1,-1,0,0,0,1,1,1 };
		var dz = new int[] {-1, 0, 1,-1, 0, 1,-1, 0, 1,-1, 0,1,-1,1,-1,0,1,-1,0,1,-1,0,1,-1,0,1 };

		// check the surrounding cube
		// 1 less than max because we don't check the center
		for (int idx = 0; idx < 26; idx++)
		{
			if (ValidZ(z + dz[idx]) && ValidY(y + dy[idx]) && ValidX(x + dx[idx]))
			{
				Increment(
					z + dz[idx],
					y + dy[idx],
					x + dx[idx]);
			}
		}
	}

	private void Increment(int z, int y, int x)
	{
		//Add one to the upper n-2 bits, the bottom two are used for other info
		var inc = (_layers[z][y][x] >> 2) + 1;
		
		//Apply the increment back to the numer, &3 retains the bottom bits of information.
		_layers[z][y][x] = (short)((inc << 2) | (_layers[z][y][x] & 3));
	}
	
	private bool ValidZ(int num) => num >= 0 && num < _layers.Count();
	private bool ValidY(int num) => num >= 0 && num < _layers[0].Count();
	private bool ValidX(int num) => num >= 0 && num < _layers[0][0].Count();

	private bool IsGonnaFlip(short num)
	{
		return (num & (1 << 1)) >> 1 == 1;
	}

	private void SetWillFlip(ref short num)
	{
		num |= (1 << 1);
	}

	private void DebugLayers()
	{
		new {
			Layers = _layers.Select(l => LayerToString(l)).ToList()
		}.Dump("State");
	}

	private string LayerToString(List<List<short>> layer)
	{
		var sb = new StringBuilder();
		
		foreach(var line in layer){
			sb.AppendLine(string.Join("", line.Select(l => (l & 1) == 1 ? '#' : '.')));
		}
		
		return sb.ToString();
	}

	public long TotalVolume(int runs = 6)
	{
		return TotalVolume(
			_layers[0][0].Count(),
			_layers[0].Count(),
			runs);
	}
	
	public long TotalVolume(int width, int height, int runs)
	{
		return (width + runs*2) * (height + runs*2) * runs;
	}
}