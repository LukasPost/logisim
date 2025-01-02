// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{

	using StringGetter = logisim.util.StringGetter;

	public class Parser
	{
		private Parser()
		{
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static Expression parse(String in, AnalyzerModel model) throws ParserException
		public static Expression parse(string @in, AnalyzerModel model)
		{
			List<Token> tokens = toTokens(@in, false);

			if (tokens.Count == 0)
			{
				return null;
			}

			foreach (Token token in tokens)
			{
				if (token.type == TOKEN_ERROR)
				{
					throw token.error(Strings.getter("invalidCharacterError", token.text));
				}
				else if (token.type == TOKEN_IDENT)
				{
					int index = model.Inputs.IndexOf(token.text);
					if (index < 0)
					{
						// ok; but maybe this is an operator
						string opText = token.text.ToUpper();
						if (opText.Equals("NOT"))
						{
							token.type = TOKEN_NOT;
						}
						else if (opText.Equals("AND"))
						{
							token.type = TOKEN_AND;
						}
						else if (opText.Equals("XOR"))
						{
							token.type = TOKEN_XOR;
						}
						else if (opText.Equals("OR"))
						{
							token.type = TOKEN_OR;
						}
						else
						{
							throw token.error(Strings.getter("badVariableName", token.text));
						}
					}
				}
			}

			return parse(tokens);
		}

		/// <summary>
		/// I wrote this without thinking, and then realized that this is quite complicated because of removing operators. I
		/// haven't bothered to do it correctly; instead, it just regenerates a string from the raw expression. static String
		/// removeVariable(String in, String variable) { StringBuilder ret = new StringBuilder(); ArrayList tokens =
		/// toTokens(in, true); Token lastWhite = null; for (int i = 0, n = tokens.size(); i < n; i++) { Token token =
		/// (Token) tokens.get(i); if (token.type == TOKEN_IDENT && token.text.equals(variable)) { ; // just ignore it } else
		/// if (token.type == TOKEN_WHITE) { if (lastWhite != null) { if (lastWhite.text.length() >= token.text.length()) { ;
		/// // don't repeat shorter whitespace } else { ret.replace(ret.length() - lastWhite.text.length(), ret.length(),
		/// token.text); lastWhite = token; } } else { lastWhite = token; ret.append(token.text); } } else { lastWhite =
		/// null; ret.append(token.text); } } return ret.toString(); }
		/// </summary>

		internal static string replaceVariable(string @in, string oldName, string newName)
		{
			StringBuilder ret = new StringBuilder();
			List<Token> tokens = toTokens(@in, true);
			foreach (Token token in tokens)
			{
				if (token.type == TOKEN_IDENT && token.text.Equals(oldName))
				{
					ret.Append(newName);
				}
				else
				{
					ret.Append(token.text);
				}
			}
			return ret.ToString();
		}

		//
		// tokenizing code
		//
		private const int TOKEN_AND = 0;
		private const int TOKEN_OR = 1;
		private const int TOKEN_XOR = 2;
		private const int TOKEN_NOT = 3;
		private const int TOKEN_NOT_POSTFIX = 4;
		private const int TOKEN_LPAREN = 5;
		private const int TOKEN_RPAREN = 6;
		private const int TOKEN_IDENT = 7;
		private const int TOKEN_CONST = 8;
		private const int TOKEN_WHITE = 9;
		private const int TOKEN_ERROR = 10;

		private class Token
		{
			internal int type;
			internal int offset;
			internal int length;
			internal string text;

			internal Token(int type, int offset, string text) : this(type, offset, text.Length, text)
			{
			}

			internal Token(int type, int offset, int length, string text)
			{
				this.type = type;
				this.offset = offset;
				this.length = length;
				this.text = text;
			}

			internal virtual ParserException error(StringGetter message)
			{
				return new ParserException(message, offset, length);
			}
		}

		private static List<Token> toTokens(string @in, bool includeWhite)
		{
			List<Token> tokens = new List<Token>();

			// Guarantee that we will stop just after reading whitespace,
			// not in the middle of a token.
			@in = @in + " ";
			int pos = 0;
			while (true)
			{
				int whiteStart = pos;
				while (pos < @in.Length && char.IsWhiteSpace(@in[pos]))
				{
					pos++;
				}
				if (includeWhite && pos != whiteStart)
				{
					tokens.Add(new Token(TOKEN_WHITE, whiteStart, @in.Substring(whiteStart, pos - whiteStart)));
				}
				if (pos == @in.Length)
				{
					return tokens;
				}

				int start = pos;
				char startChar = @in[pos];
				pos++;
				if (Character.isJavaIdentifierStart(startChar))
				{
					while (Character.isJavaIdentifierPart(@in[pos]))
					{
						pos++;
					}
					tokens.Add(new Token(TOKEN_IDENT, start, @in.Substring(start, pos - start)));
				}
				else
				{
					switch (startChar)
					{
					case '(':
						tokens.Add(new Token(TOKEN_LPAREN, start, "("));
						break;
					case ')':
						tokens.Add(new Token(TOKEN_RPAREN, start, ")"));
						break;
					case '0':
					case '1':
						tokens.Add(new Token(TOKEN_CONST, start, "" + startChar));
						break;
					case '~':
						tokens.Add(new Token(TOKEN_NOT, start, "~"));
						break;
					case '\'':
						tokens.Add(new Token(TOKEN_NOT_POSTFIX, start, "'"));
						break;
					case '^':
						tokens.Add(new Token(TOKEN_XOR, start, "^"));
						break;
					case '+':
						tokens.Add(new Token(TOKEN_OR, start, "+"));
						break;
					case '!':
						tokens.Add(new Token(TOKEN_NOT, start, "!"));
						break;
					case '&':
						if (@in[pos] == '&')
						{
							pos++;
						}
						tokens.Add(new Token(TOKEN_AND, start, @in.Substring(start, pos - start)));
						break;
					case '|':
						if (@in[pos] == '|')
						{
							pos++;
						}
						tokens.Add(new Token(TOKEN_OR, start, @in.Substring(start, pos - start)));
						break;
					default:
						while (!okCharacter(@in[pos]))
						{
							pos++;
						}
						string errorText = @in.Substring(start, pos - start);
						tokens.Add(new Token(TOKEN_ERROR, start, errorText));
					break;
					}
				}
			}
		}

		private static bool okCharacter(char c)
		{
			return char.IsWhiteSpace(c) || Character.isJavaIdentifierStart(c) || "()01~^+!&|".IndexOf(c) >= 0;
		}

		//
		// parsing code
		//
		private class Context
		{
			internal int level;
			internal Expression current;
			internal Token cause;

			internal Context(Expression current, int level, Token cause)
			{
				this.level = level;
				this.current = current;
				this.cause = cause;
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static Expression parse(java.util.ArrayList<Token> tokens) throws ParserException
		private static Expression parse(List<Token> tokens)
		{
			List<Context> stack = new List<Context>();
			Expression current = null;
			for (int i = 0; i < tokens.Count; i++)
			{
				Token t = tokens[i];
				if (t.type == TOKEN_IDENT || t.type == TOKEN_CONST)
				{
					Expression here;
					if (t.type == TOKEN_IDENT)
					{
						here = Expressions.variable(t.text);
					}
					else
					{
						here = Expressions.constant(Convert.ToInt32(t.text, 16));
					}
					while (i + 1 < tokens.Count && tokens[i + 1].type == TOKEN_NOT_POSTFIX)
					{
						here = Expressions.not(here);
						i++;
					}
					while (peekLevel(stack) == Expression.NOT_LEVEL)
					{
						here = Expressions.not(here);
						pop(stack);
					}
					current = Expressions.and(current, here);
					if (peekLevel(stack) == Expression.AND_LEVEL)
					{
						Context top = pop(stack);
						current = Expressions.and(top.current, current);
					}
				}
				else if (t.type == TOKEN_NOT)
				{
					if (current != null)
					{
						push(stack, current, Expression.AND_LEVEL, new Token(TOKEN_AND, t.offset, Strings.get("implicitAndOperator")));
					}
					push(stack, null, Expression.NOT_LEVEL, t);
					current = null;
				}
				else if (t.type == TOKEN_NOT_POSTFIX)
				{
					throw t.error(Strings.getter("unexpectedApostrophe"));
				}
				else if (t.type == TOKEN_LPAREN)
				{
					if (current != null)
					{
						push(stack, current, Expression.AND_LEVEL, new Token(TOKEN_AND, t.offset, 0, Strings.get("implicitAndOperator")));
					}
					push(stack, null, -2, t);
					current = null;
				}
				else if (t.type == TOKEN_RPAREN)
				{
					current = popTo(stack, -1, current);
					// there had better be a LPAREN atop the stack now.
					if (stack.Count == 0)
					{
						throw t.error(Strings.getter("lparenMissingError"));
					}
					pop(stack);
					while (i + 1 < tokens.Count && tokens[i + 1].type == TOKEN_NOT_POSTFIX)
					{
						current = Expressions.not(current);
						i++;
					}
					current = popTo(stack, Expression.AND_LEVEL, current);
				}
				else
				{
					if (current == null)
					{
						throw t.error(Strings.getter("missingLeftOperandError", t.text));
					}
					int level = 0;
					switch (t.type)
					{
					case TOKEN_AND:
						level = Expression.AND_LEVEL;
						break;
					case TOKEN_OR:
						level = Expression.OR_LEVEL;
						break;
					case TOKEN_XOR:
						level = Expression.XOR_LEVEL;
						break;
					}
					push(stack, popTo(stack, level, current), level, t);
					current = null;
				}
			}
			current = popTo(stack, -1, current);
			if (stack.Count > 0)
			{
				Context top = pop(stack);
				throw top.cause.error(Strings.getter("rparenMissingError"));
			}
			return current;
		}

		private static void push(List<Context> stack, Expression expr, int level, Token cause)
		{
			stack.Add(new Context(expr, level, cause));
		}

		private static int peekLevel(List<Context> stack)
		{
			if (stack.Count == 0)
			{
				return -3;
			}
			Context context = stack[stack.Count - 1];
			return context.level;
		}

		private static Context pop(List<Context> stack)
		{
			return stack.RemoveAndReturn(stack.Count - 1);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static Expression popTo(java.util.ArrayList<Context> stack, int level, Expression current) throws ParserException
		private static Expression popTo(List<Context> stack, int level, Expression current)
		{
			while (stack.Count > 0 && peekLevel(stack) >= level)
			{
				Context top = pop(stack);
				if (current == null)
				{
					throw top.cause.error(Strings.getter("missingRightOperandError", top.cause.text));
				}
				switch (top.level)
				{
				case Expression.AND_LEVEL:
					current = Expressions.and(top.current, current);
					break;
				case Expression.OR_LEVEL:
					current = Expressions.or(top.current, current);
					break;
				case Expression.XOR_LEVEL:
					current = Expressions.xor(top.current, current);
					break;
				case Expression.NOT_LEVEL:
					current = Expressions.not(current);
					break;
				}
			}
			return current;
		}
	}

}
