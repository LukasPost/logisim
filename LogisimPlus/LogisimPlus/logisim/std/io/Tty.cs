// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
    using LogisimPlus.Java;

    public class Tty : InstanceFactory
	{
		private const int CLR = 0;
		private const int CK = 1;
		private const int WE = 2;
		private const int IN = 3;

		private const int BORDER = 5;
		private const int ROW_HEIGHT = 15;
		private const int COL_WIDTH = 7;
		private static readonly Color DEFAULT_BACKGROUND = Color.FromArgb(64, 0, 0, 0);

		private static readonly Font DEFAULT_FONT = new Font("monospaced", Font.PLAIN, 12);

		private static readonly Attribute ATTR_COLUMNS = Attributes.forIntegerRange("cols", Strings.getter("ttyColsAttr"), 1, 120);
		private static readonly Attribute ATTR_ROWS = Attributes.forIntegerRange("rows", Strings.getter("ttyRowsAttr"), 1, 48);

		public Tty() : base("TTY", Strings.getter("ttyComponent"))
		{
			setAttributes(new Attribute[] {ATTR_ROWS, ATTR_COLUMNS, StdAttr.EDGE_TRIGGER, Io.ATTR_COLOR, Io.ATTR_BACKGROUND}, new object[] {Convert.ToInt32(8), Convert.ToInt32(32), StdAttr.TRIG_RISING, Color.Black, DEFAULT_BACKGROUND});
			IconName = "tty.gif";

			Port[] ps = new Port[4];
			ps[CLR] = new Port(20, 10, Port.INPUT, 1);
			ps[CK] = new Port(0, 0, Port.INPUT, 1);
			ps[WE] = new Port(10, 10, Port.INPUT, 1);
			ps[IN] = new Port(0, -10, Port.INPUT, 7);
			ps[CLR].ToolTip = Strings.getter("ttyClearTip");
			ps[CK].ToolTip = Strings.getter("ttyClockTip");
			ps[WE].ToolTip = Strings.getter("ttyEnableTip");
			ps[IN].ToolTip = Strings.getter("ttyInputTip");
			setPorts(ps);
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			int rows = getRowCount(attrs.getValue(ATTR_ROWS));
			int cols = getColumnCount(attrs.getValue(ATTR_COLUMNS));
			int width = 2 * BORDER + cols * COL_WIDTH;
			int height = 2 * BORDER + rows * ROW_HEIGHT;
			if (width < 30)
			{
				width = 30;
			}
			if (height < 30)
			{
				height = 30;
			}
			return Bounds.create(0, 10 - height, width, height);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == ATTR_ROWS || attr == ATTR_COLUMNS)
			{
				instance.recomputeBounds();
			}
		}

		public override void propagate(InstanceState circState)
		{
			object trigger = circState.getAttributeValue(StdAttr.EDGE_TRIGGER);
			TtyState state = getTtyState(circState);
			Value clear = circState.getPort(CLR);
			Value clock = circState.getPort(CK);
			Value enable = circState.getPort(WE);
			Value @in = circState.getPort(IN);

			lock (state)
			{
				Value lastClock = state.setLastClock(clock);
				if (clear == Value.TRUE)
				{
					state.clear();
				}
				else if (enable != Value.FALSE)
				{
					bool go;
					if (trigger == StdAttr.TRIG_FALLING)
					{
						go = lastClock == Value.TRUE && clock == Value.FALSE;
					}
					else
					{
						go = lastClock == Value.FALSE && clock == Value.TRUE;
					}
					if (go)
					{
						state.add(@in.FullyDefined ? (char) @in.toIntValue() : '?');
					}
				}
			}
		}

		public override void paintGhost(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			JGraphicsUtil.switchToWidth(g, 2);
			Bounds bds = painter.Bounds;
			g.drawRoundRect(bds.X, bds.Y, bds.Width, bds.Height, 10, 10);
		}

		public override void paintInstance(InstancePainter painter)
		{
			bool showState = painter.ShowState;
			JGraphics g = painter.Graphics;
			Bounds bds = painter.Bounds;
			painter.drawClock(CK, Direction.East);
			if (painter.shouldDrawColor())
			{
				g.setColor((Color)painter.getAttributeValue(Io.ATTR_BACKGROUND));
				g.fillRoundRect(bds.X, bds.Y, bds.Width, bds.Height, 10, 10);
			}
			JGraphicsUtil.switchToWidth(g, 2);
			g.setColor(Color.Black);
			g.drawRoundRect(bds.X, bds.Y, bds.Width, bds.Height, 2 * BORDER, 2 * BORDER);
			JGraphicsUtil.switchToWidth(g, 1);
			painter.drawPort(CLR);
			painter.drawPort(WE);
			painter.drawPort(IN);

			int rows = getRowCount(painter.getAttributeValue(ATTR_ROWS));
			int cols = getColumnCount(painter.getAttributeValue(ATTR_COLUMNS));

			if (showState)
			{
				string[] rowData = new string[rows];
				int curRow;
				int curCol;
				TtyState state = getTtyState(painter);
				lock (state)
				{
					for (int i = 0; i < rows; i++)
					{
						rowData[i] = state.getRowString(i);
					}
					curRow = state.CursorRow;
					curCol = state.CursorColumn;
				}

				g.setFont(DEFAULT_FONT);
				g.setColor((Color)painter.getAttributeValue(Io.ATTR_COLOR));
				FontMetrics fm = g.getFontMetrics();
				int x = bds.X + BORDER;
				int y = bds.Y + BORDER + (ROW_HEIGHT + fm.getAscent()) / 2;
				for (int i = 0; i < rows; i++)
				{
					g.drawString(rowData[i], x, y);
					if (i == curRow)
					{
						int x0 = x + fm.stringWidth(rowData[i].Substring(0, curCol));
						g.drawLine(x0, y - fm.getAscent(), x0, y);
					}
					y += ROW_HEIGHT;
				}
			}
			else
			{
				string str = Strings.get("ttyDesc", "" + rows, "" + cols);
				FontMetrics fm = g.getFontMetrics();
				int strWidth = fm.stringWidth(str);
				if (strWidth + BORDER > bds.Width)
				{
					str = Strings.get("ttyDescShort");
					strWidth = fm.stringWidth(str);
				}
				int x = bds.X + (bds.Width - strWidth) / 2;
				int y = bds.Y + (bds.Height + fm.getAscent()) / 2;
				g.drawString(str, x, y);
			}
		}

		private TtyState getTtyState(InstanceState state)
		{
			int rows = getRowCount(state.getAttributeValue(ATTR_ROWS));
			int cols = getColumnCount(state.getAttributeValue(ATTR_COLUMNS));
			TtyState ret = (TtyState) state.Data;
			if (ret == null)
			{
				ret = new TtyState(rows, cols);
				state.Data = ret;
			}
			else
			{
				ret.updateSize(rows, cols);
			}
			return ret;
		}

		public virtual void sendToStdout(InstanceState state)
		{
			TtyState tty = getTtyState(state);
			tty.SendStdout = true;
		}

		private static int getRowCount(object val)
		{
			if (val is int?)
			{
				return ((int?) val).Value;
			}
			else
			{
				return 4;
			}
		}

		private static int getColumnCount(object val)
		{
			if (val is int?)
			{
				return ((int?) val).Value;
			}
			else
			{
				return 16;
			}
		}
	}

}
