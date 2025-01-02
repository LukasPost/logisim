// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.comp
{

	using CircuitState = logisim.circuit.CircuitState;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using logisim.util;

	public abstract class ManagedComponent : AbstractComponent
	{
		private EventSourceWeakSupport<ComponentListener> listeners = new EventSourceWeakSupport<ComponentListener>();
		private Location loc;
		private AttributeSet attrs;
		private List<EndData> ends;
		private IList<EndData> endsView;
		private Bounds bounds = null;

		public ManagedComponent(Location loc, AttributeSet attrs, int num_ends)
		{
			this.loc = loc;
			this.attrs = attrs;
			this.ends = new List<EndData>(num_ends);
			this.endsView = ends.AsReadOnly();
		}

		//
		// abstract AbstractComponent methods
		//
		public override abstract ComponentFactory Factory {get;}

		public override void addComponentListener(ComponentListener l)
		{
			listeners.add(l);
		}

		public override void removeComponentListener(ComponentListener l)
		{
			listeners.remove(l);
		}

		protected internal virtual void fireEndChanged(ComponentEvent e)
		{
			ComponentEvent copy = null;
			foreach (ComponentListener l in listeners)
			{
				if (copy == null)
				{
					copy = new ComponentEvent(e.Source, Collections.singletonList(e.OldData), Collections.singletonList(e.Data));
				}
				l.endChanged(copy);
			}
		}

		protected internal virtual void fireEndsChanged(IList<EndData> oldEnds, IList<EndData> newEnds)
		{
			ComponentEvent e = null;
			foreach (ComponentListener l in listeners)
			{
				if (e == null)
				{
					e = new ComponentEvent(this, oldEnds, newEnds);
				}
				l.endChanged(e);
			}
		}

		protected internal virtual void fireComponentInvalidated(ComponentEvent e)
		{
			foreach (ComponentListener l in listeners)
			{
				l.componentInvalidated(e);
			}
		}

		public override Location Location
		{
			get
			{
				return loc;
			}
		}

		public override AttributeSet AttributeSet
		{
			get
			{
				return attrs;
			}
			set
			{
				attrs = value;
			}
		}

		public override Bounds Bounds
		{
			get
			{
				if (bounds == null)
				{
					Location loc = Location;
					Bounds offBounds = Factory.getOffsetBounds(AttributeSet);
					bounds = offBounds.translate(loc.X, loc.Y);
				}
				return bounds;
			}
			set
			{
				this.bounds = value;
			}
		}

		protected internal virtual void recomputeBounds()
		{
			bounds = null;
		}

		public override IList<EndData> Ends
		{
			get
			{
				return endsView;
			}
			set
			{
				IList<EndData> oldEnds = ends;
				int minLen = Math.Min(oldEnds.Count, value.Length);
				List<EndData> changesOld = new List<EndData>();
				List<EndData> changesNew = new List<EndData>();
				for (int i = 0; i < minLen; i++)
				{
					EndData old = oldEnds[i];
					if (value[i] != null && !value[i].Equals(old))
					{
						changesOld.Add(old);
						changesNew.Add(value[i]);
						oldEnds[i] = value[i];
					}
				}
				for (int i = oldEnds.Count - 1; i >= minLen; i--)
				{
					changesOld.Add(oldEnds.RemoveAndReturn(i));
					changesNew.Add(null);
				}
				for (int i = minLen; i < value.Length; i++)
				{
					oldEnds.Add(value[i]);
					changesOld.Add(null);
					changesNew.Add(value[i]);
				}
				fireEndsChanged(changesOld, changesNew);
			}
		}

		public virtual int EndCount
		{
			get
			{
				return ends.Count;
			}
		}

		public override abstract void propagate(CircuitState state);

		//
		// methods for altering data
		//
		public virtual void clearManager()
		{
			foreach (EndData end in ends)
			{
				fireEndChanged(new ComponentEvent(this, end, null));
			}
			ends.Clear();
			bounds = null;
		}



		public virtual void removeEnd(int index)
		{
			ends.RemoveAt(index);
		}

		public virtual void setEnd(int i, EndData data)
		{
			if (i == ends.Count)
			{
				ends.Add(data);
				fireEndChanged(new ComponentEvent(this, null, data));
			}
			else
			{
				EndData old = ends[i];
				if (old == null || !old.Equals(data))
				{
					ends[i] = data;
					fireEndChanged(new ComponentEvent(this, old, data));
				}
			}
		}

		public virtual void setEnd(int i, Location end, BitWidth width, int type)
		{
			setEnd(i, new EndData(end, width, type));
		}

		public virtual void setEnd(int i, Location end, BitWidth width, int type, bool exclusive)
		{
			setEnd(i, new EndData(end, width, type, exclusive));
		}


		public virtual Location getEndLocation(int i)
		{
			return getEnd(i).Location;
		}

		//
		// user interface methods
		//
		public override void expose(ComponentDrawContext context)
		{
			Bounds bounds = Bounds;
			java.awt.Component dest = context.Destination;
			if (bounds != null)
			{
				dest.repaint(bounds.X - 5, bounds.Y - 5, bounds.Width + 10, bounds.Height + 10);
			}
		}

		public override object getFeature(object key)
		{
			return null;
		}
	}

}
