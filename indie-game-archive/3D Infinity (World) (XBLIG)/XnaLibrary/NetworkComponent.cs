using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace XnaLibrary;

public class NetworkComponent : GameComponent
{
	private NetworkSession session;

	public NetworkSession Session
	{
		get
		{
			return session;
		}
		set
		{
			session = value;
			InitializeEvents(session);
		}
	}

	public event EventHandler<GameEndedEventArgs> GameEnded;

	public event EventHandler<GamerJoinedEventArgs> GamerJoined;

	public event EventHandler<GamerLeftEventArgs> GamerLeft;

	public event EventHandler<GameStartedEventArgs> GameStarted;

	public event EventHandler<HostChangedEventArgs> HostChanged;

	public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

	public NetworkComponent(Game game)
		: base(game)
	{
	}

	private void InitializeEvents(NetworkSession networkSession)
	{
		if (networkSession != null)
		{
			networkSession.GameEnded += networkSession_GameEnded;
			networkSession.GamerJoined += networkSession_GamerJoined;
			networkSession.GamerLeft += networkSession_GamerLeft;
			networkSession.GameStarted += networkSession_GameStarted;
			networkSession.HostChanged += networkSession_HostChanged;
			networkSession.SessionEnded += networkSession_SessionEnded;
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (Session != null && !Session.IsDisposed)
		{
			Session.Update();
		}
		((GameComponent)this).Update(gameTime);
	}

	private void networkSession_GameEnded(object sender, GameEndedEventArgs e)
	{
		if (GameEnded != null)
		{
			GameEnded(sender, e);
		}
	}

	private void networkSession_GamerJoined(object sender, GamerJoinedEventArgs e)
	{
		if (GamerJoined != null)
		{
			GamerJoined(sender, e);
		}
	}

	private void networkSession_GamerLeft(object sender, GamerLeftEventArgs e)
	{
		if (GamerLeft != null)
		{
			GamerLeft(sender, e);
		}
	}

	private void networkSession_GameStarted(object sender, GameStartedEventArgs e)
	{
		if (GameStarted != null)
		{
			GameStarted(sender, e);
		}
	}

	private void networkSession_HostChanged(object sender, HostChangedEventArgs e)
	{
		if (HostChanged != null)
		{
			HostChanged(sender, e);
		}
	}

	private void networkSession_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
	{
		if (SessionEnded != null)
		{
			SessionEnded(sender, e);
		}
	}

	public void CloseSession()
	{
		if (Session != null && !Session.IsDisposed)
		{
			Session.Dispose();
		}
		Session = null;
	}

	public void RemoveAllEvents()
	{
		GameEnded = null;
		GamerJoined = null;
		GamerLeft = null;
		GameStarted = null;
		HostChanged = null;
		SessionEnded = null;
	}
}
