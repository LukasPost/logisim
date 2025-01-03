// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using ComponentFactory = logisim.comp.ComponentFactory;
	using ListUtil = logisim.util.ListUtil;

	public abstract class Library
	{
		public virtual string Name
		{
			get
			{
	// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return this.GetType().FullName;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public abstract java.util.List<? extends Tool> getTools();
		public abstract List<Tool> Tools {get;}

		public override string ToString()
		{
			return Name;
		}

		public virtual string DisplayName
		{
			get
			{
				return Name;
			}
		}

		public virtual bool Dirty
		{
			get
			{
				return false;
			}
		}

		public virtual List<Library> Libraries
		{
			get
			{
				return [];
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.List<?> getElements()
		public virtual List<object> Elements
		{
			get
			{
				return ListUtil.joinImmutableLists(Tools, Libraries);
			}
		}

		public virtual Tool getTool(string name)
		{
			foreach (Tool tool in Tools)
			{
				if (tool.Name.Equals(name))
				{
					return tool;
				}
			}
			return null;
		}

		public virtual bool containsFromSource(Tool query)
		{
			foreach (Tool tool in Tools)
			{
				if (tool.sharesSource(query))
				{
					return true;
				}
			}
			return false;
		}

		public virtual int indexOf(ComponentFactory query)
		{
			int index = -1;
			foreach (Tool obj in Tools)
			{
				index++;
				if (obj is AddTool)
				{
					AddTool tool = (AddTool) obj;
					if (tool.Factory == query)
					{
						return index;
					}
				}
			}
			return -1;
		}

		public virtual bool contains(ComponentFactory query)
		{
			return indexOf(query) >= 0;
		}

		public virtual Library getLibrary(string name)
		{
			foreach (Library lib in Libraries)
			{
				if (lib.Name.Equals(name))
				{
					return lib;
				}
			}
			return null;
		}

	}

}
