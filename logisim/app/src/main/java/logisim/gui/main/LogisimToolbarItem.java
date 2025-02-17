/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.awt.AlphaComposite;
import java.awt.Component;
import java.awt.Composite;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;

import javax.swing.Icon;

import draw.toolbar.ToolbarItem;
import draw.tools.ToolbarToolItem;
import logisim.gui.menu.LogisimMenuItem;
import logisim.util.Icons;
import logisim.util.StringGetter;

class LogisimToolbarItem implements ToolbarItem {
	private MenuListener menu;
	private Icon icon;
	private LogisimMenuItem action;
	private StringGetter toolTip;

	public LogisimToolbarItem(MenuListener menu, String iconName, LogisimMenuItem action, StringGetter toolTip) {
		this.menu = menu;
		icon = Icons.getIcon(iconName);
		this.action = action;
		this.toolTip = toolTip;
	}

	public void setIcon(String iconName) {
		icon = Icons.getIcon(iconName);
	}

	public void setToolTip(StringGetter toolTip) {
		this.toolTip = toolTip;
	}

	public void doAction() {
		if (menu != null && menu.isEnabled(action)) menu.doAction(action);
	}

	public boolean isSelectable() {
		return menu != null && menu.isEnabled(action);
	}

	public void paintIcon(Component destination, Graphics g) {
		if (!isSelectable() && g instanceof Graphics2D) {
			Composite c = AlphaComposite.getInstance(AlphaComposite.SRC_OVER, 0.3f);
			((Graphics2D) g).setComposite(c);
		}

		if (icon == null) {
			ToolbarToolItem.drawErrorIcon(g);
		} else icon.paintIcon(destination, g, 0, 1);
	}

	public String getToolTip() {
		if (toolTip != null) return toolTip.get();
		else return null;
	}

	public Dimension getDimension(Object orientation) {
		if (icon == null) return new Dimension(16, 16);
		else {
			int w = icon.getIconWidth();
			int h = icon.getIconHeight();
			return new Dimension(w, h + 2);
		}
	}
}
