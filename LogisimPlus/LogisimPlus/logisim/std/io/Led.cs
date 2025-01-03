// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceDataSingleton = logisim.instance.InstanceDataSingleton;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstanceLogger = logisim.instance.InstanceLogger;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
    using LogisimPlus.Java;

    public class Led : InstanceFactory
	{
		public Led() : base("LED", Strings.getter("ledComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, Io.ATTR_ON_COLOR, Io.ATTR_OFF_COLOR, Io.ATTR_ACTIVE, StdAttr.LABEL, Io.ATTR_LABEL_LOC, StdAttr.LABEL_FONT, Io.ATTR_LABEL_COLOR}, new object[] {Direction.West, Color.FromArgb(255, 240, 0, 0), Color.DARK_GRAY, true, "", Io.LABEL_CENTER, StdAttr.DEFAULT_LABEL_FONT, Color.Black});
			FacingAttribute = StdAttr.FACING;
			IconName = "led.gif";
			setPorts(new Port[] {new Port(0, 0, Port.INPUT, 1)});
			InstanceLogger = typeof(Logger);
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			return Bounds.create(0, -10, 20, 20).rotate(Direction.West, facing, 0, 0);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			computeTextField(instance);
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == StdAttr.FACING)
			{
				instance.recomputeBounds();
				computeTextField(instance);
			}
			else if (attr == Io.ATTR_LABEL_LOC)
			{
				computeTextField(instance);
			}
		}

		private void computeTextField(Instance instance)
		{
			Direction facing = instance.getAttributeValue(StdAttr.FACING);
			object labelLoc = instance.getAttributeValue(Io.ATTR_LABEL_LOC);

			Bounds bds = instance.Bounds;
			int x = bds.X + bds.Width / 2;
			int y = bds.Y + bds.Height / 2;
			int halign = JGraphicsUtil.H_CENTER;
			int valign = JGraphicsUtil.V_CENTER;
			if (labelLoc == Direction.North)
			{
				y = bds.Y - 2;
				valign = JGraphicsUtil.V_BOTTOM;
			}
			else if (labelLoc == Direction.South)
			{
				y = bds.Y + bds.Height + 2;
				valign = JGraphicsUtil.V_TOP;
			}
			else if (labelLoc == Direction.East)
			{
				x = bds.X + bds.Width + 2;
				halign = JGraphicsUtil.H_LEFT;
			}
			else if (labelLoc == Direction.West)
			{
				x = bds.X - 2;
				halign = JGraphicsUtil.H_RIGHT;
			}
			if (labelLoc == facing)
			{
				if (labelLoc == Direction.North || labelLoc == Direction.South)
				{
					x += 2;
					halign = JGraphicsUtil.H_LEFT;
				}
				else
				{
					y -= 2;
					valign = JGraphicsUtil.V_BOTTOM;
				}
			}

			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, x, y, halign, valign);
		}

		public override void propagate(InstanceState state)
		{
			Value val = state.getPort(0);
			InstanceDataSingleton data = (InstanceDataSingleton) state.Data;
			if (data == null)
			{
				state.Data = new InstanceDataSingleton(val);
			}
			else
			{
				data.Value = val;
			}
		}

		public override void paintGhost(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			Bounds bds = painter.Bounds;
			JGraphicsUtil.switchToWidth(g, 2);
			g.drawOval(bds.X + 1, bds.Y + 1, bds.Width - 2, bds.Height - 2);
		}

		public override void paintInstance(InstancePainter painter)
		{
			InstanceDataSingleton data = (InstanceDataSingleton)painter.Data;
			Value val = data == null ? Value.FALSE : (Value)data.Value;
			Bounds bds = painter.Bounds.expand(-1);

			JGraphics g = painter.Graphics;
			if (painter.ShowState)
			{
				Color onColor = (Color)painter.getAttributeValue(Io.ATTR_ON_COLOR);
				Color offColor = (Color)painter.getAttributeValue(Io.ATTR_OFF_COLOR);
				bool? activ = (bool?)painter.getAttributeValue(Io.ATTR_ACTIVE);
				object desired = activ.Value ? Value.TRUE : Value.FALSE;
				g.setColor(val == desired ? onColor : offColor);
				g.fillOval(bds.X, bds.Y, bds.Width, bds.Height);
			}
			g.setColor(Color.Black);
			JGraphicsUtil.switchToWidth(g, 2);
			g.drawOval(bds.X, bds.Y, bds.Width, bds.Height);
			JGraphicsUtil.switchToWidth(g, 1);
			g.setColor((Color)painter.getAttributeValue(Io.ATTR_LABEL_COLOR));
			painter.drawLabel();
			painter.drawPorts();
		}

		public class Logger : InstanceLogger
		{
			public override string getLogName(InstanceState state, object option)
			{
				return (string)state.getAttributeValue(StdAttr.LABEL);
			}

			public override Value getLogValue(InstanceState state, object option)
			{
				InstanceDataSingleton data = (InstanceDataSingleton) state.Data;
				if (data == null)
				{
					return Value.FALSE;
				}
				return data.Value == Value.TRUE ? Value.TRUE : Value.FALSE;
			}
		}
	}

}
