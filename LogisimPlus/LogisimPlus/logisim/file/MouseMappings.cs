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

	using ComponentFactory = logisim.comp.ComponentFactory;
	using AttributeSets = logisim.data.AttributeSets;
	using AddTool = logisim.tools.AddTool;
	using SelectTool = logisim.tools.SelectTool;
	using Tool = logisim.tools.Tool;

	public class MouseMappings
	{
		public interface MouseMappingsListener
		{
			void mouseMappingsChanged();
		}

		private List<MouseMappingsListener> listeners;
		private Dictionary<int, Tool> map;
		private int cache_mods;
		private Tool cache_tool;

		public MouseMappings()
		{
			listeners = new List<MouseMappingsListener>();
			map = new Dictionary<int, Tool>();
		}

		//
		// listener methods
		//
		public virtual void addMouseMappingsListener(MouseMappingsListener l)
		{
			listeners.Add(l);
		}

		public virtual void removeMouseMappingsListener(MouseMappingsListener l)
		{
			listeners.Add(l);
		}

		private void fireMouseMappingsChanged()
		{
			foreach (MouseMappingsListener l in listeners)
			{
				l.mouseMappingsChanged();
			}
		}

		//
		// query methods
		//
		public virtual IDictionary<int, Tool> Mappings
		{
			get
			{
				return map;
			}
		}

		public virtual ISet<int> MappedModifiers
		{
			get
			{
				return map.Keys;
			}
		}

		public virtual Tool getToolFor(MouseEvent e)
		{
			return getToolFor(e.getModifiersEx());
		}

		public virtual Tool getToolFor(int mods)
		{
			if (mods == cache_mods)
			{
				return cache_tool;
			}
			else
			{
				Tool ret = map[Convert.ToInt32(mods)];
				cache_mods = mods;
				cache_tool = ret;
				return ret;
			}
		}

		public virtual Tool getToolFor(int? mods)
		{
			if (mods.Value == cache_mods)
			{
				return cache_tool;
			}
			else
			{
				Tool ret = map[mods];
				cache_mods = mods.Value;
				cache_tool = ret;
				return ret;
			}
		}

		public virtual bool usesToolFromSource(Tool query)
		{
			foreach (Tool tool in map.Values)
			{
				if (tool.sharesSource(query))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool containsSelectTool()
		{
			foreach (Tool tool in map.Values)
			{
				if (tool is SelectTool)
				{
					return true;
				}
			}
			return false;
		}

		//
		// modification methods
		//
		public virtual void copyFrom(MouseMappings other, LogisimFile file)
		{
			if (this == other)
			{
				return;
			}
			cache_mods = -1;
			this.map.Clear();
			foreach (int? mods in other.map.Keys)
			{
				Tool srcTool = other.map[mods];
				Tool dstTool = file.findTool(srcTool);
				if (dstTool != null)
				{
					dstTool = dstTool.cloneTool();
					AttributeSets.copy(srcTool.AttributeSet, dstTool.AttributeSet);
					this.map[mods] = dstTool;
				}
			}
			fireMouseMappingsChanged();
		}

		public virtual void setToolFor(MouseEvent e, Tool tool)
		{
			setToolFor(e.getModifiersEx(), tool);
		}

		public virtual void setToolFor(int mods, Tool tool)
		{
			if (mods == cache_mods)
			{
				cache_mods = -1;
			}

			if (tool == null)
			{
				object old = map.Remove(Convert.ToInt32(mods));
				if (old != null)
				{
					fireMouseMappingsChanged();
				}
			}
			else
			{
				object old = map[Convert.ToInt32(mods)] = tool;
				if (old != tool)
				{
					fireMouseMappingsChanged();
				}
			}
		}

		public virtual void setToolFor(int? mods, Tool tool)
		{
			if (mods.Value == cache_mods)
			{
				cache_mods = -1;
			}

			if (tool == null)
			{
				object old = map.Remove(mods);
				if (old != null)
				{
					fireMouseMappingsChanged();
				}
			}
			else
			{
				object old = map[mods] = tool;
				if (old != tool)
				{
					fireMouseMappingsChanged();
				}
			}
		}

		//
		// package-protected methods
		//
		internal virtual void replaceAll(IDictionary<Tool, Tool> toolMap)
		{
			bool changed = false;
			foreach (KeyValuePair<int, Tool> entry in map.SetOfKeyValuePairs())
			{
				int? key = entry.Key;
				Tool tool = entry.Value;
				if (tool is AddTool at)
				{
					ComponentFactory factory = at.Factory;
					if (toolMap.ContainsKey(factory))
					{
						changed = true;
						Tool newTool = toolMap[factory];
						if (newTool == null)
						{
							map.Remove(key);
						}
						else
						{
							Tool clone = newTool.cloneTool();
							LoadedLibrary.copyAttributes(clone.AttributeSet, tool.AttributeSet);
							map[key] = clone;
						}
					}
				}
				else
				{
					if (toolMap.ContainsKey(tool))
					{
						changed = true;
						Tool newTool = toolMap[tool];
						if (newTool == null)
						{
							map.Remove(key);
						}
						else
						{
							Tool clone = newTool.cloneTool();
							LoadedLibrary.copyAttributes(clone.AttributeSet, tool.AttributeSet);
							map[key] = clone;
						}
					}
				}
			}
			if (changed)
			{
				fireMouseMappingsChanged();
			}
		}
	}

}
