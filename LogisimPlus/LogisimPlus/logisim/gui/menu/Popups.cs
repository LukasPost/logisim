// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{


	using Circuit = logisim.circuit.Circuit;
	using LoadedLibrary = logisim.file.LoadedLibrary;
	using Loader = logisim.file.Loader;
	using LogisimFile = logisim.file.LogisimFile;
	using Frame = logisim.gui.main.Frame;
	using StatisticsDialog = logisim.gui.main.StatisticsDialog;
	using Project = logisim.proj.Project;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class Popups
	{
		private class ProjectPopup : JPopupMenu, ActionListener
		{
			internal Project proj;
			internal JMenuItem add = new JMenuItem(Strings.get("projectAddCircuitItem"));
			internal JMenu load = new JMenu(Strings.get("projectLoadLibraryItem"));
			internal JMenuItem loadBuiltin = new JMenuItem(Strings.get("projectLoadBuiltinItem"));
			internal JMenuItem loadLogisim = new JMenuItem(Strings.get("projectLoadLogisimItem"));
			internal JMenuItem loadJar = new JMenuItem(Strings.get("projectLoadJarItem"));

			internal ProjectPopup(Project proj) : base(Strings.get("projMenu"))
			{
				this.proj = proj;

				load.add(loadBuiltin);
				loadBuiltin.addActionListener(this);
				load.add(loadLogisim);
				loadLogisim.addActionListener(this);
				load.add(loadJar);
				loadJar.addActionListener(this);

				add(add);
				add.addActionListener(this);
				add(load);
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == add)
				{
					ProjectCircuitActions.doAddCircuit(proj);
				}
				else if (src == loadBuiltin)
				{
					ProjectLibraryActions.doLoadBuiltinLibrary(proj);
				}
				else if (src == loadLogisim)
				{
					ProjectLibraryActions.doLoadLogisimLibrary(proj);
				}
				else if (src == loadJar)
				{
					ProjectLibraryActions.doLoadJarLibrary(proj);
				}
			}
		}

		private class LibraryPopup : JPopupMenu, ActionListener
		{
			internal Project proj;
			internal Library lib;
			internal JMenuItem unload = new JMenuItem(Strings.get("projectUnloadLibraryItem"));
			internal JMenuItem reload = new JMenuItem(Strings.get("projectReloadLibraryItem"));

			internal LibraryPopup(Project proj, Library lib, bool is_top) : base(Strings.get("libMenu"))
			{
				this.proj = proj;
				this.lib = lib;

				add(unload);
				unload.addActionListener(this);
				add(reload);
				reload.addActionListener(this);
				unload.setEnabled(is_top);
				reload.setEnabled(is_top && lib is LoadedLibrary);
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == unload)
				{
					ProjectLibraryActions.doUnloadLibrary(proj, lib);
				}
				else if (src == reload)
				{
					Loader loader = proj.LogisimFile.Loader;
					loader.reload((LoadedLibrary) lib);
				}
			}
		}

		private class CircuitPopup : JPopupMenu, ActionListener
		{
			internal Project proj;
			internal Circuit circuit;
			internal JMenuItem analyze = new JMenuItem(Strings.get("projectAnalyzeCircuitItem"));
			internal JMenuItem stats = new JMenuItem(Strings.get("projectGetCircuitStatisticsItem"));
			internal JMenuItem main = new JMenuItem(Strings.get("projectSetAsMainItem"));
			internal JMenuItem remove = new JMenuItem(Strings.get("projectRemoveCircuitItem"));
			internal JMenuItem editLayout = new JMenuItem(Strings.get("projectEditCircuitLayoutItem"));
			internal JMenuItem editAppearance = new JMenuItem(Strings.get("projectEditCircuitAppearanceItem"));

			internal CircuitPopup(Project proj, Circuit circuit) : base(Strings.get("circuitMenu"))
			{
				this.proj = proj;
				this.circuit = circuit;

				add(editLayout);
				editLayout.addActionListener(this);
				add(editAppearance);
				editAppearance.addActionListener(this);
				add(analyze);
				analyze.addActionListener(this);
				add(stats);
				stats.addActionListener(this);
				addSeparator();
				add(main);
				main.addActionListener(this);
				add(remove);
				remove.addActionListener(this);

				bool canChange = proj.LogisimFile.contains(circuit);
				LogisimFile file = proj.LogisimFile;
				if (circuit == proj.CurrentCircuit)
				{
					if (proj.Frame.getEditorView().Equals(Frame.EDIT_APPEARANCE))
					{
						editAppearance.setEnabled(false);
					}
					else
					{
						editLayout.setEnabled(false);
					}
				}
				main.setEnabled(canChange && file.MainCircuit != circuit);
				remove.setEnabled(canChange && file.CircuitCount > 1 && proj.Dependencies.canRemove(circuit));
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object source = e.getSource();
				if (source == editLayout)
				{
					proj.CurrentCircuit = circuit;
					proj.Frame.setEditorView(Frame.EDIT_LAYOUT);
				}
				else if (source == editAppearance)
				{
					proj.CurrentCircuit = circuit;
					proj.Frame.setEditorView(Frame.EDIT_APPEARANCE);
				}
				else if (source == analyze)
				{
					ProjectCircuitActions.doAnalyze(proj, circuit);
				}
				else if (source == stats)
				{
					JFrame frame = (JFrame) SwingUtilities.getRoot(this);
					StatisticsDialog.show(frame, proj.LogisimFile, circuit);
				}
				else if (source == main)
				{
					ProjectCircuitActions.doSetAsMainCircuit(proj, circuit);
				}
				else if (source == remove)
				{
					ProjectCircuitActions.doRemoveCircuit(proj, circuit);
				}
			}
		}

		public static JPopupMenu forCircuit(Project proj, Circuit circ)
		{
			return new CircuitPopup(proj, circ);
		}

		public static JPopupMenu forTool(Project proj, Tool tool)
		{
			return null;
		}

		public static JPopupMenu forProject(Project proj)
		{
			return new ProjectPopup(proj);
		}

		public static JPopupMenu forLibrary(Project proj, Library lib, bool isTop)
		{
			return new LibraryPopup(proj, lib, isTop);
		}

	}

}
