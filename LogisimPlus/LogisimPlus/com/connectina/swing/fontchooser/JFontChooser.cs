// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * A font chooser JavaBean component.
 * Copyright (C) 2009 Dr Christos Bohoris
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License version 3 as published by the Free Software Foundation;
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 *
 * swing@connectina.com
 */
namespace com.connectina.swing.fontchooser
{

	/// <summary>
	/// Provides a pane of controls designed to allow a user to select a <code>Font</code>.
	/// 
	/// @author Christos Bohoris </summary>
	/// <seealso cref="java.awt.Font"/>
	public class JFontChooser : JPanel
	{
		internal const string SANS_SERIF = "SansSerif";
		private const long serialVersionUID = 5157499702004637097L;
		private static readonly Dictionary<Locale, ResourceBundle> bundles = new Dictionary<Locale, ResourceBundle>();

		private static ResourceBundle Bundle
		{
			get
			{
				Locale loc = Locale.getDefault();
				ResourceBundle ret = bundles[loc];
				if (ret == null)
				{
					ret = ResourceBundle.getBundle("connectina/JFontChooser");
					bundles[loc] = ret;
				}
				return ret;
			}
		}

		private FontSelectionModel selectionModel;
		/// <summary>
		/// The selection model property name.
		/// </summary>
		public const string SELECTION_MODEL_PROPERTY = "selectionModel";

		/// <summary>
		/// Creates a FontChooser pane with an initial default Font (Sans Serif, Plain, 12).
		/// </summary>
		public JFontChooser() : this(new Font(SANS_SERIF, Font.PLAIN, 12))
		{
		}

		/// <summary>
		/// Creates a FontChooser pane with the specified initial Font.
		/// </summary>
		/// <param name="initialFont"> the initial Font set in the chooser </param>
		public JFontChooser(Font initialFont) : this(new DefaultFontSelectionModel(initialFont))
		{
		}

		/// <summary>
		/// Creates a FontChooser pane with the specified <code>FontSelectionModel</code>.
		/// </summary>
		/// <param name="model"> the <code>FontSelectionModel</code> to be used </param>
		public JFontChooser(FontSelectionModel model)
		{
			this.selectionModel = model;
			ResourceBundle bundle = Bundle;
			initComponents(bundle);
			initPanel(bundle);
		}

		/// <summary>
		/// Gets the current Font value from the FontChooser. By default, this delegates to the model.
		/// </summary>
		/// <returns> the current Font value of the FontChooser </returns>
		public virtual Font SelectedFont
		{
			get
			{
				return selectionModel.SelectedFont;
			}
			set
			{
				selectionModel.SelectedFont = value;
			}
		}


		/// <summary>
		/// Returns the data model that handles Font selections.
		/// </summary>
		/// <returns> a <code>FontSelectionModel</code> object </returns>
		public virtual FontSelectionModel SelectionModel
		{
			get
			{
				return selectionModel;
			}
			set
			{
				FontSelectionModel oldModel = selectionModel;
				selectionModel = value;
				firePropertyChange(JFontChooser.SELECTION_MODEL_PROPERTY, oldModel, value);
			}
		}


		/// <summary>
		/// Shows a modal FontChooser dialog and blocks until the dialog is hidden. If the user presses the "OK" button, then
		/// this method hides/disposes the dialog and returns the selected Font. If the user presses the "Cancel" button or
		/// closes the dialog without pressing "OK", then this method hides/disposes the dialog and returns
		/// <code>null</code>.
		/// </summary>
		/// <param name="parent">      the parent <code>JFrame</code> for the dialog </param>
		/// <param name="initialFont"> the initial Font set when the FontChooser is shown </param>
		/// <returns> the selected Font or <code>null</code> if the user opted out </returns>
		/// <exception cref="HeadlessException"> if JGraphicsEnvironment.isHeadless() returns true. </exception>
		/// <seealso cref="java.awt.JGraphicsEnvironment.isHeadless"/>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static java.awt.Font showDialog(java.awt.Window parent, java.awt.Font initialFont) throws java.awt.HeadlessException
		public static Font showDialog(Window parent, Font initialFont)
		{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final JFontChooser pane = new JFontChooser(initialFont != null ? initialFont : new java.awt.Font(SANS_SERIF, java.awt.Font.PLAIN, 12));
			JFontChooser pane = new JFontChooser(initialFont != null ? initialFont : new Font(SANS_SERIF, Font.PLAIN, 12));

			FontSelectionActionListener selectionListener = new FontSelectionActionListener(pane);
			JDialog dialog = createDialog(parent, true, pane, selectionListener);

			dialog.setLocationRelativeTo(parent);
			dialog.setVisible(true);

			return selectionListener.Font;
		}

