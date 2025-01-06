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
package com.connectina.swing.fontchooser;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.HeadlessException;
import java.awt.Insets;
import java.awt.Window;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.Serializable;
import java.util.HashMap;
import java.util.Locale;
import java.util.ResourceBundle;
import javax.swing.BorderFactory;
import javax.swing.DefaultListModel;
import javax.swing.JComponent;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JSpinner;
import javax.swing.ListSelectionModel;
import javax.swing.SpinnerNumberModel;
import javax.swing.SwingConstants;
import javax.swing.event.ChangeEvent;
import javax.swing.event.ChangeListener;
import javax.swing.event.ListSelectionEvent;
import javax.swing.event.ListSelectionListener;

/**
 * Provides a pane of controls designed to allow a user to select a <code>Font</code>.
 * 
 * @author Christos Bohoris
 * @see Font
 */
public class JFontChooser extends JPanel {
    static final String SANS_SERIF = "SansSerif";
    private static final long serialVersionUID = 5157499702004637097L;
    private static final HashMap<Locale, ResourceBundle> bundles = new HashMap<>();

    private static ResourceBundle getBundle() {
        Locale loc = Locale.getDefault();
        ResourceBundle ret = bundles.get(loc);
        if (ret == null) {
            ret = ResourceBundle.getBundle("connectina/JFontChooser");
            bundles.put(loc, ret);
        }
        return ret;
    }

    private FontSelectionModel selectionModel;
    /**
     * The selection model property name.
     */
    public static final String SELECTION_MODEL_PROPERTY = "selectionModel";

    /**
     * Creates a FontChooser pane with an initial default Font (Sans Serif, Plain, 12).
     */
    public JFontChooser() {
        this(new Font(SANS_SERIF, Font.PLAIN, 12));
    }

    /**
     * Creates a FontChooser pane with the specified initial Font.
     *
     * @param initialFont the initial Font set in the chooser
     */
    public JFontChooser(Font initialFont) {
        this(new DefaultFontSelectionModel(initialFont));
    }

    /**
     * Creates a FontChooser pane with the specified <code>FontSelectionModel</code>.
     *
     * @param model the <code>FontSelectionModel</code> to be used
     */
    public JFontChooser(FontSelectionModel model) {
        selectionModel = model;
        ResourceBundle bundle = getBundle();
        initComponents(bundle);
        initPanel(bundle);
    }

    /**
     * Gets the current Font value from the FontChooser. By default, this delegates to the model.
     *
     * @return the current Font value of the FontChooser
     */
    public Font getSelectedFont() {
        return selectionModel.getSelectedFont();
    }

    /**
     * Sets the current Font of the FontChooser to the specified Font. The <code>FontSelectionModel</code> will fire a
     * <code>ChangeEvent</code>
     * 
     * @param Font the Font to be set in the Font chooser
     * @see JComponent#addPropertyChangeListener
     */
    public void setSelectedFont(Font Font) {
        selectionModel.setSelectedFont(Font);
    }

    /**
     * Returns the data model that handles Font selections.
     *
     * @return a <code>FontSelectionModel</code> object
     */
    public FontSelectionModel getSelectionModel() {
        return selectionModel;
    }

    /**
     * Sets the model containing the selected Font.
     *
     * @param newModel the new <code>FontSelectionModel</code> object
     */
    public void setSelectionModel(FontSelectionModel newModel) {
        FontSelectionModel oldModel = selectionModel;
        selectionModel = newModel;
        firePropertyChange(JFontChooser.SELECTION_MODEL_PROPERTY, oldModel, newModel);
    }

    /**
     * Shows a modal FontChooser dialog and blocks until the dialog is hidden. If the user presses the "OK" button, then
     * this method hides/disposes the dialog and returns the selected Font. If the user presses the "Cancel" button or
     * closes the dialog without pressing "OK", then this method hides/disposes the dialog and returns
     * <code>null</code>.
     *
     * @param parent      the parent <code>JFrame</code> for the dialog
     * @param initialFont the initial Font set when the FontChooser is shown
     * @return the selected Font or <code>null</code> if the user opted out
     * @exception HeadlessException if GraphicsEnvironment.isHeadless() returns true.
     * @see java.awt.GraphicsEnvironment#isHeadless
     */
    public static Font showDialog(Window parent, Font initialFont) throws HeadlessException {
        final JFontChooser pane = new JFontChooser(
                initialFont != null ? initialFont : new Font(SANS_SERIF, Font.PLAIN, 12));

        FontSelectionActionListener selectionListener = new FontSelectionActionListener(pane);
        JDialog dialog = createDialog(parent, true, pane, selectionListener);

        dialog.setLocationRelativeTo(parent);
        dialog.setVisible(true);

        return selectionListener.getFont();
    }

