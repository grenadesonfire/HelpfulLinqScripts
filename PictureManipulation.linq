<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
//	var dir = @"C:\Media\Xpray\Characters\Nergigante Pose";
	var outputDir = @"C:\Media\Creations";
	
	var dirPath = @"C:\Media\Xpray\Characters\Wennie Teacher and Aurenn\Transparent";//new DirectoryInfo(dir).GetDirectories().First(n => n.Name.Contains("Alt")).FullName;
	var basePhoto = @"C:\Media\Xpray\Characters\Wennie Teacher and Aurenn\xpr_wennie_aurenn_teacher_color (Transparent 01).png";//new DirectoryInfo(dir).GetFiles().First(n => n.Name.Contains("High")).FullName;
	
	var dirInfo = new DirectoryInfo(dirPath);
	
	var bitMaps = dirInfo.GetFiles().ToList(); 
	var stopWatch = new Stopwatch();

	stopWatch.Start();
	DiffAll(basePhoto, bitMaps);
	stopWatch.Stop();
	stopWatch.Elapsed.Dump("Created Pieces.");
	
	"Pause to verify pieces".Dump();
	Console.ReadLine();
	
	stopWatch.Reset();
	stopWatch.Start();
	CreateMatrix(Path.Combine(dirPath,"Pieces")).Dump();
	stopWatch.Stop();
	stopWatch.Elapsed.Dump("Created json for differences");

	stopWatch.Reset();
	stopWatch.Start();
	CalculatePermutations(Path.Combine(dirPath,"Pieces", "Pieces.json")).Dump();
	stopWatch.Stop();
	stopWatch.Elapsed.Dump("Calculated perumation ceiling.");

	outputDir = $@"{outputDir}\Alts_{new DirectoryInfo(outputDir).GetDirectories().Count()}";
	Directory.CreateDirectory(outputDir);
	
	stopWatch.Reset();
	stopWatch.Start();
	ExecutePermutations(
		Path.Combine(dirPath,"Pieces", "Pieces.json"), 
		basePhoto, 
		outputDir, 
		new DirectoryInfo(Path.Combine(dirPath,"Pieces"))
			.GetFiles()
			.Where(n => n.Name.Contains(".png"))
			.Select(n => n.FullName)
			.ToArray(), 
		48);
	stopWatch.Stop();
	stopWatch.Elapsed.Dump("Finished generating files");
	
	stopWatch.Reset();
	stopWatch.Start();
	VerifyDistinct(outputDir);
	stopWatch.Stop();
	stopWatch.Elapsed.Dump("Finished distinct verification");

	stopWatch.Reset();
	stopWatch.Start();
	EliminateDupes(outputDir);
	stopWatch.Stop();
	stopWatch.Elapsed.Dump("Finished distinct verification");
}

void CleanLooseBits(Bitmap bitmap, int groupThreshHold)
{
	var bitGroup = new List<List<Point>>();

	var visited = new Dictionary<int, Dictionary<int, bool>>();
	
	for (var z = 0; z < bitmap.Height; z++)
	{
		for (var x = 0; x < bitmap.Width; x++)
		{
			if (bitmap.GetPixel(x, z).A != 0 && !HasBeenVisited(visited, new Point(x,z)))
			{
				var group = GetPixelGroup(bitmap, x, z, visited);

				if (group.Count() < groupThreshHold)
				{
					foreach (var p in group)
					{
						bitmap.SetPixel(p.X, p.Y, Color.Transparent);
					}
				}
			}
		}
	}
}

bool HasBeenVisited(Dictionary<int, Dictionary<int, bool>> visited, Point p)
{
	return visited.Keys.Contains(p.Y) && visited[p.Y].Keys.Contains(p.X) && visited[p.Y][p.X];
}

void Visit(Dictionary<int, Dictionary<int, bool>> visited, Point p)
{
	if(!visited.Keys.Contains(p.Y)) visited.Add(p.Y, new Dictionary<int, bool>());
	if(!visited[p.Y].Keys.Contains(p.X)) visited[p.Y].Add(p.X, true);
}

