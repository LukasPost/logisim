// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.model
{

	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;

	public interface CanvasObject
	{
		public abstract CanvasObject clone();

		public abstract string DisplayName {get;}

		public abstract AttributeSet AttributeSet {get;}

		public abstract V getValue<V>(Attribute<V> attr);

		public abstract Bounds Bounds {get;}

		public abstract bool matches(CanvasObject other);

		public abstract int matchesHashCode();

		public abstract bool contains(Location loc, bool assumeFilled);

		public abstract bool overlaps(CanvasObject other);

		public abstract IList<Handle> getHandles(HandleGesture gesture);

		public abstract bool canRemove();

		public abstract bool canMoveHandle(Handle handle);

		public abstract Handle canInsertHandle(Location desired);

		public abstract Handle canDeleteHandle(Location desired);

		public abstract void paint(Graphics g, HandleGesture gesture);

		Handle moveHandle(HandleGesture gesture);

		void insertHandle(Handle desired, Handle previous);

		Handle deleteHandle(Handle handle);

		void translate(int dx, int dy);

		void setValue<V>(Attribute<V> attr, V value);
	}

}
