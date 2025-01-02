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

namespace logisim.analyze.gui
{

	using Expression = logisim.analyze.model.Expression;
	using logisim.analyze.model;

	internal class ExpressionView : JPanel
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private const int BADNESS_IDENT_BREAK = 10000;
		private const int BADNESS_BEFORE_SPACE = 500;
		private const int BADNESS_BEFORE_AND = 50;
		private const int BADNESS_BEFORE_XOR = 30;
		private const int BADNESS_BEFORE_OR = 0;
		private const int BADNESS_NOT_BREAK = 100;
		private const int BADNESS_PER_NOT_BREAK = 30;
		private const int BADNESS_PER_PIXEL = 1;

		private const int NOT_SEP = 3;
		private const int EXTRA_LEADING = 4;
		private const int MINIMUM_HEIGHT = 25;

		private class MyListener : ComponentListener
		{
			private readonly ExpressionView outerInstance;

			public MyListener(ExpressionView outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void componentResized(ComponentEvent arg0)
			{
				int width = getWidth();
				if (outerInstance.renderData != null && Math.Abs(outerInstance.renderData.width - width) > 2)
				{
					Graphics g = getGraphics();
					FontMetrics fm = g == null ? null : g.getFontMetrics();
					outerInstance.renderData = new RenderData(outerInstance.renderData.exprData, width, fm);
					setPreferredSize(outerInstance.renderData.PreferredSize);
					revalidate();
					repaint();
				}
			}

			public virtual void componentMoved(ComponentEvent arg0)
			{
			}

			public virtual void componentShown(ComponentEvent arg0)
			{
			}

			public virtual void componentHidden(ComponentEvent arg0)
			{
			}
		}

		private MyListener myListener;
		private RenderData renderData;

		public ExpressionView()
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			addComponentListener(myListener);
			Expression = null;
		}

		public virtual Expression Expression
		{
			set
			{
				ExpressionData exprData = new ExpressionData(value);
				Graphics g = getGraphics();
				FontMetrics fm = g == null ? null : g.getFontMetrics();
				renderData = new RenderData(exprData, getWidth(), fm);
				setPreferredSize(renderData.PreferredSize);
				revalidate();
				repaint();
			}
		}

		public override void paintComponent(Graphics g)
		{
			base.paintComponent(g);

			if (renderData != null)
			{
				int x = Math.Max(0, (getWidth() - renderData.prefWidth) / 2);
				int y = Math.Max(0, (getHeight() - renderData.height) / 2);
				renderData.paint(g, x, y);
			}
		}

		internal virtual void localeChanged()
		{
			repaint();
		}

		private class NotData
		{
			internal int startIndex;
			internal int stopIndex;
			internal int depth;
		}

		private class ExpressionData
		{
			internal string text;
			internal readonly List<NotData> nots = new List<NotData>();
			internal int[] badness;

			internal ExpressionData(Expression expr)
			{
				if (expr == null)
				{
					text = "";
					badness = new int[0];
				}
				else
				{
					computeText(expr);
					computeBadnesses();
				}
			}

			internal virtual void computeText(Expression expr)
			{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final StringBuilder text = new StringBuilder();
				StringBuilder text = new StringBuilder();
				expr.visit(new ExpressionVisitorAnonymousInnerClass(this, text));
				this.text = text.ToString();
			}

			private class ExpressionVisitorAnonymousInnerClass : ExpressionVisitor<object>
			{
				private readonly ExpressionData outerInstance;

				private StringBuilder text;

				public ExpressionVisitorAnonymousInnerClass(ExpressionData outerInstance, StringBuilder text)
				{
					this.outerInstance = outerInstance;
					this.text = text;
				}

				public object visitAnd(Expression a, Expression b)
				{
					return binary(a, b, Expression.AND_LEVEL, " ");
				}

				public object visitOr(Expression a, Expression b)
				{
					return binary(a, b, Expression.OR_LEVEL, " + ");
				}

				public object visitXor(Expression a, Expression b)
				{
					return binary(a, b, Expression.XOR_LEVEL, " ^ ");
				}

