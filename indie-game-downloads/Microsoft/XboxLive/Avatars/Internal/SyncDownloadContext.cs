using System;
using System.IO;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public class SyncDownloadContext : IDisposable
{
	public AutoResetEvent syncEvent = new AutoResetEvent(initialState: false);

	public Stream stream;

	public Exception error;

	public Stream DetachStream()
	{
		Stream result = stream;
		stream = null;
		return result;
	}

	public void Dispose(bool disposing)
	{
		if (disposing)
		{
			syncEvent.Close();
			if (stream != null)
			{
				stream.Dispose();
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
