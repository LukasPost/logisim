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
	using HandleGesture = draw.model.HandleGesture;

	public class ModelMoveHandleAction : ModelAction
	{
		private HandleGesture gesture;
		private Handle newHandle;

		public ModelMoveHandleAction(CanvasModel model, HandleGesture gesture) : base(model)
		{
			this.gesture = gesture;
		}

		public virtual Handle NewHandle
		{
			get
			{
				return newHandle;
			}
		}

		public override ICollection<CanvasObject> Objects
		{
			get
			{
				return Collections.singleton(gesture.Handle.Object);
			}
		}

		public override string Name
		{
			get
			{
				return Strings.get("actionMoveHandle");
			}
		}

		internal override void doSub(CanvasModel model)
		{
			newHandle = model.moveHandle(gesture);
		}

		internal override void undoSub(CanvasModel model)
		{
			Handle oldHandle = gesture.Handle;
			int dx = oldHandle.X - newHandle.X;
			int dy = oldHandle.Y - newHandle.Y;
			HandleGesture reverse = new HandleGesture(newHandle, dx, dy, 0);
			model.moveHandle(reverse);
		}
	}

}
