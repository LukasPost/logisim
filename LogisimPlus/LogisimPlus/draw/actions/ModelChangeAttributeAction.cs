// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.actions
{

	using AttributeMapKey = draw.model.AttributeMapKey;
	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using logisim.data;

	public class ModelChangeAttributeAction : ModelAction
	{
		private Dictionary<AttributeMapKey, object> oldValues;
		private Dictionary<AttributeMapKey, object> newValues;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private logisim.data.Attribute<?> attr;
		private Attribute attr;

		public ModelChangeAttributeAction(CanvasModel model, Dictionary<AttributeMapKey, object> oldValues, Dictionary<AttributeMapKey, object> newValues) : base(model)
		{
			this.oldValues = oldValues;
			this.newValues = newValues;
		}

		public override ICollection<CanvasObject> Objects
		{
			get
			{
				HashSet<CanvasObject> ret = new HashSet<CanvasObject>();
				foreach (AttributeMapKey key in newValues.Keys)
				{
					ret.Add(key.Object);
				}
				return ret;
			}
		}

		public override string Name
		{
			get
			{
	// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	// ORIGINAL LINE: logisim.data.Attribute<?> a = attr;
				Attribute a = attr;
				if (a == null)
				{
					bool found = false;
					foreach (AttributeMapKey key in newValues.Keys)
					{
	// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	// ORIGINAL LINE: logisim.data.Attribute<?> at = key.getAttribute();
						Attribute at = key.Attribute;
						if (found)
						{
							if (a == null ? at != null :!a.Equals(at))
							{
								a = null;
								break;
							}
						}
						else
						{
							found = true;
							a = at;
						}
					}
					attr = a;
				}
				if (a == null)
				{
					return Strings.get("actionChangeAttributes");
				}
				else
				{
					return Strings.get("actionChangeAttribute", a.DisplayName);
				}
			}
		}

		internal override void doSub(CanvasModel model)
		{
			model.AttributeValues = newValues;
		}

		internal override void undoSub(CanvasModel model)
		{
			model.AttributeValues = oldValues;
		}
	}

}
