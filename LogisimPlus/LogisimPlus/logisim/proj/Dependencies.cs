// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.proj
{
	using Circuit = logisim.circuit.Circuit;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using LogisimFile = logisim.file.LogisimFile;
	using AddTool = logisim.tools.AddTool;
	using logisim.util;

	public class Dependencies
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class MyListener : LibraryListener, CircuitListener
		{
			private readonly Dependencies outerInstance;

			public MyListener(Dependencies outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void libraryChanged(LibraryEvent e)
			{
				switch (e.Action)
				{
				case LibraryEvent.ADD_TOOL:
					if (e.Data is AddTool)
					{
						ComponentFactory factory = ((AddTool) e.Data).Factory;
						if (factory is SubcircuitFactory)
						{
							SubcircuitFactory circFact = (SubcircuitFactory) factory;
							outerInstance.processCircuit(circFact.Subcircuit);
						}
					}
					break;
				case LibraryEvent.REMOVE_TOOL:
					if (e.Data is AddTool)
					{
						ComponentFactory factory = ((AddTool) e.Data).Factory;
						if (factory is SubcircuitFactory)
						{
							SubcircuitFactory circFact = (SubcircuitFactory) factory;
							Circuit circ = circFact.Subcircuit;
							outerInstance.depends.removeNode(circ);
							circ.removeCircuitListener(this);
						}
					}
					break;
				}
			}

			public virtual void circuitChanged(CircuitEvent e)
			{
				Component comp;
				switch (e.Action)
				{
				case CircuitEvent.ACTION_ADD:
					comp = (Component) e.Data;
					if (comp.Factory is SubcircuitFactory)
					{
						SubcircuitFactory factory = (SubcircuitFactory) comp.Factory;
						outerInstance.depends.addEdge(e.Circuit, factory.Subcircuit);
					}
					break;
				case CircuitEvent.ACTION_REMOVE:
					comp = (Component) e.Data;
					if (comp.Factory is SubcircuitFactory)
					{
						SubcircuitFactory factory = (SubcircuitFactory) comp.Factory;
						bool found = false;
						foreach (Component o in e.Circuit.NonWires)
						{
							if (o.Factory == factory)
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							outerInstance.depends.removeEdge(e.Circuit, factory.Subcircuit);
						}
					}
					break;
				case CircuitEvent.ACTION_CLEAR:
					outerInstance.depends.removeNode(e.Circuit);
					break;
				}
			}
		}

		private MyListener myListener;
		private Dag<Circuit> depends = new Dag<Circuit>();

		internal Dependencies(LogisimFile file)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			addDependencies(file);
		}

		public virtual bool canRemove(Circuit circ)
		{
			return !depends.hasPredecessors(circ);
		}

		public virtual bool canAdd(Circuit circ, Circuit sub)
		{
			return depends.canFollow(sub, circ);
		}

		private void addDependencies(LogisimFile file)
		{
			file.addLibraryListener(myListener);
			foreach (Circuit circuit in file.Circuits)
			{
				processCircuit(circuit);
			}
		}

		private void processCircuit(Circuit circ)
		{
			circ.addCircuitListener(myListener);
			foreach (Component comp in circ.NonWires)
			{
				if (comp.Factory is SubcircuitFactory)
				{
					SubcircuitFactory factory = (SubcircuitFactory) comp.Factory;
					depends.addEdge(circ, factory.Subcircuit);
				}
			}
		}

	}

}
