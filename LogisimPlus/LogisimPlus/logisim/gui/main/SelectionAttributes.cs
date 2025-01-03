// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using Circuit = logisim.circuit.Circuit;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using Project = logisim.proj.Project;
	using logisim.util;

	internal class SelectionAttributes : AbstractAttributeSet
	{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final logisim.data.Attribute<?>[] EMPTY_ATTRIBUTES = new logisim.data.Attribute<?>[0];
		private static readonly Attribute[] EMPTY_ATTRIBUTES = new Attribute[0];
		private static readonly object[] EMPTY_VALUES = new object[0];

		private class Listener : Selection.Listener, AttributeListener
		{
			private readonly SelectionAttributes outerInstance;

			public Listener(SelectionAttributes outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void selectionChanged(Selection.Event e)
			{
				outerInstance.updateList(true);
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
				if (outerInstance.listening)
				{
					outerInstance.updateList(false);
				}
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				if (outerInstance.listening)
				{
					outerInstance.updateList(false);
				}
			}
		}

		private Canvas canvas;
		private Selection selection;
		private Listener listener;
		private bool listening;
		private HashSet<Component> selected;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private logisim.data.Attribute<?>[] attrs;
		private Attribute[] attrs;
		private bool[] readOnly;
		private object[] values;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<logisim.data.Attribute<?>> attrsView;
		private List<Attribute> attrsView;

		public SelectionAttributes(Canvas canvas, Selection selection)
		{
			this.canvas = canvas;
			this.selection = selection;
			this.listener = new Listener(this);
			this.listening = true;
			this.selected = [];
			this.attrs = EMPTY_ATTRIBUTES;
			this.values = EMPTY_VALUES;
			this.attrsView = [];

			selection.addListener(listener);
			updateList(true);
			Listening = true;
		}

		public virtual Selection Selection
		{
			get
			{
				return selection;
			}
		}

		internal virtual bool Listening
		{
			set
			{
				if (listening != value)
				{
					listening = value;
					if (value)
					{
						updateList(false);
					}
				}
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") private void updateList(boolean ignoreIfSelectionSame)
		private void updateList(bool ignoreIfSelectionSame)
		{
			Selection sel = selection;
            HashSet<Component> oldSel = selected;
            HashSet<Component> newSel;
			if (sel == null)
			{
				newSel = [];
			}
			else
			{
				newSel = createSet(sel.Components);
			}
			if (haveSameElements(newSel, oldSel))
			{
				if (ignoreIfSelectionSame)
				{
					return;
				}
				newSel = oldSel;
			}
			else
			{
				foreach (Component o in oldSel)
				{
					if (!newSel.Contains(o))
					{
						o.AttributeSet.removeAttributeListener(listener);
					}
				}
				foreach (Component o in newSel)
				{
					if (!oldSel.Contains(o))
					{
						o.AttributeSet.addAttributeListener(listener);
					}
				}
			}

			Dictionary<Attribute, object> attrMap = computeAttributes(newSel);
			bool same = isSame(attrMap, this.attrs, this.values);

			if (same)
			{
				if (newSel != oldSel)
				{
					this.selected = newSel;
				}
			}
			else
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] oldAttrs = this.attrs;
				Attribute[] oldAttrs = this.attrs;
				object[] oldValues = this.values;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] newAttrs = new logisim.data.Attribute[attrMap.size()];
				Attribute[] newAttrs = new Attribute[attrMap.size()];
				object[] newValues = new object[newAttrs.Length];
				bool[] newReadOnly = new bool[newAttrs.Length];
				int i = -1;
				foreach (KeyValuePair<Attribute, object> entry in attrMap.entrySet())
				{
					i++;
					newAttrs[i] = entry.Key;
					newValues[i] = entry.Value;
					newReadOnly[i] = computeReadOnly(newSel, newAttrs[i]);
				}
				if (newSel != oldSel)
				{
					this.selected = newSel;
				}
				this.attrs = newAttrs;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: this.attrsView = new logisim.util.UnmodifiableList<logisim.data.Attribute<?>>(newAttrs);
				this.attrsView = new UnmodifiableList<Attribute>(newAttrs);
				this.values = newValues;
				this.readOnly = newReadOnly;

				bool listSame = oldAttrs != null && oldAttrs.Length == newAttrs.Length;
				if (listSame)
				{
					for (i = 0; i < oldAttrs.Length; i++)
					{
						if (!oldAttrs[i].Equals(newAttrs[i]))
						{
							listSame = false;
							break;
						}
					}
				}

				if (listSame)
				{
					for (i = 0; i < oldValues.Length; i++)
					{
						object oldVal = oldValues[i];
						object newVal = newValues[i];
						bool sameVals = oldVal == null ? newVal == null : oldVal.Equals(newVal);
						if (!sameVals)
						{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute attr = (logisim.data.Attribute) oldAttrs[i];
							Attribute attr = (Attribute) oldAttrs[i];
							fireAttributeValueChanged(attr, newVal);
						}
					}
				}
				else
				{
					fireAttributeListChanged();
				}
			}
		}

		private static HashSet<Component> createSet(ICollection<Component> comps)
		{
			bool includeWires = true;
			foreach (Component comp in comps)
			{
				if (!(comp is Wire))
				{
					includeWires = false;
					break;
				}
			}

			if (includeWires)
			{
				return new HashSet<Component>(comps);
			}
			else
			{
				HashSet<Component> ret = new HashSet<Component>();
				foreach (Component comp in comps)
				{
					if (!(comp is Wire))
					{
						ret.Add(comp);
					}
				}
				return ret;
			}
		}

		private static bool haveSameElements(ICollection<Component> a, ICollection<Component> b)
		{
			if (a == null)
			{
				return b == null ? true : b.Count == 0;
			}
			else if (b == null)
			{
				return a.Count == 0;
			}
			else if (a.Count != b.Count)
			{
				return false;
			}
			else
			{
				foreach (Component item in a)
				{
					if (!b.Contains(item))
					{
						return false;
					}
				}
				return true;
			}
		}

		private static Dictionary<Attribute, object> computeAttributes(ICollection<Component> newSel)
		{
			Dictionary<Attribute, object> attrMap;
			attrMap = new Dictionary<Attribute, object>();
			IEnumerator<Component> sit = newSel.GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (sit.hasNext())
			{
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				AttributeSet first = sit.next().getAttributeSet();
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attr : first.getAttributes())
				foreach (Attribute attr in first.Attributes)
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute attrObj = (logisim.data.Attribute) attr;
					Attribute attrObj = (Attribute) attr;
					attrMap.put(attrObj, first.getValue(attr));
				}
				while (sit.MoveNext())
				{
					AttributeSet next = sit.Current.getAttributeSet();
					IEnumerator<Attribute> ait = attrMap.keySet().GetEnumerator();
					while (ait.MoveNext())
					{
						Attribute attr = ait.Current;
						if (next.containsAttribute(attr))
						{
							object v = attrMap.get(attr);
							if (v != null && !v.Equals(next.getValue(attr)))
							{
								attrMap.put(attr, null);
							}
						}
						else
						{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
							ait.remove();
						}
					}
				}
			}
			return attrMap;
		}

		private static bool isSame(Dictionary<Attribute, object> attrMap, Attribute[] oldAttrs, object[] oldValues)
		{
			if (oldAttrs.Length != attrMap.size())
			{
				return false;
			}
			else
			{
				int j = -1;
				foreach (KeyValuePair<Attribute, object> entry in attrMap.entrySet())
				{
					j++;

					Attribute a = entry.Key;
					if (!oldAttrs[j].Equals(a) || j >= oldValues.Length)
					{
						return false;
					}
					object ov = oldValues[j];
					object nv = entry.Value;
					if (ov == null ? nv != null :!ov.Equals(nv))
					{
						return false;
					}
				}
				return true;
			}
		}

		private static bool computeReadOnly(ICollection<Component> sel, Attribute attr)
		{
			foreach (Component comp in sel)
			{
				AttributeSet attrs = comp.AttributeSet;
				if (attrs.isReadOnly(attr))
				{
					return true;
				}
			}
			return false;
		}

		protected internal override void copyInto(AbstractAttributeSet dest)
		{
			throw new System.NotSupportedException("SelectionAttributes.copyInto");
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				Circuit circ = canvas.Circuit;
				if (selected.Count == 0 && circ != null)
				{
					return circ.StaticAttributes.Attributes;
				}
				else
				{
					return attrsView;
				}
			}
		}

		public override bool isReadOnly(Attribute attr)
		{
			Project proj = canvas.Project;
			Circuit circ = canvas.Circuit;
			if (!proj.LogisimFile.contains(circ))
			{
				return true;
			}
			else if (selected.Count == 0 && circ != null)
			{
				return circ.StaticAttributes.isReadOnly(attr);
			}
			else
			{
				int i = findIndex(attr);
				bool[] ro = readOnly;
				return i >= 0 && i < ro.Length ? ro[i] : true;
			}
		}

		public override bool isToSave(Attribute attr)
		{
			return false;
		}

		public override object getValue(Attribute attr)
		{
			Circuit circ = canvas.Circuit;
			if (selected.Count == 0 && circ != null)
			{
				return circ.StaticAttributes.getValue(attr);
			}
			else
			{
				int i = findIndex(attr);
				object[] vs = values;
                return  (i >= 0 && i < vs.Length ? vs[i] : null);
			}
		}

		public override void setValue(Attribute attr, object value)
		{
			Circuit circ = canvas.Circuit;
			if (selected.Count == 0 && circ != null)
			{
				circ.StaticAttributes.setValue(attr, value);
			}
			else
			{
				int i = findIndex(attr);
				object[] vs = values;
				if (i >= 0 && i < vs.Length)
				{
					vs[i] = value;
					foreach (Component comp in selected)
					{
						comp.AttributeSet.setValue(attr, value);
					}
				}
			}
		}

		private int findIndex(Attribute attr)
		{
			if (attr == null)
			{
				return -1;
			}
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?>[] as = attrs;
			Attribute[] @as = attrs;
			for (int i = 0; i < @as.Length; i++)
			{
				if (attr.Equals(@as[i]))
				{
					return i;
				}
			}
			return -1;
		}
	}

}
