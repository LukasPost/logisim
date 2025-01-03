// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.prefs
{

	using Main = logisim.Main;
	using RadixOption = logisim.circuit.RadixOption;
	using Direction = logisim.data.Direction;
	using Startup = logisim.gui.start.Startup;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;
	using PropertyChangeWeakSupport = logisim.util.PropertyChangeWeakSupport;

	public class AppPreferences
	{
		// class variables for maintaining consistency between properties,
		// internal variables, and other classes
		private static Preferences prefs = null;
		private static MyListener myListener = null;
		private static PropertyChangeWeakSupport propertySupport = new PropertyChangeWeakSupport(typeof(AppPreferences));

		// Template preferences
		public const int TEMPLATE_UNKNOWN = -1;
		public const int TEMPLATE_EMPTY = 0;
		public const int TEMPLATE_PLAIN = 1;
		public const int TEMPLATE_CUSTOM = 2;

		public const string TEMPLATE = "template";
		public const string TEMPLATE_TYPE = "templateType";
		public const string TEMPLATE_FILE = "templateFile";

		private static int templateType = TEMPLATE_PLAIN;
		private static File templateFile = null;

		private static Template plainTemplate = null;
		private static Template emptyTemplate = null;
		private static Template customTemplate = null;
		private static File customTemplateFile = null;

		// International preferences
		public const string SHAPE_SHAPED = "shaped";
		public const string SHAPE_RECTANGULAR = "rectangular";
		public const string SHAPE_DIN40700 = "din40700";

		public static readonly PrefMonitor<string> GATE_SHAPE = create(new PrefMonitorStringOpts("gateShape", new string[] {SHAPE_SHAPED, SHAPE_RECTANGULAR, SHAPE_DIN40700}, SHAPE_SHAPED));
		public static readonly PrefMonitor<string> LOCALE = create(new LocalePreference());
		public static readonly PrefMonitor<bool> ACCENTS_REPLACE = create(new PrefMonitorBoolean("accentsReplace", false));

		// Window preferences
		public const string TOOLBAR_HIDDEN = "hidden";
		public const string TOOLBAR_DOWN_MIDDLE = "downMiddle";

		public static readonly PrefMonitor<bool> SHOW_TICK_RATE = create(new PrefMonitorBoolean("showTickRate", false));
		public static readonly PrefMonitor<string> TOOLBAR_PLACEMENT = create(new PrefMonitorStringOpts("toolbarPlacement", new string[] {Direction.North.ToString(), Direction.South.ToString(), Direction.East.ToString(), Direction.West.ToString(), TOOLBAR_DOWN_MIDDLE, TOOLBAR_HIDDEN}, Direction.North.ToString()));

		// Layout preferences
		public const string ADD_AFTER_UNCHANGED = "unchanged";
		public const string ADD_AFTER_EDIT = "edit";

		public static readonly PrefMonitor<bool> PRINTER_VIEW = create(new PrefMonitorBoolean("printerView", false));
		public static readonly PrefMonitor<bool> ATTRIBUTE_HALO = create(new PrefMonitorBoolean("attributeHalo", true));
		public static readonly PrefMonitor<bool> COMPONENT_TIPS = create(new PrefMonitorBoolean("componentTips", true));
		public static readonly PrefMonitor<bool> MOVE_KEEP_CONNECT = create(new PrefMonitorBoolean("keepConnected", true));
		public static readonly PrefMonitor<bool> ADD_SHOW_GHOSTS = create(new PrefMonitorBoolean("showGhosts", true));
		public static readonly PrefMonitor<string> ADD_AFTER = create(new PrefMonitorStringOpts("afterAdd", new string[] {ADD_AFTER_EDIT, ADD_AFTER_UNCHANGED}, ADD_AFTER_EDIT));
		public static PrefMonitor<string> POKE_WIRE_RADIX1;
		public static PrefMonitor<string> POKE_WIRE_RADIX2;

		static AppPreferences()
		{
			RadixOption[] radixOptions = RadixOption.OPTIONS;
			string[] radixStrings = new string[radixOptions.Length];
			for (int i = 0; i < radixOptions.Length; i++)
			{
				radixStrings[i] = radixOptions[i].SaveString;
			}
			POKE_WIRE_RADIX1 = create(new PrefMonitorStringOpts("pokeRadix1", radixStrings, RadixOption.RADIX_2.SaveString));
			POKE_WIRE_RADIX2 = create(new PrefMonitorStringOpts("pokeRadix2", radixStrings, RadixOption.RADIX_10_SIGNED.SaveString));
		}

		// Experimental preferences
		public const string ACCEL_DEFAULT = "default";
		public const string ACCEL_NONE = "none";
		public const string ACCEL_OPENGL = "opengl";
		public const string ACCEL_D3D = "d3d";

		public static readonly PrefMonitor<string> JGraphics_ACCELERATION = create(new PrefMonitorStringOpts("JGraphicsAcceleration", new string[] {ACCEL_DEFAULT, ACCEL_NONE, ACCEL_OPENGL, ACCEL_D3D}, ACCEL_DEFAULT));

		// hidden window preferences - not part of the preferences dialog, changes
		// to preference does not affect current windows, and the values are not
		// saved until the application is closed
		public const string RECENT_PROJECTS = "recentProjects";
		private static readonly RecentProjects recentProjects = new RecentProjects();
		public static readonly PrefMonitor<double> TICK_FREQUENCY = create(new PrefMonitorDouble("tickFrequency", 1.0));
		public static readonly PrefMonitor<bool> LAYOUT_SHOW_GRID = create(new PrefMonitorBoolean("layoutGrid", true));
		public static readonly PrefMonitor<double> LAYOUT_ZOOM = create(new PrefMonitorDouble("layoutZoom", 1.0));
		public static readonly PrefMonitor<bool> APPEARANCE_SHOW_GRID = create(new PrefMonitorBoolean("appearanceGrid", true));
		public static readonly PrefMonitor<double> APPEARANCE_ZOOM = create(new PrefMonitorDouble("appearanceZoom", 1.0));
		public static readonly PrefMonitor<int> WINDOW_STATE = create(new PrefMonitorInt("windowState", JFrame.NORMAL));
		public static readonly PrefMonitor<int> WINDOW_WIDTH = create(new PrefMonitorInt("windowWidth", 640));
		public static readonly PrefMonitor<int> WINDOW_HEIGHT = create(new PrefMonitorInt("windowHeight", 480));
		public static readonly PrefMonitor<string> WINDOW_LOCATION = create(new PrefMonitorString("windowLocation", "0,0"));
		public static readonly PrefMonitor<double> WINDOW_MAIN_SPLIT = create(new PrefMonitorDouble("windowMainSplit", 0.25));
		public static readonly PrefMonitor<double> WINDOW_LEFT_SPLIT = create(new PrefMonitorDouble("windowLeftSplit", 0.5));
		public static readonly PrefMonitor<string> DIALOG_DIRECTORY = create(new PrefMonitorString("dialogDirectory", ""));

		//
		// methods for accessing preferences
		//
		private class MyListener : PreferenceChangeListener, LocaleListener
		{
			public virtual void preferenceChange(PreferenceChangeEvent @event)
			{
				Preferences prefs = @event.getNode();
				string prop = @event.getKey();
				if (ACCENTS_REPLACE.Identifier.Equals(prop))
				{
					Prefs;
					LocaleManager.ReplaceAccents = ACCENTS_REPLACE.Boolean;
				}
				else if (prop.Equals(TEMPLATE_TYPE))
				{
					int oldValue = templateType;
					int value = prefs.getInt(TEMPLATE_TYPE, TEMPLATE_UNKNOWN);
					if (value != oldValue)
					{
						templateType = value;
						propertySupport.firePropertyChange(TEMPLATE, oldValue, value);
						propertySupport.firePropertyChange(TEMPLATE_TYPE, oldValue, value);
					}
				}
				else if (prop.Equals(TEMPLATE_FILE))
				{
					File oldValue = templateFile;
					File value = convertFile(prefs.get(TEMPLATE_FILE, null));
					if (value == null ? oldValue != null :!value.Equals(oldValue))
					{
						templateFile = value;
						if (templateType == TEMPLATE_CUSTOM)
						{
							customTemplate = null;
							propertySupport.firePropertyChange(TEMPLATE, oldValue, value);
						}
						propertySupport.firePropertyChange(TEMPLATE_FILE, oldValue, value);
					}
				}
			}

			public virtual void localeChanged()
			{
				Locale loc = LocaleManager.Locale;
				string lang = loc.getLanguage();
				if (LOCALE != null)
				{
					LOCALE.set(lang);
				}
			}
		}

		private static PrefMonitor<E> create<E>(PrefMonitor<E> monitor)
		{
			return monitor;
		}

		public static void clear()
		{
			Preferences p = getPrefs(true);
			try
			{
				p.clear();
			}
			catch (BackingStoreException)
			{
			}
		}

		internal static Preferences Prefs
		{
			get
			{
				return getPrefs(false);
			}
		}

		private static Preferences getPrefs(bool shouldClear)
		{
			if (prefs == null)
			{
				lock (typeof(AppPreferences))
				{
					if (prefs == null)
					{
						Preferences p = Preferences.userNodeForPackage(typeof(Main));
						if (shouldClear)
						{
							try
							{
								p.clear();
							}
							catch (BackingStoreException)
							{
							}
						}
						myListener = new MyListener();
						p.addPreferenceChangeListener(myListener);
						prefs = p;

						TemplateFile = convertFile(p.get(TEMPLATE_FILE, null));
						TemplateType = p.getInt(TEMPLATE_TYPE, TEMPLATE_PLAIN);
					}
				}
			}
			return prefs;
		}

		private static File convertFile(string fileName)
		{
			if (string.ReferenceEquals(fileName, null) || fileName.Equals(""))
			{
				return null;
			}
			else
			{
				File file = new File(fileName);
				return file.canRead() ? file : null;
			}
		}

		//
		// PropertyChangeSource methods
		//
		public static void addPropertyChangeListener(PropertyChangeListener listener)
		{
			propertySupport.addPropertyChangeListener(listener);
		}

		public static void removePropertyChangeListener(PropertyChangeListener listener)
		{
			propertySupport.removePropertyChangeListener(listener);
		}

		public static void addPropertyChangeListener(string propertyName, PropertyChangeListener listener)
		{
			propertySupport.addPropertyChangeListener(propertyName, listener);
		}

		public static void removePropertyChangeListener(string propertyName, PropertyChangeListener listener)
		{
			propertySupport.removePropertyChangeListener(propertyName, listener);
		}

		internal static void firePropertyChange(string property, bool oldVal, bool newVal)
		{
			propertySupport.firePropertyChange(property, oldVal, newVal);
		}

		internal static void firePropertyChange(string property, object oldVal, object newVal)
		{
			propertySupport.firePropertyChange(property, oldVal, newVal);
		}

		//
		// accessor methods
		//
		public static int TemplateType
		{
			get
			{
				Prefs;
				int ret = templateType;
				if (ret == TEMPLATE_CUSTOM && templateFile == null)
				{
					ret = TEMPLATE_UNKNOWN;
				}
				return ret;
			}
			set
			{
				Prefs;
				if (value != TEMPLATE_PLAIN && value != TEMPLATE_EMPTY && value != TEMPLATE_CUSTOM)
				{
					value = TEMPLATE_UNKNOWN;
				}
				if (value != TEMPLATE_UNKNOWN && templateType != value)
				{
					Prefs.putInt(TEMPLATE_TYPE, value);
				}
			}
		}


		public static File TemplateFile
		{
			get
			{
				Prefs;
				return templateFile;
			}
		}

		public static void setTemplateFile(File value)
		{
			Prefs;
			setTemplateFile(value, null);
		}

		public static void setTemplateFile(File value, Template template)
		{
			Prefs;
			if (value != null && !value.canRead())
			{
				value = null;
			}
			if (value == null ? templateFile != null :!value.Equals(templateFile))
			{
				try
				{
					customTemplateFile = template == null ? null : value;
					customTemplate = template;
					Prefs.put(TEMPLATE_FILE, value == null ? "" : value.getCanonicalPath());
				}
				catch (IOException)
				{
				}
			}
		}

		public static void handleJGraphicsAcceleration()
		{
			string accel = JGraphics_ACCELERATION.get();
			try
			{
				if (string.ReferenceEquals(accel, ACCEL_NONE))
				{
					System.setProperty("sun.java2d.opengl", "False");
					System.setProperty("sun.java2d.d3d", "False");
				}
				else if (string.ReferenceEquals(accel, ACCEL_OPENGL))
				{
					System.setProperty("sun.java2d.opengl", "True");
					System.setProperty("sun.java2d.d3d", "False");
				}
				else if (string.ReferenceEquals(accel, ACCEL_D3D))
				{
					System.setProperty("sun.java2d.opengl", "False");
					System.setProperty("sun.java2d.d3d", "True");
				}
			}
			catch (Exception)
			{
			}
		}

		//
		// template methods
		//
		public static Template Template
		{
			get
			{
				Prefs;
				switch (templateType)
				{
				case TEMPLATE_PLAIN:
					return PlainTemplate;
				case TEMPLATE_EMPTY:
					return EmptyTemplate;
				case TEMPLATE_CUSTOM:
					return CustomTemplate;
				default:
					return PlainTemplate;
				}
			}
		}

		public static Template EmptyTemplate
		{
			get
			{
				if (emptyTemplate == null)
				{
					emptyTemplate = logisim.prefs.Template.createEmpty();
				}
				return emptyTemplate;
			}
		}

		private static Template PlainTemplate
		{
			get
			{
				if (plainTemplate == null)
				{
					ClassLoader ld = typeof(Startup).getClassLoader();
					Stream @in = ld.getResourceAsStream("logisim/default.templ");
					if (@in == null)
					{
						plainTemplate = EmptyTemplate;
					}
					else
					{
						try
						{
							try
							{
								plainTemplate = logisim.prefs.Template.create(@in);
							}
							finally
							{
								@in.Close();
							}
						}
						catch (Exception)
						{
							plainTemplate = EmptyTemplate;
						}
					}
				}
				return plainTemplate;
			}
		}

		private static Template CustomTemplate
		{
			get
			{
				File toRead = templateFile;
				if (customTemplateFile == null || !(customTemplateFile.Equals(toRead)))
				{
					if (toRead == null)
					{
						customTemplate = null;
						customTemplateFile = null;
					}
					else
					{
						FileStream reader = null;
						try
						{
							reader = new FileStream(toRead, FileMode.Open, FileAccess.Read);
							customTemplate = logisim.prefs.Template.create(reader);
							customTemplateFile = templateFile;
						}
						catch (Exception)
						{
							TemplateFile = null;
							customTemplate = null;
							customTemplateFile = null;
						}
						finally
						{
							if (reader != null)
							{
								try
								{
									reader.Close();
								}
								catch (IOException)
								{
								}
							}
						}
					}
				}
				return customTemplate == null ? PlainTemplate : customTemplate;
			}
		}

		//
		// recent projects
		//
		public static List<File> RecentFiles
		{
			get
			{
				return recentProjects.RecentFiles;
			}
		}

		public static void updateRecentFile(File file)
		{
			recentProjects.updateRecent(file);
		}

		//
		// LocalePreference
		//
		private class LocalePreference : PrefMonitorString
		{
			public LocalePreference() : base("locale", "")
			{

				string localeStr = this.get();
				if (!string.ReferenceEquals(localeStr, null) && !localeStr.Equals(""))
				{
					LocaleManager.Locale = Locale.of(localeStr);
				}
				LocaleManager.addLocaleListener(myListener);
				myListener.localeChanged();
			}

			public override void set(string value)
			{
				if (findLocale(value) != null)
				{
					base.set(value);
				}
			}

			internal static Locale findLocale(string lang)
			{
				Locale[] check;
				for (int set = 0; set < 2; set++)
				{
					if (set == 0)
					{
						check = new Locale[] {Locale.getDefault(), Locale.ENGLISH};
					}
					else
					{
						check = Locale.getAvailableLocales();
					}
					for (int i = 0; i < check.Length; i++)
					{
						Locale loc = check[i];
						if (loc != null && loc.getLanguage().Equals(lang))
						{
							return loc;
						}
					}
				}
				return null;
			}
		}
	}

}
