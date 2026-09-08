namespace System.Security.Cryptography;

internal static class PemEnumerator
{
	internal static PemEnumerator<char> Utf16(ReadOnlySpan<char> pemData)
	{
		return new PemEnumerator<char>(pemData, PemEncoding.TryFind);
	}
}
internal readonly ref struct PemEnumerator<TChar>
{
	internal delegate bool TryFindFunc(ReadOnlySpan<TChar> pemData, out PemFields fields);

	internal ref struct Enumerator
	{
		internal readonly ref struct PemFieldItem(ReadOnlySpan<TChar> contents, PemFields pemFields)
		{
			private readonly ReadOnlySpan<TChar> _contents = contents;

			private readonly PemFields _pemFields = pemFields;

			public void Deconstruct(out ReadOnlySpan<TChar> contents, out PemFields pemFields)
			{
				contents = _contents;
				pemFields = _pemFields;
			}
		}

		private ReadOnlySpan<TChar> _contents;

		private PemFields _pemFields;

		private readonly TryFindFunc _tryFindFunc;

		public readonly PemFieldItem Current => new PemFieldItem(_contents, _pemFields);

		internal Enumerator(ReadOnlySpan<TChar> contents, TryFindFunc tryFindFunc)
		{
			_contents = contents;
			_pemFields = default(PemFields);
			_tryFindFunc = tryFindFunc;
		}

		public bool MoveNext()
		{
			ref ReadOnlySpan<TChar> contents = ref _contents;
			Index end = _pemFields.Location.End;
			int length = contents.Length;
			int offset = end.GetOffset(length);
			_contents = contents.Slice(offset, length - offset);
			return _tryFindFunc(_contents, out _pemFields);
		}
	}

	private readonly ReadOnlySpan<TChar> _contents;

	private readonly TryFindFunc _tryFindFunc;

	internal PemEnumerator(ReadOnlySpan<TChar> contents, TryFindFunc findFunc)
	{
		_contents = contents;
		_tryFindFunc = findFunc;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(_contents, _tryFindFunc);
	}
}
