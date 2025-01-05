package logisim.gui.hex

import hex.HexModel
import java.io.*
import java.util.*
import kotlin.jvm.Throws

class HexFile private constructor() {

    companion object {
        private const val RAW_IMAGE_HEADER = "v2.0 raw"
        private const val COMMENT_MARKER = "#"

        private class HexReader(private val input: BufferedReader) : Closeable by input{
            private val data = IntArray(4096)
            private var curLine: StringTokenizer? = findNonEmptyLine()
            private var leftCount = 0L
            private var leftValue = 0L

            private fun findNonEmptyLine(): StringTokenizer? {
                while (true) {
                    val line = input.readLine() ?: return null
                    val content = line.substringBefore(COMMENT_MARKER).trim()
                    if (content.isNotEmpty()) return StringTokenizer(content)
                }
            }

            private fun nextToken(): String? {
                while (curLine != null) {
                    if (curLine!!.hasMoreTokens()) return curLine!!.nextToken()
                    curLine = findNonEmptyLine()
                }
                return null
            }

            fun hasNext(): Boolean {
                if (leftCount > 0) return true
                if (curLine?.hasMoreTokens() == true) return true
                curLine = findNonEmptyLine()
                return curLine != null
            }

            fun next(): IntArray {
                var pos = 0
                if (leftCount > 0) {
                    val count = minOf(data.size, leftCount.toInt())
                    data.fill(leftValue.toInt(), pos, pos + count)
                    pos += count
                    leftCount -= count
                }

                while (pos < data.size) {
                    val token = nextToken() ?: break
                    val (count, value) = parseToken(token)
                    val fillCount = minOf(data.size - pos, count)
                    data.fill(value, pos, pos + fillCount)
                    pos += fillCount
                    leftCount = (count - fillCount).toLong()
                    leftValue = value.toLong()
                }

                return if (pos < data.size) data.copyOf(pos) else data
            }

            private fun parseToken(token: String): Pair<Int, Int> {
                val parts = token.split('*')
                val count = if (parts.size == 2) parts[0].toInt() else 1
                val value = parts.last().toInt(16)
                return count to value
            }
        }

        @JvmStatic
        @Throws(IOException::class)
        fun save(output: Writer, src: HexModel) {
            var last = src.lastOffset
            while (last > src.firstOffset && src[last] == 0) last--

            var tokens = 0
            var cur = src.firstOffset
            while (cur <= last) {
                val value = src[cur]
                val start = cur
                cur = (cur + 1..last).firstOrNull { src[it] != value } ?: (last + 1)
                val length = cur - start

                if (tokens++ > 0) output.write(if (tokens % 8 == 1) "\n" else " ")
                if (length > 1) output.write("$length*")
                output.write(value.toString(16))
            }

            if (tokens > 0) output.write('\n'.code)
        }

        @JvmStatic
        @Throws(IOException::class)
        fun open(dst: HexModel, input: Reader) {
            HexReader(BufferedReader(input)).use { reader ->
                var offset = dst.firstOffset
                while (reader.hasNext()) {
                    val values = reader.next()
                    dst.set(offset, values)
                    offset += values.size
                }
                dst.fill(offset, dst.lastOffset - offset + 1, 0)
            }
        }

        @JvmStatic
        @Throws(IOException::class)
        fun parse(input: Reader): IntArray {
            val reader = HexReader(BufferedReader(input))
            val data = mutableListOf<Int>()
            while (reader.hasNext())
                data.addAll(reader.next().asList())
            return data.toIntArray()
        }

        @JvmStatic
        @Throws(IOException::class)
        fun open(dst: HexModel, src: File) {
            BufferedReader(FileReader(src)).use { reader ->
                if (reader.readLine() != RAW_IMAGE_HEADER) throw IOException("Invalid header format")
                open(dst, reader)
            }
        }

        @JvmStatic
        @Throws(IOException::class)
        fun save(dst: File, src: HexModel) {
            FileWriter(dst).use {
                it.write("$RAW_IMAGE_HEADER\n")
                save(it, src)
            }
        }
    }
}