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

	using ModelAction = draw.actions.ModelAction;
	using CanvasObject = draw.model.CanvasObject;
	using Action = draw.undo.Action;
	using Circuit = logisim.circuit.Circuit;
	using CircuitMutator = logisim.circuit.CircuitMutator;
	using CircuitTransaction = logisim.circuit.CircuitTransaction;
	using AppearanceElement = logisim.circuit.appear.AppearanceElement;
	using Project = logisim.proj.Project;

	public class CanvasActionAdapter : logisim.proj.Action
	{
		private Circuit circuit;
		private Action canvasAction;
		private bool wasDefault;

		public CanvasActionAdapter(Circuit circuit, Action action)
		{
			this.circuit = circuit;
			this.canvasAction = action;
		}

		public override string Name
		{
			get
			{
				return canvasAction.Name;
			}
		}

		public override void doIt(Project proj)
		{
			wasDefault = circuit.Appearance.DefaultAppearance;
			if (affectsPorts())
			{
				ActionTransaction xn = new ActionTransaction(this, true);
				xn.execute();
			}
			else
			{
				canvasAction.doIt();
			}
		}

		public override void undo(Project proj)
		{
			if (affectsPorts())
			{
				ActionTransaction xn = new ActionTransaction(this, false);
				xn.execute();
			}
			else
			{
				canvasAction.undo();
			}
			circuit.Appearance.DefaultAppearance = wasDefault;
		}

		private bool affectsPorts()
		{
			if (canvasAction is ModelAction)
			{
				foreach (CanvasObject o in ((ModelAction) canvasAction).Objects)
				{
					if (o is AppearanceElement)
					{
						return true;
					}
				}
			}
			return false;
		}

		private class ActionTransaction : CircuitTransaction
		{
			private readonly CanvasActionAdapter outerInstance;

			internal bool forward;

			internal ActionTransaction(CanvasActionAdapter outerInstance, bool forward)
			{
				this.outerInstance = outerInstance;
				this.forward = forward;
			}

			protected internal override IDictionary<Circuit, int> AccessedCircuits
			{
				get
				{
					IDictionary<Circuit, int> accessMap = new Dictionary<Circuit, int>();
					foreach (Circuit supercirc in outerInstance.circuit.CircuitsUsingThis)
					{
						accessMap[supercirc] = READ_WRITE;
					}
					return accessMap;
				}
			}

			protected internal override void run(CircuitMutator mutator)
			{
				if (forward)
				{
					outerInstance.canvasAction.doIt();
				}
				else
				{
					outerInstance.canvasAction.undo();
				}
			}

		}
	}

}
