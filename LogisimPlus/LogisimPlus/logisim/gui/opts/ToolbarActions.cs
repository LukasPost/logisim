// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.opts
{
	using ToolbarData = logisim.file.ToolbarData;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using Tool = logisim.tools.Tool;

	internal class ToolbarActions
	{
		private ToolbarActions()
		{
		}

		public static Action addTool(ToolbarData toolbar, Tool tool)
		{
			return new AddTool(toolbar, tool);
		}

		public static Action removeTool(ToolbarData toolbar, int pos)
		{
			return new RemoveTool(toolbar, pos);
		}

		public static Action moveTool(ToolbarData toolbar, int src, int dest)
		{
			return new MoveTool(toolbar, src, dest);
		}

		public static Action addSeparator(ToolbarData toolbar, int pos)
		{
			return new AddSeparator(toolbar, pos);
		}

		public static Action removeSeparator(ToolbarData toolbar, int pos)
		{
			return new RemoveSeparator(toolbar, pos);
		}

		private class AddTool : Action
		{
			internal ToolbarData toolbar;
			internal Tool tool;
			internal int pos;

			internal AddTool(ToolbarData toolbar, Tool tool)
			{
				this.toolbar = toolbar;
				this.tool = tool;
			}

			public override string Name
			{
				get
				{
					return Strings.get("toolbarAddAction");
				}
			}

			public override void doIt(Project proj)
			{
				pos = toolbar.Contents.Count;
				toolbar.addTool(pos, tool);
			}

			public override void undo(Project proj)
			{
				toolbar.remove(pos);
			}
		}

		private class RemoveTool : Action
		{
			internal ToolbarData toolbar;
			internal object removed;
			internal int which;

			internal RemoveTool(ToolbarData toolbar, int which)
			{
				this.toolbar = toolbar;
				this.which = which;
			}

			public override string Name
			{
				get
				{
					return Strings.get("toolbarRemoveAction");
				}
			}

			public override void doIt(Project proj)
			{
				removed = toolbar.remove(which);
			}

			public override void undo(Project proj)
			{
				if (removed is Tool)
				{
					toolbar.addTool(which, (Tool) removed);
				}
				else if (removed == null)
				{
					toolbar.addSeparator(which);
				}
			}
		}

		private class MoveTool : Action
		{
			internal ToolbarData toolbar;
			internal int oldpos;
			internal int dest;

			internal MoveTool(ToolbarData toolbar, int oldpos, int dest)
			{
				this.toolbar = toolbar;
				this.oldpos = oldpos;
				this.dest = dest;
			}

			public override string Name
			{
				get
				{
					return Strings.get("toolbarMoveAction");
				}
			}

			public override void doIt(Project proj)
			{
				toolbar.move(oldpos, dest);
			}

			public override void undo(Project proj)
			{
				toolbar.move(dest, oldpos);
			}

			public override bool shouldAppendTo(Action other)
			{
				if (other is MoveTool)
				{
					MoveTool o = (MoveTool) other;
					return this.toolbar == o.toolbar && o.dest == this.oldpos;
				}
				else
				{
					return false;
				}
			}

			public override Action append(Action other)
			{
				if (other is MoveTool)
				{
					MoveTool o = (MoveTool) other;
					if (this.toolbar == o.toolbar && this.dest == o.oldpos)
					{
						// TODO if (this.oldpos == o.dest) return null;
						return new MoveTool(toolbar, this.oldpos, o.dest);
					}
				}
				return base.append(other);
			}
		}

		private class AddSeparator : Action
		{
			internal ToolbarData toolbar;
			internal int pos;

			internal AddSeparator(ToolbarData toolbar, int pos)
			{
				this.toolbar = toolbar;
				this.pos = pos;
			}

			public override string Name
			{
				get
				{
					return Strings.get("toolbarInsertSepAction");
				}
			}

			public override void doIt(Project proj)
			{
				toolbar.addSeparator(pos);
			}

			public override void undo(Project proj)
			{
				toolbar.remove(pos);
			}
		}

		private class RemoveSeparator : Action
		{
			internal ToolbarData toolbar;
			internal int pos;

			internal RemoveSeparator(ToolbarData toolbar, int pos)
			{
				this.toolbar = toolbar;
				this.pos = pos;
			}

			public override string Name
			{
				get
				{
					return Strings.get("toolbarRemoveSepAction");
				}
			}

			public override void doIt(Project proj)
			{
				toolbar.remove(pos);
			}

			public override void undo(Project proj)
			{
				toolbar.addSeparator(pos);
			}
		}

	}

}
