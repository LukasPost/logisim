// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.shapes
{

	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;

	using CanvasObject = draw.model.CanvasObject;
	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using Handle = draw.model.Handle;
	using HandleGesture = draw.model.HandleGesture;
	using EditableLabel = draw.util.EditableLabel;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using logisim.util;

	public class Text : AbstractCanvasObject
	{
		private EditableLabel label;

		public Text(int x, int y, string text) : this(x, y, EditableLabel.LEFT, EditableLabel.BASELINE, text, DrawAttr.DEFAULT_FONT, Color.Black)
		{
		}

		private Text(int x, int y, int halign, int valign, string text, Font font, Color color)
		{
			label = new EditableLabel(x, y, text, font);
			label.Color = color;
			label.HorizontalAlignment = halign;
			label.VerticalAlignment = valign;
		}

		public override Text clone()
		{
			Text ret = (Text) base.clone();
			ret.label = this.label.clone();
			return ret;
		}

		public override bool matches(CanvasObject other)
		{
			if (other is Text)
			{
				Text that = (Text) other;
				return this.label.Equals(that.label);
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			return label.GetHashCode();
		}

		public override Element toSvgElement(Document doc)
		{
			return SvgCreator.createText(doc, this);
		}

		public virtual Location Location
		{
			get
			{
				return new Location(label.X, label.Y);
			}
		}

		public virtual string Text
		{
			get
			{
				return label.Text;
			}
			set
			{
				label.Text = value;
			}
		}

		public virtual EditableLabel Label
		{
			get
			{
				return label;
			}
		}


		public override string DisplayName
		{
			get
			{
				return Strings.get("shapeText");
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return DrawAttr.ATTRS_TEXT;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public virtual object getValue(Attribute attr)
		{
			if (attr == DrawAttr.FONT)
			{
				return label.Font;
			}
			else if (attr == DrawAttr.FILL_COLOR)
			{
				return label.Color;
			}
			else if (attr == DrawAttr.ALIGNMENT)
			{
				int halign = label.HorizontalAlignment;
				AttributeOption h;
				if (halign == EditableLabel.LEFT)
				{
					h = DrawAttr.ALIGN_LEFT;
				}
				else if (halign == EditableLabel.RIGHT)
				{
					h = DrawAttr.ALIGN_RIGHT;
				}
				else
				{
					h = DrawAttr.ALIGN_CENTER;
				}
				return h;
			}
			else
			{
				return null;
			}
		}

        protected internal override void updateValue(Attribute attr, object value)
		{
			if (attr == DrawAttr.FONT)
			{
				label.Font = (Font) value;
			}
			else if (attr == DrawAttr.FILL_COLOR)
			{
				label.Color = (Color) value;
			}
			else if (attr == DrawAttr.ALIGNMENT)
			{
				int? intVal = (int?)((AttributeOption) value).getValue();
				label.HorizontalAlignment = intVal.Value;
			}
		}

		public override Bounds Bounds
		{
			get
			{
				return label.Bounds;
			}
		}

		public override bool contains(Location loc, bool assumeFilled)
		{
			return label.contains(loc.X, loc.Y);
		}

		public override void translate(int dx, int dy)
		{
			label.setLocation(label.X + dx, label.Y + dy);
		}

		public virtual List<Handle> Handles
		{
			get
			{
				Bounds bds = label.Bounds;
				int x = bds.X;
				int y = bds.Y;
				int w = bds.Width;
				int h = bds.Height;
				return UnmodifiableList.create(new Handle[]
				{
					new Handle(this, x, y),
					new Handle(this, x + w, y),
					new Handle(this, x + w, y + h),
					new Handle(this, x, y + h)
				});
			}
		}

		public override List<Handle> getHandles(HandleGesture gesture)
		{
			return Handles;
		}

		public override void paint(JGraphics g, HandleGesture gesture)
		{
			label.paint(g);
		}
	}

}
