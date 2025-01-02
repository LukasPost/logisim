// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using Tool = logisim.tools.Tool;
	using KeyConfigurationEvent = logisim.tools.key.KeyConfigurationEvent;
	using KeyConfigurationResult = logisim.tools.key.KeyConfigurationResult;

	public class ToolAttributeAction : Action
	{
		public static Action create<T1>(Tool tool, Attribute<T1> attr, object value)
		{
			AttributeSet attrs = tool.AttributeSet;
			KeyConfigurationEvent e = new KeyConfigurationEvent(0, attrs, null, null);
			KeyConfigurationResult r = new KeyConfigurationResult(e, attr, value);
			return new ToolAttributeAction(r);
		}

		public static Action create(KeyConfigurationResult results)
		{
			return new ToolAttributeAction(results);
		}

		private KeyConfigurationResult config;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private java.util.Map<logisim.data.Attribute<?>, Object> oldValues;
		private IDictionary<Attribute<object>, object> oldValues;

		private ToolAttributeAction(KeyConfigurationResult config)
		{
			this.config = config;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: this.oldValues = new java.util.HashMap<logisim.data.Attribute<?>, Object>(2);
			this.oldValues = new Dictionary<Attribute<object>, object>(2);
		}

		public override string Name
		{
			get
			{
				return Strings.get("changeToolAttrAction");
			}
		}

		public override void doIt(Project proj)
		{
			AttributeSet attrs = config.Event.AttributeSet;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Map<logisim.data.Attribute<?>, Object> newValues = config.getAttributeValues();
			IDictionary<Attribute<object>, object> newValues = config.AttributeValues;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Map<logisim.data.Attribute<?>, Object> oldValues = new java.util.HashMap<logisim.data.Attribute<?>, Object>(newValues.size());
			IDictionary<Attribute<object>, object> oldValues = new Dictionary<Attribute<object>, object>(newValues.Count);
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (java.util.Map.Entry<logisim.data.Attribute<?>, Object> entry : newValues.entrySet())
			foreach (KeyValuePair<Attribute<object>, object> entry in newValues.SetOfKeyValuePairs())
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<Object> attr = (logisim.data.Attribute<Object>) entry.getKey();
				Attribute<object> attr = (Attribute<object>) entry.Key;
				oldValues[attr] = attrs.getValue(attr);
				attrs.setValue(attr, entry.Value);
			}
			this.oldValues = oldValues;
		}

		public override void undo(Project proj)
		{
			AttributeSet attrs = config.Event.AttributeSet;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Map<logisim.data.Attribute<?>, Object> oldValues = this.oldValues;
			IDictionary<Attribute<object>, object> oldValues = this.oldValues;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (java.util.Map.Entry<logisim.data.Attribute<?>, Object> entry : oldValues.entrySet())
			foreach (KeyValuePair<Attribute<object>, object> entry in oldValues.SetOfKeyValuePairs())
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<Object> attr = (logisim.data.Attribute<Object>) entry.getKey();
				Attribute<object> attr = (Attribute<object>) entry.Key;
				attrs.setValue(attr, entry.Value);
			}
		}

	}

}
