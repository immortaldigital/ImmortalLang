using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace ImmortalLang
{
	public static class Tokeniser
    {
		public static List<Token> getTokenArray(string src)
        {
			List<Token> tokens = new List<Token>();
			
			MatchCollection mc = TokenCodes.divider.Matches(src);
			
			foreach (Match symbol in mc)
			{
				Token temp = new Token(symbol);
				tokens.Add(temp);
			}
			
        	return tokens;
        }
    }
	
	public class Token
	{
		private int group;
		private string groupName;
		private string value;
		
		public Token(Match symbol)
		{
			value = symbol.Value;
			
			for(int i = 1; i< symbol.Groups.Count; i++)
			{
				if(symbol.Groups[i].Success)
				{
					group = i - 1;
					groupName += "1";
				} else {
					groupName += "0";
				}
			}
			
			if(group == TokenCodes.LABEL) //check if it's a keyword or label
			{
				if(TokenCodes.keywords.Match(value).Success)
				{
					group = TokenCodes.KEYWORD;
					groupName = "KEYWD";
				} else {
					groupName = "LABEL";
				}
			}
			if(group == TokenCodes.SYMBOL) //check if it's a keyword or label
			{
				if(TokenCodes.binaryExpressions.Match(value).Success)
				{
					group = TokenCodes.EXPRESSION;
					groupName = "XPRSN";
				} else {
					groupName = "NTXPN";
				}
			}
		}
		
		public Token(int group, string groupName, string value)
		{
			this.group = group;
			this.groupName = groupName;
			this.value = value;
		}
		
		public Token Clone()
		{
			Token t = new Token(group, groupName, value);
			//t.Group = group;
			//t.GroupName = groupName;
			//t.Value = value;
			return t;
		}
		
		public int Group
		{
			get { return group; }
			set { group = value; }
		}
		
		public string GroupName
		{
			get { return groupName; }
			set { groupName = value; }
		}
		
		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}
	}
	
	public static class TokenCodes
	{
		public static readonly Regex divider = new Regex(@"(?:([.0-9]+)|(\w+)|(\s+)|(.))");
		public static readonly Regex keywords = new Regex(@"(if|else|for|while|object|var|return|new|int|float|long|double)");
		public static readonly Regex binaryExpressions = new Regex(@"(\+|\-|\/|\*)");
		
		public const int NUMBER = 0;
		public const int LABEL = 1;
		public const int WHITESPACE = 2;
		public const int SYMBOL = 3;
		public const int KEYWORD = 4;
		public const int EXPRESSION = 5;
	}
}
