// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{
	using LFrame = logisim.gui.generic.LFrame;
	using About = logisim.gui.start.About;
	using MacCompatibility = logisim.util.MacCompatibility;



	internal class MenuHelp : JMenu, ActionListener
	{
		private LogisimMenuBar menubar;
		private JMenuItem tutorial = new JMenuItem();
		private JMenuItem guide = new JMenuItem();
		private JMenuItem library = new JMenuItem();
		private JMenuItem about = new JMenuItem();
		private HelpSet helpSet;
		private string helpSetUrl = "";
		private JHelp helpComponent;
		private LFrame helpFrame;

		public MenuHelp(LogisimMenuBar menubar)
		{
			this.menubar = menubar;

			tutorial.addActionListener(this);
			guide.addActionListener(this);
			library.addActionListener(this);
			about.addActionListener(this);

			add(tutorial);
			add(guide);
			add(library);
			if (!MacCompatibility.AboutAutomaticallyPresent)
			{
				addSeparator();
				add(about);
			}
		}

		public virtual void localeChanged()
		{
			this.setText(Strings.get("helpMenu"));
			if (helpFrame != null)
			{
				helpFrame.setTitle(Strings.get("helpWindowTitle"));
			}
			tutorial.setText(Strings.get("helpTutorialItem"));
			guide.setText(Strings.get("helpGuideItem"));
			library.setText(Strings.get("helpLibraryItem"));
			about.setText(Strings.get("helpAboutItem"));
			if (helpFrame != null)
			{
				helpFrame.setLocale(Locale.getDefault());
				loadBroker();
			}
		}

		public virtual void actionPerformed(ActionEvent e)
		{
			object src = e.getSource();
			if (src == guide)
			{
				showHelp("guide");
			}
			else if (src == tutorial)
			{
				showHelp("tutorial");
			}
			else if (src == library)
			{
				showHelp("libs");
			}
			else if (src == about)
			{
				About.showAboutDialog(menubar.ParentWindow);
			}
		}

		private void loadBroker()
		{
			string helpUrl = Strings.get("helpsetUrl");
			if (string.ReferenceEquals(helpUrl, null))
			{
				helpUrl = "doc/doc_en.hs";
			}
			if (helpSet == null || helpFrame == null || !helpUrl.Equals(helpSetUrl))
			{
				ClassLoader loader = typeof(MenuHelp).getClassLoader();
				try
				{
					URL hsURL = HelpSet.findHelpSet(loader, helpUrl);
					if (hsURL == null)
					{
						disableHelp();
						JOptionPane.showMessageDialog(menubar.ParentWindow, Strings.get("helpNotFoundError"));
						return;
					}
					helpSetUrl = helpUrl;
					helpSet = new HelpSet(null, hsURL);
					helpComponent = new JHelp(helpSet);
					if (helpFrame == null)
					{
						helpFrame = new LFrame();
						helpFrame.setTitle(Strings.get("helpWindowTitle"));
						helpFrame.setDefaultCloseOperation(JFrame.HIDE_ON_CLOSE);
						helpFrame.getContentPane().add(helpComponent);
						helpFrame.pack();
					}
					else
					{
						helpFrame.getContentPane().removeAll();
						helpFrame.getContentPane().add(helpComponent);
						helpComponent.revalidate();
					}
				}
				catch (Exception e)
				{
					disableHelp();
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					JOptionPane.showMessageDialog(menubar.ParentWindow, Strings.get("helpUnavailableError"));
					return;
				}
			}
		}

		private void showHelp(string target)
		{
			loadBroker();
			try
			{
				helpComponent.setCurrentID(target);
				helpFrame.toFront();
				helpFrame.setVisible(true);
			}
			catch (Exception e)
			{
				disableHelp();
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				JOptionPane.showMessageDialog(menubar.ParentWindow, Strings.get("helpDisplayError"));
			}
		}

		private void disableHelp()
		{
			guide.setEnabled(false);
			tutorial.setEnabled(false);
			library.setEnabled(false);
		}
	}

}
