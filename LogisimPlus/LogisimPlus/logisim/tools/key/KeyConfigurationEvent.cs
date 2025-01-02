// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.key
{

	using AttributeSet = logisim.data.AttributeSet;

	public class KeyConfigurationEvent
	{
		public const int KEY_PRESSED = 0;
		public const int KEY_RELEASED = 1;
		public const int KEY_TYPED = 2;

		private int type;
		private AttributeSet attrs;
		private KeyEvent @event;
		private object data;
		private bool consumed;

		public KeyConfigurationEvent(int type, AttributeSet attrs, KeyEvent @event, object data)
		{
			this.type = type;
			this.attrs = attrs;
			this.@event = @event;
			this.data = data;
			this.consumed = false;
		}

		public virtual int Type
		{
			get
			{
				return type;
			}
		}

		public virtual KeyEvent KeyEvent
		{
			get
			{
				return @event;
			}
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return attrs;
			}
		}

		public virtual void consume()
		{
			consumed = true;
		}

		public virtual bool Consumed
		{
			get
			{
				return consumed;
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
