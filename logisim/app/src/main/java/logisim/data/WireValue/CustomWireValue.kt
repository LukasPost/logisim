package logisim.data.WireValue

import logisim.data.BitWidth
import java.awt.Color


class CustomWireValue(
    override val width: Int,
    override val error: Int,
    override val unknown: Int,
    override val value: Int
) : WireValue {

    init {
        // Ensure one-bit values are unique
        require(width >= 0) { "Width must be non-negative" }
    }

    override val bitWidth: BitWidth
        get() = BitWidth.create(width)

    override fun equals(other: Any?): Boolean {
        return other is WireValue &&
            width == other.width &&
            error == other.error &&
            unknown == other.unknown &&
            value == other.value
    }

    override fun hashCode(): Int {
        var result = width
        result = 31 * result + error
        result = 31 * result + unknown
        result = 31 * result + value
        return result
    }

    override val isErrorValue: Boolean
        get() = error != 0

    override val isUnknown: Boolean
        get() = when (width) {
            32 -> error == 0 && unknown == -1
            else -> error == 0 && unknown == ((1 shl width) - 1)
        }

    override val isFullyDefined: Boolean
        get() = width > 0 && error == 0 && unknown == 0

    override val allValues: Array<WireValue>
        get() = Array(width) { get(it) }

    override fun get(which: Int): WireValue {
        if (which !in 0 until width)
            return WireValues.ERROR
        val mask = 1 shl which
        return when {
            (error and mask) != 0 -> WireValues.ERROR
            (unknown and mask) != 0 -> WireValues.UNKNOWN
            (value and mask) != 0 -> WireValues.TRUE
            else -> WireValues.FALSE
        }
    }

    override fun getAll(): Array<WireValue> {
        return Array(width) { i -> get(i) }
    }

    override fun extendWidth(newWidth: Int, others: WireValue): WireValue {
        if (width == newWidth)
            return this
        val maskInverse = if (width == 32) 0 else (-1 shl width)
        return when (others) {
            WireValues.ERROR -> WireValue.create(newWidth, error or maskInverse, unknown, value)
            WireValues.FALSE -> WireValue.create(newWidth, error, unknown, value)
            WireValues.TRUE -> WireValue.create(newWidth, error, unknown, value or maskInverse)
            else -> WireValue.create(newWidth, error, unknown or maskInverse, value)
        }
    }

    override fun set(which: Int, value: WireValue): WireValue {
        require(value.width == 1) { "Cannot set multiple values" }
        require(which in 0 until width) { "Attempt to set outside value's width" }
        if (width == 1)
            return value
        val mask = (1 shl which).inv()
        return WireValue.create(
            width,
            (error and mask) or (value.error shl which),
            (unknown and mask) or (value.unknown shl which),
            (this.value and mask) or (value.value shl which)
        )
    }

    override fun combine(other: WireValue?): WireValue {
        if (other == null || other == WireValues.NIL)
            return this
        if (this.equals(WireValues.NIL))
            return other

        if (width == 1 && other.width == 1) {
            return when {
                this == other -> this
                this.equals(WireValues.UNKNOWN) -> other
                other == WireValues.UNKNOWN -> this
                else -> WireValues.ERROR
            }
        }

        val disagree = (value xor other.value) and (unknown or other.unknown).inv()
        return WireValue.create(
            maxOf(width, other.width),
            error or other.error or disagree,
            unknown and other.unknown,
            (value and unknown.inv()) or (other.value and other.unknown.inv())
        )
    }

    override fun and(other: WireValue?): WireValue {
        if (other == null)
            return this
        val false0 = value.inv() and error.inv() and unknown.inv()
        val false1 = other.value.inv() and other.error.inv() and other.unknown.inv()
        val falses = false0 or false1
        return WireValue.create(
            maxOf(width, other.width),
            (error or other.error or unknown or other.unknown) and falses.inv(),
            0,
            value and other.value
        )
    }

    override fun or(other: WireValue?): WireValue {
        if (other == null)
            return this
        val true0 = value and error.inv() and unknown.inv()
        val true1 = other.value and other.error.inv() and other.unknown.inv()
        val trues = true0 or true1
        return WireValue.create(
            maxOf(width, other.width),
            (error or other.error or unknown or other.unknown) and trues.inv(),
            0,
            value or other.value
        )
    }

    override fun xor(other: WireValue?): WireValue {
        if (other == null)
            return this
        return WireValue.create(
            maxOf(width, other.width),
            error or other.error or unknown or other.unknown,
            0,
            value xor other.value
        )
    }

    override fun not(): WireValue {
        return WireValue.create(width, error or unknown, 0, value.inv())
    }

    override fun toIntValue(): Int {
        return when {
            error != 0 || unknown != 0 -> -1
            else -> value
        }
    }
    override fun toBinString(): String {
        return when (width) {
            0 -> "-"
            1 -> when {
                error != 0 -> "E"
                unknown != 0 -> "x"
                value != 0 -> "1"
                else -> "0"
            }
            else -> buildString {
                for (i in width - 1 downTo 0) {
                    append(get(i).toString())
                    if (i % 4 == 0 && i != 0)
                        append(" ")
                }
            }
        }
    }
    override fun toOctalString(): String = toDisplayStringFromRadix(8)
    override fun toDecimalString(signed: Boolean): String {
        return when {
            width == 0 -> "-"
            isErrorValue -> Strings.get("valueError")
            !isFullyDefined -> Strings.get("valueUnknown")
            !signed -> "" + (toIntValue().toLong() and 0xFFFFFFFFL)
            else -> {
                var value = toIntValue()
                if (width < 32 && (value shr (width - 1)) != 0)
                    value = value or ((-1) shl width)
                "" + value
            }
        }
    }
    override fun toHexString(): String = toDisplayStringFromRadix(16)

    private fun toDisplayStringFromRadix(radix : Int): String {
        if (width <= 1)
            return toString()
        val bitsPerRound = when(radix) {
            2 -> 1
            4 -> 2
            8 -> 3
            16 -> 4
            else -> throw RuntimeException("Radix must be 2, 4, 8 or 16")
        }
        var vals = getAll().toList()
        val sb = StringBuilder()
        while (vals.isNotEmpty()) {
            val value = WireValue.create(vals.take(bitsPerRound))
            vals = vals.drop(bitsPerRound)
            val charToAppend: Char = when {
                value.isErrorValue -> 'E'
                !value.isFullyDefined -> 'x'
                else -> Character.forDigit(value.value, radix)
            }
            sb.append(charToAppend)
        }
        return sb.toString()
    }

    override fun toDisplayString(radix: Int): String {
        return when {
            width == 0 -> "-"
            radix == 2 -> toBinString()
            radix == 4 -> toDisplayStringFromRadix(4)
            radix == 8 -> toDisplayStringFromRadix(8)
            radix == 16 -> toDisplayStringFromRadix(16)
            else -> toIntValue().toString(radix.coerceIn(2, 36))
        }
    }

    override fun getColor(): Color {
        return when {
            error != 0 -> WireValue.ERROR_COLOR
            width == 0 -> WireValue.NIL_COLOR
            width != 1 -> WireValue.MULTI_COLOR
            this.equals(WireValues.UNKNOWN) -> WireValue.UNKNOWN_COLOR
            this.equals(WireValues.TRUE) -> WireValue.TRUE_COLOR
            else -> WireValue.FALSE_COLOR
        }
    }
}
