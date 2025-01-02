// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;
using System.Linq;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using Component = logisim.comp.Component;

	internal class SelectionSave
	{
		public static SelectionSave create(Selection sel)
		{
			SelectionSave save = new SelectionSave();

			ICollection<Component> lifted = sel.FloatingComponents;
			if (lifted.Count > 0)
			{
				save.floating = lifted.ToArray();
			}

			ICollection<Component> selected = sel.AnchoredComponents;
			if (selected.Count > 0)
			{
				save.anchored = selected.ToArray();
			}

			return save;
		}

		private Component[] floating;
		private Component[] anchored;

		private SelectionSave()
		{
		}

		public virtual Component[] FloatingComponents
		{
			get
			{
				return floating;
			}
		}

		public virtual Component[] AnchoredComponents
		{
			get
			{
				return anchored;
			}
		}

		public virtual bool isSame(Selection sel)
		{
			return isSame(floating, sel.FloatingComponents) && isSame(anchored, sel.AnchoredComponents);
		}

		public override bool Equals(object other)
		{
			if (other is SelectionSave)
			{
				SelectionSave o = (SelectionSave) other;
				return isSame(this.floating, o.floating) && isSame(this.anchored, o.anchored);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			int ret = 0;
			if (floating != null)
			{
				foreach (Component c in floating)
				{
					ret += c.GetHashCode();
				}
			}
			if (anchored != null)
			{
				foreach (Component c in anchored)
				{
					ret += c.GetHashCode();
				}
			}
			return ret;
		}

		private static bool isSame(Component[] save, ICollection<Component> sel)
		{
			if (save == null)
			{
				return sel.Count == 0;
			}
			else
			{
				return toSet(save).Equals(sel);
			}
		}

		private static bool isSame(Component[] a, Component[] b)
		{
			if (a == null || a.Length == 0)
			{
				return b == null || b.Length == 0;
			}
			else if (b == null || b.Length == 0)
			{
				return false;
			}
			else if (a.Length != b.Length)
			{
				return false;
			}
			else
			{
				return toSet(a).Equals(toSet(b));
			}
		}

		private static HashSet<Component> toSet(Component[] comps)
		{
			HashSet<Component> ret = new HashSet<Component>(comps.Length);
			foreach (Component c in comps)
			{
				ret.Add(c);
			}
			return ret;
		}
	}

}
