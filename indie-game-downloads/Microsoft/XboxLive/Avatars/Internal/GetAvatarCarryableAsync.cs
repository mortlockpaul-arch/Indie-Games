using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetAvatarCarryableAsync
{
	public AssetLoader m_Api;

	public Guid m_CarryableId;

	public EventHandler<GetAvatarCarryableEventArgs> m_EventHandler;

	public Colorb[] m_CustomColors;

	public Skeleton m_AvatarSkeleton;

	public GetAvatarCarryableAsync(AssetLoader api, Guid carryableId, Skeleton avatarSkeleton, Colorb[] customColors, EventHandler<GetAvatarCarryableEventArgs> eventHandler)
	{
		m_Api = api;
		m_CarryableId = carryableId;
		m_EventHandler = eventHandler;
		m_AvatarSkeleton = avatarSkeleton;
		if (customColors != null)
		{
			m_CustomColors = new Colorb[customColors.Length];
			customColors.CopyTo(m_CustomColors, 0);
		}
	}

	public void Process()
	{
		GetAvatarCarryableEventArgs e = new GetAvatarCarryableEventArgs();
		try
		{
			e.Carryable = m_Api.GetAvatarCarryable(m_CarryableId, m_AvatarSkeleton, m_CustomColors);
		}
		catch (AvatarException ex)
		{
			AvatarException ex2 = (e.Exception = ex);
			AvatarException ex4 = ex2;
		}
		m_EventHandler(m_CarryableId, e);
	}
}
