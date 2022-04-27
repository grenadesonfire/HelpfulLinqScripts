<Query Kind="Program" />

void Main()
{
	Main2(TestCase1());
}

string[] TestCase0()
{
	return new List<string>() {
		"3",
		"4 2",
		"1 2",
		"1 3",
		"1",
		"3 1",
		"2 3",
		"2",
		"5 5",
		"1 2",
		"2 3",
		"3 4",
		"4 5",
		"1 5",
		"1"
	}.ToArray();
}

string[] TestCase1()
{
	return new List<string>(){
		"1",
		"7 4",
		"1 2",
		"1 3",
		"3 4",
		"2 5",
		"2"
	}.ToArray();
}

static void Main(String[] args)
{
	var queries = int.Parse(Console.ReadLine());

	for (var q = 0; q < queries; q++)
	{
		var g = new Graph();
		var graphLine = Console.ReadLine().Split(' ');

		g.NodeCount = int.Parse(graphLine[0]);
		var edgeDescriptions = int.Parse(graphLine[1]);

		// Loop through and make a row per node to hold the edges.
		for (var start = 1; start <= g.NodeCount; start++)
		{
			g.Edges.Add(start, new Dictionary<int, int>());
		}

		// Loop through the edge descriptions and add them to the graph
		for (var edge = 0; edge < edgeDescriptions; edge++)
		{
			var edSplit = Console.ReadLine().Split(' ');
			var start = int.Parse(edSplit[0]);
			var end = int.Parse(edSplit[1]);

			if (!g.Edges[start].Keys.Contains(end))
			{
				g.Edges[start].Add(end, Graph.DEFAULT_LENGTH);
				g.Edges[end].Add(start, Graph.DEFAULT_LENGTH);
			}
		}

		g.StartNode = int.Parse(Console.ReadLine());

		g.RunBack();

		PrintDistanceFromStart(g);
	}
}

static void PrintDistanceFromStart(Graph graph)
{
	var sb = new StringBuilder();
	for (var idx = 1; idx <= graph.NodeCount; idx++)
	{
		if (idx != graph.StartNode)
		{
			if (graph.Edges[graph.StartNode].Keys.Contains(idx)) sb.Append(graph.Edges[graph.StartNode][idx]);
			else sb.Append("-1");

			sb.Append(' ');
		}
	}
	Console.WriteLine(sb.ToString().Trim());
}

public class Graph
{
	public static int DEFAULT_LENGTH = 6;

	public int StartNode { get; set; }
	public int NodeCount { get; set; }

	//Could use a 2d graph but ehhhh, me lazy.
	//    This can* save space, fully loaded would be more but if there are less
	//    edges / its sparsely used then this will be smaller.
	public Dictionary<int, Dictionary<int, int>> Edges { get; set; }

	public Graph()
	{
		Edges = new Dictionary<int, Dictionary<int, int>>();
	}

	public void RunBack()
	{
		var travelQ = new Queue<Tuple<int, int>>();
		var visited = new List<int>();

		travelQ.Enqueue(new Tuple<int, int>(StartNode, 1));

		while (travelQ.Count() != 0)
		{
			var node = travelQ.Dequeue();
			if (!visited.Contains(node.Item1))
			{
				foreach (var edge in Edges[node.Item1])
				{
					travelQ.Enqueue(new Tuple<int, int>(edge.Key, node.Item2 + 1));
					if (!Edges[StartNode].Keys.Contains(edge.Key))
					{
						Edges[StartNode].Add(edge.Key, node.Item2 * 6);
					}
				}

				visited.Add(node.Item1);
			}
		}
	}
}//2 number of lines describing graph chunks
 //4 2 nodes/edges
 //1 2 edge description
 //1 3 edge description
//1 start node
//3 1 nodes/edges
//2 3 edge description
//2 start node
