/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.gui;

import java.awt.Dimension;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.GridLayout;
import java.awt.Insets;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.AbstractListModel;
import javax.swing.JButton;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTextField;
import javax.swing.ListSelectionModel;
import javax.swing.ScrollPaneConstants;
import javax.swing.event.DocumentEvent;
import javax.swing.event.DocumentListener;
import javax.swing.event.ListSelectionEvent;
import javax.swing.event.ListSelectionListener;

import logisim.analyze.model.VariableList;
import logisim.analyze.model.VariableListEvent;
import logisim.analyze.model.VariableListListener;
import logisim.util.StringUtil;

class VariableTab extends AnalyzerTab implements TabInterface {
	private static class VariableListModel extends AbstractListModel<String> implements VariableListListener {
		private VariableList list;
		private String[] listCopy;

		public VariableListModel(VariableList list) {
			this.list = list;
			updateCopy();
			list.addVariableListListener(this);
		}

		private void updateCopy() {
			listCopy = list.toArray(String[]::new);
		}

		public int getSize() {
			return listCopy.length;
		}

		public String getElementAt(int index) {
			return index >= 0 && index < listCopy.length ? listCopy[index] : null;
		}

		private void update() {
			String[] oldCopy = listCopy;
			updateCopy();
			fireContentsChanged(this, 0, oldCopy.length);
		}

		public void listChanged(VariableListEvent event) {
			String[] oldCopy = listCopy;
			updateCopy();
			int index;
			switch (event.getType()) {
			case VariableListEvent.ALL_REPLACED:
				fireContentsChanged(this, 0, oldCopy.length);
				return;
			case VariableListEvent.ADD:
				index = list.indexOf(event.getVariable());
				fireIntervalAdded(this, index, index);
				return;
			case VariableListEvent.REMOVE:
				index = (Integer) event.getData();
				fireIntervalRemoved(this, index, index);
				return;
			case VariableListEvent.MOVE:
				fireContentsChanged(this, 0, getSize());
				return;
			case VariableListEvent.REPLACE:
				index = (Integer) event.getData();
				fireContentsChanged(this, index, index);
			}
		}
	}

	private class MyListener implements ActionListener, DocumentListener, ListSelectionListener {
		public void actionPerformed(ActionEvent event) {
			Object src = event.getSource();
			if ((src == add || src == field) && add.isEnabled()) {
				String name = field.getText().trim();
				if (!name.isEmpty()) {
					data.add(name);
					if (data.contains(name))
						list.setSelectedValue(name, true);
					field.setText("");
					field.grabFocus();
				}
			} else if (src == rename) {
				String oldName = list.getSelectedValue();
				String newName = field.getText().trim();
				if (oldName != null && !newName.isEmpty()) {
					data.replace(oldName, newName);
					field.setText("");
					field.grabFocus();
				}
			} else if (src == remove) {
				String name = list.getSelectedValue();
				if (name != null)
					data.remove(name);
			} else if (src == moveUp) {
				String name = list.getSelectedValue();
				if (name != null) {
					data.move(name, -1);
					list.setSelectedValue(name, true);
				}
			} else if (src == moveDown) {
				String name = list.getSelectedValue();
				if (name != null) {
					data.move(name, 1);
					list.setSelectedValue(name, true);
				}
			}
		}

		public void insertUpdate(DocumentEvent event) {
			computeEnabled();
		}

		public void removeUpdate(DocumentEvent event) {
			insertUpdate(event);
		}

		public void changedUpdate(DocumentEvent event) {
			insertUpdate(event);
		}

		public void valueChanged(ListSelectionEvent event) {
			computeEnabled();
		}
	}

	private VariableList data;

	private JList<String> list = new JList<>();
	private JTextField field = new JTextField();
	private JButton remove = new JButton();
	private JButton moveUp = new JButton();
	private JButton moveDown = new JButton();
	private JButton add = new JButton();
	private JButton rename = new JButton();
	private JLabel error = new JLabel(" ");

