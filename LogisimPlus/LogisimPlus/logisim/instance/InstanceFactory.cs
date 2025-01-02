// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{

	using LogisimVersion = logisim.LogisimVersion;
	using CircuitState = logisim.circuit.CircuitState;
	using AbstractComponentFactory = logisim.comp.AbstractComponentFactory;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using AttributeSets = logisim.data.AttributeSets;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Loggable = logisim.gui.log.Loggable;
	using Pokable = logisim.tools.Pokable;
	using KeyConfigurator = logisim.tools.key.KeyConfigurator;
	using Icons = logisim.util.Icons;
	using StringGetter = logisim.util.StringGetter;
	using StringUtil = logisim.util.StringUtil;
	using logisim.util;

	/// <summary>
	/// Represents a category of components that appear in a circuit. This class and <code>Component</code> share the same
	/// sort of relationship as the relation between <em>classes</em> and <em>instances</em> in Java. Normally, there is only
	/// one ComponentFactory created for any particular category.
	/// </summary>
	public abstract class InstanceFactory : AbstractComponentFactory
	{
		private string name;
		private StringGetter displayName;
		private StringGetter defaultToolTip;
		private string iconName;
		private Icon icon;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private logisim.data.Attribute<?>[] attrs;
		private Attribute<object>[] attrs;
		private object[] defaults;
		private AttributeSet defaultSet;
		private Bounds bounds;
		private IList<Port> portList;
		private Attribute<Direction> facingAttribute;
		private bool? shouldSnap;
		private KeyConfigurator keyConfigurator;
		private Type pokerClass;
		private Type loggerClass;

		public InstanceFactory(string name) : this(name, StringUtil.constantGetter(name))
		{
		}

		public InstanceFactory(string name, StringGetter displayName)
		{
			this.name = name;
			this.displayName = displayName;
			this.iconName = null;
			this.icon = null;
			this.attrs = null;
			this.defaults = null;
			this.bounds = Bounds.EMPTY_BOUNDS;
			this.portList = Collections.emptyList();
			this.keyConfigurator = null;
			this.facingAttribute = null;
			this.shouldSnap = true;
		}

		public override string Name
		{
			get
			{
				return name;
			}
		}

		public override string DisplayName
		{
			get
			{
				return DisplayGetter.get();
			}
		}

		public override StringGetter DisplayGetter
		{
			get
			{
				return displayName;
			}
		}

		public virtual string IconName
		{
			set
			{
				iconName = value;
				icon = null;
			}
		}

		public virtual Icon Icon
		{
			set
			{
				iconName = "";
				icon = value;
			}
		}

		public override sealed void paintIcon(ComponentDrawContext context, int x, int y, AttributeSet attrs)
		{
			InstancePainter painter = context.InstancePainter;
			painter.setFactory(this, attrs);
			Graphics g = painter.Graphics;
			g.translate(x, y);
			paintIcon(painter);
			g.translate(-x, -y);

			if (painter.Factory == null)
			{
				Icon i = icon;
				if (i == null)
				{
					string n = iconName;
					if (!string.ReferenceEquals(n, null))
					{
						i = Icons.getIcon(n);
						if (i == null)
						{
							n = null;
						}
					}
				}
				if (i != null)
				{
					i.paintIcon(context.Destination, g, x + 2, y + 2);
				}
				else
				{
					base.paintIcon(context, x, y, attrs);
				}
			}
		}

		public override sealed Component createComponent(Location loc, AttributeSet attrs)
		{
			InstanceComponent ret = new InstanceComponent(this, loc, attrs);
			configureNewInstance(ret.Instance);
			return ret;
		}

		public virtual Bounds OffsetBounds
		{
			set
			{
				bounds = value;
			}
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Bounds ret = bounds;
			if (ret == null)
			{
				throw new Exception("offset bounds unknown: " + "use setOffsetBounds or override getOffsetBounds");
			}
			return ret;
		}

		public virtual bool contains(Location loc, AttributeSet attrs)
		{
			Bounds bds = getOffsetBounds(attrs);
			if (bds == null)
			{
				return false;
			}
			return bds.contains(loc, 1);
		}

		public virtual Attribute<Direction> FacingAttribute
		{
			get
			{
				return facingAttribute;
			}
			set
			{
				facingAttribute = value;
			}
		}


		public virtual KeyConfigurator KeyConfigurator
		{
			get
			{
				return keyConfigurator;
			}
			set
			{
				keyConfigurator = value;
			}
		}


		public virtual void setAttributes<T1>(Attribute<T1>[] attrs, object[] defaults)
		{
			this.attrs = attrs;
			this.defaults = defaults;
		}

		public override AttributeSet createAttributeSet()
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] as = attrs;
			Attribute<object>[] @as = attrs;
			AttributeSet ret = @as == null ? AttributeSets.EMPTY : AttributeSets.fixedSet(@as, defaults);
			return ret;
		}

		public virtual object getDefaultAttributeValue<T1>(Attribute<T1> attr, LogisimVersion ver)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] as = attrs;
			Attribute<object>[] @as = attrs;
			if (@as != null)
			{
				for (int i = 0; i < @as.Length; i++)
				{
					if (@as[i] == attr)
					{
						return defaults[i];
					}
				}
				return null;
			}
			else
			{
				AttributeSet dfltSet = defaultSet;
				if (dfltSet == null)
				{
					dfltSet = createAttributeSet();
					defaultSet = dfltSet;
				}
				return dfltSet.getValue(attr);
			}
		}

		public virtual void setPorts(Port[] ports)
		{
			portList = new UnmodifiableList<Port>(ports);
		}

		public virtual void setPorts(IList<Port> ports)
		{
			portList = ports.AsReadOnly();
		}

		public virtual IList<Port> Ports
		{
			get
			{
				return portList;
			}
		}

		public virtual StringGetter DefaultToolTip
		{
			set
			{
				defaultToolTip = value;
			}
			get
			{
				return defaultToolTip;
			}
		}


		public virtual Type InstancePoker
		{
			set
			{
				if (isClassOk(value, typeof(InstancePoker)))
				{
					this.pokerClass = value;
				}
			}
		}

		public virtual Type InstanceLogger
		{
			set
			{
				if (isClassOk(value, typeof(InstanceLogger)))
				{
					this.loggerClass = value;
				}
			}
		}

		public virtual bool ShouldSnap
		{
			set
			{
				shouldSnap = Convert.ToBoolean(value);
			}
		}

		private bool isClassOk(Type sub, Type sup)
		{
			bool isSub = sup.IsAssignableFrom(sub);
			if (!isSub)
			{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.Error.WriteLine(sub.FullName + " must be a subclass of " + sup.FullName); // OK
				return false;
			}
			try
			{
				sub.GetConstructor(new Type[0]);
				return true;
			}
			catch (SecurityException)
			{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.Error.WriteLine(sub.FullName + " needs its no-args constructor to be public"); // OK
			}
			catch (NoSuchMethodException)
			{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.Error.WriteLine(sub.FullName + " is missing a no-arguments constructor"); // OK
			}
			return true;
		}

		public override sealed object getFeature(object key, AttributeSet attrs)
		{
			if (key == FACING_ATTRIBUTE_KEY)
			{
				return facingAttribute;
			}
			if (key == typeof(KeyConfigurator))
			{
				return keyConfigurator;
			}
			if (key == SHOULD_SNAP)
			{
				return shouldSnap;
			}
			return base.getFeature(key, attrs);
		}

		public override sealed void drawGhost(ComponentDrawContext context, Color color, int x, int y, AttributeSet attrs)
		{
			InstancePainter painter = context.InstancePainter;
			Graphics g = painter.Graphics;
			g.setColor(color);
			g.translate(x, y);
			painter.setFactory(this, attrs);
			paintGhost(painter);
			g.translate(-x, -y);
			if (painter.Factory == null)
			{
				base.drawGhost(context, color, x, y, attrs);
			}
		}

		public virtual void paintIcon(InstancePainter painter)
		{
			painter.setFactory(null, null);
		}

		public virtual void paintGhost(InstancePainter painter)
		{
			painter.setFactory(null, null);
		}

		public abstract void paintInstance(InstancePainter painter);

		public abstract void propagate(InstanceState state);

		// event methods
		protected internal virtual void configureNewInstance(Instance instance)
		{
		}

		protected internal virtual void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
		}

		protected internal virtual object getInstanceFeature(Instance instance, object key)
		{
			if (key == typeof(Pokable) && pokerClass != null)
			{
				return new InstancePokerAdapter(instance.Component, pokerClass);
			}
			else if (key == typeof(Loggable) && loggerClass != null)
			{
				return new InstanceLoggerAdapter(instance.Component, loggerClass);
			}
			else
			{
				return null;
			}
		}

		public virtual InstanceState createInstanceState(CircuitState state, Instance instance)
		{
			return new InstanceStateImpl(state, instance.Component);
		}

		public InstanceState createInstanceState(CircuitState state, Component comp)
		{
			return createInstanceState(state, ((InstanceComponent) comp).Instance);
		}
	}

}
