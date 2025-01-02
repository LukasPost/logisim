// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.key
{

	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;

	public abstract class NumericConfigurator<V> : KeyConfigurator, ICloneable
	{
		private const int MAX_TIME_KEY_LASTS = 800;

		private Attribute<V> attr;
		private int minValue;
		private int maxValue;
		private int curValue;
		private int radix;
		private int modsEx;
		private long whenTyped;

		public NumericConfigurator(Attribute<V> attr, int min, int max, int modifiersEx) : this(attr, min, max, modifiersEx, 10)
		{
		}

		public NumericConfigurator(Attribute<V> attr, int min, int max, int modifiersEx, int radix)
		{
			this.attr = attr;
			this.minValue = min;
			this.maxValue = max;
			this.radix = radix;
			this.modsEx = modifiersEx;
			this.curValue = 0;
			this.whenTyped = 0;
		}

		public virtual NumericConfigurator<V> clone()
		{
			try
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") NumericConfigurator<V> ret = (NumericConfigurator<V>) super.clone();
				NumericConfigurator<V> ret = (NumericConfigurator<V>) base.clone();
				ret.whenTyped = 0;
				ret.curValue = 0;
				return ret;
			}
			catch (CloneNotSupportedException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				return null;
			}
		}

		protected internal virtual int getMinimumValue(AttributeSet attrs)
		{
			return minValue;
		}

		protected internal virtual int getMaximumValue(AttributeSet attrs)
		{
			return maxValue;
		}

		protected internal abstract V createValue(int value);

		public virtual KeyConfigurationResult keyEventReceived(KeyConfigurationEvent @event)
		{
			if (@event.Type == KeyConfigurationEvent.KEY_TYPED)
			{
				KeyEvent e = @event.KeyEvent;
				int digit = Character.digit(e.getKeyChar(), radix);
				if (digit >= 0 && e.getModifiersEx() == modsEx)
				{
					long now = DateTimeHelper.CurrentUnixTimeMillis();
					long sinceLast = now - whenTyped;
					AttributeSet attrs = @event.AttributeSet;
					int min = getMinimumValue(attrs);
					int max = getMaximumValue(attrs);
					int val = 0;
					if (sinceLast < MAX_TIME_KEY_LASTS)
					{
						val = radix * curValue;
						if (val > max)
						{
							val = 0;
						}
					}
					val += digit;
					if (val > max)
					{
						val = digit;
						if (val > max)
						{
							return null;
						}
					}
					@event.consume();
					whenTyped = now;
					curValue = val;

					if (val >= min)
					{
						object valObj = createValue(val);
						return new KeyConfigurationResult(@event, attr, valObj);
					}
				}
			}
			return null;
		}
	}

}
