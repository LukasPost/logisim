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
	using Handle = draw.model.Handle;

	public class ModelInsertHandleAction : ModelAction
	{
		private Handle desired;

		public ModelInsertHandleAction(CanvasModel model, Handle desired) : base(model)
		{
			this.desired = desired;
		}

		public override ICollection<CanvasObject> Objects
		{
			get
			{
				return Collections.singleton(desired.Object);
			}
		}

		public override string Name
		{
			get
			{
				return Strings.get("actionInsertHandle");
			}
		}

		internal override void doSub(CanvasModel model)
		{
			model.insertHandle(desired, null);
		}

		internal override void undoSub(CanvasModel model)
		{
			model.deleteHandle(desired);
		}
	}

}
