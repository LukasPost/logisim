// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.opts
{
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using MouseMappings = logisim.file.MouseMappings;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using Tool = logisim.tools.Tool;
	using StringUtil = logisim.util.StringUtil;

	internal class OptionsActions<E>
	{
		private OptionsActions()
		{
		}

		public static Action setAttribute<V>(AttributeSet attrs, Attribute<V> attr, V value)
		{
			V oldValue = attrs.getValue(attr);
			if (!oldValue.Equals(value))
			{
				return new SetAction<>(attrs, attr, value);
			}
			else
			{
				return null;
			}
		}

		public static Action setMapping(MouseMappings mm, int? mods, Tool tool)
		{
			return new SetMapping(mm, mods, tool);
		}

		public static Action removeMapping(MouseMappings mm, int? mods)
		{
			return new RemoveMapping(mm, mods);
		}

		private class SetAction<E> : Action
		{
			internal AttributeSet attrs;
			internal Attribute<E> attr;
			internal E newval;
			internal E oldval;

			internal SetAction(AttributeSet attrs, Attribute<E> attr, E value)
			{
				this.attrs = attrs;
				this.attr = attr;
				this.newval = value;
			}

			public override string Name
			{
				get
				{
					return StringUtil.format(Strings.get("setOptionAction"), attr.DisplayName);
				}
			}

			public override void doIt(Project proj)
			{
				oldval = attrs.getValue(attr);
				attrs.setValue(attr, newval);
			}

			public override void undo(Project proj)
			{
				attrs.setValue(attr, oldval);
			}
		}

		private class SetMapping : Action
		{
			internal MouseMappings mm;
			internal int? mods;
			internal Tool oldtool;
			internal Tool tool;

			internal SetMapping(MouseMappings mm, int? mods, Tool tool)
			{
				this.mm = mm;
				this.mods = mods;
				this.tool = tool;
			}

			public override string Name
			{
				get
				{
					return Strings.get("addMouseMappingAction");
				}
			}

			public override void doIt(Project proj)
			{
				oldtool = mm.getToolFor(mods.Value);
				mm.setToolFor(mods.Value, tool);
			}

			public override void undo(Project proj)
			{
				mm.setToolFor(mods.Value, oldtool);
			}
		}

		private class RemoveMapping : Action
		{
			internal MouseMappings mm;
			internal int? mods;
			internal Tool oldtool;

			internal RemoveMapping(MouseMappings mm, int? mods)
			{
				this.mm = mm;
				this.mods = mods;
			}

			public override string Name
			{
				get
				{
					return Strings.get("removeMouseMappingAction");
				}
			}

			public override void doIt(Project proj)
			{
				oldtool = mm.getToolFor(mods.Value);
				mm.setToolFor(mods.Value, null);
			}

			public override void undo(Project proj)
			{
				mm.setToolFor(mods.Value, oldtool);
			}
		}
	}

}
