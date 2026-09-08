using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.XboxLive.Avatars.Internal;

public class DataRequestCompletedEventArgs : AsyncCompletedEventArgs
{
	public Stream Result { get; set; }

	public string SourceAddress { get; set; }

	public DataRequestCompletedEventArgs(Exception error, bool canceled, object userState)
		: base(error, canceled, userState)
	{
	}
}
