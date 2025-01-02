// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.start
{

	using Main = logisim.Main;
	using LoadFailedException = logisim.file.LoadFailedException;
	using Loader = logisim.file.Loader;
	using Print = logisim.gui.main.Print;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using WindowManagers = logisim.gui.menu.WindowManagers;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Project = logisim.proj.Project;
	using ProjectActions = logisim.proj.ProjectActions;
	using LocaleManager = logisim.util.LocaleManager;
	using StringUtil = logisim.util.StringUtil;

	public class Startup
	{
		private static Startup startupTemp = null;

		internal static void doOpen(File file)
		{
			if (startupTemp != null)
			{
				startupTemp.doOpenFile(file);
			}
		}

		internal static void doPrint(File file)
		{
			if (startupTemp != null)
			{
				startupTemp.doPrintFile(file);
			}
		}

		private void doOpenFile(File file)
		{
			if (initialized)
			{
				ProjectActions.doOpen(null, null, file);
			}
			else
			{
				filesToOpen.Add(file);
			}
		}

		private void doPrintFile(File file)
		{
			if (initialized)
			{
				Project toPrint = ProjectActions.doOpen(null, null, file);
				Print.doPrint(toPrint);
				toPrint.Frame.dispose();
			}
			else
			{
				filesToPrint.Add(file);
			}
		}

		private static void registerHandler()
		{
			try
			{
				Type needed1 = Type.GetType("com.apple.eawt.Application");
				if (needed1 == null)
				{
					return;
				}
				Type needed2 = Type.GetType("com.apple.eawt.ApplicationAdapter");
				if (needed2 == null)
				{
					return;
				}
				MacOsAdapter.register();
				MacOsAdapter.addListeners(true);
			}
			catch (ClassNotFoundException)
			{
				return;
			}
			catch (Exception)
			{
				try
				{
					MacOsAdapter.addListeners(false);
				}
				catch (Exception)
				{
				}
			}
		}

		// based on command line
		internal bool isTty;
		private File templFile = null;
		private bool templEmpty = false;
		private bool templPlain = false;
		private List<File> filesToOpen = new List<File>();
		private bool showSplash;
		private File loadFile;
		private Dictionary<File, File> substitutions = new Dictionary<File, File>();
		private int ttyFormat = 0;

		// from other sources
		private bool initialized = false;
		private SplashScreen monitor = null;
		private List<File> filesToPrint = new List<File>();

		private Startup(bool isTty)
		{
			this.isTty = isTty;
			this.showSplash = !isTty;
		}

		internal virtual IList<File> FilesToOpen
		{
			get
			{
				return filesToOpen;
			}
		}

		internal virtual File LoadFile
		{
			get
			{
				return loadFile;
			}
		}

		internal virtual int TtyFormat
		{
			get
			{
				return ttyFormat;
			}
		}

		internal virtual IDictionary<File, File> Substitutions
		{
			get
			{
				return Collections.unmodifiableMap(substitutions);
			}
		}

		public virtual void run()
		{
			if (isTty)
			{
				try
				{
					TtyInterface.run(this);
					return;
				}
				catch (Exception t)
				{
					Console.WriteLine(t.ToString());
					Console.Write(t.StackTrace);
					Environment.Exit(-1);
					return;
				}
			}

			// kick off the progress monitor
			// (The values used for progress values are based on a single run where
			// I loaded a large file.)
			if (showSplash)
			{
				try
				{
					monitor = new SplashScreen();
					monitor.Visible = true;
				}
				catch (Exception)
				{
					monitor = null;
					showSplash = false;
				}
			}

			// pre-load the two basic component libraries, just so that the time
			// taken is shown separately in the progress bar.
			if (showSplash)
			{
				monitor.Progress = SplashScreen.LIBRARIES;
			}
			Loader templLoader = new Loader(monitor);
			int count = templLoader.Builtin.getLibrary("Base").Tools.Count + templLoader.Builtin.getLibrary("Gates").Tools.Count;
			if (count < 0)
			{
				// this will never happen, but the optimizer doesn't know that...
				Console.Error.WriteLine("FATAL ERROR - no components"); // OK
				Environment.Exit(-1);
			}

			// load in template
			loadTemplate(templLoader, templFile, templEmpty);

			// now that the splash screen is almost gone, we do some last-minute
			// interface initialization
			if (showSplash)
			{
				monitor.Progress = SplashScreen.GUI_INIT;
			}
			WindowManagers.initialize();
			new LogisimMenuBar(null, null);
			// most of the time occupied here will be in loading menus, which
			// will occur eventually anyway; we might as well do it when the
			// monitor says we are

			// if user has double-clicked a file to open, we'll
			// use that as the file to open now.
			initialized = true;

			// load file
			if (filesToOpen.Count == 0)
			{
				ProjectActions.doNew(monitor, true);
				if (showSplash)
				{
					monitor.close();
				}
			}
			else
			{
				bool first = true;
				foreach (File fileToOpen in filesToOpen)
				{
					try
					{
						ProjectActions.doOpen(monitor, fileToOpen, substitutions);
					}
					catch (LoadFailedException ex)
					{
						Console.Error.WriteLine(fileToOpen.getName() + ": " + ex.Message); // OK
						Environment.Exit(-1);
					}
					if (first)
					{
						first = false;
						if (showSplash)
						{
							monitor.close();
						}
						monitor = null;
					}
				}
			}

			foreach (File fileToPrint in filesToPrint)
			{
				doPrintFile(fileToPrint);
			}
		}

		private static string Locale
		{
			set
			{
				Locale[] opts = Strings.LocaleOptions;
				for (int i = 0; i < opts.Length; i++)
				{
					if (value.Equals(opts[i].ToString()))
					{
						LocaleManager.Locale = opts[i];
						return;
					}
				}
				Console.Error.WriteLine(Strings.get("invalidLocaleError")); // OK
				Console.Error.WriteLine(Strings.get("invalidLocaleOptionsHeader")); // OK
				for (int i = 0; i < opts.Length; i++)
				{
					Console.Error.WriteLine("   " + opts[i].ToString()); // OK
				}
				Environment.Exit(-1);
			}
		}

		private void loadTemplate(Loader loader, File templFile, bool templEmpty)
		{
			if (showSplash)
			{
				monitor.Progress = SplashScreen.TEMPLATE_OPEN;
			}
			if (templFile != null)
			{
				AppPreferences.TemplateFile = templFile;
				AppPreferences.TemplateType = AppPreferences.TEMPLATE_CUSTOM;
			}
			else if (templEmpty)
			{
				AppPreferences.TemplateType = AppPreferences.TEMPLATE_EMPTY;
			}
			else if (templPlain)
			{
				AppPreferences.TemplateType = AppPreferences.TEMPLATE_PLAIN;
			}
		}

		public static Startup parseArgs(string[] args)
		{
			// see whether we'll be using any graphics
			bool isTty = false;
			bool isClearPreferences = false;
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].Equals("-tty"))
				{
					isTty = true;
				}
				else if (args[i].Equals("-clearprefs") || args[i].Equals("-clearprops"))
				{
					isClearPreferences = true;
				}
			}

			if (!isTty)
			{
				// we're using the GUI: Set up the Look&Feel to match the platform
				System.setProperty("com.apple.mrj.application.apple.menu.about.name", "Logisim");
				System.setProperty("apple.laf.useScreenMenuBar", "true");

				LocaleManager.ReplaceAccents = false;

				// Initialize graphics acceleration if appropriate
				AppPreferences.handleGraphicsAcceleration();
			}

			Startup ret = new Startup(isTty);
			startupTemp = ret;
			if (!isTty)
			{
				registerHandler();
			}

			if (isClearPreferences)
			{
				AppPreferences.clear();
			}

			try
			{
				UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
			}
			catch (Exception)
			{
			}

			// parse arguments
			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];
				if (arg.Equals("-tty"))
				{
					if (i + 1 < args.Length)
					{
						i++;
						string[] fmts = args[i].Split(",", true);
						if (fmts.Length == 0)
						{
							Console.Error.WriteLine(Strings.get("ttyFormatError")); // OK
						}
						for (int j = 0; j < fmts.Length; j++)
						{
							string fmt = fmts[j].Trim();
							if (fmt.Equals("table"))
							{
								ret.ttyFormat |= TtyInterface.FORMAT_TABLE;
							}
							else if (fmt.Equals("speed"))
							{
								ret.ttyFormat |= TtyInterface.FORMAT_SPEED;
							}
							else if (fmt.Equals("tty"))
							{
								ret.ttyFormat |= TtyInterface.FORMAT_TTY;
							}
							else if (fmt.Equals("halt"))
							{
								ret.ttyFormat |= TtyInterface.FORMAT_HALT;
							}
							else if (fmt.Equals("stats"))
							{
								ret.ttyFormat |= TtyInterface.FORMAT_STATISTICS;
							}
							else
							{
								Console.Error.WriteLine(Strings.get("ttyFormatError")); // OK
							}
						}
					}
					else
					{
						Console.Error.WriteLine(Strings.get("ttyFormatError")); // OK
						return null;
					}
				}
				else if (arg.Equals("-sub"))
				{
					if (i + 2 < args.Length)
					{
						File a = new File(args[i + 1]);
						File b = new File(args[i + 2]);
						if (ret.substitutions.ContainsKey(a))
						{
							Console.Error.WriteLine(Strings.get("argDuplicateSubstitutionError")); // OK
							return null;
						}
						else
						{
							ret.substitutions[a] = b;
							i += 2;
						}
					}
					else
					{
						Console.Error.WriteLine(Strings.get("argTwoSubstitutionError")); // OK
						return null;
					}
				}
				else if (arg.Equals("-load"))
				{
					if (i + 1 < args.Length)
					{
						i++;
						if (ret.loadFile != null)
						{
							Console.Error.WriteLine(Strings.get("loadMultipleError")); // OK
						}
						File f = new File(args[i]);
						ret.loadFile = f;
					}
					else
					{
						Console.Error.WriteLine(Strings.get("loadNeedsFileError")); // OK
						return null;
					}
				}
				else if (arg.Equals("-empty"))
				{
					if (ret.templFile != null || ret.templEmpty || ret.templPlain)
					{
						Console.Error.WriteLine(Strings.get("argOneTemplateError")); // OK
						return null;
					}
					ret.templEmpty = true;
				}
				else if (arg.Equals("-plain"))
				{
					if (ret.templFile != null || ret.templEmpty || ret.templPlain)
					{
						Console.Error.WriteLine(Strings.get("argOneTemplateError")); // OK
						return null;
					}
					ret.templPlain = true;
				}
				else if (arg.Equals("-version"))
				{
					Console.WriteLine(Main.VERSION_NAME); // OK
					return null;
				}
				else if (arg.Equals("-gates"))
				{
					i++;
					if (i >= args.Length)
					{
						printUsage();
					}
					string a = args[i];
					if (a.Equals("shaped"))
					{
						AppPreferences.GATE_SHAPE.set(AppPreferences.SHAPE_SHAPED);
					}
					else if (a.Equals("rectangular"))
					{
						AppPreferences.GATE_SHAPE.set(AppPreferences.SHAPE_RECTANGULAR);
					}
					else
					{
						Console.Error.WriteLine(Strings.get("argGatesOptionError")); // OK
						Environment.Exit(-1);
					}
				}
				else if (arg.Equals("-locale"))
				{
					i++;
					if (i >= args.Length)
					{
						printUsage();
					}
					Locale = args[i];
				}
				else if (arg.Equals("-accents"))
				{
					i++;
					if (i >= args.Length)
					{
						printUsage();
					}
					string a = args[i];
					if (a.Equals("yes"))
					{
						AppPreferences.ACCENTS_REPLACE.Boolean = false;
					}
					else if (a.Equals("no"))
					{
						AppPreferences.ACCENTS_REPLACE.Boolean = true;
					}
					else
					{
						Console.Error.WriteLine(Strings.get("argAccentsOptionError")); // OK
						Environment.Exit(-1);
					}
				}
				else if (arg.Equals("-template"))
				{
					if (ret.templFile != null || ret.templEmpty || ret.templPlain)
					{
						Console.Error.WriteLine(Strings.get("argOneTemplateError")); // OK
						return null;
					}
					i++;
					if (i >= args.Length)
					{
						printUsage();
					}
					ret.templFile = new File(args[i]);
					if (!ret.templFile.exists())
					{
						Console.Error.WriteLine(StringUtil.format(Strings.get("templateMissingError"), args[i]));
					}
					else if (!ret.templFile.canRead())
					{
						Console.Error.WriteLine(StringUtil.format(Strings.get("templateCannotReadError"), args[i]));
					}
				}
				else if (arg.Equals("-nosplash"))
				{
					ret.showSplash = false;
				}
				else if (arg.Equals("-clearprefs"))
				{
					// already handled above
				}
				else if (arg[0] == '-')
				{
					printUsage();
					return null;
				}
				else
				{
					ret.filesToOpen.Add(new File(arg));
				}
			}
			if (ret.isTty && ret.filesToOpen.Count == 0)
			{
				Console.Error.WriteLine(Strings.get("ttyNeedsFileError")); // OK
				return null;
			}
			if (ret.loadFile != null && !ret.isTty)
			{
				Console.Error.WriteLine(Strings.get("loadNeedsTtyError")); // OK
				return null;
			}
			return ret;
		}

		private static void printUsage()
		{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Console.Error.WriteLine(StringUtil.format(Strings.get("argUsage"), typeof(Startup).FullName)); // OK
			Console.Error.WriteLine(); // OK
			Console.Error.WriteLine(Strings.get("argOptionHeader")); // OK
			Console.Error.WriteLine("   " + Strings.get("argAccentsOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argClearOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argEmptyOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argGatesOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argHelpOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argLoadOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argLocaleOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argNoSplashOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argPlainOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argSubOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argTemplateOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argTtyOption")); // OK
			Console.Error.WriteLine("   " + Strings.get("argVersionOption")); // OK
			Environment.Exit(-1);
		}
	}

}
