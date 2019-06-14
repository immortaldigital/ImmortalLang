using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImmortalLang
{
	public class Parser
	{
		private List<Token> tokens;
		private List<Statement> statements;
		
		int currentToken = 0;
		
		public Parser(List<Token> tokens)
		{
			this.tokens = new List<Token>();
			this.statements = new List<Statement>();
			
			foreach(Token t in tokens)
			{
				if(t.Group!=TokenCodes.WHITESPACE)
				{
					this.tokens.Add(t.Clone());
				}
			}
		}
		
		public void Dump()
		{
			Console.WriteLine("Statement count: {0}", statements.Count);
			foreach(Statement s in statements)
			{
				Console.WriteLine(s.ToString());
			}
		}
		
		public List<Statement> getStatements()
		{
			return statements;
		}
		
		private void eat()
		{
			currentToken++;
		}
		
		public void Parse()
		{
			foreach ( Token t in tokens )
            {
            	
            	if(t.Group != TokenCodes.WHITESPACE)
            	{
            		Console.WriteLine("Group: {0}, {2}, Value: |{1}|", t.GroupName, t.Value, t.Group);
            		//Console.Write(t.Value);
            	}
            }
			
			while(currentToken<tokens.Count)
			{
				Console.WriteLine(currentToken);
				ParseStatement();
			}
		}
		
		private void ParseStatement()
		{
			//if|else|for|while|object|var|return|new|int|float|long|double)
			if(tokens[currentToken].Group == TokenCodes.KEYWORD)
			{
				Statement s = new Statement(tokens[currentToken]);
				eat(); //keyword
				ParseExpressionRaw(s);
				
				eat(); //;
				
				statements.Add(s);
			}
		}
		
		private void ParseExpression(Statement s)
		{
			while(tokens[currentToken].Value != ";")
			{
				if(tokens[currentToken].Value == "(")
				{
					s.PushChild(tokens[currentToken]); //temporary placeholder, will be updated to binaryExpression next step
					s.Follow();
					eat();
				}
				if(tokens[currentToken].Group == TokenCodes.NUMBER)
				{
					s.PushChild(tokens[currentToken]); //temporary placeholder, will be updated to binaryExpression next step
					eat();
				}
				if(tokens[currentToken].Value == ")")
				{
					Console.WriteLine("Stepping up... : " + tokens[currentToken].Value);
					s.StepUp(); //back up a step
					eat();
				}
				if(tokens[currentToken].Group == TokenCodes.EXPRESSION)
				{
					Console.WriteLine("SYMBOL: " + tokens[currentToken].Value);
					//s.StepUp();
					s.PushCurrent(tokens[currentToken]); //now update to binaryExpression
					eat();
				}
				//Console.WriteLine();
				//eat(); //parameter
			}
		}
		private void ParseExpressionRaw(Statement s)
		{
			while(tokens[currentToken].Value != ";")
			{
				if(tokens[currentToken].Group == TokenCodes.NUMBER)
				{
					if(s.Current.Details.Group == TokenCodes.KEYWORD) //initial number
					{
						s.PushChild(tokens[currentToken]);
						s.Follow();
						eat();
					} else {
						s.PushChild(tokens[currentToken]);
						s.Follow();
						eat();
					}
				}
				
				if(tokens[currentToken].Group == TokenCodes.EXPRESSION)
				{
					if(s.Current.Parent.Details.Group != TokenCodes.EXPRESSION) //only 1 number in tree
					{
						s.PushChild(tokens[currentToken]); //now update to binaryExpression
						s.Current.RotateLeft();
						eat();
					} else if (Tokeniser.OperatorPrecedence(tokens[currentToken], s.Current.Parent.Details)) {
						s.PushChild(tokens[currentToken]); //right side precedence
						s.Current.RotateLeft();
						eat();
					} else { //left side precedence
						s.StepUp();
						s.InsertParent(tokens[currentToken]);
						s.StepUp();
						//s.PushChild(); //right side precedence
						//s.Current.RotateLeft();
						eat();
					}
					
				}
				//Console.WriteLine();
				//eat(); //parameter
			}
		}
	}
}
