<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day17")
		);
	
	FirstHalf(inputs);

	SecondHalf(inputs);
}

void FirstHalf(string[] lines)
{
	var c3d = new Conway3D(lines);
	
	for(int run=0;run<6;run++) c3d.Simulate();
	
	c3d.SumActive().Dump("Part 1");
}

void SecondHalf(string[] lines)
{
	var c4d = new Conway4D(lines);
	
	for(int run=0;run<6;run++) c4d.Simulate();
	
	c4d.SumActiveWithTime().Dump("Part 2");
}

class Conway4D : Conway3D
{
	private List<Conway3D> _hyperCubespace;
	
	public Conway4D(string[] lines){
		_hyperCubespace = new List<Conway3D>{
			new Conway3D(lines)
		};
	}

	public override void Simulate()
	{
		Expand();

		CalculateFlippers();

		//DebugTimeAndSpace(true);

		Flip();
		
		//DebugTimeAndSpace();
	}

	public override void CalculateFlippers()
	{
		foreach(var (cube, cubeIdx) in _hyperCubespace.WithIndex())
		{
			foreach (var (layer, layerIdx) in cube._layers.WithIndex())
			{
				foreach (var (line, lineIdx) in layer.WithIndex())
				{
					for (var idx = 0; idx < line.Count(); idx++)
					{
						// Check if should update counts
						if ((line[idx] & 1) == 1)
						{
							UpdateCounts(cubeIdx, layerIdx, lineIdx, idx, 1);
						}
					}
				}
			}
		}
	}

	public override void Flip()
	{
		foreach (var (cube, cubeIdx) in _hyperCubespace.WithIndex())
		{
			foreach (var (layer, layerIdx) in cube._layers.WithIndex())
			{
				foreach (var (line, lineIdx) in layer.WithIndex())
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
							else
							{
								line[idx] = 0;
							}
						}
						// Inactive
						else
						{
							if ((line[idx] >> 2) == 3)
							{
								line[idx] = 1;
							}
							else
							{
								line[idx] = 0;
							}
						}
					}
				}
			}
		}
	}

	private void UpdateCounts(int t, int z, int y, int x, int amtIncrease)
	{
		var dx = new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1 };
		var dy = new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1, -1, -1, -1, 0, 0, 0, 1, 1, 1, -1, -1, -1, 0, 0, 0, 1, 1, 1, -1, -1, -1, 0, 0, 0, 1, 1, 1, -1, -1, -1, 0, 0, 1, 1, 1, -1, -1, -1, 0, 0, 0, 1, 1, 1, -1, -1, -1, 0, 0, 0, 1, 1, 1, -1, -1, -1, 0, 0, 0, 1, 1, 1, -1, -1, -1, 0, 0, 0, 1, 1, 1 };
		var dz = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
		var dt = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

		// check the surrounding cube and time
		// 1 less than max because we don't check the center
		for (int idx = 0; idx < 80; idx++)
		{
			if (
				ValidT(t + dt[idx])
				&& _hyperCubespace[0].ValidZ(z + dz[idx])
				&& _hyperCubespace[0].ValidY(y + dy[idx])
				&& _hyperCubespace[0].ValidX(x + dx[idx]))
			{
				IncrementWithT(
					t + dt[idx],
					z + dz[idx],
					y + dy[idx],
					x + dx[idx]);
			}
		}
	}

	private void IncrementWithT(int t, int z, int y, int x)
	{
		//Add one to the upper n-2 bits, the bottom two are used for other info
		var inc = (_hyperCubespace[t]._layers[z][y][x] >> 2) + 1;

		//Apply the increment back to the numer, &3 retains the bottom bits of information.
		_hyperCubespace[t]._layers[z][y][x] = (short)((inc << 2) | (_hyperCubespace[t]._layers[z][y][x] & 3));
	}

	public override void Expand()
	{
		foreach(var c3d in _hyperCubespace){
			c3d.Expand();
		}
		
		_hyperCubespace.Insert(0, _hyperCubespace[0].CloneEmpty());
		_hyperCubespace.Insert(_hyperCubespace.Count(), _hyperCubespace[0].CloneEmpty());
	}

	public long SumActiveWithTime()
	{
		return _hyperCubespace.Sum(l => l._layers.Sum(l => l.Sum(l2 => l2.Sum(l => (long)l))));
	}

	public bool ValidT(int num) => num >= 0 && num < _hyperCubespace.Count();
	
	private void DebugTimeAndSpace(bool counts = false){
		foreach (var (cube, idx) in _hyperCubespace.WithIndex())
		{
			$"Cube: {idx}".Dump();
			cube.DebugLayers(counts);
		}
	}
}

