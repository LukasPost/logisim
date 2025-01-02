// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{

	public class AttributeSets
	{
		private AttributeSets()
		{
		}

		public static readonly AttributeSet EMPTY = new AttributeSetAnonymousInnerClass();

		private class AttributeSetAnonymousInnerClass : AttributeSet
		{
			public object clone()
			{
				return this;
			}

			public void addAttributeListener(AttributeListener l)
			{
			}

			public void removeAttributeListener(AttributeListener l)
			{
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public java.util.List<Attribute<?>> getAttributes()
			public IList<Attribute<object>> Attributes
			{
				get
				{
					return Collections.emptyList();
				}
			}

			public bool containsAttribute<T1>(Attribute<T1> attr)
			{
				return false;
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public Attribute<?> getAttribute(String name)
			public Attribute<object> getAttribute(string name)
			{
				return null;
			}

			public bool isReadOnly<T1>(Attribute<T1> attr)
			{
				return true;
			}

			public void setReadOnly<T1>(Attribute<T1> attr, bool value)
			{
				throw new System.NotSupportedException();
			}

			public bool isToSave<T1>(Attribute<T1> attr)
			{
				return true;
			}

			public V getValue<V>(Attribute<V> attr)
			{
				return default(V);
			}

			public void setValue<V>(Attribute<V> attr, V value)
			{
			}
		}

		public static AttributeSet fixedSet<V>(Attribute<V> attr, V initValue)
		{
			return new SingletonSet(attr, initValue);
		}

		public static AttributeSet fixedSet<T1>(Attribute<T1>[] attrs, object[] initValues)
		{
			if (attrs.Length > 1)
			{
				return new FixedSet(attrs, initValues);
			}
			else if (attrs.Length == 1)
			{
				return new SingletonSet(attrs[0], initValues[0]);
			}
			else
			{
				return EMPTY;
			}
		}

		public static void copy(AttributeSet src, AttributeSet dst)
		{
			if (src == null || src.Attributes == null)
			{
				return;
			}
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (Attribute<?> attr : src.getAttributes())
			foreach (Attribute<object> attr in src.Attributes)
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") Attribute<Object> attrObj = (Attribute<Object>) attr;
				Attribute<object> attrObj = (Attribute<object>) attr;
				object value = src.getValue(attr);
				dst.setValue(attrObj, value);
			}
		}

		private class SingletonSet : AbstractAttributeSet
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<Attribute<?>> attrs;
			internal IList<Attribute<object>> attrs;
			internal object value;
			internal bool readOnly = false;

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: SingletonSet(Attribute<?> attr, Object initValue)
			internal SingletonSet(Attribute<T1> attr, object initValue)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: this.attrs = new java.util.ArrayList<Attribute<?>>(1);
				this.attrs = new List<Attribute<object>>(1);
				this.attrs.Add(attr);
				this.value = initValue;
			}

			protected internal override void copyInto(AbstractAttributeSet destSet)
			{
				SingletonSet dest = (SingletonSet) destSet;
				dest.attrs = this.attrs;
				dest.value = this.value;
				dest.readOnly = this.readOnly;
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<Attribute<?>> getAttributes()
			public override IList<Attribute<object>> Attributes
			{
				get
				{
					return attrs;
				}
			}

			public virtual bool isReadOnly<T1>(Attribute<T1> attr)
			{
				return readOnly;
			}

			public virtual void setReadOnly<T1>(Attribute<T1> attr, bool value)
			{
				int index = attrs.IndexOf(attr);
				if (index < 0)
				{
					throw new System.ArgumentException("attribute " + attr.Name + " absent");
				}
				readOnly = value;
			}

			public override V getValue<V>(Attribute<V> attr)
			{
				int index = attrs.IndexOf(attr);
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") V ret = (V)(index >= 0 ? value : null);
				V ret = (V)(index >= 0 ? value : null);
				return ret;
			}

			public override void setValue<V>(Attribute<V> attr, V value)
			{
				int index = attrs.IndexOf(attr);
				if (index < 0)
				{
					throw new System.ArgumentException("attribute " + attr.Name + " absent");
				}
				if (readOnly)
				{
					throw new System.ArgumentException("read only");
				}
				this.value = value;
				fireAttributeValueChanged(attr, value);
			}
		}

		private class FixedSet : AbstractAttributeSet
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.List<Attribute<?>> attrs;
			internal IList<Attribute<object>> attrs;
			internal object[] values;
			internal int readOnly = 0;

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: FixedSet(Attribute<?>[] attrs, Object[] initValues)
			internal FixedSet(Attribute<T1>[] attrs, object[] initValues)
			{
				if (attrs.Length != initValues.Length)
				{
					throw new System.ArgumentException("attribute and value arrays must have same length");
				}
				if (attrs.Length > 32)
				{
					throw new System.ArgumentException("cannot handle more than 32 attributes");
				}
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: this.attrs = java.util.Arrays.asList(attrs);
				this.attrs = new List<Attribute<object>> {attrs};
				this.values = (object[])initValues.Clone();
			}

			protected internal override void copyInto(AbstractAttributeSet destSet)
			{
				FixedSet dest = (FixedSet) destSet;
				dest.attrs = this.attrs;
				dest.values = (object[])this.values.Clone();
				dest.readOnly = this.readOnly;
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<Attribute<?>> getAttributes()
			public override IList<Attribute<object>> Attributes
			{
				get
				{
					return attrs;
				}
			}

			public virtual bool isReadOnly<T1>(Attribute<T1> attr)
			{
				int index = attrs.IndexOf(attr);
				if (index < 0)
				{
					return true;
				}
				return isReadOnly(index);
			}

			public virtual void setReadOnly<T1>(Attribute<T1> attr, bool value)
			{
				int index = attrs.IndexOf(attr);
				if (index < 0)
				{
					throw new System.ArgumentException("attribute " + attr.Name + " absent");
				}

				if (value)
				{
					readOnly |= (1 << index);
				}
				else
				{
					readOnly &= ~(1 << index);
				}
			}

			public override V getValue<V>(Attribute<V> attr)
			{
				int index = attrs.IndexOf(attr);
				if (index < 0)
				{
					return null;
				}
				else
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") V ret = (V) values[index];
					V ret = (V) values[index];
					return ret;
				}
			}

			public override void setValue<V>(Attribute<V> attr, V value)
			{
				int index = attrs.IndexOf(attr);
				if (index < 0)
				{
					throw new System.ArgumentException("attribute " + attr.Name + " absent");
				}
				if (isReadOnly(index))
				{
					throw new System.ArgumentException("read only");
				}
				values[index] = value;
				fireAttributeValueChanged(attr, value);
			}

			internal virtual bool isReadOnly(int index)
			{
				return ((readOnly >> index) & 1) == 1;
			}
		}

	}

}
