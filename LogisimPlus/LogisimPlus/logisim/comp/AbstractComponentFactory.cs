// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{

	using LogisimVersion = logisim.LogisimVersion;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using AttributeSets = logisim.data.AttributeSets;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using Icons = logisim.util.Icons;
	using StringGetter = logisim.util.StringGetter;
	using StringUtil = logisim.util.StringUtil;

	public abstract class AbstractComponentFactory : ComponentFactory
	{
		private static readonly Icon toolIcon = Icons.getIcon("subcirc.gif");

		private AttributeSet defaultSet;

		protected internal AbstractComponentFactory()
		{
			defaultSet = null;
		}

		public override string ToString()
		{
			return Name;
		}

		public abstract string Name {get;}

		public virtual string DisplayName
		{
			get
			{
				return DisplayGetter.get();
			}
		}

		public virtual StringGetter DisplayGetter
		{
			get
			{
				return StringUtil.constantGetter(Name);
			}
		}

		public abstract Component createComponent(Location loc, AttributeSet attrs);

		public abstract Bounds getOffsetBounds(AttributeSet attrs);

		public virtual AttributeSet createAttributeSet()
		{
			return AttributeSets.EMPTY;
		}

		public virtual bool isAllDefaultValues(AttributeSet attrs, LogisimVersion ver)
		{
			return false;
		}

		public virtual object getDefaultAttributeValue<T1>(Attribute<T1> attr, LogisimVersion ver)
		{
			AttributeSet dfltSet = defaultSet;
			if (dfltSet == null)
			{
				dfltSet = (AttributeSet) createAttributeSet().clone();
				defaultSet = dfltSet;
			}
			return dfltSet.getValue(attr);
		}

		//
		// user interface methods
		//
		public virtual void drawGhost(ComponentDrawContext context, Color color, int x, int y, AttributeSet attrs)
		{
			Graphics g = context.Graphics;
			Bounds bds = getOffsetBounds(attrs);
			g.setColor(color);
			GraphicsUtil.switchToWidth(g, 2);
			g.drawRect(x + bds.X, y + bds.Y, bds.Width, bds.Height);
		}

		public virtual void paintIcon(ComponentDrawContext context, int x, int y, AttributeSet attrs)
		{
			Graphics g = context.Graphics;
			if (toolIcon != null)
			{
				toolIcon.paintIcon(context.Destination, g, x + 2, y + 2);
			}
			else
			{
				g.setColor(Color.black);
				g.drawRect(x + 5, y + 2, 11, 17);
				Value[] v = new Value[] {Value.TRUE, Value.FALSE};
				for (int i = 0; i < 3; i++)
				{
					g.setColor(v[i % 2].Color);
					g.fillOval(x + 5 - 1, y + 5 + 5 * i - 1, 3, 3);
					g.setColor(v[(i + 1) % 2].Color);
					g.fillOval(x + 16 - 1, y + 5 + 5 * i - 1, 3, 3);
				}
			}
		}

		public virtual object getFeature(object key, AttributeSet attrs)
		{
			return null;
		}

	}

}
