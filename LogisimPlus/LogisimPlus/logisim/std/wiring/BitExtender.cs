// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{

	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using JoinedConfigurator = logisim.tools.key.JoinedConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class BitExtender : InstanceFactory
	{
		private static readonly Attribute<BitWidth> ATTR_IN_WIDTH = Attributes.forBitWidth("in_width", Strings.getter("extenderInAttr"));
		private static readonly Attribute<BitWidth> ATTR_OUT_WIDTH = Attributes.forBitWidth("out_width", Strings.getter("extenderOutAttr"));
		private static readonly Attribute<AttributeOption> ATTR_TYPE = Attributes.forOption("type", Strings.getter("extenderTypeAttr"), new AttributeOption[]
		{
			new AttributeOption("zero", "zero", Strings.getter("extenderZeroType")),
			new AttributeOption("one", "one", Strings.getter("extenderOneType")),
			new AttributeOption("sign", "sign", Strings.getter("extenderSignType")),
			new AttributeOption("input", "input", Strings.getter("extenderInputType"))
		});

		public static readonly BitExtender FACTORY = new BitExtender();

		public BitExtender() : base("Bit Extender", Strings.getter("extenderComponent"))
		{
			IconName = "extender.gif";
			setAttributes(new Attribute[] {ATTR_IN_WIDTH, ATTR_OUT_WIDTH, ATTR_TYPE}, new object[] {BitWidth.create(8), BitWidth.create(16), ATTR_TYPE.parse("zero")});
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = JoinedConfigurator.create(new BitWidthConfigurator(ATTR_OUT_WIDTH), new BitWidthConfigurator(ATTR_IN_WIDTH, 1, Value.MAX_WIDTH, 0));
			OffsetBounds = Bounds.create(-40, -20, 40, 40);
		}

		//
		// graphics methods
		//
		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			FontMetrics fm = g.getFontMetrics();
			int asc = fm.getAscent();

			painter.drawBounds();

			string s0;
			string type = getType(painter.AttributeSet);
			if (type.Equals("zero"))
			{
				s0 = Strings.get("extenderZeroLabel");
			}
			else if (type.Equals("one"))
			{
				s0 = Strings.get("extenderOneLabel");
			}
			else if (type.Equals("sign"))
			{
				s0 = Strings.get("extenderSignLabel");
			}
			else if (type.Equals("input"))
			{
				s0 = Strings.get("extenderInputLabel");
			}
			else
			{
				s0 = "???"; // should never happen
			}
			string s1 = Strings.get("extenderMainLabel");
			Bounds bds = painter.Bounds;
			int x = bds.X + bds.Width / 2;
			int y0 = bds.Y + (bds.Height / 2 + asc) / 2;
			int y1 = bds.Y + (3 * bds.Height / 2 + asc) / 2;
			GraphicsUtil.drawText(g, s0, x, y0, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
			GraphicsUtil.drawText(g, s1, x, y1, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);

			BitWidth w0 = painter.getAttributeValue(ATTR_OUT_WIDTH);
			BitWidth w1 = painter.getAttributeValue(ATTR_IN_WIDTH);
			painter.drawPort(0, "" + w0.Width, Direction.West);
			painter.drawPort(1, "" + w1.Width, Direction.East);
			if (type.Equals("input"))
			{
				painter.drawPort(2);
			}
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			configurePorts(instance);
			instance.addAttributeListener();
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == ATTR_TYPE)
			{
				configurePorts(instance);
				instance.fireInvalidated();
			}
			else
			{
				instance.fireInvalidated();
			}
		}

		private void configurePorts(Instance instance)
		{
			Port p0 = new Port(0, 0, Port.OUTPUT, ATTR_OUT_WIDTH);
			Port p1 = new Port(-40, 0, Port.INPUT, ATTR_IN_WIDTH);
			string type = getType(instance.AttributeSet);
			if (type.Equals("input"))
			{
				instance.setPorts(new Port[] {p0, p1, new Port(-20, -20, Port.INPUT, 1)});
			}
			else
			{
				instance.setPorts(new Port[] {p0, p1});
			}
		}

		public override void propagate(InstanceState state)
		{
			Value @in = state.getPort(1);
			BitWidth wout = state.getAttributeValue(ATTR_OUT_WIDTH);
			string type = getType(state.AttributeSet);
			Value extend;
			if (type.Equals("one"))
			{
				extend = Value.TRUE;
			}
			else if (type.Equals("sign"))
			{
				int win = @in.Width;
				extend = win > 0 ? @in.get(win - 1) : Value.ERROR;
			}
			else if (type.Equals("input"))
			{
				extend = state.getPort(2);
				if (extend.Width != 1)
				{
					extend = Value.ERROR;
				}
			}
			else
			{
				extend = Value.FALSE;
			}

			Value @out = @in.extendWidth(wout.Width, extend);
			state.setPort(0, @out, 1);
		}

		private string getType(AttributeSet attrs)
		{
			AttributeOption topt = attrs.getValue(ATTR_TYPE);
			return (string) topt.Value;
		}
	}
}
