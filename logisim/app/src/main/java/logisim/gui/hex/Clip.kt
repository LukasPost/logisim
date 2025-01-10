package logisim.gui.hex

import java.awt.datatransfer.*
import java.io.IOException
import javax.swing.JOptionPane
import hex.Caret
import hex.HexEditor

class Clip(private val editor: HexEditor) : ClipboardOwner {

	companion object {
		private val binaryFlavor = DataFlavor(IntArray::class.java, "Binary data")
	}

	private class Data(private val data: IntArray) : Transferable {
		override fun getTransferDataFlavors(): Array<DataFlavor> =
		arrayOf(binaryFlavor, DataFlavor.stringFlavor)

		override fun isDataFlavorSupported(flavor: DataFlavor): Boolean =
				flavor == binaryFlavor || flavor == DataFlavor.stringFlavor

		@Throws(UnsupportedFlavorException::class, IOException::class)
		override fun getTransferData(flavor: DataFlavor): Any {
			return when (flavor) {
				binaryFlavor -> data
				DataFlavor.stringFlavor -> dataToHexString(data)
                else -> throw UnsupportedFlavorException(flavor)
			}
		}

		private fun dataToHexString(data: IntArray): String {
			val maxBits = data.maxOf { it.toString(2).length }
			val chars = (maxBits + 3) / 4
			return data.joinToString(separator = " ") { it.toString(16).padStart(chars, '0') }
		}
	}

	fun copy() {
		val caret = editor.caret
		val range = getValidRange(caret) ?: return
				val model = editor.model
		val data = IntArray((range.second - range.first + 1).toInt()) { i ->
				model.get(range.first + i)
		}
		editor.toolkit.systemClipboard.setContents(Data(data), this)
	}

	fun canPaste(): Boolean {
		val clipboard = editor.toolkit.systemClipboard
		val content = clipboard.getContents(this)
		return content.isDataFlavorSupported(binaryFlavor)
	}

	fun paste() {
		val clipboard = editor.toolkit.systemClipboard
		val transferable = clipboard.getContents(this) ?: return

				val data: IntArray = try {
			when {
				transferable.isDataFlavorSupported(binaryFlavor) ->
				transferable.getTransferData(binaryFlavor) as IntArray
				transferable.isDataFlavorSupported(DataFlavor.stringFlavor) ->
				parseHexString(transferable.getTransferData(DataFlavor.stringFlavor) as String)
                else -> {
					showError("hexPasteSupportedError", "hexPasteErrorTitle")
					return
				}
			}
		} catch (e: Exception) {
			showError("hexPasteErrorTitle", e.message)
			return
		}

		val caret = editor.caret
		val range = getValidRange(caret)
		if (range == null || !applyPasteData(data, range)) {
			showError("hexPasteErrorTitle", "hexPasteSizeError")
		}
	}

	override fun lostOwnership(clipboard: Clipboard?, contents: Transferable?) {
		// No specific handling required
	}

	private fun getValidRange(caret: Caret): Pair<Long, Long>? {
		var p0 = caret.mark
		var p1 = caret.dot
		if (p0 < 0 || p1 < 0) return null
		if (p0 > p1) p0 = p1.also { p1 = p0 }
		return Pair(p0, p1 + 1)
	}

	private fun applyPasteData(data: IntArray, range: Pair<Long, Long>): Boolean {
		val model = editor.model
		return if (range.first == range.second - 1) {
			if (range.first + data.size - 1 <= model.lastOffset) {
				model.set(range.first, data)
				true
			} else {
				showError("hexPasteErrorTitle", "hexPasteEndError")
				false
			}
		} else {
			if (range.second - range.first == data.size.toLong()) {
				model.set(range.first, data)
				true
			} else {
				false
			}
		}
	}

	private fun parseHexString(hexString: String): IntArray {
		return hexString.lineSequence()
				.flatMap { it.split(Regex("\\s+")) }
            .map { it.toInt(16) }
            .toList()
				.toIntArray()
	}

	private fun showError(titleKey: String, messageKey: String?) {
		JOptionPane.showMessageDialog(editor.rootPane, messageKey, titleKey, JOptionPane.ERROR_MESSAGE)
	}
}
