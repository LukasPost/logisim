// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/*
* @(#)ColorPickerSliderUI.java  1.0  2008-03-01
*
* Copyright (c) 2008 Jeremy Wood
* E-mail: mickleness@gmail.com
* All rights reserved.
*
* The copyright of this software is owned by Jeremy Wood.
* You may not use, copy or modify this software, except in
* accordance with the license agreement you entered into with
* Jeremy Wood. For details see accompanying license terms.
*/

namespace com.bric.swing
{
	using com.bric.awt;



	/// <summary>
	/// This is a non-public SliderUI designed specifically for the <code>ColorPicker</code>.
	/// 
	/// @version 1.1
	/// @author Jeremy Wood
	/// </summary>
	internal class ColorPickerSliderUI : BasicSliderUI
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			bi = new BufferedImage(1, intArray.Length, BufferedImage.TYPE_INT_RGB);
		}

		internal ColorPicker colorPicker;

		/// <summary>
		/// Half of the height of the arrow </summary>
		internal int ARROW_HALF = 8;

		internal int[] intArray = new int[Toolkit.getDefaultToolkit().getScreenSize().height];
		internal BufferedImage bi;
		internal int lastMode = -1;

		internal ColorPickerSliderUI(JSlider b, ColorPicker cp) : base(b)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			colorPicker = cp;
			cp.ColorPanel.addComponentListener(new ComponentAdapterAnonymousInnerClass(this));
		}

		private class ComponentAdapterAnonymousInnerClass : ComponentAdapter
		{
			private readonly ColorPickerSliderUI outerInstance;

			public ComponentAdapterAnonymousInnerClass(ColorPickerSliderUI outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void componentResized(ComponentEvent e)
			{
				outerInstance.calculateGeometry();
				slider.repaint();
			}
		}

		public virtual void paintThumb(Graphics g)
		{
			int y = thumbRect.y + thumbRect.height / 2;
			Polygon polygon = new Polygon();
			polygon.addPoint(0, y - ARROW_HALF);
			polygon.addPoint(ARROW_HALF, y);
			polygon.addPoint(0, y + ARROW_HALF);

			Graphics2D g2 = (Graphics2D) g;
			g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
			g2.setColor(Color.black);
			g2.fill(polygon);
			g2.setColor(Color.white);
			g2.draw(polygon);
		}

		protected internal virtual void calculateThumbSize()
		{
			base.calculateThumbSize();
			thumbRect.height += 4;
			thumbRect.y -= 2;
		}

		protected internal virtual void calculateTrackRect()
		{
			base.calculateTrackRect();
			ColorPickerPanel cp = colorPicker.ColorPanel;
			int size = Math.Min(ColorPickerPanel.MAX_SIZE, Math.Min(cp.getWidth(), cp.getHeight()));
			int max = slider.getHeight() - ARROW_HALF * 2 - 2;
			if (size > max)
			{
				size = max;
			}
			trackRect.y = slider.getHeight() / 2 - size / 2;
			trackRect.height = size;
		}

		public virtual void paintTrack(Graphics g)
		{
			lock (this)
			{
				int mode = colorPicker.Mode;
				if (mode == ColorPicker.HUE || mode == ColorPicker.BRI || mode == ColorPicker.SAT)
				{
					float[] hsb = colorPicker.HSB;
					if (mode == ColorPicker.HUE)
					{
						for (int y = 0; y < trackRect.height; y++)
						{
							float hue = ((float) y) / ((float) trackRect.height);
							intArray[y] = Color.HSBtoRGB(hue, 1, 1);
						}
					}
					else if (mode == ColorPicker.SAT)
					{
						for (int y = 0; y < trackRect.height; y++)
						{
							float sat = 1 - ((float) y) / ((float) trackRect.height);
							intArray[y] = Color.HSBtoRGB(hsb[0], sat, hsb[2]);
						}
					}
					else
					{
						for (int y = 0; y < trackRect.height; y++)
						{
							float bri = 1 - ((float) y) / ((float) trackRect.height);
							intArray[y] = Color.HSBtoRGB(hsb[0], hsb[1], bri);
						}
					}
				}
				else
				{
					int[] rgb = colorPicker.RGB;
					if (mode == ColorPicker.RED)
					{
						for (int y = 0; y < trackRect.height; y++)
						{
							int red = 255 - (int)(y * 255 / trackRect.height + .49);
							intArray[y] = (red << 16) + (rgb[1] << 8) + (rgb[2]);
						}
					}
					else if (mode == ColorPicker.GREEN)
					{
						for (int y = 0; y < trackRect.height; y++)
						{
							int green = 255 - (int)(y * 255 / trackRect.height + .49);
							intArray[y] = (rgb[0] << 16) + (green << 8) + (rgb[2]);
						}
					}
					else if (mode == ColorPicker.BLUE)
					{
						for (int y = 0; y < trackRect.height; y++)
						{
							int blue = 255 - (int)(y * 255 / trackRect.height + .49);
							intArray[y] = (rgb[0] << 16) + (rgb[1] << 8) + (blue);
						}
					}
				}
				Graphics2D g2 = (Graphics2D) g;
				Rectangle r = new Rectangle(6, trackRect.y, 14, trackRect.height);
				if (slider.hasFocus())
				{
					PaintUtils.paintFocus(g2, r, 3);
				}
        
				bi.getRaster().setDataElements(0, 0, 1, trackRect.height, intArray);
				TexturePaint p = new TexturePaint(bi, new Rectangle(0, trackRect.y, 1, bi.getHeight()));
				g2.setPaint(p);
				g2.fillRect(r.x, r.y, r.width, r.height);
        
				PaintUtils.drawBevel(g2, r);
			}
		}

		public virtual void paintFocus(Graphics g)
		{
		}

		/// <summary>
		/// This overrides the default behavior for this slider and sets the thumb to where the user clicked. From a design
		/// standpoint, users probably don't want to scroll through several colors to get where they clicked: they simply
		/// want the color they selected.
		/// </summary>
		internal MouseInputAdapter myMouseListener = new MouseInputAdapterAnonymousInnerClass();

		private class MouseInputAdapterAnonymousInnerClass : MouseInputAdapter
		{
			public void mousePressed(MouseEvent e)
			{
				slider.setValueIsAdjusting(true);
				updateSliderValue(e);
			}

			private void updateSliderValue(MouseEvent e)
			{
				int v;
				if (slider.getOrientation() == JSlider.HORIZONTAL)
				{
					int x = e.getX();
					v = valueForXPosition(x);
				}
				else
				{
					int y = e.getY();
					v = valueForYPosition(y);
				}
				slider.setValue(v);
			}

			public void mouseReleased(MouseEvent e)
			{
				updateSliderValue(e);
				slider.setValueIsAdjusting(false);
			}

			public void mouseDragged(MouseEvent e)
			{
				updateSliderValue(e);
			}
		}

		protected internal virtual void installListeners(JSlider slider)
		{
			base.installListeners(slider);
			slider.removeMouseListener(trackListener);
			slider.removeMouseMotionListener(trackListener);
			slider.addMouseListener(myMouseListener);
			slider.addMouseMotionListener(myMouseListener);
			slider.setOpaque(false);
		}

		protected internal virtual void uninstallListeners(JSlider slider)
		{
			base.uninstallListeners(slider);
			slider.removeMouseListener(myMouseListener);
			slider.removeMouseMotionListener(myMouseListener);
		}

	}
}
