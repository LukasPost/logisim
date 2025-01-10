/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.util;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.font.FontRenderContext;
import java.awt.font.TextLayout;
import java.awt.geom.AffineTransform;
import java.awt.geom.Rectangle2D;
import java.util.Arrays;

import javax.swing.JTextField;

import logisim.data.Bounds;
import logisim.data.Location;

public class EditableLabel implements Cloneable {
	public static final int LEFT = JTextField.LEFT;
	public static final int RIGHT = JTextField.RIGHT;
	public static final int CENTER = JTextField.CENTER;

	public static final int TOP = 8;
	public static final int MIDDLE = 9;
	public static final int BASELINE = 10;
	public static final int BOTTOM = 11;

	private Location loc;
	private String text;
	private Font font;
	private Color color;
	private int horzAlign;
	private int vertAlign;
	private boolean dimsKnown;
	private int width;
	private int ascent;
	private int descent;
	private int[] charX;
	private int[] charY;

	public EditableLabel(Location loc, String text, Font font) {
		this.loc = loc;
		this.text = text;
		this.font = font;
		color = Color.BLACK;
		horzAlign = LEFT;
		vertAlign = BASELINE;
		dimsKnown = false;
	}

	@Override
	public EditableLabel clone() {
		try {
			return (EditableLabel) super.clone();
		}
		catch (CloneNotSupportedException e) {
			return new EditableLabel(loc, text, font);
		}
	}

	@Override
	public boolean equals(Object other) {
		return other instanceof EditableLabel that
				&& loc.equals(that.loc)
				&& text.equals(that.text)
				&& font.equals(that.font)
				&& color.equals(that.color)
				&& horzAlign == that.horzAlign
				&& vertAlign == that.vertAlign;
	}

	@Override
	public int hashCode() {
		int ret = loc.x() * 31 + loc.y();
		ret = ret * 31 + text.hashCode();
		ret = ret * 31 + font.hashCode();
		ret = ret * 31 + color.hashCode();
		ret = ret * 31 + horzAlign;
		ret = ret * 31 + vertAlign;
		return ret;
	}

	//
	// accessor methods
	//

	public Location getLocation() { return loc; }
	public void setLocation(Location loc) { this.loc = loc; }

	public String getText() {
		return text;
	}

	public void setText(String value) {
		dimsKnown = false;
		text = value;
	}

	public Font getFont() {
		return font;
	}

	public void setFont(Font value) {
		font = value;
		dimsKnown = false;
	}

	public Color getColor() {
		return color;
	}

	public void setColor(Color value) {
		color = value;
	}

	public int getHorizontalAlignment() {
		return horzAlign;
	}

	public void setHorizontalAlignment(int value) {
		if (value != LEFT && value != CENTER && value != RIGHT)
			throw new IllegalArgumentException("argument must be LEFT, CENTER, or RIGHT");
		horzAlign = value;
		dimsKnown = false;
	}

	public int getVerticalAlignment() {
		return vertAlign;
	}

	public void setVerticalAlignment(int value) {
		if (value != TOP && value != MIDDLE && value != BASELINE && value != BOTTOM)
			throw new IllegalArgumentException("argument must be TOP, MIDDLE, BASELINE, or BOTTOM");
		vertAlign = value;
		dimsKnown = false;
	}

	//
	// more complex methods
	//
	public Bounds getBounds() {
		int x0 = getLeftX();
		int y0 = getBaseY() - ascent;
		int w = width;
		int h = ascent + descent;
		return Bounds.create(x0, y0, w, h);
	}

	public boolean contains(Location query) {
		int x0 = getLeftX();
		int y0 = getBaseY();
		if (query.x() < x0 || query.x() >= x0 + width || query.y() < y0 - ascent || query.y() >= y0 + descent)
			return false;

		if (charX == null || charY == null)
			return true;
		int index = Arrays.binarySearch(charX, query.x() - x0);
		if (index < 0)
			index = -(index + 1);
		if (index >= charX.length)
			return false;
		int asc = (charY[index] >> 16) & 0xFFFF;
		int desc = charY[index] & 0xFFFF;
		int dy = y0 - query.y();
		return dy >= -desc && dy <= asc;

	}

	private int getLeftX() {
		return switch (horzAlign) {
			case CENTER -> loc.x() - width / 2;
			case RIGHT -> loc.x() - width;
			default -> loc.x();
		};
	}

	private int getBaseY() {
		return switch (vertAlign) {
			case TOP -> loc.y() + ascent;
			case MIDDLE -> loc.y() + (ascent - descent) / 2;
			case BOTTOM -> loc.y() - descent;
			default -> loc.y();
		};
	}

	public void configureTextField(EditableLabelField field) {
		configureTextField(field, 1.0);
	}

	public void configureTextField(EditableLabelField field, double zoom) {
		Font f = font;
		if (zoom != 1.0)
			f = f.deriveFont(AffineTransform.getScaleInstance(zoom, zoom));
		field.setFont(f);

		Dimension dim = field.getPreferredSize();
		int w;
		int border = EditableLabelField.FIELD_BORDER;
		if (dimsKnown)
			w = width + 1 + 2 * border;
		else {
			FontMetrics fm = field.getFontMetrics(font);
			ascent = fm.getAscent();
			descent = fm.getDescent();
			w = 0;
		}

		int x0 = loc.x();
		int y0 = getBaseY() - ascent;
		if (zoom != 1.0) {
			x0 = (int) Math.round(x0 * zoom);
			y0 = (int) Math.round(y0 * zoom);
			w = (int) Math.round(w * zoom);
		}

		w = Math.max(w, dim.width);
		int h = dim.height;
		x0 = switch (horzAlign) {
			case CENTER -> x0 - (w / 2) + 1;
			case RIGHT -> x0 - w + border + 1;
			default -> x0 - border;
		};
		y0 = y0 - border;

		field.setHorizontalAlignment(horzAlign);
		field.setForeground(color);
		field.setBounds(x0, y0, w, h);
	}

	public void paint(Graphics g) {
		g.setFont(font);
		if (!dimsKnown)
			computeDimensions(g, font, g.getFontMetrics());
		int x0 = getLeftX();
		int y0 = getBaseY();
		g.setColor(color);
		g.drawString(text, x0, y0);
	}

	private void computeDimensions(Graphics g, Font font, FontMetrics fm) {
		String s = text;
		FontRenderContext frc = ((Graphics2D) g).getFontRenderContext();
		width = fm.stringWidth(s);
		ascent = fm.getAscent();
		descent = fm.getDescent();
		int[] xs = new int[s.length()];
		int[] ys = new int[s.length()];
		for (int i = 0; i < xs.length; i++) {
			xs[i] = fm.stringWidth(s.substring(0, i + 1));
			TextLayout lay = new TextLayout(s.substring(i, i + 1), font, frc);
			Rectangle2D rect = lay.getBounds();
			int asc = (int) Math.ceil(-rect.getMinY());
			int desc = (int) Math.ceil(rect.getMaxY());
			if (asc < 0)
				asc = 0;
			if (asc > 0xFFFF)
				asc = 0xFFFF;
			if (desc < 0)
				desc = 0;
			if (desc > 0xFFFF)
				desc = 0xFFFF;
			ys[i] = (asc << 16) | desc;
		}
		charX = xs;
		charY = ys;
		dimsKnown = true;
	}
}
