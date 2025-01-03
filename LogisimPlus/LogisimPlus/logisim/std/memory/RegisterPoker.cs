// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{
    using LogisimPlus.Java;
    using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;
	using StdAttr = logisim.instance.StdAttr;

	public class RegisterPoker : InstancePoker
	{
		private int initValue;
		private int curValue;

		public override bool init(InstanceState state, MouseEvent e)
		{
			RegisterData data = (RegisterData) state.Data;
			if (data == null)
			{
				data = new RegisterData();
				state.Data = data;
			}
			initValue = data.value;
			curValue = initValue;
			return true;
		}

		public override void paint(InstancePainter painter)
		{
			Bounds bds = painter.Bounds;
			BitWidth dataWidth = (BitWidth)painter.getAttributeValue(StdAttr.Width);
			int width = dataWidth == null ? 8 : dataWidth.Width;
			int len = (width + 3) / 4;

			JGraphics g = painter.Graphics;
			g.setColor(Color.Red);
			if (len > 4)
			{
				g.drawRect(bds.X, bds.Y + 3, bds.Width, 25);
			}
			else
			{
				int wid = 7 * len + 2;
				g.drawRect(bds.X + (bds.Width - wid) / 2, bds.Y + 4, wid, 15);
			}
			g.setColor(Color.Black);
		}

		public override void keyTyped(InstanceState state, KeyEvent e)
		{
			int val = Character.digit(e.getKeyChar(), 16);
			if (val < 0)
			{
				return;
			}

			BitWidth dataWidth = state.getAttributeValue(StdAttr.Width);
			if (dataWidth == null)
			{
				dataWidth = BitWidth.create(8);
			}
			curValue = (curValue * 16 + val) & dataWidth.Mask;
			RegisterData data = (RegisterData) state.Data;
			data.value = curValue;

			state.fireInvalidated();
		}
	}

}
