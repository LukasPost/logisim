/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import logisim.circuit.Circuit;
import logisim.comp.ComponentUserEvent;
import logisim.proj.Action;

public interface TextEditable {
	Caret getTextCaret(ComponentUserEvent event);

	Action getCommitAction(Circuit circuit, String oldText, String newText);
}
