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
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using Bounds = logisim.data.Bounds;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceData = logisim.instance.InstanceData;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using DurationAttribute = logisim.std.wiring.DurationAttribute;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	// TODO repropagate when rows/cols change

	public class DotMatrix : InstanceFactory
	{
		internal static readonly AttributeOption INPUT_SELECT = new AttributeOption("select", Strings.getter("ioInputSelect"));
		internal static readonly AttributeOption INPUT_COLUMN = new AttributeOption("column", Strings.getter("ioInputColumn"));
		internal static readonly AttributeOption INPUT_ROW = new AttributeOption("row", Strings.getter("ioInputRow"));

		internal static readonly AttributeOption SHAPE_CIRCLE = new AttributeOption("circle", Strings.getter("ioShapeCircle"));
		internal static readonly AttributeOption SHAPE_SQUARE = new AttributeOption("square", Strings.getter("ioShapeSquare"));

		internal static readonly Attribute<AttributeOption> ATTR_INPUT_TYPE = Attributes.forOption("inputtype", Strings.getter("ioMatrixInput"), new AttributeOption[] {INPUT_COLUMN, INPUT_ROW, INPUT_SELECT});
		internal static readonly Attribute<int> ATTR_MATRIX_COLS = Attributes.forIntegerRange("matrixcols", Strings.getter("ioMatrixCols"), 1, Value.MAX_WIDTH);
		internal static readonly Attribute<int> ATTR_MATRIX_ROWS = Attributes.forIntegerRange("matrixrows", Strings.getter("ioMatrixRows"), 1, Value.MAX_WIDTH);
		internal static readonly Attribute<AttributeOption> ATTR_DOT_SHAPE = Attributes.forOption("dotshape", Strings.getter("ioMatrixShape"), new AttributeOption[] {SHAPE_CIRCLE, SHAPE_SQUARE});
		internal static readonly Attribute<int> ATTR_PERSIST = new DurationAttribute("persist", Strings.getter("ioMatrixPersistenceAttr"), 0, int.MaxValue);

		public DotMatrix() : base("DotMatrix", Strings.getter("dotMatrixComponent"))
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: setAttributes(new logisim.data.Attribute<?>[] { ATTR_INPUT_TYPE, ATTR_MATRIX_COLS, ATTR_MATRIX_ROWS, Io.ATTR_ON_COLOR, Io.ATTR_OFF_COLOR, ATTR_PERSIST, ATTR_DOT_SHAPE }, new Object[] { INPUT_COLUMN, System.Convert.ToInt32(5), System.Convert.ToInt32(7), java.awt.Color.GREEN, java.awt.Color.DARK_GRAY, System.Convert.ToInt32(0), SHAPE_SQUARE });
			setAttributes(new Attribute<object>[] {ATTR_INPUT_TYPE, ATTR_MATRIX_COLS, ATTR_MATRIX_ROWS, Io.ATTR_ON_COLOR, Io.ATTR_OFF_COLOR, ATTR_PERSIST, ATTR_DOT_SHAPE}, new object[] {INPUT_COLUMN, Convert.ToInt32(5), Convert.ToInt32(7), Color.GREEN, Color.DARK_GRAY, Convert.ToInt32(0), SHAPE_SQUARE});
			IconName = "dotmat.gif";
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			object input = attrs.getValue(ATTR_INPUT_TYPE);
			int cols = (int)attrs.getValue(ATTR_MATRIX_COLS);
			int rows = (int)attrs.getValue(ATTR_MATRIX_ROWS);
			if (input == INPUT_COLUMN)
			{
				return Bounds.create(-5, -10 * rows, 10 * cols, 10 * rows);
			}
			else if (input == INPUT_ROW)
			{
				return Bounds.create(0, -5, 10 * cols, 10 * rows);
			}
			else
			{ // input == INPUT_SELECT
				if (rows == 1)
				{
					return Bounds.create(0, -5, 10 * cols, 10 * rows);
				}
				else
				{
					return Bounds.create(0, -5 * rows + 5, 10 * cols, 10 * rows);
				}
			}
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			updatePorts(instance);
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == ATTR_MATRIX_ROWS || attr == ATTR_MATRIX_COLS || attr == ATTR_INPUT_TYPE)
			{
				instance.recomputeBounds();
				updatePorts(instance);
			}
		}

		private void updatePorts(Instance instance)
		{
			object input = instance.getAttributeValue(ATTR_INPUT_TYPE);
			int rows = (int)instance.getAttributeValue(ATTR_MATRIX_ROWS);
			int cols = (int)instance.getAttributeValue(ATTR_MATRIX_COLS);
			Port[] ps;
			if (input == INPUT_COLUMN)
			{
				ps = new Port[cols];
				for (int i = 0; i < cols; i++)
				{
					ps[i] = new Port(10 * i, 0, Port.INPUT, rows);
				}
			}
			else if (input == INPUT_ROW)
			{
				ps = new Port[rows];
				for (int i = 0; i < rows; i++)
				{
					ps[i] = new Port(0, 10 * i, Port.INPUT, cols);
				}
			}
			else
			{
				if (rows <= 1)
				{
					ps = new Port[] {new Port(0, 0, Port.INPUT, cols)};
				}
				else if (cols <= 1)
				{
					ps = new Port[] {new Port(0, 0, Port.INPUT, rows)};
				}
				else
				{
					ps = new Port[]
					{
						new Port(0, 0, Port.INPUT, cols),
						new Port(0, 10, Port.INPUT, rows)
					};
				}
			}
			instance.setPorts(ps);
		}

		private State getState(InstanceState state)
		{
			int rows = (int)state.getAttributeValue(ATTR_MATRIX_ROWS);
			int cols = (int)state.getAttributeValue(ATTR_MATRIX_COLS);
			long clock = state.TickCount;

			State data = (State) state.Data;
			if (data == null)
			{
				data = new State(rows, cols, clock);
				state.Data = data;
			}
			else
			{
				data.updateSize(rows, cols, clock);
			}
			return data;
		}

		public override void propagate(InstanceState state)
		{
			object type = state.getAttributeValue(ATTR_INPUT_TYPE);
			int rows = (int)state.getAttributeValue(ATTR_MATRIX_ROWS);
			int cols = (int)state.getAttributeValue(ATTR_MATRIX_COLS);
			long clock = state.TickCount;
			long persist = clock + (int)state.getAttributeValue(ATTR_PERSIST);

			State data = getState(state);
			if (type == INPUT_ROW)
			{
				for (int i = 0; i < rows; i++)
				{
					data.setRow(i, state.getPort(i), persist);
				}
			}
			else if (type == INPUT_COLUMN)
			{
				for (int i = 0; i < cols; i++)
				{
					data.setColumn(i, state.getPort(i), persist);
				}
			}
			else if (type == INPUT_SELECT)
			{
				data.setSelect(state.getPort(1), state.getPort(0), persist);
			}
			else
			{
				throw new Exception("unexpected matrix type");
			}
		}

		public override void paintInstance(InstancePainter painter)
		{
			Color onColor = painter.getAttributeValue(Io.ATTR_ON_COLOR);
			Color offColor = painter.getAttributeValue(Io.ATTR_OFF_COLOR);
			bool drawSquare = painter.getAttributeValue(ATTR_DOT_SHAPE) == SHAPE_SQUARE;

			State data = getState(painter);
			long ticks = painter.TickCount;
			Bounds bds = painter.Bounds;
			bool showState = painter.ShowState;
			Graphics g = painter.Graphics;
			int rows = data.rows;
			int cols = data.cols;
			for (int j = 0; j < rows; j++)
			{
				for (int i = 0; i < cols; i++)
				{
					int x = bds.X + 10 * i;
					int y = bds.Y + 10 * j;
					if (showState)
					{
						Value val = data.get(j, i, ticks);
						Color c;
						if (val == Value.TRUE)
						{
							c = onColor;
						}
						else if (val == Value.FALSE)
						{
							c = offColor;
						}
						else
						{
							c = Value.ERROR_COLOR;
						}
						g.setColor(c);

						if (drawSquare)
						{
							g.fillRect(x, y, 10, 10);
						}
						else
						{
							g.fillOval(x + 1, y + 1, 8, 8);
						}
					}
					else
					{
						g.setColor(Color.GRAY);
						g.fillOval(x + 1, y + 1, 8, 8);
					}
				}
			}
			g.setColor(Color.BLACK);
			GraphicsUtil.switchToWidth(g, 2);
			g.drawRect(bds.X, bds.Y, bds.Width, bds.Height);
			GraphicsUtil.switchToWidth(g, 1);
			painter.drawPorts();
		}

		private class State : InstanceData, ICloneable
		{
			internal int rows;
			internal int cols;
			internal Value[] grid;
			internal long[] persistTo;

			public State(int rows, int cols, long curClock)
			{
				this.rows = -1;
				this.cols = -1;
				updateSize(rows, cols, curClock);
			}

			public virtual object clone()
			{
				try
				{
					State ret = (State) base.clone();
					ret.grid = (Value[])this.grid.Clone();
					ret.persistTo = (long[])this.persistTo.Clone();
					return ret;
				}
				catch (CloneNotSupportedException)
				{
					return null;
				}
			}

			internal virtual void updateSize(int rows, int cols, long curClock)
			{
				if (this.rows != rows || this.cols != cols)
				{
					this.rows = rows;
					this.cols = cols;
					int length = rows * cols;
					grid = new Value[length];
					persistTo = new long[length];
					Arrays.Fill(grid, Value.UNKNOWN);
					Arrays.Fill(persistTo, curClock - 1);
				}
			}

			internal virtual Value get(int row, int col, long curTick)
			{
				int index = row * cols + col;
				Value ret = grid[index];
				if (ret == Value.FALSE && persistTo[index] - curTick >= 0)
				{
					ret = Value.TRUE;
				}
				return ret;
			}

			internal virtual void setRow(int index, Value rowVector, long persist)
			{
				int gridloc = (index + 1) * cols - 1;
				int stride = -1;
				Value[] vals = rowVector.All;
				for (int i = 0; i < vals.Length; i++, gridloc += stride)
				{
					Value val = vals[i];
					if (grid[gridloc] == Value.TRUE)
					{
						persistTo[gridloc] = persist - 1;
					}
					grid[gridloc] = vals[i];
					if (val == Value.TRUE)
					{
						persistTo[gridloc] = persist;
					}
				}
			}

			internal virtual void setColumn(int index, Value colVector, long persist)
			{
				int gridloc = (rows - 1) * cols + index;
				int stride = -cols;
				Value[] vals = colVector.All;
				for (int i = 0; i < vals.Length; i++, gridloc += stride)
				{
					Value val = vals[i];
					if (grid[gridloc] == Value.TRUE)
					{
						persistTo[gridloc] = persist - 1;
					}
					grid[gridloc] = val;
					if (val == Value.TRUE)
					{
						persistTo[gridloc] = persist;
					}
				}
			}

			internal virtual void setSelect(Value rowVector, Value colVector, long persist)
			{
				Value[] rowVals = rowVector.All;
				Value[] colVals = colVector.All;
				int gridloc = 0;
				for (int i = rowVals.Length - 1; i >= 0; i--)
				{
					Value wholeRow = rowVals[i];
					if (wholeRow == Value.TRUE)
					{
						for (int j = colVals.Length - 1; j >= 0; j--, gridloc++)
						{
							Value val = colVals[colVals.Length - 1 - j];
							if (grid[gridloc] == Value.TRUE)
							{
								persistTo[gridloc] = persist - 1;
							}
							grid[gridloc] = val;
							if (val == Value.TRUE)
							{
								persistTo[gridloc] = persist;
							}
						}
					}
					else
					{
						if (wholeRow != Value.FALSE)
						{
							wholeRow = Value.ERROR;
						}
						for (int j = colVals.Length - 1; j >= 0; j--, gridloc++)
						{
							if (grid[gridloc] == Value.TRUE)
							{
								persistTo[gridloc] = persist - 1;
							}
							grid[gridloc] = wholeRow;
						}
					}
				}
			}
		}
	}

}
