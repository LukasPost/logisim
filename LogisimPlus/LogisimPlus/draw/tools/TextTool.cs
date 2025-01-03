// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.tools
{


	using ModelAddAction = draw.actions.ModelAddAction;
	using ModelEditTextAction = draw.actions.ModelEditTextAction;
	using ModelRemoveAction = draw.actions.ModelRemoveAction;
	using Canvas = draw.canvas.Canvas;
	using CanvasObject = draw.model.CanvasObject;
	using DrawAttr = draw.shapes.DrawAttr;
	using Text = draw.shapes.Text;
	using EditableLabelField = draw.util.EditableLabelField;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using Location = logisim.data.Location;
	using Icons = logisim.util.Icons;
    using LogisimPlus.Java;

    public class TextTool : AbstractTool
	{
		private class FieldListener : AbstractAction, AttributeListener
		{
			private readonly TextTool outerInstance;

			public FieldListener(TextTool outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				outerInstance.commitText(outerInstance.curCanvas);
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
				Text cur = outerInstance.curText;
				if (cur != null)
				{
					double zoom = outerInstance.curCanvas.ZoomFactor;
					cur.Label.configureTextField(outerInstance.field, zoom);
					outerInstance.curCanvas.repaint();
				}
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				attributeListChanged(e);
			}
		}

		private class CancelListener : AbstractAction
		{
			private readonly TextTool outerInstance;

			public CancelListener(TextTool outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				outerInstance.cancelText(outerInstance.curCanvas);
			}
		}

		private DrawingAttributeSet attrs;
		private EditableLabelField field;
		private FieldListener fieldListener;

		private Text curText;
		private Canvas curCanvas;
		private bool isTextNew;

		public TextTool(DrawingAttributeSet attrs)
		{
			this.attrs = attrs;
			curText = null;
			isTextNew = false;
			field = new EditableLabelField();

			fieldListener = new FieldListener(this);
			InputMap fieldInput = field.getInputMap();
			fieldInput.put(KeyStroke.getKeyStroke(KeyEvent.VK_ENTER, 0), "commit");
			fieldInput.put(KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0), "cancel");
			ActionMap fieldAction = field.getActionMap();
			fieldAction.put("commit", fieldListener);
			fieldAction.put("cancel", new CancelListener(this));
		}

		public override Icon Icon
		{
			get
			{
				return Icons.getIcon("text.gif");
			}
		}

		public override Cursor getCursor(Canvas canvas)
		{
			return Cursor.getPredefinedCursor(Cursor.TEXT_CURSOR);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return DrawAttr.ATTRS_TEXT_TOOL;
			}
		}

		public override void toolSelected(Canvas canvas)
		{
			cancelText(canvas);
		}

		public override void toolDeselected(Canvas canvas)
		{
			commitText(canvas);
		}

		public override void mousePressed(Canvas canvas, MouseEvent e)
		{
			if (curText != null)
			{
				commitText(canvas);
			}

			Text clicked = null;
			bool found = false;
			int mx = e.getX();
			int my = e.getY();
			Location mloc = new Location(mx, my);
			foreach (CanvasObject o in canvas.Model.ObjectsFromTop)
			{
				if (o is Text text && o.contains(mloc, true))
				{
					clicked = text;
					break;
				}
			}
			if (clicked == null)
			{
				clicked = attrs.applyTo(new Text(mx, my, ""));
			}

			curText = clicked;
			curCanvas = canvas;
			isTextNew = !found;
			clicked.Label.configureTextField(field, canvas.ZoomFactor);
			field.setText(clicked.Text);
			canvas.add(field);

			Point fieldLoc = field.getLocation();
			double zoom = canvas.ZoomFactor;
			fieldLoc.x = (int) (long)Math.Round(mx * zoom - fieldLoc.x, MidpointRounding.AwayFromZero);
			fieldLoc.y = (int) (long)Math.Round(my * zoom - fieldLoc.y, MidpointRounding.AwayFromZero);
			int caret = field.viewToModel2D(fieldLoc);
			if (caret >= 0)
			{
				field.setCaretPosition(caret);
			}
			field.requestFocus();

			canvas.Selection.setSelected(clicked, true);
			canvas.Selection.setHidden(Collections.singleton(clicked), true);
			clicked.addAttributeListener(fieldListener);
			canvas.repaint();
		}

		public override void zoomFactorChanged(Canvas canvas)
		{
			Text t = curText;
			if (t != null)
			{
				t.Label.configureTextField(field, canvas.ZoomFactor);
			}
		}

		public override void draw(Canvas canvas, JGraphics g)
		{
			; // actually, there's nothing to do here - it's handled by the field
		}

		private void cancelText(Canvas canvas)
		{
			Text cur = curText;
			if (cur != null)
			{
				curText = null;
				cur.removeAttributeListener(fieldListener);
				canvas.remove(field);
				canvas.Selection.clearSelected();
				canvas.repaint();
			}
		}

		private void commitText(Canvas canvas)
		{
			Text cur = curText;
			bool isNew = isTextNew;
			string newText = field.getText();
			if (cur == null)
			{
				return;
			}
			cancelText(canvas);

			if (isNew)
			{
				if (!newText.Equals(""))
				{
					cur.Text = newText;
					canvas.doAction(new ModelAddAction(canvas.Model, cur));
				}
			}
			else
			{
				string oldText = cur.Text;
				if (newText.Equals(""))
				{
					canvas.doAction(new ModelRemoveAction(canvas.Model, cur));
				}
				else if (!oldText.Equals(newText))
				{
					canvas.doAction(new ModelEditTextAction(canvas.Model, cur, newText));
				}
			}
		}
	}

}
