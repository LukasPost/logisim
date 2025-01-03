// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{

	using Element = org.w3c.dom.Element;

	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using Circuit = logisim.circuit.Circuit;
	using CircuitMutator = logisim.circuit.CircuitMutator;
	using CircuitTransaction = logisim.circuit.CircuitTransaction;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using AttributeSet = logisim.data.AttributeSet;
	using Location = logisim.data.Location;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class XmlCircuitReader : CircuitTransaction
	{
		private XmlReader.ReadContext reader;
		private List<XmlReader.CircuitData> circuitsData;

		public XmlCircuitReader(XmlReader.ReadContext reader, List<XmlReader.CircuitData> circDatas)
		{
			this.reader = reader;
			this.circuitsData = circDatas;
		}

		protected internal override Dictionary<Circuit, int> AccessedCircuits
		{
			get
			{
				Dictionary<Circuit, int> access = new Dictionary<Circuit, int>();
				foreach (XmlReader.CircuitData data in circuitsData)
				{
					access[data.circuit] = READ_WRITE;
				}
				return access;
			}
		}

		protected internal override void run(CircuitMutator mutator)
		{
			foreach (XmlReader.CircuitData circuitData in circuitsData)
			{
				buildCircuit(circuitData, mutator);
			}
		}

		private void buildCircuit(XmlReader.CircuitData circData, CircuitMutator mutator)
		{
			Element elt = circData.circuitElement;
			Circuit dest = circData.circuit;
			Dictionary<Element, Component> knownComponents = circData.knownComponents;
			if (knownComponents == null)
			{
				knownComponents = Collections.emptyMap();
			}
			try
			{
				reader.initAttributeSet(circData.circuitElement, dest.StaticAttributes, null);
			}
			catch (XmlReaderException e)
			{
				reader.addErrors(e, circData.circuit.Name + ".static");
			}

			foreach (Element sub_elt in XmlIterator.forChildElements(elt))
			{
				string sub_elt_name = sub_elt.getTagName();
				if (sub_elt_name.Equals("comp"))
				{
					try
					{
						Component comp = knownComponents[sub_elt];
						if (comp == null)
						{
							comp = getComponent(sub_elt, reader);
						}
						mutator.add(dest, comp);
					}
					catch (XmlReaderException e)
					{
						reader.addErrors(e, circData.circuit.Name + "." + toComponentString(sub_elt));
					}
				}
				else if (sub_elt_name.Equals("wire"))
				{
					try
					{
						addWire(dest, mutator, sub_elt);
					}
					catch (XmlReaderException e)
					{
						reader.addErrors(e, circData.circuit.Name + "." + toWireString(sub_elt));
					}
				}
			}

			List<AbstractCanvasObject> appearance = circData.appearance;
			if (appearance != null && appearance.Count > 0)
			{
				dest.Appearance.ObjectsForce = appearance;
				dest.Appearance.DefaultAppearance = false;
			}
		}

		private string toComponentString(Element elt)
		{
			string name = elt.getAttribute("name");
			string loc = elt.getAttribute("loc");
			return name + "(" + loc + ")";
		}

		private string toWireString(Element elt)
		{
			string from = elt.getAttribute("from");
			string to = elt.getAttribute("to");
			return "w" + from + "-" + to;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void addWire(logisim.circuit.Circuit dest, logisim.circuit.CircuitMutator mutator, org.w3c.dom.Element elt) throws XmlReaderException
		internal virtual void addWire(Circuit dest, CircuitMutator mutator, Element elt)
		{
			Location pt0;
			try
			{
				string str = elt.getAttribute("from");
				if (string.ReferenceEquals(str, null) || str.Equals(""))
				{
					throw new XmlReaderException(Strings.get("wireStartMissingError"));
				}
				pt0 = Location.parse(str);
			}
			catch (System.FormatException)
			{
				throw new XmlReaderException(Strings.get("wireStartInvalidError"));
			}

			Location pt1;
			try
			{
				string str = elt.getAttribute("to");
				if (string.ReferenceEquals(str, null) || str.Equals(""))
				{
					throw new XmlReaderException(Strings.get("wireEndMissingError"));
				}
				pt1 = Location.parse(str);
			}
			catch (System.FormatException)
			{
				throw new XmlReaderException(Strings.get("wireEndInvalidError"));
			}

			mutator.add(dest, Wire.create(pt0, pt1));
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static logisim.comp.Component getComponent(org.w3c.dom.Element elt, XmlReader.ReadContext reader) throws XmlReaderException
		internal static Component getComponent(Element elt, XmlReader.ReadContext reader)
		{
			// Determine the factory that creates this element
			string name = elt.getAttribute("name");
			if (string.ReferenceEquals(name, null) || name.Equals(""))
			{
				throw new XmlReaderException(Strings.get("compNameMissingError"));
			}

			string libName = elt.getAttribute("lib");
			Library lib = reader.findLibrary(libName);
			if (lib == null)
			{
				throw new XmlReaderException(Strings.get("compUnknownError", "no-lib"));
			}

			Tool tool = lib.getTool(name);
			if (tool == null || !(tool is AddTool))
			{
				if (string.ReferenceEquals(libName, null) || libName.Equals(""))
				{
					throw new XmlReaderException(Strings.get("compUnknownError", name));
				}
				else
				{
					throw new XmlReaderException(Strings.get("compAbsentError", name, libName));
				}
			}
			ComponentFactory source = ((AddTool) tool).Factory;

			// Determine attributes
			string loc_str = elt.getAttribute("loc");
			AttributeSet attrs = source.createAttributeSet();
			reader.initAttributeSet(elt, attrs, source);

			// Create component if location known
			if (string.ReferenceEquals(loc_str, null) || loc_str.Equals(""))
			{
				throw new XmlReaderException(Strings.get("compLocMissingError", source.Name));
			}
			else
			{
				try
				{
					Location loc = Location.parse(loc_str);
					return source.createComponent(loc, attrs);
				}
				catch (System.FormatException)
				{
					throw new XmlReaderException(Strings.get("compLocInvalidError", source.Name, loc_str));
				}
			}
		}
	}

}
