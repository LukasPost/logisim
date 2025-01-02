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
	using ZOrder = draw.util.ZOrder;

	public class ModelRemoveAction : ModelAction
	{
		private IDictionary<CanvasObject, int> removed;

		public ModelRemoveAction(CanvasModel model, CanvasObject removed) : this(model, Collections.singleton(removed))
		{
		}

		public ModelRemoveAction(CanvasModel model, ICollection<CanvasObject> removed) : base(model)
		{
			this.removed = ZOrder.getZIndex(removed, model);
		}

		public override ICollection<CanvasObject> Objects
		{
			get
			{
				return Collections.unmodifiableSet(removed.Keys);
			}
		}

		public override string Name
		{
			get
			{
				return Strings.get("actionRemove", getShapesName(removed.Keys));
			}
		}

		internal override void doSub(CanvasModel model)
		{
			model.removeObjects(removed.Keys);
		}

		internal override void undoSub(CanvasModel model)
		{
			model.addObjects(removed);
		}
	}

}
