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

	internal class DrawingOverlaps
	{
		private Dictionary<CanvasObject, List<CanvasObject>> map;
		private HashSet<CanvasObject> untested;

		public DrawingOverlaps()
		{
			map = new Dictionary<CanvasObject, List<CanvasObject>>();
			untested = new HashSet<CanvasObject>();
		}

		public virtual ICollection<CanvasObject> getObjectsOverlapping(CanvasObject o)
		{
			ensureUpdated();

			List<CanvasObject> ret = map[o];
			if (ret == null || ret.Count == 0)
			{
				return [];
			}
			else
			{
				return ret.AsReadOnly();
			}
		}

		private void ensureUpdated()
		{
			foreach (CanvasObject o in untested)
			{
				List<CanvasObject> over = new List<CanvasObject>();
				foreach (CanvasObject o2 in map.Keys)
				{
					if (o != o2 && o.overlaps(o2))
					{
						over.Add(o2);
						addOverlap(o2, o);
					}
				}
				map[o] = over;
			}
			untested.Clear();
		}

		private void addOverlap(CanvasObject a, CanvasObject b)
		{
			List<CanvasObject> alist = map[a];
			if (alist == null)
			{
				alist = new List<CanvasObject>();
				map[a] = alist;
			}
			if (!alist.Contains(b))
			{
				alist.Add(b);
			}
		}

		public virtual void addShape(CanvasObject shape)
		{
			untested.Add(shape);
		}

		public virtual void removeShape(CanvasObject shape)
		{
			untested.remove(shape);
			List<CanvasObject> mapped = map.Remove(shape);
			if (mapped != null)
			{
				foreach (CanvasObject o in mapped)
				{
					List<CanvasObject> reverse = map[o];
					if (reverse != null)
					{
						reverse.Remove(shape);
					}
				}
			}
		}

		public virtual void invalidateShape(CanvasObject shape)
		{
			removeShape(shape);
			untested.Add(shape);
		}

		public virtual void invalidateShapes<T1>(ICollection<T1> shapes) where T1 : CanvasObject
		{
			foreach (CanvasObject o in shapes)
			{
				invalidateShape(o);
			}
		}
	}

}
