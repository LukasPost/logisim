// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.util
{

	using Bounds = logisim.data.Bounds;

	public class EditableLabel : ICloneable
	{
		public const int LEFT = JTextField.LEFT;
		public const int RIGHT = JTextField.RIGHT;
		public const int CENTER = JTextField.CENTER;

		public const int TOP = 8;
		public const int MIDDLE = 9;
		public const int BASELINE = 10;
		public const int BOTTOM = 11;

		private int x;
		private int y;
		private string text;
		private Font font;
		private Color color;
		private int horzAlign;
		private int vertAlign;
		private bool dimsKnown;
		private int width;
		private int ascent;
		private int descent;
		private int[] charX;
		private int[] charY;

		public EditableLabel(int x, int y, string text, Font font)
		{
			this.x = x;
			this.y = y;
			this.text = text;
			this.font = font;
			this.color = Color.Black;
			this.horzAlign = LEFT;
			this.vertAlign = BASELINE;
			this.dimsKnown = false;
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}

		public override bool Equals(object other)
		{
			if (other is EditableLabel)
			{
				EditableLabel that = (EditableLabel) other;
				return this.x == that.x && this.y == that.y && this.text.Equals(that.text) && this.font.Equals(that.font) && this.color.Equals(that.color) && this.horzAlign == that.horzAlign && this.vertAlign == that.vertAlign;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			int ret = x * 31 + y;
			ret = ret * 31 + text.GetHashCode();
			ret = ret * 31 + font.GetHashCode();
			ret = ret * 31 + color.GetHashCode();
			ret = ret * 31 + horzAlign;
			ret = ret * 31 + vertAlign;
			return ret;
		}

		//
		// accessor methods
		//
		public virtual int X
		{
			get
			{
				return x;
			}
		}

		public virtual int Y
		{
			get
			{
				return y;
			}
		}

		public virtual void setLocation(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public virtual string Text
		{
			get
			{
				return text;
			}
			set
			{
				dimsKnown = false;
				text = value;
			}
		}


		public virtual Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				dimsKnown = false;
			}
		}


		public virtual Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}


		public virtual int HorizontalAlignment
		{
			get
			{
				return horzAlign;
			}
			set
			{
				if (value != LEFT && value != CENTER && value != RIGHT)
				{
					throw new System.ArgumentException("argument must be LEFT, CENTER, or RIGHT");
				}
				horzAlign = value;
				dimsKnown = false;
			}
		}


		public virtual int VerticalAlignment
		{
			get
			{
				return vertAlign;
			}
			set
			{
				if (value != TOP && value != MIDDLE && value != BASELINE && value != BOTTOM)
				{
					throw new System.ArgumentException("argument must be TOP, MIDDLE, BASELINE, or BOTTOM");
				}
				vertAlign = value;
				dimsKnown = false;
			}
		}


		//
		// more complex methods
		//
		public virtual Bounds Bounds
		{
			get
			{
				int x0 = LeftX;
				int y0 = BaseY - ascent;
				int w = width;
				int h = ascent + descent;
				return Bounds.create(x0, y0, w, h);
			}
		}

		public virtual bool contains(int qx, int qy)
		{
			int x0 = LeftX;
			int y0 = BaseY;
			if (qx >= x0 && qx < x0 + width && qy >= y0 - ascent && qy < y0 + descent)
			{
				int[] xs = charX;
				int[] ys = charY;
				if (xs == null || ys == null)
				{
					return true;
				}
				else
				{
					int i = Array.BinarySearch(xs, qx - x0);
					if (i < 0)
					{
						i = -(i + 1);
					}
					if (i >= xs.Length)
					{
						return false;
					}
					else
					{
						int asc = (ys[i] >> 16) & 0xFFFF;
						int desc = ys[i] & 0xFFFF;
						int dy = y0 - qy;
						return dy >= -desc && dy <= asc;
					}
				}
			}
			else
			{
				return false;
			}
		}

		private int LeftX
		{
			get
			{
				switch (horzAlign)
				{
				case LEFT:
					return x;
				case CENTER:
					return x - width / 2;
				case RIGHT:
					return x - width;
				default:
					return x;
				}
			}
		}

		private int BaseY
		{
			get
			{
				switch (vertAlign)
				{
				case TOP:
					return y + ascent;
				case MIDDLE:
					return y + (ascent - descent) / 2;
				case BASELINE:
					return y;
				case BOTTOM:
					return y - descent;
				default:
					return y;
				}
			}
		}

		public virtual void configureTextField(EditableLabelField field)
		{
			configureTextField(field, 1.0);
		}

		public virtual void configureTextField(EditableLabelField field, double zoom)
		{
			Font f = font;
			if (zoom != 1.0)
			{
				f = f.deriveFont(AffineTransform.getScaleInstance(zoom, zoom));
			}
			field.setFont(f);

			Size dim = field.getPreferredSize();
			int w;
			int border = EditableLabelField.FIELD_BORDER;
			if (dimsKnown)
			{
				w = width + 1 + 2 * border;
			}
			else
			{
				FontMetrics fm = field.getFontMetrics(font);
				ascent = fm.getAscent();
				descent = fm.getDescent();
				w = 0;
			}

			int x0 = x;
			int y0 = BaseY - ascent;
			if (zoom != 1.0)
			{
				x0 = (int) (long)Math.Round(x0 * zoom, MidpointRounding.AwayFromZero);
				y0 = (int) (long)Math.Round(y0 * zoom, MidpointRounding.AwayFromZero);
				w = (int) (long)Math.Round(w * zoom, MidpointRounding.AwayFromZero);
			}

			w = Math.Max(w, dim.Width);
			int h = dim.Height;
			switch (horzAlign)
			{
			case LEFT:
				x0 = x0 - border;
				break;
			case CENTER:
				x0 = x0 - (w / 2) + 1;
				break;
			case RIGHT:
				x0 = x0 - w + border + 1;
				break;
			default:
				x0 = x0 - border;
			break;
			}
			y0 = y0 - border;

			field.setHorizontalAlignment(horzAlign);
			field.setForeground(color);
			field.setBounds(x0, y0, w, h);
		}

		public virtual void paint(JGraphics g)
		{
			g.setFont(font);
			if (!dimsKnown)
			{
				computeSizes(g, font, g.getFontMetrics());
			}
			int x0 = LeftX;
			int y0 = BaseY;
			g.setColor(color);
			g.drawString(text, x0, y0);
		}

		private void computeSizes(JGraphics g, Font font, FontMetrics fm)
		{
			string s = text;
			FontRenderContext frc = g.getFontRenderContext();
			width = fm.stringWidth(s);
			ascent = fm.getAscent();
			descent = fm.getDescent();
			int[] xs = new int[s.Length];
			int[] ys = new int[s.Length];
			for (int i = 0; i < xs.Length; i++)
			{
				xs[i] = fm.stringWidth(s.Substring(0, i + 1));
				TextLayout lay = new TextLayout(s.Substring(i, 1), font, frc);
				Rectangle2D rect = lay.getBounds();
				int asc = (int) Math.Ceiling(-rect.getMinY());
				int desc = (int) Math.Ceiling(rect.getMaxY());
				if (asc < 0)
				{
					asc = 0;
				}
				if (asc > 0xFFFF)
				{
					asc = 0xFFFF;
				}
				if (desc < 0)
				{
					desc = 0;
				}
				if (desc > 0xFFFF)
				{
					desc = 0xFFFF;
				}
				ys[i] = (asc << 16) | desc;
			}
			charX = xs;
			charY = ys;
			dimsKnown = true;
		}
	}

}
