<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
</Query>

readonly Point FRONT_TOP = new Point(50, 50);
readonly Point FRONT_BOTTOM = new Point(800, 1100);

readonly Point BACK_TOP = new Point(850, 50);
readonly Point BACK_BOTTOM = new Point(1600, 1100);

void Main()
{
	ExtractFolder(@"D:\Media\Jumpstart\Raw", @"D:\Media\Jumpstart\Complete");
}

void ExtractFolder(string rawImages, string outputFolder)
{
	foreach (var file in Directory.GetFiles(rawImages))
	{
		var front = ExtractImage(file, FRONT_TOP, FRONT_BOTTOM);
		
		ExtractImage(file, FRONT_TOP, FRONT_BOTTOM)
			.AddBorder(50, Color.Black)
			.Save(
				Path.Combine(
					outputFolder,
					new FileInfo(file)
						.Name
						.Replace(".png", "") + ".front.png"));

		ExtractImage(file, BACK_TOP, BACK_BOTTOM)
			.AddBorder(50, Color.Black)
			.Save(
				Path.Combine(
					outputFolder,
					new FileInfo(file)
						.Name
						.Replace(".png", "") + ".back.png"));
	}
}

Bitmap ExtractImage(string file, Point top, Point bottom)
{
	using (var bitMap = new System.Drawing.Bitmap(file))
	{

		var front = new Bitmap(bottom.X - top.X, bottom.Y - top.Y);

		for (int y = top.Y; y < bottom.Y; y++)
		{
			for (int x = top.X; x < bottom.X; x++)
			{
				front.SetPixel(
					x - top.X,
					y - top.Y,
					bitMap.GetPixel(x, y));
			}
		}

		return front;
	}
}

void ExtractAndSave(string file, string nameReplace, string newName, Point top, Point bottom)
{
	var front = ExtractImage(file, top, bottom);
	front.Save(file.Replace(nameReplace, newName));
}
// You can define other methods, fields, classes and namespaces here
