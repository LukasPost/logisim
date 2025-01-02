// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.start
{


	using Main = logisim.Main;
	using Value = logisim.data.Value;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class About
	{
		internal const int IMAGE_BORDER = 10;
		internal const int IMAGE_WIDTH = 380;
		internal const int IMAGE_HEIGHT = 284;

		private class PanelThread : Thread
		{
			internal MyPanel panel;
			internal bool running = true;

			internal PanelThread(MyPanel panel)
			{
				this.panel = panel;
			}

			public override void run()
			{
				long start = DateTimeHelper.CurrentUnixTimeMillis();
				while (running)
				{
					long elapse = DateTimeHelper.CurrentUnixTimeMillis() - start;
					int count = (int)(elapse / 500) % 4;
					panel.upper = (count == 2 || count == 3) ? Value.TRUE : Value.FALSE;
					panel.lower = (count == 1 || count == 2) ? Value.TRUE : Value.FALSE;
					panel.credits.Scroll = (int) elapse;
					panel.repaint();
					try
					{
						Thread.Sleep(20);
					}
					catch (InterruptedException)
					{
					}
				}
			}
		}

		private class MyPanel : JPanel, AncestorListener
		{
			internal readonly Color fadeColor = new Color(255, 255, 255, 128);
			internal readonly Color headerColor = new Color(143, 0, 0);
			internal readonly Color gateColor = Color.DARK_GRAY;
			internal readonly Font headerFont = new Font("Monospaced", Font.BOLD, 72);
			internal readonly Font versionFont = new Font("Serif", Font.PLAIN | Font.ITALIC, 32);
			internal readonly Font copyrightFont = new Font("Serif", Font.ITALIC, 18);

			internal Value upper = Value.FALSE;
			internal Value lower = Value.TRUE;
			internal AboutCredits credits;
			internal PanelThread thread = null;

			public MyPanel()
			{
				setLayout(null);

				int prefWidth = IMAGE_WIDTH + 2 * IMAGE_BORDER;
				int prefHeight = IMAGE_HEIGHT + 2 * IMAGE_BORDER;
				setPreferredSize(new Dimension(prefWidth, prefHeight));
				setBackground(Color.WHITE);
				addAncestorListener(this);

				credits = new AboutCredits();
				credits.setBounds(0, prefHeight / 2, prefWidth, prefHeight / 2);
				add(credits);
			}

			public override void paintComponent(Graphics g)
			{
				base.paintComponent(g);

				try
				{
					int x = IMAGE_BORDER;
					int y = IMAGE_BORDER;
					drawCircuit(g, x + 10, y + 55);
					g.setColor(fadeColor);
					g.fillRect(x, y, IMAGE_WIDTH, IMAGE_HEIGHT);
					drawText(g, x, y);
				}
				catch (Exception)
				{
				}
			}

			internal virtual void drawCircuit(Graphics g, int x0, int y0)
			{
				if (g is Graphics2D)
				{
					Graphics2D g2 = (Graphics2D) g;
					g2.setStroke(new BasicStroke(5.0f));
				}
				drawWires(g, x0, y0);
				g.setColor(gateColor);
				drawNot(g, x0, y0, 70, 10);
				drawNot(g, x0, y0, 70, 110);
				drawAnd(g, x0, y0, 130, 30);
				drawAnd(g, x0, y0, 130, 90);
				drawOr(g, x0, y0, 220, 60);
			}

			internal virtual void drawWires(Graphics g, int x0, int y0)
			{
				Value upperNot = upper.not();
				Value lowerNot = lower.not();
				Value upperAnd = upperNot.and(lower);
				Value lowerAnd = lowerNot.and(upper);
				Value @out = upperAnd.or(lowerAnd);
				int x;
				int y;

				g.setColor(upper.Color);
				x = toX(x0, 20);
				y = toY(y0, 10);
				g.fillOval(x - 7, y - 7, 14, 14);
				g.drawLine(toX(x0, 0), y, toX(x0, 40), y);
				g.drawLine(x, y, x, toY(y0, 70));
				y = toY(y0, 70);
				g.drawLine(x, y, toX(x0, 80), y);
				g.setColor(upperNot.Color);
				y = toY(y0, 10);
				g.drawLine(toX(x0, 70), y, toX(x0, 80), y);

				g.setColor(lower.Color);
				x = toX(x0, 30);
				y = toY(y0, 110);
				g.fillOval(x - 7, y - 7, 14, 14);
				g.drawLine(toX(x0, 0), y, toX(x0, 40), y);
				g.drawLine(x, y, x, toY(y0, 50));
				y = toY(y0, 50);
				g.drawLine(x, y, toX(x0, 80), y);
				g.setColor(lowerNot.Color);
				y = toY(y0, 110);
				g.drawLine(toX(x0, 70), y, toX(x0, 80), y);

				g.setColor(upperAnd.Color);
				x = toX(x0, 150);
				y = toY(y0, 30);
				g.drawLine(toX(x0, 130), y, x, y);
				g.drawLine(x, y, x, toY(y0, 45));
				y = toY(y0, 45);
				g.drawLine(x, y, toX(x0, 174), y);
				g.setColor(lowerAnd.Color);
				y = toY(y0, 90);
				g.drawLine(toX(x0, 130), y, x, y);
				g.drawLine(x, y, x, toY(y0, 75));
				y = toY(y0, 75);
				g.drawLine(x, y, toX(x0, 174), y);

				g.setColor(@out.Color);
				y = toY(y0, 60);
				g.drawLine(toX(x0, 220), y, toX(x0, 240), y);
			}

			internal virtual void drawNot(Graphics g, int x0, int y0, int x, int y)
			{
				int[] xp = new int[4];
				int[] yp = new int[4];
				xp[0] = toX(x0, x - 10);
				yp[0] = toY(y0, y);
				xp[1] = toX(x0, x - 29);
				yp[1] = toY(y0, y - 7);
				xp[2] = xp[1];
				yp[2] = toY(y0, y + 7);
				xp[3] = xp[0];
				yp[3] = yp[0];
				g.drawPolyline(xp, yp, 4);
				int diam = toDim(10);
				g.drawOval(xp[0], yp[0] - diam / 2, diam, diam);
			}

			internal virtual void drawAnd(Graphics g, int x0, int y0, int x, int y)
			{
				int[] xp = new int[4];
				int[] yp = new int[4];
				xp[0] = toX(x0, x - 25);
				yp[0] = toY(y0, y - 25);
				xp[1] = toX(x0, x - 50);
				yp[1] = yp[0];
				xp[2] = xp[1];
				yp[2] = toY(y0, y + 25);
				xp[3] = xp[0];
				yp[3] = yp[2];
				int diam = toDim(50);
				g.drawArc(xp[1], yp[1], diam, diam, -90, 180);
				g.drawPolyline(xp, yp, 4);
			}

			internal virtual void drawOr(Graphics g, int x0, int y0, int x, int y)
			{
				int cx = toX(x0, x - 50);
				int cd = toDim(62);
				GraphicsUtil.drawCenteredArc(g, cx, toY(y0, y - 37), cd, -90, 53);
				GraphicsUtil.drawCenteredArc(g, cx, toY(y0, y + 37), cd, 90, -53);
				GraphicsUtil.drawCenteredArc(g, toX(x0, x - 93), toY(y0, y), toDim(50), -30, 60);
			}

			internal static int toX(int x0, int offs)
			{
				return x0 + offs * 3 / 2;
			}

			internal static int toY(int y0, int offs)
			{
				return y0 + offs * 3 / 2;
			}

			internal static int toDim(int offs)
			{
				return offs * 3 / 2;
			}

			internal virtual void drawText(Graphics g, int x, int y)
			{
				FontMetrics fm;
				string str;

				g.setColor(headerColor);
				g.setFont(headerFont);
				g.drawString("Logisim", x, y + 45);
				g.setFont(copyrightFont);
				fm = g.getFontMetrics();
				str = "\u00a9 " + Main.COPYRIGHT_YEAR;
				g.drawString(str, x + IMAGE_WIDTH - fm.stringWidth(str), y + 16);
				g.setFont(versionFont);
				fm = g.getFontMetrics();
				str = "Version " + Main.VERSION_NAME;
				g.drawString(str, x + IMAGE_WIDTH - fm.stringWidth(str), y + 75);
			}

			public virtual void ancestorAdded(AncestorEvent arg0)
			{
				if (thread == null)
				{
					thread = new PanelThread(this);
					thread.Start();
				}
			}

			public virtual void ancestorRemoved(AncestorEvent arg0)
			{
				if (thread != null)
				{
					thread.running = false;
				}
			}

			public virtual void ancestorMoved(AncestorEvent arg0)
			{
			}
		}

		private About()
		{
		}

		public static MyPanel ImagePanel
		{
			get
			{
				return new MyPanel();
			}
		}

		public static void showAboutDialog(JFrame owner)
		{
			MyPanel imgPanel = ImagePanel;
			JPanel panel = new JPanel(new BorderLayout());
			panel.add(imgPanel);
			panel.setBorder(BorderFactory.createLineBorder(Color.BLACK, 2));

			JOptionPane.showMessageDialog(owner, panel, "Logisim " + Main.VERSION_NAME, JOptionPane.PLAIN_MESSAGE);
		}
	}
}
