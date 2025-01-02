// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{

	using CanvasObject = draw.model.CanvasObject;
	using Circuit = logisim.circuit.Circuit;
	using CircuitAppearance = logisim.circuit.appear.CircuitAppearance;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;

	public class RevertAppearanceAction : Action
	{
		private Circuit circuit;
		private List<CanvasObject> old;
		private bool wasDefault;

		public RevertAppearanceAction(Circuit circuit)
		{
			this.circuit = circuit;
		}

		public override string Name
		{
			get
			{
				return Strings.get("revertAppearanceAction");
			}
		}

		public override void doIt(Project proj)
		{
			CircuitAppearance appear = circuit.Appearance;
			wasDefault = appear.DefaultAppearance;
			old = new List<CanvasObject>(appear.ObjectsFromBottom);
			appear.DefaultAppearance = true;
		}

		public override void undo(Project proj)
		{
			CircuitAppearance appear = circuit.Appearance;
			appear.ObjectsForce = old;
			appear.DefaultAppearance = wasDefault;
		}
	}

}
