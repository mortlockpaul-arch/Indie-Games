using System;
using System.Text;

namespace Quasar.GameUtils.Network;

public abstract class Session : IDisposable
{
	protected ISessionProperties sessionProperties;

	public abstract SessionState SessionState { get; }

	public abstract float SimulatedLatency { get; set; }

	public abstract float SimulatedPacketLoss { get; set; }

	public abstract int AllGamersCount { get; }

	public abstract int MaxGamers { get; }

	public abstract int LocalGamersCount { get; }

	public abstract bool IsEveryoneReady { get; }

	public abstract bool IsHost { get; }

	public abstract INetworkGamer Host { get; }

	public abstract bool AllowHostMigration { get; set; }

	public abstract bool AllowJoinInProgress { get; set; }

	public ISessionProperties SessionProperties => sessionProperties;

	public event Action<object, GamerLeftEventArgs> GamerLeft;

	public event Action<object, GamerJoinedEventArgs> GamerJoined;

	public event Action<object, GameStartedEventArgs> GameStarted;

	public event Action<object, GameEndedEventArgs> GameEnded;

	public event Action<object, HostChangedEventArgs> HostChanged;

	public event Action<object, SessionEndedEventArgs> SessionEnded;

	protected void OnGamerLeft(GamerLeftEventArgs args)
	{
		if (GamerLeft != null)
		{
			GamerLeft(this, args);
		}
	}

	protected void OnGamerJoined(GamerJoinedEventArgs args)
	{
		if (GamerJoined != null)
		{
			GamerJoined(this, args);
		}
	}

	protected void OnGameStarted(GameStartedEventArgs args)
	{
		if (GameStarted != null)
		{
			GameStarted(this, args);
		}
	}

	protected void OnGameEnded(GameEndedEventArgs args)
	{
		if (GameEnded != null)
		{
			GameEnded(this, args);
		}
	}

	protected void OnHostChanged(HostChangedEventArgs args)
	{
		if (HostChanged != null)
		{
			HostChanged(this, args);
		}
	}

	protected void OnSessionEnded(SessionEndedEventArgs args)
	{
		if (SessionEnded != null)
		{
			SessionEnded(this, args);
		}
	}

	public abstract IPacketWriter CreatePacketWriter();

	public abstract IPacketReader CreatePacketReader();

	public abstract INetworkGamer GetGamer(int index);

	public abstract ILocalNetworkGamer GetLocalGamer(int index);

	public abstract INetworkGamer FindGamerById(byte id);

	public abstract void StartGame();

	public abstract void EndGame();

	public abstract void Update();

	public abstract void GetHostAddress(StringBuilder builder);

	public virtual void Dispose()
	{
	}
}