	VariableTab(VariableList data) {
		this.data = data;

		list.setModel(new VariableListModel(data));
		list.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
		MyListener myListener = new MyListener();
		list.addListSelectionListener(myListener);
		remove.addActionListener(myListener);
		moveUp.addActionListener(myListener);
		moveDown.addActionListener(myListener);
		add.addActionListener(myListener);
		rename.addActionListener(myListener);
		field.addActionListener(myListener);
		field.getDocument().addDocumentListener(myListener);

		JScrollPane listPane = new JScrollPane(list, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS,
				ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
		listPane.setPreferredSize(new Dimension(100, 100));

		JPanel topPanel = new JPanel(new GridLayout(3, 1));
		topPanel.add(remove);
		topPanel.add(moveUp);
		topPanel.add(moveDown);

		JPanel fieldPanel = new JPanel();
		fieldPanel.add(rename);
		fieldPanel.add(add);

		GridBagLayout gb = new GridBagLayout();
		GridBagConstraints gc = new GridBagConstraints();
		setLayout(gb);
		Insets oldInsets = gc.insets;

		gc.insets = new Insets(10, 10, 0, 0);
		gc.fill = GridBagConstraints.BOTH;
		gc.weightx = 1.0;
		gb.setConstraints(listPane, gc);
		add(listPane);

		gc.fill = GridBagConstraints.NONE;
		gc.anchor = GridBagConstraints.PAGE_START;
		gc.weightx = 0.0;
		gb.setConstraints(topPanel, gc);
		add(topPanel);

		gc.insets = new Insets(10, 10, 0, 10);
		gc.gridwidth = GridBagConstraints.REMAINDER;
		gc.gridx = 0;
		gc.gridy = GridBagConstraints.RELATIVE;
		gc.fill = GridBagConstraints.HORIZONTAL;
		gb.setConstraints(field, gc);
		add(field);

		gc.insets = oldInsets;
		gc.fill = GridBagConstraints.NONE;
		gc.anchor = GridBagConstraints.LINE_END;
		gb.setConstraints(fieldPanel, gc);
		add(fieldPanel);

		gc.fill = GridBagConstraints.HORIZONTAL;
		gb.setConstraints(error, gc);
		add(error);

		if (!data.isEmpty())
			list.setSelectedValue(data.get(0), true);
		computeEnabled();
	}

	void localeChanged() {
		remove.setText(Strings.get("variableRemoveButton"));
		moveUp.setText(Strings.get("variableMoveUpButton"));
		moveDown.setText(Strings.get("variableMoveDownButton"));
		add.setText(Strings.get("variableAddButton"));
		rename.setText(Strings.get("variableRenameButton"));
		validateInput();
	}

	@Override
	void updateTab() {
		VariableListModel model = (VariableListModel) list.getModel();
		model.update();
	}

	void registerDefaultButtons(DefaultRegistry registry) {
		registry.registerDefaultButton(field, add);
	}

	private void computeEnabled() {
		int index = list.getSelectedIndex();
		boolean selected = index >= 0 && index < list.getModel().getSize();
		remove.setEnabled(selected);
		moveUp.setEnabled(selected && index > 0);
		moveDown.setEnabled(selected);

		boolean ok = validateInput();
		add.setEnabled(ok && data.size() < data.getMaximumSize());
		rename.setEnabled(ok && selected);
	}

	private boolean validateInput() {
		String text = field.getText().trim();
		boolean ok = true;
		boolean errorShown = true;
		if (text.isEmpty()) {
			errorShown = false;
			ok = false;
		} else if (!Character.isJavaIdentifierStart(text.charAt(0))) {
			error.setText(Strings.get("variableStartError"));
			ok = false;
		} else for (int i = 1; i < text.length() && ok; i++) {
			char c = text.charAt(i);
			if (!Character.isJavaIdentifierPart(c)) {
				error.setText(StringUtil.format(Strings.get("variablePartError"), "" + c));
				ok = false;
			}
		}
		if (ok && data.stream().anyMatch(text::equals)) {
			error.setText(Strings.get("variableDuplicateError"));
			ok = false;
		}
		if (ok || !errorShown)
			error.setText(data.size() >= data.getMaximumSize() ? StringUtil.format(Strings.get("variableMaximumError"), "" + data.getMaximumSize()) : " ");
		return ok;
	}

	public void copy() {
		field.requestFocus();
		field.copy();
	}

	public void paste() {
		field.requestFocus();
		field.paste();
	}

	public void delete() {
		field.requestFocus();
		field.replaceSelection("");
	}

	public void selectAll() {
		field.requestFocus();
		field.selectAll();
	}
}
