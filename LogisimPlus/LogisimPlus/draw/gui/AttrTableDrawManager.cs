// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.gui
{

	using Canvas = draw.canvas.Canvas;
	using AbstractTool = draw.tools.AbstractTool;
	using DrawingAttributeSet = draw.tools.DrawingAttributeSet;
	using SelectTool = draw.tools.SelectTool;
	using AttrTable = logisim.gui.generic.AttrTable;

	public class AttrTableDrawManager : PropertyChangeListener
	{
		private Canvas canvas;
		private AttrTable table;
		private AttrTableSelectionModel selectionModel;
		private AttrTableToolModel toolModel;

		public AttrTableDrawManager(Canvas canvas, AttrTable table, DrawingAttributeSet attrs)
		{
			this.canvas = canvas;
			this.table = table;
			this.selectionModel = new AttrTableSelectionModel(canvas);
			this.toolModel = new AttrTableToolModel(attrs, null);

			canvas.addPropertyChangeListener(Canvas.TOOL_PROPERTY, this);
			updateToolAttributes();
		}

		public virtual void attributesSelected()
		{
			updateToolAttributes();
		}

		//
		// PropertyChangeListener method
		//
		public virtual void propertyChange(PropertyChangeEvent evt)
		{
			string prop = evt.getPropertyName();
			if (prop.Equals(Canvas.TOOL_PROPERTY))
			{
				updateToolAttributes();
			}
		}

		private void updateToolAttributes()
		{
			object tool = canvas.Tool;
			if (tool is SelectTool)
			{
				table.AttrTableModel = selectionModel;
			}
			else if (tool is AbstractTool)
			{
				toolModel.Tool = (AbstractTool) tool;
				table.AttrTableModel = toolModel;
			}
			else
			{
				table.AttrTableModel = null;
			}
		}
	}

}
