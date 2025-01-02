// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using Wire = logisim.circuit.Wire;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceDataSingleton = logisim.instance.InstanceDataSingleton;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstanceLogger = logisim.instance.InstanceLogger;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class Button : InstanceFactory
	{
		private const int DEPTH = 3;

		public Button() : base("Button", Strings.getter("buttonComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, Io.ATTR_COLOR, StdAttr.LABEL, Io.ATTR_LABEL_LOC, StdAttr.LABEL_FONT, Io.ATTR_LABEL_COLOR}, new object[] {Direction.East, Color.WHITE, "", Io.LABEL_CENTER, StdAttr.DEFAULT_LABEL_FONT, Color.BLACK});
			FacingAttribute = StdAttr.FACING;
			IconName = "button.gif";
			setPorts(new Port[] {new Port(0, 0, Port.OUTPUT, 1)});
			InstancePoker = typeof(Poker);
			InstanceLogger = typeof(Logger);
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			return Bounds.create(-20, -10, 20, 20).rotate(Direction.East, facing, 0, 0);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			computeTextField(instance);
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
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
			int halign = GraphicsUtil.H_CENTER;
			int valign = GraphicsUtil.V_CENTER;
			if (labelLoc == Io.LABEL_CENTER)
			{
				x = bds.X + (bds.Width - DEPTH) / 2;
				y = bds.Y + (bds.Height - DEPTH) / 2;
			}
			else if (labelLoc == Direction.North)
			{
				y = bds.Y - 2;
				valign = GraphicsUtil.V_BOTTOM;
			}
			else if (labelLoc == Direction.South)
			{
				y = bds.Y + bds.Height + 2;
				valign = GraphicsUtil.V_TOP;
			}
			else if (labelLoc == Direction.East)
			{
				x = bds.X + bds.Width + 2;
				halign = GraphicsUtil.H_LEFT;
			}
			else if (labelLoc == Direction.West)
			{
				x = bds.X - 2;
				halign = GraphicsUtil.H_RIGHT;
			}
			if (labelLoc == facing)
			{
				if (labelLoc == Direction.North || labelLoc == Direction.South)
				{
					x += 2;
					halign = GraphicsUtil.H_LEFT;
				}
				else
				{
					y -= 2;
					valign = GraphicsUtil.V_BOTTOM;
				}
			}

			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, x, y, halign, valign);
		}

		public override void propagate(InstanceState state)
		{
			InstanceDataSingleton data = (InstanceDataSingleton) state.Data;
			Value val = data == null ? Value.FALSE : (Value) data.Value;
			state.setPort(0, val, 1);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Bounds bds = painter.Bounds;
			int x = bds.X;
			int y = bds.Y;
			int w = bds.Width;
			int h = bds.Height;

			Value val;
			if (painter.ShowState)
			{
				InstanceDataSingleton data = (InstanceDataSingleton) painter.Data;
				val = data == null ? Value.FALSE : (Value) data.Value;
			}
			else
			{
				val = Value.FALSE;
			}

			Color color = painter.getAttributeValue(Io.ATTR_COLOR);
			if (!painter.shouldDrawColor())
			{
				int hue = (color.getRed() + color.getGreen() + color.getBlue()) / 3;
				color = new Color(hue, hue, hue);
			}

			Graphics g = painter.Graphics;
			int depress;
			if (val == Value.TRUE)
			{
				x += DEPTH;
				y += DEPTH;
				object labelLoc = painter.getAttributeValue(Io.ATTR_LABEL_LOC);
				if (labelLoc == Io.LABEL_CENTER || labelLoc == Direction.North || labelLoc == Direction.West)
				{
					depress = DEPTH;
				}
				else
				{
					depress = 0;
				}

				object facing = painter.getAttributeValue(StdAttr.FACING);
				if (facing == Direction.North || facing == Direction.West)
				{
					Location p = painter.Location;
					int px = p.X;
					int py = p.Y;
					GraphicsUtil.switchToWidth(g, Wire.WIDTH);
					g.setColor(Value.TRUE_COLOR);
					if (facing == Direction.North)
					{
						g.drawLine(px, py, px, py + 10);
					}
					else
					{
						g.drawLine(px, py, px + 10, py);
					}
					GraphicsUtil.switchToWidth(g, 1);
				}

				g.setColor(color);
				g.fillRect(x, y, w - DEPTH, h - DEPTH);
				g.setColor(Color.BLACK);
				g.drawRect(x, y, w - DEPTH, h - DEPTH);
			}
			else
			{
				depress = 0;
				int[] xp = new int[] {x, x + w - DEPTH, x + w, x + w, x + DEPTH, x};
				int[] yp = new int[] {y, y, y + DEPTH, y + h, y + h, y + h - DEPTH};
				g.setColor(color.darker());
				g.fillPolygon(xp, yp, xp.Length);
				g.setColor(color);
				g.fillRect(x, y, w - DEPTH, h - DEPTH);
				g.setColor(Color.BLACK);
				g.drawRect(x, y, w - DEPTH, h - DEPTH);
				g.drawLine(x + w - DEPTH, y + h - DEPTH, x + w, y + h);
				g.drawPolygon(xp, yp, xp.Length);
			}

			g.translate(depress, depress);
			g.setColor(painter.getAttributeValue(Io.ATTR_LABEL_COLOR));
			painter.drawLabel();
			g.translate(-depress, -depress);
			painter.drawPorts();
		}

		public class Poker : InstancePoker
		{
			public override void mousePressed(InstanceState state, MouseEvent e)
			{
				setValue(state, Value.TRUE);
			}

			public override void mouseReleased(InstanceState state, MouseEvent e)
			{
				setValue(state, Value.FALSE);
			}

			internal virtual void setValue(InstanceState state, Value val)
			{
				InstanceDataSingleton data = (InstanceDataSingleton) state.Data;
				if (data == null)
				{
					state.Data = new InstanceDataSingleton(val);
				}
				else
				{
					data.Value = val;
				}
				state.Instance.fireInvalidated();
			}
		}

		public class Logger : InstanceLogger
		{
			public override string getLogName(InstanceState state, object option)
			{
				return state.getAttributeValue(StdAttr.LABEL);
			}

			public override Value getLogValue(InstanceState state, object option)
			{
				InstanceDataSingleton data = (InstanceDataSingleton) state.Data;
				return data == null ? Value.FALSE : (Value) data.Value;
			}
		}
	}

}
