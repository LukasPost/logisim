// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.instance
{

	using Circuit = logisim.circuit.Circuit;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentUserEvent = logisim.comp.ComponentUserEvent;
	using TextField = logisim.comp.TextField;
	using TextFieldEvent = logisim.comp.TextFieldEvent;
	using TextFieldListener = logisim.comp.TextFieldListener;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Canvas = logisim.gui.main.Canvas;
	using Action = logisim.proj.Action;
	using Caret = logisim.tools.Caret;
	using SetAttributeAction = logisim.tools.SetAttributeAction;
	using TextEditable = logisim.tools.TextEditable;

	public class InstanceTextField : AttributeListener, TextFieldListener, TextEditable
	{
		private Canvas canvas;
		private InstanceComponent comp;
		private TextField field;
		private Attribute<string> labelAttr;
		private Attribute<Font> fontAttr;
		private int fieldX;
		private int fieldY;
		private int halign;
		private int valign;

		internal InstanceTextField(InstanceComponent comp)
		{
			this.comp = comp;
			this.field = null;
			this.labelAttr = null;
			this.fontAttr = null;
		}

		internal virtual void update(Attribute<string> labelAttr, Attribute<Font> fontAttr, int x, int y, int halign, int valign)
		{
			bool wasReg = shouldRegister();
			this.labelAttr = labelAttr;
			this.fontAttr = fontAttr;
			this.fieldX = x;
			this.fieldY = y;
			this.halign = halign;
			this.valign = valign;
			bool shouldReg = shouldRegister();
			AttributeSet attrs = comp.AttributeSet;
			if (!wasReg && shouldReg)
			{
				attrs.addAttributeListener(this);
			}
			if (wasReg && !shouldReg)
			{
				attrs.removeAttributeListener(this);
			}

			updateField(attrs);
		}

		private void updateField(AttributeSet attrs)
		{
			string text = attrs.getValue(labelAttr);
			if (string.ReferenceEquals(text, null) || text.Equals(""))
			{
				if (field != null)
				{
					field.removeTextFieldListener(this);
					field = null;
				}
			}
			else
			{
				if (field == null)
				{
					createField(attrs, text);
				}
				else
				{
					Font font = attrs.getValue(fontAttr);
					if (font != null)
					{
						field.Font = font;
					}
					field.setLocation(fieldX, fieldY, halign, valign);
					field.Text = text;
				}
			}
		}

		private void createField(AttributeSet attrs, string text)
		{
			Font font = attrs.getValue(fontAttr);
			field = new TextField(fieldX, fieldY, halign, valign, font);
			field.Text = text;
			field.addTextFieldListener(this);
		}

		private bool shouldRegister()
		{
			return labelAttr != null || fontAttr != null;
		}

		internal virtual Bounds getBounds(Graphics g)
		{
			return field == null ? Bounds.EMPTY_BOUNDS : field.getBounds(g);
		}

		internal virtual void draw(Component comp, ComponentDrawContext context)
		{
			if (field != null)
			{
				Graphics g = context.Graphics.create();
				field.draw(g);
				g.dispose();
			}
		}

		public virtual void attributeListChanged(AttributeEvent e)
		{
		}

		public virtual void attributeValueChanged(AttributeEvent e)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> attr = e.getAttribute();
			Attribute<object> attr = e.Attribute;
			if (attr == labelAttr)
			{
				updateField(comp.AttributeSet);
			}
			else if (attr == fontAttr)
			{
				if (field != null)
				{
					field.Font = (Font) e.Value;
				}
			}
		}

		public virtual void textChanged(TextFieldEvent e)
		{
			string prev = e.OldText;
			string next = e.Text;
			if (!next.Equals(prev))
			{
				comp.AttributeSet.setValue(labelAttr, next);
			}
		}

		public virtual Action getCommitAction(Circuit circuit, string oldText, string newText)
		{
			SetAttributeAction act = new SetAttributeAction(circuit, Strings.getter("changeLabelAction"));
			act.set(comp, labelAttr, newText);
			return act;
		}

		public virtual Caret getTextCaret(ComponentUserEvent @event)
		{
			canvas = @event.Canvas;
			Graphics g = canvas.getGraphics();

			// if field is absent, create it empty
			// and if it is empty, just return a caret at its beginning
			if (field == null)
			{
				createField(comp.AttributeSet, "");
			}
			string text = field.Text;
			if (string.ReferenceEquals(text, null) || text.Equals(""))
			{
				return field.getCaret(g, 0);
			}

			Bounds bds = field.getBounds(g);
			if (bds.Width < 4 || bds.Height < 4)
			{
				Location loc = comp.Location;
				bds = bds.add(Bounds.create(loc).expand(2));
			}

			int x = @event.X;
			int y = @event.Y;
			if (bds.contains(x, y))
			{
				return field.getCaret(g, x, y);
			}
			else
			{
				return null;
			}
		}
	}

}
