using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetAvatarAssetsAsync
{
	public AssetLoader m_Api;

	public AvatarManifest m_Manifest;

	public AvatarComponentMasks m_ComponentMask;

	public EventHandler<GetAvatarAssetsEventArgs> m_EventHandler;

	public GetAvatarAssetsAsync(AssetLoader api, AvatarManifest manifest, AvatarComponentMasks componentMask, EventHandler<GetAvatarAssetsEventArgs> eventHandler)
	{
		m_Api = api;
		m_Manifest = manifest;
		m_ComponentMask = componentMask;
		m_EventHandler = eventHandler;
	}

	public void Process()
	{
		GetAvatarAssetsEventArgs e = new GetAvatarAssetsEventArgs();
		try
		{
			e.Avatar = m_Api.CreateAvatar(m_Manifest, m_ComponentMask);
		}
		catch (AvatarException ex)
		{
			AvatarException ex2 = (e.Exception = ex);
			AvatarException ex4 = ex2;
		}
		m_EventHandler(m_Manifest, e);
	}
}
