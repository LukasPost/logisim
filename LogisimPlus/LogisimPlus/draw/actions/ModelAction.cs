// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.actions
{

	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using Action = draw.undo.Action;

	public abstract class ModelAction : Action
	{
		private CanvasModel model;

		public ModelAction(CanvasModel model)
		{
			this.model = model;
		}

		public virtual ICollection<CanvasObject> Objects
		{
			get
			{
				return Collections.emptySet();
			}
		}

		public override abstract string Name {get;}

		internal abstract void doSub(CanvasModel model);

		internal abstract void undoSub(CanvasModel model);

		public override sealed void doIt()
		{
			doSub(model);
		}

		public override sealed void undo()
		{
			undoSub(model);
		}

		public virtual CanvasModel Model
		{
			get
			{
				return model;
			}
		}

		internal static string getShapesName(ICollection<CanvasObject> coll)
		{
			if (coll.Count != 1)
			{
				return Strings.get("shapeMultiple");
			}
			else
			{
				CanvasObject shape = coll.GetEnumerator().next();
				return shape.DisplayName;
			}
		}
	}

}
