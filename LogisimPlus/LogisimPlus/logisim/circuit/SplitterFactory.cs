// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using LogisimVersion = logisim.LogisimVersion;
	using AbstractComponentFactory = logisim.comp.AbstractComponentFactory;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using IntegerConfigurator = logisim.tools.key.IntegerConfigurator;
	using JoinedConfigurator = logisim.tools.key.JoinedConfigurator;
	using KeyConfigurator = logisim.tools.key.KeyConfigurator;
	using ParallelConfigurator = logisim.tools.key.ParallelConfigurator;
	using Icons = logisim.util.Icons;
	using StringGetter = logisim.util.StringGetter;

	public class SplitterFactory : AbstractComponentFactory
	{
		public static readonly SplitterFactory instance = new SplitterFactory();

		private static readonly Icon toolIcon = Icons.getIcon("splitter.gif");

		private SplitterFactory()
		{
		}

		public override string Name
		{
			get
			{
				return "Splitter";
			}
		}

		public override StringGetter DisplayGetter
		{
			get
			{
				return Strings.getter("splitterComponent");
			}
		}

		public override AttributeSet createAttributeSet()
		{
			return new SplitterAttributes();
		}

		public virtual object getDefaultAttributeValue(Attribute attr, LogisimVersion ver)
		{
			if (attr == SplitterAttributes.ATTR_APPEARANCE)
			{
				if (ver.compareTo(LogisimVersion.get(2, 6, 3, 202)) < 0)
				{
					return SplitterAttributes.APPEAR_LEGACY;
				}
				else
				{
					return SplitterAttributes.APPEAR_LEFT;
				}
			}
			else if (attr is SplitterAttributes.BitOutAttribute)
			{
				SplitterAttributes.BitOutAttribute a;
				a = (SplitterAttributes.BitOutAttribute) attr;
				return a.Default;
			}
			else
			{
				return base.getDefaultAttributeValue(attr, ver);
			}
		}

		public override Component createComponent(Location loc, AttributeSet attrs)
		{
			return new Splitter(loc, attrs);
		}

		public override Bounds getOffsetBounds(AttributeSet attrsBase)
		{
			SplitterAttributes attrs = (SplitterAttributes) attrsBase;
			int fanout = attrs.fanout;
			SplitterParameters parms = attrs.Parameters;
			int xEnd0 = parms.End0X;
			int yEnd0 = parms.End0Y;
			Bounds bds = Bounds.create(0, 0, 1, 1);
			bds = bds.add(xEnd0, yEnd0);
			bds = bds.add(xEnd0 + (fanout - 1) * parms.EndToEndDeltaX, yEnd0 + (fanout - 1) * parms.EndToEndDeltaY);
			return bds;
		}

		//
		// user interface methods
		//
		public override void drawGhost(ComponentDrawContext context, Color color, int x, int y, AttributeSet attrsBase)
		{
			SplitterAttributes attrs = (SplitterAttributes) attrsBase;
			context.Graphics.setColor(color);
			Location loc = new Location(x, y);
			if (attrs.appear == SplitterAttributes.APPEAR_LEGACY)
			{
				SplitterPainter.drawLegacy(context, attrs, loc);
			}
			else
			{
				SplitterPainter.drawLines(context, attrs, loc);
			}
		}

		public override void paintIcon(ComponentDrawContext c, int x, int y, AttributeSet attrs)
		{
			JGraphics g = c.Graphics;
			if (toolIcon != null)
			{
				toolIcon.paintIcon(c.Destination, g, x + 2, y + 2);
			}
		}

		public override object getFeature(object key, AttributeSet attrs)
		{
			if (key == FACING_ATTRIBUTE_KEY)
			{
				return StdAttr.FACING;
			}
			else if (key == typeof(KeyConfigurator))
			{
				KeyConfigurator altConfig = ParallelConfigurator.create(new BitWidthConfigurator(SplitterAttributes.ATTR_WIDTH), new IntegerConfigurator(SplitterAttributes.ATTR_FANOUT, 1, 32, InputEvent.ALT_DOWN_MASK));
				return JoinedConfigurator.create(new IntegerConfigurator(SplitterAttributes.ATTR_FANOUT, 1, 32, 0), altConfig);
			}
			return base.getFeature(key, attrs);
		}
	}

}
