// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/*
* @(#)ColorSwatch.java  1.0  2008-03-01
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
    using LogisimPlus.Java;

    /// <summary>
    /// This is a square, opaque panel used to indicate a certain color.
    /// <P>
    /// The color is assigned with the <code>setForeground()</code> method.
    /// <P>
    /// Also the user can right-click this panel and select 'Copy' to send a 100x100 image of this color to the clipboard.
    /// (This feature was added at the request of a friend who paints; she wanted to select a color and then quickly print it
    /// off, and then mix her paints to match that shade.)
    /// 
    /// @version 1.0
    /// @author Jeremy Wood
    /// </summary>
    public class ColorSwatch : JPanel
	{
		private const long serialVersionUID = 1L;

		internal JPopupMenu menu;
		internal JMenuItem copyItem;
		internal MouseListener mouseListener = new MouseAdapterAnonymousInnerClass();

		private class MouseAdapterAnonymousInnerClass : MouseAdapter
		{
			public void mousePressed(MouseEvent e)
			{
				if (e.isPopupTrigger())
				{
					if (outerInstance.menu == null)
					{
						outerInstance.menu = new JPopupMenu();
						outerInstance.copyItem = new JMenuItem(ColorPicker.strings.getObject("Copy").ToString());
						outerInstance.menu.add(outerInstance.copyItem);
						outerInstance.copyItem.addActionListener(outerInstance.actionListener);
					}
					outerInstance.menu.show(outerInstance, e.getX(), e.getY());
				}
			}
		}
		internal ActionListener actionListener = new ActionListenerAnonymousInnerClass();

		private class ActionListenerAnonymousInnerClass : ActionListener
		{
			public void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == outerInstance.copyItem)
				{
					BufferedImage image = new BufferedImage(100, 100, BufferedImage.TYPE_INT_RGB);
					JGraphics g = image.createJGraphics();
					g.setColor(getBackground());
					g.fillRect(0, 0, image.getWidth(), image.getHeight());
					g.dispose();
					Transferable contents = new ImageTransferable(image);
					Toolkit.getDefaultToolkit().getSystemClipboard().setContents(contents, null);
				}
			}
		}
		internal int w;

		public ColorSwatch(int width)
		{
			w = width;
			setPreferredSize(new Size(width, width));
			setMinimumSize(new Size(width, width));
			addMouseListener(mouseListener);
		}

		private static TexturePaint checkerPaint = null;

		private static TexturePaint CheckerPaint
		{
			get
			{
				if (checkerPaint == null)
				{
					int t = 8;
					BufferedImage bi = new BufferedImage(t * 2, t * 2, BufferedImage.TYPE_INT_RGB);
					JGraphics g = bi.createJGraphics();
					g.setColor(Color.White);
					g.fillRect(0, 0, 2 * t, 2 * t);
					g.setColor(Color.LightGray);
					g.fillRect(0, 0, t, t);
					g.fillRect(t, t, t, t);
					checkerPaint = new TexturePaint(bi, new Rectangle(0, 0, bi.getWidth(), bi.getHeight()));
				}
				return checkerPaint;
			}
		}

		public virtual void paint(JGraphics g)
		{
			base.paint(g); // may be necessary for some look-and-feels?

			Color c = getForeground();
			int w2 = Math.Min(getWidth(), w);
			int h2 = Math.Min(getHeight(), w);
			Rectangle r = new Rectangle(getWidth() / 2 - w2 / 2, getHeight() / 2 - h2 / 2, w2, h2);

			if (c.A < 255)
			{
				TexturePaint checkers = CheckerPaint;
				g.setPaint(checkers);
				g.fillRect(r.X, r.Y, r.Width, r.Height);
			}
			g.setColor(c);
			g.fillRect(r.X, r.Y, r.Width, r.Height);
			PaintUtils.drawBevel(g, r);
		}
	}

}
