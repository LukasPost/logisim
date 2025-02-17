/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package draw.tools;

import java.awt.Color;
import java.awt.Component;
import java.awt.Dimension;
import java.awt.Graphics;

import javax.swing.Icon;

import draw.toolbar.ToolbarItem;

public class ToolbarToolItem implements ToolbarItem {
	private AbstractTool tool;
	private Icon icon;

	public ToolbarToolItem(AbstractTool tool) {
		this.tool = tool;
		icon = tool.getIcon();
	}

	public AbstractTool getTool() {
		return tool;
	}

	public boolean isSelectable() {
		return true;
	}

	public void paintIcon(Component destination, Graphics g) {
		if (icon != null) {
			icon.paintIcon(destination, g, 4, 4);
			return;
		}
		drawErrorIcon(g);
	}

	public static void drawErrorIcon(Graphics g) {
		g.setColor(new Color(255, 128, 128));
		g.fillRect(4, 4, 8, 8);
		g.setColor(Color.BLACK);
		g.drawLine(4, 4, 12, 12);
		g.drawLine(4, 12, 12, 4);
		g.drawRect(4, 4, 8, 8);
	}

	public String getToolTip() {
		return tool.getDescription();
	}

	public Dimension getDimension(Object orientation) {
		return icon == null
				? new Dimension(16, 16)
				: new Dimension(icon.getIconWidth() + 8, icon.getIconHeight() + 8);
	}
}
