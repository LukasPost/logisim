// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Value = logisim.data.Value;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;
	using StdAttr = logisim.instance.StdAttr;

	public class ShiftRegisterPoker : InstancePoker
	{
		private int loc;

		public override bool init(InstanceState state, MouseEvent e)
		{
			loc = computeStage(state, e);
			return loc >= 0;
		}

		private int computeStage(InstanceState state, MouseEvent e)
		{
			int? lenObj = state.getAttributeValue(ShiftRegister.ATTR_LENGTH);
			BitWidth widObj = state.getAttributeValue(StdAttr.WIDTH);
			bool? loadObj = state.getAttributeValue(ShiftRegister.ATTR_LOAD);
			Bounds bds = state.Instance.Bounds;

			int y = bds.Y;
			string label = state.getAttributeValue(StdAttr.LABEL);
			if (string.ReferenceEquals(label, null) || label.Equals(""))
			{
				y += bds.Height / 2;
			}
			else
			{
				y += 3 * bds.Height / 4;
			}
			y = e.getY() - y;
			if (y <= -6 || y >= 8)
			{
				return -1;
			}

			int x = e.getX() - (bds.X + 15);
			if (!loadObj.Value || widObj.Width > 4)
			{
				return -1;
			}
			if (x < 0 || x >= lenObj.Value * 10)
			{
				return -1;
			}
			return x / 10;
		}

		public override void paint(InstancePainter painter)
		{
			int loc = this.loc;
			if (loc < 0)
			{
				return;
			}
			Bounds bds = painter.getInstance().Bounds;
			int x = bds.X + 15 + loc * 10;
			int y = bds.Y;
			string label = painter.getAttributeValue(StdAttr.LABEL);
			if (string.ReferenceEquals(label, null) || label.Equals(""))
			{
				y += bds.Height / 2;
			}
			else
			{
				y += 3 * bds.Height / 4;
			}
			Graphics g = painter.Graphics;
			g.setColor(Color.RED);
			g.drawRect(x, y - 6, 10, 13);
		}

		public override void mousePressed(InstanceState state, MouseEvent e)
		{
			loc = computeStage(state, e);
		}

		public override void mouseReleased(InstanceState state, MouseEvent e)
		{
			int oldLoc = loc;
			if (oldLoc < 0)
			{
				return;
			}
			BitWidth widObj = state.getAttributeValue(StdAttr.WIDTH);
			if (widObj.Equals(BitWidth.ONE))
			{
				int newLoc = computeStage(state, e);
				if (oldLoc == newLoc)
				{
					ShiftRegisterData data = (ShiftRegisterData) state.Data;
					int i = data.Length - 1 - loc;
					Value v = data.get(i);
					if (v == Value.FALSE)
					{
						v = Value.TRUE;
					}
					else
					{
						v = Value.FALSE;
					}
					data.set(i, v);
					state.fireInvalidated();
				}
			}
		}

		public override void keyTyped(InstanceState state, KeyEvent e)
		{
			int loc = this.loc;
			if (loc < 0)
			{
				return;
			}
			char c = e.getKeyChar();
			if (c == ' ')
			{
				int? lenObj = state.getAttributeValue(ShiftRegister.ATTR_LENGTH);
				if (loc < lenObj.Value - 1)
				{
					this.loc = loc + 1;
					state.fireInvalidated();
				}
			}
			else if (c == '\u0008')
			{
				if (loc > 0)
				{
					this.loc = loc - 1;
					state.fireInvalidated();
				}
			}
			else
			{
				try
				{
					int val = Convert.ToInt32("" + e.getKeyChar(), 16);
					BitWidth widObj = state.getAttributeValue(StdAttr.WIDTH);
					if ((val & ~widObj.Mask) != 0)
					{
						return;
					}
					Value valObj = Value.createKnown(widObj, val);
					ShiftRegisterData data = (ShiftRegisterData) state.Data;
					int i = data.Length - 1 - loc;
					if (!data.get(i).Equals(valObj))
					{
						data.set(i, valObj);
						state.fireInvalidated();
					}
				}
				catch (System.FormatException)
				{
					return;
				}
			}
		}
	}

}
