/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import logisim.circuit.Circuit;
import logisim.comp.ComponentUserEvent;
import logisim.proj.Action;

public interface TextEditable {
	public Caret getTextCaret(ComponentUserEvent event);

	public Action getCommitAction(Circuit circuit, String oldText, String newText);
}
