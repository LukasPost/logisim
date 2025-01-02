/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.opts;

import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.file.MouseMappings;
import logisim.proj.Action;
import logisim.proj.Project;
import logisim.tools.Tool;
import logisim.util.StringUtil;

class OptionsActions<E> {
	private OptionsActions() {
	}

	public static <V> Action setAttribute(AttributeSet attrs, Attribute<V> attr, V value) {
		V oldValue = attrs.getValue(attr);
		if (!oldValue.equals(value)) {
			return new SetAction<>(attrs, attr, value);
		} else {
			return null;
		}
	}

	public static Action setMapping(MouseMappings mm, Integer mods, Tool tool) {
		return new SetMapping(mm, mods, tool);
	}

	public static Action removeMapping(MouseMappings mm, Integer mods) {
		return new RemoveMapping(mm, mods);
	}

	private static class SetAction<E> extends Action {
		private AttributeSet attrs;
		private Attribute<E> attr;
		private E newval;
		private E oldval;

		SetAction(AttributeSet attrs, Attribute<E> attr, E value) {
			this.attrs = attrs;
			this.attr = attr;
			this.newval = value;
		}

		@Override
		public String getName() {
			return StringUtil.format(Strings.get("setOptionAction"), attr.getDisplayName());
		}

		@Override
		public void doIt(Project proj) {
			oldval = attrs.getValue(attr);
			attrs.setValue(attr, newval);
		}

		@Override
		public void undo(Project proj) {
			attrs.setValue(attr, oldval);
		}
	}

	private static class SetMapping extends Action {
		MouseMappings mm;
		Integer mods;
		Tool oldtool;
		Tool tool;

		SetMapping(MouseMappings mm, Integer mods, Tool tool) {
			this.mm = mm;
			this.mods = mods;
			this.tool = tool;
		}

		@Override
		public String getName() {
			return Strings.get("addMouseMappingAction");
		}

		@Override
		public void doIt(Project proj) {
			oldtool = mm.getToolFor(mods);
			mm.setToolFor(mods, tool);
		}

		@Override
		public void undo(Project proj) {
			mm.setToolFor(mods, oldtool);
		}
	}

	private static class RemoveMapping extends Action {
		MouseMappings mm;
		Integer mods;
		Tool oldtool;

		RemoveMapping(MouseMappings mm, Integer mods) {
			this.mm = mm;
			this.mods = mods;
		}

		@Override
		public String getName() {
			return Strings.get("removeMouseMappingAction");
		}

		@Override
		public void doIt(Project proj) {
			oldtool = mm.getToolFor(mods);
			mm.setToolFor(mods, null);
		}

		@Override
		public void undo(Project proj) {
			mm.setToolFor(mods, oldtool);
		}
	}
}
