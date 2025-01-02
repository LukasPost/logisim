// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.canvas
{

	using CanvasObject = draw.model.CanvasObject;

	public class SelectionEvent : EventObject
	{
		public const int ACTION_ADDED = 0;
		public const int ACTION_REMOVED = 1;
		public const int ACTION_HANDLE = 2;

		private int action;
		private ICollection<CanvasObject> affected;

		public SelectionEvent(Selection source, int action, ICollection<CanvasObject> affected) : base(source)
		{
			this.action = action;
			this.affected = affected;
		}

		public virtual Selection Selection
		{
			get
			{
				return (Selection) getSource();
			}
		}

		public virtual int Action
		{
			get
			{
				return action;
			}
		}

		public virtual ICollection<CanvasObject> Affected
		{
			get
			{
				return affected;
			}
		}
	}

}
