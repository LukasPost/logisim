// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.key
{

	using logisim.data;

	public class ParallelConfigurator : KeyConfigurator, ICloneable
	{
		public static ParallelConfigurator create(KeyConfigurator a, KeyConfigurator b)
		{
			return new ParallelConfigurator(new KeyConfigurator[] {a, b});
		}

		public static ParallelConfigurator create(KeyConfigurator[] configs)
		{
			return new ParallelConfigurator(configs);
		}

		private KeyConfigurator[] handlers;

		private ParallelConfigurator(KeyConfigurator[] handlers)
		{
			this.handlers = handlers;
		}

		public virtual object Clone()
		{
			ParallelConfigurator ret = (ParallelConfigurator) base.MemberwiseClone();
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
			KeyConfigurationResult first = null;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.HashMap<logisim.data.Attribute<?>, Object> map = null;
			Dictionary<Attribute, object> map = null;
			for (int i = 0; i < hs.Length; i++)
			{
				KeyConfigurationResult result = hs[i].keyEventReceived(@event);
				if (result != null)
				{
					if (first == null)
					{
						first = result;
					}
					else if (map == null)
					{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: map = new java.util.HashMap<logisim.data.Attribute<?>, Object>(first.getAttributeValues());
						map = new Dictionary<Attribute, object>(first.AttributeValues);
						map.PutAll(result.AttributeValues);
					}
					else
					{
						map.PutAll(result.AttributeValues);
					}
				}
			}
			if (map != null)
			{
				return new KeyConfigurationResult(@event, map);
			}
			else
			{
				return first;
			}
		}
	}

}