List<Point> GetPixelGroup(Bitmap bp, int x, int z, Dictionary<int, Dictionary<int, bool>> visited)
{
	var q = new Queue<Point>();
	var points = new List<Point>();
	
	q.Enqueue(new Point(x,z));

	while (q.TryDequeue(out var p))
	{
		if(!HasBeenVisited(visited, p) )
		{
			Visit(visited, p);
			
			points.Add(p);

			var dx = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
			var dz = new int[] { -1, -1, -1, 0, 0, 1, 1, 1 };

			for (var idx = 0; idx < dx.Length; idx++)
			{
				var next = new Point(p.X + dx[idx], p.Y + dz[idx]);

				if (
					!(next.X < 0 || next.Y < 0 || next.X >= bp.Width || next.Y >= bp.Height) && 
					!HasBeenVisited(visited, next) &&
					!q.Any(v => v.X == next.X && v.Y == next.Y) &&
					bp.GetPixel(next.X, next.Y).A != 0)
				{
					q.Enqueue(next);
				}
			}
		}
	}
	
	return points;
}

void Examples()
{
	//CompareMethods(@"C:\Media\Alts\", @"C:\Media\Alts_1_1\");
	//Diff(
	//	@"C:\Media\Alts_3\Creation_07 43 02.jpg",
	//	@"C:\Media\Alts_3_1\Creation_03 09 17.png").Dump();

	//EnumerateCombinations(
	//	basePhoto,
	//	@$"{dirPath}\Pieces",
	//	@"C:\Media\Alts_8");

	//CombineDirect(
	//	basePhoto,
	//	new string[]{
	//		@"C:\Media\Xpray\May 2019 - Royal Tier\Aurenn Pose 2019\Alt\Pieces\Ass\ass.png",
	//		@"C:\Media\Xpray\May 2019 - Royal Tier\Aurenn Pose 2019\Alt\Pieces\Head\eyes_2.png",
	//		@"C:\Media\Xpray\May 2019 - Royal Tier\Aurenn Pose 2019\Alt\Pieces\Cock\xpr_aurennpose2019_color (Alt 11).jpg",
	//		@"C:\Media\Xpray\May 2019 - Royal Tier\Aurenn Pose 2019\Alt\Pieces\Legs\legs.png",
	//		@"C:\Media\Xpray\May 2019 - Royal Tier\Aurenn Pose 2019\Alt\Pieces\z_Arms\arm.png",
	//		
	//	},
	//	@"C:\Media\Alts_6");


	//Combine(
	//	basePhoto, 
	//	@$"{dirPath}\Pieces",
	//	new string[] {
	//		"xpr_aurennpose2019_color (Alt 02).png"
	//		}, @"C:\Media\Alts_6");
	//
	//new Bitmap(@"E:\Data\Alt\Pieces\xpr_renamon_krcosplay_color (Alt 09).jpg").GetPixel(0,0).ToArgb().Dump();
}

int CalculatePermutations(string path)
{
	var maps = System.Text.Json.JsonSerializer.Deserialize<CollisionMap[]>(File.ReadAllText(path)).ToList();
	
	var ret = 0;
	
	foreach(var map in maps){
		ret += CalculatePermutations(map, maps, map.Collisions, new List<string>());
	}
	
	return ret;
}

int CalculatePermutations(CollisionMap map, List<CollisionMap> maps, List<string> cantVisit, List<string> visited)
{
	var ret = 0;
	
	if(map.NoCollisions.Any(c => !cantVisit.Contains(c) && !visited.Contains(c))){
		visited.Add(map.FileName);
		
		foreach(var noCollision in map.NoCollisions.Where(nc => !cantVisit.Contains(nc) && !visited.Contains(nc)))
		{
			var node = maps.FirstOrDefault(m => m.FileName == noCollision);
			var blocked = node.Collisions.ToList();
			blocked.AddRange(cantVisit);
			ret += CalculatePermutations(node, maps, blocked, visited);
		}
		
		visited.Remove(map.FileName);
	}
	
	ret += 1;
	
	return ret;
}

void ExecutePermutations(string path, string basePhoto, string outputDir, string[] files, int threadMax)
{
	try
	{
		var maps = System.Text.Json.JsonSerializer.Deserialize<CollisionMap[]>(File.ReadAllText(path)).ToList();
		var ret = new List<List<string>>();
		
		foreach (var map in maps)
		{
			ExecutePermutations(map, maps, map.Collisions, new List<string>(), basePhoto, outputDir, ret);
		}
		
		var threadSafe = new System.Collections.Concurrent.ConcurrentQueue<List<string>>(ret);
		
		for(var threadCnt = 0; threadCnt < threadMax; threadCnt++){
			new Task(() => {
				while (threadSafe.Count() > 0 && threadSafe.TryDequeue(out var item))
				{
					try
					{
						CombineDirect(
							basePhoto,
							item.ToArray(),
							outputDir,
							GetFileName(item.ToArray(), files));
					}
					catch {
						GetFileName(item.ToArray(), files).Dump("Hmm");
					}
				}
			}).Start();
		}
		
		while(threadSafe.Count() > 0 && threadSafe.TryPeek(out var nothing));
		
		Task.Delay(15 * 1000).GetAwaiter().GetResult();
	}
	catch(Exception ex)
	{
		ex.Dump();
	}
}

