using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace Maximinus.DebugTools;

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

	private bool IsHost = true;

	private Regex packetRe = new Regex("\\$(?<header>[^$]+)\\$:(?<text>.+)");

	private PacketReader packetReader = new PacketReader();

	private PacketWriter packetWriter = new PacketWriter();

	private IAsyncResult asyncResult;

	private ConnectionPahse phase;

	public NetworkSession NetworkSession { get; set; }

	public bool OwnsNetworkSession { get; private set; }

	public RemoteDebugCommand(Game game)
		: base(game)
	{
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
				GamerServicesDispatcher.WindowHandle = base.Game.Window.Handle;
				NetworkSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2);
				OwnsNetworkSession = true;
			}
		}
		base.Initialize();
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
		switch (phase)
		{
		case ConnectionPahse.EnsureSignedIn:
			GamerServicesDispatcher.Update();
			break;
		case ConnectionPahse.FindSessions:
			GamerServicesDispatcher.Update();
			if (asyncResult.IsCompleted)
			{
				AvailableNetworkSessionCollection availableNetworkSessionCollection = NetworkSession.EndFind(asyncResult);
				if (availableNetworkSessionCollection.Count > 0)
				{
					asyncResult = NetworkSession.BeginJoin(availableNetworkSessionCollection[0], null, null);
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
				foreach (LocalNetworkGamer localGamer in NetworkSession.LocalGamers)
				{
					while (localGamer.IsDataAvailable)
					{
						localGamer.ReceiveData(packetReader, out var sender);
						if (!sender.IsLocal)
						{
							ProcessRecievedPacket(packetReader.ReadString());
						}
					}
				}
			}
		}
		base.Update(gameTime);
	}

	private void SendPacket(string header, string text)
	{
		if (NetworkSession != null)
		{
			packetWriter.Write("$" + header + "$:" + text);
			NetworkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
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
			debugCommandUI.Prompt = "CMD>";
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
				GamerServicesDispatcher.WindowHandle = base.Game.Window.Handle;
				GamerServicesDispatcher.Initialize(base.Game.Services);
			}
			catch
			{
			}
			if (Gamer.SignedInGamers.Count > 0)
			{
				commandHost.Echo("Finding available sessions...");
				asyncResult = NetworkSession.BeginFind(NetworkSessionType.SystemLink, 1, null, null, null);
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
