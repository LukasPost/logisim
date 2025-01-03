// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.plexers
{

	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
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
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	public class PriorityEncoder : InstanceFactory
	{
		private const int OUT = 0;
		private const int EN_IN = 1;
		private const int EN_OUT = 2;
		private const int GS = 3;

		public PriorityEncoder() : base("Priority Encoder", Strings.getter("priorityEncoderComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, Plexers.ATTR_SELECT, Plexers.ATTR_DISABLED}, new object[] {Direction.East, BitWidth.create(3), Plexers.DISABLED_FLOATING});
			KeyConfigurator = new BitWidthConfigurator(Plexers.ATTR_SELECT, 1, 5, 0);
			IconName = "priencod.gif";
			FacingAttribute = StdAttr.FACING;
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction dir = attrs.getValue(StdAttr.FACING);
			BitWidth select = attrs.getValue(Plexers.ATTR_SELECT);
			int inputs = 1 << select.Width;
			int offs = -5 * inputs;
			int len = 10 * inputs + 10;
			if (dir == Direction.North)
			{
				return Bounds.create(offs, 0, len, 40);
			}
			else if (dir == Direction.South)
			{
				return Bounds.create(offs, -40, len, 40);
			}
			else if (dir == Direction.West)
			{
				return Bounds.create(0, offs, 40, len);
			}
			else
			{ // dir == Direction.EAST
				return Bounds.create(-40, offs, 40, len);
			}
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			updatePorts(instance);
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == StdAttr.FACING || attr == Plexers.ATTR_SELECT)
			{
				instance.recomputeBounds();
				updatePorts(instance);
			}
			else if (attr == Plexers.ATTR_SELECT)
			{
				updatePorts(instance);
			}
			else if (attr == Plexers.ATTR_DISABLED)
			{
				instance.fireInvalidated();
			}
		}

		private void updatePorts(Instance instance)
		{
			object dir = instance.getAttributeValue(StdAttr.FACING);
			BitWidth select = instance.getAttributeValue(Plexers.ATTR_SELECT);
			int n = 1 << select.Width;
			Port[] ps = new Port[n + 4];
			if (dir == Direction.North || dir == Direction.South)
			{
				int x = -5 * n + 10;
				int y = dir == Direction.North ? 40 : -40;
				for (int i = 0; i < n; i++)
				{
					ps[i] = new Port(x + 10 * i, y, Port.INPUT, 1);
				}
				ps[n + OUT] = new Port(0, 0, Port.OUTPUT, select.Width);
				ps[n + EN_IN] = new Port(x + 10 * n, y / 2, Port.INPUT, 1);
				ps[n + EN_OUT] = new Port(x - 10, y / 2, Port.OUTPUT, 1);
				ps[n + GS] = new Port(10, 0, Port.OUTPUT, 1);
			}
			else
			{
				int x = dir == Direction.East ? -40 : 40;
				int y = -5 * n + 10;
				for (int i = 0; i < n; i++)
				{
					ps[i] = new Port(x, y + 10 * i, Port.INPUT, 1);
				}
				ps[n + OUT] = new Port(0, 0, Port.OUTPUT, select.Width);
				ps[n + EN_IN] = new Port(x / 2, y + 10 * n, Port.INPUT, 1);
				ps[n + EN_OUT] = new Port(x / 2, y - 10, Port.OUTPUT, 1);
				ps[n + GS] = new Port(0, 10, Port.OUTPUT, 1);
			}

			for (int i = 0; i < n; i++)
			{
				ps[i].ToolTip = Strings.getter("priorityEncoderInTip", "" + i);
			}
			ps[n + OUT].ToolTip = Strings.getter("priorityEncoderOutTip");
			ps[n + EN_IN].ToolTip = Strings.getter("priorityEncoderEnableInTip");
			ps[n + EN_OUT].ToolTip = Strings.getter("priorityEncoderEnableOutTip");
			ps[n + GS].ToolTip = Strings.getter("priorityEncoderGroupSignalTip");

			instance.Ports = ps;
		}

		public override void propagate(InstanceState state)
		{
			BitWidth select = state.getAttributeValue(Plexers.ATTR_SELECT);
			int n = 1 << select.Width;
			bool enabled = state.getPort(n + EN_IN) != Value.FALSE;

			int @out = -1;
			Value outDefault;
			if (enabled)
			{
				outDefault = Value.createUnknown(select);
				for (int i = n - 1; i >= 0; i--)
				{
					if (state.getPort(i) == Value.TRUE)
					{
						@out = i;
						break;
					}
				}
			}
			else
			{
				object opt = state.getAttributeValue(Plexers.ATTR_DISABLED);
				Value @base = opt == Plexers.DISABLED_ZERO ? Value.FALSE : Value.UNKNOWN;
				outDefault = Value.repeat(@base, select.Width);
			}
			if (@out < 0)
			{
				state.setPort(n + OUT, outDefault, Plexers.DELAY);
				state.setPort(n + EN_OUT, enabled ? Value.TRUE : Value.FALSE, Plexers.DELAY);
				state.setPort(n + GS, Value.FALSE, Plexers.DELAY);
			}
			else
			{
				state.setPort(n + OUT, Value.createKnown(select, @out), Plexers.DELAY);
				state.setPort(n + EN_OUT, Value.FALSE, Plexers.DELAY);
				state.setPort(n + GS, Value.TRUE, Plexers.DELAY);
			}
		}

		public override void paintInstance(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			Direction facing = painter.getAttributeValue(StdAttr.FACING);

			painter.drawBounds();
			Bounds bds = painter.Bounds;
			g.setColor(Color.Gray);
			int x0;
			int y0;
			int halign;
			if (facing == Direction.West)
			{
				x0 = bds.X + bds.Width - 3;
				y0 = bds.Y + 15;
				halign = JGraphicsUtil.H_RIGHT;
			}
			else if (facing == Direction.North)
			{
				x0 = bds.X + 10;
				y0 = bds.Y + bds.Height - 2;
				halign = JGraphicsUtil.H_CENTER;
			}
			else if (facing == Direction.South)
			{
				x0 = bds.X + 10;
				y0 = bds.Y + 12;
				halign = JGraphicsUtil.H_CENTER;
			}
			else
			{
				x0 = bds.X + 3;
				y0 = bds.Y + 15;
				halign = JGraphicsUtil.H_LEFT;
			}
			JGraphicsUtil.drawText(g, "0", x0, y0, halign, JGraphicsUtil.V_BASELINE);
			g.setColor(Color.Black);
			JGraphicsUtil.drawCenteredText(g, "Pri", bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
			painter.drawPorts();
		}
	}

}
