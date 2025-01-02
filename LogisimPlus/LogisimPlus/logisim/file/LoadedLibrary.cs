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

	using Circuit = logisim.circuit.Circuit;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Project = logisim.proj.Project;
	using Projects = logisim.proj.Projects;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;
	using logisim.util;

	public class LoadedLibrary : Library, LibraryEventSource
	{
		private class MyListener : LibraryListener
		{
			private readonly LoadedLibrary outerInstance;

			public MyListener(LoadedLibrary outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				outerInstance.fireLibraryEvent(@event);
			}
		}

		private Library @base;
		private bool dirty;
		private MyListener myListener;
		private EventSourceWeakSupport<LibraryListener> listeners;

		internal LoadedLibrary(Library @base)
		{
			dirty = false;
			myListener = new MyListener(this);
			listeners = new EventSourceWeakSupport<LibraryListener>();

			while (@base is LoadedLibrary)
			{
				@base = ((LoadedLibrary) @base).@base;
			}
			this.@base = @base;
			if (@base is LibraryEventSource)
			{
				((LibraryEventSource) @base).addLibraryListener(myListener);
			}
		}

		public virtual void addLibraryListener(LibraryListener l)
		{
			listeners.add(l);
		}

		public virtual void removeLibraryListener(LibraryListener l)
		{
			listeners.remove(l);
		}

		public override string Name
		{
			get
			{
				return @base.Name;
			}
		}

		public override string DisplayName
		{
			get
			{
				return @base.DisplayName;
			}
		}

		public override bool Dirty
		{
			get
			{
				return dirty || @base.Dirty;
			}
			set
			{
				if (dirty != value)
				{
					dirty = value;
					fireLibraryEvent(LibraryEvent.DIRTY_STATE, Dirty ? true : false);
				}
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<? extends logisim.tools.Tool> getTools()
		public override IList<Tool> Tools
		{
			get
			{
				return @base.Tools;
			}
		}

		public override IList<Library> Libraries
		{
			get
			{
				return @base.Libraries;
			}
		}


		internal virtual Library Base
		{
			get
			{
				return @base;
			}
			set
			{
				if (@base is LibraryEventSource)
				{
					((LibraryEventSource) @base).removeLibraryListener(myListener);
				}
				Library old = @base;
				@base = value;
				resolveChanges(old);
				if (@base is LibraryEventSource)
				{
					((LibraryEventSource) @base).addLibraryListener(myListener);
				}
			}
		}


		private void fireLibraryEvent(int action, object data)
		{
			fireLibraryEvent(new LibraryEvent(this, action, data));
		}

		private void fireLibraryEvent(LibraryEvent @event)
		{
			if (@event.Source != this)
			{
				@event = new LibraryEvent(this, @event.Action, @event.Data);
			}
			foreach (LibraryListener l in listeners)
			{
				l.libraryChanged(@event);
			}
		}

		private void resolveChanges(Library old)
		{
			if (listeners.Empty)
			{
				return;
			}

			if (!@base.DisplayName.Equals(old.DisplayName))
			{
				fireLibraryEvent(LibraryEvent.SET_NAME, @base.DisplayName);
			}

			HashSet<Library> changes = new HashSet<Library>(old.Libraries);
			changes.RemoveAll(@base.Libraries);
			foreach (Library lib in changes)
			{
				fireLibraryEvent(LibraryEvent.REMOVE_LIBRARY, lib);
			}

			changes.Clear();
			changes.addAll(@base.Libraries);
			changes.RemoveAll(old.Libraries);
			foreach (Library lib in changes)
			{
				fireLibraryEvent(LibraryEvent.ADD_LIBRARY, lib);
			}

			Dictionary<ComponentFactory, ComponentFactory> componentMap;
			Dictionary<Tool, Tool> toolMap;
			componentMap = new Dictionary<ComponentFactory, ComponentFactory>();
			toolMap = new Dictionary<Tool, Tool>();
			foreach (Tool oldTool in old.Tools)
			{
				Tool newTool = @base.getTool(oldTool.Name);
				toolMap[oldTool] = newTool;
				if (oldTool is AddTool)
				{
					ComponentFactory oldFactory = ((AddTool) oldTool).Factory;
					if (newTool != null && newTool is AddTool)
					{
						ComponentFactory newFactory = ((AddTool) newTool).Factory;
						componentMap[oldFactory] = newFactory;
					}
					else
					{
						componentMap[oldFactory] = null;
					}
				}
			}
			replaceAll(componentMap, toolMap);

			HashSet<Tool> toolChanges = new HashSet<Tool>(old.Tools);
			toolChanges.RemoveAll(toolMap.Keys);
			foreach (Tool tool in toolChanges)
			{
				fireLibraryEvent(LibraryEvent.REMOVE_TOOL, tool);
			}

			toolChanges = new HashSet<Tool>(@base.Tools);
			toolChanges.RemoveAll(toolMap.Values);
			foreach (Tool tool in toolChanges)
			{
				fireLibraryEvent(LibraryEvent.ADD_TOOL, tool);
			}
		}

		private static void replaceAll(IDictionary<ComponentFactory, ComponentFactory> compMap, IDictionary<Tool, Tool> toolMap)
		{
			foreach (Project proj in Projects.OpenProjects)
			{
				Tool oldTool = proj.Tool;
				Circuit oldCircuit = proj.CurrentCircuit;
				if (toolMap.ContainsKey(oldTool))
				{
					proj.Tool = toolMap[oldTool];
				}
				SubcircuitFactory oldFactory = oldCircuit.SubcircuitFactory;
				if (compMap.ContainsKey(oldFactory))
				{
					SubcircuitFactory newFactory;
					newFactory = (SubcircuitFactory) compMap[oldFactory];
					proj.CurrentCircuit = newFactory.Subcircuit;
				}
				replaceAll(proj.LogisimFile, compMap, toolMap);
			}
			foreach (LogisimFile file in LibraryManager.instance.LogisimLibraries)
			{
				replaceAll(file, compMap, toolMap);
			}
		}

		private static void replaceAll(LogisimFile file, IDictionary<ComponentFactory, ComponentFactory> compMap, IDictionary<Tool, Tool> toolMap)
		{
			file.Options.getToolbarData().replaceAll(toolMap);
			file.Options.getMouseMappings().replaceAll(toolMap);
			foreach (Circuit circuit in file.Circuits)
			{
				replaceAll(circuit, compMap);
			}
		}

		private static void replaceAll(Circuit circuit, IDictionary<ComponentFactory, ComponentFactory> compMap)
		{
			List<Component> toReplace = null;
			foreach (Component comp in circuit.NonWires)
			{
				if (compMap.ContainsKey(comp.Factory))
				{
					if (toReplace == null)
					{
						toReplace = new List<Component>();
					}
					toReplace.Add(comp);
				}
			}
			if (toReplace != null)
			{
				CircuitMutation xn = new CircuitMutation(circuit);
				foreach (Component comp in toReplace)
				{
					xn.remove(comp);
					ComponentFactory factory = compMap[comp.Factory];
					if (factory != null)
					{
						AttributeSet newAttrs = createAttributes(factory, comp.AttributeSet);
						xn.add(factory.createComponent(comp.Location, newAttrs));
					}
				}
				xn.execute();
			}
		}

		private static AttributeSet createAttributes(ComponentFactory factory, AttributeSet src)
		{
			AttributeSet dest = factory.createAttributeSet();
			copyAttributes(dest, src);
			return dest;
		}

		internal static void copyAttributes(AttributeSet dest, AttributeSet src)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> destAttr : dest.getAttributes())
			foreach (Attribute<object> destAttr in dest.Attributes)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> srcAttr = src.getAttribute(destAttr.getName());
				Attribute<object> srcAttr = src.getAttribute(destAttr.Name);
				if (srcAttr != null)
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<Object> destAttr2 = (logisim.data.Attribute<Object>) destAttr;
					Attribute<object> destAttr2 = (Attribute<object>) destAttr;
					dest.setValue(destAttr2, src.getValue(srcAttr));
				}
			}
		}
	}

}
