// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.proj
{

	using Loader = logisim.file.Loader;
	using Frame = logisim.gui.main.Frame;
	using MacCompatibility = logisim.util.MacCompatibility;
	using PropertyChangeWeakSupport = logisim.util.PropertyChangeWeakSupport;

	public class Projects
	{
		public const string projectListProperty = "projectList";

		private static readonly WeakHashMap<Window, Point> frameLocations = new WeakHashMap<Window, Point>();

		private static void projectRemoved(Project proj, Frame frame, MyListener listener)
		{
			frame.removeWindowListener(listener);
			openProjects.Remove(proj);
			proj.Simulator.shutDown();
			propertySupport.firePropertyChange(projectListProperty, null, null);
		}

		private class MyListener : WindowAdapter
		{
			public override void windowActivated(WindowEvent @event)
			{
				mostRecentFrame = (Frame) @event.getSource();
			}

			public override void windowClosing(WindowEvent @event)
			{
				Frame frame = (Frame) @event.getSource();
				if ((frame.getExtendedState() & Frame.ICONIFIED) == 0)
				{
					mostRecentFrame = frame;
					try
					{
						frameLocations.put(frame, frame.getLocationOnScreen());
					}
					catch (Exception)
					{
					}
				}
			}

			public override void windowClosed(WindowEvent @event)
			{
				Frame frame = (Frame) @event.getSource();
				Project proj = frame.Project;

				if (frame == proj.Frame)
				{
					projectRemoved(proj, frame, this);
				}
				if (openProjects.Count == 0 && !MacCompatibility.SwingUsingScreenMenuBar)
				{
					ProjectActions.doQuit();
				}
			}

			public override void windowOpened(WindowEvent @event)
			{
				Frame frame = (Frame) @event.getSource();
				Project proj = frame.Project;

				if (frame == proj.Frame && !openProjects.Contains(proj))
				{
					openProjects.Add(proj);
					propertySupport.firePropertyChange(projectListProperty, null, null);
				}
			}
		}

		private static readonly MyListener myListener = new MyListener();
		private static readonly PropertyChangeWeakSupport propertySupport = new PropertyChangeWeakSupport(typeof(Projects));
		private static List<Project> openProjects = new List<Project>();
		private static Frame mostRecentFrame = null;

		private Projects()
		{
		}

		public static Frame TopFrame
		{
			get
			{
				Frame ret = mostRecentFrame;
				if (ret == null)
				{
					Frame backup = null;
					foreach (Project proj in openProjects)
					{
						Frame frame = proj.Frame;
						if (ret == null)
						{
							ret = frame;
						}
						if (ret.isVisible() && (ret.getExtendedState() & Frame.ICONIFIED) != 0)
						{
							backup = ret;
						}
					}
					if (ret == null)
					{
						ret = backup;
					}
				}
				return ret;
			}
		}

		internal static void windowCreated(Project proj, Frame oldFrame, Frame frame)
		{
			if (oldFrame != null)
			{
				projectRemoved(proj, oldFrame, myListener);
			}

			if (frame == null)
			{
				return;
			}

			// locate the window
			Point lowest = null;
			foreach (Project p in openProjects)
			{
				Frame f = p.Frame;
				if (f == null)
				{
					continue;
				}
				Point loc = p.Frame.getLocation();
				if (lowest == null || loc.y > lowest.y)
				{
					lowest = loc;
				}
			}
			if (lowest != null)
			{
				Size sz = frame.getToolkit().getScreenSize();
				int x = Math.Min(lowest.x + 20, sz.width - 200);
				int y = Math.Min(lowest.y + 20, sz.height - 200);
				if (x < 0)
				{
					x = 0;
				}
				if (y < 0)
				{
					y = 0;
				}
				frame.setLocation(x, y);
			}

			if (frame.isVisible() && !openProjects.Contains(proj))
			{
				openProjects.Add(proj);
				propertySupport.firePropertyChange(projectListProperty, null, null);
			}
			frame.addWindowListener(myListener);
		}

		public static IList<Project> OpenProjects
		{
			get
			{
				return openProjects.AsReadOnly();
			}
		}

		public static bool windowNamed(string name)
		{
			foreach (Project proj in openProjects)
			{
				if (proj.LogisimFile.getName().Equals(name))
				{
					return true;
				}
			}
			return false;
		}

		public static Project findProjectFor(File query)
		{
			foreach (Project proj in openProjects)
			{
				Loader loader = proj.LogisimFile.getLoader();
				if (loader == null)
				{
					continue;
				}
				File f = loader.MainFile;
				if (query.Equals(f))
				{
					return proj;
				}
			}
			return null;
		}

		//
		// PropertyChangeSource methods
		//
		public static void addPropertyChangeListener(PropertyChangeListener listener)
		{
			propertySupport.addPropertyChangeListener(listener);
		}

		public static void addPropertyChangeListener(string propertyName, PropertyChangeListener listener)
		{
			propertySupport.addPropertyChangeListener(propertyName, listener);
		}

		public static void removePropertyChangeListener(PropertyChangeListener listener)
		{
			propertySupport.removePropertyChangeListener(listener);
		}

		public static void removePropertyChangeListener(string propertyName, PropertyChangeListener listener)
		{
			propertySupport.removePropertyChangeListener(propertyName, listener);
		}

		public static Point getLocation(Window win)
		{
			Point ret = frameLocations.get(win);
			return ret == null ? null : (Point) ret.clone();
		}
	}

}
