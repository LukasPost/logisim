/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package hex;

public interface HexModelListener {
	void metainfoChanged(HexModel source);

	void bytesChanged(HexModel source, long start, long numBytes, int[] oldValues);
}
