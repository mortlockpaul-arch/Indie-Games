using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace DebugSample;

public class RemoteDebugCommand : GameComponent, IDebugCommandExecutioner, IDebugEchoListner
{
	private enum ConnectionPahse
	{
		None,
		EnsureSignedIn,
		FindSessions,
		Joining
	}

	private const string StartPacketHeader = "RmtStart";

	private const string ExecutePacketHeader = "RmtCmd";

	private const string EchoPacketHeader = "RmtEcho";

	private const string ErrorPacketHeader = "RmtErr";

	private const string WarningPacketHeader = "RmtWrn";

	private const string QuitPacketHeader = "RmtQuit";

	private IDebugCommandHost commandHost;

	private bool IsHost;

	private Regex packetRe;

	private PacketReader packetReader;

	private PacketWriter packetWriter;

	private IAsyncResult asyncResult;

	private ConnectionPahse phase;

	public NetworkSession NetworkSession { get; set; }

	public bool OwnsNetworkSession { get; private set; }

	public RemoteDebugCommand(Game game)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		IsHost = true;
		packetRe = new Regex("\\$(?<header>[^$]+)\\$:(?<text>.+)");
		packetReader = new PacketReader();
		packetWriter = new PacketWriter();
		((GameComponent)this)._002Ector(game);
		commandHost = game.Services.GetService(typeof(IDebugCommandHost)) as IDebugCommandHost;
		if (!IsHost)
		{
			commandHost.RegisterCommand("remote", "Start remote command", ExecuteRemoteCommand);
		}
	}

	public override void Initialize()
	{
		if (IsHost)
		{
			commandHost.RegisterEchoListner(this);
			if (NetworkSession == null)
			{
				GamerServicesDispatcher.WindowHandle = ((GameComponent)this).Game.Window.Handle;
				GamerServicesDispatcher.Initialize((IServiceProvider)((GameComponent)this).Game.Services);
				NetworkSession = NetworkSession.Create((NetworkSessionType)1, 1, 2);
				OwnsNetworkSession = true;
			}
		}
		((GameComponent)this).Initialize();
	}

	public bool ProcessRecievedPacket(string packetString)
	{
		bool result = false;
		Match match = packetRe.Match(packetString);
		if (match.Success)
		{
			string value = match.Groups["header"].Value;
			string value2 = match.Groups["text"].Value;
			switch (value)
			{
			case "RmtCmd":
				commandHost.ExecuteCommand(value2);
				result = true;
				break;
			case "RmtEcho":
				commandHost.Echo(value2);
				result = true;
				break;
			case "RmtErr":
				commandHost.EchoError(value2);
				result = true;
				break;
			case "RmtWrn":
				commandHost.EchoWarning(value2);
				result = true;
				break;
			case "RmtStart":
				ConnectedToRemote();
				commandHost.Echo(value2);
				result = true;
				break;
			case "RmtQuit":
				commandHost.Echo(value2);
				DisconnectedFromRemote();
				result = true;
				break;
			}
		}
		return result;
	}

	public override void Update(GameTime gameTime)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		switch (phase)
		{
		case ConnectionPahse.EnsureSignedIn:
			GamerServicesDispatcher.Update();
			break;
		case ConnectionPahse.FindSessions:
			GamerServicesDispatcher.Update();
			if (asyncResult.IsCompleted)
			{
				AvailableNetworkSessionCollection val = NetworkSession.EndFind(asyncResult);
				if (((ReadOnlyCollection<AvailableNetworkSession>)(object)val).Count > 0)
				{
					asyncResult = NetworkSession.BeginJoin(((ReadOnlyCollection<AvailableNetworkSession>)(object)val)[0], (AsyncCallback)null, (object)null);
					commandHost.EchoError("Connecting to the host...");
					phase = ConnectionPahse.Joining;
				}
				else
				{
					commandHost.EchoError("Couldn't find a session.");
					phase = ConnectionPahse.None;
				}
			}
			break;
		case ConnectionPahse.Joining:
			GamerServicesDispatcher.Update();
			if (asyncResult.IsCompleted)
			{
				NetworkSession = NetworkSession.EndJoin(asyncResult);
				NetworkSession.SessionEnded += NetworkSession_SessionEnded;
				OwnsNetworkSession = true;
				commandHost.EchoError("Connected to the host.");
				phase = ConnectionPahse.None;
				asyncResult = null;
				ConnectedToRemote();
			}
			break;
		}
		if (OwnsNetworkSession)
		{
			GamerServicesDispatcher.Update();
			NetworkSession.Update();
			if (NetworkSession != null)
			{
				GamerCollectionEnumerator<LocalNetworkGamer> enumerator = NetworkSession.LocalGamers.GetEnumerator();
				try
				{
					NetworkGamer val2 = default(NetworkGamer);
					while (enumerator.MoveNext())
					{
						LocalNetworkGamer current = enumerator.Current;
						while (current.IsDataAvailable)
						{
							current.ReceiveData(packetReader, ref val2);
							if (!val2.IsLocal)
							{
								ProcessRecievedPacket(((BinaryReader)(object)packetReader).ReadString());
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		((GameComponent)this).Update(gameTime);
	}

	private void SendPacket(string header, string text)
	{
		if (NetworkSession != null)
		{
			((BinaryWriter)(object)packetWriter).Write("$" + header + "$:" + text);
			((ReadOnlyCollection<LocalNetworkGamer>)(object)NetworkSession.LocalGamers)[0].SendData(packetWriter, (SendDataOptions)3);
		}
	}

	private void ConnectedToRemote()
	{
		DebugCommandUI debugCommandUI = commandHost as DebugCommandUI;
		if (IsHost)
		{
			if (debugCommandUI != null)
			{
				debugCommandUI.Prompt = "[Host]>";
			}
		}
		else
		{
			if (debugCommandUI != null)
			{
				debugCommandUI.Prompt = "[Client]>";
			}
			commandHost.PushExecutioner(this);
			SendPacket("RmtStart", "Remote Debug Command Started!!");
		}
		commandHost.RegisterCommand("quit", "Quit from remote command", ExecuteQuitCommand);
	}

	private void DisconnectedFromRemote()
	{
		if (commandHost is DebugCommandUI debugCommandUI)
		{
			debugCommandUI.Prompt = debugCommandUI.DefaultPrompt;
		}
		commandHost.UnregisterCommand("quit");
		if (!IsHost)
		{
			commandHost.PopExecutioner();
			if (OwnsNetworkSession)
			{
				NetworkSession.Dispose();
				NetworkSession = null;
				OwnsNetworkSession = false;
			}
		}
	}

	private void ExecuteRemoteCommand(IDebugCommandHost host, string command, IList<string> arguments)
	{
		if (NetworkSession == null)
		{
			try
			{
				GamerServicesDispatcher.WindowHandle = ((GameComponent)this).Game.Window.Handle;
				GamerServicesDispatcher.Initialize((IServiceProvider)((GameComponent)this).Game.Services);
			}
			catch
			{
			}
			if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 0)
			{
				commandHost.Echo("Finding available sessions...");
				asyncResult = NetworkSession.BeginFind((NetworkSessionType)1, 1, (NetworkSessionProperties)null, (AsyncCallback)null, (object)null);
				phase = ConnectionPahse.FindSessions;
			}
			else
			{
				host.Echo("Please signed in.");
				phase = ConnectionPahse.EnsureSignedIn;
			}
		}
		else
		{
			ConnectedToRemote();
		}
	}

	private void ExecuteQuitCommand(IDebugCommandHost host, string command, IList<string> arguments)
	{
		SendPacket("RmtQuit", "End Remote Debug Command.");
		DisconnectedFromRemote();
	}

	public void ExecuteCommand(string command)
	{
		SendPacket("RmtCmd", command);
	}

	public void Echo(DebugCommandMessage messageType, string text)
	{
		switch (messageType)
		{
		case DebugCommandMessage.Standard:
			SendPacket("RmtEcho", text);
			break;
		case DebugCommandMessage.Warning:
			SendPacket("RmtWrn", text);
			break;
		case DebugCommandMessage.Error:
			SendPacket("RmtErr", text);
			break;
		}
	}

	private void NetworkSession_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
	{
		DisconnectedFromRemote();
		commandHost.EchoWarning("Disconnected from the Host.");
	}
}
