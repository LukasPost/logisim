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
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using logisim.data;
	using AttrTableSetException = logisim.gui.generic.AttrTableSetException;
	using AttributeSetTableModel = logisim.gui.generic.AttributeSetTableModel;
	using Project = logisim.proj.Project;

	public class AttrTableCircuitModel : AttributeSetTableModel
	{
		private Project proj;
		private Circuit circ;

		public AttrTableCircuitModel(Project proj, Circuit circ) : base(circ.StaticAttributes)
		{
			this.proj = proj;
			this.circ = circ;
		}

		public override string Title
		{
			get
			{
				return Strings.get("circuitAttrTitle", circ.Name);
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void setValueRequested(logisim.data.Attribute attr, Object value) throws logisim.gui.generic.AttrTableSetException
		protected internal override void setValueRequested(Attribute attr, object value)
		{
			if (!proj.LogisimFile.contains(circ))
			{
				string msg = Strings.get("cannotModifyCircuitError");
				throw new AttrTableSetException(msg);
			}
			else
			{
				CircuitMutation xn = new CircuitMutation(circ);
				xn.setForCircuit(attr, value);
				proj.doAction(xn.toAction(Strings.getter("changeCircuitAttrAction")));
			}
		}
	}

}
