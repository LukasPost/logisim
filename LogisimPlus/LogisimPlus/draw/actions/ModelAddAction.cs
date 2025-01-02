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

	public class ModelAddAction : ModelAction
	{
		private List<CanvasObject> added;
		private int addIndex;

		public ModelAddAction(CanvasModel model, CanvasObject added) : this(model, Collections.singleton(added))
		{
		}

		public ModelAddAction(CanvasModel model, ICollection<CanvasObject> added) : base(model)
		{
			this.added = new List<CanvasObject>(added);
			this.addIndex = model.ObjectsFromBottom.Count;
		}

		public ModelAddAction(CanvasModel model, ICollection<CanvasObject> added, int index) : base(model)
		{
			this.added = new List<CanvasObject>(added);
			this.addIndex = index;
		}

		public virtual int DestinationIndex
		{
			get
			{
				return addIndex;
			}
		}

		public override ICollection<CanvasObject> Objects
		{
			get
			{
				return added.AsReadOnly();
			}
		}

		public override string Name
		{
			get
			{
				return Strings.get("actionAdd", getShapesName(added));
			}
		}

		internal override void doSub(CanvasModel model)
		{
			model.addObjects(addIndex, added);
		}

		internal override void undoSub(CanvasModel model)
		{
			model.removeObjects(added);
		}
	}

}
