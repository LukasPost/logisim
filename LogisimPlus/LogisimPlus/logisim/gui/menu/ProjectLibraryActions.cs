// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{


	using Loader = logisim.file.Loader;
	using LogisimFile = logisim.file.LogisimFile;
	using LogisimFileActions = logisim.file.LogisimFileActions;
	using Project = logisim.proj.Project;
	using Library = logisim.tools.Library;

	public class ProjectLibraryActions
	{
		private ProjectLibraryActions()
		{
		}

		private class BuiltinOption
		{
			internal Library lib;

			internal BuiltinOption(Library lib)
			{
				this.lib = lib;
			}

			public override string ToString()
			{
				return lib.DisplayName;
			}
		}

		private class LibraryJList : JList<BuiltinOption>
		{
			internal LibraryJList(IList<Library> libraries)
			{
				BuiltinOption[] options = new BuiltinOption[libraries.Count];
				for (int i = 0; i < libraries.Count; i++)
				{
					options[i] = new BuiltinOption(libraries[i]);
				}
				setListData(options);
			}

			internal virtual Library[] SelectedLibraries
			{
				get
				{
					var selected = getSelectedValuesList();
					if (selected != null && selected.size() > 0)
					{
						Library[] libs = new Library[selected.size()];
						for (int i = 0; i < selected.size(); i++)
						{
							libs[i] = selected.get(i).lib;
						}
						return libs;
					}
					else
					{
						return new Library[0];
					}
				}
			}
		}

		public static void doLoadBuiltinLibrary(Project proj)
		{
			LogisimFile file = proj.LogisimFile;
			IList<Library> baseBuilt = file.Loader.Builtin.Libraries;
			List<Library> builtins = new List<Library>(baseBuilt);
			builtins.RemoveAll(file.Libraries);
			if (builtins.Count == 0)
			{
				JOptionPane.showMessageDialog(proj.Frame, Strings.get("loadBuiltinNoneError"), Strings.get("loadBuiltinErrorTitle"), JOptionPane.INFORMATION_MESSAGE);
				return;
			}
			LibraryJList list = new LibraryJList(builtins);
			JScrollPane listPane = new JScrollPane(list);
			int action = JOptionPane.showConfirmDialog(proj.Frame, listPane, Strings.get("loadBuiltinDialogTitle"), JOptionPane.OK_CANCEL_OPTION, JOptionPane.QUESTION_MESSAGE);
			if (action == JOptionPane.OK_OPTION)
			{
				Library[] libs = list.SelectedLibraries;
				if (libs != null)
				{
					proj.doAction(LogisimFileActions.loadLibraries(libs));
				}
			}
		}

		public static void doLoadLogisimLibrary(Project proj)
		{
			Loader loader = proj.LogisimFile.Loader;
			JFileChooser chooser = loader.createChooser();
			chooser.setDialogTitle(Strings.get("loadLogisimDialogTitle"));
			chooser.setFileFilter(Loader.LOGISIM_FILTER);
			int check = chooser.showOpenDialog(proj.Frame);
			if (check == JFileChooser.APPROVE_OPTION)
			{
				File f = chooser.getSelectedFile();
				Library lib = loader.loadLogisimLibrary(f);
				if (lib != null)
				{
					proj.doAction(LogisimFileActions.loadLibrary(lib));
				}
			}
		}

		public static void doLoadJarLibrary(Project proj)
		{
			Loader loader = proj.LogisimFile.Loader;
			JFileChooser chooser = loader.createChooser();
			chooser.setDialogTitle(Strings.get("loadJarDialogTitle"));
			chooser.setFileFilter(Loader.JAR_FILTER);
			int check = chooser.showOpenDialog(proj.Frame);
			if (check == JFileChooser.APPROVE_OPTION)
			{
				File f = chooser.getSelectedFile();
				string className = null;

				// try to retrieve the class name from the "Library-Class"
				// attribute in the manifest. This section of code was contributed
				// by Christophe Jacquet (Request Tracker #2024431).
				JarFile jarFile = null;
				try
				{
					jarFile = new JarFile(f);
					Manifest manifest = jarFile.getManifest();
					className = manifest.getMainAttributes().getValue("Library-Class");
				}
				catch (IOException)
				{
					// if opening the JAR file failed, do nothing
				}
				finally
				{
					if (jarFile != null)
					{
						try
						{
							jarFile.close();
						}
						catch (IOException)
						{
						}
					}
				}

				// if the class name was not found, go back to the good old dialog
				if (string.ReferenceEquals(className, null))
				{
					className = JOptionPane.showInputDialog(proj.Frame, Strings.get("jarClassNamePrompt"), Strings.get("jarClassNameTitle"), JOptionPane.QUESTION_MESSAGE);
					// if user canceled selection, abort
					if (string.ReferenceEquals(className, null))
					{
						return;
					}
				}

				Library lib = loader.loadJarLibrary(f, className);
				if (lib != null)
				{
					proj.doAction(LogisimFileActions.loadLibrary(lib));
				}
			}
		}

		public static void doUnloadLibraries(Project proj)
		{
			LogisimFile file = proj.LogisimFile;
			List<Library> canUnload = new List<Library>();
			foreach (Library lib in file.Libraries)
			{
				string message = file.getUnloadLibraryMessage(lib);
				if (string.ReferenceEquals(message, null))
				{
					canUnload.Add(lib);
				}
			}
			if (canUnload.Count == 0)
			{
				JOptionPane.showMessageDialog(proj.Frame, Strings.get("unloadNoneError"), Strings.get("unloadErrorTitle"), JOptionPane.INFORMATION_MESSAGE);
				return;
			}
			LibraryJList list = new LibraryJList(canUnload);
			JScrollPane listPane = new JScrollPane(list);
			int action = JOptionPane.showConfirmDialog(proj.Frame, listPane, Strings.get("unloadLibrariesDialogTitle"), JOptionPane.OK_CANCEL_OPTION, JOptionPane.QUESTION_MESSAGE);
			if (action == JOptionPane.OK_OPTION)
			{
				Library[] libs = list.SelectedLibraries;
				if (libs != null)
				{
					proj.doAction(LogisimFileActions.unloadLibraries(libs));
				}
			}
		}

		public static void doUnloadLibrary(Project proj, Library lib)
		{
			string message = proj.LogisimFile.getUnloadLibraryMessage(lib);
			if (!string.ReferenceEquals(message, null))
			{
				JOptionPane.showMessageDialog(proj.Frame, message, Strings.get("unloadErrorTitle"), JOptionPane.ERROR_MESSAGE);
			}
			else
			{
				proj.doAction(LogisimFileActions.unloadLibrary(lib));
			}
		}
	}

}
