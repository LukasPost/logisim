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
	using Text = draw.shapes.Text;

	public class ModelEditTextAction : ModelAction
	{
		private Text text;
		private string oldValue;
		private string newValue;

		public ModelEditTextAction(CanvasModel model, Text text, string newValue) : base(model)
		{
			this.text = text;
			this.oldValue = text.Text;
			this.newValue = newValue;
		}

		public override ICollection<CanvasObject> Objects
		{
			get
			{
				return Collections.singleton((CanvasObject) text);
			}
		}

		public override string Name
		{
			get
			{
				return Strings.get("actionEditText");
			}
		}

		internal override void doSub(CanvasModel model)
		{
			model.setText(text, newValue);
		}

		internal override void undoSub(CanvasModel model)
		{
			model.setText(text, oldValue);
		}
	}

}
