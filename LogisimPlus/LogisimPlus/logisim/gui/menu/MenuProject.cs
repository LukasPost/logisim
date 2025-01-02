// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{


	using Project = logisim.proj.Project;

	internal class MenuProject : Menu
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
			addCircuit = new MenuItemImpl(this, LogisimMenuBar.ADD_CIRCUIT);
			moveUp = new MenuItemImpl(this, LogisimMenuBar.MOVE_CIRCUIT_UP);
			moveDown = new MenuItemImpl(this, LogisimMenuBar.MOVE_CIRCUIT_DOWN);
			remove = new MenuItemImpl(this, LogisimMenuBar.REMOVE_CIRCUIT);
			setAsMain = new MenuItemImpl(this, LogisimMenuBar.SET_MAIN_CIRCUIT);
			revertAppearance = new MenuItemImpl(this, LogisimMenuBar.REVERT_APPEARANCE);
			layout = new MenuItemImpl(this, LogisimMenuBar.EDIT_LAYOUT);
			appearance = new MenuItemImpl(this, LogisimMenuBar.EDIT_APPEARANCE);
			viewToolbox = new MenuItemImpl(this, LogisimMenuBar.VIEW_TOOLBOX);
			viewSimulation = new MenuItemImpl(this, LogisimMenuBar.VIEW_SIMULATION);
			analyze = new MenuItemImpl(this, LogisimMenuBar.ANALYZE_CIRCUIT);
			stats = new MenuItemImpl(this, LogisimMenuBar.CIRCUIT_STATS);
		}

		private class MyListener : ActionListener
		{
			private readonly MenuProject outerInstance;

			public MyListener(MenuProject outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				Project proj = outerInstance.menubar.Project;
				if (src == outerInstance.loadBuiltin)
				{
					ProjectLibraryActions.doLoadBuiltinLibrary(proj);
				}
				else if (src == outerInstance.loadLogisim)
				{
					ProjectLibraryActions.doLoadLogisimLibrary(proj);
				}
				else if (src == outerInstance.loadJar)
				{
					ProjectLibraryActions.doLoadJarLibrary(proj);
				}
				else if (src == outerInstance.unload)
				{
					ProjectLibraryActions.doUnloadLibraries(proj);
				}
				else if (src == outerInstance.options)
				{
					JFrame frame = proj.getOptionsFrame(true);
					frame.setVisible(true);
				}
			}
		}

		private LogisimMenuBar menubar;
		private MyListener myListener;

		private MenuItemImpl addCircuit;
		private JMenu loadLibrary = new JMenu();
		private JMenuItem loadBuiltin = new JMenuItem();
		private JMenuItem loadLogisim = new JMenuItem();
		private JMenuItem loadJar = new JMenuItem();
		private JMenuItem unload = new JMenuItem();
		private MenuItemImpl moveUp;
		private MenuItemImpl moveDown;
		private MenuItemImpl remove;
		private MenuItemImpl setAsMain;
		private MenuItemImpl revertAppearance;
		private MenuItemImpl layout;
		private MenuItemImpl appearance;
		private MenuItemImpl viewToolbox;
		private MenuItemImpl viewSimulation;
		private MenuItemImpl analyze;
		private MenuItemImpl stats;
		private JMenuItem options = new JMenuItem();

		internal MenuProject(LogisimMenuBar menubar)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.menubar = menubar;

			menubar.registerItem(LogisimMenuBar.ADD_CIRCUIT, addCircuit);
			loadBuiltin.addActionListener(myListener);
			loadLogisim.addActionListener(myListener);
			loadJar.addActionListener(myListener);
			unload.addActionListener(myListener);
			menubar.registerItem(LogisimMenuBar.MOVE_CIRCUIT_UP, moveUp);
			menubar.registerItem(LogisimMenuBar.MOVE_CIRCUIT_DOWN, moveDown);
			menubar.registerItem(LogisimMenuBar.SET_MAIN_CIRCUIT, setAsMain);
			menubar.registerItem(LogisimMenuBar.REMOVE_CIRCUIT, remove);
			menubar.registerItem(LogisimMenuBar.REVERT_APPEARANCE, revertAppearance);
			menubar.registerItem(LogisimMenuBar.EDIT_LAYOUT, layout);
			menubar.registerItem(LogisimMenuBar.EDIT_APPEARANCE, appearance);
			menubar.registerItem(LogisimMenuBar.VIEW_TOOLBOX, viewToolbox);
			menubar.registerItem(LogisimMenuBar.VIEW_SIMULATION, viewSimulation);
			menubar.registerItem(LogisimMenuBar.ANALYZE_CIRCUIT, analyze);
			menubar.registerItem(LogisimMenuBar.CIRCUIT_STATS, stats);
			options.addActionListener(myListener);

			loadLibrary.add(loadBuiltin);
			loadLibrary.add(loadLogisim);
			loadLibrary.add(loadJar);

			add(addCircuit);
			add(loadLibrary);
			add(unload);
			addSeparator();
			add(moveUp);
			add(moveDown);
			add(setAsMain);
			add(remove);
			add(revertAppearance);
			addSeparator();
			add(viewToolbox);
			add(viewSimulation);
			add(layout);
			add(appearance);
			addSeparator();
			add(analyze);
			add(stats);
			addSeparator();
			add(options);

			bool known = menubar.Project != null;
			loadLibrary.setEnabled(known);
			loadBuiltin.setEnabled(known);
			loadLogisim.setEnabled(known);
			loadJar.setEnabled(known);
			unload.setEnabled(known);
			options.setEnabled(known);
			computeEnabled();
		}

		public virtual void localeChanged()
		{
			setText(Strings.get("projectMenu"));
			addCircuit.setText(Strings.get("projectAddCircuitItem"));
			loadLibrary.setText(Strings.get("projectLoadLibraryItem"));
			loadBuiltin.setText(Strings.get("projectLoadBuiltinItem"));
			loadLogisim.setText(Strings.get("projectLoadLogisimItem"));
			loadJar.setText(Strings.get("projectLoadJarItem"));
			unload.setText(Strings.get("projectUnloadLibrariesItem"));
			moveUp.setText(Strings.get("projectMoveCircuitUpItem"));
			moveDown.setText(Strings.get("projectMoveCircuitDownItem"));
			setAsMain.setText(Strings.get("projectSetAsMainItem"));
			remove.setText(Strings.get("projectRemoveCircuitItem"));
			revertAppearance.setText(Strings.get("projectRevertAppearanceItem"));
			layout.setText(Strings.get("projectEditCircuitLayoutItem"));
			appearance.setText(Strings.get("projectEditCircuitAppearanceItem"));
			viewToolbox.setText(Strings.get("projectViewToolboxItem"));
			viewSimulation.setText(Strings.get("projectViewSimulationItem"));
			analyze.setText(Strings.get("projectAnalyzeCircuitItem"));
			stats.setText(Strings.get("projectGetCircuitStatisticsItem"));
			options.setText(Strings.get("projectOptionsItem"));
		}

		internal override void computeEnabled()
		{
			setEnabled(menubar.Project != null || addCircuit.hasListeners() || moveUp.hasListeners() || moveDown.hasListeners() || setAsMain.hasListeners() || remove.hasListeners() || layout.hasListeners() || revertAppearance.hasListeners() || appearance.hasListeners() || viewToolbox.hasListeners() || viewSimulation.hasListeners() || analyze.hasListeners() || stats.hasListeners());
			menubar.fireEnableChanged();
		}
	}

}
