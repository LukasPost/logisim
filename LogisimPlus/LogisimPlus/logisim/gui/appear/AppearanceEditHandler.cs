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

	using ModelDeleteHandleAction = draw.actions.ModelDeleteHandleAction;
	using ModelInsertHandleAction = draw.actions.ModelInsertHandleAction;
	using ModelReorderAction = draw.actions.ModelReorderAction;
	using Canvas = draw.canvas.Canvas;
	using Selection = draw.canvas.Selection;
	using SelectionEvent = draw.canvas.SelectionEvent;
	using SelectionListener = draw.canvas.SelectionListener;
	using CanvasModel = draw.model.CanvasModel;
	using CanvasModelEvent = draw.model.CanvasModelEvent;
	using CanvasModelListener = draw.model.CanvasModelListener;
	using CanvasObject = draw.model.CanvasObject;
	using Handle = draw.model.Handle;
	using draw.util;
	using ZOrder = draw.util.ZOrder;
	using Circuit = logisim.circuit.Circuit;
	using AppearanceAnchor = logisim.circuit.appear.AppearanceAnchor;
	using AppearanceElement = logisim.circuit.appear.AppearanceElement;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using EditHandler = logisim.gui.main.EditHandler;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using Project = logisim.proj.Project;

	public class AppearanceEditHandler : EditHandler, SelectionListener, PropertyChangeListener, CanvasModelListener
	{
		private AppearanceCanvas canvas;

		internal AppearanceEditHandler(AppearanceCanvas canvas)
		{
			this.canvas = canvas;
			canvas.Selection.addSelectionListener(this);
			CanvasModel model = canvas.Model;
			if (model != null)
			{
				model.addCanvasModelListener(this);
			}
			canvas.addPropertyChangeListener(Canvas.MODEL_PROPERTY, this);
		}

		public override void computeEnabled()
		{
			Project proj = canvas.Project;
			Circuit circ = canvas.Circuit;
			Selection sel = canvas.Selection;
			bool selEmpty = sel.Empty;
			bool canChange = proj.LogisimFile.contains(circ);
			bool clipExists = !Clipboard.Empty;
			bool selHasRemovable = false;
			foreach (CanvasObject o in sel.Selected)
			{
				if (!(o is AppearanceElement))
				{
					selHasRemovable = true;
				}
			}
			bool canRaise;
			bool canLower;
			if (!selEmpty && canChange)
			{
				IDictionary<CanvasObject, int> zs = ZOrder.getZIndex(sel.Selected, canvas.Model);
				int zmin = int.MaxValue;
				int zmax = int.MinValue;
				int count = 0;
				foreach (KeyValuePair<CanvasObject, int> entry in zs.SetOfKeyValuePairs())
				{
					if (!(entry.Key is AppearanceElement))
					{
						count++;
						int z = entry.Value.intValue();
						if (z < zmin)
						{
							zmin = z;
						}
						if (z > zmax)
						{
							zmax = z;
						}
					}
				}
				int maxPoss = AppearanceCanvas.getMaxIndex(canvas.Model);
				if (count > 0 && count <= maxPoss)
				{
					canRaise = zmin <= maxPoss - count;
					canLower = zmax >= count;
				}
				else
				{
					canRaise = false;
					canLower = false;
				}
			}
			else
			{
				canRaise = false;
				canLower = false;
			}
			bool canAddCtrl = false;
			bool canRemCtrl = false;
			Handle handle = sel.SelectedHandle;
			if (handle != null && canChange)
			{
				CanvasObject o = handle.Object;
				canAddCtrl = o.canInsertHandle(handle.Location) != null;
				canRemCtrl = o.canDeleteHandle(handle.Location) != null;
			}

			setEnabled(LogisimMenuBar.CUT, selHasRemovable && canChange);
			setEnabled(LogisimMenuBar.COPY, !selEmpty);
			setEnabled(LogisimMenuBar.PASTE, canChange && clipExists);
			setEnabled(LogisimMenuBar.DELETE, selHasRemovable && canChange);
			setEnabled(LogisimMenuBar.DUPLICATE, !selEmpty && canChange);
			setEnabled(LogisimMenuBar.SELECT_ALL, true);
			setEnabled(LogisimMenuBar.RAISE, canRaise);
			setEnabled(LogisimMenuBar.LOWER, canLower);
			setEnabled(LogisimMenuBar.RAISE_TOP, canRaise);
			setEnabled(LogisimMenuBar.LOWER_BOTTOM, canLower);
			setEnabled(LogisimMenuBar.ADD_CONTROL, canAddCtrl);
			setEnabled(LogisimMenuBar.REMOVE_CONTROL, canRemCtrl);
		}

		public override void cut()
		{
			if (!canvas.Selection.Empty)
			{
				canvas.Project.doAction(ClipboardActions.cut(canvas));
			}
		}

		public override void copy()
		{
			if (!canvas.Selection.Empty)
			{
				canvas.Project.doAction(ClipboardActions.copy(canvas));
			}
		}

		public override void paste()
		{
			ClipboardContents clip = Clipboard.get();
			ICollection<CanvasObject> contents = clip.Elements;
			IList<CanvasObject> add = new List<CanvasObject>(contents.Count);
			foreach (CanvasObject o in contents)
			{
				add.Add(o.clone());
			}
			if (add.Count == 0)
			{
				return;
			}

			// find how far we have to translate shapes so that at least one of the
			// pasted shapes doesn't match what's already in the model
			ICollection<CanvasObject> raw = canvas.Model.ObjectsFromBottom;
			MatchingSet<CanvasObject> cur = new MatchingSet<CanvasObject>(raw);
			int dx = 0;
			while (true)
			{
				// if any shapes in "add" aren't in canvas, we are done
				bool allMatch = true;
				foreach (CanvasObject o in add)
				{
					if (!cur.contains(o))
					{
						allMatch = false;
						break;
					}
				}
				if (!allMatch)
				{
					break;
				}

				// otherwise translate everything by 10 pixels and repeat test
				foreach (CanvasObject o in add)
				{
					o.translate(10, 10);
				}
				dx += 10;
			}

			Location anchorLocation = clip.AnchorLocation;
			if (anchorLocation != null && dx != 0)
			{
				anchorLocation = anchorLocation.translate(dx, dx);
			}

			canvas.Project.doAction(new SelectionAction(canvas, Strings.getter("pasteClipboardAction"), null, add, add, anchorLocation, clip.AnchorFacing));
		}

		public override void delete()
		{
			Selection sel = canvas.Selection;
			int n = sel.Selected.Count;
			IList<CanvasObject> select = new List<CanvasObject>(n);
			IList<CanvasObject> remove = new List<CanvasObject>(n);
			Location anchorLocation = null;
			Direction anchorFacing = null;
			foreach (CanvasObject o in sel.Selected)
			{
				if (o.canRemove())
				{
					remove.Add(o);
				}
				else
				{
					select.Add(o);
					if (o is AppearanceAnchor)
					{
						AppearanceAnchor anchor = (AppearanceAnchor) o;
						anchorLocation = anchor.Location;
						anchorFacing = anchor.Facing;
					}
				}
			}

			if (remove.Count > 0)
			{
				canvas.Project.doAction(new SelectionAction(canvas, Strings.getter("deleteSelectionAction"), remove, null, select, anchorLocation, anchorFacing));
			}
		}

		public override void duplicate()
		{
			Selection sel = canvas.Selection;
			int n = sel.Selected.Count;
			IList<CanvasObject> select = new List<CanvasObject>(n);
			IList<CanvasObject> clones = new List<CanvasObject>(n);
			foreach (CanvasObject o in sel.Selected)
			{
				if (o.canRemove())
				{
					CanvasObject copy = o.clone();
					copy.translate(10, 10);
					clones.Add(copy);
					select.Add(copy);
				}
				else
				{
					select.Add(o);
				}
			}

			if (clones.Count > 0)
			{
				canvas.Project.doAction(new SelectionAction(canvas, Strings.getter("duplicateSelectionAction"), null, clones, select, null, null));
			}
		}

		public override void selectAll()
		{
			Selection sel = canvas.Selection;
			sel.setSelected(canvas.Model.ObjectsFromBottom, true);
			canvas.repaint();
		}

		public override void raise()
		{
			ModelReorderAction act = ModelReorderAction.createRaise(canvas.Model, canvas.Selection.Selected);
			if (act != null)
			{
				canvas.doAction(act);
			}
		}

		public override void lower()
		{
			ModelReorderAction act = ModelReorderAction.createLower(canvas.Model, canvas.Selection.Selected);
			if (act != null)
			{
				canvas.doAction(act);
			}
		}

		public override void raiseTop()
		{
			ModelReorderAction act = ModelReorderAction.createRaiseTop(canvas.Model, canvas.Selection.Selected);
			if (act != null)
			{
				canvas.doAction(act);
			}
		}

		public override void lowerBottom()
		{
			ModelReorderAction act = ModelReorderAction.createLowerBottom(canvas.Model, canvas.Selection.Selected);
			if (act != null)
			{
				canvas.doAction(act);
			}
		}

		public override void addControlPoint()
		{
			Selection sel = canvas.Selection;
			Handle handle = sel.SelectedHandle;
			canvas.doAction(new ModelInsertHandleAction(canvas.Model, handle));
		}

		public override void removeControlPoint()
		{
			Selection sel = canvas.Selection;
			Handle handle = sel.SelectedHandle;
			canvas.doAction(new ModelDeleteHandleAction(canvas.Model, handle));
		}

		public virtual void selectionChanged(SelectionEvent e)
		{
			computeEnabled();
		}

		public virtual void propertyChange(PropertyChangeEvent e)
		{
			string prop = e.getPropertyName();
			if (prop.Equals(Canvas.MODEL_PROPERTY))
			{
				CanvasModel oldModel = (CanvasModel) e.getOldValue();
				if (oldModel != null)
				{
					oldModel.removeCanvasModelListener(this);
				}
				CanvasModel newModel = (CanvasModel) e.getNewValue();
				if (newModel != null)
				{
					newModel.addCanvasModelListener(this);
				}
			}
		}

		public virtual void modelChanged(CanvasModelEvent @event)
		{
			computeEnabled();
		}
	}

}
