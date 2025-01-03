// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.proj
{


	using Circuit = logisim.circuit.Circuit;
	using LoadFailedException = logisim.file.LoadFailedException;
	using Loader = logisim.file.Loader;
	using LogisimFile = logisim.file.LogisimFile;
	using Frame = logisim.gui.main.Frame;
	using SplashScreen = logisim.gui.start.SplashScreen;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Tool = logisim.tools.Tool;
	using JFileChoosers = logisim.util.JFileChoosers;
	using StringUtil = logisim.util.StringUtil;

	public class ProjectActions
	{
		private ProjectActions()
		{
		}

		private class CreateFrame : ThreadStart
		{
			internal Loader loader;
			internal Project proj;
			internal bool isStartupScreen;

			public CreateFrame(Loader loader, Project proj, bool isStartup)
			{
				this.loader = loader;
				this.proj = proj;
				this.isStartupScreen = isStartup;
			}

			public virtual void run()
			{
				Frame frame = createFrame(null, proj);
				frame.setVisible(true);
				frame.toFront();
				frame.Canvas.requestFocus();
				loader.Parent = frame;
				if (isStartupScreen)
				{
					proj.StartupScreen = true;
				}
			}
		}

		public static Project doNew(SplashScreen monitor)
		{
			return doNew(monitor, false);
		}

		public static Project doNew(SplashScreen monitor, bool isStartupScreen)
		{
			if (monitor != null)
			{
				monitor.Progress = SplashScreen.FILE_CREATE;
			}
			Loader loader = new Loader(monitor);
			Stream templReader = AppPreferences.Template.createStream();
			LogisimFile file = null;
			try
			{
				file = loader.openLogisimFile(templReader);
			}
			catch (IOException ex)
			{
				displayException(monitor, ex);
			}
			catch (LoadFailedException ex)
			{
				displayException(monitor, ex);
			}
			finally
			{
				try
				{
					templReader.Close();
				}
				catch (IOException)
				{
				}
			}
			if (file == null)
			{
				file = createEmptyFile(loader);
			}
			return completeProject(monitor, loader, file, isStartupScreen);
		}

		private static void displayException(Component parent, Exception ex)
		{
			string msg = StringUtil.format(Strings.get("templateOpenError"), ex.ToString());
			string ttl = Strings.get("templateOpenErrorTitle");
			JOptionPane.showMessageDialog(parent, msg, ttl, JOptionPane.ERROR_MESSAGE);
		}

		private static LogisimFile createEmptyFile(Loader loader)
		{
			Stream templReader = AppPreferences.EmptyTemplate.createStream();
			LogisimFile file;
			try
			{
				file = loader.openLogisimFile(templReader);
			}
			catch (Exception)
			{
				file = LogisimFile.createNew(loader);
				file.addCircuit(new Circuit("main"));
			}
			finally
			{
				try
				{
					templReader.Close();
				}
				catch (IOException)
				{
				}
			}
			return file;
		}

		private static Project completeProject(SplashScreen monitor, Loader loader, LogisimFile file, bool isStartup)
		{
			if (monitor != null)
			{
				monitor.Progress = SplashScreen.PROJECT_CREATE;
			}
			Project ret = new Project(file);

			if (monitor != null)
			{
				monitor.Progress = SplashScreen.FRAME_CREATE;
			}
			SwingUtilities.invokeLater(new CreateFrame(loader, ret, isStartup));
			return ret;
		}

		public static LogisimFile createNewFile(Project baseProject)
		{
			Loader loader = new Loader(baseProject == null ? null : baseProject.Frame);
			Stream templReader = AppPreferences.Template.createStream();
			LogisimFile file;
			try
			{
				file = loader.openLogisimFile(templReader);
			}
			catch (IOException ex)
			{
				if (baseProject != null)
				{
					displayException(baseProject.Frame, ex);
				}
				file = createEmptyFile(loader);
			}
			catch (LoadFailedException ex)
			{
				if (!ex.isShown() && baseProject != null)
				{
					displayException(baseProject.Frame, ex);
				}
				file = createEmptyFile(loader);
			}
			finally
			{
				try
				{
					templReader.Close();
				}
				catch (IOException)
				{
				}
			}
			return file;
		}

		private static Frame createFrame(Project sourceProject, Project newProject)
		{
			if (sourceProject != null)
			{
				Frame frame = sourceProject.Frame;
				if (frame != null)
				{
					frame.savePreferences();
				}
			}
			Frame newFrame = new Frame(newProject);
			newProject.Frame = newFrame;
			return newFrame;
		}

		public static Project doNew(Project baseProject)
		{
			LogisimFile file = createNewFile(baseProject);
			Project newProj = new Project(file);
			Frame frame = createFrame(baseProject, newProj);
			frame.setVisible(true);
			frame.Canvas.requestFocus();
			newProj.LogisimFile.Loader.Parent = frame;
			return newProj;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static Project doOpen(logisim.gui.start.SplashScreen monitor, java.io.File source, java.util.Map<java.io.File, java.io.File> substitutions) throws logisim.file.LoadFailedException
		public static Project doOpen(SplashScreen monitor, File source, Dictionary<File, File> substitutions)
		{
			if (monitor != null)
			{
				monitor.Progress = SplashScreen.FILE_LOAD;
			}
			Loader loader = new Loader(monitor);
			LogisimFile file = loader.openLogisimFile(source, substitutions);
			AppPreferences.updateRecentFile(source);

			return completeProject(monitor, loader, file, false);
		}

		public static void doOpen(Component parent, Project baseProject)
		{
			JFileChooser chooser;
			if (baseProject != null)
			{
				Loader oldLoader = baseProject.LogisimFile.Loader;
				chooser = oldLoader.createChooser();
				if (oldLoader.MainFile != null)
				{
					chooser.setSelectedFile(oldLoader.MainFile);
				}
			}
			else
			{
				chooser = JFileChoosers.create();
			}
			chooser.setFileFilter(Loader.LOGISIM_FILTER);

			int returnVal = chooser.showOpenDialog(parent);
			if (returnVal != JFileChooser.APPROVE_OPTION)
			{
				return;
			}
			File selected = chooser.getSelectedFile();
			if (selected != null)
			{
				doOpen(parent, baseProject, selected);
			}
		}

		public static Project doOpen(Component parent, Project baseProject, File f)
		{
			Project proj = Projects.findProjectFor(f);
			Loader loader = null;
			if (proj != null)
			{
				proj.Frame.toFront();
				loader = proj.LogisimFile.getLoader();
				if (proj.FileDirty)
				{
					string message = StringUtil.format(Strings.get("openAlreadyMessage"), proj.LogisimFile.getName());
					string[] options = new string[] {Strings.get("openAlreadyLoseChangesOption"), Strings.get("openAlreadyNewWindowOption"), Strings.get("openAlreadyCancelOption")};
					int result = JOptionPane.showOptionDialog(proj.Frame, message, Strings.get("openAlreadyTitle"), 0, JOptionPane.QUESTION_MESSAGE, null, options, options[2]);
					if (result == 0)
					{
						; // keep proj as is, so that load happens into the window
					}
					else if (result == 1)
					{
						proj = null; // we'll create a new project
					}
					else
					{
						return proj;
					}
				}
			}

			if (proj == null && baseProject != null && baseProject.StartupScreen)
			{
				proj = baseProject;
				proj.StartupScreen = false;
				loader = baseProject.LogisimFile.Loader;
			}
			else
			{
				loader = new Loader(baseProject == null ? parent : baseProject.Frame);
			}

			try
			{
				LogisimFile lib = loader.openLogisimFile(f);
				AppPreferences.updateRecentFile(f);
				if (lib == null)
				{
					return null;
				}
				if (proj == null)
				{
					proj = new Project(lib);
				}
				else
				{
					proj.LogisimFile = lib;
				}
			}
			catch (LoadFailedException ex)
			{
				if (!ex.Shown)
				{
					JOptionPane.showMessageDialog(parent, StringUtil.format(Strings.get("fileOpenError"), ex.ToString()), Strings.get("fileOpenErrorTitle"), JOptionPane.ERROR_MESSAGE);
				}
				return null;
			}

			Frame frame = proj.Frame;
			if (frame == null)
			{
				frame = createFrame(baseProject, proj);
			}
			frame.setVisible(true);
			frame.toFront();
			frame.Canvas.requestFocus();
			proj.LogisimFile.getLoader().setParent(frame);
			return proj;
		}

		// returns true if save is completed
		public static bool doSaveAs(Project proj)
		{
			Loader loader = proj.LogisimFile.getLoader();
			JFileChooser chooser = loader.createChooser();
			chooser.setFileFilter(Loader.LOGISIM_FILTER);
			if (loader.MainFile != null)
			{
				chooser.setSelectedFile(loader.MainFile);
			}
			int returnVal = chooser.showSaveDialog(proj.Frame);
			if (returnVal != JFileChooser.APPROVE_OPTION)
			{
				return false;
			}

			File f = chooser.getSelectedFile();
			string circExt = Loader.LOGISIM_EXTENSION;
			if (!f.getName().EndsWith(circExt))
			{
				string old = f.getName();
				int ext0 = old.LastIndexOf('.');
				if (ext0 < 0 || !Pattern.matches("\\.\\p{L}{2,}[0-9]?", old.Substring(ext0)))
				{
					f = new File(f.getParentFile(), old + circExt);
				}
				else
				{
					string ext = old.Substring(ext0);
					string ttl = Strings.get("replaceExtensionTitle");
					string msg = Strings.get("replaceExtensionMessage", ext);
					object[] options = new object[] {Strings.get("replaceExtensionReplaceOpt", ext), Strings.get("replaceExtensionAddOpt", circExt), Strings.get("replaceExtensionKeepOpt")};
					JOptionPane dlog = new JOptionPane(msg);
					dlog.setMessageType(JOptionPane.QUESTION_MESSAGE);
					dlog.setOptions(options);
					dlog.createDialog(proj.Frame, ttl).setVisible(true);

					object result = dlog.getValue();
					if (result == options[0])
					{
						string name = old.Substring(0, ext0) + circExt;
						f = new File(f.getParentFile(), name);
					}
					else if (result == options[1])
					{
						f = new File(f.getParentFile(), old + circExt);
					}
				}
			}

			if (f.exists())
			{
				int confirm = JOptionPane.showConfirmDialog(proj.Frame, Strings.get("confirmOverwriteMessage"), Strings.get("confirmOverwriteTitle"), JOptionPane.YES_NO_OPTION);
				if (confirm != JOptionPane.YES_OPTION)
				{
					return false;
				}
			}
			return doSave(proj, f);
		}

		public static bool doSave(Project proj)
		{
			Loader loader = proj.LogisimFile.getLoader();
			File f = loader.MainFile;
			if (f == null)
			{
				return doSaveAs(proj);
			}
			else
			{
				return doSave(proj, f);
			}
		}

		private static bool doSave(Project proj, File f)
		{
			Loader loader = proj.LogisimFile.getLoader();
			Tool oldTool = proj.Tool;
			proj.Tool = null;
			bool ret = loader.save(proj.LogisimFile, f);
			if (ret)
			{
				AppPreferences.updateRecentFile(f);
				proj.setFileAsClean();
			}
			proj.Tool = oldTool;
			return ret;
		}

		public static void doQuit()
		{
			Frame top = Projects.TopFrame;
			top.savePreferences();

			foreach (Project proj in new List<Project>(Projects.OpenProjects))
			{
				if (!proj.confirmClose(Strings.get("confirmQuitTitle")))
				{
					return;
				}
			}
			Environment.Exit(0);
		}
	}

}
