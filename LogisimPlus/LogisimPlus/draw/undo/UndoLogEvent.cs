// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.undo
{

	public class UndoLogEvent : EventObject
	{
		public const int ACTION_DONE = 0;
		public const int ACTION_UNDONE = 1;

		private int action;
		private Action actionObject;

		public UndoLogEvent(UndoLog source, int action, Action actionObject) : base(source)
		{
			this.action = action;
			this.actionObject = actionObject;
		}

		public virtual UndoLog UndoLog
		{
			get
			{
				return (UndoLog) getSource();
			}
		}

		public virtual int Action
		{
			get
			{
				return action;
			}
		}

		public virtual Action ActionObject
		{
			get
			{
				return actionObject;
			}
		}
	}

}
