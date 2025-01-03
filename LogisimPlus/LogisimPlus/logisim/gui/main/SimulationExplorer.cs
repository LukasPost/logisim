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
	using CircuitState = logisim.circuit.CircuitState;
	using Simulator = logisim.circuit.Simulator;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
    using LogisimPlus.Java;

    internal class SimulationExplorer : JPanel, ProjectListener, MouseListener
	{
		private Project project;
		private SimulationTreeModel model;
		private JTree tree;

		internal SimulationExplorer(Project proj, MenuListener menu) : base(new BorderLayout())
		{
			this.project = proj;

			SimulationToolbarModel toolbarModel = new SimulationToolbarModel(proj, menu);
			Toolbar toolbar = new Toolbar(toolbarModel);
			add(toolbar, BorderLayout.NORTH);

			model = new SimulationTreeModel(proj.Simulator.CircuitState);
			model.CurrentView = project.CircuitState;
			tree = new JTree(model);
			tree.setCellRenderer(new SimulationTreeRenderer());
			tree.addMouseListener(this);
			tree.setToggleClickCount(3);
			add(new JScrollPane(tree), BorderLayout.CENTER);
			proj.addProjectListener(this);
		}

		//
		// ProjectListener methods
		//
		public virtual void projectChanged(ProjectEvent @event)
		{
			int action = @event.Action;
			if (action == ProjectEvent.ACTION_SET_STATE)
			{
				Simulator sim = project.Simulator;
				CircuitState root = sim.CircuitState;
				if (model.RootState != root)
				{
					model = new SimulationTreeModel(root);
					tree.setModel(model);
				}
				model.CurrentView = project.CircuitState;
				TreePath path = model.mapToPath(project.CircuitState);
				if (path != null)
				{
					tree.scrollPathToVisible(path);
				}
			}
		}

		//
		// MouseListener methods
		//
		//
		// MouseListener methods
		//
		public virtual void mouseEntered(MouseEvent e)
		{
		}

		public virtual void mouseExited(MouseEvent e)
		{
		}

		public virtual void mousePressed(MouseEvent e)
		{
			requestFocus();
			checkForPopup(e);
		}

		public virtual void mouseReleased(MouseEvent e)
		{
			checkForPopup(e);
		}

		private void checkForPopup(MouseEvent e)
		{
			if (e.isPopupTrigger())
			{
				; // do nothing
			}
		}

		public virtual void mouseClicked(MouseEvent e)
		{
			if (e.getClickCount() == 2)
			{
				TreePath path = tree.getPathForLocation(e.getX(), e.getY());
				if (path != null)
				{
					object last = path.getLastPathComponent();
					if (last is SimulationTreeCircuitNode)
					{
						SimulationTreeCircuitNode node;
						node = (SimulationTreeCircuitNode) last;
						project.CircuitState = node.CircuitState;
					}
				}
			}
		}
	}

}
