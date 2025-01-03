// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.gui
{
	using AbstractTool = draw.tools.AbstractTool;
	using DrawingAttributeSet = draw.tools.DrawingAttributeSet;
	using logisim.data;
	using AttrTableSetException = logisim.gui.generic.AttrTableSetException;
	using AttributeSetTableModel = logisim.gui.generic.AttributeSetTableModel;

	internal class AttrTableToolModel : AttributeSetTableModel
	{
		private DrawingAttributeSet defaults;
		private AbstractTool currentTool;

		public AttrTableToolModel(DrawingAttributeSet defaults, AbstractTool tool) : base(defaults.createSubset(tool))
		{
			this.defaults = defaults;
			this.currentTool = tool;
		}

		public virtual AbstractTool Tool
		{
			set
			{
				currentTool = value;
				AttributeSet = defaults.createSubset(value);
				fireTitleChanged();
			}
		}

		public override string Title
		{
			get
			{
				return currentTool.Description;
			}
		}

        // JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        // ORIGINAL LINE: @Override public void setValueRequested(logisim.data.Attribute attr, Object value) throws logisim.gui.generic.AttrTableSetException
        protected internal override void setValueRequested(Attribute attr, object value)
		{
			defaults.setValue(attr, value);
		}
	}

}
