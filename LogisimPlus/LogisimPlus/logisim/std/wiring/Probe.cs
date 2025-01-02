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

	using RadixOption = logisim.circuit.RadixOption;
	using TextField = logisim.comp.TextField;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceData = logisim.instance.InstanceData;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstanceLogger = logisim.instance.InstanceLogger;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class Probe : InstanceFactory
	{
		public static readonly Probe FACTORY = new Probe();

		private class StateData : InstanceData, ICloneable
		{
			internal Value curValue = Value.NIL;

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

		public class ProbeLogger : InstanceLogger
		{
			public ProbeLogger()
			{
			}

			public override string getLogName(InstanceState state, object option)
			{
				string ret = state.getAttributeValue(StdAttr.LABEL);
				return !string.ReferenceEquals(ret, null) && !ret.Equals("") ? ret : null;
			}

			public override Value getLogValue(InstanceState state, object option)
			{
				return getValue(state);
			}
		}

		public Probe() : base("Probe", Strings.getter("probeComponent"))
		{
			IconName = "probe.gif";
			FacingAttribute = StdAttr.FACING;
			InstanceLogger = typeof(ProbeLogger);
		}

		public override AttributeSet createAttributeSet()
		{
			return new ProbeAttributes();
		}

		public override Bounds getOffsetBounds(AttributeSet attrsBase)
		{
			ProbeAttributes attrs = (ProbeAttributes) attrsBase;
			return getOffsetBounds(attrs.facing, attrs.width, attrs.radix);
		}

		//
		// graphics methods
		//
		public override void paintGhost(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			Bounds bds = painter.OffsetBounds;
			g.drawOval(bds.X + 1, bds.Y + 1, bds.Width - 1, bds.Height - 1);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Value value = getValue(painter);

			Graphics g = painter.Graphics;
			Bounds bds = painter.Bounds; // intentionally with no graphics object - we don't want label included
			int x = bds.X;
			int y = bds.Y;
			g.setColor(Color.WHITE);
			g.fillRect(x + 5, y + 5, bds.Width - 10, bds.Height - 10);
			g.setColor(Color.GRAY);
			if (value.Width <= 1)
			{
				g.drawOval(x + 1, y + 1, bds.Width - 2, bds.Height - 2);
			}
			else
			{
				g.drawRoundRect(x + 1, y + 1, bds.Width - 2, bds.Height - 2, 6, 6);
			}

			g.setColor(Color.BLACK);
			painter.drawLabel();

			if (!painter.ShowState)
			{
				if (value.Width > 0)
				{
					GraphicsUtil.drawCenteredText(g, "x" + value.Width, bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
				}
			}
			else
			{
				paintValue(painter, value);
			}

			painter.drawPorts();
		}

		internal static void paintValue(InstancePainter painter, Value value)
		{
			Graphics g = painter.Graphics;
			Bounds bds = painter.Bounds; // intentionally with no graphics object - we don't want label included

			RadixOption radix = painter.getAttributeValue(RadixOption.ATTRIBUTE);
			if (radix == null || radix == RadixOption.RADIX_2)
			{
				int x = bds.X;
				int y = bds.Y;
				int wid = value.Width;
				if (wid == 0)
				{
					x += bds.Width / 2;
					y += bds.Height / 2;
					GraphicsUtil.switchToWidth(g, 2);
					g.drawLine(x - 4, y, x + 4, y);
					return;
				}
				int x0 = bds.X + bds.Width - 5;
				int compWidth = wid * 10;
				if (compWidth < bds.Width - 3)
				{
					x0 = bds.X + (bds.Width + compWidth) / 2 - 5;
				}
				int cx = x0;
				int cy = bds.Y + bds.Height - 12;
				int cur = 0;
				for (int k = 0; k < wid; k++)
				{
					GraphicsUtil.drawCenteredText(g, value.get(k).toDisplayString(), cx, cy);
					++cur;
					if (cur == 8)
					{
						cur = 0;
						cx = x0;
						cy -= 20;
					}
					else
					{
						cx -= 10;
					}
				}
			}
			else
			{
				string text = radix.toString(value);
				GraphicsUtil.drawCenteredText(g, text, bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
			}
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			instance.setPorts(new Port[] {new Port(0, 0, Port.INPUT, BitWidth.UNKNOWN)});
			instance.addAttributeListener();
			configureLabel(instance);
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == Pin.ATTR_LABEL_LOC)
			{
				configureLabel(instance);
			}
			else if (attr == StdAttr.FACING || attr == RadixOption.ATTRIBUTE)
			{
				instance.recomputeBounds();
				configureLabel(instance);
			}
		}

		public override void propagate(InstanceState state)
		{
			StateData oldData = (StateData) state.Data;
			Value oldValue = oldData == null ? Value.NIL : oldData.curValue;
			Value newValue = state.getPort(0);
			bool same = oldValue == null ? newValue == null : oldValue.Equals(newValue);
			if (!same)
			{
				if (oldData == null)
				{
					oldData = new StateData();
					oldData.curValue = newValue;
					state.Data = oldData;
				}
				else
				{
					oldData.curValue = newValue;
				}
				int oldWidth = oldValue == null ? 1 : oldValue.BitWidth.Width;
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") int newWidth = newValue.getBitWidth().getWidth();
				int newWidth = newValue.BitWidth.Width;
				if (oldWidth != newWidth)
				{
					ProbeAttributes attrs = (ProbeAttributes) state.AttributeSet;
					attrs.width = newValue.BitWidth;
					state.Instance.recomputeBounds();
					configureLabel(state.Instance);
				}
				state.fireInvalidated();
			}
		}

		private static Value getValue(InstanceState state)
		{
			StateData data = (StateData) state.Data;
			return data == null ? Value.NIL : data.curValue;
		}

		internal virtual void configureLabel(Instance instance)
		{
			ProbeAttributes attrs = (ProbeAttributes) instance.AttributeSet;
			Probe.configureLabel(instance, attrs.labelloc, attrs.facing);
		}

		//
		// static methods
		//
		internal static Bounds getOffsetBounds(Direction dir, BitWidth width, RadixOption radix)
		{
			Bounds ret = null;
			int len = radix == null || radix == RadixOption.RADIX_2 ? width.Width : radix.getMaxLength(width);
			if (dir == Direction.East)
			{
				switch (len)
				{
				case 0:
				case 1:
					ret = Bounds.create(-20, -10, 20, 20);
					break;
				case 2:
					ret = Bounds.create(-20, -10, 20, 20);
					break;
				case 3:
					ret = Bounds.create(-30, -10, 30, 20);
					break;
				case 4:
					ret = Bounds.create(-40, -10, 40, 20);
					break;
				case 5:
					ret = Bounds.create(-50, -10, 50, 20);
					break;
				case 6:
					ret = Bounds.create(-60, -10, 60, 20);
					break;
				case 7:
					ret = Bounds.create(-70, -10, 70, 20);
					break;
				case 8:
					ret = Bounds.create(-80, -10, 80, 20);
					break;
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
					ret = Bounds.create(-80, -20, 80, 40);
					break;
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
					ret = Bounds.create(-80, -30, 80, 60);
					break;
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
					ret = Bounds.create(-80, -40, 80, 80);
					break;
				}
			}
			else if (dir == Direction.West)
			{
				switch (len)
				{
				case 0:
				case 1:
					ret = Bounds.create(0, -10, 20, 20);
					break;
				case 2:
					ret = Bounds.create(0, -10, 20, 20);
					break;
				case 3:
					ret = Bounds.create(0, -10, 30, 20);
					break;
				case 4:
					ret = Bounds.create(0, -10, 40, 20);
					break;
				case 5:
					ret = Bounds.create(0, -10, 50, 20);
					break;
				case 6:
					ret = Bounds.create(0, -10, 60, 20);
					break;
				case 7:
					ret = Bounds.create(0, -10, 70, 20);
					break;
				case 8:
					ret = Bounds.create(0, -10, 80, 20);
					break;
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
					ret = Bounds.create(0, -20, 80, 40);
					break;
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
					ret = Bounds.create(0, -30, 80, 60);
					break;
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
					ret = Bounds.create(0, -40, 80, 80);
					break;
				}
			}
			else if (dir == Direction.South)
			{
				switch (len)
				{
				case 0:
				case 1:
					ret = Bounds.create(-10, -20, 20, 20);
					break;
				case 2:
					ret = Bounds.create(-10, -20, 20, 20);
					break;
				case 3:
					ret = Bounds.create(-15, -20, 30, 20);
					break;
				case 4:
					ret = Bounds.create(-20, -20, 40, 20);
					break;
				case 5:
					ret = Bounds.create(-25, -20, 50, 20);
					break;
				case 6:
					ret = Bounds.create(-30, -20, 60, 20);
					break;
				case 7:
					ret = Bounds.create(-35, -20, 70, 20);
					break;
				case 8:
					ret = Bounds.create(-40, -20, 80, 20);
					break;
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
					ret = Bounds.create(-40, -40, 80, 40);
					break;
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
					ret = Bounds.create(-40, -60, 80, 60);
					break;
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
					ret = Bounds.create(-40, -80, 80, 80);
					break;
				}
			}
			else if (dir == Direction.North)
			{
				switch (len)
				{
				case 0:
				case 1:
					ret = Bounds.create(-10, 0, 20, 20);
					break;
				case 2:
					ret = Bounds.create(-10, 0, 20, 20);
					break;
				case 3:
					ret = Bounds.create(-15, 0, 30, 20);
					break;
				case 4:
					ret = Bounds.create(-20, 0, 40, 20);
					break;
				case 5:
					ret = Bounds.create(-25, 0, 50, 20);
					break;
				case 6:
					ret = Bounds.create(-30, 0, 60, 20);
					break;
				case 7:
					ret = Bounds.create(-35, 0, 70, 20);
					break;
				case 8:
					ret = Bounds.create(-40, 0, 80, 20);
					break;
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
					ret = Bounds.create(-40, 0, 80, 40);
					break;
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
					ret = Bounds.create(-40, 0, 80, 60);
					break;
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
					ret = Bounds.create(-40, 0, 80, 80);
					break;
				}
			}
			if (ret == null)
			{
				ret = Bounds.create(0, -10, 20, 20); // should never happen
			}
			return ret;
		}

		internal static void configureLabel(Instance instance, Direction labelLoc, Direction facing)
		{
			Bounds bds = instance.Bounds;
			int x;
			int y;
			int halign;
			int valign;
			if (labelLoc == Direction.North)
			{
				halign = TextField.H_CENTER;
				valign = TextField.V_BOTTOM;
				x = bds.X + bds.Width / 2;
				y = bds.Y - 2;
				if (facing == labelLoc)
				{
					halign = TextField.H_LEFT;
					x += 2;
				}
			}
			else if (labelLoc == Direction.South)
			{
				halign = TextField.H_CENTER;
				valign = TextField.V_TOP;
				x = bds.X + bds.Width / 2;
				y = bds.Y + bds.Height + 2;
				if (facing == labelLoc)
				{
					halign = TextField.H_LEFT;
					x += 2;
				}
			}
			else if (labelLoc == Direction.East)
			{
				halign = TextField.H_LEFT;
				valign = TextField.V_CENTER;
				x = bds.X + bds.Width + 2;
				y = bds.Y + bds.Height / 2;
				if (facing == labelLoc)
				{
					valign = TextField.V_BOTTOM;
					y -= 2;
				}
			}
			else
			{ // WEST
				halign = TextField.H_RIGHT;
				valign = TextField.V_CENTER;
				x = bds.X - 2;
				y = bds.Y + bds.Height / 2;
				if (facing == labelLoc)
				{
					valign = TextField.V_BOTTOM;
					y -= 2;
				}
			}

			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, x, y, halign, valign);
		}
	}
}
