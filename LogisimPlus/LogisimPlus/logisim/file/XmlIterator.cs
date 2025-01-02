// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{

	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	public class XmlIterator<E> : IEnumerable<E>, IEnumerator<E>, ICloneable where E : org.w3c.dom.Node
	{
		public static XmlIterator<Node> forChildren(Element node)
		{
			return new XmlIterator<Node>(node.getChildNodes());
		}

		public static IEnumerable<Element> forChildElements(Element node)
		{
			NodeList nodes = node.getChildNodes();
			List<Element> ret = new List<Element>();
			for (int i = 0, n = nodes.getLength(); i < n; i++)
			{
				Node sub = nodes.item(i);
				if (sub.getNodeType() == Node.ELEMENT_NODE)
				{
					ret.Add((Element) sub);
				}
			}
			return ret;
		}

		public static IEnumerable<Element> forChildElements(Element node, string tagName)
		{
			NodeList nodes = node.getChildNodes();
			List<Element> ret = new List<Element>();
			for (int i = 0, n = nodes.getLength(); i < n; i++)
			{
				Node sub = nodes.item(i);
				if (sub.getNodeType() == Node.ELEMENT_NODE)
				{
					Element elt = (Element) sub;
					if (elt.getTagName().Equals(tagName))
					{
						ret.Add(elt);
					}
				}
			}
			return ret;
		}

		public static IEnumerable<Element> forDescendantElements(Element node, string tagName)
		{
			return new XmlIterator<Element>(node.getElementsByTagName(tagName));
		}

		private NodeList list;
		private int index;

		public XmlIterator(NodeList nodes)
		{
			list = nodes;
			index = 0;
		}

		public override XmlIterator<E> clone()
		{
			try
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") XmlIterator<E> ret = (XmlIterator<E>) super.clone();
				XmlIterator<E> ret = (XmlIterator<E>) base.clone();
				return ret;
			}
			catch (CloneNotSupportedException)
			{
				return this;
			}
		}

		public virtual IEnumerator<E> GetEnumerator()
		{
			XmlIterator<E> ret = this.clone();
			ret.index = 0;
			return ret;
		}

		public virtual bool hasNext()
		{
			return list != null && index < list.getLength();
		}

		public virtual E next()
		{
			Node ret = list.item(index);
			if (ret == null)
			{
				throw new NoSuchElementException();
			}
			else
			{
				index++;
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") E ret2 = (E) ret;
				E ret2 = (E) ret;
				return ret2;
			}
		}

		public virtual void remove()
		{
			throw new System.NotSupportedException("XmlChildIterator.remove");
		}
	}

}
