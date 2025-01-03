// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{


	using SAXException = org.xml.sax.SAXException;

	using Circuit = logisim.circuit.Circuit;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using Projects = logisim.proj.Projects;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;
	using logisim.util;
	using ListUtil = logisim.util.ListUtil;
	using StringUtil = logisim.util.StringUtil;

	public class LogisimFile : Library, LibraryEventSource
	{
		private class WritingThread : Thread
		{
			internal Stream @out;
			internal LogisimFile file;

			internal WritingThread(Stream @out, LogisimFile file)
			{
				this.@out = @out;
				this.file = file;
			}

			public override void run()
			{
				try
				{
					file.write(@out, file.loader);
				}
				catch (IOException e)
				{
					file.loader.showError(StringUtil.format(Strings.get("fileDuplicateError"), e.ToString()));
				}
				try
				{
					@out.Close();
				}
				catch (IOException e)
				{
					file.loader.showError(StringUtil.format(Strings.get("fileDuplicateError"), e.ToString()));
				}
			}
		}

		private EventSourceWeakSupport<LibraryListener> listeners = new EventSourceWeakSupport<LibraryListener>();
		private Loader loader;
		private LinkedList<string> messages = new LinkedList<string>();
		private Options options = new Options();
		private LinkedList<AddTool> tools = new LinkedList<AddTool>();
		private LinkedList<Library> libraries = new LinkedList<Library>();
		private Circuit main = null;
		private string name;
		private bool dirty = false;

		internal LogisimFile(Loader loader)
		{
			this.loader = loader;

			name = Strings.get("defaultProjectName");
			if (Projects.windowNamed(name))
			{
				for (int i = 2; true; i++)
				{
					if (!Projects.windowNamed(name + " " + i))
					{
						name += " " + i;
						break;
					}
				}
			}

		}

		//
		// access methods
		//
		public override string Name
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
				fireEvent(LibraryEvent.SET_NAME, value);
			}
		}

		public override bool Dirty
		{
			get
			{
				return dirty;
			}
			set
			{
				if (dirty != value)
				{
					dirty = value;
					fireEvent(LibraryEvent.DIRTY_STATE, value ? true : false);
				}
			}
		}

		public virtual string Message
		{
			get
			{
				if (messages.Count == 0)
				{
					return null;
				}
				return messages.RemoveFirst();
			}
		}

		public virtual Loader Loader
		{
			get
			{
				return loader;
			}
		}

		public virtual Options Options
		{
			get
			{
				return options;
			}
		}

		public override List<AddTool> Tools
		{
			get
			{
				return tools;
			}
		}

		public override List<Library> Libraries
		{
			get
			{
				return libraries;
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<?> getElements()
		public override List<object> Elements
		{
			get
			{
				return ListUtil.joinImmutableLists(tools, libraries);
			}
		}

		public virtual Circuit getCircuit(string name)
		{
			if (string.ReferenceEquals(name, null))
			{
				return null;
			}
			foreach (AddTool tool in tools)
			{
				SubcircuitFactory factory = (SubcircuitFactory) tool.Factory;
				if (name.Equals(factory.Name))
				{
					return factory.Subcircuit;
				}
			}
			return null;
		}

		public virtual bool contains(Circuit circ)
		{
			foreach (AddTool tool in tools)
			{
				SubcircuitFactory factory = (SubcircuitFactory) tool.Factory;
				if (factory.Subcircuit == circ)
				{
					return true;
				}
			}
			return false;
		}

		public virtual List<Circuit> Circuits
		{
			get
			{
				List<Circuit> ret = new List<Circuit>(tools.Count);
				foreach (AddTool tool in tools)
				{
					SubcircuitFactory factory = (SubcircuitFactory) tool.Factory;
					ret.Add(factory.Subcircuit);
				}
				return ret;
			}
		}

		public virtual AddTool getAddTool(Circuit circ)
		{
			foreach (AddTool tool in tools)
			{
				SubcircuitFactory factory = (SubcircuitFactory) tool.Factory;
				if (factory.Subcircuit == circ)
				{
					return tool;
				}
			}
			return null;
		}

		public virtual Circuit MainCircuit
		{
			get
			{
				return main;
			}
			set
			{
				if (value == null)
				{
					return;
				}
				this.main = value;
				fireEvent(LibraryEvent.SET_MAIN, value);
			}
		}

		public virtual int CircuitCount
		{
			get
			{
				return tools.Count;
			}
		}

		//
		// listener methods
		//
		public virtual void addLibraryListener(LibraryListener what)
		{
			listeners.add(what);
		}

		public virtual void removeLibraryListener(LibraryListener what)
		{
			listeners.remove(what);
		}

		private void fireEvent(int action, object data)
		{
			LibraryEvent e = new LibraryEvent(this, action, data);
			foreach (LibraryListener l in listeners)
			{
				l.libraryChanged(e);
			}
		}

		//
		// modification actions
		//
		public virtual void addMessage(string msg)
		{
			messages.AddLast(msg);
		}



		public virtual void addCircuit(Circuit circuit)
		{
			addCircuit(circuit, tools.Count);
		}

		public virtual void addCircuit(Circuit circuit, int index)
		{
			AddTool tool = new AddTool(circuit.SubcircuitFactory);
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the 2-parameter Java 'add' method:
			tools.add(index, tool);
			if (tools.Count == 1)
			{
				MainCircuit = circuit;
			}
			fireEvent(LibraryEvent.ADD_TOOL, tool);
		}

		public virtual void removeCircuit(Circuit circuit)
		{
			if (tools.Count <= 1)
			{
				throw new Exception("Cannot remove last circuit");
			}

			int index = Circuits.IndexOf(circuit);
			if (index >= 0)
			{
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
				Tool circuitTool = tools.remove(index);

				if (main == circuit)
				{
					AddTool dflt_tool = tools.ToList()[0];
					SubcircuitFactory factory = (SubcircuitFactory) dflt_tool.Factory;
					MainCircuit = factory.Subcircuit;
				}
				fireEvent(LibraryEvent.REMOVE_TOOL, circuitTool);
			}
		}

		public virtual void moveCircuit(AddTool tool, int index)
		{
			int oldIndex = tools.IndexOf(tool);
			if (oldIndex < 0)
			{
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the 2-parameter Java 'add' method:
				tools.add(index, tool);
				fireEvent(LibraryEvent.ADD_TOOL, tool);
			}
			else
			{
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
				AddTool value = tools.remove(oldIndex);
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the 2-parameter Java 'add' method:
				tools.add(index, value);
				fireEvent(LibraryEvent.MOVE_TOOL, tool);
			}
		}

		public virtual void addLibrary(Library lib)
		{
			libraries.AddLast(lib);
			fireEvent(LibraryEvent.ADD_LIBRARY, lib);
		}

		public virtual void removeLibrary(Library lib)
		{
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
			libraries.remove(lib);
			fireEvent(LibraryEvent.REMOVE_LIBRARY, lib);
		}

		public virtual string getUnloadLibraryMessage(Library lib)
		{
			HashSet<ComponentFactory> factories = new HashSet<ComponentFactory>();
			foreach (Tool tool in lib.Tools)
			{
				if (tool is AddTool)
				{
					factories.Add(((AddTool) tool).Factory);
				}
			}

			foreach (Circuit circuit in Circuits)
			{
				foreach (Component comp in circuit.NonWires)
				{
					if (factories.Contains(comp.Factory))
					{
						return StringUtil.format(Strings.get("unloadUsedError"), circuit.Name);
					}
				}
			}

			ToolbarData tb = options.ToolbarData;
			MouseMappings mm = options.MouseMappings;
			foreach (Tool t in lib.Tools)
			{
				if (tb.usesToolFromSource(t))
				{
					return Strings.get("unloadToolbarError");
				}
				if (mm.usesToolFromSource(t))
				{
					return Strings.get("unloadMappingError");
				}
			}

			return null;
		}


		//
		// other methods
		//
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void write(java.io.OutputStream out, LibraryLoader loader) throws java.io.IOException
		internal virtual void write(Stream @out, LibraryLoader loader)
		{
			try
			{
				XmlWriter.write(this, @out, loader);
			}
			catch (TransformerConfigurationException)
			{
				loader.showError("internal error configuring transformer");
			}
			catch (ParserConfigurationException)
			{
				loader.showError("internal error configuring parser");
			}
			catch (TransformerException e)
			{
				string msg = e.Message;
				string err = Strings.get("xmlConversionError");
				if (string.ReferenceEquals(msg, null))
				{
					err += ": " + msg;
				}
				loader.showError(err);
			}
		}

		public virtual LogisimFile cloneLogisimFile(Loader newloader)
		{
			PipedInputStream reader = new PipedInputStream();
			PipedOutputStream writer = new PipedOutputStream();
			try
			{
				reader.connect(writer);
			}
			catch (IOException e)
			{
				newloader.showError(StringUtil.format(Strings.get("fileDuplicateError"), e.ToString()));
				return null;
			}
			(new WritingThread(writer, this)).Start();
			try
			{
				return LogisimFile.load(reader, newloader);
			}
			catch (IOException e)
			{
				newloader.showError(StringUtil.format(Strings.get("fileDuplicateError"), e.ToString()));
				return null;
			}
		}

		internal virtual Tool findTool(Tool query)
		{
			foreach (Library lib in Libraries)
			{
				Tool ret = findTool(lib, query);
				if (ret != null)
				{
					return ret;
				}
			}
			return null;
		}

		private Tool findTool(Library lib, Tool query)
		{
			foreach (Tool tool in lib.Tools)
			{
				if (tool.Equals(query))
				{
					return tool;
				}
			}
			return null;
		}

		//
		// creation methods
		//
		public static LogisimFile createNew(Loader loader)
		{
			LogisimFile ret = new LogisimFile(loader);
			ret.main = new Circuit("main");
			// The name will be changed in LogisimPreferences
			ret.tools.AddLast(new AddTool(ret.main.SubcircuitFactory));
			return ret;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static LogisimFile load(java.io.File file, Loader loader) throws java.io.IOException
		public static LogisimFile load(File file, Loader loader)
		{
			Stream @in = new FileStream(file, FileMode.Open, FileAccess.Read);
			SAXException firstExcept = null;
			try
			{
				return loadSub(@in, loader);
			}
			catch (SAXException e)
			{
				firstExcept = e;
			}
			finally
			{
				@in.Close();
			}

			if (firstExcept != null)
			{
				// We'll now try to do it using a reader. This is to work around
				// Logisim versions prior to 2.5.1, when files were not saved using
				// UTF-8 as the encoding (though the XML file reported otherwise).
				try
				{
					@in = new ReaderInputStream(new StreamReader(file), "UTF8");
					return loadSub(@in, loader);
				}
				catch (Exception)
				{
					loader.showError(StringUtil.format(Strings.get("xmlFormatError"), firstExcept.ToString()));
				}
				finally
				{
					try
					{
						@in.Close();
					}
					catch (Exception)
					{
					}
				}
			}

			return null;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static LogisimFile load(java.io.InputStream in, Loader loader) throws java.io.IOException
		public static LogisimFile load(Stream @in, Loader loader)
		{
			try
			{
				return loadSub(@in, loader);
			}
			catch (SAXException e)
			{
				loader.showError(StringUtil.format(Strings.get("xmlFormatError"), e.ToString()));
				return null;
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static LogisimFile loadSub(java.io.InputStream in, Loader loader) throws IOException, org.xml.sax.SAXException
		public static LogisimFile loadSub(Stream @in, Loader loader)
		{
			// fetch first line and then reset
			BufferedInputStream inBuffered = new BufferedInputStream(@in);
			string firstLine = getFirstLine(inBuffered);

			if (string.ReferenceEquals(firstLine, null))
			{
				throw new IOException("File is empty");
			}
			else if (firstLine.Equals("Logisim v1.0"))
			{
				// if this is a 1.0 file, then set up a pipe to translate to
				// 2.0 and then interpret as a 2.0 file
				throw new IOException("Version 1.0 files no longer supported");
			}

			XmlReader xmlReader = new XmlReader(loader);
			LogisimFile ret = xmlReader.readLibrary(inBuffered);
			ret.loader = loader;
			return ret;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static String getFirstLine(java.io.BufferedInputStream in) throws java.io.IOException
		private static string getFirstLine(BufferedInputStream @in)
		{
			sbyte[] first = new sbyte[512];
			@in.mark(first.Length - 1);
			@in.read(first);
			@in.reset();

			int lineBreak = first.Length;
			for (int i = 0; i < lineBreak; i++)
			{
				if (first[i] == (sbyte)'\n')
				{
					lineBreak = i;
				}
			}
			return StringHelper.NewString(first, 0, lineBreak, "UTF-8");
		}
	}

}
