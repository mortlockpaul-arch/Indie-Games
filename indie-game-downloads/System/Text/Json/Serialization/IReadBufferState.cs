using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json.Serialization;

internal interface IReadBufferState<TReadBufferState, TStream> : IDisposable where TReadBufferState : struct, IReadBufferState<TReadBufferState, TStream>
{
	bool IsFinalBlock { get; }

	ValueTask<TReadBufferState> ReadAsync(TStream utf8Json, CancellationToken cancellationToken, bool fillBuffer = true);

	void Read(TStream utf8Json);

	void Advance(long bytesConsumed);

	void GetReader(JsonReaderState jsonReaderState, out Utf8JsonReader reader);
}
