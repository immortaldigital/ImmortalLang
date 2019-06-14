using System;
using System.Collections.Generic;

namespace ImmortalLang
{
	public class Statement
	{
		public Node Tree { get; set; }
		public Node Current;
		
		public Statement(Token topLevel)
		{
			if(topLevel.Group != TokenCodes.KEYWORD)
			{
				Console.WriteLine("Error: First token " + topLevel.Value + " must be KEYWORD type");
			}
			
			Tree = new Node(topLevel, null);
			Current = Tree;
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
		
		public void InsertParent(Token t)
		{
			Node temp = Current.Parent;
			Node n = new Node(t, temp);
			
			temp.Children.Add(n);
			temp.Children.Remove(Current);
			n.Children.Add(Current);
			Current.Parent = n;
		}
		
		public void PushCurrent(Token t)
		{
			//Node n = new Node(t, Current.Parent);
			Current.Details = t.Clone();
		}
		public void PushChild(Token t)
		{
			Node n = new Node(t, Current);
			Current.Children.Add(n);
		}
		public void Follow() //can be called after PushChild to follow kiddo
		{
			Current = Current.Children[Current.Children.Count - 1];
		}
		public void SetChild(Token t, int index)
		{
			if(index<Current.Children.Count)
			{
				Current.Children[index].Details = t.Clone();
			} else { Console.WriteLine("Error: YOU'RE TRYING TO CHANGE A KID THAT DOESN'T EXIST"); }
		}
		public void StepDown(int index)
		{
			if(index<Current.Children.Count)
			{
				Current = Current.Children[index];
			} else { Console.WriteLine("Error: Child doesn't exist, maybe they were killed? (index out of bonds)"); }
		}
		public void StepUp()
		{
			if(Current.Parent != null)
			{
				Current = Current.Parent;
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
		
		public void RotateRight()
		{
			Token temp = Details.Clone();
			if(Children.Count > 0)
			{
				Details = Children[0].Details.Clone();
				Children[0].Details = temp; // in case of only 1 child node
			}
			if(Children.Count > 1)
			{
				for(int i=0; i<Children.Count - 1; i++)
				{
					Children[i].Details = Children[i + 1].Details;
				}
				Children[Children.Count - 1].Details = temp;
			}
		}
		public void RotateLeft() //honest we don't need to use .Clone() here but will incase we extend Token later to use references
		{
			Token temp = Details.Clone();
			if(Children.Count > 0)
			{
				Details = Children[Children.Count - 1].Details.Clone();
				Children[Children.Count - 1].Details = temp; // in case of only 1 child node
			}
			if(Children.Count > 1)
			{
				for(int i = Children.Count - 1; i > 0; i--)
				{
					Children[i].Details = Children[i - 1].Details;
				}
				Children[0].Details = temp;
			}
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
