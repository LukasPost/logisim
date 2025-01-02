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

	public class AttributeSetImpl : AbstractAttributeSet
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			list = new AttrList(this);
		}

		private class Node
		{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: Attribute<?> attr;
			internal Attribute<object> attr;
			internal object value;
			internal bool is_read_only;
			internal Node next;

// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
// ORIGINAL LINE: Node(Attribute<?> attr, Object value, boolean is_read_only, Node next)
			internal Node(Attribute<T1> attr, object value, bool is_read_only, Node next)
			{
				this.attr = attr;
				this.value = value;
				this.is_read_only = is_read_only;
				this.next = next;
			}

			internal Node(Node other)
			{
				this.attr = other.attr;
				this.value = other.value;
				this.is_read_only = other.is_read_only;
				this.next = other.next;
			}
		}

		private class AttrIterator : IEnumerator<Attribute<JavaToCSharpGenericWildcard>>
		{
			private readonly AttributeSetImpl outerInstance;

			internal Node n;

			internal AttrIterator(AttributeSetImpl outerInstance, Node n)
			{
				this.outerInstance = outerInstance;
				this.n = n;
			}

			public virtual bool hasNext()
			{
				return n != null;
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public Attribute<?> next()
			public virtual Attribute<object> next()
			{
				Node ret = n;
				n = n.next;
				return ret.attr;
			}

			public virtual void remove()
			{
				throw new System.NotSupportedException();
			}
		}

		private class AttrList : System.Collections.ObjectModel.Collection<Attribute<JavaToCSharpGenericWildcard>>
		{
			private readonly AttributeSetImpl outerInstance;

			public AttrList(AttributeSetImpl outerInstance)
			{
				this.outerInstance = outerInstance;
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.Iterator<Attribute<?>> iterator()
			public override IEnumerator<Attribute<object>> iterator()
			{
				return new AttrIterator(outerInstance, outerInstance.head);
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public Attribute<?> get(int i)
			public override Attribute<object> get(int i)
			{
				Node n = outerInstance.head;
				int remaining = i;
				while (remaining != 0 && n != null)
				{
					n = n.next;
					--remaining;
				}
				if (remaining != 0 || n == null)
				{
					throw new System.IndexOutOfRangeException(i + " not in list " + " [" + outerInstance.count + " elements]");
				}
				return n.attr;
			}

			public override bool contains(object o)
			{
				return IndexOf(o) != -1;
			}

			public override int indexOf(object o)
			{
				Node n = outerInstance.head;
				int ret = 0;
				while (n != null)
				{
					if (o.Equals(n.attr))
					{
						return ret;
					}
					n = n.next;
					++ret;
				}
				return -1;
			}

			public override int size()
			{
				return outerInstance.count;
			}
		}

		private AttrList list;
		private Node head = null;
		private Node tail = null;
		private int count = 0;

		public AttributeSetImpl()
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
		}

		public AttributeSetImpl(Attribute<object>[] attrs, object[] values)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			if (attrs.Length != values.Length)
			{
				throw new System.ArgumentException("arrays must have same length");
			}

			for (int i = 0; i < attrs.Length; i++)
			{
				addAttribute(attrs[i], values[i]);
			}
		}

		protected internal override void copyInto(AbstractAttributeSet destObj)
		{
			AttributeSetImpl dest = (AttributeSetImpl) destObj;
			if (this.head != null)
			{
				dest.head = new Node(head);
				Node copy_prev = dest.head;
				Node cur = this.head.next;
				while (cur != null)
				{
					Node copy_cur = new Node(cur);
					copy_prev.next = copy_cur;
					copy_prev = copy_cur;
					cur = cur.next;
				}
				dest.tail = copy_prev;
				dest.count = this.count;
			}
		}

		//
		// attribute access methods
		//
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return list;
			}
		}

// JAVA TO C# CONVERTER TASK: There is no C# equivalent to the Java 'super' constraint:
// ORIGINAL LINE: public <V> void addAttribute(Attribute<? super V> attr, V value)
		public virtual void addAttribute<V, T1>(Attribute<T1> attr, V value)
		{
			if (attr == null)
			{
				throw new System.ArgumentException("Adding null attribute");
			}
			if (findNode(attr) != null)
			{
				throw new System.ArgumentException("Attribute " + attr + " already created");
			}

			Node n = new Node(attr, value, false, null);
			if (head == null)
			{
				head = n;
			}
			else
			{
				tail.next = n;
			}
			tail = n;
			++count;
			fireAttributeListChanged();
		}

		public virtual void removeAttribute<T1>(Attribute<T1> attr)
		{
			Node prev = null;
			Node n = head;
			while (n != null)
			{
				if (n.attr.Equals(attr))
				{
					if (tail == n)
					{
						tail = prev;
					}
					if (prev == null)
					{
						head = n.next;
					}
					else
					{
						prev.next = n.next;
					}
					--count;
					fireAttributeListChanged();
					return;
				}
				prev = n;
				n = n.next;
			}
			throw new System.ArgumentException("Attribute " + attr + " absent");
		}

		//
		// read-only methods
		//
		public virtual bool isReadOnly<T1>(Attribute<T1> attr)
		{
			Node n = findNode(attr);
			if (n == null)
			{
				throw new System.ArgumentException("Unknown attribute " + attr);
			}
			return n.is_read_only;
		}

		public virtual void setReadOnly<T1>(Attribute<T1> attr, bool value)
		{
			Node n = findNode(attr);
			if (n == null)
			{
				throw new System.ArgumentException("Unknown attribute " + attr);
			}
			n.is_read_only = value;
		}

		//
		// value access methods
		//
		public override V getValue<V>(Attribute<V> attr)
		{
			Node n = findNode(attr);
			if (n == null)
			{
				throw new System.ArgumentException("Unknown attribute " + attr);
			}
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") V ret = (V) n.value;
			V ret = (V) n.value;
			return ret;
		}

		public override void setValue<V>(Attribute<V> attr, V value)
		{
			if (value is string)
			{
				value = attr.parse((string) value);
			}

			Node n = findNode(attr);
			if (n == null)
			{
				throw new System.ArgumentException("Unknown attribute " + attr);
			}
			if (n.is_read_only)
			{
				throw new System.ArgumentException("Attribute " + attr + " is read-only");
			}
			if (value.Equals(n.value))
			{
				; // do nothing - why change what's already there?
			}
			else
			{
				n.value = value;
				fireAttributeValueChanged(attr, value);
			}
		}

		//
		// private helper methods
		//
		private Node findNode<T1>(Attribute<T1> attr)
		{
			for (Node n = head; n != null; n = n.next)
			{
				if (n.attr.Equals(attr))
				{
					return n;
				}
			}
			return null;
		}
	}

}
