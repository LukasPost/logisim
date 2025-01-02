// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{
	public class Expressions
	{
		private Expressions()
		{
		}

		private abstract class Binary : Expression
		{
			protected internal readonly Expression a;
			protected internal readonly Expression b;

			internal Binary(Expression a, Expression b)
			{
				this.a = a;
				this.b = b;
			}

			public override bool Equals(object other)
			{
				if (other == null)
				{
					return false;
				}
				if (this.GetType() != other.GetType())
				{
					return false;
				}
				Binary o = (Binary) other;
				return this.a.Equals(o.a) && this.b.Equals(o.b);
			}

			public override int GetHashCode()
			{
				return 31 * (31 * this.GetType().GetHashCode() + a.GetHashCode()) + b.GetHashCode();
			}
		}

		private class And : Binary
		{
			internal And(Expression a, Expression b) : base(a, b)
			{
			}

			public override T visit<T>(ExpressionVisitor<T> visitor)
			{
				return visitor.visitAnd(a, b);
			}

			internal override void visit(Visitor visitor)
			{
				visitor.visitAnd(a, b);
			}

			internal override int visit(IntVisitor visitor)
			{
				return visitor.visitAnd(a, b);
			}

			public override int Precedence
			{
				get
				{
					return Expression.AND_LEVEL;
				}
			}
		}

		private class Or : Binary
		{
			internal Or(Expression a, Expression b) : base(a, b)
			{
			}

			public override T visit<T>(ExpressionVisitor<T> visitor)
			{
				return visitor.visitOr(a, b);
			}

			internal override void visit(Visitor visitor)
			{
				visitor.visitOr(a, b);
			}

			internal override int visit(IntVisitor visitor)
			{
				return visitor.visitOr(a, b);
			}

			public override int Precedence
			{
				get
				{
					return Expression.OR_LEVEL;
				}
			}
		}

		private class Xor : Binary
		{
			internal Xor(Expression a, Expression b) : base(a, b)
			{
			}

			public override T visit<T>(ExpressionVisitor<T> visitor)
			{
				return visitor.visitXor(a, b);
			}

			internal override void visit(Visitor visitor)
			{
				visitor.visitXor(a, b);
			}

			internal override int visit(IntVisitor visitor)
			{
				return visitor.visitXor(a, b);
			}

			public override int Precedence
			{
				get
				{
					return Expression.XOR_LEVEL;
				}
			}
		}

		private class Not : Expression
		{
			internal Expression a;

			internal Not(Expression a)
			{
				this.a = a;
			}

			public override T visit<T>(ExpressionVisitor<T> visitor)
			{
				return visitor.visitNot(a);
			}

			internal override void visit(Visitor visitor)
			{
				visitor.visitNot(a);
			}

			internal override int visit(IntVisitor visitor)
			{
				return visitor.visitNot(a);
			}

			public override int Precedence
			{
				get
				{
					return Expression.NOT_LEVEL;
				}
			}

			public override bool Equals(object other)
			{
				if (!(other is Not))
				{
					return false;
				}
				Not o = (Not) other;
				return this.a.Equals(o.a);
			}

			public override int GetHashCode()
			{
				return 31 * a.GetHashCode();
			}
		}

		private class Variable : Expression
		{
			internal string name;

			internal Variable(string name)
			{
				this.name = name;
			}

			public override T visit<T>(ExpressionVisitor<T> visitor)
			{
				return visitor.visitVariable(name);
			}

			internal override void visit(Visitor visitor)
			{
				visitor.visitVariable(name);
			}

			internal override int visit(IntVisitor visitor)
			{
				return visitor.visitVariable(name);
			}

			public override int Precedence
			{
				get
				{
					return int.MaxValue;
				}
			}

			public override bool Equals(object other)
			{
				if (!(other is Variable))
				{
					return false;
				}
				Variable o = (Variable) other;
				return this.name.Equals(o.name);
			}

			public override int GetHashCode()
			{
				return name.GetHashCode();
			}
		}

		private class Constant : Expression
		{
			internal int value;

			internal Constant(int value)
			{
				this.value = value;
			}

			public override T visit<T>(ExpressionVisitor<T> visitor)
			{
				return visitor.visitConstant(value);
			}

			internal override void visit(Visitor visitor)
			{
				visitor.visitConstant(value);
			}

			internal override int visit(IntVisitor visitor)
			{
				return visitor.visitConstant(value);
			}

			public override int Precedence
			{
				get
				{
					return int.MaxValue;
				}
			}

			public override bool Equals(object other)
			{
				if (!(other is Constant))
				{
					return false;
				}
				Constant o = (Constant) other;
				return this.value == o.value;
			}

			public override int GetHashCode()
			{
				return value;
			}
		}

		public static Expression and(Expression a, Expression b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			return new And(a, b);
		}

		public static Expression or(Expression a, Expression b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			return new Or(a, b);
		}

		public static Expression xor(Expression a, Expression b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			return new Xor(a, b);
		}

		public static Expression not(Expression a)
		{
			if (a == null)
			{
				return null;
			}
			return new Not(a);
		}

		public static Expression variable(string name)
		{
			return new Variable(name);
		}

		public static Expression constant(int value)
		{
			return new Constant(value);
		}
	}

}
