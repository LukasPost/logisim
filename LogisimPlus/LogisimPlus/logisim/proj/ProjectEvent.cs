// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.proj
{
	using Circuit = logisim.circuit.Circuit;
	using LogisimFile = logisim.file.LogisimFile;
	using Tool = logisim.tools.Tool;

	public class ProjectEvent
	{
		public const int ACTION_SET_FILE = 0; // change file
		public const int ACTION_SET_CURRENT = 1; // change current
		public const int ACTION_SET_TOOL = 2; // change tool
		public const int ACTION_SELECTION = 3; // selection alterd
		public const int ACTION_SET_STATE = 4; // circuit state changed
		public const int ACTION_START = 5; // action about to start
		public const int ACTION_COMPLETE = 6; // action has completed
		public const int ACTION_MERGE = 7; // one action has been appended to another
		public const int UNDO_START = 8; // undo about to start
		public const int UNDO_COMPLETE = 9; // undo has completed
		public const int REPAINT_REQUEST = 10; // canvas should be repainted

		private int action;
		private Project proj;
		private object old_data;
		private object data;

		internal ProjectEvent(int action, Project proj, object old, object data)
		{
			this.action = action;
			this.proj = proj;
			this.old_data = old;
			this.data = data;
		}

		internal ProjectEvent(int action, Project proj, object data)
		{
			this.action = action;
			this.proj = proj;
			this.data = data;
		}

		internal ProjectEvent(int action, Project proj)
		{
			this.action = action;
			this.proj = proj;
			this.data = null;
		}

		// access methods
		public virtual int Action
		{
			get
			{
				return action;
			}
		}

		public virtual Project Project
		{
			get
			{
				return proj;
			}
		}

		public virtual object OldData
		{
			get
			{
				return old_data;
			}
		}

		public virtual object Data
		{
			get
			{
				return data;
			}
		}

		// convenience methods
		public virtual LogisimFile LogisimFile
		{
			get
			{
				return proj.LogisimFile;
			}
		}

		public virtual Circuit Circuit
		{
			get
			{
				return proj.CurrentCircuit;
			}
		}

		public virtual Tool Tool
		{
			get
			{
				return proj.Tool;
			}
		}

	}

}
