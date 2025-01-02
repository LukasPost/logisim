// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	internal class CanvasPaintThread : Thread
	{
		private const int REPAINT_TIMESPAN = 50; // 50 ms between repaints

		private Canvas canvas;
		private object @lock;
		private bool repaintRequested;
		private long nextRepaint;
		private bool alive;
		private Rectangle repaintRectangle;

		public CanvasPaintThread(Canvas canvas)
		{
			this.canvas = canvas;
			@lock = new object();
			repaintRequested = false;
			alive = true;
			nextRepaint = DateTimeHelper.CurrentUnixTimeMillis();
		}

		public virtual void requestStop()
		{
			lock (@lock)
			{
				alive = false;
				Monitor.PulseAll(@lock);
			}
		}

		public virtual void requentRepaint(Rectangle rect)
		{
			lock (@lock)
			{
				if (repaintRequested)
				{
					if (repaintRectangle != null)
					{
						repaintRectangle.add(rect);
					}
				}
				else
				{
					repaintRequested = true;
					repaintRectangle = rect;
					Monitor.PulseAll(@lock);
				}
			}
		}

		public virtual void requestRepaint()
		{
			lock (@lock)
			{
				if (!repaintRequested)
				{
					repaintRequested = true;
					repaintRectangle = null;
					Monitor.PulseAll(@lock);
				}
			}
		}

		public override void run()
		{
			while (alive)
			{
				long now = DateTimeHelper.CurrentUnixTimeMillis();
				lock (@lock)
				{
					long wait = nextRepaint - now;
					while (alive && !(repaintRequested && wait <= 0))
					{
						try
						{
							if (wait > 0)
							{
								Monitor.Wait(@lock, TimeSpan.FromMilliseconds(wait));
							}
							else
							{
								Monitor.Wait(@lock);
							}
						}
						catch (InterruptedException)
						{
						}
						now = DateTimeHelper.CurrentUnixTimeMillis();
						wait = nextRepaint - now;
					}
					if (!alive)
					{
						break;
					}
					repaintRequested = false;
					nextRepaint = now + REPAINT_TIMESPAN;
				}
				canvas.repaint();
			}
		}
	}

}
