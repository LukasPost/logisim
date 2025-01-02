// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{
	using HexModel = hex.HexModel;
	using HexModelListener = hex.HexModelListener;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;

	internal class RomContentsListener : HexModelListener
	{
		private class Change : Action
		{
			internal RomContentsListener source;
			internal MemContents contents;
			internal long start;
			internal int[] oldValues;
			internal int[] newValues;
			internal bool completed = true;

			internal Change(RomContentsListener source, MemContents contents, long start, int[] oldValues, int[] newValues)
			{
				this.source = source;
				this.contents = contents;
				this.start = start;
				this.oldValues = oldValues;
				this.newValues = newValues;
			}

			public override string Name
			{
				get
				{
					return Strings.get("romChangeAction");
				}
			}

			public override void doIt(Project proj)
			{
				if (!completed)
				{
					completed = true;
					try
					{
						source.Enabled = false;
						contents.set(start, newValues);
					}
					finally
					{
						source.Enabled = true;
					}
				}
			}

			public override void undo(Project proj)
			{
				if (completed)
				{
					completed = false;
					try
					{
						source.Enabled = false;
						contents.set(start, oldValues);
					}
					finally
					{
						source.Enabled = true;
					}
				}
			}

			public override bool shouldAppendTo(Action other)
			{
				if (other is Change)
				{
					Change o = (Change) other;
					long oEnd = o.start + o.newValues.Length;
					long end = start + newValues.Length;
					if (oEnd >= start && end >= o.start)
					{
						return true;
					}
				}
				return base.shouldAppendTo(other);
			}

			public override Action append(Action other)
			{
				if (other is Change)
				{
					Change o = (Change) other;
					long oEnd = o.start + o.newValues.Length;
					long end = start + newValues.Length;
					if (oEnd >= start && end >= o.start)
					{
						long nStart = Math.Min(start, o.start);
						long nEnd = Math.Max(end, oEnd);
						int[] nOld = new int[(int)(nEnd - nStart)];
						int[] nNew = new int[(int)(nEnd - nStart)];
						Array.Copy(o.oldValues, 0, nOld, (int)(o.start - nStart), o.oldValues.Length);
						Array.Copy(oldValues, 0, nOld, (int)(start - nStart), oldValues.Length);
						Array.Copy(newValues, 0, nNew, (int)(start - nStart), newValues.Length);
						Array.Copy(o.newValues, 0, nNew, (int)(o.start - nStart), o.newValues.Length);
						return new Change(source, contents, nStart, nOld, nNew);
					}
				}
				return base.append(other);
			}
		}

		internal Project proj;
		internal bool enabled = true;

		internal RomContentsListener(Project proj)
		{
			this.proj = proj;
		}

		internal virtual bool Enabled
		{
			set
			{
				enabled = value;
			}
		}

		public virtual void metainfoChanged(HexModel source)
		{
			// ignore - this can only come from an already-registered
			// action
		}

		public virtual void bytesChanged(HexModel source, long start, long numBytes, int[] oldValues)
		{
			if (enabled && proj != null && oldValues != null)
			{
				// this change needs to be logged in the undo log
				int[] newValues = new int[oldValues.Length];
				for (int i = 0; i < newValues.Length; i++)
				{
					newValues[i] = source.get(start + i);
				}
				proj.doAction(new Change(this, (MemContents) source, start, oldValues, newValues));
			}
		}
	}

}