    /**
     * Shows a modal FontChooser dialog and blocks until the dialog is hidden. If the user presses the "OK" button, then
     * this method hides/disposes the dialog and returns the selected Font. If the user presses the "Cancel" button or
     * closes the dialog without pressing "OK", then this method hides/disposes the dialog and returns
     * <code>null</code>.
     *
     * @param parent      the parent <code>JFrame</code> for the dialog
     * @param fontChooser the FontChooser to be used in this dialog
     * @param initialFont the initial Font set when the FontChooser is shown
     * @return the selected Font or <code>null</code> if the user opted out
     * @exception HeadlessException if GraphicsEnvironment.isHeadless() returns true.
     * @see java.awt.GraphicsEnvironment#isHeadless
     */
    public static Font showDialog(Window parent, JFontChooser fontChooser, Font initialFont) throws HeadlessException {
        fontChooser.setSelectedFont(initialFont != null ? initialFont : new Font(SANS_SERIF, Font.PLAIN, 12));

        FontSelectionActionListener selectionListener = new FontSelectionActionListener(fontChooser);
        JDialog dialog = createDialog(parent, true, fontChooser, selectionListener);

        dialog.setLocationRelativeTo(parent);
        dialog.setVisible(true);

        return selectionListener.getFont();
    }

    /**
     * Creates and returns a new dialog containing the specified <code>FontChooser</code> pane along with "OK" and
     * "Cancel" buttons. If the "OK" or "Cancel" buttons are pressed, the dialog is automatically hidden (but not
     * disposed).
     *
     * @param parent      the parent component for the dialog
     * @param modal       a boolean. When true, the remainder of the program is inactive until the dialog is closed.
     * @param chooserPane the Font-chooser to be placed inside the dialog
     * @param okListener  the ActionListener invoked when "OK" is pressed
     * @return a new dialog containing the FontChooser pane
     * @exception HeadlessException if GraphicsEnvironment.isHeadless() returns true.
     * @see java.awt.GraphicsEnvironment#isHeadless
     */
    public static JDialog createDialog(Window parent, boolean modal, JFontChooser chooserPane,
            ActionListener okListener) throws HeadlessException {
        if (parent instanceof JDialog) return new FontChooserDialog((JDialog) parent, modal, chooserPane, okListener);
		else if (parent instanceof JFrame)
			return new FontChooserDialog((JFrame) parent, modal, chooserPane, okListener);
		else throw new IllegalArgumentException("JFrame or JDialog parent is required.");
    }

    /**
     * Adds a <code>ChangeListener</code> to the model.
     *
     * @param l the <code>ChangeListener</code> to be added
     */
    public void addChangeListener(ChangeListener l) {
        selectionModel.addChangeListener(l);
    }

    /**
     * Removes a <code>ChangeListener</code> from the model.
     * 
     * @param l the <code>ChangeListener</code> to be removed
     */
    public void removeChangeListener(ChangeListener l) {
        selectionModel.removeChangeListener(l);
    }

    private void initPanel(ResourceBundle bundle) {
        // Set the font family names
        DefaultListModel<String> fontFamilyNameModel = new DefaultListModel<>();
        for (String s : selectionModel.getAvailableFontNames()) fontFamilyNameModel.addElement(s);
        familyList.setModel(fontFamilyNameModel);
        familyList.getSelectionModel().setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
        familyList.setSelectedValue(selectionModel.getSelectedFont().getName(), true);
        familyList.addListSelectionListener(new FamilyListSelectionListener());

        // Set the font styles
        DefaultListModel<String> fontStyleModel = new DefaultListModel<>();

        fontStyleModel.addElement(getFontStyleName(Font.PLAIN, bundle));
        fontStyleModel.addElement(getFontStyleName(Font.BOLD, bundle));
        fontStyleModel.addElement(getFontStyleName(Font.ITALIC, bundle));
        fontStyleModel.addElement(getFontStyleName(Font.BOLD + Font.ITALIC, bundle));
        styleList.setModel(fontStyleModel);
        styleList.getSelectionModel().setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
        styleList.setSelectedIndex(selectionModel.getSelectedFont().getStyle());
        styleList.addListSelectionListener(new StyleListSelectionListener());

        // Set the font sizes
        DefaultListModel<Integer> fontSizeModel = new DefaultListModel<>();
        int size = 6;
        int step = 1;
        int ceil = 14;
        do {
            fontSizeModel.addElement(size);
            if (size == ceil) {
                ceil += ceil;
                step += step;
            }
            size = size + step;
        } while (size <= 128);

        sizeList.setModel(fontSizeModel);
        sizeList.getSelectionModel().setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
        Integer selectedSize = selectionModel.getSelectedFont().getSize();
        if (fontSizeModel.contains(selectedSize)) sizeList.setSelectedValue(selectedSize, true);
        sizeList.addListSelectionListener(new SizeListSelectionListener());
        sizeSpinner.addChangeListener(new SizeSpinnerListener());
        sizeSpinner.setValue(selectedSize);
        previewAreaLabel.setFont(selectionModel.getSelectedFont());
        previewAreaLabel.setText(bundle.getString("font.preview.text"));
    }

