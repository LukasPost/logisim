// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{


	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using SAXException = org.xml.sax.SAXException;

	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using LogisimVersion = logisim.LogisimVersion;
	using Main = logisim.Main;
	using Circuit = logisim.circuit.Circuit;
	using AppearanceSvgReader = logisim.circuit.appear.AppearanceSvgReader;
	using Component = logisim.comp.Component;
	using logisim.data;
	using AttributeDefaultProvider = logisim.data.AttributeDefaultProvider;
	using AttributeSet = logisim.data.AttributeSet;
	using Location = logisim.data.Location;
	using Instance = logisim.instance.Instance;
	using Pin = logisim.std.wiring.Pin;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;
	using InputEventUtil = logisim.util.InputEventUtil;
	using StringUtil = logisim.util.StringUtil;

	internal class XmlReader
	{
		internal class CircuitData
		{
			internal Element circuitElement;
			internal Circuit circuit;
			internal IDictionary<Element, Component> knownComponents;
			internal IList<AbstractCanvasObject> appearance;

			public CircuitData(Element circuitElement, Circuit circuit)
			{
				this.circuitElement = circuitElement;
				this.circuit = circuit;
			}
		}

		internal class ReadContext
		{
			private readonly XmlReader outerInstance;

			internal LogisimFile file;
			internal LogisimVersion sourceVersion;
			internal Dictionary<string, Library> libs = new Dictionary<string, Library>();
			internal List<string> messages;

			internal ReadContext(XmlReader outerInstance, LogisimFile file)
			{
				this.outerInstance = outerInstance;
				this.file = file;
				this.messages = new List<string>();
			}

			internal virtual void addError(string message, string context)
			{
				messages.Add(message + " [" + context + "]");
			}

			internal virtual void addErrors(XmlReaderException exception, string context)
			{
				foreach (string msg in exception.Messages)
				{
					messages.Add(msg + " [" + context + "]");
				}
			}

			internal virtual void toLogisimFile(Element elt)
			{
				// determine the version producing this file
				string versionString = elt.getAttribute("source");
				if (versionString.Equals(""))
				{
					sourceVersion = Main.VERSION;
				}
				else
				{
					sourceVersion = LogisimVersion.parse(versionString);
				}

				// first, load the sublibraries
				foreach (Element o in XmlIterator.forChildElements(elt, "lib"))
				{
					Library lib = toLibrary(o);
					if (lib != null)
					{
						file.addLibrary(lib);
					}
				}

				// second, create the circuits - empty for now
				IList<CircuitData> circuitsData = new List<CircuitData>();
				foreach (Element circElt in XmlIterator.forChildElements(elt, "circuit"))
				{
					string name = circElt.getAttribute("name");
					if (string.ReferenceEquals(name, null) || name.Equals(""))
					{
						addError(Strings.get("circNameMissingError"), "C??");
					}
					CircuitData circData = new CircuitData(circElt, new Circuit(name));
					file.addCircuit(circData.circuit);
					circData.knownComponents = loadKnownComponents(circElt);
					foreach (Element appearElt in XmlIterator.forChildElements(circElt, "appear"))
					{
						loadAppearance(appearElt, circData, name + ".appear");
					}
					circuitsData.Add(circData);
				}

				// third, process the other child elements
				foreach (Element sub_elt in XmlIterator.forChildElements(elt))
				{
					string name = sub_elt.getTagName();
					if (name.Equals("circuit") || name.Equals("lib"))
					{
						; // Nothing to do: Done earlier.
					}
					else if (name.Equals("options"))
					{
						try
						{
							initAttributeSet(sub_elt, file.Options.AttributeSet, null);
						}
						catch (XmlReaderException e)
						{
							addErrors(e, "options");
						}
					}
					else if (name.Equals("mappings"))
					{
						initMouseMappings(sub_elt);
					}
					else if (name.Equals("toolbar"))
					{
						initToolbarData(sub_elt);
					}
					else if (name.Equals("main"))
					{
						string main = sub_elt.getAttribute("name");
						Circuit circ = file.getCircuit(main);
						if (circ != null)
						{
							file.MainCircuit = circ;
						}
					}
					else if (name.Equals("message"))
					{
						file.addMessage(sub_elt.getAttribute("value"));
					}
				}

				// fourth, execute a transaction that initializes all the circuits
				XmlCircuitReader builder;
				builder = new XmlCircuitReader(this, circuitsData);
				builder.execute();
			}

			internal virtual Library toLibrary(Element elt)
			{
				if (!elt.hasAttribute("name"))
				{
					outerInstance.loader.showError(Strings.get("libNameMissingError"));
					return null;
				}
				if (!elt.hasAttribute("desc"))
				{
					outerInstance.loader.showError(Strings.get("libDescMissingError"));
					return null;
				}
				string name = elt.getAttribute("name");
				string desc = elt.getAttribute("desc");
				Library ret = outerInstance.loader.loadLibrary(desc);
				if (ret == null)
				{
					return null;
				}
				libs[name] = ret;
				foreach (Element sub_elt in XmlIterator.forChildElements(elt, "tool"))
				{
					if (!sub_elt.hasAttribute("name"))
					{
						outerInstance.loader.showError(Strings.get("toolNameMissingError"));
					}
					else
					{
						string tool_str = sub_elt.getAttribute("name");
						Tool tool = ret.getTool(tool_str);
						if (tool != null)
						{
							try
							{
								initAttributeSet(sub_elt, tool.AttributeSet, tool);
							}
							catch (XmlReaderException e)
							{
								addErrors(e, "lib." + name + "." + tool_str);
							}
						}
					}
				}
				return ret;
			}

			internal virtual IDictionary<Element, Component> loadKnownComponents(Element elt)
			{
				IDictionary<Element, Component> known = new Dictionary<Element, Component>();
				foreach (Element sub in XmlIterator.forChildElements(elt, "comp"))
				{
					try
					{
						Component comp = XmlCircuitReader.getComponent(sub, this);
						known[sub] = comp;
					}
					catch (XmlReaderException)
					{
					}
				}
				return known;
			}

			internal virtual void loadAppearance(Element appearElt, CircuitData circData, string context)
			{
				IDictionary<Location, Instance> pins = new Dictionary<Location, Instance>();
				foreach (Component comp in circData.knownComponents.Values)
				{
					if (comp.Factory == Pin.FACTORY)
					{
						Instance instance = Instance.getInstanceFor(comp);
						pins[comp.Location] = instance;
					}
				}

				IList<AbstractCanvasObject> shapes = new List<AbstractCanvasObject>();
				foreach (Element sub in XmlIterator.forChildElements(appearElt))
				{
					try
					{
						AbstractCanvasObject m = AppearanceSvgReader.createShape(sub, pins);
						if (m == null)
						{
							addError(Strings.get("fileAppearanceNotFound", sub.getTagName()), context + "." + sub.getTagName());
						}
						else
						{
							shapes.Add(m);
						}
					}
					catch (Exception)
					{
						addError(Strings.get("fileAppearanceError", sub.getTagName()), context + "." + sub.getTagName());
					}
				}
				if (shapes.Count > 0)
				{
					if (circData.appearance == null)
					{
						circData.appearance = shapes;
					}
					else
					{
						((List<AbstractCanvasObject>)circData.appearance).AddRange(shapes);
					}
				}
			}

			internal virtual void initMouseMappings(Element elt)
			{
				MouseMappings map = file.Options.MouseMappings;
				foreach (Element sub_elt in XmlIterator.forChildElements(elt, "tool"))
				{
					Tool tool;
					try
					{
						tool = toTool(sub_elt);
					}
					catch (XmlReaderException e)
					{
						addErrors(e, "mapping");
						continue;
					}

					string mods_str = sub_elt.getAttribute("map");
					if (string.ReferenceEquals(mods_str, null) || mods_str.Equals(""))
					{
						outerInstance.loader.showError(Strings.get("mappingMissingError"));
						continue;
					}
					int mods;
					try
					{
						mods = InputEventUtil.fromString(mods_str);
					}
					catch (System.FormatException)
					{
						outerInstance.loader.showError(StringUtil.format(Strings.get("mappingBadError"), mods_str));
						continue;
					}

					tool = tool.cloneTool();
					try
					{
						initAttributeSet(sub_elt, tool.AttributeSet, tool);
					}
					catch (XmlReaderException e)
					{
						addErrors(e, "mapping." + tool.Name);
					}

					map.setToolFor(mods, tool);
				}
			}

			internal virtual void initToolbarData(Element elt)
			{
				ToolbarData toolbar = file.Options.ToolbarData;
				foreach (Element sub_elt in XmlIterator.forChildElements(elt))
				{
					if (sub_elt.getTagName().Equals("sep"))
					{
						toolbar.addSeparator();
					}
					else if (sub_elt.getTagName().Equals("tool"))
					{
						Tool tool;
						try
						{
							tool = toTool(sub_elt);
						}
						catch (XmlReaderException e)
						{
							addErrors(e, "toolbar");
							continue;
						}
						if (tool != null)
						{
							tool = tool.cloneTool();
							try
							{
								initAttributeSet(sub_elt, tool.AttributeSet, tool);
							}
							catch (XmlReaderException e)
							{
								addErrors(e, "toolbar." + tool.Name);
							}
							toolbar.addTool(tool);
						}
					}
				}
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: logisim.tools.Tool toTool(org.w3c.dom.Element elt) throws XmlReaderException
			internal virtual Tool toTool(Element elt)
			{
				Library lib = findLibrary(elt.getAttribute("lib"));
				string name = elt.getAttribute("name");
				if (string.ReferenceEquals(name, null) || name.Equals(""))
				{
					throw new XmlReaderException(Strings.get("toolNameMissing"));
				}
				Tool tool = lib.getTool(name);
				if (tool == null)
				{
					throw new XmlReaderException(Strings.get("toolNotFound"));
				}
				return tool;
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void initAttributeSet(org.w3c.dom.Element parentElt, logisim.data.AttributeSet attrs, logisim.data.AttributeDefaultProvider defaults) throws XmlReaderException
			internal virtual void initAttributeSet(Element parentElt, AttributeSet attrs, AttributeDefaultProvider defaults)
			{
				List<string> messages = null;

				Dictionary<string, string> attrsDefined = new Dictionary<string, string>();
				foreach (Element attrElt in XmlIterator.forChildElements(parentElt, "a"))
				{
					if (!attrElt.hasAttribute("name"))
					{
						if (messages == null)
						{
							messages = new List<string>();
						}
						messages.Add(Strings.get("attrNameMissingError"));
					}
					else
					{
						string attrName = attrElt.getAttribute("name");
						string attrVal;
						if (attrElt.hasAttribute("val"))
						{
							attrVal = attrElt.getAttribute("val");
						}
						else
						{
							attrVal = attrElt.getTextContent();
						}
						attrsDefined[attrName] = attrVal;
					}
				}

				if (attrs == null)
				{
					return;
				}

				LogisimVersion ver = sourceVersion;
				bool setDefaults = defaults != null && !defaults.isAllDefaultValues(attrs, ver);
				// We need to process this in order, and we have to refetch the
				// attribute list each time because it may change as we iterate
				// (as it will for a splitter).
				for (int i = 0; true; i++)
				{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.List<logisim.data.Attribute<?>> attrList = attrs.getAttributes();
					IList<Attribute<object>> attrList = attrs.Attributes;
					if (i >= attrList.Count)
					{
						break;
					}
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<Object> attr = (logisim.data.Attribute<Object>) attrList.get(i);
					Attribute<object> attr = (Attribute<object>) attrList[i];
					string attrName = attr.Name;
					string attrVal = attrsDefined[attrName];
					if (string.ReferenceEquals(attrVal, null))
					{
						if (setDefaults)
						{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("null") Object val = defaults.getDefaultAttributeValue(attr, ver);
							object val = defaults.getDefaultAttributeValue(attr, ver);
							if (val != null)
							{
								attrs.setValue(attr, val);
							}
						}
					}
					else
					{
						try
						{
							object val = attr.parse(attrVal);
							attrs.setValue(attr, val);
						}
						catch (System.FormatException)
						{
							if (messages == null)
							{
								messages = new List<string>();
							}
							messages.Add(StringUtil.format(Strings.get("attrValueInvalidError"), attrVal, attrName));
						}
					}
				}
				if (messages != null)
				{
					throw new XmlReaderException(messages);
				}
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: logisim.tools.Library findLibrary(String lib_name) throws XmlReaderException
			internal virtual Library findLibrary(string lib_name)
			{
				if (string.ReferenceEquals(lib_name, null) || lib_name.Equals(""))
				{
					return file;
				}

				Library ret = libs[lib_name];
				if (ret == null)
				{
					throw new XmlReaderException(StringUtil.format(Strings.get("libMissingError"), lib_name));
				}
				else
				{
					return ret;
				}
			}
		}

		private LibraryLoader loader;

		internal XmlReader(Loader loader)
		{
			this.loader = loader;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: LogisimFile readLibrary(java.io.InputStream is) throws IOException, org.xml.sax.SAXException
		internal virtual LogisimFile readLibrary(Stream @is)
		{
			Document doc = loadXmlFrom(@is);
			Element elt = doc.getDocumentElement();
			considerRepairs(doc, elt);
			LogisimFile file = new LogisimFile((Loader) loader);
			ReadContext context = new ReadContext(this, file);
			context.toLogisimFile(elt);
			if (file.CircuitCount == 0)
			{
				file.addCircuit(new Circuit("main"));
			}
			if (context.messages.Count > 0)
			{
				StringBuilder all = new StringBuilder();
				foreach (string msg in context.messages)
				{
					all.Append(msg);
					all.Append("\n");
				}
				loader.showError(all.Substring(0, all.Length - 1));
			}
			return file;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private org.w3c.dom.Document loadXmlFrom(java.io.InputStream is) throws SAXException, java.io.IOException
		private Document loadXmlFrom(Stream @is)
		{
			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			factory.setNamespaceAware(true);
			DocumentBuilder builder = null;
			try
			{
				builder = factory.newDocumentBuilder();
				return builder.parse(@is);
			}
			catch (ParserConfigurationException)
			{
			}
			return null;
		}

		private void considerRepairs(Document doc, Element root)
		{
			LogisimVersion version = LogisimVersion.parse(root.getAttribute("source"));
			if (version.compareTo(LogisimVersion.get(2, 3, 0)) < 0)
			{
				// This file was saved before an Edit tool existed. Most likely
				// we should replace the Select and Wiring tools in the toolbar
				// with the Edit tool instead.
				foreach (Element toolbar in XmlIterator.forChildElements(root, "toolbar"))
				{
					Element wiring = null;
					Element select = null;
					Element edit = null;
					foreach (Element elt in XmlIterator.forChildElements(toolbar, "tool"))
					{
						string eltName = elt.getAttribute("name");
						if (!string.ReferenceEquals(eltName, null) && !eltName.Equals(""))
						{
							if (eltName.Equals("Select Tool"))
							{
								select = elt;
							}
							if (eltName.Equals("Wiring Tool"))
							{
								wiring = elt;
							}
							if (eltName.Equals("Edit Tool"))
							{
								edit = elt;
							}
						}
					}
					if (select != null && wiring != null && edit == null)
					{
						select.setAttribute("name", "Edit Tool");
						toolbar.removeChild(wiring);
					}
				}
			}
			if (version.compareTo(LogisimVersion.get(2, 6, 3)) < 0)
			{
				foreach (Element circElt in XmlIterator.forChildElements(root, "circuit"))
				{
					foreach (Element attrElt in XmlIterator.forChildElements(circElt, "a"))
					{
						string name = attrElt.getAttribute("name");
						if (!string.ReferenceEquals(name, null) && name.StartsWith("label", StringComparison.Ordinal))
						{
							attrElt.setAttribute("name", "c" + name);
						}
					}
				}

				repairForWiringLibrary(doc, root);
				repairForLegacyLibrary(doc, root);
			}
		}

		private void repairForWiringLibrary(Document doc, Element root)
		{
			Element oldBaseElt = null;
			string oldBaseLabel = null;
			Element gatesElt = null;
			string gatesLabel = null;
			int maxLabel = -1;
			Element firstLibElt = null;
			Element lastLibElt = null;
			foreach (Element libElt in XmlIterator.forChildElements(root, "lib"))
			{
				string desc = libElt.getAttribute("desc");
				string label = libElt.getAttribute("name");
				if (string.ReferenceEquals(desc, null))
				{
					// skip these tests
				}
				else if (desc.Equals("#Base"))
				{
					oldBaseElt = libElt;
					oldBaseLabel = label;
				}
				else if (desc.Equals("#Wiring"))
				{
					// Wiring library already in file. This shouldn't happen, but if
					// somehow it does, we don't want to add it again.
					return;
				}
				else if (desc.Equals("#Gates"))
				{
					gatesElt = libElt;
					gatesLabel = label;
				}

				if (firstLibElt == null)
				{
					firstLibElt = libElt;
				}
				lastLibElt = libElt;
				try
				{
					if (!string.ReferenceEquals(label, null))
					{
						int thisLabel = int.Parse(label);
						if (thisLabel > maxLabel)
						{
							maxLabel = thisLabel;
						}
					}
				}
				catch (System.FormatException)
				{
				}
			}

			if (lastLibElt == null)
			{
				return;
			}

			Element wiringElt;
			string wiringLabel;
			Element newBaseElt;
			string newBaseLabel;
			if (oldBaseElt != null)
			{
				wiringLabel = oldBaseLabel;
				wiringElt = oldBaseElt;
				wiringElt.setAttribute("desc", "#Wiring");

				newBaseLabel = "" + (maxLabel + 1);
				newBaseElt = doc.createElement("lib");
				newBaseElt.setAttribute("desc", "#Base");
				newBaseElt.setAttribute("name", newBaseLabel);
				root.insertBefore(newBaseElt, lastLibElt.getNextSibling());
			}
			else
			{
				wiringLabel = "" + (maxLabel + 1);
				wiringElt = doc.createElement("lib");
				wiringElt.setAttribute("desc", "#Wiring");
				wiringElt.setAttribute("name", wiringLabel);
				root.insertBefore(wiringElt, lastLibElt.getNextSibling());

				newBaseLabel = null;
				newBaseElt = null;
			}

			Dictionary<string, string> labelMap = new Dictionary<string, string>();
			addToLabelMap(labelMap, oldBaseLabel, newBaseLabel, "Poke Tool;" + "Edit Tool;Select Tool;Wiring Tool;Text Tool;Menu Tool;Text");
			addToLabelMap(labelMap, oldBaseLabel, wiringLabel, "Splitter;Pin;" + "Probe;Tunnel;Clock;Pull Resistor;Bit Extender");
			addToLabelMap(labelMap, gatesLabel, wiringLabel, "Constant");
			relocateTools(oldBaseElt, newBaseElt, labelMap);
			relocateTools(oldBaseElt, wiringElt, labelMap);
			relocateTools(gatesElt, wiringElt, labelMap);
			updateFromLabelMap(XmlIterator.forDescendantElements(root, "comp"), labelMap);
			updateFromLabelMap(XmlIterator.forDescendantElements(root, "tool"), labelMap);
		}

		private void addToLabelMap(Dictionary<string, string> labelMap, string srcLabel, string dstLabel, string toolNames)
		{
			if (!string.ReferenceEquals(srcLabel, null) && !string.ReferenceEquals(dstLabel, null))
			{
				foreach (string tool in toolNames.Split(";", true))
				{
					labelMap[srcLabel + ":" + tool] = dstLabel;
				}
			}
		}

		private void relocateTools(Element src, Element dest, Dictionary<string, string> labelMap)
		{
			if (src == null || src == dest)
			{
				return;
			}
			string srcLabel = src.getAttribute("name");
			if (string.ReferenceEquals(srcLabel, null))
			{
				return;
			}

			List<Element> toRemove = new List<Element>();
			foreach (Element elt in XmlIterator.forChildElements(src, "tool"))
			{
				string name = elt.getAttribute("name");
				if (!string.ReferenceEquals(name, null) && labelMap.ContainsKey(srcLabel + ":" + name))
				{
					toRemove.Add(elt);
				}
			}
			foreach (Element elt in toRemove)
			{
				src.removeChild(elt);
				if (dest != null)
				{
					dest.appendChild(elt);
				}
			}
		}

		private void updateFromLabelMap(IEnumerable<Element> elts, Dictionary<string, string> labelMap)
		{
			foreach (Element elt in elts)
			{
				string oldLib = elt.getAttribute("lib");
				string name = elt.getAttribute("name");
				if (!string.ReferenceEquals(oldLib, null) && !string.ReferenceEquals(name, null))
				{
					string newLib = labelMap[oldLib + ":" + name];
					if (!string.ReferenceEquals(newLib, null))
					{
						elt.setAttribute("lib", newLib);
					}
				}
			}
		}

		private void repairForLegacyLibrary(Document doc, Element root)
		{
			Element legacyElt = null;
			string legacyLabel = null;
			foreach (Element libElt in XmlIterator.forChildElements(root, "lib"))
			{
				string desc = libElt.getAttribute("desc");
				string label = libElt.getAttribute("name");
				if (!string.ReferenceEquals(desc, null) && desc.Equals("#Legacy"))
				{
					legacyElt = libElt;
					legacyLabel = label;
				}
			}

			if (legacyElt != null)
			{
				root.removeChild(legacyElt);

				List<Element> toRemove = new List<Element>();
				findLibraryUses(toRemove, legacyLabel, XmlIterator.forDescendantElements(root, "comp"));
				bool componentsRemoved = toRemove.Count > 0;
				findLibraryUses(toRemove, legacyLabel, XmlIterator.forDescendantElements(root, "tool"));
				foreach (Element elt in toRemove)
				{
					elt.getParentNode().removeChild(elt);
				}
				if (componentsRemoved)
				{
					string error = "Some components have been deleted;" + " the Legacy library is no longer supported.";
					Element elt = doc.createElement("message");
					elt.setAttribute("value", error);
					root.appendChild(elt);
				}
			}
		}

		private static void findLibraryUses(List<Element> dest, string label, IEnumerable<Element> candidates)
		{
			foreach (Element elt in candidates)
			{
				string lib = elt.getAttribute("lib");
				if (lib.Equals(label))
				{
					dest.Add(elt);
				}
			}
		}
	}

}