		/// <summary>
		/// Shows a modal FontChooser dialog and blocks until the dialog is hidden. If the user presses the "OK" button, then
		/// this method hides/disposes the dialog and returns the selected Font. If the user presses the "Cancel" button or
		/// closes the dialog without pressing "OK", then this method hides/disposes the dialog and returns
		/// <code>null</code>.
		/// </summary>
		/// <param name="parent">      the parent <code>JFrame</code> for the dialog </param>
		/// <param name="fontChooser"> the FontChooser to be use in this dialog </param>
		/// <param name="initialFont"> the initial Font set when the FontChooser is shown </param>
		/// <returns> the selected Font or <code>null</code> if the user opted out </returns>
		/// <exception cref="HeadlessException"> if JGraphicsEnvironment.isHeadless() returns true. </exception>
		/// <seealso cref="java.awt.JGraphicsEnvironment.isHeadless"/>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static java.awt.Font showDialog(java.awt.Window parent, JFontChooser fontChooser, java.awt.Font initialFont) throws java.awt.HeadlessException
		public static Font showDialog(Window parent, JFontChooser fontChooser, Font initialFont)
		{
			fontChooser.SelectedFont = initialFont != null ? initialFont : new Font(SANS_SERIF, Font.PLAIN, 12);

			FontSelectionActionListener selectionListener = new FontSelectionActionListener(fontChooser);
			JDialog dialog = createDialog(parent, true, fontChooser, selectionListener);

			dialog.setLocationRelativeTo(parent);
			dialog.setVisible(true);

			return selectionListener.Font;
		}

		/// <summary>
		/// Creates and returns a new dialog containing the specified <code>FontChooser</code> pane along with "OK" and
		/// "Cancel" buttons. If the "OK" or "Cancel" buttons are pressed, the dialog is automatically hidden (but not
		/// disposed).
		/// </summary>
		/// <param name="parent">      the parent component for the dialog </param>
		/// <param name="modal">       a boolean. When true, the remainder of the program is inactive until the dialog is closed. </param>
		/// <param name="chooserPane"> the Font-chooser to be placed inside the dialog </param>
		/// <param name="okListener">  the ActionListener invoked when "OK" is pressed </param>
		/// <returns> a new dialog containing the FontChooser pane </returns>
		/// <exception cref="HeadlessException"> if JGraphicsEnvironment.isHeadless() returns true. </exception>
		/// <seealso cref="java.awt.JGraphicsEnvironment.isHeadless"/>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static javax.swing.JDialog createDialog(java.awt.Window parent, boolean modal, JFontChooser chooserPane, java.awt.event.ActionListener okListener) throws java.awt.HeadlessException
		public static JDialog createDialog(Window parent, bool modal, JFontChooser chooserPane, ActionListener okListener)
		{
			if (parent is JDialog)
			{
				return new FontChooserDialog((JDialog) parent, modal, chooserPane, okListener);
			}
			else if (parent is JFrame)
			{
				return new FontChooserDialog((JFrame) parent, modal, chooserPane, okListener);
			}
			else
			{
				throw new System.ArgumentException("JFrame or JDialog parent is required.");
			}
		}

		/// <summary>
		/// Adds a <code>ChangeListener</code> to the model.
		/// </summary>
		/// <param name="l"> the <code>ChangeListener</code> to be added </param>
		public virtual void addChangeListener(ChangeListener l)
		{
			selectionModel.addChangeListener(l);
		}

		/// <summary>
		/// Removes a <code>ChangeListener</code> from the model.
		/// </summary>
		/// <param name="l"> the <code>ChangeListener</code> to be removed </param>
		public virtual void removeChangeListener(ChangeListener l)
		{
			selectionModel.removeChangeListener(l);
		}

