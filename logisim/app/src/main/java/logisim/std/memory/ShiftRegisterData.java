/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;

import java.util.Arrays;

import logisim.data.BitWidth;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.InstanceData;

class ShiftRegisterData extends ClockState implements InstanceData {
	private BitWidth width;
	private WireValue[] vs;
	private int vsPos;

	public ShiftRegisterData(BitWidth width, int len) {
		this.width = width;
		vs = new WireValue[len];
		Arrays.fill(vs, WireValue.Companion.createKnown(width, 0));
		vsPos = 0;
	}

	@Override
	public ShiftRegisterData clone() {
		ShiftRegisterData ret = (ShiftRegisterData) super.clone();
		ret.vs = vs.clone();
		return ret;
	}

	public int getLength() {
		return vs.length;
	}

	public void setDimensions(BitWidth newWidth, int newLength) {
		WireValue[] v = vs;
		BitWidth oldWidth = width;
		int oldW = oldWidth.getWidth();
		int newW = newWidth.getWidth();
		if (v.length != newLength) {
			WireValue[] newV = new WireValue[newLength];
			int j = vsPos;
			int copy = Math.min(newLength, v.length);
			for (int i = 0; i < copy; i++) {
				newV[i] = v[j];
				j++;
				if (j == v.length)
					j = 0;
			}
			Arrays.fill(newV, copy, newLength, WireValue.Companion.createKnown(newWidth, 0));
			v = newV;
			vsPos = 0;
			vs = newV;
		}
		if (oldW != newW) {
			for (int i = 0; i < v.length; i++) {
				WireValue vi = v[i];
				if (vi.getWidth() != newW) v[i] = vi.extendWidth(newW, WireValues.FALSE);
			}
			width = newWidth;
		}
	}

	public void clear() {
		Arrays.fill(vs, WireValue.Companion.createKnown(width, 0));
		vsPos = 0;
	}

	public void push(WireValue v) {
		int pos = vsPos;
		vs[pos] = v;
		vsPos = pos >= vs.length - 1 ? 0 : pos + 1;
	}

	public WireValue get(int index) {
		int i = vsPos + index;
		WireValue[] v = vs;
		if (i >= v.length)
			i -= v.length;
		return v[i];
	}

	public void set(int index, WireValue val) {
		int i = vsPos + index;
		WireValue[] v = vs;
		if (i >= v.length)
			i -= v.length;
		v[i] = val;
	}
}