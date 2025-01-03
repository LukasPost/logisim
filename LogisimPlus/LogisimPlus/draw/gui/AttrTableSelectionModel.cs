// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.gui
{

	using ModelChangeAttributeAction = draw.actions.ModelChangeAttributeAction;
	using Canvas = draw.canvas.Canvas;
	using Selection = draw.canvas.Selection;
	using SelectionEvent = draw.canvas.SelectionEvent;
	using SelectionListener = draw.canvas.SelectionListener;
	using AttributeMapKey = draw.model.AttributeMapKey;
	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using AttrTableSetException = logisim.gui.generic.AttrTableSetException;
	using AttributeSetTableModel = logisim.gui.generic.AttributeSetTableModel;

	internal class AttrTableSelectionModel : AttributeSetTableModel, SelectionListener
	{
		private Canvas canvas;

		public AttrTableSelectionModel(Canvas canvas) : base(new SelectionAttributes(canvas.Selection))
		{
			this.canvas = canvas;
			canvas.Selection.addSelectionListener(this);
		}

		public override string Title
		{
			get
			{
				Selection sel = canvas.Selection;
				Type commonClass = null;
				int commonCount = 0;
				CanvasObject firstObject = null;
				int totalCount = 0;
				foreach (CanvasObject obj in sel.Selected)
				{
					if (firstObject == null)
					{
						firstObject = obj;
						commonClass = obj.GetType();
						commonCount = 1;
					}
					else if (obj.GetType() == commonClass)
					{
						commonCount++;
					}
					else
					{
						commonClass = null;
					}
					totalCount++;
				}
    
				if (firstObject == null)
				{
					return null;
				}
				else if (commonClass == null)
				{
					return Strings.get("selectionVarious", "" + totalCount);
				}
				else if (commonCount == 1)
				{
					return Strings.get("selectionOne", firstObject.DisplayName);
				}
				else
				{
					return Strings.get("selectionMultiple", firstObject.DisplayName, "" + commonCount);
				}
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void setValueRequested(logisim.data.Attribute attr, Object value) throws logisim.gui.generic.AttrTableSetException
		protected internal override void setValueRequested(Attribute attr, object value)
		{
			SelectionAttributes attrs = (SelectionAttributes) AttributeSet;
			Dictionary<AttributeMapKey, object> oldVals;
			oldVals = new Dictionary<AttributeMapKey, object>();
			Dictionary<AttributeMapKey, object> newVals;
			newVals = new Dictionary<AttributeMapKey, object>();
			foreach (KeyValuePair<AttributeSet, CanvasObject> ent in attrs.entries())
			{
				AttributeMapKey key = new AttributeMapKey(attr, ent.Value);
				oldVals[key] = ent.Key.getValue(attr);
				newVals[key] = value;
			}
			CanvasModel model = canvas.Model;
			canvas.doAction(new ModelChangeAttributeAction(model, oldVals, newVals));
		}

		//
		// SelectionListener method
		//
		public virtual void selectionChanged(SelectionEvent e)
		{
			fireTitleChanged();
		}
	}

}
