// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{


	using Toolbar = draw.toolbar.Toolbar;
	using Project = logisim.proj.Project;
	using Tool = logisim.tools.Tool;

	internal class Toolbox : JPanel
	{
		private ProjectExplorer toolbox;

		internal Toolbox(Project proj, MenuListener menu) : base(new BorderLayout())
		{

			ToolboxToolbarModel toolbarModel = new ToolboxToolbarModel(menu);
			Toolbar toolbar = new Toolbar(toolbarModel);
			add(toolbar, BorderLayout.NORTH);

			toolbox = new ProjectExplorer(proj);
			toolbox.Listener = new ToolboxManip(proj, toolbox);
			add(new JScrollPane(toolbox), BorderLayout.CENTER);
		}

		internal virtual Tool HaloedTool
		{
			set
			{
				toolbox.HaloedTool = value;
			}
		}
	}

}
