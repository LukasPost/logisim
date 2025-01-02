// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{

	using Bounds = logisim.data.Bounds;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class TextField
	{
		public const int H_LEFT = GraphicsUtil.H_LEFT;
		public const int H_CENTER = GraphicsUtil.H_CENTER;
		public const int H_RIGHT = GraphicsUtil.H_RIGHT;
		public const int V_TOP = GraphicsUtil.V_TOP;
		public const int V_CENTER = GraphicsUtil.V_CENTER;
		public const int V_CENTER_OVERALL = GraphicsUtil.V_CENTER_OVERALL;
		public const int V_BASELINE = GraphicsUtil.V_BASELINE;
		public const int V_BOTTOM = GraphicsUtil.V_BOTTOM;

		private int x;
		private int y;
		private int halign;
		private int valign;
		private Font font;
		private string text = "";
		private LinkedList<TextFieldListener> listeners = new LinkedList<TextFieldListener>();

		public TextField(int x, int y, int halign, int valign) : this(x, y, halign, valign, null)
		{
		}

		public TextField(int x, int y, int halign, int valign, Font font)
		{
			this.x = x;
			this.y = y;
			this.halign = halign;
			this.valign = valign;
			this.font = font;
		}

		//
		// listener methods
		//
		public virtual void addTextFieldListener(TextFieldListener l)
		{
			listeners.AddLast(l);
		}

		public virtual void removeTextFieldListener(TextFieldListener l)
		{
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
			listeners.remove(l);
		}

		public virtual void fireTextChanged(TextFieldEvent e)
		{
			foreach (TextFieldListener l in new List<TextFieldListener>(listeners))
			{
				l.textChanged(e);
			}
		}

		//
		// access methods
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

		public virtual int HAlign
		{
			get
			{
				return halign;
			}
		}

		public virtual int VAlign
		{
			get
			{
				return valign;
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
				this.font = value;
			}
		}

		public virtual string Text
		{
			get
			{
				return text;
			}
			set
			{
				if (!value.Equals(this.text))
				{
					TextFieldEvent e = new TextFieldEvent(this, this.text, value);
					this.text = value;
					fireTextChanged(e);
				}
			}
		}

		public virtual TextFieldCaret getCaret(Graphics g, int pos)
		{
			return new TextFieldCaret(this, g, pos);
		}


		public virtual void setLocation(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public virtual void setLocation(int x, int y, int halign, int valign)
		{
			this.x = x;
			this.y = y;
			this.halign = halign;
			this.valign = valign;
		}

		public virtual void setAlign(int halign, int valign)
		{
			this.halign = halign;
			this.valign = valign;
		}

		public virtual int HorzAlign
		{
			set
			{
				this.halign = value;
			}
		}

		public virtual int VertAlign
		{
			set
			{
				this.valign = value;
			}
		}


		//
		// graphics methods
		//
		public virtual TextFieldCaret getCaret(Graphics g, int x, int y)
		{
			return new TextFieldCaret(this, g, x, y);
		}

		public virtual Bounds getBounds(Graphics g)
		{
			int x = this.x;
			int y = this.y;
			FontMetrics fm;
			if (font == null)
			{
				fm = g.getFontMetrics();
			}
			else
			{
				fm = g.getFontMetrics(font);
			}
			int width = fm.stringWidth(text);
			int ascent = fm.getAscent();
			int descent = fm.getDescent();
			switch (halign)
			{
			case TextField.H_CENTER:
				x -= width / 2;
				break;
			case TextField.H_RIGHT:
				x -= width;
				break;
			default:
				break;
			}
			switch (valign)
			{
			case TextField.V_TOP:
				y += ascent;
				break;
			case TextField.V_CENTER:
				y += ascent / 2;
				break;
			case TextField.V_CENTER_OVERALL:
				y += (ascent - descent) / 2;
				break;
			case TextField.V_BOTTOM:
				y -= descent;
				break;
			default:
				break;
			}
			return Bounds.create(x, y - ascent, width, ascent + descent);
		}

		public virtual void draw(Graphics g)
		{
			Font old = g.getFont();
			if (font != null)
			{
				g.setFont(font);
			}

			int x = this.x;
			int y = this.y;
			FontMetrics fm = g.getFontMetrics();
			int width = fm.stringWidth(text);
			int ascent = fm.getAscent();
			int descent = fm.getDescent();
			switch (halign)
			{
			case TextField.H_CENTER:
				x -= width / 2;
				break;
			case TextField.H_RIGHT:
				x -= width;
				break;
			default:
				break;
			}
			switch (valign)
			{
			case TextField.V_TOP:
				y += ascent;
				break;
			case TextField.V_CENTER:
				y += ascent / 2;
				break;
			case TextField.V_CENTER_OVERALL:
				y += (ascent - descent) / 2;
				break;
			case TextField.V_BOTTOM:
				y -= descent;
				break;
			default:
				break;
			}
			g.drawString(text, x, y);
			g.setFont(old);
		}

	}

}
