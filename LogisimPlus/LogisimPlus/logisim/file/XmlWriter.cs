// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{


	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;

	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using LogisimVersion = logisim.LogisimVersion;
	using Main = logisim.Main;
	using Circuit = logisim.circuit.Circuit;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using logisim.data;
	using AttributeDefaultProvider = logisim.data.AttributeDefaultProvider;
	using AttributeSet = logisim.data.AttributeSet;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;
	using InputEventUtil = logisim.util.InputEventUtil;
	using StringUtil = logisim.util.StringUtil;

	internal class XmlWriter
	{
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static void write(LogisimFile file, java.io.OutputStream out, LibraryLoader loader) throws ParserConfigurationException, TransformerConfigurationException, javax.xml.transform.TransformerException
		internal static void write(LogisimFile file, Stream @out, LibraryLoader loader)
		{
			DocumentBuilderFactory docFactory = DocumentBuilderFactory.newInstance();
			DocumentBuilder docBuilder = docFactory.newDocumentBuilder();

			Document doc = docBuilder.newDocument();
			XmlWriter context = new XmlWriter(file, doc, loader);
			context.fromLogisimFile();

			TransformerFactory tfFactory = TransformerFactory.newInstance();
			try
			{
				tfFactory.setAttribute("indent-number", Convert.ToInt32(2));
			}
			catch (System.ArgumentException)
			{
			}
			Transformer tf = tfFactory.newTransformer();
			tf.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
			tf.setOutputProperty(OutputKeys.INDENT, "yes");
			try
			{
				tf.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "2");
			}
			catch (System.ArgumentException)
			{
			}

			Source src = new DOMSource(doc);
			Result dest = new StreamResult(@out);
			tf.transform(src, dest);
		}

		private LogisimFile file;
		private Document doc;
		private LibraryLoader loader;
		private Dictionary<Library, string> libs = new Dictionary<Library, string>();

		private XmlWriter(LogisimFile file, Document doc, LibraryLoader loader)
		{
			this.file = file;
			this.doc = doc;
			this.loader = loader;
		}

		internal virtual Element fromLogisimFile()
		{
			Element ret = doc.createElement("project");
			doc.appendChild(ret);
			ret.appendChild(doc.createTextNode("\nThis file is intended to be " + "loaded by Logisim (http://www.cburch.com/logisim/).\n"));
			ret.setAttribute("version", "1.0");
			ret.setAttribute("source", Main.VERSION_NAME);

			foreach (Library lib in file.Libraries)
			{
				Element elt = fromLibrary(lib);
				if (elt != null)
				{
					ret.appendChild(elt);
				}
			}

			if (file.MainCircuit != null)
			{
				Element mainElt = doc.createElement("main");
				mainElt.setAttribute("name", file.MainCircuit.getName());
				ret.appendChild(mainElt);
			}

			ret.appendChild(fromOptions());
			ret.appendChild(fromMouseMappings());
			ret.appendChild(fromToolbarData());

			foreach (Circuit circ in file.Circuits)
			{
				ret.appendChild(fromCircuit(circ));
			}
			return ret;
		}

		internal virtual Element fromLibrary(Library lib)
		{
			Element ret = doc.createElement("lib");
			if (libs.ContainsKey(lib))
			{
				return null;
			}
			string name = "" + libs.Count;
			string desc = loader.getDescriptor(lib);
			if (string.ReferenceEquals(desc, null))
			{
				loader.showError("library location unknown: " + lib.Name);
				return null;
			}
			libs[lib] = name;
			ret.setAttribute("name", name);
			ret.setAttribute("desc", desc);
			foreach (Tool t in lib.Tools)
			{
				AttributeSet attrs = t.AttributeSet;
				if (attrs != null)
				{
					Element toAdd = doc.createElement("tool");
					toAdd.setAttribute("name", t.Name);
					addAttributeSetContent(toAdd, attrs, t);
					if (toAdd.getChildNodes().getLength() > 0)
					{
						ret.appendChild(toAdd);
					}
				}
			}
			return ret;
		}

		internal virtual Element fromOptions()
		{
			Element elt = doc.createElement("options");
			addAttributeSetContent(elt, file.Options.getAttributeSet(), null);
			return elt;
		}

		internal virtual Element fromMouseMappings()
		{
			Element elt = doc.createElement("mappings");
			MouseMappings map = file.Options.getMouseMappings();
			foreach (KeyValuePair<int, Tool> entry in map.Mappings.SetOfKeyValuePairs())
			{
				int? mods = entry.Key;
				Tool tool = entry.Value;
				Element toolElt = fromTool(tool);
				string mapValue = InputEventUtil.toString(mods.Value);
				toolElt.setAttribute("map", mapValue);
				elt.appendChild(toolElt);
			}
			return elt;
		}

		internal virtual Element fromToolbarData()
		{
			Element elt = doc.createElement("toolbar");
			ToolbarData toolbar = file.Options.getToolbarData();
			foreach (Tool tool in toolbar.Contents)
			{
				if (tool == null)
				{
					elt.appendChild(doc.createElement("sep"));
				}
				else
				{
					elt.appendChild(fromTool(tool));
				}
			}
			return elt;
		}

		internal virtual Element fromTool(Tool tool)
		{
			Library lib = findLibrary(tool);
			string lib_name;
			if (lib == null)
			{
				loader.showError(StringUtil.format("tool `%s' not found", tool.DisplayName));
				return null;
			}
			else if (lib == file)
			{
				lib_name = null;
			}
			else
			{
				lib_name = libs[lib];
				if (string.ReferenceEquals(lib_name, null))
				{
					loader.showError("unknown library within file");
					return null;
				}
			}

			Element elt = doc.createElement("tool");
			if (!string.ReferenceEquals(lib_name, null))
			{
				elt.setAttribute("lib", lib_name);
			}
			elt.setAttribute("name", tool.Name);
			addAttributeSetContent(elt, tool.AttributeSet, tool);
			return elt;
		}

		internal virtual Element fromCircuit(Circuit circuit)
		{
			Element ret = doc.createElement("circuit");
			ret.setAttribute("name", circuit.Name);
			addAttributeSetContent(ret, circuit.StaticAttributes, null);
			if (!circuit.Appearance.DefaultAppearance)
			{
				Element appear = doc.createElement("appear");
				foreach (object o in circuit.Appearance.ObjectsFromBottom)
				{
					if (o is AbstractCanvasObject)
					{
						Element elt = ((AbstractCanvasObject) o).toSvgElement(doc);
						if (elt != null)
						{
							appear.appendChild(elt);
						}
					}
				}
				ret.appendChild(appear);
			}
			foreach (Wire w in circuit.Wires)
			{
				ret.appendChild(fromWire(w));
			}
			foreach (Component comp in circuit.NonWires)
			{
				Element elt = fromComponent(comp);
				if (elt != null)
				{
					ret.appendChild(elt);
				}
			}
			return ret;
		}

		internal virtual Element fromComponent(Component comp)
		{
			ComponentFactory source = comp.Factory;
			Library lib = findLibrary(source);
			string lib_name;
			if (lib == null)
			{
				loader.showError(source.Name + " component not found");
				return null;
			}
			else if (lib == file)
			{
				lib_name = null;
			}
			else
			{
				lib_name = libs[lib];
				if (string.ReferenceEquals(lib_name, null))
				{
					loader.showError("unknown library within file");
					return null;
				}
			}

			Element ret = doc.createElement("comp");
			if (!string.ReferenceEquals(lib_name, null))
			{
				ret.setAttribute("lib", lib_name);
			}
			ret.setAttribute("name", source.Name);
			ret.setAttribute("loc", comp.Location.ToString());
			addAttributeSetContent(ret, comp.AttributeSet, comp.Factory);
			return ret;
		}

		internal virtual Element fromWire(Wire w)
		{
			Element ret = doc.createElement("wire");
			ret.setAttribute("from", w.End0.ToString());
			ret.setAttribute("to", w.End1.ToString());
			return ret;
		}

		internal virtual void addAttributeSetContent(Element elt, AttributeSet attrs, AttributeDefaultProvider source)
		{
			if (attrs == null)
			{
				return;
			}
			LogisimVersion ver = Main.VERSION;
			if (source != null && source.isAllDefaultValues(attrs, ver))
			{
				return;
			}
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attrBase : attrs.getAttributes())
			foreach (Attribute<object> attrBase in attrs.Attributes)
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<Object> attr = (logisim.data.Attribute<Object>) attrBase;
				Attribute<object> attr = (Attribute<object>) attrBase;
				object val = attrs.getValue(attr);
				if (attrs.isToSave(attr) && val != null)
				{
					object dflt = source == null ? null : source.getDefaultAttributeValue(attr, ver);
					if (dflt == null || !dflt.Equals(val))
					{
						Element a = doc.createElement("a");
						a.setAttribute("name", attr.Name);
						string value = attr.toStandardString(val);
						if (value.IndexOf("\n", StringComparison.Ordinal) >= 0)
						{
							a.appendChild(doc.createTextNode(value));
						}
						else
						{
							a.setAttribute("val", attr.toStandardString(val));
						}
						elt.appendChild(a);
					}
				}
			}
		}

		internal virtual Library findLibrary(Tool tool)
		{
			if (libraryContains(file, tool))
			{
				return file;
			}
			foreach (Library lib in file.Libraries)
			{
				if (libraryContains(lib, tool))
				{
					return lib;
				}
			}
			return null;
		}

		internal virtual Library findLibrary(ComponentFactory source)
		{
			if (file.contains(source))
			{
				return file;
			}
			foreach (Library lib in file.Libraries)
			{
				if (lib.contains(source))
				{
					return lib;
				}
			}
			return null;
		}

		internal virtual bool libraryContains(Library lib, Tool query)
		{
			foreach (Tool tool in lib.Tools)
			{
				if (tool.sharesSource(query))
				{
					return true;
				}
			}
			return false;
		}
	}

}
