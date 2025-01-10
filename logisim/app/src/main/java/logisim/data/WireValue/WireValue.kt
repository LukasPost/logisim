package logisim.data.WireValue

import logisim.data.BitWidth
import java.awt.Color

interface WireValue {

    companion object {
        const val MAX_WIDTH = 32

        val NIL_COLOR = Color.GRAY
        val FALSE_COLOR = Color(0, 100, 0)
        val TRUE_COLOR = Color(0, 210, 0)
        val UNKNOWN_COLOR = Color(40, 40, 255)
        val ERROR_COLOR = Color(192, 0, 0)
        val WIDTH_ERROR_COLOR = Color(255, 123, 0)
        val MULTI_COLOR = Color.BLACK


        fun create(values: Iterable<WireValue>): WireValue {
            require(values.count() <= MAX_WIDTH) { "Cannot have more than $MAX_WIDTH bits in a value" }
            if (values.count() == 0)
                return WireValues.NIL
            if (values.count() == 1)
                return values.first()

            val width = values.count()
            var value = 0
            var unknown = 0
            var error = 0

            values.forEachIndexed { i, v ->
                val mask = 1 shl i
                when (v) {
                    WireValues.TRUE -> value = value or mask
                    WireValues.FALSE -> Unit
                    WireValues.UNKNOWN -> unknown = unknown or mask
                    WireValues.ERROR -> error = error or mask
                    else -> throw RuntimeException("Unrecognized value $v")
                }
            }
            return create(width, error, unknown, value)
        }


        fun create(values: Array<WireValue>): WireValue =
            create(values.asIterable());

        fun createKnown(bits: BitWidth, value: Int): WireValue =
            create(bits.width, 0, 0, value)

        fun createUnknown(bits: BitWidth): WireValue =
            create(bits.width, 0, -1, 0)

        fun createError(bits: BitWidth): WireValue =
            create(bits.width, -1, 0, 0)

        fun create(width: Int, error: Int, unknown: Int, value: Int): WireValue {
            if (width == 0) return WireValues.NIL
            if (width == 1) {
                return when {
                    (error and 1) != 0 -> WireValues.ERROR
                    (unknown and 1) != 0 -> WireValues.UNKNOWN
                    (value and 1) != 0 -> WireValues.TRUE
                    else -> WireValues.FALSE
                }
            }

            val mask = if (width == 32) -1 else (1 shl width) - 1
            val maskedError = error and mask
            val maskedUnknown = unknown and mask and maskedError.inv()
            val maskedValue = value and mask and maskedUnknown.inv() and maskedError.inv()

            return CustomWireValue(width, maskedError, maskedUnknown, maskedValue)
        }

        fun repeat(base: WireValue, bits: Int): WireValue {
            require(base.width == 1) { "First parameter must be one bit" }

            return if (bits == 1) base
            else create(List(bits) { base })
        }
    }


    val width: Int
    val bitWidth: BitWidth
    val error: Int
    val unknown: Int
    val value: Int

    val isErrorValue: Boolean
    val isUnknown: Boolean
    val isFullyDefined: Boolean
    val allValues: Array<WireValue>
    fun get(which: Int): WireValue
    fun getAll() : Array<WireValue>

    fun extendWidth(newWidth: Int, others: WireValue): WireValue
    fun set(which: Int, value: WireValue): WireValue
    fun combine(other: WireValue?): WireValue
    fun and(other: WireValue?): WireValue
    fun or(other: WireValue?): WireValue
    fun xor(other: WireValue?): WireValue
    fun not(): WireValue

    fun toIntValue(): Int
    fun toOctalString(): String
    fun toHexString(): String
    fun toDecimalString(signed: Boolean): String
    fun toDisplayString(radix: Int): String
    fun toBinString(): String
    fun getColor(): Color
}
