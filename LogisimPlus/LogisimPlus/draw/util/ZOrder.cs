// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.util
{

	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;

	public class ZOrder
	{
		private ZOrder()
		{
		}

		public static int getZIndex(CanvasObject query, CanvasModel model)
		{
			// returns 0 for bottommost element, large number for topmost
			return getIndex(query, model.ObjectsFromBottom);
		}

		public static IDictionary<CanvasObject, int> getZIndex<T1>(ICollection<T1> query, CanvasModel model) where T1 : draw.model.CanvasObject
		{
			// returns 0 for bottommost element, large number for topmost, ordered
			// from the bottom up.
			if (query == null)
			{
				return Collections.emptyMap();
			}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: java.util.Set<? extends draw.model.CanvasObject> querySet = toSet(query);
			ISet<CanvasObject> querySet = toSet(query);
			IDictionary<CanvasObject, int> ret;
			ret = new LinkedHashMap<CanvasObject, int>(query.Count);
			int z = -1;
			foreach (CanvasObject o in model.ObjectsFromBottom)
			{
				z++;
				if (querySet.Contains(o))
				{
					ret[o] = Convert.ToInt32(z);
				}
			}
			return ret;
		}

		public static IList<E> sortTopFirst<E>(ICollection<E> objects, CanvasModel model) where E : draw.model.CanvasObject
		{
			return sortXFirst(objects, model, model.ObjectsFromBottom);
		}

		public static IList<E> sortBottomFirst<E>(ICollection<E> objects, CanvasModel model) where E : draw.model.CanvasObject
		{
			return sortXFirst(objects, model, model.ObjectsFromTop);
		}

		private static IList<E> sortXFirst<E>(ICollection<E> objects, CanvasModel model, ICollection<CanvasObject> objs) where E : draw.model.CanvasObject
		{
			ISet<E> set = toSet(objects);
			List<E> ret = new List<E>(objects.Count);
			foreach (CanvasObject o in objs)
			{
				if (set.Contains(o))
				{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") E toAdd = (E) o;
					E toAdd = (E) o;
					ret.Add(toAdd);
				}
			}
			return ret;
		}

		private static ISet<E> toSet<E>(ICollection<E> objects)
		{
			if (objects is ISet<object>)
			{
				return (ISet<E>) objects;
			}
			else
			{
				return new HashSet<E>(objects);
			}
		}

		// returns first object above query in the z-order that overlaps query
		public static CanvasObject getObjectAbove<T1>(CanvasObject query, CanvasModel model, ICollection<T1> ignore) where T1 : draw.model.CanvasObject
		{
			return getPrevious(query, model.ObjectsFromTop, model, ignore);
		}

		// returns first object below query in the z-order that overlaps query
		public static CanvasObject getObjectBelow<T1>(CanvasObject query, CanvasModel model, ICollection<T1> ignore) where T1 : draw.model.CanvasObject
		{
			return getPrevious(query, model.ObjectsFromBottom, model, ignore);
		}

		private static CanvasObject getPrevious<T1>(CanvasObject query, IList<CanvasObject> objs, CanvasModel model, ICollection<T1> ignore) where T1 : draw.model.CanvasObject
		{
			int index = getIndex(query, objs);
			if (index <= 0)
			{
				return null;
			}
			else
			{
				ISet<CanvasObject> set = toSet(model.getObjectsOverlapping(query));
				IEnumerator<CanvasObject> it = objs.listIterator(index);
				while (it.hasPrevious())
				{
					CanvasObject o = it.previous();
					if (set.Contains(o) && !ignore.Contains(o))
					{
						return o;
					}
				}
				return null;
			}
		}

		private static int getIndex(CanvasObject query, IList<CanvasObject> objs)
		{
			int index = -1;
			foreach (CanvasObject o in objs)
			{
				index++;
				if (o == query)
				{
					return index;
				}
			}
			return -1;
		}

	}

}
