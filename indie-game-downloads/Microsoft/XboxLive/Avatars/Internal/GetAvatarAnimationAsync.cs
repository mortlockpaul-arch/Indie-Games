using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetAvatarAnimationAsync
{
	public AssetLoader m_Api;

	public Guid m_AnimationId;

	public EventHandler<GetAvatarAnimationEventArgs> m_EventHandler;

	public GetAvatarAnimationAsync(AssetLoader api, Guid animationId, EventHandler<GetAvatarAnimationEventArgs> eventHandler)
	{
		m_Api = api;
		m_AnimationId = animationId;
		m_EventHandler = eventHandler;
	}

	public void Process()
	{
		GetAvatarAnimationEventArgs e = new GetAvatarAnimationEventArgs();
		try
		{
			e.Animation = m_Api.GetAvatarAnimation(m_AnimationId);
		}
		catch (AvatarException ex)
		{
			AvatarException ex2 = (e.Exception = ex);
			AvatarException ex4 = ex2;
		}
		m_EventHandler(m_AnimationId, e);
	}
}
