using System;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetAvatarComponentAsync
{
	public AssetLoader m_Api;

	public Guid m_ComponentId;

	public EventHandler<GetAvatarComponentEventArgs> m_EventHandler;

	public Colorb[] m_CustomColors;

	public GetAvatarComponentAsync(AssetLoader api, Guid componentId, Colorb[] customColors, EventHandler<GetAvatarComponentEventArgs> eventHandler)
	{
		m_Api = api;
		m_ComponentId = componentId;
		m_EventHandler = eventHandler;
		if (customColors != null)
		{
			m_CustomColors = new Colorb[customColors.Length];
			customColors.CopyTo(m_CustomColors, 0);
		}
	}

	public void Process()
	{
		GetAvatarComponentEventArgs e = new GetAvatarComponentEventArgs();
		try
		{
			e.Component = m_Api.GetAvatarComponent(m_ComponentId, m_CustomColors);
		}
		catch (AvatarException ex)
		{
			AvatarException ex2 = (e.Exception = ex);
			AvatarException ex4 = ex2;
		}
		m_EventHandler(m_ComponentId, e);
	}
}
