// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit.appear
{

	using ReplacementMap = logisim.circuit.ReplacementMap;
	using Component = logisim.comp.Component;
	using ComponentEvent = logisim.comp.ComponentEvent;
	using ComponentListener = logisim.comp.ComponentListener;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using Instance = logisim.instance.Instance;
	using StdAttr = logisim.instance.StdAttr;
	using Pin = logisim.std.wiring.Pin;

	public class CircuitPins
	{
		private class MyComponentListener : ComponentListener, AttributeListener
		{
			private readonly CircuitPins outerInstance;

			public MyComponentListener(CircuitPins outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void endChanged(ComponentEvent e)
			{
				outerInstance.appearanceManager.updatePorts();
			}

			public virtual void componentInvalidated(ComponentEvent e)
			{
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> attr = e.getAttribute();
				Attribute attr = e.Attribute;
				if (attr == StdAttr.FACING || attr == StdAttr.LABEL || attr == Pin.ATTR_TYPE)
				{
					outerInstance.appearanceManager.updatePorts();
				}
			}
		}

		private PortManager appearanceManager;
		private MyComponentListener myComponentListener;
		private HashSet<Instance> pins;

		internal CircuitPins(PortManager appearanceManager)
		{
			this.appearanceManager = appearanceManager;
			myComponentListener = new MyComponentListener(this);
			pins = new HashSet<Instance>();
		}

		public virtual void transactionCompleted(ReplacementMap repl)
		{
			// determine the changes
			HashSet<Instance> adds = new HashSet<Instance>();
			HashSet<Instance> removes = new HashSet<Instance>();
			Dictionary<Instance, Instance> replaces = new Dictionary<Instance, Instance>();
			foreach (Component comp in repl.Additions)
			{
				if (comp.Factory is Pin)
				{
					Instance @in = Instance.getInstanceFor(comp);
					bool added = pins.Add(@in);
					if (added)
					{
						comp.addComponentListener(myComponentListener);
						@in.AttributeSet.addAttributeListener(myComponentListener);
						adds.Add(@in);
					}
				}
			}
			foreach (Component comp in repl.Removals)
			{
				if (comp.Factory is Pin)
				{
					Instance @in = Instance.getInstanceFor(comp);
					bool removed = pins.remove(@in);
					if (removed)
					{
						comp.removeComponentListener(myComponentListener);
						@in.AttributeSet.removeAttributeListener(myComponentListener);
						ICollection<Component> rs = repl.getComponentsReplacing(comp);
						if (rs.Count == 0)
						{
							removes.Add(@in);
						}
						else
						{
							Component r = rs.GetEnumerator().next();
							Instance rin = Instance.getInstanceFor(r);
							adds.remove(rin);
							replaces[@in] = rin;
						}
					}
				}
			}

			appearanceManager.updatePorts(adds, removes, replaces, Pins);
		}

		public virtual ICollection<Instance> Pins
		{
			get
			{
				return new List<Instance>(pins);
			}
		}
	}

}
