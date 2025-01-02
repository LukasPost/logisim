// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{
	public interface ExpressionVisitor<T>
	{
		T visitAnd(Expression a, Expression b);

		T visitOr(Expression a, Expression b);

		T visitXor(Expression a, Expression b);

		T visitNot(Expression a);

		T visitVariable(string name);

		T visitConstant(int value);
	}

}
