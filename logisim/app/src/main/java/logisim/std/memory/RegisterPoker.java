/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;

import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.instance.InstancePainter;
import logisim.instance.InstancePoker;
import logisim.instance.InstanceState;
import logisim.instance.StdAttr;

public class RegisterPoker extends InstancePoker {
	private int curValue;

	@Override
	public boolean init(InstanceState state, MouseEvent e) {
		RegisterData data = (RegisterData) state.getData();
		if (data == null) {
			data = new RegisterData();
			state.setData(data);
		}
		curValue = data.value;
		return true;
	}

	@Override
	public void paint(InstancePainter painter) {
		Bounds bds = painter.getBounds();
		BitWidth dataWidth = painter.getAttributeValue(StdAttr.WIDTH);
		int width = dataWidth == null ? 8 : dataWidth.getWidth();
		int len = (width + 3) / 4;

		Graphics g = painter.getGraphics();
		g.setColor(Color.RED);
		if (len > 4) g.drawRect(bds.getX(), bds.getY() + 3, bds.getWidth(), 25);
		else {
			int wid = 7 * len + 2;
			g.drawRect(bds.getX() + (bds.getWidth() - wid) / 2, bds.getY() + 4, wid, 15);
		}
		g.setColor(Color.BLACK);
	}

	@Override
	public void keyTyped(InstanceState state, KeyEvent e) {
		int val = Character.digit(e.getKeyChar(), 16);
		if (val < 0)
			return;

		BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
		if (dataWidth == null)
			dataWidth = BitWidth.create(8);
		curValue = (curValue * 16 + val) & dataWidth.getMask();
		RegisterData data = (RegisterData) state.getData();
		data.value = curValue;

		state.fireInvalidated();
	}
}
