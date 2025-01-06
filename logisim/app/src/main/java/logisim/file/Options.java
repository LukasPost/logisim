/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.AttributeSet;
import logisim.data.AttributeSets;
import logisim.data.Attributes;

public class Options {
	public static final AttributeOption GATE_UNDEFINED_IGNORE = new AttributeOption("ignore",
			Strings.getter("gateUndefinedIgnore"));
	public static final AttributeOption GATE_UNDEFINED_ERROR = new AttributeOption("error",
			Strings.getter("gateUndefinedError"));

	public static final Attribute<Integer> sim_limit_attr = Attributes.forInteger("simlimit",
			Strings.getter("simLimitOption"));
	public static final Attribute<Integer> sim_rand_attr = Attributes.forInteger("simrand",
			Strings.getter("simRandomOption"));
	public static final Attribute<AttributeOption> ATTR_GATE_UNDEFINED = Attributes.forOption("gateUndefined",
			Strings.getter("gateUndefinedOption"),
			new AttributeOption[] { GATE_UNDEFINED_IGNORE, GATE_UNDEFINED_ERROR });

	public static final Integer sim_rand_dflt = 32;

	private static final Attribute<?>[] ATTRIBUTES = { ATTR_GATE_UNDEFINED, sim_limit_attr, sim_rand_attr, };
	private static final Object[] DEFAULTS = { GATE_UNDEFINED_IGNORE, 1000, 0, };

	private AttributeSet attrs;
	private MouseMappings mmappings;
	private ToolbarData toolbar;

	public Options() {
		attrs = AttributeSets.fixedSet(ATTRIBUTES, DEFAULTS);
		mmappings = new MouseMappings();
		toolbar = new ToolbarData();
	}

	public AttributeSet getAttributeSet() {
		return attrs;
	}

	public MouseMappings getMouseMappings() {
		return mmappings;
	}

	public ToolbarData getToolbarData() {
		return toolbar;
	}

	public void copyFrom(Options other, LogisimFile dest) {
		AttributeSets.copy(other.attrs, attrs);
		toolbar.copyFrom(other.toolbar, dest);
		mmappings.copyFrom(other.mmappings, dest);
	}
}
