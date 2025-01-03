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

	using AnalyzerManager = logisim.analyze.gui.AnalyzerManager;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using PreferencesFrame = logisim.gui.prefs.PreferencesFrame;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using Projects = logisim.proj.Projects;
	using WindowMenuItemManager = logisim.util.WindowMenuItemManager;

	public class WindowManagers
	{
		private WindowManagers()
		{
		}

		public static void initialize()
		{
			if (!initialized)
			{
				initialized = true;
				AnalyzerManager.initialize();
				PreferencesFrame.initializeManager();
				Projects.addPropertyChangeListener(Projects.projectListProperty, myListener);
				computeListeners();
			}
		}

		private static bool initialized = false;
		private static MyListener myListener = new MyListener();
		private static Dictionary<Project, ProjectManager> projectMap = new LinkedHashMap<Project, ProjectManager>();

		private class MyListener : PropertyChangeListener
		{
			public virtual void propertyChange(PropertyChangeEvent @event)
			{
				computeListeners();
			}
		}

		private class ProjectManager : WindowMenuItemManager, ProjectListener, LibraryListener
		{
			internal Project proj;

			internal ProjectManager(Project proj) : base(proj.LogisimFile.Name, false)
			{
				this.proj = proj;
				proj.addProjectListener(this);
				proj.addLibraryListener(this);
				frameOpened(proj.Frame);
			}

			public override JFrame getJFrame(bool create)
			{
				return proj.Frame;
			}

			public virtual void projectChanged(ProjectEvent @event)
			{
				if (@event.Action == ProjectEvent.ACTION_SET_FILE)
				{
					Text = proj.LogisimFile.Name;
				}
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				if (@event.Action == LibraryEvent.SET_NAME)
				{
					Text = (string) @event.Data;
				}
			}
		}

		private static void computeListeners()
		{
			List<Project> nowOpen = Projects.OpenProjects;

			Dictionary<Project, ProjectManager>.KeyCollection closed = new HashSet<Project>(projectMap.Keys);
			closed.removeAll(nowOpen);
			foreach (Project proj in closed)
			{
				ProjectManager manager = projectMap[proj];
				manager.frameClosed(manager.getJFrame(false));
				projectMap.Remove(proj);
			}

			HashSet<Project> opened = new LinkedHashSet<Project>(nowOpen);
			opened.RemoveAll(projectMap.Keys);
			foreach (Project proj in opened)
			{
				ProjectManager manager = new ProjectManager(proj);
				projectMap[proj] = manager;
			}
		}
	}

}