class Conway3D
{
	public List<List<List<short>>> _layers;
	
	public Conway3D(string[] lines)
	{
		_layers = new List<List<List<short>>>();
		
		var initLayer = new List<List<short>>();
		
		foreach(var line in lines){
			initLayer.Add(line.Select(l => l == '#' ? (short)1 : (short)0).ToList());
		}
		
		_layers.Add(initLayer);
	}

	public Conway3D()
	{
		_layers = new List<System.Collections.Generic.List<System.Collections.Generic.List<short>>>();
	}

	public virtual void Simulate() 
	{		
		Expand();
		
		CalculateFlippers();
		
		Flip();
	}
	
	public virtual void Expand()
	{
		//Expand
		var width = _layers[0][0].Count();
		var height = _layers[0].Count();


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
		
		var topLayer = new List<List<short>>();
		var bottomLayer = new List<List<short>>();
		
		foreach(var line in _layers[0]){
			topLayer.Insert(0, Enumerable.Repeat((short)0, width+2).ToList());
			bottomLayer.Insert(0, Enumerable.Repeat((short)0, width+2).ToList());
		}
		
		_layers.Insert(0, topLayer);
		_layers.Insert(_layers.Count(), bottomLayer);
	}
	
	public virtual void CalculateFlippers()
	{
		foreach(var (layer, layerIdx) in _layers.WithIndex())
		{
			foreach(var (line, lineIdx) in layer.WithIndex()){
				for(var idx=0;idx<line.Count();idx++)
				{
					// Check if should update counts
					if((line[idx] & 1) == 1){
						UpdateCounts(layerIdx, lineIdx, idx, 1);
					}
				}
			}
		}
	}
	
	public virtual void Flip()
	{
		foreach (var (layer, layerIdx) in _layers.WithIndex())
		{
			foreach (var (line, lineIdx) in layer.WithIndex())
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
					// Inactive
					else
					{
						if((line[idx] >> 2)	== 3){
							line[idx] = 1;
						}
						else
						{
							line[idx] = 0;
						}
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

	public void Increment(int z, int y, int x)
	{
		//Add one to the upper n-2 bits, the bottom two are used for other info
		var inc = (_layers[z][y][x] >> 2) + 1;
		
		//Apply the increment back to the numer, &3 retains the bottom bits of information.
		_layers[z][y][x] = (short)((inc << 2) | (_layers[z][y][x] & 3));
	}
	
	public bool ValidZ(int num) => num >= 0 && num < _layers.Count();
	public bool ValidY(int num) => num >= 0 && num < _layers[0].Count();
	public bool ValidX(int num) => num >= 0 && num < _layers[0][0].Count();

	public void DebugLayers(bool outputCounts = false)
	{
		new {
			Layers = _layers.Select(l => LayerToString(l, outputCounts)).ToList()
		}.Dump("State");
	}

	private string LayerToString(List<List<short>> layer, bool outputCounts)
	{
		var sb = new StringBuilder();
		
		foreach(var line in layer){
			if(outputCounts) sb.AppendLine(string.Join(' ', line.Select(l => l >> 2)));
			else sb.AppendLine(string.Join("", line.Select(l => (l & 1) == 1 ? '#' : '.')));
		}
		
		return sb.ToString();
	}

	public long SumActive()
	{
		return _layers.Sum(l => l.Sum(l2 => l2.Sum(l => (long)l)));
	}
	
	public Conway3D CloneEmpty(){
		var ret = new Conway3D();
		
		foreach(var layer in _layers){
			var copylayer = new List<List<short>>();
			foreach(var line in layer){
				copylayer.Add(Enumerable.Repeat((short)0, line.Count()).ToList());
			}
			ret._layers.Add(copylayer);
		}
		
		return ret;
	}
}