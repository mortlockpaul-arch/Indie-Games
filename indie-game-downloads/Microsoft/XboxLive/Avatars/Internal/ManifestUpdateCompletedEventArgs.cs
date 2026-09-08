using System;
using System.ComponentModel;

namespace Microsoft.XboxLive.Avatars.Internal;

public class ManifestUpdateCompletedEventArgs : AsyncCompletedEventArgs
{
	public bool m_updateStatus;

	public bool UpdateStatus
	{
		get
		{
			RaiseExceptionIfNecessary();
			return m_updateStatus;
		}
	}

	public ManifestUpdateCompletedEventArgs(Exception e, bool updateStatus)
		: base(e, cancelled: false, null)
	{
		m_updateStatus = updateStatus;
	}
}
