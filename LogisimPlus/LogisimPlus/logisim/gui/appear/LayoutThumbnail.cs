// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;
using System.Collections.Generic;
using System.Linq;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using AppearancePort = logisim.circuit.appear.AppearancePort;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using Bounds = logisim.data.Bounds;
	using Instance = logisim.instance.Instance;
	using Pin = logisim.std.wiring.Pin;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	public class LayoutThumbnail : JComponent
	{
		private const int BORDER = 10;

		private CircuitState circuitState;
		private ICollection<Instance> ports;

		public LayoutThumbnail()
		{
			circuitState = null;
			ports = null;
			setBackground(Color.LightGray);
			setPreferredSize(new Size(200, 200));
		}

		public virtual void setCircuit(CircuitState circuitState, ICollection<Instance> ports)
		{
			this.circuitState = circuitState;
			this.ports = ports;
			repaint();
		}

		public internal override void paintComponent(JGraphics g)
		{
			if (circuitState != null)
			{
				Circuit circuit = circuitState.Circuit;
				Bounds bds = circuit.getBounds(g);
				Size size = getSize();
				double scaleX = (double)(size.Width - 2 * BORDER) / bds.Width;
				double scaleY = (double)(size.Height - 2 * BORDER) / bds.Height;
				double scale = Math.Min(1.0, Math.Min(scaleX, scaleY));

				JGraphics gCopy = g.create();
				int borderX = (int)((size.Width - bds.Width * scale) / 2);
				int borderY = (int)((size.Height - bds.Height * scale) / 2);
				gCopy.translate(borderX, borderY);
				if (scale != 1.0)
				{
					gCopy.scale(scale, scale);
				}
				gCopy.translate(-bds.X, -bds.Y);

				ComponentDrawContext context = new ComponentDrawContext(this, circuit, circuitState, g, gCopy);
				context.ShowState = false;
				context.ShowColor = false;
				circuit.draw(context, []);
				if (ports != null)
				{
					gCopy.setColor(AppearancePort.COLOR);
					int width = Math.Max(4, (int)((2 / scale) + 0.5));
					JGraphicsUtil.switchToWidth(gCopy, width);
					foreach (Instance port in ports)
					{
						Bounds b = port.Bounds;
						int x = b.X;
						int y = b.Y;
						int w = b.Width;
						int h = b.Height;
						if (Pin.FACTORY.isInputPin(port))
						{
							gCopy.drawRect(x, y, w, h);
						}
						else
						{
							if (b.Width > 25)
							{
								gCopy.drawRoundRect(x, y, w, h, 4, 4);
							}
							else
							{
								gCopy.drawOval(x, y, w, h);
							}
						}
					}
				}
				gCopy.dispose();

				g.setColor(Color.Black);
				JGraphicsUtil.switchToWidth(g, 2);
				g.drawRect(0, 0, size.Width - 2, size.Height - 2);
			}
		}

	}

}
