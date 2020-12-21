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
	private List<List<List<bool>>> _layers;
	
	public Conway3D(string[] lines)
	{
		_layers = new List<List<List<bool>>>();
		
		var initLayer = new List<List<bool>>();
		
		foreach(var line in lines){
			initLayer.Add(line.Select(l => l == '#').ToList());
		}
		
		_layers.Add(initLayer);
	}
	
	public void Simulate() 
	{
		Expand();
		
		DebugLayers();
	}
	
	private void Expand()
	{
		//Expand
		var width = _layers[0][0].Count();
		var height = _layers[0].Count();

		foreach (var layer in _layers)
		{
			foreach (var line in layer)
			{
				line.Insert(0, false);
				line.Insert(line.Count(), false);
			}

			layer.Insert(0, Enumerable.Repeat(false, width+2).ToList());
			layer.Insert(layer.Count(), Enumerable.Repeat(false, width+2).ToList());
		}
		
		var topLayer = new List<List<bool>>();
		var bottomLayer = new List<List<bool>>();
		
		foreach(var line in _layers[0]){
			topLayer.Insert(0, Enumerable.Repeat(false, width+2).ToList());
			bottomLayer.Insert(0, Enumerable.Repeat(false, width+2).ToList());
		}
		
		_layers.Insert(0, topLayer);
		_layers.Insert(_layers.Count(), bottomLayer);
	}

	private void DebugLayers()
	{
		new {
			Layers = _layers.Select(l => LayerToString(l)).ToList()
		}.Dump("State");
	}

	private string LayerToString(List<List<bool>> layer)
	{
		var sb = new StringBuilder();
		
		foreach(var line in layer){
			sb.AppendLine(string.Join("", line.Select(l => l ? '#' : '.')));
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