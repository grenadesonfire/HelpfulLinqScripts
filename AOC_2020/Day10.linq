<Query Kind="Program" />

const string INPUT_FOLDER = @"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\";
void Main()
{
	var inputs = File.ReadAllLines(
    	Path.Combine(INPUT_FOLDER, "Day10")
		);

	FirstHalf(inputs).Dump();

	SecondHalf(inputs);
}

int FirstHalf(string[] lines){
	var ints = lines.Select(l => int.Parse(l)).OrderBy(l => l).ToList();
	ints.Insert(0,0);
	
	var count1 = ints.Where(i => ints.IndexOf(i) != 0 && i - ints[ints.IndexOf(i)-1] == 1).Count().Dump("Count 1");
	
	var count2 = ints.Where(i => ints.IndexOf(i) != 0 && i - ints[ints.IndexOf(i)-1] == 2).Count().Dump("Count 2");
	
	var count3 = ints.Where(i => ints.IndexOf(i) != 0 && i - ints[ints.IndexOf(i)-1] == 3).Count().Dump("Count 3");
	
	return count1 * (count3+1);
}

void SecondHalf(string[] lines)
{
	var ints = lines.Select(l => int.Parse(l)).OrderBy(l => l).ToList();
	
	var target = ints.Last();
	
	var treeNode = Tree.Arrange(ints);
	
	treeNode.CountPathsBackward().Dump("Paths:");
}

class Tree{
	private TreeNode _root;
	private List<TreeNode> _items;

	public long CountPathsBackward()
	{
		_items.Where(i => i.BackWardJumps.Count() == 0).Count().Dump("Number of Nodes with no parent");

		long ret = 0;

		var endNode = _items.LastOrDefault();

		foreach (var jump in endNode.BackWardJumps)
		{
			ret += CountPathsBackward(jump);
		}

		return ret;
	}
	
	private long CountPathsBackward(TreeNode node){
		long ret = 0;
		
		if(node.CalculatedBackPaths != -1) return node.CalculatedBackPaths;
		
		foreach(var path in node.BackWardJumps){
			ret += CountPathsBackward(path);
		}
		
		node.CalculatedBackPaths = Math.Max(ret, 1);
		
		return node.CalculatedBackPaths;
	}
	
	public void Sort(){
		_root = _items[0];
		
		foreach(var item in _items){
			for(var jump = 1;jump<=3;jump++){
				var jumpItem = _items.FirstOrDefault(fj => fj.Value == item.Value + jump);

				if (jumpItem != null)
				{
					item.ForwardJumps.Add(jumpItem);
					jumpItem.BackWardJumps.Add(item);
				}
			}
		}
	}
	
	public static Tree Arrange(List<int> numbers){
		var t = new Tree();
		
		numbers.Insert(0,0);

		t._items = numbers.Select(n => new TreeNode { Value = n}).ToList();
		
		t.Sort();
		
		return t;
	}
}

class TreeNode{
	public int Value { get; set; }
	
	public List<TreeNode> ForwardJumps { get; set; } = new List<TreeNode>();
	
	public List<TreeNode> BackWardJumps { get; set; } = new List<TreeNode>();
	
	public long CalculatedBackPaths { get; set; } = -1;
	
	public int ValidJumps {
		get {
			return ForwardJumps.Count();
		}
	}
}

