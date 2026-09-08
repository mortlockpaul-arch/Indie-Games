using System;
using System.ComponentModel;

namespace Microsoft.XboxLive.Avatars.Internal;

public class CreateRandomManifestCompletedEventArgs : AsyncCompletedEventArgs
{
	public AvatarManifest[] m_randomManifests;

	public AvatarManifest[] RandomManifests
	{
		get
		{
			RaiseExceptionIfNecessary();
			return m_randomManifests;
		}
	}

	public CreateRandomManifestCompletedEventArgs(Exception e, AvatarManifest[] randomManifests)
		: base(e, cancelled: false, null)
	{
		m_randomManifests = randomManifests;
	}
}
