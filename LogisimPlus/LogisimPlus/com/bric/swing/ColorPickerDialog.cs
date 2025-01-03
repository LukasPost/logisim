// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/*
* @(#)ColorPickerDialog.java  1.0  2008-03-01
*
* Copyright (c) 2008 Jeremy Wood
* E-mail: mickleness@gmail.com
* All rights reserved.
*
* The copyright of this software is owned by Jeremy Wood.
* You may not use, copy or modify this software, except in
* accordance with the license agreement you entered into with
* Jeremy Wood. For details see accompanying license terms.
*/
using LogisimPlus.Java;

namespace com.bric.swing
{

	/// <summary>
	/// This wraps a <code>ColorPicker</code> in a simple dialog with "OK" and "Cancel" options.
	/// <P>
	/// (This object is used by the static calls in <code>ColorPicker</code> to show a dialog.)
	/// 
	/// </summary>
	internal class ColorPickerDialog : JDialog
	{

		private const long serialVersionUID = 1L;

		internal ColorPicker cp;
		internal int alpha;
		internal JButton ok = new JButton(ColorPicker.strings.getObject("OK").ToString());
		internal JButton cancel = new JButton(ColorPicker.strings.getObject("Cancel").ToString());
		internal Color returnValue = null;
		internal ActionListener buttonListener = new ActionListenerAnonymousInnerClass();

		private class ActionListenerAnonymousInnerClass : ActionListener
		{
			public void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == outerInstance.ok)
				{
					outerInstance.returnValue = outerInstance.cp.getColor();
				}
				setVisible(false);
			}
		}

		public ColorPickerDialog(Frame owner, Color color, bool includeOpacity) : base(owner)
		{
			initialize(owner, color, includeOpacity);
		}

		public ColorPickerDialog(Dialog owner, Color color, bool includeOpacity) : base(owner)
		{
			initialize(owner, color, includeOpacity);
		}

		private void initialize(JComponent owner, Color color, bool includeOpacity)
		{
			cp = new ColorPicker(true, includeOpacity);
			setModal(true);
			setResizable(false);
			getContentPane().setLayout(new GridBagLayout());
			GridBagConstraints c = new GridBagConstraints();
			c.gridx = 0;
			c.gridy = 0;
			c.weightx = 1;
			c.weighty = 1;
			c.fill = GridBagConstraints.BOTH;
			c.gridwidth = GridBagConstraints.REMAINDER;
			c.insets = new Insets(10, 10, 10, 10);
			getContentPane().add(cp, c);
			c.gridy++;
			c.gridwidth = 1;
			getContentPane().add(new JPanel(), c);
			c.gridx++;
			c.weightx = 0;
			getContentPane().add(cancel, c);
			c.gridx++;
			c.weightx = 0;
			getContentPane().add(ok, c);
			cp.setRGB(color.R, color.G, color.B);
			cp.Opacity = ((float) color.A) / 255f;
			alpha = color.A;
			pack();
			setLocationRelativeTo(owner);

			ok.addActionListener(buttonListener);
			cancel.addActionListener(buttonListener);

			getRootPane().setDefaultButton(ok);
		}

		/// <returns> the color committed when the user clicked 'OK'. Note this returns <code>null</code> if the user canceled
		///         this dialog, or exited via the close decoration. </returns>
		public virtual Color Color
		{
			get
			{
				return returnValue;
			}
		}
	}

}
