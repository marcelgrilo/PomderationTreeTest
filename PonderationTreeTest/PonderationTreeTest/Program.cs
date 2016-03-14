using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonderationTreeTest
{
	class Program
	{
		public class Node
		{
			#region tree
			public Node Parent { get; set; }
			private Node _left;
			public Node LeftNode { get { return _left; } set { _left = value; _left.Parent = this; } }
			private Node _right;
			public Node RightNode { get { return _right; } set { _right = value; _right.Parent = this; } }
			#endregion


			#region Ponderation
			private double _p;
			public double P
			{
				get { return _p; }
				set { if (value != _p && value.IsBetween(0, 1)) _p = value; }
			}
			public double OneMinusP { get { return 1 - P; } }
			#endregion


			#region Constructors
			public Node(double p)
			{
				this.P = p;
			}
			#endregion

			public int Depth(Node n)
			{
				if (n == null)
					return 0;
				return 1 + Depth(n.Parent);
			}

			public static Random r = new Random();
			public static Node GeneratePonderationTree(int nLeafs = 2)
			{
				// gera uma fila de nodos.
				var nodes = new Queue<Node>();
				for (int i = 0; i < nLeafs - 1; i++)
				{
					//nodes.Enqueue(new Node((double) i / (nLeafs - 1)));
					//nodes.Enqueue(new Node(.5));
					//nodes.Enqueue(new Node(1));
					nodes.Enqueue(new Node(r.NextDouble()));
				}
				//guarda uma referencia da raiz
				var root = nodes.Dequeue();

				// cria uma fila auxiliar de movimentacao.
				Queue<Node> q = new Queue<Node>();
				q.Enqueue(root);
				//linka a arvore
				while (nodes.Count > 0)
				{
					Node toInsert = q.Dequeue();

					if (toInsert.LeftNode == null)
					{
						if (nodes.Count == 0)
							break;
						toInsert.LeftNode = nodes.Dequeue();
						q.Enqueue(toInsert.LeftNode);
					}
					if (toInsert.RightNode == null)
					{
						if (nodes.Count == 0)
							break;
						toInsert.RightNode = nodes.Dequeue();
						q.Enqueue(toInsert.RightNode);
					}
				}
				return root;
			}

			public string TreeToStr(Node root)
			{
				var sb = new StringBuilder();

				sb.Append("\ntree\n");
				int c = 0;
				int clog = 0;
				// cria uma fila auxiliar de movimentacao.
				Queue<Node> q = new Queue<Node>();
				q.Enqueue(root);
				//linka a arvore
				while (q.Count > 0)
				{
					Node toInsert = q.Dequeue();
					c++;
					if (((int) Math.Log(c, 2)) > clog)
					{
						sb.Append("\n");
						clog = ((int) Math.Log(c, 2));
					}
					sb.Append(string.Format("[{0:0.000}]", toInsert.P));
					if (toInsert.LeftNode != null)
					{
						q.Enqueue(toInsert.LeftNode);
					}
					//if (q.Count == 1)
					//	sb.Append("\n");
					if (toInsert.RightNode != null)
					{
						q.Enqueue(toInsert.RightNode);
					}
					//if (q.Count == 1)
					//	sb.Append("\n");
				}

				return sb.ToString();


			}
			public override string ToString()
			{
				return string.Format("[{0:0.000}]", P);
			}

			public string AllvaluesStr(Node m)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("AllValues\n");
				foreach (var item in AllValues(m))
				{
					sb.AppendFormat("[{0:0.000}]", item);
				}
				return sb.ToString();
			}

			public double[] AllValues(Node root)
			{
				var allValues = new List<double>();
				SearchAndMultiplyAllValues(root, ref allValues);
				return allValues.ToArray();
			}
			private void SearchAndMultiplyAllValues(Node node, ref List<double> values, double value = 1)
			{
				if (node == null)
				{
					values.Add(value);
					return;
				}
				SearchAndMultiplyAllValues(node.LeftNode, ref values, value * node.P);
				SearchAndMultiplyAllValues(node.RightNode, ref values, value * node.OneMinusP);
			}
		}
		/// <summary>
		/// As expected, only the trees that have the number of leafs queals to Math.Pow(2,Level), 
		/// presents balanced proportions for the pnderations (p,1-p) pairs. 
		/// On run, the system will log on console,the binary tree of ponderations, the  the means of each ponderation, 
		/// generated randomly by GeneratePonderationTree
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			Node m;
			for (int i = 3; i < 17; i++)
			{
				m = Node.GeneratePonderationTree(i);
				Console.WriteLine(m.TreeToStr(m));
				Console.WriteLine(m.AllvaluesStr(m));
				double[] d = new double[i];
				for (int j = 0; j < 10000; j++)
				{
					m = Node.GeneratePonderationTree(i);
					var vals = m.AllValues(m);
					for (int k = 0; k < i; k++)
					{
						d[k] += vals[k];
					}
				}
				var sb = new StringBuilder();
				for (int k = 0; k < i; k++)
				{
					sb.AppendFormat("\n{0:0.000}/{1:0.000}={2:0.000}", d[k], 10000, d[k] / 10000);
				}
				Console.WriteLine(sb.ToString());
			}

			Console.ReadLine();
		}




	}
	public static class MathHelper
	{
		public static bool IsBetween(this double value, double min, double max, bool isInclusive = true)
		{
			return isInclusive ? (value >= min && value <= max) : (value > min && value < max);
		}
	}
}
