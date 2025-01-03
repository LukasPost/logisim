﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.gui
{

	using Selection = draw.canvas.Selection;
	using SelectionEvent = draw.canvas.SelectionEvent;
	using SelectionListener = draw.canvas.SelectionListener;
	using CanvasObject = draw.model.CanvasObject;
	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;

	public class SelectionAttributes : AbstractAttributeSet
	{
		private class Listener : SelectionListener, AttributeListener
		{
			private readonly SelectionAttributes outerInstance;

			public Listener(SelectionAttributes outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			//
			// SelectionListener
			//
			public virtual void selectionChanged(SelectionEvent ex)
			{
				Dictionary<AttributeSet, CanvasObject> oldSel = outerInstance.selected;
				Dictionary<AttributeSet, CanvasObject> newSel = new Dictionary<AttributeSet, CanvasObject>();
				foreach (CanvasObject o in outerInstance.selection.Selected)
				{
					newSel[o.AttributeSet] = o;
				}
				outerInstance.selected = newSel;
				bool change = false;
				foreach (AttributeSet attrs in oldSel.Keys)
				{
					if (!newSel.ContainsKey(attrs))
					{
						change = true;
						attrs.removeAttributeListener(this);
					}
				}
				foreach (AttributeSet attrs in newSel.Keys)
				{
					if (!oldSel.ContainsKey(attrs))
					{
						change = true;
						attrs.addAttributeListener(this);
					}
				}
				if (change)
				{
					computeAttributeList(newSel.Keys);
					outerInstance.fireAttributeListChanged();
				}
			}

			internal virtual void computeAttributeList(IEnumerable<AttributeSet> attrsSet)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Set<logisim.data.Attribute<?>> attrSet = new java.util.LinkedHashSet<logisim.data.Attribute<?>>();
				HashSet<Attribute> attrSet = new HashSet<Attribute>();
				IEnumerator<AttributeSet> sit = attrsSet.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (sit.hasNext())
				{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					AttributeSet first = sit.next();
					attrSet.addAll(first.Attributes);
					while (sit.MoveNext())
					{
						AttributeSet next = sit.Current;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (java.util.Iterator<logisim.data.Attribute<?>> ait = attrSet.iterator(); ait.hasNext();)
						for (IEnumerator<Attribute> ait = attrSet.GetEnumerator(); ait.MoveNext();)
						{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> attr = ait.Current;
							Attribute attr = ait.Current;
							if (!next.containsAttribute(attr))
							{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
								ait.remove();
							}
						}
					}
				}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] attrs = new logisim.data.Attribute[attrSet.size()];
				Attribute[] attrs = new Attribute[attrSet.Count];
				object[] values = new object[attrs.Length];
				int i = 0;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : attrSet)
				foreach (Attribute attr in attrSet)
				{
					attrs[i] = attr;
					values[i] = getSelectionValue(attr, attrsSet);
					i++;
				}
				outerInstance.selAttrs = attrs;
				outerInstance.selValues = values;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: SelectionAttributes.this.attrsView = java.util.Collections.unmodifiableList(java.util.Arrays.asList(attrs));
				outerInstance.attrsView = attrs.ToList();
				outerInstance.fireAttributeListChanged();
			}

			//
			// AttributeSet listener
			//
			public virtual void attributeListChanged(AttributeEvent e)
			{
				// show selection attributes
				computeAttributeList(outerInstance.selected.Keys);
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				if (outerInstance.selected.ContainsKey(e.Source))
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute attr = (logisim.data.Attribute) e.getAttribute();
					Attribute attr = (Attribute) e.Attribute;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] attrs = SelectionAttributes.this.selAttrs;
					Attribute[] attrs = outerInstance.selAttrs;
					object[] values = outerInstance.selValues;
					for (int i = 0; i < attrs.Length; i++)
					{
						if (attrs[i] == attr)
						{
							values[i] = getSelectionValue(attr, outerInstance.selected.Keys);
						}
					}
				}
			}
		}

		private Selection selection;
		private Listener listener;
		private Dictionary<AttributeSet, CanvasObject> selected;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private logisim.data.Attribute<?>[] selAttrs;
		private Attribute[] selAttrs;
		private object[] selValues;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<logisim.data.Attribute<?>> attrsView;
		private List<Attribute> attrsView;

		public SelectionAttributes(Selection selection)
		{
			this.selection = selection;
			this.listener = new Listener(this);
			this.selected = [];
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: this.selAttrs = new logisim.data.Attribute<?>[0];
			this.selAttrs = new Attribute[0];
			this.selValues = new object[0];
			this.attrsView = selAttrs.ToList();

			selection.addSelectionListener(listener);
			listener.selectionChanged(null);
		}

		public virtual IEnumerable<KeyValuePair<AttributeSet, CanvasObject>> entries()
		{
			HashSet<KeyValuePair<AttributeSet, CanvasObject>> raw = selected.SetOfKeyValuePairs();
			List<KeyValuePair<AttributeSet, CanvasObject>> ret;
			ret = new List<KeyValuePair<AttributeSet, CanvasObject>>(raw);
			return ret;
		}

		//
		// AbstractAttributeSet methods
		//
		protected internal override void copyInto(AbstractAttributeSet dest)
		{
			listener = new Listener(this);
			selection.addSelectionListener(listener);
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return attrsView;
			}
		}

		public override object getValue(Attribute attr)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] attrs = this.selAttrs;
			Attribute[] attrs = this.selAttrs;
			object[] values = this.selValues;
			for (int i = 0; i < attrs.Length; i++)
			{
				if (attrs[i].Equals(attr))
				{
                    // JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
                    // ORIGINAL LINE: @SuppressWarnings("unchecked") V ret = (V) values[i];
                    return values[i];
				}
			}
			return null;
		}

		public override void setValue(Attribute attr, object value)
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] attrs = this.selAttrs;
			Attribute[] attrs = this.selAttrs;
			object[] values = this.selValues;
			for (int i = 0; i < attrs.Length; i++)
			{
				if (attrs[i] == attr)
				{
					bool same = value == null ? values[i] == null : value.Equals(values[i]);
					if (!same)
					{
						values[i] = value;
						foreach (AttributeSet objAttrs in selected.Keys)
						{
							objAttrs.setValue(attr, value);
						}
					}
					break;
				}
			}
		}

		private static object getSelectionValue(Attribute attr, IEnumerable<AttributeSet> sel)
		{
			object ret = null;
			foreach (AttributeSet attrs in sel)
			{
				if (attrs.containsAttribute(attr))
				{
					object val = attrs.getValue(attr);
					if (ret == null)
					{
						ret = val;
					}
					else if (val != null && val.Equals(ret))
					{
						; // keep on, making sure everything else matches
					}
					else
					{
						return null;
					}
				}
			}
			return ret;
		}
	}

}
