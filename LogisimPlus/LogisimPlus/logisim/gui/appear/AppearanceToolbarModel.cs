// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{

	using Canvas = draw.canvas.Canvas;
	using AbstractToolbarModel = draw.toolbar.AbstractToolbarModel;
	using ToolbarItem = draw.toolbar.ToolbarItem;
	using AbstractTool = draw.tools.AbstractTool;
	using CurveTool = draw.tools.CurveTool;
	using DrawingAttributeSet = draw.tools.DrawingAttributeSet;
	using LineTool = draw.tools.LineTool;
	using OvalTool = draw.tools.OvalTool;
	using PolyTool = draw.tools.PolyTool;
	using RectangleTool = draw.tools.RectangleTool;
	using RoundRectangleTool = draw.tools.RoundRectangleTool;
	using TextTool = draw.tools.TextTool;
	using ToolbarToolItem = draw.tools.ToolbarToolItem;

	internal class AppearanceToolbarModel : AbstractToolbarModel, PropertyChangeListener
	{
		private Canvas canvas;
		private IList<ToolbarItem> items;

		public AppearanceToolbarModel(AbstractTool selectTool, Canvas canvas, DrawingAttributeSet attrs)
		{
			this.canvas = canvas;

			AbstractTool[] tools = new AbstractTool[] {selectTool, new TextTool(attrs), new LineTool(attrs), new CurveTool(attrs), new PolyTool(false, attrs), new RectangleTool(attrs), new RoundRectangleTool(attrs), new OvalTool(attrs), new PolyTool(true, attrs)};

			List<ToolbarItem> rawItems = new List<ToolbarItem>();
			foreach (AbstractTool tool in tools)
			{
				rawItems.Add(new ToolbarToolItem(tool));
			}
			items = rawItems.AsReadOnly();
			canvas.addPropertyChangeListener(Canvas.TOOL_PROPERTY, this);
		}

		internal virtual AbstractTool FirstTool
		{
			get
			{
				ToolbarToolItem item = (ToolbarToolItem) items[0];
				return item.Tool;
			}
		}

		public override IList<ToolbarItem> Items
		{
			get
			{
				return items;
			}
		}

		public override bool isSelected(ToolbarItem item)
		{
			if (item is ToolbarToolItem)
			{
				AbstractTool tool = ((ToolbarToolItem) item).Tool;
				return canvas != null && tool == canvas.Tool;
			}
			else
			{
				return false;
			}
		}

		public override void itemSelected(ToolbarItem item)
		{
			if (item is ToolbarToolItem)
			{
				AbstractTool tool = ((ToolbarToolItem) item).Tool;
				canvas.Tool = tool;
				fireToolbarAppearanceChanged();
			}
		}

		public virtual void propertyChange(PropertyChangeEvent e)
		{
			string prop = e.getPropertyName();
			if (Canvas.TOOL_PROPERTY.Equals(prop))
			{
				fireToolbarAppearanceChanged();
			}
		}
	}

}
