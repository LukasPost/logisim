// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitEvent = logisim.circuit.CircuitEvent;
	using CircuitListener = logisim.circuit.CircuitListener;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentUserEvent = logisim.comp.ComponentUserEvent;
	using AttributeSet = logisim.data.AttributeSet;
	using Location = logisim.data.Location;
	using Canvas = logisim.gui.main.Canvas;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using Text = logisim.std.@base.Text;

	public class TextTool : Tool
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			listener = new MyListener(this);
		}

		private class MyListener : CaretListener, CircuitListener
		{
			private readonly TextTool outerInstance;

			public MyListener(TextTool outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void editingCanceled(CaretEvent e)
			{
				if (e.Caret != outerInstance.caret)
				{
					e.Caret.removeCaretListener(this);
					return;
				}
				outerInstance.caret.removeCaretListener(this);
				outerInstance.caretCircuit.removeCircuitListener(this);

				outerInstance.caretCircuit = null;
				outerInstance.caretComponent = null;
				outerInstance.caretCreatingText = false;
				outerInstance.caret = null;
			}

			public virtual void editingStopped(CaretEvent e)
			{
				if (e.Caret != outerInstance.caret)
				{
					e.Caret.removeCaretListener(this);
					return;
				}
				outerInstance.caret.removeCaretListener(this);
				outerInstance.caretCircuit.removeCircuitListener(this);

				string val = outerInstance.caret.Text;
				bool isEmpty = (string.ReferenceEquals(val, null) || val.Equals(""));
				Action a;
				Project proj = outerInstance.caretCanvas.Project;
				if (outerInstance.caretCreatingText)
				{
					if (!isEmpty)
					{
						CircuitMutation xn = new CircuitMutation(outerInstance.caretCircuit);
						xn.add(outerInstance.caretComponent);
						a = xn.toAction(Strings.getter("addComponentAction", Text.FACTORY.DisplayGetter));
					}
					else
					{
						a = null; // don't add the blank text field
					}
				}
				else
				{
					if (isEmpty && outerInstance.caretComponent.Factory is Text)
					{
						CircuitMutation xn = new CircuitMutation(outerInstance.caretCircuit);
						xn.add(outerInstance.caretComponent);
						a = xn.toAction(Strings.getter("removeComponentAction", Text.FACTORY.DisplayGetter));
					}
					else
					{
						object obj = outerInstance.caretComponent.getFeature(typeof(TextEditable));
						if (obj == null)
						{ // should never happen
							a = null;
						}
						else
						{
							TextEditable editable = (TextEditable) obj;
							a = editable.getCommitAction(outerInstance.caretCircuit, e.OldText, e.Text);
						}
					}
				}

				outerInstance.caretCircuit = null;
				outerInstance.caretComponent = null;
				outerInstance.caretCreatingText = false;
				outerInstance.caret = null;

				if (a != null)
				{
					proj.doAction(a);
				}
			}

			public virtual void circuitChanged(CircuitEvent @event)
			{
				if (@event.Circuit != outerInstance.caretCircuit)
				{
					@event.Circuit.removeCircuitListener(this);
					return;
				}
				int action = @event.Action;
				if (action == CircuitEvent.ACTION_REMOVE)
				{
					if (@event.Data == outerInstance.caretComponent)
					{
						outerInstance.caret.cancelEditing();
					}
				}
				else if (action == CircuitEvent.ACTION_CLEAR)
				{
					if (outerInstance.caretComponent != null)
					{
						outerInstance.caret.cancelEditing();
					}
				}
			}
		}

		private static Cursor cursor = Cursor.getPredefinedCursor(Cursor.TEXT_CURSOR);

		private MyListener listener;
		private AttributeSet attrs;
		private Caret caret = null;
		private bool caretCreatingText = false;
		private Canvas caretCanvas = null;
		private Circuit caretCircuit = null;
		private Component caretComponent = null;

		public TextTool()
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			attrs = Text.FACTORY.createAttributeSet();
		}

		public override bool Equals(object other)
		{
			return other is TextTool;
		}

		public override int GetHashCode()
		{
			return typeof(TextTool).GetHashCode();
		}

		public override string Name
		{
			get
			{
				return "Text Tool";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("textTool");
			}
		}

		public override string Description
		{
			get
			{
				return Strings.get("textToolDesc");
			}
		}

		public override AttributeSet AttributeSet
		{
			get
			{
				return attrs;
			}
		}

		public override void paintIcon(ComponentDrawContext c, int x, int y)
		{
			Text.FACTORY.paintIcon(c, x, y, null);
		}

		public override void draw(Canvas canvas, ComponentDrawContext context)
		{
			if (caret != null)
			{
				caret.draw(context.Graphics);
			}
		}

		public override void deselect(Canvas canvas)
		{
			if (caret != null)
			{
				caret.stopEditing();
				caret = null;
			}
		}

		public override void mousePressed(Canvas canvas, Graphics g, MouseEvent e)
		{
			Project proj = canvas.Project;
			Circuit circ = canvas.Circuit;

			if (!proj.LogisimFile.contains(circ))
			{
				if (caret != null)
				{
					caret.cancelEditing();
				}
				canvas.ErrorMessage = Strings.getter("cannotModifyError");
				return;
			}

			// Maybe user is clicking within the current caret.
			if (caret != null)
			{
				if (caret.getBounds(g).contains(e.getX(), e.getY()))
				{ // Yes
					caret.mousePressed(e);
					proj.repaintCanvas();
					return;
				}
				else
				{ // No. End the current caret.
					caret.stopEditing();
				}
			}
			// caret will be null at this point

			// Otherwise search for a new caret.
			int x = e.getX();
			int y = e.getY();
			Location loc = new Location(x, y);
			ComponentUserEvent @event = new ComponentUserEvent(canvas, x, y);

			// First search in selection.
			foreach (Component comp in proj.Selection.getComponentsContaining(loc, g))
			{
				TextEditable editable = (TextEditable) comp.getFeature(typeof(TextEditable));
				if (editable != null)
				{
					caret = editable.getTextCaret(@event);
					if (caret != null)
					{
						proj.Frame.viewComponentAttributes(circ, comp);
						caretComponent = comp;
						caretCreatingText = false;
						break;
					}
				}
			}

			// Then search in circuit
			if (caret == null)
			{
				foreach (Component comp in circ.getAllContaining(loc, g))
				{
					TextEditable editable = (TextEditable) comp.getFeature(typeof(TextEditable));
					if (editable != null)
					{
						caret = editable.getTextCaret(@event);
						if (caret != null)
						{
							proj.Frame.viewComponentAttributes(circ, comp);
							caretComponent = comp;
							caretCreatingText = false;
							break;
						}
					}
				}
			}

			// if nothing found, create a new label
			if (caret == null)
			{
				if (loc.X < 0 || loc.Y < 0)
				{
					return;
				}
				AttributeSet copy = (AttributeSet) attrs.clone();
				caretComponent = Text.FACTORY.createComponent(loc, copy);
				caretCreatingText = true;
				TextEditable editable = (TextEditable) caretComponent.getFeature(typeof(TextEditable));
				if (editable != null)
				{
					caret = editable.getTextCaret(@event);
					proj.Frame.viewComponentAttributes(circ, caretComponent);
				}
			}

			if (caret != null)
			{
				caretCanvas = canvas;
				caretCircuit = canvas.Circuit;
				caret.addCaretListener(listener);
				caretCircuit.addCircuitListener(listener);
			}
			proj.repaintCanvas();
		}

		public override void mouseDragged(Canvas canvas, Graphics g, MouseEvent e)
		{
			// TODO: enhance label editing
		}

		public override void mouseReleased(Canvas canvas, Graphics g, MouseEvent e)
		{
			// TODO: enhance label editing
		}

		public override void keyPressed(Canvas canvas, KeyEvent e)
		{
			if (caret != null)
			{
				caret.keyPressed(e);
				canvas.Project.repaintCanvas();
			}
		}

		public override void keyReleased(Canvas canvas, KeyEvent e)
		{
			if (caret != null)
			{
				caret.keyReleased(e);
				canvas.Project.repaintCanvas();
			}
		}

		public override void keyTyped(Canvas canvas, KeyEvent e)
		{
			if (caret != null)
			{
				caret.keyTyped(e);
				canvas.Project.repaintCanvas();
			}
		}

		public override Cursor Cursor
		{
			get
			{
				return cursor;
			}
		}
	}

}
