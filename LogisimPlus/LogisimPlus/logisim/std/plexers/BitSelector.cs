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
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
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

	public class BitSelector : InstanceFactory
	{
		public static readonly Attribute<BitWidth> GROUP_ATTR = Attributes.forBitWidth("group", Strings.getter("bitSelectorGroupAttr"));

		public BitSelector() : base("BitSelector", Strings.getter("bitSelectorComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, StdAttr.WIDTH, GROUP_ATTR}, new object[] {Direction.East, BitWidth.create(8), BitWidth.ONE});
			KeyConfigurator = JoinedConfigurator.create(new BitWidthConfigurator(GROUP_ATTR, 1, Value.MAX_WIDTH, 0), new BitWidthConfigurator(StdAttr.WIDTH));

			IconName = "bitSelector.gif";
			FacingAttribute = StdAttr.FACING;
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			Bounds @base = Bounds.create(-30, -15, 30, 30);
			return @base.rotate(Direction.East, facing, 0, 0);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			updatePorts(instance);
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == StdAttr.FACING)
			{
				instance.recomputeBounds();
				updatePorts(instance);
			}
			else if (attr == StdAttr.WIDTH || attr == GROUP_ATTR)
			{
				updatePorts(instance);
			}
		}

		private void updatePorts(Instance instance)
		{
			Direction facing = instance.getAttributeValue(StdAttr.FACING);
			BitWidth data = instance.getAttributeValue(StdAttr.WIDTH);
			BitWidth group = instance.getAttributeValue(GROUP_ATTR);
			int groups = (data.Width + group.Width - 1) / group.Width - 1;
			int selectBits = 1;
			if (groups > 0)
			{
				while (groups != 1)
				{
					groups >>= 1;
					selectBits++;
				}
			}
			BitWidth select = BitWidth.create(selectBits);

			Location inPt;
			Location selPt;
			if (facing == Direction.West)
			{
				inPt = new Location(30, 0);
				selPt = new Location(10, 10);
			}
			else if (facing == Direction.North)
			{
				inPt = new Location(0, 30);
				selPt = new Location(-10, 10);
			}
			else if (facing == Direction.South)
			{
				inPt = new Location(0, -30);
				selPt = new Location(-10, -10);
			}
			else
			{
				inPt = new Location(-30, 0);
				selPt = new Location(-10, 10);
			}

			Port[] ps = new Port[3];
			ps[0] = new Port(0, 0, Port.OUTPUT, group.Width);
			ps[1] = new Port(inPt.X, inPt.Y, Port.INPUT, data.Width);
			ps[2] = new Port(selPt.X, selPt.Y, Port.INPUT, select.Width);
			ps[0].setToolTip(Strings.getter("bitSelectorOutputTip"));
			ps[1].setToolTip(Strings.getter("bitSelectorDataTip"));
			ps[2].setToolTip(Strings.getter("bitSelectorSelectTip"));
			instance.setPorts(ps);
		}

		public override void propagate(InstanceState state)
		{
			Value data = state.getPort(1);
			Value select = state.getPort(2);
			BitWidth groupBits = state.getAttributeValue(GROUP_ATTR);
			Value group;
			if (!select.FullyDefined)
			{
				group = Value.createUnknown(groupBits);
			}
			else
			{
				int shift = select.toIntValue() * groupBits.Width;
				if (shift >= data.Width)
				{
					group = Value.createKnown(groupBits, 0);
				}
				else if (groupBits.Width == 1)
				{
					group = data.get(shift);
				}
				else
				{
					Value[] bits = new Value[groupBits.Width];
					for (int i = 0; i < bits.Length; i++)
					{
						if (shift + i >= data.Width)
						{
							bits[i] = Value.FALSE;
						}
						else
						{
							bits[i] = data.get(shift + i);
						}
					}
					group = Value.create(bits);
				}
			}
			state.setPort(0, group, Plexers.DELAY);
		}

		public override void paintGhost(InstancePainter painter)
		{
			Plexers.drawTrapezoid(painter.Graphics, painter.Bounds, painter.getAttributeValue(StdAttr.FACING), 9);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			Direction facing = painter.getAttributeValue(StdAttr.FACING);

			Plexers.drawTrapezoid(g, painter.Bounds, facing, 9);
			Bounds bds = painter.Bounds;
			g.setColor(Color.BLACK);
			GraphicsUtil.drawCenteredText(g, "Sel", bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
			painter.drawPorts();
		}
	}

}
