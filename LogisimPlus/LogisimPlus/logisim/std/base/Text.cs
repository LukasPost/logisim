// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.@base
{

	using TextField = logisim.comp.TextField;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	public class Text : InstanceFactory
	{
		public static Attribute<string> ATTR_TEXT = Attributes.forString("text", Strings.getter("textTextAttr"));
		public static Attribute ATTR_FONT = Attributes.forFont("font", Strings.getter("textFontAttr"));
		public static Attribute ATTR_HALIGN = Attributes.forOption("halign", Strings.getter("textHorzAlignAttr"), new AttributeOption[]
		{
			new AttributeOption(Convert.ToInt32(TextField.H_LEFT), "left", Strings.getter("textHorzAlignLeftOpt")),
			new AttributeOption(Convert.ToInt32(TextField.H_RIGHT), "right", Strings.getter("textHorzAlignRightOpt")),
			new AttributeOption(Convert.ToInt32(TextField.H_CENTER), "center", Strings.getter("textHorzAlignCenterOpt"))
		});
		public static Attribute ATTR_VALIGN = Attributes.forOption("valign", Strings.getter("textVertAlignAttr"), new AttributeOption[]
		{
			new AttributeOption(Convert.ToInt32(TextField.V_TOP), "top", Strings.getter("textVertAlignTopOpt")),
			new AttributeOption(Convert.ToInt32(TextField.V_BASELINE), "base", Strings.getter("textVertAlignBaseOpt")),
			new AttributeOption(Convert.ToInt32(TextField.V_BOTTOM), "bottom", Strings.getter("textVertAlignBottomOpt")),
			new AttributeOption(Convert.ToInt32(TextField.H_CENTER), "center", Strings.getter("textVertAlignCenterOpt"))
		});

		public static readonly Text FACTORY = new Text();

		private Text() : base("Text", Strings.getter("textComponent"))
		{
			IconName = "text.gif";
			ShouldSnap = false;
		}

		public override AttributeSet createAttributeSet()
		{
			return new TextAttributes();
		}

		public override Bounds getOffsetBounds(AttributeSet attrsBase)
		{
			TextAttributes attrs = (TextAttributes) attrsBase;
			string text = attrs.Text;
			if (string.ReferenceEquals(text, null) || text.Equals(""))
			{
				return Bounds.EMPTY_BOUNDS;
			}
			else
			{
				Bounds bds = attrs.OffsetBounds;
				if (bds == null)
				{
					bds = estimateBounds(attrs);
					attrs.OffsetBounds = bds;
				}
				return bds == null ? Bounds.EMPTY_BOUNDS : bds;
			}
		}

		private Bounds estimateBounds(TextAttributes attrs)
		{
			// TODO - you can imagine being more clever here
			string text = attrs.Text;
			if (string.ReferenceEquals(text, null) || text.Length == 0)
			{
				return Bounds.EMPTY_BOUNDS;
			}
			int size = attrs.Font.getSize();
			int h = size;
			int w = size * text.Length / 2;
			int ha = attrs.HorizontalAlign;
			int va = attrs.VerticalAlign;
			int x;
			int y;
			if (ha == TextField.H_LEFT)
			{
				x = 0;
			}
			else if (ha == TextField.H_RIGHT)
			{
				x = -w;
			}
			else
			{
				x = -w / 2;
			}
			if (va == TextField.V_TOP)
			{
				y = 0;
			}
			else if (va == TextField.V_CENTER)
			{
				y = -h / 2;
			}
			else
			{
				y = -h;
			}
			return Bounds.create(x, y, w, h);
		}

		//
		// JGraphics methods
		//
		public override void paintGhost(InstancePainter painter)
		{
			TextAttributes attrs = (TextAttributes) painter.AttributeSet;
			string text = attrs.Text;
			if (string.ReferenceEquals(text, null) || text.Equals(""))
			{
				return;
			}

			int halign = attrs.HorizontalAlign;
			int valign = attrs.VerticalAlign;
			JGraphics g = painter.Graphics;
			Font old = g.getFont();
			g.setFont(attrs.Font);
			JGraphicsUtil.drawText(g, text, 0, 0, halign, valign);

			string textTrim = text.EndsWith(" ", StringComparison.Ordinal) ? text.Substring(0, text.Length - 1) : text;
			Bounds newBds;
			if (textTrim.Equals(""))
			{
				newBds = Bounds.EMPTY_BOUNDS;
			}
			else
			{
				Rectangle bdsOut = JGraphicsUtil.getTextBounds(g, textTrim, 0, 0, halign, valign);
				newBds = Bounds.create(bdsOut).expand(4);
			}
			if (attrs.setOffsetBounds(newBds))
			{
				Instance instance = painter.getInstance();
				if (instance != null)
				{
					instance.recomputeBounds();
				}
			}

			g.setFont(old);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;
			JGraphics g = painter.Graphics;
			g.translate(x, y);
			g.setColor(Color.Black);
			paintGhost(painter);
			g.translate(-x, -y);
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			configureLabel(instance);
			instance.addAttributeListener();
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == ATTR_HALIGN || attr == ATTR_VALIGN)
			{
				configureLabel(instance);
			}
		}

		private void configureLabel(Instance instance)
		{
			TextAttributes attrs = (TextAttributes) instance.AttributeSet;
			Location loc = instance.Location;
			instance.setTextField(ATTR_TEXT, ATTR_FONT, loc.X, loc.Y, attrs.HorizontalAlign, attrs.VerticalAlign);
		}

		public override void propagate(InstanceState state)
		{
		}
	}

}