    private String getFontStyleName(int index, ResourceBundle bundle) {

		return switch (index) {
			case 1 -> bundle.getString("style.bold");
			case 2 -> bundle.getString("style.italic");
			case 3 -> bundle.getString("style.bolditalic");
			default -> bundle.getString("style.plain");
		};
    }

    private class FamilyListSelectionListener implements ListSelectionListener {

        public void valueChanged(ListSelectionEvent e) {
            if (!e.getValueIsAdjusting()) {
                Font sel = new Font(familyList.getSelectedValue(), styleList.getSelectedIndex(),
                        Integer.parseInt(sizeSpinner.getValue().toString()));
                selectionModel.setSelectedFont(sel);
                previewAreaLabel.setFont(selectionModel.getSelectedFont());
            }
        }
    }

    private class StyleListSelectionListener implements ListSelectionListener {

        public void valueChanged(ListSelectionEvent e) {
            if (!e.getValueIsAdjusting()) {
                selectionModel
                        .setSelectedFont(selectionModel.getSelectedFont().deriveFont(styleList.getSelectedIndex()));

                previewAreaLabel.setFont(selectionModel.getSelectedFont());
            }
        }
    }

    private class SizeListSelectionListener implements ListSelectionListener {

        public void valueChanged(ListSelectionEvent e) {
            if (!e.getValueIsAdjusting()) {
                int index = ((DefaultListModel<Integer>) sizeList.getModel()).indexOf(sizeList.getSelectedValue());
                if (index > -1) sizeSpinner.setValue(sizeList.getSelectedValue());
                float newSize = Float.parseFloat(sizeSpinner.getValue().toString());
                Font newFont = selectionModel.getSelectedFont().deriveFont(newSize);
                selectionModel.setSelectedFont(newFont);

                previewAreaLabel.setFont(selectionModel.getSelectedFont());
            }
        }
    }

    private class SizeSpinnerListener implements ChangeListener {

        public void stateChanged(ChangeEvent e) {
            Integer value = (Integer) sizeSpinner.getValue();
            int index = ((DefaultListModel<Integer>) sizeList.getModel()).indexOf(value);
            if (index > -1) sizeList.setSelectedValue(value, true);
			else sizeList.clearSelection();
        }
    }

    private static class FontSelectionActionListener implements ActionListener, Serializable {

        private static final long serialVersionUID = 8141913945783951693L;
        private JFontChooser chooser;
        private Font font;

        public FontSelectionActionListener(JFontChooser c) {
            chooser = c;
        }

        public void actionPerformed(ActionEvent e) {
            font = chooser.getSelectedFont();
        }

        public Font getFont() {
            return font;
        }
    }

