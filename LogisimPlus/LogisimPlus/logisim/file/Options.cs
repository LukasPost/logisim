// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using AttributeSets = logisim.data.AttributeSets;
	using Attributes = logisim.data.Attributes;

	public class Options
	{
		public static readonly AttributeOption GATE_UNDEFINED_IGNORE = new AttributeOption("ignore", Strings.getter("gateUndefinedIgnore"));
		public static readonly AttributeOption GATE_UNDEFINED_ERROR = new AttributeOption("error", Strings.getter("gateUndefinedError"));

		public static readonly Attribute<int> sim_limit_attr = Attributes.forInteger("simlimit", Strings.getter("simLimitOption"));
		public static readonly Attribute<int> sim_rand_attr = Attributes.forInteger("simrand", Strings.getter("simRandomOption"));
		public static readonly Attribute<AttributeOption> ATTR_GATE_UNDEFINED = Attributes.forOption("gateUndefined", Strings.getter("gateUndefinedOption"), new AttributeOption[] {GATE_UNDEFINED_IGNORE, GATE_UNDEFINED_ERROR});

		public static readonly int sim_rand_dflt = Convert.ToInt32(32);

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static final logisim.data.Attribute<?>[] ATTRIBUTES = { ATTR_GATE_UNDEFINED, sim_limit_attr, sim_rand_attr};
		private static readonly Attribute<object>[] ATTRIBUTES = new Attribute<object>[] {ATTR_GATE_UNDEFINED, sim_limit_attr, sim_rand_attr};
		private static readonly object[] DEFAULTS = new object[] {GATE_UNDEFINED_IGNORE, Convert.ToInt32(1000), Convert.ToInt32(0)};

		private AttributeSet attrs;
		private MouseMappings mmappings;
		private ToolbarData toolbar;

		public Options()
		{
			attrs = AttributeSets.fixedSet(ATTRIBUTES, DEFAULTS);
			mmappings = new MouseMappings();
			toolbar = new ToolbarData();
		}

		public virtual AttributeSet AttributeSet
		{
			get
			{
				return attrs;
			}
		}

		public virtual MouseMappings MouseMappings
		{
			get
			{
				return mmappings;
			}
		}

		public virtual ToolbarData ToolbarData
		{
			get
			{
				return toolbar;
			}
		}

		public virtual void copyFrom(Options other, LogisimFile dest)
		{
			AttributeSets.copy(other.attrs, this.attrs);
			this.toolbar.copyFrom(other.toolbar, dest);
			this.mmappings.copyFrom(other.mmappings, dest);
		}
	}

}