string GetFileName(string[] parts, string[] allFiles){
	var sb = new StringBuilder();
	foreach(var file in allFiles){
		sb.Append(parts.Contains(file) ? "1" : "0");
	}
	sb.Append(".png");
	return sb.ToString();
}

void ExecutePermutations(
	CollisionMap map, 
	List<CollisionMap> maps, 
	List<string> cantVisit, 
	List<string> visited, 
	string basePhoto, 
	string outputDir,
	List<List<string>> combinations)
{
	if (map.NoCollisions.Any(c => !cantVisit.Contains(c) && !visited.Contains(c)))
	{
		visited.Add(map.FileName);
		foreach (var noCollision in map.NoCollisions.Where(nc => !cantVisit.Contains(nc) && !visited.Contains(nc)))
		{
			var node = maps.FirstOrDefault(m => m.FileName == noCollision);
			var blocked = node.Collisions.ToList();
			blocked.AddRange(cantVisit);
			ExecutePermutations(node, maps, blocked.Distinct().ToList(), visited, basePhoto, outputDir, combinations);
		}
		visited.Remove(map.FileName);
	}
	
	var parts = new List<string> { map.FileName};
	parts.AddRange(visited);
	
	combinations.Add(parts);
	
	// Do combination
	//CombineDirect(
	//	basePhoto,
	//	parts.ToArray(),
	//	outputDir);
}

