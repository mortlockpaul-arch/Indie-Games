using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public class DownloadProgressEventArgs : EventArgs
{
	public int TotalNumberOfFilesToDownload { get; set; }

	public int NumberOfDownloadingFiles { get; set; }

	public int ProgressPercentage { get; set; }

	public long BytesReceived { get; set; }

	public long TotalBytesToReceive { get; set; }
}
