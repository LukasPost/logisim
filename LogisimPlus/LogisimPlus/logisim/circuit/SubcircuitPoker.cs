// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{
    using LogisimPlus.Java;
    using Bounds = logisim.data.Bounds;
	using Canvas = logisim.gui.main.Canvas;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;

	public class SubcircuitPoker : InstancePoker
	{

		private static readonly Color MAGNIFYING_INTERIOR = Color.FromArgb(255, 200, 200, 255, 64);
		private static readonly Color MAGNIFYING_INTERIOR_DOWN = Color.FromArgb(255, 128, 128, 255, 192);

		private bool mouseDown;

		public override Bounds getBounds(InstancePainter painter)
		{
			Bounds bds = painter.getInstance().Bounds;
			int cx = bds.X + bds.Width / 2;
			int cy = bds.Y + bds.Height / 2;
			return Bounds.create(cx - 5, cy - 5, 15, 15);
		}

		public override void paint(InstancePainter painter)
		{
			if (painter.Destination is Canvas && painter.Data is CircuitState)
			{
				Bounds bds = painter.getInstance().Bounds;
				int cx = bds.X + bds.Width / 2;
				int cy = bds.Y + bds.Height / 2;

				int tx = cx + 3;
				int ty = cy + 3;
				int[] xp = new int[] {tx - 1, cx + 8, cx + 10, tx + 1};
				int[] yp = new int[] {ty + 1, cy + 10, cy + 8, ty - 1};
				JGraphics g = painter.Graphics;
				if (mouseDown)
				{
					g.setColor(MAGNIFYING_INTERIOR_DOWN);
				}
				else
				{
					g.setColor(MAGNIFYING_INTERIOR);
				}
				g.fillOval(cx - 5, cy - 5, 10, 10);
				g.setColor(Color.Black);
				g.drawOval(cx - 5, cy - 5, 10, 10);
				g.fillPolygon(xp, yp, xp.Length);
			}
		}

		public override void mousePressed(InstanceState state, MouseEvent e)
		{
			if (isWithin(state, e))
			{
				mouseDown = true;
				state.Instance.fireInvalidated();
			}
		}

		public override void mouseReleased(InstanceState state, MouseEvent e)
		{
			if (mouseDown)
			{
				mouseDown = false;
				object sub = state.Data;
				if (e.getClickCount() == 2 && isWithin(state, e) && sub is CircuitState)
				{
					state.Project.CircuitState = (CircuitState) sub;
				}
				else
				{
					state.Instance.fireInvalidated();
				}
			}
		}

		private bool isWithin(InstanceState state, MouseEvent e)
		{
			Bounds bds = state.Instance.Bounds;
			int cx = bds.X + bds.Width / 2;
			int cy = bds.Y + bds.Height / 2;
			int dx = e.getX() - cx;
			int dy = e.getY() - cy;
			return dx * dx + dy * dy <= 60;
		}
	}

}
