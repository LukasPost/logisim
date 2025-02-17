/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.util;

import java.awt.Component;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.net.URL;

import javax.swing.Icon;
import javax.swing.ImageIcon;

import logisim.data.Direction;

public class Icons {
	private static final String path = "logisim/icons";

	private Icons() {
	}

	public static Icon getIcon(String name) {
		URL url = Icons.class.getClassLoader().getResource(path + "/" + name);
		if (url == null)
			return null;
		return new ImageIcon(url);
	}

	public static void paintRotated(Graphics g, int x, int y, Direction dir, Icon icon, Component dest) {
		if (!(g instanceof Graphics2D) || dir == Direction.East) {
			icon.paintIcon(dest, g, x, y);
			return;
		}

		Graphics2D g2 = (Graphics2D) g.create();
		double cx = x + icon.getIconWidth() / 2.0;
		double cy = y + icon.getIconHeight() / 2.0;
		if (dir == Direction.West) g2.rotate(Math.PI, cx, cy);
		else if (dir == Direction.North) g2.rotate(-Math.PI / 2.0, cx, cy);
		else if (dir == Direction.South) g2.rotate(Math.PI / 2.0, cx, cy);
		else g2.translate(-x, -y);
		icon.paintIcon(dest, g2, x, y);
		g2.dispose();
	}
}
