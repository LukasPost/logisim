// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.model
{
	public class OutputExpressionsEvent
	{
		public const int ALL_VARIABLES_REPLACED = 0;
		public const int OUTPUT_EXPRESSION = 1;
		public const int OUTPUT_MINIMAL = 2;

		private AnalyzerModel model;
		private int type;
		private string variable;
		private object data;

		public OutputExpressionsEvent(AnalyzerModel model, int type, string variable, object data)
		{
			this.model = model;
			this.type = type;
			this.variable = variable;
			this.data = data;
		}

		public virtual AnalyzerModel Model
		{
			get
			{
				return model;
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
