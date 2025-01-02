﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.undo
{
	public abstract class Action
	{
		public virtual bool Modification
		{
			get
			{
				return true;
			}
		}

		public abstract string Name {get;}

		public abstract void doIt();

		public abstract void undo();

		public virtual bool shouldAppendTo(Action other)
		{
			return false;
		}

		public virtual Action append(Action other)
		{
			return new ActionUnion(this, other);
		}
	}

}