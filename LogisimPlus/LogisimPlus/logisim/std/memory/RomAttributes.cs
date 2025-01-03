// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using AbstractAttributeSet = logisim.data.AbstractAttributeSet;
	using logisim.data;
	using BitWidth = logisim.data.BitWidth;
	using HexFrame = logisim.gui.hex.HexFrame;
	using Project = logisim.proj.Project;

	internal class RomAttributes : AbstractAttributeSet
	{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static java.util.List<logisim.data.Attribute<?>> ATTRIBUTES = java.util.Arrays.asList(new logisim.data.Attribute<?>[] { Mem.ADDR_ATTR, Mem.DATA_ATTR, Rom.CONTENTS_ATTR });
		private static List<Attribute> ATTRIBUTES = new List<Attribute> {Mem.ADDR_ATTR, Mem.DATA_ATTR, Rom.CONTENTS_ATTR};

		private static WeakHashMap<MemContents, RomContentsListener> listenerRegistry = new WeakHashMap<MemContents, RomContentsListener>();
		private static WeakHashMap<MemContents, HexFrame> windowRegistry = new WeakHashMap<MemContents, HexFrame>();

		internal static void register(MemContents value, Project proj)
		{
			if (proj == null || listenerRegistry.containsKey(value))
			{
				return;
			}
			RomContentsListener l = new RomContentsListener(proj);
			value.addHexModelListener(l);
			listenerRegistry.put(value, l);
		}

		internal static HexFrame getHexFrame(MemContents value, Project proj)
		{
			lock (windowRegistry)
			{
				HexFrame ret = windowRegistry.get(value);
				if (ret == null)
				{
					ret = new HexFrame(proj, value);
					windowRegistry.put(value, ret);
				}
				return ret;
			}
		}

		private BitWidth addrBits = BitWidth.create(8);
		private BitWidth dataBits = BitWidth.create(8);
		private MemContents contents;

		internal RomAttributes()
		{
			contents = MemContents.create(addrBits.Width, dataBits.Width);
		}

		internal virtual Project Project
		{
			set
			{
				register(contents, value);
			}
		}

		protected internal override void copyInto(AbstractAttributeSet dest)
		{
			RomAttributes d = (RomAttributes) dest;
			d.addrBits = addrBits;
			d.dataBits = dataBits;
			d.contents = contents.clone();
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return ATTRIBUTES;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public <V> V getValue(logisim.data.Attribute<V> attr)
		public override object getValue(Attribute attr)
		{
			if (attr == Mem.ADDR_ATTR)
			{
				return addrBits;
			}
			if (attr == Mem.DATA_ATTR)
			{
				return dataBits;
			}
			if (attr == Rom.CONTENTS_ATTR)
			{
				return contents;
			}
			return null;
		}

		public override void setValue(Attribute attr, object value)
		{
			if (attr == Mem.ADDR_ATTR)
			{
				addrBits = (BitWidth) value;
				contents.setSizes(addrBits.Width, dataBits.Width);
			}
			else if (attr == Mem.DATA_ATTR)
			{
				dataBits = (BitWidth) value;
				contents.setSizes(addrBits.Width, dataBits.Width);
			}
			else if (attr == Rom.CONTENTS_ATTR)
			{
				contents = (MemContents) value;
			}
			fireAttributeValueChanged(attr, value);
		}
	}

}
