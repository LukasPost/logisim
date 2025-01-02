// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{


	using WindowClosable = logisim.util.WindowClosable;

	public class LFrame : JFrame, WindowClosable
	{
		private const string PATH = "logisim/img/logisim-icon-";
		private static readonly int[] SIZES = new int[] {16, 20, 24, 48, 64, 128};
		private static IList<Image> ICONS = null;
		private const int DEFAULT_SIZE = 48;
		private static Image DEFAULT_ICON = null;

		public static void attachIcon(Window frame)
		{
			if (ICONS == null)
			{
				IList<Image> loadedIcons = new List<Image>();
				ClassLoader loader = typeof(LFrame).getClassLoader();
				foreach (int size in SIZES)
				{
					URL url = loader.getResource(PATH + size + ".png");
					if (url != null)
					{
						ImageIcon icon = new ImageIcon(url);
						loadedIcons.Add(icon.getImage());
						if (size == DEFAULT_SIZE)
						{
							DEFAULT_ICON = icon.getImage();
						}
					}
				}
				ICONS = loadedIcons;
			}

			bool success = false;
			try
			{
				if (ICONS != null && ICONS.Count > 0)
				{
					System.Reflection.MethodInfo set = frame.GetType().GetMethod("setIconImages", typeof(System.Collections.IList));
					set.invoke(frame, ICONS);
					success = true;
				}
			}
			catch (Exception)
			{
			}

			if (!success && frame is JFrame && DEFAULT_ICON != null)
			{
				((JFrame) frame).setIconImage(DEFAULT_ICON);
			}
		}

		public LFrame()
		{
			LFrame.attachIcon(this);
		}

		public virtual void requestClose()
		{
			WindowEvent closing = new WindowEvent(this, WindowEvent.WINDOW_CLOSING);
			processWindowEvent(closing);
		}
	}

}
