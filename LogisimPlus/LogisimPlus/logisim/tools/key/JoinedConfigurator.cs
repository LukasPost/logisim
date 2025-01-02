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
	public class JoinedConfigurator : KeyConfigurator, ICloneable
	{
		public static JoinedConfigurator create(KeyConfigurator a, KeyConfigurator b)
		{
			return new JoinedConfigurator(new KeyConfigurator[] {a, b});
		}

		public static JoinedConfigurator create(KeyConfigurator[] configs)
		{
			return new JoinedConfigurator(configs);
		}

		private KeyConfigurator[] handlers;

		private JoinedConfigurator(KeyConfigurator[] handlers)
		{
			this.handlers = handlers;
		}

		public virtual JoinedConfigurator clone()
		{
			JoinedConfigurator ret;
			try
			{
				ret = (JoinedConfigurator) base.clone();
			}
			catch (CloneNotSupportedException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				return null;
			}
			int len = this.handlers.Length;
			ret.handlers = new KeyConfigurator[len];
			for (int i = 0; i < len; i++)
			{
				ret.handlers[i] = this.handlers[i].clone();
			}
			return ret;
		}

		public virtual KeyConfigurationResult keyEventReceived(KeyConfigurationEvent @event)
		{
			KeyConfigurator[] hs = handlers;
			if (@event.Consumed)
			{
				return null;
			}
			for (int i = 0; i < hs.Length; i++)
			{
				KeyConfigurationResult result = hs[i].keyEventReceived(@event);
				if (result != null || @event.Consumed)
				{
					return result;
				}
			}
			return null;
		}
	}

}
