// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{
	using Circuit = logisim.circuit.Circuit;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using logisim.data;
	using AttrTableSetException = logisim.gui.generic.AttrTableSetException;
	using AttributeSetTableModel = logisim.gui.generic.AttributeSetTableModel;
	using Event = logisim.gui.main.Selection.Event;
	using Project = logisim.proj.Project;
	using SetAttributeAction = logisim.tools.SetAttributeAction;

	internal class AttrTableSelectionModel : AttributeSetTableModel, Selection.Listener
	{
		private Project project;
		private Frame frame;

		public AttrTableSelectionModel(Project project, Frame frame) : base(frame.Canvas.Selection.AttributeSet)
		{
			this.project = project;
			this.frame = frame;
			frame.Canvas.Selection.addListener(this);
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") @Override public String getTitle()
		public override string Title
		{
			get
			{
				ComponentFactory wireFactory = null;
				ComponentFactory factory = null;
				int factoryCount = 0;
				int totalCount = 0;
				bool variousFound = false;
    
				Selection selection = frame.Canvas.Selection;
				foreach (Component comp in selection.Components)
				{
					ComponentFactory fact = comp.Factory;
					if (fact == factory)
					{
						factoryCount++;
					}
					else if (comp is Wire)
					{
						wireFactory = fact;
						if (factory == null)
						{
							factoryCount++;
						}
					}
					else if (factory == null)
					{
						factory = fact;
						factoryCount = 1;
					}
					else
					{
						variousFound = true;
					}
					if (!(comp is Wire))
					{
						totalCount++;
					}
				}
    
				if (factory == null)
				{
					factory = wireFactory;
				}
    
				if (variousFound)
				{
					return Strings.get("selectionVarious", "" + totalCount);
				}
				else if (factoryCount == 0)
				{
					string circName = frame.Canvas.Circuit.Name;
					return Strings.get("circuitAttrTitle", circName);
				}
				else if (factoryCount == 1)
				{
					return Strings.get("selectionOne", factory.DisplayName);
				}
				else
				{
					return Strings.get("selectionMultiple", factory.DisplayName, "" + factoryCount);
				}
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void setValueRequested(logisim.data.Attribute<Object> attr, Object value) throws logisim.gui.generic.AttrTableSetException
		public override void setValueRequested(Attribute<object> attr, object value)
		{
			Selection selection = frame.Canvas.Selection;
			Circuit circuit = frame.Canvas.Circuit;
			if (selection.Empty && circuit != null)
			{
				AttrTableCircuitModel circuitModel = new AttrTableCircuitModel(project, circuit);
				circuitModel.setValueRequested(attr, value);
			}
			else
			{
				SetAttributeAction act = new SetAttributeAction(circuit, Strings.getter("selectionAttributeAction"));
				foreach (Component comp in selection.Components)
				{
					if (!(comp is Wire))
					{
						act.set(comp, attr, value);
					}
				}
				project.doAction(act);
			}
		}

		//
		// Selection.Listener methods
		public virtual void selectionChanged(Event @event)
		{
			fireTitleChanged();
			frame.AttrTableModel = this;
		}
	}

}
