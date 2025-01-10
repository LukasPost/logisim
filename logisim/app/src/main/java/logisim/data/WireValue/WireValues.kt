package logisim.data.WireValue;

enum class WireValues(private val internalValue: CustomWireValue) : WireValue by internalValue {
	FALSE(CustomWireValue(1, 0, 0, 0)),
	TRUE(CustomWireValue(1, 0, 0, 1)),
	UNKNOWN(CustomWireValue(1, 0, 1, 0)),
	ERROR(CustomWireValue(1, 1, 0, 0)),
	NIL(CustomWireValue(0, 0, 0, 0));

	fun equals(other:WireValue):Boolean {
		if (other is WireValues)
			return this == other
		return internalValue == other
	}
	override fun toString(): String {
		return internalValue.toString()
	}
}