/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;

import java.awt.Component;
import java.awt.Window;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.io.IOException;
import java.io.StringReader;
import java.io.StringWriter;
import java.util.NoSuchElementException;
import java.util.StringTokenizer;

import javax.swing.JLabel;

import logisim.circuit.CircuitState;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.BitWidth;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.gui.hex.HexFile;
import logisim.gui.hex.HexFrame;
import logisim.gui.main.Frame;
import logisim.instance.Instance;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.proj.Project;

public class Rom extends Mem {
	public static Attribute<MemContents> CONTENTS_ATTR = new ContentsAttribute();

	// The following is so that instance's MemListeners aren't freed by the
	// garbage collector until the instance itself is ready to be freed.

	public Rom() {
		super("ROM", Strings.getter("romComponent"));
		setIconName("rom.gif");
	}

	@Override
	void configurePorts(Instance instance) {
		Port[] ps = new Port[MEM_INPUTS];
		configureStandardPorts(ps);
		instance.setPorts(ps);
	}

	@Override
	public AttributeSet createAttributeSet() {
		return new RomAttributes();
	}

	@Override
	MemState getState(Instance instance, CircuitState state) {
		MemState ret = (MemState) instance.getData(state);
		if (ret == null) {
			MemContents contents = getMemContents(instance);
			ret = new MemState(contents);
			instance.setData(state, ret);
		}
		return ret;
	}

	@Override
	MemState getState(InstanceState state) {
		MemState ret = (MemState) state.getData();
		if (ret == null) {
			MemContents contents = getMemContents(state.getInstance());
			ret = new MemState(contents);
			state.setData(ret);
		}
		return ret;
	}

	@Override
	HexFrame getHexFrame(Project proj, Instance instance, CircuitState state) {
		return RomAttributes.getHexFrame(getMemContents(instance), proj);
	}

	// TODO - maybe delete this method?
	MemContents getMemContents(Instance instance) {
		return instance.getAttributeValue(CONTENTS_ATTR);
	}

	@Override
	public void propagate(InstanceState state) {
		MemState myState = getState(state);
		BitWidth dataBits = state.getAttributeValue(DATA_ATTR);

		WireValue addrValue = state.getPort(ADDR);
		boolean chipSelect = state.getPort(CS) != WireValues.FALSE;

		if (!chipSelect) {
			myState.setCurrent(-1);
			state.setPort(DATA, WireValue.Companion.createUnknown(dataBits), DELAY);
			return;
		}

		int addr = addrValue.toIntValue();
		if (!addrValue.isFullyDefined() || addr < 0)
			return;
		if (addr != myState.getCurrent()) {
			myState.setCurrent(addr);
			myState.scrollToShow(addr);
		}

		int val = myState.getContents().get(addr);
		state.setPort(DATA, WireValue.Companion.createKnown(dataBits, val), DELAY);
	}

	@Override
	protected void configureNewInstance(Instance instance) {
		super.configureNewInstance(instance);
		MemContents contents = getMemContents(instance);
		MemListener listener = new MemListener(instance);
		contents.addHexModelListener(listener);
	}

	private static class ContentsAttribute extends Attribute<MemContents> {
		public ContentsAttribute() {
			super("contents", Strings.getter("romContentsAttr"));
		}

		@Override
		public Component getCellEditor(Window source, MemContents value) {
			if (source instanceof Frame) {
				Project proj = ((Frame) source).getProject();
				RomAttributes.register(value, proj);
			}
			ContentsCell ret = new ContentsCell(source, value);
			ret.mouseClicked(null);
			return ret;
		}

		@Override
		public String toDisplayString(MemContents value) {
			return Strings.get("romContentsValue");
		}

		@Override
		public String toStandardString(MemContents state) {
			int addr = state.getLogLength();
			int data = state.getWidth();
			StringWriter ret = new StringWriter();
			ret.write("addr/data: " + addr + " " + data + "\n");
			try {
				HexFile.save(ret, state);
			}
			catch (IOException e) {
			}
			return ret.toString();
		}

		@Override
		public MemContents parse(String value) {
			int lineBreak = value.indexOf('\n');
			String first = lineBreak < 0 ? value : value.substring(0, lineBreak);
			String rest = lineBreak < 0 ? "" : value.substring(lineBreak + 1);
			StringTokenizer toks = new StringTokenizer(first);
			try {
				String header = toks.nextToken();
				if (!"addr/data:".equals(header))
					return null;
				int addr = Integer.parseInt(toks.nextToken());
				int data = Integer.parseInt(toks.nextToken());
				MemContents ret = MemContents.create(addr, data);
				HexFile.open(ret, new StringReader(rest));
				return ret;
			}
			catch (IOException | NumberFormatException | NoSuchElementException e) {
				return null;
			}
		}
	}

	private static class ContentsCell extends JLabel implements MouseListener {
		Window source;
		MemContents contents;

		ContentsCell(Window source, MemContents contents) {
			super(Strings.get("romContentsValue"));
			this.source = source;
			this.contents = contents;
			addMouseListener(this);
		}

		public void mouseClicked(MouseEvent e) {
			if (contents == null)
				return;
			Project proj = source instanceof Frame ? ((Frame) source).getProject() : null;
			HexFrame frame = RomAttributes.getHexFrame(contents, proj);
			frame.setVisible(true);
			frame.toFront();
		}

		public void mousePressed(MouseEvent e) {
		}

		public void mouseReleased(MouseEvent e) {
		}

		public void mouseEntered(MouseEvent e) {
		}

		public void mouseExited(MouseEvent e) {
		}
	}
}
