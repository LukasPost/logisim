package logisim.gui.hex

import hex.HexEditor
import hex.HexModel
import logisim.gui.generic.LFrame
import logisim.gui.menu.LogisimMenuBar
import logisim.proj.Project
import logisim.util.JFileChoosers
import logisim.util.LocaleListener
import logisim.util.LocaleManager
import logisim.util.WindowMenuItemManager
import java.awt.BorderLayout
import java.awt.Dimension
import java.awt.event.ActionEvent
import java.awt.event.ActionListener
import java.awt.event.WindowEvent
import java.io.File
import java.io.IOException
import javax.swing.*
import javax.swing.event.ChangeEvent
import javax.swing.event.ChangeListener

class HexFrame(
	proj: Project,
	private val model: HexModel
) : LFrame() {

	private val windowManager = WindowMenuManager()
	private val editListener = EditListener()
	private val myListener = MyListener()
	private val editor = HexEditor(model)

	private val openButton = JButton()
	private val saveButton = JButton()
	private val closeButton = JButton()

	init {
		defaultCloseOperation = HIDE_ON_CLOSE

		val menubar = LogisimMenuBar(this, proj).apply { setJMenuBar(this) }

		val buttonPanel = JPanel().apply {
			add(openButton.apply { addActionListener(myListener) })
			add(saveButton.apply { addActionListener(myListener) })
			add(closeButton.apply { addActionListener(myListener) })
		}

		val scrollPane = JScrollPane(editor, JScrollPane.VERTICAL_SCROLLBAR_ALWAYS, JScrollPane.HORIZONTAL_SCROLLBAR_NEVER).apply {
			preferredSize = editor.preferredSize.also { it.height = minOf(it.height, it.width * 3 / 2) }
			viewport.background = editor.background
		}

		contentPane.apply {
			add(scrollPane, BorderLayout.CENTER)
			add(buttonPanel, BorderLayout.SOUTH)
		}

		LocaleManager.addLocaleListener(myListener)
		myListener.localeChanged()

		pack()
		adjustSizeToFitScreen()

		editor.caret.apply {
			addChangeListener(editListener)
			setDot(0, false)
		}
		editListener.register(menubar)
	}

	override fun setVisible(value: Boolean) {
		if (value && !isVisible) {
			windowManager.frameOpened(this)
		}
		super.setVisible(value)
	}

	private fun adjustSizeToFitScreen() {
		val screenSize = toolkit.screenSize
		size = size.let {
			Dimension(minOf(it.width, screenSize.width), minOf(it.height, screenSize.height))
		}
	}

	private inner class WindowMenuManager : WindowMenuItemManager(Strings.get("hexFrameMenuItem"), false), LocaleListener {
		init {
			LocaleManager.addLocaleListener(this)
		}

		override fun getJFrame(create: Boolean) = this@HexFrame

		override fun localeChanged() {
			text = Strings.get("hexFrameMenuItem")
		}
	}

	private inner class MyListener : AbstractAction(), LocaleListener {
		private var lastFile: File? = null

		override fun actionPerformed(e: ActionEvent) {
			when (e.source) {
				openButton -> handleFileAction("openButton", JFileChooser::showOpenDialog) { file ->
					try {
						HexFile.open(model, file)
						lastFile = file
					} catch (ex: IOException) {
						showErrorDialog("hexOpenErrorTitle", ex.message)
					}
				}
				saveButton -> handleFileAction("saveButton", JFileChooser::showSaveDialog) { file ->
					try {
						HexFile.save(file, model)
						lastFile = file
					} catch (ex: IOException) {
						showErrorDialog("hexSaveErrorTitle", ex.message)
					}
				}
				closeButton -> processWindowEvent(WindowEvent(this@HexFrame, WindowEvent.WINDOW_CLOSING))
			}
		}

		override fun localeChanged() {
			title = Strings.get("hexFrameTitle")
			openButton.text = Strings.get("openButton")
			saveButton.text = Strings.get("saveButton")
			closeButton.text = Strings.get("closeButton")
		}

		private fun handleFileAction(titleKey: String, dialogAction: JFileChooser.(HexFrame) -> Int, fileAction: (File) -> Unit) {
			val chooser = JFileChoosers.createSelected(lastFile).apply { dialogTitle = Strings.get(titleKey) }
			if (chooser.dialogAction(this@HexFrame) == JFileChooser.APPROVE_OPTION) {
				fileAction(chooser.selectedFile)
			}
		}

		private fun showErrorDialog(titleKey: String, message: String?) {
			JOptionPane.showMessageDialog(this@HexFrame, message, Strings.get(titleKey), JOptionPane.ERROR_MESSAGE)
		}
	}

	private inner class EditListener : ActionListener, ChangeListener {
		private var clip: Clip? = null

		private fun getClip() = clip ?: Clip(editor).also { clip = it }

		fun register(menubar: LogisimMenuBar) {
			with(menubar) {
				arrayOf(LogisimMenuBar.CUT, LogisimMenuBar.COPY, LogisimMenuBar.PASTE, LogisimMenuBar.DELETE, LogisimMenuBar.SELECT_ALL).forEach {
					addActionListener(it, this@EditListener)
				}
				enableMenuItems(this)
			}
		}

		override fun actionPerformed(e: ActionEvent) {
			when (e.source) {
				LogisimMenuBar.CUT -> {
					getClip().copy()
					editor.delete()
				}
				LogisimMenuBar.COPY -> getClip().copy()
				LogisimMenuBar.PASTE -> getClip().paste()
				LogisimMenuBar.DELETE -> editor.delete()
				LogisimMenuBar.SELECT_ALL -> editor.selectAll()
			}
		}

		override fun stateChanged(e: ChangeEvent?) {
			enableMenuItems(getJMenuBar() as LogisimMenuBar)
		}

		private fun enableMenuItems(menubar: LogisimMenuBar) {
			val selectionExists = editor.selectionExists()
			val clipboardAvailable = true // TODO: Implement clipboard check
			with(menubar) {
				setEnabled(LogisimMenuBar.CUT, selectionExists)
				setEnabled(LogisimMenuBar.COPY, selectionExists)
				setEnabled(LogisimMenuBar.PASTE, clipboardAvailable)
				setEnabled(LogisimMenuBar.DELETE, selectionExists)
				setEnabled(LogisimMenuBar.SELECT_ALL, true)
			}
		}
	}
}
