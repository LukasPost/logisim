// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.wiring
{


	using CircuitState = logisim.circuit.CircuitState;
	using RadixOption = logisim.circuit.RadixOption;
	using EndData = logisim.comp.EndData;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Canvas = logisim.gui.main.Canvas;
	using Instance = logisim.instance.Instance;
	using InstanceData = logisim.instance.InstanceData;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstanceLogger = logisim.instance.InstanceLogger;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using DirectionConfigurator = logisim.tools.key.DirectionConfigurator;
	using JoinedConfigurator = logisim.tools.key.JoinedConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using Icons = logisim.util.Icons;

	public class Pin : InstanceFactory
	{
		public static readonly Attribute<bool> ATTR_TRISTATE = Attributes.forBoolean("tristate", Strings.getter("pinThreeStateAttr"));
		public static readonly Attribute<bool> ATTR_TYPE = Attributes.forBoolean("output", Strings.getter("pinOutputAttr"));
		public static readonly Attribute<Direction> ATTR_LABEL_LOC = Attributes.forDirection("labelloc", Strings.getter("pinLabelLocAttr"));

		public static readonly AttributeOption PULL_NONE = new AttributeOption("none", Strings.getter("pinPullNoneOption"));
		public static readonly AttributeOption PULL_UP = new AttributeOption("up", Strings.getter("pinPullUpOption"));
		public static readonly AttributeOption PULL_DOWN = new AttributeOption("down", Strings.getter("pinPullDownOption"));
		public static readonly Attribute<AttributeOption> ATTR_PULL = Attributes.forOption("pull", Strings.getter("pinPullAttr"), new AttributeOption[] {PULL_NONE, PULL_UP, PULL_DOWN});

		public static readonly Pin FACTORY = new Pin();

		private static readonly Icon ICON_IN = Icons.getIcon("pinInput.gif");
		private static readonly Icon ICON_OUT = Icons.getIcon("pinOutput.gif");
		private static readonly Font ICON_WIDTH_FONT = new Font("SansSerif", Font.BOLD, 9);
		private static readonly Color ICON_WIDTH_COLOR = Value.WIDTH_ERROR_COLOR.darker();

		public Pin() : base("Pin", Strings.getter("pinComponent"))
		{
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = JoinedConfigurator.create(new BitWidthConfigurator(StdAttr.WIDTH), new DirectionConfigurator(ATTR_LABEL_LOC, KeyEvent.ALT_DOWN_MASK));
			InstanceLogger = typeof(PinLogger);
			InstancePoker = typeof(PinPoker);
		}

		public override AttributeSet createAttributeSet()
		{
			return new PinAttributes();
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
			BitWidth width = attrs.getValue(StdAttr.WIDTH);
			return Probe.getOffsetBounds(facing, width, RadixOption.RADIX_2);
		}

		//
		// graphics methods
		//
		public override void paintIcon(InstancePainter painter)
		{
			paintIconBase(painter);
			BitWidth w = painter.getAttributeValue(StdAttr.WIDTH);
			if (!w.Equals(BitWidth.ONE))
			{
				Graphics g = painter.Graphics;
				g.setColor(ICON_WIDTH_COLOR);
				g.setFont(ICON_WIDTH_FONT);
				GraphicsUtil.drawCenteredText(g, "" + w.Width, 10, 9);
				g.setColor(Color.BLACK);
			}
		}

		private void paintIconBase(InstancePainter painter)
		{
			PinAttributes attrs = (PinAttributes) painter.AttributeSet;
			Direction dir = attrs.facing;
			bool output = attrs.Output;
			Graphics g = painter.Graphics;
			if (output)
			{
				if (ICON_OUT != null)
				{
					Icons.paintRotated(g, 2, 2, dir, ICON_OUT, painter.Destination);
					return;
				}
			}
			else
			{
				if (ICON_IN != null)
				{
					Icons.paintRotated(g, 2, 2, dir, ICON_IN, painter.Destination);
					return;
				}
			}
			int pinx = 16;
			int piny = 9;
			if (dir == Direction.East)
			{ // keep defaults
			}
			else if (dir == Direction.West)
			{
				pinx = 4;
			}
			else if (dir == Direction.North)
			{
				pinx = 9;
				piny = 4;
			}
			else if (dir == Direction.South)
			{
				pinx = 9;
				piny = 16;
			}

			g.setColor(Color.black);
			if (output)
			{
				g.drawOval(4, 4, 13, 13);
			}
			else
			{
				g.drawRect(4, 4, 13, 13);
			}
			g.setColor(Value.TRUE.Color);
			g.fillOval(7, 7, 8, 8);
			g.fillOval(pinx, piny, 3, 3);
		}

		public override void paintGhost(InstancePainter painter)
		{
			PinAttributes attrs = (PinAttributes) painter.AttributeSet;
			Location loc = painter.Location;
			Bounds bds = painter.OffsetBounds;
			int x = loc.X;
			int y = loc.Y;
			Graphics g = painter.Graphics;
			GraphicsUtil.switchToWidth(g, 2);
			bool output = attrs.Output;
			if (output)
			{
				BitWidth width = attrs.getValue(StdAttr.WIDTH);
				if (width == BitWidth.ONE)
				{
					g.drawOval(x + bds.X + 1, y + bds.Y + 1, bds.Width - 1, bds.Height - 1);
				}
				else
				{
					g.drawRoundRect(x + bds.X + 1, y + bds.Y + 1, bds.Width - 1, bds.Height - 1, 6, 6);
				}
			}
			else
			{
				g.drawRect(x + bds.X + 1, y + bds.Y + 1, bds.Width - 1, bds.Height - 1);
			}
		}

		public override void paintInstance(InstancePainter painter)
		{
			PinAttributes attrs = (PinAttributes) painter.AttributeSet;
			Graphics g = painter.Graphics;
			Bounds bds = painter.getInstance().Bounds; // intentionally with no graphics object - we don't want label
															// included
			int x = bds.X;
			int y = bds.Y;
			GraphicsUtil.switchToWidth(g, 2);
			g.setColor(Color.black);
			if (attrs.type == EndData.OUTPUT_ONLY)
			{
				if (attrs.width.Width == 1)
				{
					g.drawOval(x + 1, y + 1, bds.Width - 1, bds.Height - 1);
				}
				else
				{
					g.drawRoundRect(x + 1, y + 1, bds.Width - 1, bds.Height - 1, 6, 6);
				}
			}
			else
			{
				g.drawRect(x + 1, y + 1, bds.Width - 1, bds.Height - 1);
			}

			painter.drawLabel();

			if (!painter.ShowState)
			{
				g.setColor(Color.BLACK);
				GraphicsUtil.drawCenteredText(g, "x" + attrs.width.Width, bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
			}
			else
			{
				PinState state = getState(painter);
				if (attrs.width.Width <= 1)
				{
					Value receiving = state.receiving;
					g.setColor(receiving.Color);
					g.fillOval(x + 4, y + 4, 13, 13);

					if (attrs.width.Width == 1)
					{
						g.setColor(Color.WHITE);
						GraphicsUtil.drawCenteredText(g, state.sending.toDisplayString(), x + 11, y + 9);
					}
				}
				else
				{
					Probe.paintValue(painter, state.sending);
				}
			}

			painter.drawPorts();
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			PinAttributes attrs = (PinAttributes) instance.AttributeSet;
			instance.addAttributeListener();
			configurePorts(instance);
			Probe.configureLabel(instance, attrs.labelloc, attrs.facing);
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == ATTR_TYPE)
			{
				configurePorts(instance);
			}
			else if (attr == StdAttr.WIDTH || attr == StdAttr.FACING || attr == Pin.ATTR_LABEL_LOC)
			{
				instance.recomputeBounds();
				PinAttributes attrs = (PinAttributes) instance.AttributeSet;
				Probe.configureLabel(instance, attrs.labelloc, attrs.facing);
			}
		}

		private void configurePorts(Instance instance)
		{
			PinAttributes attrs = (PinAttributes) instance.AttributeSet;
			string endType = attrs.Output ? Port.INPUT : Port.OUTPUT;
			Port port = new Port(0, 0, endType, StdAttr.WIDTH);
			if (attrs.Output)
			{
				port.setToolTip(Strings.getter("pinOutputToolTip"));
			}
			else
			{
				port.setToolTip(Strings.getter("pinInputToolTip"));
			}
			instance.setPorts(new Port[] {port});
		}

		public override void propagate(InstanceState state)
		{
			PinAttributes attrs = (PinAttributes) state.AttributeSet;
			Value val = state.getPort(0);

			PinState q = getState(state);
			if (attrs.type == EndData.OUTPUT_ONLY)
			{
				q.sending = val;
				q.receiving = val;
				state.setPort(0, Value.createUnknown(attrs.width), 1);
			}
			else
			{
				if (!val.FullyDefined && !attrs.threeState && state.CircuitRoot)
				{
					q.sending = pull2(q.sending, attrs.width);
					q.receiving = pull2(val, attrs.width);
					state.setPort(0, q.sending, 1);
				}
				else
				{
					q.receiving = val;
					if (!val.Equals(q.sending))
					{ // ignore if no change
						state.setPort(0, q.sending, 1);
					}
				}
			}
		}

		private static Value pull2(Value mod, BitWidth expectedWidth)
		{
			if (mod.Width == expectedWidth.Width)
			{
				Value[] vs = mod.All;
				for (int i = 0; i < vs.Length; i++)
				{
					if (vs[i] == Value.UNKNOWN)
					{
						vs[i] = Value.FALSE;
					}
				}
				return Value.create(vs);
			}
			else
			{
				return Value.createKnown(expectedWidth, 0);
			}
		}

		//
		// basic information methods
		//
		public virtual BitWidth getWidth(Instance instance)
		{
			PinAttributes attrs = (PinAttributes) instance.AttributeSet;
			return attrs.width;
		}

		public virtual int getType(Instance instance)
		{
			PinAttributes attrs = (PinAttributes) instance.AttributeSet;
			return attrs.type;
		}

		public virtual bool isInputPin(Instance instance)
		{
			PinAttributes attrs = (PinAttributes) instance.AttributeSet;
			return attrs.type != EndData.OUTPUT_ONLY;
		}

		//
		// state information methods
		//
		public virtual Value getValue(InstanceState state)
		{
			return getState(state).sending;
		}

		public virtual void setValue(InstanceState state, Value value)
		{
			PinAttributes attrs = (PinAttributes) state.AttributeSet;
			object pull = attrs.pull;
			if (pull != PULL_NONE && pull != null && !value.FullyDefined)
			{
				Value[] bits = value.All;
				if (pull == PULL_UP)
				{
					for (int i = 0; i < bits.Length; i++)
					{
						if (bits[i] != Value.FALSE)
						{
							bits[i] = Value.TRUE;
						}
					}
				}
				else if (pull == PULL_DOWN)
				{
					for (int i = 0; i < bits.Length; i++)
					{
						if (bits[i] != Value.TRUE)
						{
							bits[i] = Value.FALSE;
						}
					}
				}
				value = Value.create(bits);
			}

			PinState myState = getState(state);
			if (value == Value.NIL)
			{
				myState.sending = Value.createUnknown(attrs.width);
			}
			else
			{
				myState.sending = value;
			}
		}

		private static PinState getState(InstanceState state)
		{
			PinAttributes attrs = (PinAttributes) state.AttributeSet;
			BitWidth width = attrs.width;
			PinState ret = (PinState) state.Data;
			if (ret == null)
			{
				Value val = attrs.threeState ? Value.UNKNOWN : Value.FALSE;
				if (width.Width > 1)
				{
					Value[] arr = new Value[width.Width];
					Arrays.Fill(arr, val);
					val = Value.create(arr);
				}
				ret = new PinState(val, val);
				state.Data = ret;
			}
			if (ret.sending.Width != width.Width)
			{
				ret.sending = ret.sending.extendWidth(width.Width, attrs.threeState ? Value.UNKNOWN : Value.FALSE);
			}
			if (ret.receiving.Width != width.Width)
			{
				ret.receiving = ret.receiving.extendWidth(width.Width, Value.UNKNOWN);
			}
			return ret;
		}

		private class PinState : InstanceData, ICloneable
		{
			internal Value sending;
			internal Value receiving;

			public PinState(Value sending, Value receiving)
			{
				this.sending = sending;
				this.receiving = receiving;
			}

			public virtual object clone()
			{
				try
				{
					return base.clone();
				}
				catch (CloneNotSupportedException)
				{
					return null;
				}
			}
		}

		public class PinPoker : InstancePoker
		{
			internal int bitPressed = -1;

			public override void mousePressed(InstanceState state, MouseEvent e)
			{
				bitPressed = getBit(state, e);
			}

			public override void mouseReleased(InstanceState state, MouseEvent e)
			{
				int bit = getBit(state, e);
				if (bit == bitPressed && bit >= 0)
				{
					handleBitPress(state, bit, e);
				}
				bitPressed = -1;
			}

			internal virtual void handleBitPress(InstanceState state, int bit, MouseEvent e)
			{
				PinAttributes attrs = (PinAttributes) state.AttributeSet;
				if (!attrs.Input)
				{
					return;
				}

				java.awt.Component sourceComp = e.getComponent();
				if (sourceComp is Canvas && !state.CircuitRoot)
				{
					Canvas canvas = (Canvas) e.getComponent();
					CircuitState circState = canvas.CircuitState;
					java.awt.Component frame = SwingUtilities.getRoot(canvas);
					int choice = JOptionPane.showConfirmDialog(frame, Strings.get("pinFrozenQuestion"), Strings.get("pinFrozenTitle"), JOptionPane.OK_CANCEL_OPTION, JOptionPane.WARNING_MESSAGE);
					if (choice == JOptionPane.OK_OPTION)
					{
						circState = circState.cloneState();
						canvas.Project.CircuitState = circState;
						state = circState.getInstanceState(state.Instance);
					}
					else
					{
						return;
					}
				}

				PinState pinState = getState(state);
				Value val = pinState.sending.get(bit);
				if (val == Value.FALSE)
				{
					val = Value.TRUE;
				}
				else if (val == Value.TRUE)
				{
					val = attrs.threeState ? Value.UNKNOWN : Value.FALSE;
				}
				else
				{
					val = Value.FALSE;
				}
				pinState.sending = pinState.sending.set(bit, val);
				state.fireInvalidated();
			}

			internal virtual int getBit(InstanceState state, MouseEvent e)
			{
				BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
				if (width.Width == 1)
				{
					return 0;
				}
				else
				{
					Bounds bds = state.Instance.Bounds; // intentionally with no graphics object - we don't want
																	// label included
					int i = (bds.X + bds.Width - e.getX()) / 10;
					int j = (bds.Y + bds.Height - e.getY()) / 20;
					int bit = 8 * j + i;
					if (bit < 0 || bit >= width.Width)
					{
						return -1;
					}
					else
					{
						return bit;
					}
				}
			}
		}

		public class PinLogger : InstanceLogger
		{
			public override string getLogName(InstanceState state, object option)
			{
				PinAttributes attrs = (PinAttributes) state.AttributeSet;
				string ret = attrs.label;
				if (string.ReferenceEquals(ret, null) || ret.Equals(""))
				{
					string type = attrs.type == EndData.INPUT_ONLY ? Strings.get("pinInputName") : Strings.get("pinOutputName");
					return type + state.Instance.Location;
				}
				else
				{
					return ret;
				}
			}

			public override Value getLogValue(InstanceState state, object option)
			{
				PinState s = getState(state);
				return s.sending;
			}
		}
	}
}
