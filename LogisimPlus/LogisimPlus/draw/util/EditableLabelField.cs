// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

using LogisimPlus.Java;

namespace draw.util
{


	public class EditableLabelField : JTextField
	{
		internal const int FIELD_BORDER = 2;

		public EditableLabelField() : base(10)
		{
			setBackground(Color.FromArgb(255, 255, 255, 255, 128));
			setOpaque(false);
			setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Color.Black), BorderFactory.createEmptyBorder(1, 1, 1, 1)));
		}

		protected internal override void paintComponent(JGraphics g)
		{
			g.setColor(getBackground());
			g.fillRect(0, 0, getWidth(), getHeight());
			base.paintComponent(g);
		}
	}

}
