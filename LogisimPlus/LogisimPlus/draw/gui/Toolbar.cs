// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.gui
{


	using Canvas = draw.canvas.Canvas;
	using CanvasTool = draw.canvas.CanvasTool;
	using AbstractTool = draw.tools.AbstractTool;
	using DrawingAttributeSet = draw.tools.DrawingAttributeSet;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class Toolbar : JComponent
	{
		private static int ICON_WIDTH = 16;
		private static int ICON_HEIGHT = 16;
		private static int ICON_SEP = 4;

		private class Listener : MouseListener, MouseMotionListener
		{
			private readonly Toolbar outerInstance;

			public Listener(Toolbar outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal AbstractTool toolPressed;
			internal bool inTool;
			internal int toolX;
			internal int toolY;

			public virtual void mouseClicked(MouseEvent e)
			{
			}

			public virtual void mouseEntered(MouseEvent e)
			{
			}

			public virtual void mouseExited(MouseEvent e)
			{
			}

			public virtual void mousePressed(MouseEvent e)
			{
				int mx = e.getX();
				int my = e.getY();
				int col = (e.getX() - ICON_SEP) / (ICON_WIDTH + ICON_SEP);
				int row = (e.getY() - ICON_SEP) / (ICON_HEIGHT + ICON_SEP);
				int x0 = ICON_SEP + col * (ICON_SEP + ICON_WIDTH);
				int y0 = ICON_SEP + row * (ICON_SEP + ICON_HEIGHT);

				if (mx >= x0 && mx < x0 + ICON_WIDTH && my >= y0 && my < y0 + ICON_HEIGHT && col >= 0 && col < outerInstance.tools.Length && row >= 0 && row < outerInstance.tools[col].Length)
				{
					toolPressed = outerInstance.tools[col][row];
					inTool = true;
					toolX = x0;
					toolY = y0;
					repaint();
				}
				else
				{
					toolPressed = null;
					inTool = false;
				}
			}

			public virtual void mouseReleased(MouseEvent e)
			{
				mouseDragged(e);
				if (inTool)
				{
					outerInstance.canvas.Tool = toolPressed;
					repaint();
				}
				toolPressed = null;
				inTool = false;
			}

			public virtual void mouseDragged(MouseEvent e)
			{
				int mx = e.getX();
				int my = e.getY();
				int x0 = toolX;
				int y0 = toolY;

				bool was = inTool;
				bool now = toolPressed != null && mx >= x0 && mx < x0 + ICON_WIDTH && my >= y0 && my < y0 + ICON_HEIGHT;
				if (was != now)
				{
					inTool = now;
					repaint();
				}
			}

			public virtual void mouseMoved(MouseEvent e)
			{
			}

		}

		private Canvas canvas;
		private AbstractTool[][] tools;
		private Listener listener;

		public Toolbar(Canvas canvas, DrawingAttributeSet attrs)
		{
			this.canvas = canvas;
			this.tools = new AbstractTool[][] {AbstractTool.getTools(attrs)};
			this.listener = new Listener(this);

			AbstractTool[] toolBase = AbstractTool.getTools(attrs);
			this.tools = new AbstractTool[2][];
			this.tools[0] = new AbstractTool[(toolBase.Length + 1) / 2];
			this.tools[1] = new AbstractTool[toolBase.Length / 2];
			for (int i = 0; i < toolBase.Length; i++)
			{
				this.tools[i % 2][i / 2] = toolBase[i];
			}

			setPreferredSize(new Size(3 * ICON_SEP + 2 * ICON_WIDTH, ICON_SEP + tools[0].Length * (ICON_HEIGHT + ICON_SEP)));
			addMouseListener(listener);
			addMouseMotionListener(listener);
		}

		public virtual AbstractTool DefaultTool
		{
			get
			{
				return tools[0][0];
			}
		}

		public override void paintComponent(Graphics g)
		{
			g.clearRect(0, 0, getWidth(), getHeight());
			CanvasTool current = canvas.Tool;
			for (int i = 0; i < tools.Length; i++)
			{
				AbstractTool[] column = tools[i];
				int x = ICON_SEP + i * (ICON_SEP + ICON_WIDTH);
				int y = ICON_SEP;
				for (int j = 0; j < column.Length; j++)
				{
					AbstractTool tool = column[j];
					if (tool == listener.toolPressed && listener.inTool)
					{
						g.setColor(Color.darkGray);
						g.fillRect(x, y, ICON_WIDTH, ICON_HEIGHT);
					}
					Icon icon = tool.Icon;
					if (icon != null)
					{
						icon.paintIcon(this, g, x, y);
					}
					if (tool == current)
					{
						GraphicsUtil.switchToWidth(g, 2);
						g.setColor(Color.black);
						g.drawRect(x - 1, y - 1, ICON_WIDTH + 2, ICON_HEIGHT + 2);
					}
					y += ICON_HEIGHT + ICON_SEP;
				}
			}
			g.setColor(Color.black);
			GraphicsUtil.switchToWidth(g, 1);
		}
	}

}
