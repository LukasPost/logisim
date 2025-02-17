/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.base;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.Font;
import java.awt.Rectangle;

import logisim.comp.TextField;
import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.AttributeSet;
import logisim.data.Attributes;
import logisim.data.Bounds;
import logisim.data.Location;
import logisim.instance.Instance;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.util.GraphicsUtil;

public class Text extends InstanceFactory {
	public static Attribute<String> ATTR_TEXT = Attributes.forString("text", Strings.getter("textTextAttr"));
	public static Attribute<Font> ATTR_FONT = Attributes.forFont("font", Strings.getter("textFontAttr"));
	public static Attribute<AttributeOption> ATTR_HALIGN = Attributes.forOption("halign",
			Strings.getter("textHorzAlignAttr"),
			new AttributeOption[] {
					new AttributeOption(TextField.H_LEFT, "left",
							Strings.getter("textHorzAlignLeftOpt")),
					new AttributeOption(TextField.H_RIGHT, "right",
							Strings.getter("textHorzAlignRightOpt")),
					new AttributeOption(TextField.H_CENTER, "center",
							Strings.getter("textHorzAlignCenterOpt")), });
	public static Attribute<AttributeOption> ATTR_VALIGN = Attributes.forOption("valign",
			Strings.getter("textVertAlignAttr"),
			new AttributeOption[] {
					new AttributeOption(TextField.V_TOP, "top", Strings.getter("textVertAlignTopOpt")),
					new AttributeOption(TextField.V_BASELINE, "base",
							Strings.getter("textVertAlignBaseOpt")),
					new AttributeOption(TextField.V_BOTTOM, "bottom",
							Strings.getter("textVertAlignBottomOpt")),
					new AttributeOption(TextField.H_CENTER, "center",
							Strings.getter("textVertAlignCenterOpt")), });

	public static final Text FACTORY = new Text();

	private Text() {
		super("Text", Strings.getter("textComponent"));
		setIconName("text.gif");
		setShouldSnap(false);
	}

	@Override
	public AttributeSet createAttributeSet() {
		return new TextAttributes();
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrsBase) {
		TextAttributes attrs = (TextAttributes) attrsBase;
		String text = attrs.getText();
		if (text == null || text.isEmpty()) return Bounds.EMPTY_BOUNDS;
		else {
			Bounds bds = attrs.getOffsetBounds();
			if (bds == null) {
				bds = estimateBounds(attrs);
				attrs.setOffsetBounds(bds);
			}
			return bds == null ? Bounds.EMPTY_BOUNDS : bds;
		}
	}

	private Bounds estimateBounds(TextAttributes attrs) {
		// TODO - you can imagine being more clever here
		String text = attrs.getText();
		if (text == null || text.isEmpty())
			return Bounds.EMPTY_BOUNDS;
		int size = attrs.getFont().getSize();
		int w = size * text.length() / 2;
		int ha = attrs.getHorizontalAlign();
		int va = attrs.getVerticalAlign();
		int x;
		int y;
		if (ha == TextField.H_LEFT) x = 0;
		else if (ha == TextField.H_RIGHT) x = -w;
		else x = -w / 2;
		if (va == TextField.V_TOP) y = 0;
		else if (va == TextField.V_CENTER) y = -size / 2;
		else y = -size;
		return Bounds.create(x, y, w, size);
	}

	//
	// graphics methods
	//
	@Override
	public void paintGhost(InstancePainter painter) {
		TextAttributes attrs = (TextAttributes) painter.getAttributeSet();
		String text = attrs.getText();
		if (text == null || text.isEmpty())
			return;

		int halign = attrs.getHorizontalAlign();
		int valign = attrs.getVerticalAlign();
		Graphics g = painter.getGraphics();
		Font old = g.getFont();
		g.setFont(attrs.getFont());
		GraphicsUtil.drawText(g, text, 0, 0, halign, valign);

		String textTrim = text.endsWith(" ") ? text.substring(0, text.length() - 1) : text;
		Bounds newBds;
		if (textTrim.isEmpty()) newBds = Bounds.EMPTY_BOUNDS;
		else {
			Rectangle bdsOut = GraphicsUtil.getTextBounds(g, textTrim, 0, 0, halign, valign);
			newBds = Bounds.create(bdsOut).expand(4);
		}
		if (attrs.setOffsetBounds(newBds)) {
			Instance instance = painter.getInstance();
			if (instance != null)
				instance.recomputeBounds();
		}

		g.setFont(old);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Location loc = painter.getLocation();
		int x = loc.x();
		int y = loc.y();
		Graphics g = painter.getGraphics();
		g.translate(x, y);
		g.setColor(Color.BLACK);
		paintGhost(painter);
		g.translate(-x, -y);
	}

	//
	// methods for instances
	//
	@Override
	protected void configureNewInstance(Instance instance) {
		configureLabel(instance);
		instance.addAttributeListener();
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == ATTR_HALIGN || attr == ATTR_VALIGN) configureLabel(instance);
	}

	private void configureLabel(Instance instance) {
		TextAttributes attrs = (TextAttributes) instance.getAttributeSet();
		Location loc = instance.getLocation();
		instance.setTextField(ATTR_TEXT, ATTR_FONT, loc.x(), loc.y(), attrs.getHorizontalAlign(),
				attrs.getVerticalAlign());
	}

	@Override
	public void propagate(InstanceState state) {
	}
}
