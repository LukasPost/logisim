// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{
	using Circuit = logisim.circuit.Circuit;
	using Component = logisim.comp.Component;
	using logisim.data;
	using AttrTableSetException = logisim.gui.generic.AttrTableSetException;
	using AttributeSetTableModel = logisim.gui.generic.AttributeSetTableModel;
	using Project = logisim.proj.Project;
	using SetAttributeAction = logisim.tools.SetAttributeAction;

	internal class AttrTableComponentModel : AttributeSetTableModel
	{
		internal Project proj;
		internal Circuit circ;
		internal Component comp;

		internal AttrTableComponentModel(Project proj, Circuit circ, Component comp) : base(comp.AttributeSet)
		{
			this.proj = proj;
			this.circ = circ;
			this.comp = comp;
		}

		public virtual Circuit Circuit
		{
			get
			{
				return circ;
			}
		}

		public virtual Component Component
		{
			get
			{
				return comp;
			}
		}

		public override string Title
		{
			get
			{
				return comp.Factory.DisplayName;
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void setValueRequested(logisim.data.Attribute<Object> attr, Object value) throws logisim.gui.generic.AttrTableSetException
		public override void setValueRequested(Attribute<object> attr, object value)
		{
			if (!proj.LogisimFile.contains(circ))
			{
				string msg = Strings.get("cannotModifyCircuitError");
				throw new AttrTableSetException(msg);
			}
			else
			{
				SetAttributeAction act = new SetAttributeAction(circ, Strings.getter("changeAttributeAction"));
				act.set(comp, attr, value);
				proj.doAction(act);
			}
		}
	}

}