    /**
     * This method is called from within the constructor to initialize the form. WARNING: Do NOT modify this code. The
     * content of this method is always regenerated by the Form Editor.
     */
    // Java5 @SuppressWarnings("unchecked")
    // <editor-fold defaultstate="collapsed" desc="Generated
    // Code">//GEN-BEGIN:initComponents
    private void initComponents(ResourceBundle bundle) {

		JPanel fontPanel = new JPanel();
		JLabel familyLabel = new JLabel();
		JLabel styleLabel = new JLabel();
		JLabel sizeLabel = new JLabel();
		JScrollPane familyScrollPane = new JScrollPane();
        familyList = new JList<>();
		JScrollPane styleScrollPane = new JScrollPane();
        styleList = new JList<>();
        sizeSpinner = new JSpinner();
        int spinnerHeight = (int) sizeSpinner.getPreferredSize().getHeight();
        sizeSpinner.setPreferredSize(new Dimension(60, spinnerHeight));
		JScrollPane sizeScrollPane = new JScrollPane();
        sizeList = new JList<>();
		JPanel previewPanel = new JPanel();
		JLabel previewLabel = new JLabel();
		JPanel previewAreaPanel = new JPanel();
        previewAreaLabel = new JLabel();

        setLayout(new BorderLayout());

        fontPanel.setLayout(new GridBagLayout());

        familyLabel.setLabelFor(familyList);
		GridBagConstraints gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.anchor = GridBagConstraints.WEST;
        gridBagConstraints.insets = new Insets(0, 0, 5, 11);
        fontPanel.add(familyLabel, gridBagConstraints);

        styleLabel.setLabelFor(styleList);
        gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.anchor = GridBagConstraints.WEST;
        gridBagConstraints.insets = new Insets(0, 0, 5, 11);
        fontPanel.add(styleLabel, gridBagConstraints);

        sizeLabel.setLabelFor(sizeList);
        gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.anchor = GridBagConstraints.WEST;
        gridBagConstraints.insets = new Insets(0, 0, 5, 0);
        fontPanel.add(sizeLabel, gridBagConstraints);

        familyScrollPane.setMinimumSize(new Dimension(80, 50));
        familyScrollPane.setPreferredSize(new Dimension(240, 150));
        familyScrollPane.setViewportView(familyList);

        gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.gridy = 1;
        gridBagConstraints.gridheight = 2;
        gridBagConstraints.fill = GridBagConstraints.BOTH;
        gridBagConstraints.anchor = GridBagConstraints.WEST;
        gridBagConstraints.weightx = 1.0;
        gridBagConstraints.weighty = 1.0;
        gridBagConstraints.insets = new Insets(0, 0, 11, 11);
        fontPanel.add(familyScrollPane, gridBagConstraints);

        styleScrollPane.setMinimumSize(new Dimension(60, 120));
        styleScrollPane.setPreferredSize(new Dimension(80, 150));
        styleScrollPane.setViewportView(styleList);

        gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.gridy = 1;
        gridBagConstraints.gridheight = 2;
        gridBagConstraints.fill = GridBagConstraints.VERTICAL;
        gridBagConstraints.anchor = GridBagConstraints.WEST;
        gridBagConstraints.weighty = 1.0;
        gridBagConstraints.insets = new Insets(0, 0, 11, 11);
        fontPanel.add(styleScrollPane, gridBagConstraints);

        sizeSpinner.setModel(new SpinnerNumberModel(12, 6, 128, 1));
        gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.gridx = 2;
        gridBagConstraints.gridy = 1;
        gridBagConstraints.anchor = GridBagConstraints.WEST;
        gridBagConstraints.insets = new Insets(0, 0, 5, 0);
        fontPanel.add(sizeSpinner, gridBagConstraints);

        sizeScrollPane.setMinimumSize(new Dimension(50, 120));
        sizeScrollPane.setPreferredSize(new Dimension(60, 150));
        sizeScrollPane.setViewportView(sizeList);

        gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.gridy = 2;
        gridBagConstraints.fill = GridBagConstraints.VERTICAL;
        gridBagConstraints.anchor = GridBagConstraints.WEST;
        gridBagConstraints.weighty = 1.0;
        gridBagConstraints.insets = new Insets(0, 0, 11, 0);
        fontPanel.add(sizeScrollPane, gridBagConstraints);

        add(fontPanel, BorderLayout.CENTER);
        familyLabel.setDisplayedMnemonic(bundle.getString("font.family.mnemonic").charAt(0));
        familyLabel.setText(bundle.getString("font.family"));
        styleLabel.setDisplayedMnemonic(bundle.getString("font.style.mnemonic").charAt(0));
        styleLabel.setText(bundle.getString("font.style"));
        sizeLabel.setDisplayedMnemonic(bundle.getString("font.size.mnemonic").charAt(0));
        sizeLabel.setText(bundle.getString("font.size"));
        previewLabel.setDisplayedMnemonic(bundle.getString("font.preview.mnemonic").charAt(0));
        previewLabel.setText(bundle.getString("font.preview"));

        previewPanel.setLayout(new GridBagLayout());

        gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.anchor = GridBagConstraints.WEST;
        gridBagConstraints.insets = new Insets(0, 0, 5, 0);
        previewPanel.add(previewLabel, gridBagConstraints);

        previewAreaPanel.setBackground(new Color(255, 255, 255));
        previewAreaPanel.setBorder(BorderFactory.createEtchedBorder());
        previewAreaPanel.setPreferredSize(new Dimension(200, 80));
        previewAreaPanel.setLayout(new BorderLayout());

        previewAreaLabel.setHorizontalAlignment(SwingConstants.CENTER);
        previewAreaPanel.add(previewAreaLabel, BorderLayout.CENTER);

        gridBagConstraints = new GridBagConstraints();
        gridBagConstraints.gridy = 1;
        gridBagConstraints.fill = GridBagConstraints.HORIZONTAL;
        gridBagConstraints.weightx = 1.0;
        previewPanel.add(previewAreaPanel, gridBagConstraints);

        add(previewPanel, BorderLayout.SOUTH);
    }// </editor-fold>//GEN-END:initComponents
     // Variables declaration - do not modify//GEN-BEGIN:variables

	private JList<String> familyList;
	private JLabel previewAreaLabel;
	private JList<Integer> sizeList;
	private JSpinner sizeSpinner;
	private JList<String> styleList;
	// End of variables declaration//GEN-END:variables
}
