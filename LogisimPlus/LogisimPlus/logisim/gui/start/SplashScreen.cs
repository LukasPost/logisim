// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.start
{


	public class SplashScreen : JWindow, ActionListener
	{
		public const int LIBRARIES = 0;
		public const int TEMPLATE_CREATE = 1;
		public const int TEMPLATE_OPEN = 2;
		public const int TEMPLATE_LOAD = 3;
		public const int TEMPLATE_CLOSE = 4;
		public const int GUI_INIT = 5;
		public const int FILE_CREATE = 6;
		public const int FILE_LOAD = 7;
		public const int PROJECT_CREATE = 8;
		public const int FRAME_CREATE = 9;

		private const int PROGRESS_MAX = 3568;
		private const bool PRINT_TIMES = false;

		private class Marker
		{
			internal int count;
			internal string message;

			internal Marker(int count, string message)
			{
				this.count = count;
				this.message = message;
			}
		}

		internal Marker[] markers = new Marker[]
		{
			new Marker(377, Strings.get("progressLibraries")),
			new Marker(990, Strings.get("progressTemplateCreate")),
			new Marker(1002, Strings.get("progressTemplateOpen")),
			new Marker(1002, Strings.get("progressTemplateLoad")),
			new Marker(1470, Strings.get("progressTemplateClose")),
			new Marker(1478, Strings.get("progressGuiInitialize")),
			new Marker(2114, Strings.get("progressFileCreate")),
			new Marker(2114, Strings.get("progressFileLoad")),
			new Marker(2383, Strings.get("progressProjectCreate")),
			new Marker(2519, Strings.get("progressFrameCreate"))
		};
		internal bool inClose = false; // for avoiding mutual recursion
		internal JProgressBar progress = new JProgressBar(0, PROGRESS_MAX);
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		internal JButton close_Conflict = new JButton(Strings.get("startupCloseButton"));
		internal JButton cancel = new JButton(Strings.get("startupQuitButton"));
		internal long startTime = DateTimeHelper.CurrentUnixTimeMillis();

		public SplashScreen()
		{
			JPanel imagePanel = About.ImagePanel;
			imagePanel.setBorder(null);

			progress.setStringPainted(true);

			JPanel buttonPanel = new JPanel();
			buttonPanel.add(close_Conflict);
			close_Conflict.addActionListener(this);
			buttonPanel.add(cancel);
			cancel.addActionListener(this);

			JPanel contents = new JPanel(new BorderLayout());
			contents.add(imagePanel, BorderLayout.NORTH);
			contents.add(progress, BorderLayout.CENTER);
			contents.add(buttonPanel, BorderLayout.SOUTH);
			contents.setBorder(BorderFactory.createLineBorder(Color.BLACK, 2));

			Color bg = imagePanel.getBackground();
			contents.setBackground(bg);
			buttonPanel.setBackground(bg);
			setBackground(bg);
			setContentPane(contents);
		}

		public virtual int Progress
		{
			set
			{
	// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	// ORIGINAL LINE: final Marker marker = markers == null ? null : markers[value];
				Marker marker = markers == null ? null : markers[value];
				if (marker != null)
				{
					SwingUtilities.invokeLater(() =>
					{
					progress.setString(marker.message);
					progress.setValue(marker.count);
					});
					if (PRINT_TIMES)
					{
						Console.Error.WriteLine((DateTimeHelper.CurrentUnixTimeMillis() - startTime) + " " + marker.message);
					}
				}
				else
				{
					if (PRINT_TIMES)
					{
						Console.Error.WriteLine((DateTimeHelper.CurrentUnixTimeMillis() - startTime) + " ??"); // OK
					}
				}
			}
		}

		public override bool Visible
		{
			set
			{
				if (value)
				{
					pack();
					Size dim = getToolkit().getScreenSize();
					int x = (int)(dim.getWidth() - getWidth()) / 2;
					int y = (int)(dim.getHeight() - getHeight()) / 2;
					setLocation(x, y);
				}
				base.setVisible(value);
			}
		}

		public virtual void close()
		{
			if (inClose)
			{
				return;
			}
			inClose = true;
			Visible = false;
			inClose = false;
			if (PRINT_TIMES)
			{
				Console.Error.WriteLine((DateTimeHelper.CurrentUnixTimeMillis() - startTime) + " closed");
			}
			markers = null;
		}

		public virtual void actionPerformed(ActionEvent e)
		{
			object src = e.getSource();
			if (src == cancel)
			{
				Environment.Exit(0);
			}
			else if (src == close_Conflict)
			{
				close();
			}
		}
	}

}
