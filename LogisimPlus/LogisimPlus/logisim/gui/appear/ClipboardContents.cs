// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;
using System.Linq;

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{

	using CanvasObject = draw.model.CanvasObject;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;

	internal class ClipboardContents
	{
		internal static readonly ClipboardContents EMPTY = new ClipboardContents(Enumerable.Empty<CanvasObject>(), null, null);

		private ICollection<CanvasObject> onClipboard;
		private Location anchorLocation;
		private Direction anchorFacing;

		public ClipboardContents(ICollection<CanvasObject> onClipboard, Location anchorLocation, Direction anchorFacing)
		{
			this.onClipboard = (new List<CanvasObject>(onClipboard)).AsReadOnly();
			this.anchorLocation = anchorLocation;
			this.anchorFacing = anchorFacing;
		}

		public virtual ICollection<CanvasObject> Elements
		{
			get
			{
				return onClipboard;
			}
		}

		public virtual Location AnchorLocation
		{
			get
			{
				return anchorLocation;
			}
		}

		public virtual Direction AnchorFacing
		{
			get
			{
				return anchorFacing;
			}
		}
	}

}
