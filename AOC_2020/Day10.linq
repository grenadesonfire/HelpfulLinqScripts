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
	
	var count3 = ints.Where(i => ints.IndexOf(i) != 0 && i - ints[ints.IndexOf(i)-1] == 3).Count();
	
	return count1 * (count3+1);
}

void SecondHalf(string[] lines)
{
	var ints = lines.Select(l => int.Parse(l)).OrderBy(l => l).ToList();
	
	var target = ints.Last();
	
	var treeNode = Tree.Arrange(ints);
	
	treeNode.CountPaths().Dump("Paths:");
}

class Tree{
	private TreeNode _root;
	private List<TreeNode> _items;
	
	public long CountPaths()
	{
		var path = new List<int>() { _root.Value };
		return CountPaths(_root.Jump1) 
			+  CountPaths(_root.Jump2) 
			+ CountPaths(_root.Jump3);
	}
	
	private long CountPaths(TreeNode node){
		if(node==null) return 0;

		if (node.Jump1 == null && node.Jump2 == null && node.Jump3 == null){
			return node.Value == _items.Max(n => n.Value) ? 1 : 0;
		}
		
		return 
			CountPaths(node.Jump1) 
			+ CountPaths(node.Jump2)
			+ CountPaths(node.Jump3);
	}
	
	public void Sort(){
		//Assumes pre sorted list.
		_root = _items[0];
		
		foreach(var item in _items){
			item.Jump1 = _items.FirstOrDefault(n => n.Value == item.Value + 1);
			item.Jump2 = _items.FirstOrDefault(n => n.Value == item.Value + 2);
			item.Jump3 = _items.FirstOrDefault(n => n.Value == item.Value + 3);
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
	public TreeNode Jump1 { get; set; }
	public TreeNode Jump2 { get; set; }
	public TreeNode Jump3 { get; set; }
}

