/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.awt.BorderLayout;

import javax.swing.JPanel;
import javax.swing.JScrollPane;

import draw.toolbar.Toolbar;
import logisim.proj.Project;
import logisim.tools.Tool;

class Toolbox extends JPanel {
	private ProjectExplorer toolbox;
	
	Toolbox(Project proj, MenuListener menu) {
		super(new BorderLayout());
		
		ToolboxToolbarModel toolbarModel = new ToolboxToolbarModel(menu);
		Toolbar toolbar = new Toolbar(toolbarModel);
		add(toolbar, BorderLayout.NORTH);
		
		toolbox = new ProjectExplorer(proj);
		toolbox.setListener(new ToolboxManip(proj, toolbox));
		add(new JScrollPane(toolbox), BorderLayout.CENTER);
	}
	
	void setHaloedTool(Tool value) {
		toolbox.setHaloedTool(value);
	}
}
