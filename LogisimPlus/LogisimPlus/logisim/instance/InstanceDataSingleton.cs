// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{
	public class InstanceDataSingleton : InstanceData, ICloneable
	{
		private object value;

		public InstanceDataSingleton(object value)
		{
			this.value = value;
		}

		public virtual InstanceDataSingleton clone()
		{
			try
			{
				return (InstanceDataSingleton) base.clone();
			}
			catch (CloneNotSupportedException)
			{
				return null;
			}
		}

		public virtual object Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

	}

}
