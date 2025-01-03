// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{
    using LogisimPlus.Java;
    using Bounds = logisim.data.Bounds;

	public interface Caret
	{
		// listener methods
		void addCaretListener(CaretListener e);

		void removeCaretListener(CaretListener e);

		// query/JGraphics methods
		string Text {get;}

		Bounds getBounds(JGraphics g);

		void draw(JGraphics g);

		// finishing
		void commitText(string text);

		void cancelEditing();

		void stopEditing();

		// events to handle
		void mousePressed(MouseEvent e);

		void mouseDragged(MouseEvent e);

		void mouseReleased(MouseEvent e);

		void keyPressed(KeyEvent e);

		void keyReleased(KeyEvent e);

		void keyTyped(KeyEvent e);
	}

}
