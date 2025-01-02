// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{
	using Library = logisim.tools.Library;

	public class LibraryEvent
	{
		public const int ADD_TOOL = 0;
		public const int REMOVE_TOOL = 1;
		public const int MOVE_TOOL = 2;
		public const int ADD_LIBRARY = 3;
		public const int REMOVE_LIBRARY = 4;
		public const int SET_MAIN = 5;
		public const int SET_NAME = 6;
		public const int DIRTY_STATE = 7;

		private Library source;
		private int action;
		private object data;

		internal LibraryEvent(Library source, int action, object data)
		{
			this.source = source;
			this.action = action;
			this.data = data;
		}

		public virtual Library Source
		{
			get
			{
				return source;
			}
		}

		public virtual int Action
		{
			get
			{
				return action;
			}
		}

		public virtual object Data
		{
			get
			{
				return data;
			}
		}

	}

}
