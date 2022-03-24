namespace Tool.Compet.Core {
	using System.Threading;

	public class DkAtomicBool {
		/// True: != 0, False: == 0
		private long rawValue;

		public bool value => Interlocked.Read(ref rawValue) != 0;

		public bool Set(bool value) {
			if (value) {
				return Interlocked.Or(ref rawValue, 1) != 0;
			}
			return Interlocked.And(ref rawValue, 0) == 0;
		}
	}

	public class DkAtomicInt {
		private long rawValue;

		public int value => (int)Interlocked.Read(ref rawValue);

		public int Increment() {
			return (int)Interlocked.Increment(ref rawValue);
		}

		public int Decrement() {
			return (int)Interlocked.Decrement(ref rawValue);
		}

		public int Add(int more) {
			return (int)Interlocked.Add(ref rawValue, more);
		}
	}

	public class DkAtomicLong {
		private long rawValue;

		public long value => Interlocked.Read(ref rawValue);

		public long Increment() {
			return Interlocked.Increment(ref rawValue);
		}

		public long Decrement() {
			return Interlocked.Decrement(ref rawValue);
		}

		public long Add(long more) {
			return Interlocked.Add(ref rawValue, more);
		}
	}
}