void EnumerateCombinations(string basePath, string directoryOfParts, string outputDir)
{	
	if(!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
	
	var partDirectories = Directory.GetDirectories(directoryOfParts);
	
	var combinations = 1;
	
	foreach(var dir in partDirectories)
	{
		combinations *= (Directory.GetFiles(dir).Length.Dump(dir) + 1);
	}
	
	combinations.Dump("Total");
	
	EnumerateRecursive(basePath, null, partDirectories, outputDir).Dump("Total created:");
}

int EnumerateRecursive(string basePhoto, List<string> chosenParts, string[] partDirectories, string outputDir)
{
	if(partDirectories?.Length == 0 && chosenParts?.Count == 0) {
		// Generate nothing, this is the base file
		return 1;
	}
	
	if(partDirectories?.Length == 0)
	{
		//Create the actual file.
		CombineDirect(basePhoto, chosenParts.ToArray(), outputDir);
		return 1;
	}
	
	var files = Directory.GetFiles(partDirectories[0]);
	
	if(chosenParts == null) chosenParts = new List<string>();
	var ret = 0;
	for(int i=0;i<=files.Length;i++)
	{
		if(i<files.Length) chosenParts.Add(files[i]);
		
		ret += EnumerateRecursive(basePhoto, chosenParts, partDirectories.Skip(1).ToArray(), outputDir);
		
		if(i<files.Length) chosenParts.Remove(files[i]);
	}
	
	return ret;
}

void Combine(string basePhoto, string piecesPath, string[] selectedParts, string outDir)
{
	var dir = new DirectoryInfo(piecesPath).GetFiles();
	using (var baseBmp = new Bitmap(basePhoto))
	{
		for (int part = 0; part < selectedParts.Length; part++)
		{
			using (var addon = new Bitmap(dir.FirstOrDefault(d => d.Name == selectedParts[part]).FullName))
			{
				for (int h = 0; h < baseBmp.Height; h++)
				{
					for (int w = 0; w < baseBmp.Width; w++)
					{
						var pixel = addon.GetPixel(w, h);
						if (pixel.A != 0 && pixel.ToArgb() != 0)
						{
							baseBmp.SetPixel(w, h, addon.GetPixel(w, h));
						}
					}
				}
			}
		}
		var now = DateTime.Now;
		baseBmp.Save(Path.Combine(outDir, $"Test_{now.Hour}_{now.Minute}_{now.Second}.jpg"));
	}
}

void Combine(string basePhoto, string piecesPath, int[] selectedParts, string outDir)
{
	var dir = new DirectoryInfo(piecesPath).GetFiles();
	using(var baseBmp = new Bitmap(basePhoto))
	{
		for(int part=0;part<selectedParts.Length;part++)
		{
			using (var addon = new Bitmap(dir[selectedParts[part]].FullName))
			{
				for (int h = 0; h < baseBmp.Height; h++)
				{
					for (int w = 0; w < baseBmp.Width; w++)
					{
						if(addon.GetPixel(w,h).ToArgb() != 0)
						{
							baseBmp.SetPixel(w,h, addon.GetPixel(w,h));
						}
					}
				}
			}
		}
		
		baseBmp.Save(Path.Combine(outDir, "Test.jpg"));
	}
}

void CombineDirect(string basePhoto, string[] selectedParts, string outDir, string fileName = "")
{
	using (var baseBmp = new Bitmap(basePhoto))
	{
		for (int part = 0; part < selectedParts.Length; part++)
		{
			using (var addon = new Bitmap(selectedParts[part]))
			{
				for (int h = 0; h < baseBmp.Height; h++)
				{
					for (int w = 0; w < baseBmp.Width; w++)
					{
						if (addon.GetPixel(w, h).ToArgb() != 0 && addon.GetPixel(w,h).A > 0)
						{
							baseBmp.SetPixel(w, h, addon.GetPixel(w, h));
						}
					}
				}
			}
		}

		baseBmp.Save(
			Path.Combine(
				outDir, 
					string.IsNullOrWhiteSpace(fileName) ? $"Creation_{DateTime.Now:hh mm ss}_{Guid.NewGuid().ToString().Replace("-","")}.png" : fileName));
	}
}

void DiffAll(string basePath, IEnumerable<FileInfo> fpaths, int groupThreshHold = 250)
{
	var dirInfo = new DirectoryInfo(Path.Combine(fpaths.FirstOrDefault().Directory.FullName, "Pieces"));
	
	if(!dirInfo.Exists) dirInfo.Create();
	
	fpaths.ToList()
		.ForEach(f =>
		{
			using(var bmp = Diff(basePath, f.FullName))
			{
				CleanLooseBits(bmp, groupThreshHold);
				bmp.Save(Path.Combine(dirInfo.FullName, f.Name.Replace(".jpg",".png")).Dump());
			}
		});
	
	//CreateMatrix(dirInfo.FullName);
}

List<CollisionMap> CreateMatrix(string dirPath)
{
	var collisions = new Dictionary<string,bool[]>();
	var maps = new List<CollisionMap>();
	
	var dir = new DirectoryInfo(dirPath);
	var files = dir.GetFiles();
	
	for(var fileIdx = 0; fileIdx < files.Count(); fileIdx++)
	{
		var map = new CollisionMap
		{
			FileName = files[fileIdx].FullName
		};
		for(var compareIdx = fileIdx+1; compareIdx < files.Count(); compareIdx++)
		{
			if(IsCollision(files[fileIdx].FullName, files[compareIdx].FullName)){
				map.Collisions.Add(files[compareIdx].FullName);
			}
			else{
				map.NoCollisions.Add(files[compareIdx].FullName);
			}
		}
		
		maps.Add(map);
	}
	
	File.WriteAllText(
		Path.Combine(dirPath, "Pieces.json"),
		System.Text.Json.JsonSerializer.Serialize(
			maps,
			new JsonSerializerOptions
			{
				WriteIndented = true
			}));
	
	return maps;
}

class CollisionMap
{
	public string FileName { get; set; }
	public List<string> Collisions { get; set; } = new List<string>();
	public List<string> NoCollisions { get; set; } = new List<string>();
	public int Total { get => Collisions.Count() + NoCollisions.Count(); }
}

Bitmap Diff(string basePath, string diff)
{
	try
	{
		using (var baseBmp = new Bitmap(basePath))
		{
			using (var diffBmp = new Bitmap(diff))
			{
				var retBmp = new Bitmap(baseBmp.Width, baseBmp.Height);

				for (int h = 0; h < baseBmp.Height; h++)
				{
					for (int w = 0; w < baseBmp.Width; w++)
					{
						if (baseBmp.GetPixel(w, h) != diffBmp.GetPixel(w, h))
						{
							retBmp.SetPixel(w, h, diffBmp.GetPixel(w, h));
						}
						else
						{
							retBmp.SetPixel(w, h, Color.FromArgb(0, 0, 0, 0));
						}
					}
				}

				return retBmp;
			}
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"error: {basePath}, {diff}");
		throw ex;
	}
}

bool IsCollision(string basePath, string diff)
{
	using (var baseBmp = new Bitmap(basePath))
	{
		using (var diffBmp = new Bitmap(diff))
		{
			for (int h = 0; h < baseBmp.Height; h++)
			{
				for (int w = 0; w < baseBmp.Width; w++)
				{
					var orig = baseBmp.GetPixel(w, h);
					var other = diffBmp.GetPixel(w, h);
					if(orig.A ==0 && other.A == 0 || orig == other) continue;
					else if ((orig.A > 0) == (other.A > 0) && !WithinThreshHold(orig,other))
					{
						return true;
					}
				}
			}
			
			return false;
		}
	}
}

bool WithinThreshHold(System.Drawing.Color a, System.Drawing.Color b, int threshHold = 16)
{
	return 
		Math.Abs(a.R - b.R) <= threshHold && 
		Math.Abs(a.G - b.G) <= threshHold && 
		Math.Abs(a.B - b.B) <= threshHold;
}

void Average(string dirPath, IEnumerable<FileInfo> bitMaps)
{

	var height = 3225;
	var width = 2850;
	//	var f = bitMaps.FirstOrDefault();
	//	
	var average = new Bitmap(width, height);

	try
	{
		for (int h = 0; h < height; h++)
		{
			for (int w = 0; w < width; w++)
			{
				average.SetPixel(w, h, GetMostCommonPixel(bitMaps, w, h));
			}
			Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}][{(h * 1.0) / (height * 1.0)}%]");
			//Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}][{(w * 1.0) / (width * 1.0)}%]");
		}

		average.Dump();
	}
	catch { }

	average.Save(Path.Combine(dirPath, "behold.jpg"));

	//var file = Bitmap.FromFile(dirInfo.GetFiles().FirstOrDefault().FullName);
}

