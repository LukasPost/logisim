// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{
	using Circuit = logisim.circuit.Circuit;
	using ComponentUserEvent = logisim.comp.ComponentUserEvent;
	using Action = logisim.proj.Action;

	public interface TextEditable
	{
		Caret getTextCaret(ComponentUserEvent @event);

		Action getCommitAction(Circuit circuit, string oldText, string newText);
	}

}
