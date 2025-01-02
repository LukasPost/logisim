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
	using Direction = logisim.data.Direction;

	public class DirectionConfigurator : KeyConfigurator, ICloneable
	{
		private Attribute<Direction> attr;
		private int modsEx;

		public DirectionConfigurator(Attribute<Direction> attr, int modifiersEx)
		{
			this.attr = attr;
			this.modsEx = modifiersEx;
		}

		public virtual DirectionConfigurator clone()
		{
			try
			{
				return (DirectionConfigurator) base.clone();
			}
			catch (CloneNotSupportedException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				return null;
			}
		}

		public virtual KeyConfigurationResult keyEventReceived(KeyConfigurationEvent @event)
		{
			if (@event.Type == KeyConfigurationEvent.KEY_PRESSED)
			{
				KeyEvent e = @event.KeyEvent;
				if (e.getModifiersEx() == modsEx)
				{
					Direction value = null;
					switch (e.getKeyCode())
					{
					case KeyEvent.VK_UP:
						value = Direction.North;
						break;
					case KeyEvent.VK_DOWN:
						value = Direction.South;
						break;
					case KeyEvent.VK_LEFT:
						value = Direction.West;
						break;
					case KeyEvent.VK_RIGHT:
						value = Direction.East;
						break;
					}
					if (value != null)
					{
						@event.consume();
						return new KeyConfigurationResult(@event, attr, value);
					}
				}
			}
			return null;
		}
	}

}
