// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using ComponentFactory = logisim.comp.ComponentFactory;
	using Icons = logisim.util.Icons;
	using StringGetter = logisim.util.StringGetter;

	/// <summary>
	/// This class allows an object to be created holding all the information essential to showing a ComponentFactory in the
	/// explorer window, but without actually loading the ComponentFactory unless a program genuinely gets around to needing
	/// to use it. Note that for this to work, the relevant ComponentFactory class must be in the same package as its Library
	/// class, the ComponentFactory class must be public, and it must include a public no-arguments constructor.
	/// </summary>
	public class FactoryDescription
	{
		public static IList<Tool> getTools(Type @base, FactoryDescription[] descriptions)
		{
			Tool[] tools = new Tool[descriptions.Length];
			for (int i = 0; i < tools.Length; i++)
			{
				tools[i] = new AddTool(@base, descriptions[i]);
			}
			return new List<Tool> {tools};
		}

		private string name;
		private StringGetter displayName;
		private string iconName;
		private bool iconLoadAttempted;
		private Icon icon;
		private string factoryClassName;
		private bool factoryLoadAttempted;
		private ComponentFactory factory;
		private StringGetter toolTip;

		public FactoryDescription(string name, StringGetter displayName, string iconName, string factoryClassName) : this(name, displayName, factoryClassName)
		{
			this.iconName = iconName;
			this.iconLoadAttempted = false;
			this.icon = null;
		}

		public FactoryDescription(string name, StringGetter displayName, Icon icon, string factoryClassName) : this(name, displayName, factoryClassName)
		{
			this.iconName = "???";
			this.iconLoadAttempted = true;
			this.icon = icon;
		}

		public FactoryDescription(string name, StringGetter displayName, string factoryClassName)
		{
			this.name = name;
			this.displayName = displayName;
			this.iconName = "???";
			this.iconLoadAttempted = true;
			this.icon = null;
			this.factoryClassName = factoryClassName;
			this.factoryLoadAttempted = false;
			this.factory = null;
			this.toolTip = null;
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual string DisplayName
		{
			get
			{
				return displayName.get();
			}
		}

		public virtual bool FactoryLoaded
		{
			get
			{
				return factoryLoadAttempted;
			}
		}

		public virtual Icon Icon
		{
			get
			{
				Icon ret = icon;
				if (ret != null || iconLoadAttempted)
				{
					return ret;
				}
				else
				{
					ret = Icons.getIcon(iconName);
					icon = ret;
					iconLoadAttempted = true;
					return ret;
				}
			}
		}

		public virtual ComponentFactory getFactory(Type libraryClass)
		{
			ComponentFactory ret = factory;
			if (factory != null || factoryLoadAttempted)
			{
				return ret;
			}
			else
			{
				string msg = "";
				try
				{
					msg = "getting class loader";
					ClassLoader loader = libraryClass.getClassLoader();
					msg = "getting package name";
					string name;
					Package pack = libraryClass.Assembly;
					if (pack == null)
					{
						name = factoryClassName;
					}
					else
					{
						name = pack.getName() + "." + factoryClassName;
					}
					msg = "loading class";
					Type factoryClass = loader.loadClass(name);
					msg = "creating instance";
					object factoryValue = factoryClass.GetConstructor().newInstance();
					msg = "converting to factory";
					if (factoryValue is ComponentFactory)
					{
						ret = (ComponentFactory) factoryValue;
						factory = ret;
						factoryLoadAttempted = true;
						return ret;
					}
				}
				catch (Exception t)
				{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					string name = t.GetType().FullName;
					string m = t.Message;
					if (!string.ReferenceEquals(m, null))
					{
						msg = msg + ": " + name + ": " + m;
					}
					else
					{
						msg = msg + ": " + name;
					}
				}
				Console.Error.WriteLine("error while " + msg); // OK
				factory = null;
				factoryLoadAttempted = true;
				return null;
			}
		}

		public virtual FactoryDescription setToolTip(StringGetter getter)
		{
			toolTip = getter;
			return this;
		}

		public virtual string ToolTip
		{
			get
			{
				StringGetter getter = toolTip;
				return getter == null ? null : getter.get();
			}
		}
	}

}
