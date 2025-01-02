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

	public abstract class Expression
	{
		public const int OR_LEVEL = 0;
		public const int XOR_LEVEL = 1;
		public const int AND_LEVEL = 2;
		public const int NOT_LEVEL = 3;

		internal interface Visitor
		{
			void visitAnd(Expression a, Expression b);

			void visitOr(Expression a, Expression b);

			void visitXor(Expression a, Expression b);

			void visitNot(Expression a);

			void visitVariable(string name);

			void visitConstant(int value);
		}

		internal interface IntVisitor
		{
			int visitAnd(Expression a, Expression b);

			int visitOr(Expression a, Expression b);

			int visitXor(Expression a, Expression b);

			int visitNot(Expression a);

			int visitVariable(string name);

			int visitConstant(int value);
		}

		public abstract int Precedence {get;}

		public abstract T visit<T>(ExpressionVisitor<T> visitor);

		internal abstract void visit(Visitor visitor);

		internal abstract int visit(IntVisitor visitor);

		public virtual bool evaluate(in Assignments assignments)
		{
			int ret = visit(new IntVisitorAnonymousInnerClass(this, assignments));
			return (ret & 1) != 0;
		}

		private class IntVisitorAnonymousInnerClass : IntVisitor
		{
			private readonly Expression outerInstance;

			private logisim.analyze.model.Assignments assignments;

			public IntVisitorAnonymousInnerClass(Expression outerInstance, logisim.analyze.model.Assignments assignments)
			{
				this.outerInstance = outerInstance;
				this.assignments = assignments;
			}

			public int visitAnd(Expression a, Expression b)
			{
				return a.visit(this) & b.visit(this);
			}

			public int visitOr(Expression a, Expression b)
			{
				return a.visit(this) | b.visit(this);
			}

			public int visitXor(Expression a, Expression b)
			{
				return a.visit(this) ^ b.visit(this);
			}

			public int visitNot(Expression a)
			{
				return ~a.visit(this);
			}

			public int visitVariable(string name)
			{
				return assignments.get(name) ? 1 : 0;
			}

			public int visitConstant(int value)
			{
				return value;
			}
		}

		public override string ToString()
		{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final StringBuilder text = new StringBuilder();
			StringBuilder text = new StringBuilder();
			visit(new VisitorAnonymousInnerClass(this, text));
			return text.ToString();
		}

		private class VisitorAnonymousInnerClass : Visitor
		{
			private readonly Expression outerInstance;

			private StringBuilder text;

			public VisitorAnonymousInnerClass(Expression outerInstance, StringBuilder text)
			{
				this.outerInstance = outerInstance;
				this.text = text;
			}

			public void visitAnd(Expression a, Expression b)
			{
				binary(a, b, AND_LEVEL, " ");
			}

			public void visitOr(Expression a, Expression b)
			{
				binary(a, b, OR_LEVEL, " + ");
			}

			public void visitXor(Expression a, Expression b)
			{
				binary(a, b, XOR_LEVEL, " ^ ");
			}

			private void binary(Expression a, Expression b, int level, string op)
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
			}

			public void visitNot(Expression a)
			{
				text.Append("~");
				if (a.Precedence < NOT_LEVEL)
				{
					text.Append("(");
					a.visit(this);
					text.Append(")");
				}
				else
				{
					a.visit(this);
				}
			}

			public void visitVariable(string name)
			{
				text.Append(name);
			}

			public void visitConstant(int value)
			{
				text.Append("" + Convert.ToString(value, 16));
			}
		}

		public virtual bool Circular
		{
			get
			{
	// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	// ORIGINAL LINE: final java.util.HashSet<Expression> visited = new java.util.HashSet<Expression>();
				HashSet<Expression> visited = new HashSet<Expression>();
				visited.Add(this);
				return 1 == visit(new IntVisitorAnonymousInnerClass2(this, visited));
			}
		}

		private class IntVisitorAnonymousInnerClass2 : IntVisitor
		{
			private readonly Expression outerInstance;

			private HashSet<Expression> visited;

			public IntVisitorAnonymousInnerClass2(Expression outerInstance, HashSet<Expression> visited)
			{
				this.outerInstance = outerInstance;
				this.visited = visited;
			}

			public int visitAnd(Expression a, Expression b)
			{
				return binary(a, b);
			}

			public int visitOr(Expression a, Expression b)
			{
				return binary(a, b);
			}

			public int visitXor(Expression a, Expression b)
			{
				return binary(a, b);
			}

			public int visitNot(Expression a)
			{
				if (!visited.Add(a))
				{
					return 1;
				}
				if (a.visit(this) == 1)
				{
					return 1;
				}
				visited.Remove(a);
				return 0;
			}

			public int visitVariable(string name)
			{
				return 0;
			}

			public int visitConstant(int value)
			{
				return 0;
			}

			private int binary(Expression a, Expression b)
			{
				if (!visited.Add(a))
				{
					return 1;
				}
				if (a.visit(this) == 1)
				{
					return 1;
				}
				visited.Remove(a);

				if (!visited.Add(b))
				{
					return 1;
				}
				if (b.visit(this) == 1)
				{
					return 1;
				}
				visited.Remove(b);

				return 0;
			}
		}

		internal virtual Expression removeVariable(in string input)
		{
			return visit(new ExpressionVisitorAnonymousInnerClass(this, input));
		}

		private class ExpressionVisitorAnonymousInnerClass : ExpressionVisitor<Expression>
		{
			private readonly Expression outerInstance;

			private string input;

			public ExpressionVisitorAnonymousInnerClass(Expression outerInstance, string input)
			{
				this.outerInstance = outerInstance;
				this.input = input;
			}

			public Expression visitAnd(Expression a, Expression b)
			{
				Expression l = a.visit(this);
				Expression r = b.visit(this);
				if (l == null)
				{
					return r;
				}
				if (r == null)
				{
					return l;
				}
				return Expressions.and(l, r);
			}

			public Expression visitOr(Expression a, Expression b)
			{
				Expression l = a.visit(this);
				Expression r = b.visit(this);
				if (l == null)
				{
					return r;
				}
				if (r == null)
				{
					return l;
				}
				return Expressions.or(l, r);
			}

			public Expression visitXor(Expression a, Expression b)
			{
				Expression l = a.visit(this);
				Expression r = b.visit(this);
				if (l == null)
				{
					return r;
				}
				if (r == null)
				{
					return l;
				}
				return Expressions.xor(l, r);
			}

			public Expression visitNot(Expression a)
			{
				Expression l = a.visit(this);
				if (l == null)
				{
					return null;
				}
				return Expressions.not(l);
			}

			public Expression visitVariable(string name)
			{
				return name.Equals(input) ? null : Expressions.variable(name);
			}

			public Expression visitConstant(int value)
			{
				return Expressions.constant(value);
			}
		}

		internal virtual Expression replaceVariable(in string oldName, in string newName)
		{
			return visit(new ExpressionVisitorAnonymousInnerClass2(this, oldName, newName));
		}

		private class ExpressionVisitorAnonymousInnerClass2 : ExpressionVisitor<Expression>
		{
			private readonly Expression outerInstance;

			private string oldName;
			private string newName;

			public ExpressionVisitorAnonymousInnerClass2(Expression outerInstance, string oldName, string newName)
			{
				this.outerInstance = outerInstance;
				this.oldName = oldName;
				this.newName = newName;
			}

			public Expression visitAnd(Expression a, Expression b)
			{
				Expression l = a.visit(this);
				Expression r = b.visit(this);
				return Expressions.and(l, r);
			}

			public Expression visitOr(Expression a, Expression b)
			{
				Expression l = a.visit(this);
				Expression r = b.visit(this);
				return Expressions.or(l, r);
			}

			public Expression visitXor(Expression a, Expression b)
			{
				Expression l = a.visit(this);
				Expression r = b.visit(this);
				return Expressions.xor(l, r);
			}

			public Expression visitNot(Expression a)
			{
				Expression l = a.visit(this);
				return Expressions.not(l);
			}

			public Expression visitVariable(string name)
			{
				return Expressions.variable(name.Equals(oldName) ? newName : name);
			}

			public Expression visitConstant(int value)
			{
				return Expressions.constant(value);
			}
		}

		public virtual bool containsXor()
		{
			return 1 == visit(new IntVisitorAnonymousInnerClass3(this));
		}

		private class IntVisitorAnonymousInnerClass3 : IntVisitor
		{
			private readonly Expression outerInstance;

			public IntVisitorAnonymousInnerClass3(Expression outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public int visitAnd(Expression a, Expression b)
			{
				return a.visit(this) == 1 || b.visit(this) == 1 ? 1 : 0;
			}

			public int visitOr(Expression a, Expression b)
			{
				return a.visit(this) == 1 || b.visit(this) == 1 ? 1 : 0;
			}

			public int visitXor(Expression a, Expression b)
			{
				return 1;
			}

			public int visitNot(Expression a)
			{
				return a.visit(this);
			}

			public int visitVariable(string name)
			{
				return 0;
			}

			public int visitConstant(int value)
			{
				return 0;
			}
		}

		public virtual bool Cnf
		{
			get
			{
				return 1 == visit(new IntVisitorAnonymousInnerClass4(this));
			}
		}

		private class IntVisitorAnonymousInnerClass4 : IntVisitor
		{
			private readonly Expression outerInstance;

			public IntVisitorAnonymousInnerClass4(Expression outerInstance)
			{
				this.outerInstance = outerInstance;
				level = 0;
			}

			internal int level;

			public int visitAnd(Expression a, Expression b)
			{
				if (level > 1)
				{
					return 0;
				}
				int oldLevel = level;
				level = 1;
				int ret = a.visit(this) == 1 && b.visit(this) == 1 ? 1 : 0;
				level = oldLevel;
				return ret;
			}

			public int visitOr(Expression a, Expression b)
			{
				if (level > 0)
				{
					return 0;
				}
				return a.visit(this) == 1 && b.visit(this) == 1 ? 1 : 0;
			}

			public int visitXor(Expression a, Expression b)
			{
				return 0;
			}

			public int visitNot(Expression a)
			{
				if (level == 2)
				{
					return 0;
				}
				int oldLevel = level;
				level = 2;
				int ret = a.visit(this);
				level = oldLevel;
				return ret;
			}

			public int visitVariable(string name)
			{
				return 1;
			}

			public int visitConstant(int value)
			{
				return 1;
			}
		}
	}

}
