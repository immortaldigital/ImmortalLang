using System;
using System.Collections.Generic;

namespace ImmortalLang
{
	public class Statement
	{
		public Node Tree { get; set; }
		private Node current;
		
		public Statement(Token topLevel)
		{
			if(topLevel.Group != TokenCodes.KEYWORD)
			{
				Console.WriteLine("Error: First token " + topLevel.Value + " must be KEYWORD type");
			}
			
			Tree = new Node(topLevel, null);
			current = Tree;
		}
		
		public void PushAuto(Token sub)
		{
			switch(Tree.Details.Value)
			{
				case "return":
					if(sub.Group == TokenCodes.NUMBER)
					{
						Node n = new Node(sub,Tree );
						Tree.Children.Add(n);
					} else {Console.WriteLine("Error: return must be followed by a number BITCH");}
				break;
			}
		}
		
		public void PushCurrent(Token t)
		{
			//Node n = new Node(t, current.Parent);
			current.Details = t.Clone();
		}
		public void PushChild(Token t)
		{
			Node n = new Node(t, current);
			current.Children.Add(n);
		}
		public void Follow() //can be called after PushChild to follow kiddo
		{
			current = current.Children[current.Children.Count - 1];
		}
		public void SetChild(Token t, int index)
		{
			if(index<current.Children.Count)
			{
				current.Children[index].Details = t.Clone();
			} else { Console.WriteLine("Error: YOU'RE TRYING TO CHANGE A KID THAT DOESN'T EXIST"); }
		}
		public void StepDown(int index)
		{
			if(index<current.Children.Count)
			{
				current = current.Children[index];
			} else { Console.WriteLine("Error: Child doesn't exist, maybe they were killed? (index out of bonds)"); }
		}
		public void StepUp()
		{
			if(current.Parent != null)
			{
				current = current.Parent;
			} else { Console.WriteLine("Error: Parent suffered results existential threat (null)"); }
		}
		
		public override string ToString()
		{
			
			return string.Format("[Statement = {0}]", Tree);
		}
	}
	
	public class Node
	{
		public List<Node> Children { get; set; }
		public Node Parent { get; set; }
		public Token Details { get; set; }
		
		public Node()
		{
			Children = new List<Node>();
			Details = null;
		}
		
		public Node(Token t, Node parent)
		{
			Children = new List<Node>();
			Details = t.Clone();
			Parent = parent;
		}
		
		public bool isTerminal()
		{
			return (Children.Count == 0);
		}
		
		public void PrintPretty(string indent, bool last)
		{
			Console.Write(indent);
			if (last)
			{
				Console.Write("\\-");
				indent += "  ";
			} else {
				Console.Write("|-");
				indent += "| ";
			}
			Console.WriteLine(Details.Value);
			
			for (int i = 0; i < Children.Count; i++)
			{
				Children[i].PrintPretty(indent, i == Children.Count - 1);
			}
		}
		
		public override string ToString()
		{
			string builder = "";
			
			builder += String.Format("{0}, ", Details.Value);
			
			foreach(Node n in Children)
			{
				builder += n;
			}
			
			return builder;
		}

	}
}
