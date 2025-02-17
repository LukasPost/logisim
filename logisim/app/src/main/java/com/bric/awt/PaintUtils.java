/*
* @(#)PaintUtils.java  1.0  2008-03-01
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

package com.bric.awt;

import java.awt.*;

import javax.swing.*;

/**
 * Some static methods for some common painting functions.
 *
 * @version 1.0
 * @author Jeremy Wood
 **/
public class PaintUtils {

	/** Four shades of white, each with increasing opacity. */
	public static final Color[] whites = { new Color(255, 255, 255, 50), new Color(255, 255, 255, 100),
			new Color(255, 255, 255, 150) };

	/** Four shades of black, each with increasing opacity. */
	public static final Color[] blacks = { new Color(0, 0, 0, 50), new Color(0, 0, 0, 100),
			new Color(0, 0, 0, 150) };

	/**
	 * @return the color used to indicate when a component has focus. By default this uses the color (64,113,167), but
	 *         you can override this by calling: <BR>
	 *         <code>UIManager.put("focusRing",customColor);</code>
	 */
	public static Color getFocusRingColor() {
		Color obj = UIManager.getColor("focusRing");
		if (obj != null)
			return obj;
		return new Color(64, 113, 167);
	}

	/**
	 * Paints 3 different strokes around a shape to indicate focus. The widest stroke is the most transparent, so this
	 * achieves a nice "glow" effect.
	 * <P>
	 * The catch is that you have to render this underneath the shape, and the shape should be filled completely.
	 * 
	 * @param g             the graphics to paint to
	 * @param shape         the shape to outline
	 * @param biggestStroke the widest stroke to use.
	 */
	public static void paintFocus(Graphics2D g, Shape shape, int biggestStroke) {
		Color focusColor = getFocusRingColor();
		Color[] focusArray = {
				new Color(focusColor.getRed(), focusColor.getGreen(), focusColor.getBlue(), 255),
				new Color(focusColor.getRed(), focusColor.getGreen(), focusColor.getBlue(), 170),
				new Color(focusColor.getRed(), focusColor.getGreen(), focusColor.getBlue(), 110) };
		g.setStroke(new BasicStroke(biggestStroke));
		g.setColor(focusArray[2]);
		g.draw(shape);
		g.setStroke(new BasicStroke(biggestStroke - 1));
		g.setColor(focusArray[1]);
		g.draw(shape);
		g.setStroke(new BasicStroke(biggestStroke - 2));
		g.setColor(focusArray[0]);
		g.draw(shape);
		g.setStroke(new BasicStroke(1));
	}

	/**
	 * Uses translucent shades of white and black to draw highlights and shadows around a rectangle, and then frames the
	 * rectangle with a shade of gray (120).
	 * <P>
	 * This should be called to add a finishing touch on top of existing graphics.
	 * 
	 * @param g the graphics to paint to.
	 * @param r the rectangle to paint.
	 */
	public static void drawBevel(Graphics g, Rectangle r) {
		drawColors(blacks, g, r.x, r.y + r.height, r.x + r.width, r.y + r.height, SwingConstants.SOUTH);
		drawColors(blacks, g, r.x + r.width, r.y, r.x + r.width, r.y + r.height, SwingConstants.EAST);

		drawColors(whites, g, r.x, r.y, r.x + r.width, r.y, SwingConstants.NORTH);
		drawColors(whites, g, r.x, r.y, r.x, r.y + r.height, SwingConstants.WEST);

		g.setColor(new Color(120, 120, 120));
		g.drawRect(r.x, r.y, r.width, r.height);
	}

	private static void drawColors(Color[] colors, Graphics g, int x1, int y1, int x2, int y2, int direction) {
		for (int a = 0; a < colors.length; a++) {
			g.setColor(colors[colors.length - a - 1]);
			if (direction == SwingConstants.SOUTH) g.drawLine(x1, y1 - a, x2, y2 - a);
			else if (direction == SwingConstants.NORTH) g.drawLine(x1, y1 + a, x2, y2 + a);
			else if (direction == SwingConstants.EAST) g.drawLine(x1 - a, y1, x2 - a, y2);
			else if (direction == SwingConstants.WEST) g.drawLine(x1 + a, y1, x2 + a, y2);
		}
	}
}
