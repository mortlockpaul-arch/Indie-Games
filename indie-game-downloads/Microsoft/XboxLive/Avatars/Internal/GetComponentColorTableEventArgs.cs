using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetComponentColorTableEventArgs : EventArgs
{
	public ComponentColors[] m_CustomColors;

	public Guid AssetId { get; set; }

	public AvatarException Exception { get; set; }

	public ComponentColors[] GetCustomColors()
	{
		return m_CustomColors;
	}
}