Color GetMostCommonPixel(IEnumerable<FileInfo> bitmapFileNames, int x, int y)
{
	return 
		Color.FromArgb(
			bitmapFileNames
				.Select(
					fn => GetPixel(fn.FullName, x, y))
				.GroupBy(c => c.ToArgb())
				.OrderBy(c => c.Count())
				.FirstOrDefault()
				.Key
			);
}

Color GetPixel(string fpath, int x, int y)
{
	using(var bmp = new Bitmap(fpath))
	{
		return bmp.GetPixel(x,y);
	}
}

void VerifyDistinct(string dir1)
{
	using (var md5 = MD5.Create())
	{
		var hashes =
			new DirectoryInfo(dir1)
				.GetFiles()
				.Where(n => n.FullName.Contains(".jpg") || n.FullName.Contains(".png"))
				.Select(n =>
				{
					using (var f1 = n.OpenRead())
					{
						return new
						{
							Filename = n.FullName,
							Hash = BitConverter.ToString(md5.ComputeHash(f1))
						};
					}
				});

		hashes.Count().Dump("Total:");
		hashes.Select(h => h.Hash).Distinct().Count().Dump("Distinct:");

		hashes.GroupBy(h => h.Hash).Where(h => h.Count() > 1).Dump("Duplicates");
	}
}

void EliminateDupes(string dir1)
{
	using (var md5 = MD5.Create())
	{
		var hashes =
			new DirectoryInfo(dir1)
				.GetFiles()
				.Where(n => n.FullName.Contains(".jpg") || n.FullName.Contains(".png"))
				.Select(n =>
				{
					using (var f1 = n.OpenRead())
					{
						return new
						{
							Filename = n.FullName,
							Hash = BitConverter.ToString(md5.ComputeHash(f1))
						};
					}
				});

		hashes
			.GroupBy(h => h.Hash)
			.Where(h => h.Count() > 1)
			.ToList()
			.ForEach(h =>
			{
				h.Skip(1).ToList().ForEach(item => File.Delete(item.Filename));
			});
	}
}

void CompareMethods(string dir1, string dir2)
{
	using (var md5 = MD5.Create())
	{
		var hashes =
			new DirectoryInfo(dir1)
				.GetFiles()
				.Where(n => n.FullName.Contains(".jpg") || n.FullName.Contains(".png"))
				.Select(n =>
				{
					using (var f1 = n.OpenRead())
					{
						return new
						{
							Filename = n.FullName,
							Hash = BitConverter.ToString(md5.ComputeHash(f1))
						};
					}
				});

		var hashes2 =
			new DirectoryInfo(dir2)
				.GetFiles()
				.Where(n => n.FullName.Contains(".jpg") || n.FullName.Contains(".png"))
				.Select(n =>
				{
					using (var f1 = n.OpenRead())
					{
						return new
						{
							Filename = n.FullName,
							Hash = BitConverter.ToString(md5.ComputeHash(f1))
						};
					}
				});

		hashes.Where(h => !hashes2.Any(ha => ha.Hash == h.Hash)).Dump("Original Method:");
		hashes2.Where(h => !hashes.Any(ha => ha.Hash == h.Hash)).Dump("New Method:");
	}
}
// Define other methods and classes here