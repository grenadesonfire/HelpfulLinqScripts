<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
</Query>

void Main()
{
	var bmpPath = @"C:\Media\010000001000000000001.png";
	
	var bmp = new Bitmap(bmpPath);
	
	bmp.Save(bmpPath +".test.png", ImageFormat.);
	
	SplitImage('R', bmpPath, bmp);
	SplitImage('G', bmpPath, bmp);
	SplitImage('B', bmpPath, bmp);
}

void SplitImage(char pixel, string path, Bitmap bmp)
{
	var copy = new Bitmap(bmp.Width, bmp.Height);

	for (var zIdx = 0; zIdx < bmp.Height; zIdx++)
	{
		for (var xIdx = 0; xIdx < bmp.Width; xIdx++)
		{
			var orig = bmp.GetPixel(xIdx,zIdx);
			var color = Color.FromArgb(orig.A,0,0,0);
			
			switch (pixel)
			{
				case 'R': color = Color.FromArgb(orig.A,orig.R,0,0); break;
				case 'G': color = Color.FromArgb(orig.A,0,orig.G,0); break;
				case 'B': color = Color.FromArgb(orig.A,0,0,orig.B); break;
			}
			
			copy.SetPixel(xIdx,zIdx,color);
		}
	}

	copy.Save($"{path}.{pixel}.png", ImageFormat.Png);
	new FileInfo($"{path}.{pixel}.png").Length.Dump();
}

public void TestCompression()
{
	var bmpPath = @"C:\Media\010000001000000000001.png";

	var bmpFile = new FileInfo(bmpPath);

	bmpFile.Length.Dump();

	var bmp = new Bitmap(bmpPath);
	var testFilepath = bmpFile.FullName.Replace(".png", ".nick");

	if (File.Exists(testFilepath)) File.Delete(testFilepath);

	var redCount = new PixelCounter(bmp.GetPixel(0, 0).R, 'R');
	//var greenStr = new StringBuilder();
	//var blueStr = new StringBuilder();
	//var alphaStr = new StringBuilder();

	for (var zIdx = 0; zIdx < bmp.Height; zIdx++)
	{
		for (var xIdx = 1; xIdx < bmp.Width; xIdx++)
		{
			var pixel = bmp.GetPixel(xIdx, zIdx);

			redCount.Count(pixel);
		}
	}
	File.WriteAllLines(
		testFilepath,
		new string[]{
			redCount.Val
		});

	new FileInfo(testFilepath).Length.Dump();
}

public class PixelCounter{
	public string Val { get => sb.ToString(); }
	private StringBuilder sb {get; set;}
	private byte CurrentVal { get; set; }
	private long CurrentStreak { get; set; }
	private char SwapVal;
	
	public PixelCounter(byte CurrentVal, char swapVal){
		this.CurrentVal = CurrentVal;
		CurrentStreak = 1;
		sb = new StringBuilder();
		SwapVal = swapVal;
	}
	
	public void Count(System.Drawing.Color c)
	{
		var comp = (byte)0;
		
		switch (SwapVal)
		{
			case 'R': comp = c.R; break;
			case 'G': comp = c.G; break;
			case 'B': comp = c.B; break;
			case 'A': comp = c.A; break;
			default: throw new Exception("not a valid pixel component");
		}
		
		if (comp != CurrentVal)
		{
			sb.Append($"{CurrentStreak:X2} {CurrentVal:X2} ");
			CurrentVal = comp;
			CurrentStreak = 1;
		}
		else
		{
			CurrentStreak++;
		}
	}
}

// You can define other methods, fields, classes and namespaces here
