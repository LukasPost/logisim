// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{

	using Project = logisim.proj.Project;
	using Projects = logisim.proj.Projects;

	internal class ProjectsDirty
	{
		private ProjectsDirty()
		{
		}

		private class DirtyListener : LibraryListener
		{
			internal Project proj;

			internal DirtyListener(Project proj)
			{
				this.proj = proj;
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				if (@event.Action == LibraryEvent.DIRTY_STATE)
				{
					LogisimFile lib = proj.LogisimFile;
					File file = lib.Loader.MainFile;
					LibraryManager.instance.setDirty(file, lib.Dirty);
				}
			}
		}

		private class ProjectListListener : PropertyChangeListener
		{
			public virtual void propertyChange(PropertyChangeEvent @event)
			{
				lock (this)
				{
					foreach (DirtyListener l in listeners)
					{
						l.proj.removeLibraryListener(l);
					}
					listeners.Clear();
					foreach (Project proj in Projects.OpenProjects)
					{
						DirtyListener l = new DirtyListener(proj);
						proj.addLibraryListener(l);
						listeners.Add(l);
        
						LogisimFile lib = proj.LogisimFile;
						LibraryManager.instance.setDirty(lib.Loader.MainFile, lib.Dirty);
					}
				}
			}
		}

		private static ProjectListListener projectListListener = new ProjectListListener();
		private static List<DirtyListener> listeners = new List<DirtyListener>();

		public static void initialize()
		{
			Projects.addPropertyChangeListener(Projects.projectListProperty, projectListListener);
		}
	}

}
