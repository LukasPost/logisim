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

	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using ZOrder = draw.util.ZOrder;
	using AppearanceAnchor = logisim.circuit.appear.AppearanceAnchor;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;

	public class ClipboardActions : Action
	{

		public static Action cut(AppearanceCanvas canvas)
		{
			return new ClipboardActions(true, canvas);
		}

		public static Action copy(AppearanceCanvas canvas)
		{
			return new ClipboardActions(false, canvas);
		}

		private bool remove;
		private AppearanceCanvas canvas;
		private CanvasModel canvasModel;
		private ClipboardContents oldClipboard;
		private IDictionary<CanvasObject, int> affected;
		private ClipboardContents newClipboard;

		private ClipboardActions(bool remove, AppearanceCanvas canvas)
		{
			this.remove = remove;
			this.canvas = canvas;
			this.canvasModel = canvas.Model;

			List<CanvasObject> contents = new List<CanvasObject>();
			Direction anchorFacing = null;
			Location anchorLocation = null;
			List<CanvasObject> aff = new List<CanvasObject>();
			foreach (CanvasObject o in canvas.Selection.Selected)
			{
				if (o.canRemove())
				{
					aff.Add(o);
					contents.Add(o.clone());
				}
				else if (o is AppearanceAnchor)
				{
					AppearanceAnchor anch = (AppearanceAnchor) o;
					anchorFacing = anch.Facing;
					anchorLocation = anch.Location;
				}
			}
			contents.TrimExcess();
			affected = ZOrder.getZIndex(aff, canvasModel);
			newClipboard = new ClipboardContents(contents, anchorLocation, anchorFacing);
		}

		public override string Name
		{
			get
			{
				if (remove)
				{
					return Strings.get("cutSelectionAction");
				}
				else
				{
					return Strings.get("copySelectionAction");
				}
			}
		}

		public override void doIt(Project proj)
		{
			oldClipboard = Clipboard.get();
			Clipboard.set(newClipboard);
			if (remove)
			{
				canvasModel.removeObjects(affected.Keys);
			}
		}

		public override void undo(Project proj)
		{
			if (remove)
			{
				canvasModel.addObjects(affected);
				canvas.Selection.clearSelected();
				canvas.Selection.setSelected(affected.Keys, true);
			}
			Clipboard.set(oldClipboard);
		}

	}

}
