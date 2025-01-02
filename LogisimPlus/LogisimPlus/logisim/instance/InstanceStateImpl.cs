// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{
	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using Component = logisim.comp.Component;
	using EndData = logisim.comp.EndData;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Project = logisim.proj.Project;

	internal class InstanceStateImpl : InstanceState
	{
		private CircuitState circuitState;
		private Component component;

		public InstanceStateImpl(CircuitState circuitState, Component component)
		{
			this.circuitState = circuitState;
			this.component = component;
		}

		public virtual void repurpose(CircuitState circuitState, Component component)
		{
			this.circuitState = circuitState;
			this.component = component;
		}

		internal virtual CircuitState CircuitState
		{
			get
			{
				return circuitState;
			}
		}

		public virtual Project Project
		{
			get
			{
				return circuitState.Project;
			}
		}

		public virtual Instance Instance
		{
			get
			{
				if (component is InstanceComponent)
				{
					return ((InstanceComponent) component).Instance;
				}
				else
				{
					return null;
				}
			}
		}

		public virtual InstanceFactory Factory
		{
			get
			{
				if (component is InstanceComponent)
				{
					InstanceComponent comp = (InstanceComponent) component;
					return (InstanceFactory) comp.Factory;
				}
				else
				{
					return null;
				}
			}
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return component.AttributeSet;
			}
		}

		public virtual E getAttributeValue<E>(Attribute<E> attr)
		{
			return component.AttributeSet.getValue(attr);
		}

		public virtual Value getPort(int portIndex)
		{
			EndData data = component.getEnd(portIndex);
			return circuitState.getValue(data.Location);
		}

		public virtual bool isPortConnected(int index)
		{
			Circuit circ = circuitState.Circuit;
			Location loc = component.getEnd(index).Location;
			return circ.isConnected(loc, component);
		}

		public virtual void setPort(int portIndex, Value value, int delay)
		{
			EndData end = component.getEnd(portIndex);
			circuitState.setValue(end.Location, value, component, delay);
		}

		public virtual InstanceData Data
		{
			get
			{
				return (InstanceData) circuitState.getData(component);
			}
			set
			{
				circuitState.setData(component, value);
			}
		}


		public virtual void fireInvalidated()
		{
			if (component is InstanceComponent)
			{
				((InstanceComponent) component).fireInvalidated();
			}
		}

		public virtual bool CircuitRoot
		{
			get
			{
				return !circuitState.Substate;
			}
		}

		public virtual long TickCount
		{
			get
			{
				return circuitState.Propagator.TickCount;
			}
		}
	}

}
