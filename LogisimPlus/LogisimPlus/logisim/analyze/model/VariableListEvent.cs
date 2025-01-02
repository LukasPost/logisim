// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{
	public class VariableListEvent
	{
		public const int ALL_REPLACED = 0;
		public const int ADD = 1;
		public const int REMOVE = 2;
		public const int MOVE = 3;
		public const int REPLACE = 4;

		private VariableList source;
		private int type;
		private string variable;
		private object data;

		public VariableListEvent(VariableList source, int type, string variable, object data)
		{
			this.source = source;
			this.type = type;
			this.variable = variable;
			this.data = data;
		}

		public virtual VariableList Source
		{
			get
			{
				return source;
			}
		}

		public virtual int Type
		{
			get
			{
				return type;
			}
		}

		public virtual string Variable
		{
			get
			{
				return variable;
			}
		}

		public virtual object Data
		{
			get
			{
				return data;
			}
		}
	}

}
