// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{
	public class ComponentEvent
	{
		private Component source;
		private object oldData;
		private object newData;

		public ComponentEvent(Component source) : this(source, null, null)
		{
		}

		public ComponentEvent(Component source, object oldData, object newData)
		{
			this.source = source;
			this.oldData = oldData;
			this.newData = newData;
		}

		public virtual Component Source
		{
			get
			{
				return source;
			}
		}

		public virtual object Data
		{
			get
			{
				return newData;
			}
		}

		public virtual object OldData
		{
			get
			{
				return oldData;
			}
		}
	}

}
