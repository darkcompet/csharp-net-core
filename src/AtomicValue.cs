namespace Tool.Compet.Core;

using System.Threading;

public class DkAtomicBool {
	/// True: != 0, False: == 0
	private long rawValue;

	public bool value => Interlocked.Read(ref this.rawValue) != 0;

	public bool Set(bool value) {
		if (value) {
			return Interlocked.Or(ref this.rawValue, 1) != 0;
		}
		return Interlocked.And(ref this.rawValue, 0) == 0;
	}
}

public class DkAtomicInt {
	private long rawValue;

	public int value => (int)Interlocked.Read(ref this.rawValue);

	public int Increment() {
		return (int)Interlocked.Increment(ref this.rawValue);
	}

	public int Decrement() {
		return (int)Interlocked.Decrement(ref this.rawValue);
	}

	public int Add(int more) {
		return (int)Interlocked.Add(ref this.rawValue, more);
	}
}

public class DkAtomicLong {
	private long rawValue;

	public long value => Interlocked.Read(ref this.rawValue);

	public long Increment() {
		return Interlocked.Increment(ref this.rawValue);
	}

	public long Decrement() {
		return Interlocked.Decrement(ref this.rawValue);
	}

	public long Add(long more) {
		return Interlocked.Add(ref this.rawValue, more);
	}
}
