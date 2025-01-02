// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using CircuitAppearanceEvent = logisim.circuit.appear.CircuitAppearanceEvent;
	using CircuitAppearanceListener = logisim.circuit.appear.CircuitAppearanceListener;
	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using AttributeSets = logisim.data.AttributeSets;
	using Attributes = logisim.data.Attributes;
	using Direction = logisim.data.Direction;
	using Instance = logisim.instance.Instance;
	using StdAttr = logisim.instance.StdAttr;

	public class CircuitAttributes : AbstractAttributeSet
	{
		public static readonly Attribute<string> NAME_ATTR = Attributes.forString("circuit", Strings.getter("circuitName"));

		public static readonly Attribute<Direction> LABEL_LOCATION_ATTR = Attributes.forDirection("labelloc", Strings.getter("circuitLabelLocAttr"));

		public static readonly Attribute<string> CIRCUIT_LABEL_ATTR = Attributes.forString("clabel", Strings.getter("circuitLabelAttr"));

		public static readonly Attribute<Direction> CIRCUIT_LABEL_FACING_ATTR = Attributes.forDirection("clabelup", Strings.getter("circuitLabelDirAttr"));

		public static readonly Attribute<Font> CIRCUIT_LABEL_FONT_ATTR = Attributes.forFont("clabelfont", Strings.getter("circuitLabelFontAttr"));

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final logisim.data.Attribute<?>[] STATIC_ATTRS = { NAME_ATTR, CIRCUIT_LABEL_ATTR, CIRCUIT_LABEL_FACING_ATTR, CIRCUIT_LABEL_FONT_ATTR};
		private static readonly Attribute<object>[] STATIC_ATTRS = new Attribute<object>[] {NAME_ATTR, CIRCUIT_LABEL_ATTR, CIRCUIT_LABEL_FACING_ATTR, CIRCUIT_LABEL_FONT_ATTR};
		private static readonly object[] STATIC_DEFAULTS = new object[] {"", "", Direction.East, StdAttr.DEFAULT_LABEL_FONT};
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final java.util.List<logisim.data.Attribute<?>> INSTANCE_ATTRS = java.util.Arrays.asList(new logisim.data.Attribute<?>[] { logisim.instance.StdAttr.FACING, logisim.instance.StdAttr.LABEL, LABEL_LOCATION_ATTR, logisim.instance.StdAttr.LABEL_FONT, CircuitAttributes.NAME_ATTR, CIRCUIT_LABEL_ATTR, CIRCUIT_LABEL_FACING_ATTR, CIRCUIT_LABEL_FONT_ATTR});
		private static readonly IList<Attribute<object>> INSTANCE_ATTRS = new List<Attribute<object>> {StdAttr.FACING, StdAttr.LABEL, LABEL_LOCATION_ATTR, StdAttr.LABEL_FONT, CircuitAttributes.NAME_ATTR, CIRCUIT_LABEL_ATTR, CIRCUIT_LABEL_FACING_ATTR, CIRCUIT_LABEL_FONT_ATTR};

		private class StaticListener : AttributeListener
		{
			internal Circuit source;

			internal StaticListener(Circuit s)
			{
				source = s;
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				if (e.Attribute == NAME_ATTR)
				{
					source.fireEvent(CircuitEvent.ACTION_SET_NAME, e.Value);
				}
			}
		}

		private class MyListener : AttributeListener, CircuitAppearanceListener
		{
			private readonly CircuitAttributes outerInstance;

			public MyListener(CircuitAttributes outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<Object> a = (logisim.data.Attribute<Object>) e.getAttribute();
				Attribute<object> a = (Attribute<object>) e.Attribute;
				outerInstance.fireAttributeValueChanged(a, e.Value);
			}

			public virtual void circuitAppearanceChanged(CircuitAppearanceEvent e)
			{
				SubcircuitFactory factory;
				factory = (SubcircuitFactory) outerInstance.subcircInstance.Factory;
				if (e.isConcerning(CircuitAppearanceEvent.PORTS))
				{
					factory.computePorts(outerInstance.subcircInstance);
				}
				if (e.isConcerning(CircuitAppearanceEvent.BOUNDS))
				{
					outerInstance.subcircInstance.recomputeBounds();
				}
				outerInstance.subcircInstance.fireInvalidated();
			}
		}

		internal static AttributeSet createBaseAttrs(Circuit source, string name)
		{
			AttributeSet ret = AttributeSets.fixedSet(STATIC_ATTRS, STATIC_DEFAULTS);
			ret.setValue(CircuitAttributes.NAME_ATTR, name);
			ret.addAttributeListener(new StaticListener(source));
			return ret;
		}

		private Circuit source;
		private Instance subcircInstance;
		private Direction facing;
		private string label;
		private Direction labelLocation;
		private Font labelFont;
		private MyListener listener;
		private Instance[] pinInstances;

		public CircuitAttributes(Circuit source)
		{
			this.source = source;
			subcircInstance = null;
			facing = source.Appearance.Facing;
			label = "";
			labelLocation = Direction.North;
			labelFont = StdAttr.DEFAULT_LABEL_FONT;
			pinInstances = new Instance[0];
		}

		internal virtual Instance Subcircuit
		{
			set
			{
				subcircInstance = value;
				if (subcircInstance != null && listener == null)
				{
					listener = new MyListener(this);
					source.StaticAttributes.addAttributeListener(listener);
					source.Appearance.addCircuitAppearanceListener(listener);
				}
			}
		}

		internal virtual Instance[] PinInstances
		{
			get
			{
				return pinInstances;
			}
			set
			{
				pinInstances = value;
			}
		}


		public virtual Direction Facing
		{
			get
			{
				return facing;
			}
		}

		protected internal override void copyInto(AbstractAttributeSet dest)
		{
			CircuitAttributes other = (CircuitAttributes) dest;
			other.subcircInstance = null;
			other.listener = null;
		}

		public override bool isToSave<T1>(Attribute<T1> attr)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] statics = STATIC_ATTRS;
			Attribute<object>[] statics = STATIC_ATTRS;
			for (int i = 0; i < statics.Length; i++)
			{
				if (statics[i] == attr)
				{
					return false;
				}
			}
			return true;
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return INSTANCE_ATTRS;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <E> E getValue(logisim.data.Attribute<E> attr)
		public override E getValue<E>(Attribute<E> attr)
		{
			if (attr == StdAttr.FACING)
			{
				return (E) facing;
			}
			if (attr == StdAttr.LABEL)
			{
				return (E) label;
			}
			if (attr == StdAttr.LABEL_FONT)
			{
				return (E) labelFont;
			}
			if (attr == LABEL_LOCATION_ATTR)
			{
				return (E) labelLocation;
			}
			else
			{
				return source.StaticAttributes.getValue(attr);
			}
		}

		public override void setValue(Attribute attr, object value)
		{
			if (attr == StdAttr.FACING)
			{
				Direction val = (Direction) value;
				facing = val;
				fireAttributeValueChanged(StdAttr.FACING, val);
				if (subcircInstance != null)
				{
					subcircInstance.recomputeBounds();
				}
			}
			else if (attr == StdAttr.LABEL)
			{
				string val = (string) value;
				label = val;
				fireAttributeValueChanged(StdAttr.LABEL, val);
			}
			else if (attr == StdAttr.LABEL_FONT)
			{
				Font val = (Font) value;
				labelFont = val;
				fireAttributeValueChanged(StdAttr.LABEL_FONT, val);
			}
			else if (attr == LABEL_LOCATION_ATTR)
			{
				Direction val = (Direction) value;
				labelLocation = val;
				fireAttributeValueChanged(LABEL_LOCATION_ATTR, val);
			}
			else
			{
				source.StaticAttributes.setValue(attr, value);
				if (attr == NAME_ATTR)
				{
					source.fireEvent(CircuitEvent.ACTION_SET_NAME, value);
				}
			}
		}
	}

}
