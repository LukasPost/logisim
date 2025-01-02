// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.model
{

	public class HandleGesture
	{
		private Handle handle;
		private int dx;
		private int dy;
		private int modifiersEx;
		private Handle resultingHandle;

		public HandleGesture(Handle handle, int dx, int dy, int modifiersEx)
		{
			this.handle = handle;
			this.dx = dx;
			this.dy = dy;
			this.modifiersEx = modifiersEx;
		}

		public virtual Handle Handle
		{
			get
			{
				return handle;
			}
		}

		public virtual int DeltaX
		{
			get
			{
				return dx;
			}
		}

		public virtual int DeltaY
		{
			get
			{
				return dy;
			}
		}

		public virtual int ModifiersEx
		{
			get
			{
				return modifiersEx;
			}
		}

		public virtual bool ShiftDown
		{
			get
			{
				return (modifiersEx & InputEvent.SHIFT_DOWN_MASK) != 0;
			}
		}

		public virtual bool ControlDown
		{
			get
			{
				return (modifiersEx & InputEvent.CTRL_DOWN_MASK) != 0;
			}
		}

		public virtual bool AltDown
		{
			get
			{
				return (modifiersEx & InputEvent.ALT_DOWN_MASK) != 0;
			}
		}

		public virtual Handle ResultingHandle
		{
			set
			{
				resultingHandle = value;
			}
			get
			{
				return resultingHandle;
			}
		}

	}

}
