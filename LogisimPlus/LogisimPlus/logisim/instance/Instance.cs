// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{

	using CircuitState = logisim.circuit.CircuitState;
	using Component = logisim.comp.Component;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;

	public class Instance
	{
		public static Instance getInstanceFor(Component comp)
		{
			if (comp is InstanceComponent)
			{
				return ((InstanceComponent) comp).Instance;
			}
			else
			{
				return null;
			}
		}

		public static Component getComponentFor(Instance instance)
		{
			return instance.comp;
		}

		private InstanceComponent comp;

		internal Instance(InstanceComponent comp)
		{
			this.comp = comp;
		}

		internal virtual InstanceComponent Component
		{
			get
			{
				return comp;
			}
		}

		public virtual InstanceFactory Factory
		{
			get
			{
				return (InstanceFactory) comp.Factory;
			}
		}

		public virtual Location Location
		{
			get
			{
				return comp.Location;
			}
		}

		public virtual Bounds Bounds
		{
			get
			{
				return comp.Bounds;
			}
		}

		public virtual void setAttributeReadOnly(Attribute attr, bool value)
		{
			comp.AttributeSet.setReadOnly(attr, value);
		}

		public virtual object getAttributeValue(Attribute attr)
		{
			return comp.AttributeSet.getValue(attr);
		}

		public virtual void addAttributeListener()
		{
			comp.addAttributeListener(this);
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return comp.AttributeSet;
			}
		}

		public virtual List<Port> Ports
		{
			get
			{
				return comp.Ports;
			}
			set
			{
				comp..Ports = value;
			}
		}

		public virtual Location getPortLocation(int index)
		{
			return comp.getEnd(index).Location;
		}


		public virtual void recomputeBounds()
		{
			comp.recomputeBounds();
		}

		public virtual void setTextField(Attribute labelAttr, Attribute fontAttr, int x, int y, int halign, int valign)
		{
			comp.setTextField(labelAttr, fontAttr, x, y, halign, valign);
		}

		public virtual InstanceData getData(CircuitState state)
		{
			return (InstanceData) state.getData(comp);
		}

		public virtual void setData(CircuitState state, InstanceData data)
		{
			state.setData(comp, data);
		}

		public virtual void fireInvalidated()
		{
			comp.fireInvalidated();
		}
	}

}
