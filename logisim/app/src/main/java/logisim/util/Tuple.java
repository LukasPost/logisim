package logisim.util;

public class Tuple<TFirst, TSecond> {
	public TFirst first;
	public TSecond second;

	public Tuple(TFirst first, TSecond second) {
		this.first = first;
		this.second = second;
	}
}
