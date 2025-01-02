// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{
	using logisim.data;
	using AttributeSetTableModel = logisim.gui.generic.AttributeSetTableModel;
	using Project = logisim.proj.Project;
	using Tool = logisim.tools.Tool;

	public class AttrTableToolModel : AttributeSetTableModel
	{
		internal Project proj;
		internal Tool tool;

		public AttrTableToolModel(Project proj, Tool tool) : base(tool.AttributeSet)
		{
			this.proj = proj;
			this.tool = tool;
		}

		public override string Title
		{
			get
			{
				return Strings.get("toolAttrTitle", tool.DisplayName);
			}
		}

		public virtual Tool Tool
		{
			get
			{
				return tool;
			}
		}

		public override void setValueRequested(Attribute<object> attr, object value)
		{
			proj.doAction(ToolAttributeAction.create(tool, attr, value));
		}
	}

}
