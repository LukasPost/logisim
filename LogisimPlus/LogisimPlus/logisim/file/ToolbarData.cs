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

	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using AttributeSets = logisim.data.AttributeSets;
	using Tool = logisim.tools.Tool;
	using logisim.util;

	public class ToolbarData
	{
		public interface ToolbarListener
		{
			void toolbarChanged();
		}

		private EventSourceWeakSupport<ToolbarListener> listeners;
		private EventSourceWeakSupport<AttributeListener> toolListeners;
		private List<Tool> contents;

		public ToolbarData()
		{
			listeners = new EventSourceWeakSupport<ToolbarListener>();
			toolListeners = new EventSourceWeakSupport<AttributeListener>();
			contents = new List<Tool>();
		}

		//
		// listener methods
		//
		public virtual void addToolbarListener(ToolbarListener l)
		{
			listeners.add(l);
		}

		public virtual void removeToolbarListener(ToolbarListener l)
		{
			listeners.remove(l);
		}

		public virtual void addToolAttributeListener(AttributeListener l)
		{
			foreach (Tool tool in contents)
			{
				if (tool != null)
				{
					AttributeSet attrs = tool.AttributeSet;
					if (attrs != null)
					{
						attrs.addAttributeListener(l);
					}
				}
			}
			toolListeners.add(l);
		}

		public virtual void removeToolAttributeListener(AttributeListener l)
		{
			foreach (Tool tool in contents)
			{
				if (tool != null)
				{
					AttributeSet attrs = tool.AttributeSet;
					if (attrs != null)
					{
						attrs.removeAttributeListener(l);
					}
				}
			}
			toolListeners.remove(l);
		}

		private void addAttributeListeners(Tool tool)
		{
			foreach (AttributeListener l in toolListeners)
			{
				AttributeSet attrs = tool.AttributeSet;
				if (attrs != null)
				{
					attrs.addAttributeListener(l);
				}
			}
		}

		private void removeAttributeListeners(Tool tool)
		{
			foreach (AttributeListener l in toolListeners)
			{
				AttributeSet attrs = tool.AttributeSet;
				if (attrs != null)
				{
					attrs.removeAttributeListener(l);
				}
			}
		}

		public virtual void fireToolbarChanged()
		{
			foreach (ToolbarListener l in listeners)
			{
				l.toolbarChanged();
			}
		}

		//
		// query methods
		//
		public virtual List<Tool> Contents
		{
			get
			{
				return contents;
			}
		}

		public virtual Tool FirstTool
		{
			get
			{
				foreach (Tool tool in contents)
				{
					if (tool != null)
					{
						return tool;
					}
				}
				return null;
			}
		}

		public virtual int size()
		{
			return contents.Count;
		}

		public virtual Tool get(int index)
		{
			return contents[index];
		}

		//
		// modification methods
		//
		public virtual void copyFrom(ToolbarData other, LogisimFile file)
		{
			if (this == other)
			{
				return;
			}
			foreach (Tool tool in contents)
			{
				if (tool != null)
				{
					removeAttributeListeners(tool);
				}
			}
			this.contents.Clear();
			foreach (Tool srcTool in other.contents)
			{
				if (srcTool == null)
				{
					this.addSeparator();
				}
				else
				{
					Tool toolCopy = file.findTool(srcTool);
					if (toolCopy != null)
					{
						Tool dstTool = toolCopy.cloneTool();
						AttributeSets.copy(srcTool.AttributeSet, dstTool.AttributeSet);
						this.addTool(dstTool);
						addAttributeListeners(toolCopy);
					}
				}
			}
			fireToolbarChanged();
		}

		public virtual void addSeparator()
		{
			contents.Add(null);
			fireToolbarChanged();
		}

		public virtual void addTool(Tool tool)
		{
			contents.Add(tool);
			addAttributeListeners(tool);
			fireToolbarChanged();
		}

		public virtual void addTool(int pos, Tool tool)
		{
			contents.Insert(pos, tool);
			addAttributeListeners(tool);
			fireToolbarChanged();
		}

		public virtual void addSeparator(int pos)
		{
			contents.Insert(pos, null);
			fireToolbarChanged();
		}

		public virtual object move(int from, int to)
		{
			Tool moved = contents.RemoveAndReturn(from);
			contents.Insert(to, moved);
			fireToolbarChanged();
			return moved;
		}

		public virtual object remove(int pos)
		{
			object ret = contents.RemoveAndReturn(pos);
			if (ret is Tool)
			{
				removeAttributeListeners((Tool) ret);
			}
			fireToolbarChanged();
			return ret;
		}

		internal virtual bool usesToolFromSource(Tool query)
		{
			foreach (Tool tool in contents)
			{
				if (tool != null && tool.sharesSource(query))
				{
					return true;
				}
			}
			return false;
		}

		//
		// package-protected methods
		//
		internal virtual void replaceAll(Dictionary<Tool, Tool> toolMap)
		{
			bool changed = false;
// JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
			for (IEnumerator<Tool> it = contents.GetEnumerator(); it.MoveNext();)
			{
				object old = it.Current;
				if (toolMap.ContainsKey(old))
				{
					changed = true;
					removeAttributeListeners((Tool) old);
					Tool newTool = toolMap[old];
					if (newTool == null)
					{
// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
						it.remove();
					}
					else
					{
						Tool addedTool = newTool.cloneTool();
						addAttributeListeners(addedTool);
						LoadedLibrary.copyAttributes(addedTool.AttributeSet, ((Tool) old).AttributeSet);
						it.set(addedTool);
					}
				}
			}
			if (changed)
			{
				fireToolbarChanged();
			}
		}
	}

}