				private object binary(Expression a, Expression b, int level, string op)
				{
					if (a.Precedence < level)
					{
						text.Append("(");
						a.visit(this);
						text.Append(")");
					}
					else
					{
						a.visit(this);
					}
					text.Append(op);
					if (b.Precedence < level)
					{
						text.Append("(");
						b.visit(this);
						text.Append(")");
					}
					else
					{
						b.visit(this);
					}
					return null;
				}

				public object visitNot(Expression a)
				{
					NotData notData = new NotData();
					notData.startIndex = text.Length;
					outerInstance.nots.Add(notData);
					a.visit(this);
					notData.stopIndex = text.Length;
					return null;
				}

				public object visitVariable(string name)
				{
					text.Append(name);
					return null;
				}

				public object visitConstant(int value)
				{
					text.Append("" + Convert.ToString(value, 16));
					return null;
				}
			}

			internal virtual void computeBadnesses()
			{
				badness = new int[text.Length + 1];
				badness[text.Length] = 0;
				if (text.Length == 0)
				{
					return;
				}

				badness[0] = int.MaxValue;
				NotData curNot = nots.Count == 0 ? null : (NotData) nots[0];
				int curNotIndex = 0;
				char prev = text[0];
				for (int i = 1; i < text.Length; i++)
				{
					// invariant: curNot.stopIndex >= i (and is first such),
					// or curNot == null if none such exists
					char cur = text[i];
					if (cur == ' ')
					{
						badness[i] = BADNESS_BEFORE_SPACE;
						;
					}
					else if (Character.isJavaIdentifierPart(cur))
					{
						if (Character.isJavaIdentifierPart(prev))
						{
							badness[i] = BADNESS_IDENT_BREAK;
						}
						else
						{
							badness[i] = BADNESS_BEFORE_AND;
						}
					}
					else if (cur == '+')
					{
						badness[i] = BADNESS_BEFORE_OR;
					}
					else if (cur == '^')
					{
						badness[i] = BADNESS_BEFORE_XOR;
					}
					else if (cur == ')')
					{
						badness[i] = BADNESS_BEFORE_SPACE;
					}
					else
					{ // cur == '('
						badness[i] = BADNESS_BEFORE_AND;
					}

					while (curNot != null && curNot.stopIndex <= i)
					{
						++curNotIndex;
						curNot = (curNotIndex >= nots.Count ? null : (NotData) nots[curNotIndex]);
					}

					if (curNot != null && badness[i] < BADNESS_IDENT_BREAK)
					{
						int depth = 0;
						NotData nd = curNot;
						int ndi = curNotIndex;
						while (nd != null && nd.startIndex < i)
						{
							if (nd.stopIndex > i)
							{
								++depth;
							}
							++ndi;
							nd = ndi < nots.Count ? (NotData) nots[ndi] : null;
						}
						if (depth > 0)
						{
							badness[i] += BADNESS_NOT_BREAK + (depth - 1) * BADNESS_PER_NOT_BREAK;
						}
					}

					prev = cur;
				}
			}
		}

		private class RenderData
		{
			internal ExpressionData exprData;
			internal int prefWidth;
			internal int width;
			internal int height;
			internal string[] lineText;
			internal List<List<NotData>> lineNots;
			internal int[] lineY;

			internal RenderData(ExpressionData exprData, int width, FontMetrics fm)
			{
				this.exprData = exprData;
				this.width = width;
				height = MINIMUM_HEIGHT;

				if (fm == null)
				{
					lineText = new string[] {exprData.text};
					lineNots = new List<List<NotData>>();
					lineNots.Add(exprData.nots);
					computeNotDepths();
					lineY = new int[] {MINIMUM_HEIGHT};
				}
				else
				{
					if (exprData.text.Length == 0)
					{
						lineText = new string[] {Strings.get("expressionEmpty")};
						lineNots = new List<List<NotData>>();
						lineNots.Add(new List<NotData>());
					}
					else
					{
						computeLineText(fm);
						computeLineNots();
						computeNotDepths();
					}
					computeLineY(fm);
					prefWidth = lineText.Length > 1 ? width : fm.stringWidth(lineText[0]);
				}
			}