		private void initPanel(ResourceBundle bundle)
		{
			// Set the font family names
			DefaultListModel<string> fontFamilyNameModel = new DefaultListModel<string>();
			foreach (string s in selectionModel.AvailableFontNames)
			{
				fontFamilyNameModel.addElement(s);
			}
			familyList.setModel(fontFamilyNameModel);
			familyList.getSelectionModel().setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
			familyList.setSelectedValue(selectionModel.SelectedFont.getName(), true);
			familyList.addListSelectionListener(new FamilyListSelectionListener(this));

			// Set the font styles
			DefaultListModel<string> fontStyleModel = new DefaultListModel<string>();

			fontStyleModel.addElement(getFontStyleName(Font.PLAIN, bundle));
			fontStyleModel.addElement(getFontStyleName(Font.BOLD, bundle));
			fontStyleModel.addElement(getFontStyleName(Font.ITALIC, bundle));
			fontStyleModel.addElement(getFontStyleName(Font.BOLD + Font.ITALIC, bundle));
			styleList.setModel(fontStyleModel);
			styleList.getSelectionModel().setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
			styleList.setSelectedIndex(selectionModel.SelectedFont.getStyle());
			styleList.addListSelectionListener(new StyleListSelectionListener(this));

			// Set the font sizes
			DefaultListModel<int> fontSizeModel = new DefaultListModel<int>();
			int size = 6;
			int step = 1;
			int ceil = 14;
			do
			{
				fontSizeModel.addElement(size);
				if (size == ceil)
				{
					ceil += ceil;
					step += step;
				}
				size = size + step;
			} while (size <= 128);

			sizeList.setModel(fontSizeModel);
			sizeList.getSelectionModel().setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
			int? selectedSize = Convert.ToInt32(selectionModel.SelectedFont.getSize());
			if (fontSizeModel.contains(selectedSize))
			{
				sizeList.setSelectedValue(selectedSize, true);
			}
			sizeList.addListSelectionListener(new SizeListSelectionListener(this));
			sizeSpinner.addChangeListener(new SizeSpinnerListener(this));
			sizeSpinner.setValue(selectedSize);
			previewAreaLabel.setFont(selectionModel.SelectedFont);
			previewAreaLabel.setText(bundle.getString("font.preview.text"));
		}

		private string getFontStyleName(int index, ResourceBundle bundle)
		{
			string result = null;
			switch (index)
			{
			case 0:
				result = bundle.getString("style.plain");
				break;
			case 1:
				result = bundle.getString("style.bold");
				break;
			case 2:
				result = bundle.getString("style.italic");
				break;
			case 3:
				result = bundle.getString("style.bolditalic");
				break;
			default:
				result = bundle.getString("style.plain");
			break;
			}

			return result;
		}

		private class FamilyListSelectionListener : ListSelectionListener
		{
			private readonly JFontChooser outerInstance;

			public FamilyListSelectionListener(JFontChooser outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void valueChanged(ListSelectionEvent e)
			{
				if (!e.getValueIsAdjusting())
				{
					Font sel = new Font(outerInstance.familyList.getSelectedValue().ToString(), outerInstance.styleList.getSelectedIndex(), int.Parse(outerInstance.sizeSpinner.getValue().ToString()));
					outerInstance.selectionModel.SelectedFont = sel;
					outerInstance.previewAreaLabel.setFont(outerInstance.selectionModel.SelectedFont);
				}
			}
		}

		private class StyleListSelectionListener : ListSelectionListener
		{
			private readonly JFontChooser outerInstance;

			public StyleListSelectionListener(JFontChooser outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void valueChanged(ListSelectionEvent e)
			{
				if (!e.getValueIsAdjusting())
				{
					outerInstance.selectionModel.SelectedFont = outerInstance.selectionModel.SelectedFont.deriveFont(outerInstance.styleList.getSelectedIndex());

					outerInstance.previewAreaLabel.setFont(outerInstance.selectionModel.SelectedFont);
				}
			}
		}

		private class SizeListSelectionListener : ListSelectionListener
		{
			private readonly JFontChooser outerInstance;

			public SizeListSelectionListener(JFontChooser outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void valueChanged(ListSelectionEvent e)
			{
				if (!e.getValueIsAdjusting())
				{
					int index = ((DefaultListModel<int>) outerInstance.sizeList.getModel()).indexOf(outerInstance.sizeList.getSelectedValue());
					if (index > -1)
					{
						outerInstance.sizeSpinner.setValue((int?) outerInstance.sizeList.getSelectedValue());
					}
					float newSize = float.Parse(outerInstance.sizeSpinner.getValue().ToString());
					Font newFont = outerInstance.selectionModel.SelectedFont.deriveFont(newSize);
					outerInstance.selectionModel.SelectedFont = newFont;

					outerInstance.previewAreaLabel.setFont(outerInstance.selectionModel.SelectedFont);
				}
			}
		}

