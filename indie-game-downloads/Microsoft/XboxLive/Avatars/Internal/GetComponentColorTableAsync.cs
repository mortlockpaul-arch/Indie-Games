using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetComponentColorTableAsync
{
	public AssetLoader m_Api;

	public Guid m_ComponentId;

	public EventHandler<GetComponentColorTableEventArgs> m_EventHandler;

	public GetComponentColorTableAsync(AssetLoader api, Guid componentId, EventHandler<GetComponentColorTableEventArgs> eventHandler)
	{
		m_Api = api;
		m_ComponentId = componentId;
		m_EventHandler = eventHandler;
	}

	public void Process()
	{
		GetComponentColorTableEventArgs e = new GetComponentColorTableEventArgs();
		try
		{
			e.AssetId = m_ComponentId;
			e.m_CustomColors = m_Api.GetComponentColorTable(m_ComponentId);
		}
		catch (AvatarException ex)
		{
			AvatarException ex2 = (e.Exception = ex);
			AvatarException ex4 = ex2;
		}
		m_EventHandler(m_ComponentId, e);
	}
}
