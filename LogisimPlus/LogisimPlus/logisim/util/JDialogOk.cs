// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public abstract class JDialogOk : JDialog
	{
		private class MyListener : WindowAdapter, ActionListener
		{
			private readonly JDialogOk outerInstance;

			public MyListener(JDialogOk outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == outerInstance.ok)
				{
					outerInstance.okClicked();
					dispose();
				}
				else if (src == outerInstance.cancel)
				{
					outerInstance.cancelClicked();
					dispose();
				}
			}

			public override void windowClosing(WindowEvent e)
			{
				outerInstance.removeWindowListener(this);
				outerInstance.cancelClicked();
				dispose();
			}
		}

		private JPanel contents = new JPanel(new BorderLayout());
		protected internal JButton ok = new JButton(Strings.get("dlogOkButton"));
		protected internal JButton cancel = new JButton(Strings.get("dlogCancelButton"));

		public JDialogOk(Dialog parent, string title, bool model) : base(parent, title, true)
		{
			configure();
		}

		public JDialogOk(Frame parent, string title, bool model) : base(parent, title, true)
		{
			configure();
		}

		private void configure()
		{
			MyListener listener = new MyListener(this);
			this.addWindowListener(listener);
			ok.addActionListener(listener);
			cancel.addActionListener(listener);

			Box buttons = Box.createHorizontalBox();
			buttons.setBorder(BorderFactory.createEmptyBorder(10, 10, 10, 10));
			buttons.add(Box.createHorizontalGlue());
			buttons.add(ok);
			buttons.add(Box.createHorizontalStrut(10));
			buttons.add(cancel);
			buttons.add(Box.createHorizontalGlue());

			Container pane = base.getContentPane();
			pane.add(contents, BorderLayout.CENTER);
			pane.add(buttons, BorderLayout.SOUTH);
		}

		public override Container ContentPane
		{
			get
			{
				return contents;
			}
		}

		public abstract void okClicked();

		public virtual void cancelClicked()
		{
		}

	}

}
