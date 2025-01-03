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

	using CanvasModel = draw.model.CanvasModel;
	using CanvasObject = draw.model.CanvasObject;
	using ReorderRequest = draw.model.ReorderRequest;
	using ZOrder = draw.util.ZOrder;

	public class ModelReorderAction : ModelAction
	{
		public static ModelReorderAction createRaise<T1>(CanvasModel model, IEnumerable<T1> objs) where T1 : draw.model.CanvasObject
		{
			IEnumerable<CanvasObject> objects = objs.Cast<CanvasObject>();
			List<ReorderRequest> reqs = new List<ReorderRequest>();
			Dictionary<CanvasObject, int> zmap = ZOrder.getZIndex(objects, model);
			foreach (KeyValuePair<CanvasObject, int> entry in zmap.SetOfKeyValuePairs())
			{
				CanvasObject obj = entry.Key;
				int from = entry.Value.intValue();
				CanvasObject above = ZOrder.getObjectAbove(obj, model, objects);
				if (above != null)
				{
					int to = ZOrder.getZIndex(above, model);
					if (objects.Contains(above))
					{
						to--;
					}
					reqs.Add(new ReorderRequest(obj, from, to));
				}
			}
			if (reqs.Count == 0)
			{
				return null;
			}
			else
			{
				reqs.Sort(ReorderRequest.DESCENDING_FROM);
				repairRequests(reqs);
				return new ModelReorderAction(model, reqs);
			}
		}

		public static ModelReorderAction createLower<T1>(CanvasModel model, IEnumerable<T1> objs) where T1 : draw.model.CanvasObject
		{

            IEnumerable<CanvasObject> objects = objs.Cast<CanvasObject>();
            List<ReorderRequest> reqs = new List<ReorderRequest>();
			Dictionary<CanvasObject, int> zmap = ZOrder.getZIndex(objects, model);
			foreach (KeyValuePair<CanvasObject, int> entry in zmap.SetOfKeyValuePairs())
			{
				CanvasObject obj = entry.Key;
				int from = entry.Value.intValue();
				CanvasObject above = ZOrder.getObjectBelow(obj, model, objects);
				if (above != null)
				{
					int to = ZOrder.getZIndex(above, model);
					if (objects.Contains(above))
					{
						to++;
					}
					reqs.Add(new ReorderRequest(obj, from, to));
				}
			}
			if (reqs.Count == 0)
			{
				return null;
			}
			else
			{
				reqs.Sort(ReorderRequest.ASCENDING_FROM);
				repairRequests(reqs);
				return new ModelReorderAction(model, reqs);
			}
		}

		public static ModelReorderAction createRaHashSetop<T1>(CanvasModel model, IEnumerable<T1> objects) where T1 : draw.model.CanvasObject
		{
			List<ReorderRequest> reqs = new List<ReorderRequest>();
			Dictionary<CanvasObject, int> zmap = ZOrder.getZIndex(objects, model);
			int to = model.ObjectsFromBottom.Count - 1;
			foreach (KeyValuePair<CanvasObject, int> entry in zmap.SetOfKeyValuePairs())
			{
				CanvasObject obj = entry.Key;
				int from = entry.Value.intValue();
				reqs.Add(new ReorderRequest(obj, from, to));
			}
			if (reqs.Count == 0)
			{
				return null;
			}
			else
			{
				reqs.Sort(ReorderRequest.ASCENDING_FROM);
				repairRequests(reqs);
				return new ModelReorderAction(model, reqs);
			}
		}

		public static ModelReorderAction createLowerBottom<T1>(CanvasModel model, IEnumerable<T1> objects) where T1 : draw.model.CanvasObject
		{
			List<ReorderRequest> reqs = new List<ReorderRequest>();
			Dictionary<CanvasObject, int> zmap = ZOrder.getZIndex(objects, model);
			int to = 0;
			foreach (KeyValuePair<CanvasObject, int> entry in zmap.SetOfKeyValuePairs())
			{
				CanvasObject obj = entry.Key;
				int from = entry.Value.intValue();
				reqs.Add(new ReorderRequest(obj, from, to));
			}
			if (reqs.Count == 0)
			{
				return null;
			}
			else
			{
				reqs.Sort(ReorderRequest.ASCENDING_FROM);
				repairRequests(reqs);
				return new ModelReorderAction(model, reqs);
			}
		}

		private static void repairRequests(List<ReorderRequest> reqs)
		{
			for (int i = 0, n = reqs.Count; i < n; i++)
			{
				ReorderRequest req = reqs[i];
				int from = req.FromIndex;
				int to = req.ToIndex;
				for (int j = 0; j < i; j++)
				{
					ReorderRequest prev = reqs[j];
					int prevFrom = prev.FromIndex;
					int prevTo = prev.ToIndex;
					if (prevFrom <= from && from < prevTo)
					{
						from--;
					}
					else if (prevTo <= from && from < prevFrom)
					{
						from++;
					}
					if (prevFrom <= to && to < prevTo)
					{
						to--;
					}
					else if (prevTo <= to && to < prevFrom)
					{
						to++;
					}
				}
				if (from != req.FromIndex || to != req.ToIndex)
				{
					reqs[i] = new ReorderRequest(req.Object, from, to);
				}
			}
			for (int i = reqs.Count - 1; i >= 0; i--)
			{
				ReorderRequest req = reqs[i];
				if (req.FromIndex == req.ToIndex)
				{
					reqs.RemoveAt(i);
				}
			}
		}

		private List<ReorderRequest> requests;
		private List<CanvasObject> objects;
		private int type;

		public ModelReorderAction(CanvasModel model, List<ReorderRequest> requests) : base(model)
		{
			this.requests = new List<ReorderRequest>(requests);
			this.objects = new List<CanvasObject>(requests.Count);
			foreach (ReorderRequest r in requests)
			{
				objects.Add(r.Object);
			}
			int type = 0; // 0 = mixed/unknown, -1 = to greater index, 1 = to smaller index
			foreach (ReorderRequest r in requests)
			{
				int thisType;
				int from = r.FromIndex;
				int to = r.ToIndex;
				if (to < from)
				{
					thisType = -1;
				}
				else if (to > from)
				{
					thisType = 1;
				}
				else
				{
					thisType = 0;
				}
				if (type == 2)
				{
					type = thisType;
				}
				else if (type != thisType)
				{
					type = 0;
					break;
				}
			}
			this.type = type;
		}

		public virtual List<ReorderRequest> ReorderRequests
		{
			get
			{
				return requests.AsReadOnly();
			}
		}

		public override ICollection<CanvasObject> Objects
		{
			get
			{
				return objects;
			}
		}

		public override string Name
		{
			get
			{
				if (type < 0)
				{
					return Strings.get("actionRaise", getShapesName(objects));
				}
				else if (type > 0)
				{
					return Strings.get("actionLower", getShapesName(objects));
				}
				else
				{
					return Strings.get("actionReorder", getShapesName(objects));
				}
			}
		}

		internal override void doSub(CanvasModel model)
		{
			model.reorderObjects(requests);
		}

		internal override void undoSub(CanvasModel model)
		{
			List<ReorderRequest> inv = new List<ReorderRequest>(requests.Count);
			for (int i = requests.Count - 1; i >= 0; i--)
			{
				ReorderRequest r = requests[i];
				inv.Add(new ReorderRequest(r.Object, r.ToIndex, r.FromIndex));
			}
			model.reorderObjects(inv);
		}
	}

}
