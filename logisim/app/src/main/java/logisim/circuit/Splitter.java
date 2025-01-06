/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import javax.swing.JPopupMenu;

import logisim.circuit.CircuitWires.SplitterData;
import logisim.comp.ComponentEvent;
import logisim.comp.ComponentFactory;
import logisim.comp.ComponentDrawContext;
import logisim.comp.ComponentUserEvent;
import logisim.comp.EndData;
import logisim.comp.ManagedComponent;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeSet;
import logisim.data.BitWidth;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.instance.StdAttr;
import logisim.proj.Project;
import logisim.tools.MenuExtender;
import logisim.tools.ToolTipMaker;
import logisim.tools.WireRepair;
import logisim.tools.WireRepairData;
import logisim.util.StringUtil;

public class Splitter extends ManagedComponent implements WireRepair, ToolTipMaker, MenuExtender, AttributeListener {
	// basic data
	byte[] bit_thread; // how each bit maps to thread within end

	// derived data
	SplitterData wire_data;

	public Splitter(Location loc, AttributeSet attrs) {
		super(loc, attrs, 3);
		configureComponent();
		attrs.addAttributeListener(this);
	}

	//
	// abstract ManagedComponent methods
	//
	@Override
	public ComponentFactory getFactory() {
		return SplitterFactory.instance;
	}

	@Override
	public void propagate(CircuitState state) {
		// handled by CircuitWires, nothing to do
	}

	@Override
	public boolean contains(Location loc) {
		if (super.contains(loc)) {
			Location myLoc = getLocation();
			Direction facing = getAttributeSet().getValue(StdAttr.FACING);
			if (facing == Direction.East || facing == Direction.West)
				return Math.abs(loc.x() - myLoc.x()) > 5 || loc.manhattanDistanceTo(myLoc) <= 5;
			else return Math.abs(loc.y() - myLoc.y()) > 5 || loc.manhattanDistanceTo(myLoc) <= 5;
		} else return false;
	}

	private synchronized void configureComponent() {
		SplitterAttributes attrs = (SplitterAttributes) getAttributeSet();
		SplitterParameters parms = attrs.getParameters();
		int fanout = attrs.fanout;
		byte[] bit_end = attrs.bit_end;

		// compute width of each end
		bit_thread = new byte[bit_end.length];
		byte[] end_width = new byte[fanout + 1];
		end_width[0] = (byte) bit_end.length;
		for (int i = 0; i < bit_end.length; i++) {
			byte thr = bit_end[i];
			if (thr > 0) {
				bit_thread[i] = end_width[thr];
				end_width[thr]++;
			} else bit_thread[i] = -1;
		}

		// compute end positions
		Location origin = getLocation();
		int x = origin.x() + parms.getEnd0X();
		int y = origin.y() + parms.getEnd0Y();
		int dx = parms.getEndToEndDeltaX();
		int dy = parms.getEndToEndDeltaY();

		EndData[] ends = new EndData[fanout + 1];
		ends[0] = new EndData(origin, BitWidth.create(bit_end.length), EndData.INPUT_OUTPUT);
		for (int i = 0; i < fanout; i++) {
			ends[i + 1] = new EndData(new Location(x, y), BitWidth.create(end_width[i + 1]), EndData.INPUT_OUTPUT);
			x += dx;
			y += dy;
		}
		wire_data = new SplitterData(fanout);
		setEnds(ends);
		recomputeBounds();
		fireComponentInvalidated(new ComponentEvent(this));
	}

	//
	// user interface methods
	//
	public void draw(ComponentDrawContext context) {
		SplitterAttributes attrs = (SplitterAttributes) getAttributeSet();
		if (attrs.appear == SplitterAttributes.APPEAR_LEGACY) SplitterPainter.drawLegacy(context, attrs, getLocation());
		else {
			Location loc = getLocation();
			SplitterPainter.drawLines(context, attrs, loc);
			SplitterPainter.drawLabels(context, attrs, loc);
			context.drawPins(this);
		}
	}

	@Override
	public Object getFeature(Object key) {
		if (key == WireRepair.class)
			return this;
		if (key == ToolTipMaker.class)
			return this;
		if (key == MenuExtender.class)
			return this;
		else
			return super.getFeature(key);
	}

	public boolean shouldRepairWire(WireRepairData data) {
		return true;
	}

	public String getToolTip(ComponentUserEvent e) {
		int end = -1;
		for (int i = getEnds().size() - 1; i >= 0; i--)
			if (getEndLocation(i).manhattanDistanceTo(e.getX(), e.getY()) < 10) {
				end = i;
				break;
			}

		if (end == 0) return Strings.get("splitterCombinedTip");
		else if (end > 0) {
			int bits = 0;
			StringBuilder buf = new StringBuilder();
			SplitterAttributes attrs = (SplitterAttributes) getAttributeSet();
			byte[] bit_end = attrs.bit_end;
			boolean inString = false;
			int beginString = 0;
			for (int i = 0; i < bit_end.length; i++)
				if (bit_end[i] == end) {
					bits++;
					if (!inString) {
						inString = true;
						beginString = i;
					}
				}
				else if (inString) {
					appendBuf(buf, beginString, i - 1);
					inString = false;
				}
			if (inString)
				appendBuf(buf, beginString, bit_end.length - 1);
			String base = switch (bits) {
				case 0 -> Strings.get("splitterSplit0Tip");
				case 1 -> Strings.get("splitterSplit1Tip");
				default -> Strings.get("splitterSplitManyTip");
			};
			return StringUtil.format(base, buf.toString());
		} else return null;
	}

	private static void appendBuf(StringBuilder buf, int start, int end) {
		if (!buf.isEmpty())
			buf.append(",");
		if (start == end) buf.append(start);
		else buf.append(start).append("-").append(end);
	}

	public void configureMenu(JPopupMenu menu, Project proj) {
		menu.addSeparator();
		menu.add(new SplitterDistributeItem(proj, this, 1));
		menu.add(new SplitterDistributeItem(proj, this, -1));
	}

	//
	// AttributeListener methods
	//
	public void attributeListChanged(AttributeEvent e) {
	}

	public void attributeValueChanged(AttributeEvent e) {
		configureComponent();
	}
}