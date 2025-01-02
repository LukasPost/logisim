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

	using Selection = draw.canvas.Selection;
	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using ZOrder = draw.util.ZOrder;
	using AppearanceAnchor = logisim.circuit.appear.AppearanceAnchor;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using StringGetter = logisim.util.StringGetter;

	internal class SelectionAction : Action
	{
		private StringGetter displayName;
		private AppearanceCanvas canvas;
		private CanvasModel canvasModel;
		private IDictionary<CanvasObject, int> toRemove;
		private ICollection<CanvasObject> toAdd;
		private ICollection<CanvasObject> oldSelection;
		private ICollection<CanvasObject> newSelection;
		private Location anchorNewLocation;
		private Direction anchorNewFacing;
		private Location anchorOldLocation;
		private Direction anchorOldFacing;

		public SelectionAction(AppearanceCanvas canvas, StringGetter displayName, ICollection<CanvasObject> toRemove, ICollection<CanvasObject> toAdd, ICollection<CanvasObject> newSelection, Location anchorLocation, Direction anchorFacing)
		{
			this.canvas = canvas;
			this.canvasModel = canvas.Model;
			this.displayName = displayName;
			this.toRemove = toRemove == null ? null : ZOrder.getZIndex(toRemove, canvasModel);
			this.toAdd = toAdd;
			this.oldSelection = new List<CanvasObject>(canvas.Selection.Selected);
			this.newSelection = newSelection;
			this.anchorNewLocation = anchorLocation;
			this.anchorNewFacing = anchorFacing;
		}

		public override string Name
		{
			get
			{
				return displayName.get();
			}
		}

		public override void doIt(Project proj)
		{
			Selection sel = canvas.Selection;
			sel.clearSelected();
			if (toRemove != null)
			{
				canvasModel.removeObjects(toRemove.Keys);
			}
			int dest = AppearanceCanvas.getMaxIndex(canvasModel) + 1;
			if (toAdd != null)
			{
				canvasModel.addObjects(dest, toAdd);
			}

			AppearanceAnchor anchor = findAnchor(canvasModel);
			if (anchor != null && anchorNewLocation != null)
			{
				anchorOldLocation = anchor.Location;
				anchor.translate(anchorNewLocation.X - anchorOldLocation.X, anchorNewLocation.Y - anchorOldLocation.Y);
			}
			if (anchor != null && anchorNewFacing != null)
			{
				anchorOldFacing = anchor.Facing;
				anchor.setValue(AppearanceAnchor.FACING, anchorNewFacing);
			}
			sel.setSelected(newSelection, true);
			canvas.repaint();
		}

		private AppearanceAnchor findAnchor(CanvasModel canvasModel)
		{
			foreach (object o in canvasModel.ObjectsFromTop)
			{
				if (o is AppearanceAnchor)
				{
					return (AppearanceAnchor) o;
				}
			}
			return null;
		}

		public override void undo(Project proj)
		{
			AppearanceAnchor anchor = findAnchor(canvasModel);
			if (anchor != null && anchorOldLocation != null)
			{
				anchor.translate(anchorOldLocation.X - anchorNewLocation.X, anchorOldLocation.Y - anchorNewLocation.Y);
			}
			if (anchor != null && anchorOldFacing != null)
			{
				anchor.setValue(AppearanceAnchor.FACING, anchorOldFacing);
			}
			Selection sel = canvas.Selection;
			sel.clearSelected();
			if (toAdd != null)
			{
				canvasModel.removeObjects(toAdd);
			}
			if (toRemove != null)
			{
				canvasModel.addObjects(toRemove);
			}
			sel.setSelected(oldSelection, true);
			canvas.repaint();
		}
	}

}
