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

	public class ModelTranslateAction : ModelAction
	{
		private HashSet<CanvasObject> moved;
		private int dx;
		private int dy;

		public ModelTranslateAction(CanvasModel model, ICollection<CanvasObject> moved, int dx, int dy) : base(model)
		{
			this.moved = new HashSet<CanvasObject>(moved);
			this.dx = dx;
			this.dy = dy;
		}

		public override ICollection<CanvasObject> Objects
		{
			get
			{
				return Collections.unmodifiableSet(moved);
			}
		}

		public override string Name
		{
			get
			{
				return Strings.get("actionTranslate", getShapesName(moved));
			}
		}

		internal override void doSub(CanvasModel model)
		{
			model.translateObjects(moved, dx, dy);
		}

		internal override void undoSub(CanvasModel model)
		{
			model.translateObjects(moved, -dx, -dy);
		}

		public override bool shouldAppendTo(Action other)
		{
			if (other is ModelTranslateAction)
			{
				ModelTranslateAction o = (ModelTranslateAction) other;
				return this.moved.Equals(o.moved);
			}
			else
			{
				return false;
			}
		}

		public override Action append(Action other)
		{
			if (other is ModelTranslateAction)
			{
				ModelTranslateAction o = (ModelTranslateAction) other;
				if (this.moved.Equals(o.moved))
				{
					return new ModelTranslateAction(Model, moved, this.dx + o.dx, this.dy + o.dy);
				}
			}
			return base.append(other);
		}
	}

}
