/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

import java.util.Collections;
import java.util.List;

class XmlReaderException extends Exception {
	private List<String> messages;

	public XmlReaderException(String message) {
		this(Collections.singletonList(message));
	}

	public XmlReaderException(List<String> messages) {
		this.messages = messages;
	}

	@Override
	public String getMessage() {
		return messages.getFirst();
	}

	public List<String> getMessages() {
		return messages;
	}
}
