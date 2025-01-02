// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class LocaleManager
	{
		// static members
		private const string SETTINGS_NAME = "settings";
		private static List<LocaleManager> managers = new List<LocaleManager>();

		private class LocaleGetter : StringGetter
		{
			internal LocaleManager source;
			internal string key;

			internal LocaleGetter(LocaleManager source, string key)
			{
				this.source = source;
				this.key = key;
			}

			public virtual string get()
			{
				return source.get(key);
			}

			public override string ToString()
			{
				return get();
			}
		}

		private static List<LocaleListener> listeners = new List<LocaleListener>();
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private static bool replaceAccents_Conflict = false;
		private static Dictionary<char, string> repl = null;
		private static Locale curLocale = null;

		public static Locale Locale
		{
			get
			{
				Locale ret = curLocale;
				if (ret == null)
				{
					ret = Locale.getDefault();
					curLocale = ret;
				}
				return ret;
			}
			set
			{
				Locale cur = Locale;
				if (!value.Equals(cur))
				{
					Locale[] opts = Strings.LocaleManager.LocaleOptions;
					Locale select = null;
					Locale backup = null;
					string locLang = value.getLanguage();
					foreach (Locale opt in opts)
					{
						if (select == null && opt.Equals(value))
						{
							select = opt;
						}
						if (backup == null && opt.getLanguage().Equals(locLang))
						{
							backup = opt;
						}
					}
					if (select == null)
					{
						if (backup == null)
						{
							select = Locale.of("en");
						}
						else
						{
							select = backup;
						}
					}
    
					curLocale = select;
					Locale.setDefault(select);
					foreach (LocaleManager man in managers)
					{
						man.loadDefault();
					}
					repl = replaceAccents_Conflict ? fetchReplaceAccents() : null;
					fireLocaleChanged();
				}
			}
		}


		public static bool canReplaceAccents()
		{
			return fetchReplaceAccents() != null;
		}

		public static bool ReplaceAccents
		{
			set
			{
				Dictionary<char, string> newRepl = value ? fetchReplaceAccents() : null;
				replaceAccents_Conflict = value;
				repl = newRepl;
				fireLocaleChanged();
			}
		}

		private static Dictionary<char, string> fetchReplaceAccents()
		{
			Dictionary<char, string> ret = null;
			string val;
			try
			{
				val = Strings.source.locale.getString("accentReplacements");
			}
			catch (MissingResourceException)
			{
				return null;
			}
			StringTokenizer toks = new StringTokenizer(val, "/");
			while (toks.hasMoreTokens())
			{
				string tok = toks.nextToken().Trim();
				char c = '\0';
				string s = null;
				if (tok.Length == 1)
				{
					c = tok[0];
					s = "";
				}
				else if (tok.Length >= 2 && tok[1] == ' ')
				{
					c = tok[0];
					s = tok.Substring(2).Trim();
				}
				if (!string.ReferenceEquals(s, null))
				{
					if (ret == null)
					{
						ret = new Dictionary<char, string>();
					}
					ret[Convert.ToChar(c)] = s;
				}
			}
			return ret;
		}

		public static void addLocaleListener(LocaleListener l)
		{
			listeners.Add(l);
		}

		public static void removeLocaleListener(LocaleListener l)
		{
			listeners.Remove(l);
		}

		private static void fireLocaleChanged()
		{
			foreach (LocaleListener l in listeners)
			{
				l.localeChanged();
			}
		}

		// instance members
		private static string dir_name = "logisim";
		private string file_start;
		private ResourceBundle settings = null;
		private ResourceBundle locale = null;
		private ResourceBundle dflt_locale = null;

		public LocaleManager(string file_start)
		{
			this.file_start = file_start;
			loadDefault();
			managers.Add(this);
		}

		private void loadDefault()
		{
			if (settings == null)
			{
				try
				{
					settings = ResourceBundle.getBundle(dir_name + "." + SETTINGS_NAME);
				}
				catch (MissingResourceException)
				{
				}
			}

			try
			{
				loadLocale(Locale.getDefault());
				if (locale != null)
				{
					return;
				}
			}
			catch (MissingResourceException)
			{
			}
			try
			{
				loadLocale(Locale.ENGLISH);
				if (locale != null)
				{
					return;
				}
			}
			catch (MissingResourceException)
			{
			}
			Locale[] choices = LocaleOptions;
			if (choices != null && choices.Length > 0)
			{
				loadLocale(choices[0]);
			}
			if (locale != null)
			{
				return;
			}
			throw new Exception("No locale bundles are available");
		}

		private void loadLocale(Locale loc)
		{
			string bundleName = dir_name + "." + loc.getLanguage() + "." + file_start;
			locale = ResourceBundle.getBundle(bundleName);
		}

		public virtual string get(string key)
		{
			string ret;
			try
			{
				ret = locale.getString(key);
			}
			catch (MissingResourceException)
			{
				ResourceBundle backup = dflt_locale;
				if (backup == null)
				{
					Locale backup_loc = Locale.US;
					backup = ResourceBundle.getBundle(dir_name + ".en." + file_start, backup_loc);
					dflt_locale = backup;
				}
				try
				{
					ret = backup.getString(key);
				}
				catch (MissingResourceException)
				{
					ret = key;
				}
			}
			Dictionary<char, string> repl = LocaleManager.repl;
			if (repl != null)
			{
				ret = replaceAccents(ret, repl);
			}
			return ret;
		}

		public virtual StringGetter getter(string key)
		{
			return new LocaleGetter(this, key);
		}

		public virtual StringGetter getter(string key, string arg)
		{
			return StringUtil.formatter(getter(key), arg);
		}

		public virtual StringGetter getter(string key, StringGetter arg)
		{
			return StringUtil.formatter(getter(key), arg);
		}

		public virtual Locale[] LocaleOptions
		{
			get
			{
				string locs = null;
				try
				{
					if (settings != null)
					{
						locs = settings.getString("locales");
					}
				}
				catch (MissingResourceException)
				{
				}
				if (string.ReferenceEquals(locs, null))
				{
					return new Locale[] {};
				}
    
				List<Locale> retl = new List<Locale>();
				StringTokenizer toks = new StringTokenizer(locs);
				while (toks.hasMoreTokens())
				{
					string f = toks.nextToken();
					string language;
					string country;
					if (f.Length >= 2)
					{
						language = f.Substring(0, 2);
						country = (f.Length >= 5 ? f.Substring(3, 2) : null);
					}
					else
					{
						language = null;
						country = null;
					}
					if (!string.ReferenceEquals(language, null))
					{
						Locale loc = string.ReferenceEquals(country, null) ? Locale.of(language) : Locale.of(language, country);
						retl.Add(loc);
					}
				}
    
				return retl.ToArray();
			}
		}

		public virtual JComponent createLocaleSelector()
		{
			Locale[] locales = LocaleOptions;
			if (locales == null || locales.Length == 0)
			{
				Locale cur = Locale;
				if (cur == null)
				{
					cur = Locale.of("en");
				}
				locales = new Locale[] {cur};
			}
			return new JScrollPane(new LocaleSelector(locales));
		}

		private static string replaceAccents(string src, Dictionary<char, string> repl)
		{
			// find first non-standard character - so we can avoid the
			// replacement process if possible
			int i = 0;
			int n = src.Length;
			for (; i < n; i++)
			{
				char ci = src[i];
				if (ci < (char)32 || ci >= (char)127)
				{
					break;
				}
			}
			if (i == n)
			{
				return src;
			}

			// ok, we'll have to consider replacing accents
			char[] cs = src.ToCharArray();
			StringBuilder ret = new StringBuilder(src.Substring(0, i));
			for (int j = i; j < cs.Length; j++)
			{
				char cj = cs[j];
				if (cj < (char)32 || cj >= (char)127)
				{
					string @out = repl[Convert.ToChar(cj)];
					if (!string.ReferenceEquals(@out, null))
					{
						ret.Append(@out);
					}
					else
					{
						ret.Append(cj);
					}
				}
				else
				{
					ret.Append(cj);
				}
			}
			return ret.ToString();
		}
	}

}