			internal virtual void computeLineText(FontMetrics fm)
			{
				string text = exprData.text;
				int[] badness = exprData.badness;

				if (fm.stringWidth(text) <= width)
				{
					lineText = new string[] {text};
					return;
				}

				int startPos = 0;
				List<string> lines = new List<string>();
				while (startPos < text.Length)
				{
					int stopPos = startPos + 1;
					string bestLine = text.Substring(startPos, stopPos - startPos);
					if (stopPos >= text.Length)
					{
						lines.Add(bestLine);
						break;
					}
					int bestStopPos = stopPos;
					int lineWidth = fm.stringWidth(bestLine);
					int bestBadness = badness[stopPos] + (width - lineWidth) * BADNESS_PER_PIXEL;
					while (stopPos < text.Length)
					{
						++stopPos;
						string line = text.Substring(startPos, stopPos - startPos);
						lineWidth = fm.stringWidth(line);
						if (lineWidth > width)
						{
							break;
						}

						int lineBadness = badness[stopPos] + (width - lineWidth) * BADNESS_PER_PIXEL;
						if (lineBadness < bestBadness)
						{
							bestBadness = lineBadness;
							bestStopPos = stopPos;
							bestLine = line;
						}
					}
					lines.Add(bestLine);
					startPos = bestStopPos;
				}
				lineText = lines.ToArray();
			}

			internal virtual void computeLineNots()
			{
				List<NotData> allNots = exprData.nots;
				lineNots = new List<List<NotData>>();
				for (int i = 0; i < lineText.Length; i++)
				{
					lineNots.Add(new List<NotData>());
				}
				foreach (NotData nd in allNots)
				{
					int pos = 0;
					for (int j = 0; j < lineNots.Count && pos < nd.stopIndex; j++)
					{
						string line = lineText[j];
						int nextPos = pos + line.Length;
						if (nextPos > nd.startIndex)
						{
							NotData toAdd = new NotData();
							toAdd.startIndex = Math.Max(pos, nd.startIndex) - pos;
							toAdd.stopIndex = Math.Min(nextPos, nd.stopIndex) - pos;
							lineNots[j].Add(toAdd);
						}
						pos = nextPos;
					}
				}
			}

			internal virtual void computeNotDepths()
			{
				foreach (List<NotData> nots in lineNots)
				{
					int n = nots.Count;
					int[] stack = new int[n];
					for (int i = 0; i < nots.Count; i++)
					{
						NotData nd = nots[i];
						int depth = 0;
						int top = 0;
						stack[0] = nd.stopIndex;
						for (int j = i + 1; j < nots.Count; j++)
						{
							NotData nd2 = nots[j];
							if (nd2.startIndex >= nd.stopIndex)
							{
								break;
							}
							while (nd2.startIndex >= stack[top])
							{
								top--;
							}
							++top;
							stack[top] = nd2.stopIndex;
							if (top > depth)
							{
								depth = top;
							}
						}
						nd.depth = depth;
					}
				}
			}

			internal virtual void computeLineY(FontMetrics fm)
			{
				lineY = new int[lineNots.Count];
				int curY = 0;
				for (int i = 0; i < lineY.Length; i++)
				{
					int maxDepth = -1;
					List<NotData> nots = lineNots[i];
					foreach (NotData nd in nots)
					{
						if (nd.depth > maxDepth)
						{
							maxDepth = nd.depth;
						}
					}
					lineY[i] = curY + maxDepth * NOT_SEP;
					curY = lineY[i] + fm.getHeight() + EXTRA_LEADING;
				}
				height = Math.Max(MINIMUM_HEIGHT, curY - fm.getLeading() - EXTRA_LEADING);
			}

			public virtual Size PreferredSize
			{
				get
				{
					return new Size(10, height);
				}
			}

			public virtual void paint(Graphics g, int x, int y)
			{
				FontMetrics fm = g.getFontMetrics();
				int i = -1;
				foreach (string line in lineText)
				{
					i++;
					g.drawString(line, x, y + lineY[i] + fm.getAscent());

					List<NotData> nots = lineNots[i];
					foreach (NotData nd in nots)
					{
						int notY = y + lineY[i] - nd.depth * NOT_SEP;
						int startX = x + fm.stringWidth(line.Substring(0, nd.startIndex));
						int stopX = x + fm.stringWidth(line.Substring(0, nd.stopIndex));
						g.drawLine(startX, notY, stopX, notY);
					}
				}
			}
		}
	}

}