		private class SizeSpinnerListener : ChangeListener
		{
			private readonly JFontChooser outerInstance;

			public SizeSpinnerListener(JFontChooser outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void stateChanged(ChangeEvent e)
			{
				int? value = (int?) outerInstance.sizeSpinner.getValue();
				int index = ((DefaultListModel<int>) outerInstance.sizeList.getModel()).indexOf(value);
				if (index > -1)
				{
					outerInstance.sizeList.setSelectedValue(value, true);
				}
				else
				{
					outerInstance.sizeList.clearSelection();
				}
			}
		}

		[Serializable]
		private class FontSelectionActionListener : ActionListener
		{

			internal const long serialVersionUID = 8141913945783951693L;
			internal JFontChooser chooser;
			internal Font font;

			public FontSelectionActionListener(JFontChooser c)
			{
				chooser = c;
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				font = chooser.SelectedFont;
			}

			public virtual Font Font
			{
				get
				{
					return font;
				}
			}
		}

		/// <summary>
		/// This method is called from within the constructor to initialize the form. WARNING: Do NOT modify this code. The
		/// content of this method is always regenerated by the Form Editor.
		/// </summary>
		// Java5 @SuppressWarnings("unchecked")
		// <editor-fold defaultstate="collapsed" desc="Generated
		// Code">//GEN-BEGIN:initComponents
		private void initComponents(ResourceBundle bundle)
		{
			java.awt.GridBagConstraints gridBagConstraints;

			fontPanel = new JPanel();
			familyLabel = new javax.swing.JLabel();
			styleLabel = new javax.swing.JLabel();
			sizeLabel = new javax.swing.JLabel();
			familyScrollPane = new javax.swing.JScrollPane();
			familyList = new javax.swing.JList<string>();
			styleScrollPane = new javax.swing.JScrollPane();
			styleList = new javax.swing.JList<string>();
			sizeSpinner = new javax.swing.JSpinner();
			int spinnerHeight = (int) sizeSpinner.getPreferredSize().getHeight();
			sizeSpinner.setPreferredSize(new Size(60, spinnerHeight));
			sizeScrollPane = new javax.swing.JScrollPane();
			sizeList = new javax.swing.JList<int>();
			previewPanel = new JPanel();
			previewLabel = new javax.swing.JLabel();
			previewAreaPanel = new JPanel();
			previewAreaLabel = new javax.swing.JLabel();

			setLayout(new java.awt.BorderLayout());

			fontPanel.setLayout(new java.awt.GridBagLayout());

			familyLabel.setLabelFor(familyList);
			gridBagConstraints = new java.awt.GridBagConstraints();
			gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
			gridBagConstraints.insets = new java.awt.Insets(0, 0, 5, 11);
			fontPanel.add(familyLabel, gridBagConstraints);

			styleLabel.setLabelFor(styleList);
			gridBagConstraints = new java.awt.GridBagConstraints();
			gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
			gridBagConstraints.insets = new java.awt.Insets(0, 0, 5, 11);
			fontPanel.add(styleLabel, gridBagConstraints);

			sizeLabel.setLabelFor(sizeList);
			gridBagConstraints = new java.awt.GridBagConstraints();
			gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
			gridBagConstraints.insets = new java.awt.Insets(0, 0, 5, 0);
			fontPanel.add(sizeLabel, gridBagConstraints);

			familyScrollPane.setMinimumSize(new Size(80, 50));
			familyScrollPane.setPreferredSize(new Size(240, 150));
			familyScrollPane.setViewportView(familyList);

			gridBagConstraints = new java.awt.GridBagConstraints();
			gridBagConstraints.gridy = 1;
			gridBagConstraints.gridheight = 2;
			gridBagConstraints.fill = java.awt.GridBagConstraints.BOTH;
			gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
			gridBagConstraints.weightx = 1.0;
			gridBagConstraints.weighty = 1.0;
			gridBagConstraints.insets = new java.awt.Insets(0, 0, 11, 11);
			fontPanel.add(familyScrollPane, gridBagConstraints);

			styleScrollPane.setMinimumSize(new Size(60, 120));
			styleScrollPane.setPreferredSize(new Size(80, 150));
			styleScrollPane.setViewportView(styleList);

			gridBagConstraints = new java.awt.GridBagConstraints();
			gridBagConstraints.gridy = 1;
			gridBagConstraints.gridheight = 2;
			gridBagConstraints.fill = java.awt.GridBagConstraints.VERTICAL;
			gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
			gridBagConstraints.weighty = 1.0;
			gridBagConstraints.insets = new java.awt.Insets(0, 0, 11, 11);
			fontPanel.add(styleScrollPane, gridBagConstraints);

			sizeSpinner.setModel(new javax.swing.SpinnerNumberModel(12, 6, 128, 1));
			gridBagConstraints = new java.awt.GridBagConstraints();
			gridBagConstraints.gridx = 2;
			gridBagConstraints.gridy = 1;
			gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
			gridBagConstraints.insets = new java.awt.Insets(0, 0, 5, 0);
			fontPanel.add(sizeSpinner, gridBagConstraints);

			sizeScrollPane.setMinimumSize(new Size(50, 120));
			sizeScrollPane.setPreferredSize(new Size(60, 150));
			sizeScrollPane.setViewportView(sizeList);

			gridBagConstraints = new java.awt.GridBagConstraints();
			gridBagConstraints.gridy = 2;
			gridBagConstraints.fill = java.awt.GridBagConstraints.VERTICAL;
			gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
			gridBagConstraints.weighty = 1.0;
			gridBagConstraints.insets = new java.awt.Insets(0, 0, 11, 0);
			fontPanel.add(sizeScrollPane, gridBagConstraints);

			add(fontPanel, java.awt.BorderLayout.CENTER);
			familyLabel.setDisplayedMnemonic(bundle.getString("font.family.mnemonic").charAt(0));
			familyLabel.setText(bundle.getString("font.family"));
			styleLabel.setDisplayedMnemonic(bundle.getString("font.style.mnemonic").charAt(0));
			styleLabel.setText(bundle.getString("font.style"));
			sizeLabel.setDisplayedMnemonic(bundle.getString("font.size.mnemonic").charAt(0));
			sizeLabel.setText(bundle.getString("font.size"));
			previewLabel.setDisplayedMnemonic(bundle.getString("font.preview.mnemonic").charAt(0));
			previewLabel.setText(bundle.getString("font.preview"));

			previewPanel.setLayout(new java.awt.GridBagLayout());

			gridBagConstraints = new java.awt.GridBagConstraints();
			gridBagConstraints.anchor = java.awt.GridBagConstraints.WEST;
			gridBagConstraints.insets = new java.awt.Insets(0, 0, 5, 0);
			previewPanel.add(previewLabel, gridBagConstraints);

			previewAreaPanel.setBackground(new java.awt.Color(255, 255, 255));
			previewAreaPanel.setBorder(javax.swing.BorderFactory.createEtchedBorder());
			previewAreaPanel.setPreferredSize(new Size(200, 80));
			previewAreaPanel.setLayout(new java.awt.BorderLayout());

			previewAreaLabel.setHorizontalAlignment(javax.swing.SwingConstants.CENTER);
			previewAreaPanel.add(previewAreaLabel, java.awt.BorderLayout.CENTER);

			gridBagConstraints = new java.awt.GridBagConstraints();
			gridBagConstraints.gridy = 1;
			gridBagConstraints.fill = java.awt.GridBagConstraints.HORIZONTAL;
			gridBagConstraints.weightx = 1.0;
			previewPanel.add(previewAreaPanel, gridBagConstraints);

			add(previewPanel, java.awt.BorderLayout.SOUTH);
		} // </editor-fold>//GEN-END:initComponents
		 // Variables declaration - do not modify//GEN-BEGIN:variables

		private javax.swing.JLabel familyLabel;
		private javax.swing.JList<string> familyList;
		private javax.swing.JScrollPane familyScrollPane;
		private JPanel fontPanel;
		private javax.swing.JLabel previewAreaLabel;
		private JPanel previewAreaPanel;
		private javax.swing.JLabel previewLabel;
		private JPanel previewPanel;
		private javax.swing.JLabel sizeLabel;
		private javax.swing.JList<int> sizeList;
		private javax.swing.JScrollPane sizeScrollPane;
		private javax.swing.JSpinner sizeSpinner;
		private javax.swing.JLabel styleLabel;
		private javax.swing.JList<string> styleList;
		private javax.swing.JScrollPane styleScrollPane;
		// End of variables declaration//GEN-END:variables
	}

}
