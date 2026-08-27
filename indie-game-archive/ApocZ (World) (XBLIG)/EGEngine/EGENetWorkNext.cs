using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class EGENetWorkNext
{
	public const int maxNetGamers = 15;

	public const int maxLocalGamers = 1;

	public static bool inviteToGameScheduled = false;

	public static NetworkSession networkSession = null;

	public static PacketWriter packetWriter = new PacketWriter();

	public static PacketReader packetReader = new PacketReader();

	public static ePacketTypes LastPacketRead = ePacketTypes.DamageData;

	public static IAsyncResult asyncResultJoin = null;

	public static IAsyncResult asyncResultFind = null;

	public static bool NetPlayersInitialized = false;

	public static PlayerBase[] NetPlayers = new PlayerBase[15];

	private static int currentValidListIndex = 0;

	private static List<MyNetworkSessionEntry>[] MyAvailableSessions;

	private static AvailableNetworkSessionCollection availableSessions;

	private static NetworkSessionProperties sessionProperties = new NetworkSessionProperties();

	private static string errorMessage;

	public static float HostMigrateTimer = 0f;

	public static float InSessionTimer = 0f;

	public static bool HostCreatingWorld = false;

	private static HalfVector2 readPHV2 = default(HalfVector2);

	private static HalfVector4 readPHV4 = default(HalfVector4);

	private static NormalizedByte4 readNB4_0 = default(NormalizedByte4);

	private static NormalizedByte4 readNB4_1 = default(NormalizedByte4);

	private static Vector2 readUnpackerV2 = Vector2.Zero;

	private static Vector4 readUnpackerV4 = Vector4.Zero;

	private static Cue FacePunchSound;

	private static int readDataBufferSize = 32768;

	private static byte[] readDataBuffer = new byte[readDataBufferSize];

	private static bool initialized = false;

	public static bool JoinInviteInProgress = false;

	public static float NetworkCurrentPing = 1000f;

	private static float NetworkPingTimer = 0f;

	private static float NetworkUpdateTimeStep = 0.5f;

	private static float LocalPlayerUpdateTimer = 0f;

	private static float ServerTransmitUpdateTimer = 0f;

	private static int ClientUpdateTimer = 0;

	private static int ServerUpdateTimer = 0;

	private static Vector4 readDir = Vector4.Zero;

	private static Vector2 readSpeedSteer = Vector2.Zero;

	private static Vector3 direction = Vector3.Zero;

	public static List<MyNetworkSessionEntry> GetAvailableSessions()
	{
		return MyAvailableSessions[currentValidListIndex];
	}

	private static void Init()
	{
		initialized = true;
		MyAvailableSessions = new List<MyNetworkSessionEntry>[2];
		MyAvailableSessions[0] = new List<MyNetworkSessionEntry>();
		MyAvailableSessions[1] = new List<MyNetworkSessionEntry>();
		FacePunchSound = EndGameEngine.SoundBnk.GetCue("FacePunch00");
		for (int i = 0; i < 15; i++)
		{
			NetPlayers[i] = new PlayerBase();
			NetPlayers[i].LoadContent(-1);
		}
		NetPlayersInitialized = true;
		NetworkSession.InviteAccepted += InviteAcceptedEventHandler;
		sessionProperties[0] = 2672;
		sessionProperties[1] = 1;
		sessionProperties[2] = 1;
	}

	public static void ResetNetworkPlayersRagdoll()
	{
		for (int i = 0; i < 15; i++)
		{
			NetPlayers[i].mRagdoll.IsValid = false;
		}
	}

	public static PlayerBase NextNetPlayerReference(ref int index)
	{
		while (index < 15)
		{
			if (NetPlayers[index].NetGamerRef != null)
			{
				return NetPlayers[index];
			}
			index++;
		}
		return null;
	}

	public static void Update(float eTime, int qIndex)
	{
		if (!initialized)
		{
			Init();
		}
		HostMigrateTimer -= 0.0334f;
		if (HostMigrateTimer <= 0f)
		{
			HostCreatingWorld = false;
		}
		if (networkSession != null)
		{
			InSessionTimer += 0.0334f;
			UpdateNetworkSession(eTime, qIndex);
		}
	}

	public static void Draw(int qIndex, PlayerBase viewer)
	{
		if (networkSession == null)
		{
			return;
		}
		for (int i = 0; i < 15; i++)
		{
			if (NetPlayers[i].NetGamerRef != null)
			{
				NetPlayers[i].DrawNetPlayer(qIndex, viewer);
			}
		}
	}

	public static void DrawAlpha(PlayerBase viewer, int qIndex)
	{
		if (networkSession == null)
		{
			return;
		}
		for (int i = 0; i < 15; i++)
		{
			if (NetPlayers[i].NetGamerRef != null)
			{
				NetPlayers[i].DrawFlashLightGlare(qIndex, viewer);
			}
		}
	}

	public static void DrawMuzzleFlash(int qIndex, PlayerBase viewer)
	{
		if (networkSession == null)
		{
			return;
		}
		for (int i = 0; i < 15; i++)
		{
			if (NetPlayers[i].NetGamerRef != null)
			{
				NetPlayers[i].DrawNetMuzzleFlash(qIndex);
			}
		}
	}

	public static void DrawPost(int qIndex)
	{
		if (networkSession == null)
		{
			return;
		}
		Menu.spriteBatch.Begin();
		Vector2 zero = Vector2.Zero;
		if (HostMigrateTimer > 0f)
		{
			LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].OverrideInput = true;
			Color black = Color.Black;
			black.R = 180;
			black.G = 180;
			black.B = 180;
			black.A = 180;
			Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Bounds, black);
			string text = "Host Migrating...";
			zero.X = 640f - Menu.defaultFont.MeasureString(text).X * 0.5f;
			zero.Y = 360f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.LightGray);
			if (HostCreatingWorld)
			{
				text = "Host Creating World New...";
				zero.X = 640f - Menu.defaultFont.MeasureString(text).X * 0.5f;
				zero.Y = 390f;
				Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.LightGray);
			}
		}
		Menu.spriteBatch.End();
	}

	public static bool CreateSessionFunc(NetworkSessionType e)
	{
		bool result = false;
		try
		{
			networkSession = NetworkSession.Create(e, 1, 15, 0, sessionProperties);
			if (networkSession != null && !networkSession.IsDisposed)
			{
				HookSessionEvents();
				networkSession.StartGame();
				result = true;
			}
			else
			{
				MessagePump.AddGamerMessage("Error Creating Session: UNKNOWN", "", "", Color.DarkRed, Color.DarkRed);
			}
		}
		catch (Exception ex)
		{
			errorMessage = ex.Message;
			MessagePump.AddMessage(ex.Message);
		}
		return result;
	}

	public static NetSessionRetCodes JoinSessionFunc(int e)
	{
		try
		{
			if (asyncResultJoin != null)
			{
				return NetSessionRetCodes.Porcessing;
			}
			if (availableSessions != null)
			{
				if (e < 0 || e >= availableSessions.Count)
				{
					MessagePump.AddGamerMessage("Join Session Error...", "", "", Color.DarkRed, Color.DarkRed);
					MessagePump.AddMessage("Invalid Session Or Session ended...");
					return NetSessionRetCodes.Error;
				}
				asyncResultJoin = NetworkSession.BeginJoin(availableSessions[e], null, null);
			}
		}
		catch (Exception ex)
		{
			asyncResultJoin = null;
			errorMessage = ex.Message;
			return NetSessionRetCodes.Error;
		}
		return NetSessionRetCodes.Begin;
	}

	public static NetSessionRetCodes JoinSessionFuncComplete()
	{
		try
		{
			if (asyncResultJoin != null && asyncResultJoin.IsCompleted)
			{
				networkSession = NetworkSession.EndJoin(asyncResultJoin);
				asyncResultJoin = null;
				if (networkSession != null && !networkSession.IsDisposed)
				{
					HookSessionEvents();
					return NetSessionRetCodes.Complete;
				}
				MessagePump.AddGamerMessage("Error Joining Session: UNKNOWN", "", "", Color.DarkRed, Color.DarkRed);
				return NetSessionRetCodes.Error;
			}
		}
		catch (Exception ex)
		{
			asyncResultJoin = null;
			errorMessage = ex.Message;
			return NetSessionRetCodes.Error;
		}
		return NetSessionRetCodes.Porcessing;
	}

	public static void InviteAcceptedEventHandler(object sender, InviteAcceptedEventArgs e)
	{
		if (e.IsCurrentSession)
		{
			MessagePump.AddMessage("Already In Match...");
			return;
		}
		if (Guide.IsTrialMode)
		{
			MessagePump.AddMessage("Cant Join Match In Tial Mode...");
			return;
		}
		MessagePump.AddGamerMessage("Join From Invite...", "", "", Color.DarkGreen, Color.DarkGreen);
		inviteToGameScheduled = true;
	}

	public static void JoinInvite()
	{
		JoinInviteInProgress = true;
		inviteToGameScheduled = false;
		FPSGameMenu.Close();
		AIBase.BlackFadeTimer = 8f;
		if (networkSession != null)
		{
			ExitSession();
		}
		MainMenu.SpawningPlayerTimer = float.MinValue;
		AIBase.ScheduledWorldDownloads.Clear();
		AIBase.AllWorldItems.Reset();
		AIBase.ResetZombies();
		AIBase.ResetVehicles();
		AIBase.AllWeapons.Load("");
		ZombiePositionGrid.Reset();
		LevelBaseMenu.PrepareLoadLevel();
		try
		{
			networkSession = NetworkSession.JoinInvited(1);
			if (networkSession != null && !networkSession.IsDisposed)
			{
				HookSessionEvents();
				LevelBaseMenu.isLocalMode = false;
				LevelBaseMenu.isTrialMode = false;
				string gamerTag = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag;
				Storage.PlayerCharacterFilename = gamerTag + "_Character";
				Storage.PlayerStatisFilename = gamerTag + "_OnlineStatis";
				Storage.PlayerInventoryFilename = gamerTag + "_OnlineInventory";
				Storage.PlayerTentsFilename = gamerTag + "_OnlineTents";
				ApocZSaveDataCls.SyncingToServer = true;
				PlayerBase.ApocalypseZ_Hack = true;
				LevelBaseMenu.gameMode = GameMode.SurvivorLocal;
				EndGameEngine.menuMgr.MakeActive(GameMenus.FPSGame);
				for (int i = 0; i < 4; i++)
				{
					FPSGameMenu.TrialTime = 90f;
					LevelBaseMenu.Players[i].TargetPraticeMessage = false;
					LevelBaseMenu.Players[i].AvRStartMessage = false;
					LevelBaseMenu.Players[i].DeathTimer = -1f;
					LevelBaseMenu.Players[i].ToggledRespawn = true;
					LevelBaseMenu.Players[i].CurrentBulletsHitCount = 0;
					LevelBaseMenu.Players[i].CurrentBulletsFiredCount = 0;
					LevelBaseMenu.Players[i].CurrentTargetScore = 0;
					LevelBaseMenu.Players[i].CurrentRatioScore = 0;
					LevelBaseMenu.Players[i].CurrentTimeScore = 0;
					LevelBaseMenu.Players[i].IsSplitScreen = false;
					LevelBaseMenu.Players[i].Spawned = false;
				}
				LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerId = networkSession.LocalGamers[0].Id;
				LevelOutside.Reset();
				LevelBaseMenu.AvRai.ResetWave();
				TriggerData.TargetsActive = true;
				StartMenu.PlayThemeMusic(e: false);
				EndGameEngine.UpdatePresence(GamerPresenceMode.Multiplayer);
			}
			else
			{
				MessagePump.AddGamerMessage("Error joining Invite: UNKNOWN", "", "", Color.DarkRed, Color.DarkRed);
				MainMenu.SpawningPlayerIntoWorld = false;
				MainMenu.SpawningPlayerTimer = 0f;
				StartMenu.ApocThemeMusicRampUp = true;
				StartMenu.PlayThemeMusic(e: true);
				EndGameEngine.LevelMgr.UpdateMenuReset();
				EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
			}
		}
		catch (Exception ex)
		{
			MessagePump.AddGamerMessage("Invite Accept Error", "", "", Color.DarkRed, Color.DarkRed);
			MessagePump.AddMessage(ex.Message);
			MainMenu.SpawningPlayerIntoWorld = false;
			MainMenu.SpawningPlayerTimer = 0f;
			StartMenu.ApocThemeMusicRampUp = true;
			StartMenu.PlayThemeMusic(e: true);
			EndGameEngine.LevelMgr.UpdateMenuReset();
			EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
		}
		JoinInviteInProgress = false;
	}

	public static void GetAvailableSessionFunc()
	{
		int num = ((currentValidListIndex + 1 <= 1) ? (currentValidListIndex + 1) : 0);
		try
		{
			if (availableSessions != null)
			{
				availableSessions.Dispose();
				availableSessions = null;
			}
			availableSessions = NetworkSession.Find(NetworkSessionType.PlayerMatch, 1, sessionProperties);
			if (availableSessions.Count == 0)
			{
				errorMessage = "No network sessions found.";
			}
			MyAvailableSessions[num].Clear();
			for (int i = 0; i < availableSessions.Count; i++)
			{
				for (int j = 0; j < MyAvailableSessions[currentValidListIndex].Count; j++)
				{
					if (MyAvailableSessions[currentValidListIndex][j].HostGamertag == availableSessions[i].HostGamertag)
					{
						MyNetworkSessionEntry myNetworkSessionEntry = new MyNetworkSessionEntry();
						myNetworkSessionEntry.HostGamertag = MyAvailableSessions[currentValidListIndex][j].HostGamertag;
						myNetworkSessionEntry.CurrentGamerCount = MyAvailableSessions[currentValidListIndex][j].CurrentGamerCount;
						myNetworkSessionEntry.OpenPublicGamerSlots = MyAvailableSessions[currentValidListIndex][j].OpenPublicGamerSlots;
						myNetworkSessionEntry.BytesPerSecondDownstream = MyAvailableSessions[currentValidListIndex][j].BytesPerSecondDownstream;
						myNetworkSessionEntry.BytesPerSecondUpstream = MyAvailableSessions[currentValidListIndex][j].BytesPerSecondUpstream;
						myNetworkSessionEntry.Ping = MyAvailableSessions[currentValidListIndex][j].Ping;
						MyAvailableSessions[num].Add(myNetworkSessionEntry);
						break;
					}
				}
			}
			for (int k = 0; k < availableSessions.Count; k++)
			{
				bool flag = false;
				for (int l = 0; l < MyAvailableSessions[num].Count; l++)
				{
					if (MyAvailableSessions[num][l].HostGamertag == availableSessions[k].HostGamertag)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					MyNetworkSessionEntry myNetworkSessionEntry2 = new MyNetworkSessionEntry();
					myNetworkSessionEntry2.HostGamertag = availableSessions[k].HostGamertag;
					myNetworkSessionEntry2.CurrentGamerCount = availableSessions[k].CurrentGamerCount;
					myNetworkSessionEntry2.OpenPublicGamerSlots = availableSessions[k].OpenPublicGamerSlots;
					if (availableSessions[k].QualityOfService.IsAvailable)
					{
						myNetworkSessionEntry2.BytesPerSecondDownstream = availableSessions[k].QualityOfService.BytesPerSecondDownstream;
						myNetworkSessionEntry2.BytesPerSecondUpstream = availableSessions[k].QualityOfService.BytesPerSecondUpstream;
						myNetworkSessionEntry2.Ping = (int)availableSessions[k].QualityOfService.AverageRoundtripTime.TotalMilliseconds;
					}
					MyAvailableSessions[num].Add(myNetworkSessionEntry2);
				}
			}
			currentValidListIndex = num;
		}
		catch (Exception ex)
		{
			currentValidListIndex = num;
			errorMessage = ex.Message;
			MessagePump.AddMessage(ex.Message);
		}
	}

	public static void UpdateSessionQualities()
	{
		try
		{
			if (availableSessions == null || availableSessions.IsDisposed)
			{
				return;
			}
			for (int i = 0; i < availableSessions.Count; i++)
			{
				if (MyAvailableSessions[currentValidListIndex] == null || i >= MyAvailableSessions[currentValidListIndex].Count)
				{
					continue;
				}
				try
				{
					MyAvailableSessions[currentValidListIndex][i].HostGamertag = availableSessions[i].HostGamertag;
					MyAvailableSessions[currentValidListIndex][i].CurrentGamerCount = availableSessions[i].CurrentGamerCount;
					MyAvailableSessions[currentValidListIndex][i].OpenPublicGamerSlots = availableSessions[i].OpenPublicGamerSlots;
					if (availableSessions[i].QualityOfService.IsAvailable)
					{
						MyAvailableSessions[currentValidListIndex][i].BytesPerSecondDownstream = availableSessions[i].QualityOfService.BytesPerSecondDownstream;
						MyAvailableSessions[currentValidListIndex][i].BytesPerSecondUpstream = availableSessions[i].QualityOfService.BytesPerSecondUpstream;
						MyAvailableSessions[currentValidListIndex][i].Ping = (int)availableSessions[i].QualityOfService.AverageRoundtripTime.TotalMilliseconds;
					}
				}
				catch
				{
					try
					{
						if (MyAvailableSessions[currentValidListIndex][i] != null)
						{
							MyAvailableSessions[currentValidListIndex][i].HostGamertag = "empty";
							MyAvailableSessions[currentValidListIndex][i].CurrentGamerCount = 0;
							MyAvailableSessions[currentValidListIndex][i].OpenPublicGamerSlots = 0;
							MyAvailableSessions[currentValidListIndex][i].BytesPerSecondDownstream = 1;
							MyAvailableSessions[currentValidListIndex][i].BytesPerSecondUpstream = 1;
							MyAvailableSessions[currentValidListIndex][i].Ping = 1000;
						}
					}
					catch (Exception ex)
					{
						MessagePump.AddMessage("UpdateSessionQualitie(): InnerLoop" + ex.Message);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			MessagePump.AddMessage("UpdateSessionQualitie(): " + ex2.Message);
		}
	}

	private static void UpdateNetworkSession(float eTime, int qIndex)
	{
		foreach (LocalNetworkGamer localGamer in networkSession.LocalGamers)
		{
			UpdateLocalGamer(localGamer);
		}
		foreach (NetworkGamer allGamer in networkSession.AllGamers)
		{
			if (allGamer.IsLocal)
			{
				continue;
			}
			PlayerBase playerBase = allGamer.Tag as PlayerBase;
			playerBase.TargetFrameCounter++;
			float num = 1f;
			if (playerBase.numFramesSinceLastUpdate > 0)
			{
				num /= (float)playerBase.numFramesSinceLastUpdate;
				num *= (float)playerBase.TargetFrameCounter;
			}
			num = 0.25f;
			if (playerBase.Angles.X > 270f && playerBase.vecTargetAngles.X < 90f)
			{
				playerBase.Angles.X = MathHelper.Lerp(playerBase.Angles.X, playerBase.vecTargetAngles.X + 360f, 0.25f);
			}
			else if (playerBase.Angles.X < 90f && playerBase.vecTargetAngles.X > 270f)
			{
				playerBase.Angles.X = MathHelper.Lerp(playerBase.Angles.X, playerBase.vecTargetAngles.X - 360f, 0.25f);
			}
			else
			{
				playerBase.Angles.X = MathHelper.Lerp(playerBase.Angles.X, playerBase.vecTargetAngles.X, 0.25f);
			}
			playerBase.Angles.X = ((playerBase.Angles.X > 360f) ? (playerBase.Angles.X - 360f) : playerBase.Angles.X);
			playerBase.Angles.X = ((playerBase.Angles.X < 0f) ? (playerBase.Angles.X + 360f) : playerBase.Angles.X);
			playerBase.Angles.Y = MathHelper.Lerp(playerBase.Angles.Y, playerBase.vecTargetAngles.Y, 0.25f);
			playerBase.AngleTorsoCharacter = 0f;
			playerBase.vecCharacterDir = Vector3.Transform(Vector3.UnitZ, Matrix.CreateRotationY(MathHelper.ToRadians(playerBase.Angles.X)));
			Vector3 vector = Vector3.Cross(playerBase.vecCharacterDir, Vector3.UnitY);
			playerBase.vecTargetPosition += playerBase.vecCharacterDir * playerBase.Speed;
			playerBase.vecTargetPosition += vector * playerBase.SideStep;
			vector = playerBase.vecTargetPosition - playerBase.vecPosition;
			vector.Y = 0f;
			float num2 = vector.LengthSquared();
			if (num2 > 32400f)
			{
				playerBase.vecPosition.X = MathHelper.Lerp(playerBase.vecPosition.X, playerBase.vecTargetPosition.X, 0.05f);
				playerBase.vecPosition.Z = MathHelper.Lerp(playerBase.vecPosition.Z, playerBase.vecTargetPosition.Z, 0.05f);
			}
			else if (num2 > 1296f)
			{
				vector.Normalize();
				playerBase.vecPosition += vector * 24f;
			}
			else if (num2 > 4f)
			{
				vector.Normalize();
				num2 = num2 / 1296f * 24f;
				playerBase.vecPosition += vector * num2;
			}
			else
			{
				playerBase.vecPosition.X = playerBase.vecTargetPosition.X;
				playerBase.vecPosition.Z = playerBase.vecTargetPosition.Z;
			}
			if (playerBase.IsAttached0)
			{
				VehicleCls attachedVehicle = AIBase.GetAttachedVehicle(playerBase);
				if (attachedVehicle != null)
				{
					playerBase.vecTargetPosition = attachedVehicle.Position;
					playerBase.vecPosition = playerBase.vecTargetPosition;
				}
			}
			if (float.IsNaN(playerBase.vecPosition.X) || float.IsNaN(playerBase.vecPosition.Y) || float.IsNaN(playerBase.vecPosition.Z))
			{
				playerBase.vecPosition = playerBase.vecTargetPosition;
			}
			float height = HeightMapPhysics.GetHeight(ref playerBase.vecPosition);
			if (height + 62f > playerBase.vecPosition.Y)
			{
				playerBase.GravityAccel = 0f;
				playerBase.vecPosition.Y = height + 62f;
			}
			else if (height + 64f < playerBase.vecPosition.Y)
			{
				playerBase.GravityAccel += 24f * EndGameEngine.fFIXED_TIME_STEP;
				playerBase.vecPosition.Y -= playerBase.GravityAccel;
			}
			playerBase.UpdateThirdPersonCharacter(EndGameEngine.currentEleapsedTime, qIndex, isRemotePlayer: true);
		}
		if (networkSession.IsHost)
		{
			UpdateServer();
		}
		networkSession.Update();
		if (networkSession == null)
		{
			return;
		}
		foreach (LocalNetworkGamer localGamer2 in networkSession.LocalGamers)
		{
			if (localGamer2.IsHost)
			{
				ServerReadInputFromClients(localGamer2);
			}
			else
			{
				ClientReadGameStateFromServer(localGamer2);
			}
		}
		if (networkSession != null && !networkSession.IsHost)
		{
			NetworkPingTimer += 0.0333334f;
			if (NetworkPingTimer > 10f)
			{
				NetworkPingTimer = 0f;
				packetWriter.Write((byte)149);
				networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.InOrder, networkSession.Host);
			}
		}
	}

	private static void UpdateLocalGamer(LocalNetworkGamer gamer)
	{
		if (networkSession.IsHost)
		{
			return;
		}
		ClientUpdateTimer--;
		if (ClientUpdateTimer > 0)
		{
			return;
		}
		ClientUpdateTimer = 8;
		foreach (NetworkGamer allGamer in networkSession.AllGamers)
		{
			if (!allGamer.IsLocal)
			{
				PlayerNetWorkPacket.WriteLocalGamer(packetWriter, gamer);
				if (packetWriter.Length > 0)
				{
					gamer.SendData(packetWriter, SendDataOptions.InOrder, allGamer);
				}
			}
		}
	}

	private static void UpdateServer()
	{
		ServerUpdateTimer--;
		if (ServerUpdateTimer <= 0)
		{
			ServerUpdateTimer = 8;
			LocalNetworkGamer localNetworkGamer = (LocalNetworkGamer)networkSession.Host;
			foreach (NetworkGamer allGamer in networkSession.AllGamers)
			{
				if (!allGamer.IsLocal)
				{
					PlayerNetWorkPacket.WriteLocalGamer(packetWriter, localNetworkGamer);
					if (packetWriter.Length > 0)
					{
						localNetworkGamer.SendData(packetWriter, SendDataOptions.InOrder, allGamer);
					}
				}
			}
		}
		if (packetWriter.Length > 0)
		{
			LocalNetworkGamer localNetworkGamer2 = (LocalNetworkGamer)networkSession.Host;
			localNetworkGamer2.SendData(packetWriter, SendDataOptions.InOrder);
		}
	}

	private static void ServerReadInputFromClients(LocalNetworkGamer gamer)
	{
		try
		{
			while (gamer.IsDataAvailable)
			{
				gamer.ReceiveData(packetReader, out var sender);
				if (sender.IsLocal)
				{
					continue;
				}
				switch (LastPacketRead = (ePacketTypes)packetReader.ReadByte())
				{
				case ePacketTypes.PingHost:
					EGENetWorkNext.packetWriter.Write((byte)149);
					EGENetWorkNext.packetWriter.Write(sender.Id);
					networkSession.LocalGamers[0].SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					break;
				case ePacketTypes.InviteToTeam:
					AIBase.Clans.AddPlayerToClan(sender);
					break;
				case ePacketTypes.DeleteFromTeam:
					AIBase.Clans.DeleteFromClan(sender);
					break;
				case ePacketTypes.AcceptToTeam:
					AIBase.Clans.AddPlayerToClan(sender, accept: true);
					break;
				case ePacketTypes.SilentInviteToTeam:
					AIBase.Clans.SilentAddPlayerToClan(sender);
					break;
				case ePacketTypes.Hacker:
					MessagePump.AddMessage(sender.Gamertag + " uses modified save data");
					break;
				case ePacketTypes.ResyncWithServer:
					AIBase.GamerJoinedSession(sender);
					break;
				case ePacketTypes.PlayerData:
					PlayerNetWorkPacket.ServerReadClientGamer(packetReader, sender);
					break;
				case ePacketTypes.PlayerPosition:
				{
					Vector3 vecTargetPosition = packetReader.ReadVector3();
					PlayerBase playerBase6 = sender.Tag as PlayerBase;
					playerBase6.vecTargetPosition = vecTargetPosition;
					float num11 = (playerBase6.vecPosition - playerBase6.vecTargetPosition).LengthSquared();
					if (num11 > 160000f)
					{
						playerBase6.vecPosition = playerBase6.vecTargetPosition;
					}
					break;
				}
				case ePacketTypes.WorldItemRequest:
				{
					ItemCls itemCls = new ItemCls();
					itemCls.NetworkRead(packetReader);
					if (AIBase.AllWorldItems.ServerRequestPickupItem(itemCls))
					{
						AIBase.AllWorldItems.ServerUpdateItemToClients(itemCls, sender.Id);
					}
					break;
				}
				case ePacketTypes.WorldItemDrop:
				{
					ItemCls itemCls2 = new ItemCls();
					itemCls2.NetworkRead(packetReader);
					AIBase.AllWorldItems.ServerDropItem(itemCls2.pos, itemCls2, sender.Id);
					break;
				}
				case ePacketTypes.VehicleRequestAttach:
				{
					ushort num5 = packetReader.ReadUInt16();
					byte b8 = packetReader.ReadByte();
					if (AIBase.CanPlayerAttachToVehicle(sender, num5, b8))
					{
						EGENetWorkNext.packetWriter.Write((byte)114);
						EGENetWorkNext.packetWriter.Write(sender.Id);
						EGENetWorkNext.packetWriter.Write(num5);
						EGENetWorkNext.packetWriter.Write(b8);
						gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.ReliableInOrder);
					}
					break;
				}
				case ePacketTypes.VehicleGamerDetach:
				{
					ushort num6 = packetReader.ReadUInt16();
					byte b9 = packetReader.ReadByte();
					byte b10 = packetReader.ReadByte();
					EGENetWorkNext.packetWriter.Write((byte)115);
					EGENetWorkNext.packetWriter.Write(sender.Id);
					EGENetWorkNext.packetWriter.Write(num6);
					EGENetWorkNext.packetWriter.Write(b9);
					EGENetWorkNext.packetWriter.Write(b10);
					gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.ReliableInOrder);
					AIBase.DetachRemotePlayerFromVehicle(sender, num6, b10, b9);
					break;
				}
				case ePacketTypes.VehicleGamerTranslation:
				{
					ushort num3 = packetReader.ReadUInt16();
					Vector3 vector = packetReader.ReadVector3();
					byte b6 = packetReader.ReadByte();
					readNB4_0.PackedValue = packetReader.ReadUInt32();
					readPHV2.PackedValue = packetReader.ReadUInt32();
					byte b7 = packetReader.ReadByte();
					readDir = readNB4_0.ToVector4();
					readSpeedSteer = readPHV2.ToVector2();
					direction.X = readDir.X;
					direction.Y = readDir.Y;
					direction.Z = readDir.Z;
					float reverse = ((float)(int)b6 - 127f) * 0.007874f;
					AIBase.VehicleNetworkTranslation(sender.Id, num3, vector, direction, readSpeedSteer.X, readSpeedSteer.Y, reverse, b7);
					EGENetWorkNext.packetWriter.Write((byte)116);
					EGENetWorkNext.packetWriter.Write(sender.Id);
					EGENetWorkNext.packetWriter.Write(num3);
					EGENetWorkNext.packetWriter.Write(vector);
					EGENetWorkNext.packetWriter.Write(b6);
					EGENetWorkNext.packetWriter.Write(readNB4_0.PackedValue);
					EGENetWorkNext.packetWriter.Write(readPHV2.PackedValue);
					EGENetWorkNext.packetWriter.Write(b7);
					gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					break;
				}
				case ePacketTypes.VehicleData:
				{
					int num4 = packetReader.ReadInt32();
					AIBase.UpdateVehicleData(packetReader, num4);
					AIBase.AllVehicles[num4].SendVehicleDataPacket(EGENetWorkNext.packetWriter, num4, isHost: true);
					break;
				}
				case ePacketTypes.ZombieDamageData:
				{
					byte b12 = packetReader.ReadByte();
					byte b13 = packetReader.ReadByte();
					byte b14 = packetReader.ReadByte();
					NetworkGamer networkGamer3 = networkSession.FindGamerById(b12);
					PlayerBase playerBase4 = ((networkGamer3 != null) ? (networkGamer3.Tag as PlayerBase) : null);
					if (playerBase4 != null)
					{
						float num9 = playerBase4.BloodLevel - (float)(int)b13;
						playerBase4.BloodLevel = ((num9 < 0f) ? 0f : num9);
						playerBase4.BloodLoss = ((b14 > 0) ? 1 : 0);
						EGENetWorkNext.packetWriter.Write((byte)129);
						EGENetWorkNext.packetWriter.Write(b12);
						EGENetWorkNext.packetWriter.Write(b13);
						EGENetWorkNext.packetWriter.Write(b14);
						gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					}
					break;
				}
				case ePacketTypes.DamageData:
				{
					byte b15 = packetReader.ReadByte();
					byte b16 = packetReader.ReadByte();
					byte b17 = packetReader.ReadByte();
					byte b18 = packetReader.ReadByte();
					NetworkGamer networkGamer4 = networkSession.FindGamerById(b16);
					PlayerBase playerBase5 = ((networkGamer4 != null) ? (networkGamer4.Tag as PlayerBase) : null);
					if (playerBase5 != null)
					{
						float num10 = playerBase5.BloodLevel - (float)(int)b17;
						playerBase5.BloodLevel = ((num10 < 0f) ? 0f : num10);
						playerBase5.BloodLoss = ((b18 > 0) ? 1 : 0);
						EGENetWorkNext.packetWriter.Write((byte)130);
						EGENetWorkNext.packetWriter.Write(b15);
						EGENetWorkNext.packetWriter.Write(b16);
						EGENetWorkNext.packetWriter.Write(b17);
						EGENetWorkNext.packetWriter.Write(b18);
						gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
						if (networkGamer4.IsLocal && b15 == 5)
						{
							FacePunchSound.Dispose();
							FacePunchSound = EndGameEngine.SoundBnk.GetCue("FacePunch00");
							FacePunchSound.Play();
						}
					}
					break;
				}
				case ePacketTypes.GamerDeath:
				{
					byte b11 = packetReader.ReadByte();
					NetworkGamer networkGamer2 = networkSession.FindGamerById(b11);
					PlayerBase playerBase3 = ((networkGamer2 != null) ? (networkGamer2.Tag as PlayerBase) : null);
					if (playerBase3 != null && !networkGamer2.IsLocal)
					{
						Vector3 damageDir = Vector3.Zero;
						playerBase3.ProcessDeath(DamegePacketType.None, ref damageDir);
						EGENetWorkNext.packetWriter.Write((byte)132);
						EGENetWorkNext.packetWriter.Write(b11);
						gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					}
					break;
				}
				case ePacketTypes.GamerSpawned:
				{
					Vector3 vector2 = packetReader.ReadVector3();
					Vector3 vector3 = packetReader.ReadVector3();
					Vector3 vector4 = packetReader.ReadVector3();
					float num7 = packetReader.ReadSingle();
					float num8 = packetReader.ReadSingle();
					EGENetWorkNext.packetWriter.Write((byte)133);
					EGENetWorkNext.packetWriter.Write(sender.Id);
					EGENetWorkNext.packetWriter.Write(vector2);
					EGENetWorkNext.packetWriter.Write(vector3);
					EGENetWorkNext.packetWriter.Write(vector4);
					EGENetWorkNext.packetWriter.Write(num7);
					EGENetWorkNext.packetWriter.Write(num8);
					gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					PlayerBase playerBase2 = sender.Tag as PlayerBase;
					playerBase2.IsAttached0 = false;
					playerBase2.vecPosition = vector2;
					playerBase2.vecCharacterDir = vector3;
					playerBase2.Angles = vector4;
					playerBase2.Spawned = true;
					playerBase2.Health = 100f;
					playerBase2.BloodLevel = num7;
					playerBase2.BloodLoss = num8;
					break;
				}
				case ePacketTypes.ZombieDeath:
				{
					byte senderId = packetReader.ReadByte();
					AIBase.ZombieDeath(packetReader, senderId, netBroadcast: true);
					break;
				}
				case ePacketTypes.ZombieDeathConfirm:
				{
					ushort uid3 = packetReader.ReadUInt16();
					ZombiePositionGrid.ConfirmSync(uid3);
					break;
				}
				case ePacketTypes.ZombieLineOfSight:
				{
					ushort uid2 = packetReader.ReadUInt16();
					byte disQuant = packetReader.ReadByte();
					AIBase.ZombieSetLOS(uid2, disQuant, sender);
					break;
				}
				case ePacketTypes.ZombieClientRequestNewState:
				{
					ushort uid = packetReader.ReadUInt16();
					byte anim = packetReader.ReadByte();
					byte state = packetReader.ReadByte();
					AIBase.ZombieNewState(sender.Id, uid, anim, state);
					break;
				}
				case ePacketTypes.PlayerOptionsSync:
				{
					byte b = packetReader.ReadByte();
					byte b2 = packetReader.ReadByte();
					float num = (int)packetReader.ReadByte();
					float num2 = (int)packetReader.ReadByte();
					byte b3 = packetReader.ReadByte();
					byte b4 = packetReader.ReadByte();
					byte b5 = packetReader.ReadByte();
					NetworkGamer networkGamer = networkSession.FindGamerById(b);
					if (networkGamer != null && !networkGamer.IsLocal && networkGamer.Tag is PlayerBase playerBase)
					{
						playerBase.SetCharacter(b2, num, num2);
						playerBase.FlashLightOn = b4 > 0;
						playerBase.CurrentBacpPack = b3;
						playerBase.CurrentDay = b5;
						PacketWriter packetWriter = EGENetWorkNext.packetWriter;
						packetWriter.Write((byte)135);
						packetWriter.Write(b);
						packetWriter.Write(b2);
						packetWriter.Write((byte)num);
						packetWriter.Write((byte)num2);
						packetWriter.Write(b3);
						packetWriter.Write(b4);
						packetWriter.Write(b5);
						gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					}
					break;
				}
				}
			}
		}
		catch (Exception ex)
		{
			MessagePump.AddMessage("HostReadClientPacket: Last Message: " + LastPacketRead.ToString() + ", Error: " + ex.Message);
		}
	}

	private static void ClientReadGameStateFromServer(LocalNetworkGamer gamer)
	{
		try
		{
			while (gamer.IsDataAvailable)
			{
				gamer.ReceiveData(packetReader, out var sender);
				while (packetReader.Position < packetReader.Length)
				{
					switch (LastPacketRead = (ePacketTypes)packetReader.ReadByte())
					{
					case ePacketTypes.PingHost:
					{
						byte b16 = packetReader.ReadByte();
						_ = gamer.Id;
						NetworkCurrentPing = NetworkPingTimer;
						break;
					}
					case ePacketTypes.Hacker:
						MessagePump.AddMessage(sender.Gamertag + " uses modified save data");
						break;
					case ePacketTypes.InviteToTeam:
						AIBase.Clans.AddPlayerToClan(sender);
						break;
					case ePacketTypes.DeleteFromTeam:
						AIBase.Clans.DeleteFromClan(sender);
						break;
					case ePacketTypes.AcceptToTeam:
						AIBase.Clans.AddPlayerToClan(sender, accept: true);
						break;
					case ePacketTypes.SilentInviteToTeam:
						AIBase.Clans.SilentAddPlayerToClan(sender);
						break;
					case ePacketTypes.PlayerData:
						PlayerNetWorkPacket.ServerReadClientGamer(packetReader, sender);
						break;
					case ePacketTypes.PlayerPosition:
					{
						Vector3 vecTargetPosition = packetReader.ReadVector3();
						PlayerBase playerBase6 = sender.Tag as PlayerBase;
						playerBase6.vecTargetPosition = vecTargetPosition;
						float num4 = (playerBase6.vecPosition - playerBase6.vecTargetPosition).LengthSquared();
						if (num4 > 160000f)
						{
							playerBase6.vecPosition = playerBase6.vecTargetPosition;
						}
						break;
					}
					case ePacketTypes.PlayerDamage:
					{
						byte gamerId2 = packetReader.ReadByte();
						int num2 = packetReader.ReadByte();
						int num3 = packetReader.ReadByte();
						NetworkGamer networkGamer3 = networkSession.FindGamerById(gamerId2);
						if (networkGamer3 != null && networkGamer3.IsLocal)
						{
							PlayerBase playerBase3 = networkGamer3.Tag as PlayerBase;
							playerBase3.Health -= num2;
							playerBase3.BloodLoss = ((playerBase3.BloodLoss > (float)num3) ? playerBase3.BloodLoss : ((float)num3));
						}
						break;
					}
					case ePacketTypes.WorldItemCreate:
						AIBase.AllWorldItems.ReadCreateFromServer(packetReader, sender);
						break;
					case ePacketTypes.WorldItemUpdate:
					{
						byte b6 = packetReader.ReadByte();
						ItemCls itemCls = new ItemCls();
						itemCls.NetworkRead(packetReader);
						AIBase.AllWorldItems.PickupItem(itemCls, gamer.Id == b6);
						break;
					}
					case ePacketTypes.WorldItemUpdateDesc:
					{
						ItemCls itemCls3 = new ItemCls();
						itemCls3.NetworkRead(packetReader);
						AIBase.AllWorldItems.ClientUpdateItem(itemCls3);
						break;
					}
					case ePacketTypes.WorldItemAdd:
					{
						byte b7 = packetReader.ReadByte();
						ItemCls itemCls2 = new ItemCls();
						itemCls2.NetworkRead(packetReader);
						AIBase.AllWorldItems.AddItemToList(itemCls2.pos, itemCls2);
						if (gamer.Id != b7)
						{
						}
						break;
					}
					case ePacketTypes.WorldCreateLoadTents:
					{
						byte b2 = packetReader.ReadByte();
						if (gamer.Id == b2)
						{
							ApocZSaveDataCls.ScheduleWorldItemLoad = true;
						}
						break;
					}
					case ePacketTypes.WorldCreateDone:
					{
						byte b15 = packetReader.ReadByte();
						if (gamer.Id == b15)
						{
							ApocZSaveDataCls.SyncingToServer = false;
						}
						break;
					}
					case ePacketTypes.VehicleGamerAttach:
					{
						byte gamerId6 = packetReader.ReadByte();
						ushort uid5 = packetReader.ReadUInt16();
						byte vSeat = packetReader.ReadByte();
						AIBase.AttachPlayerToVehicle(gamerId6, uid5, vSeat);
						break;
					}
					case ePacketTypes.VehicleGamerDetach:
					{
						byte gamerId7 = packetReader.ReadByte();
						ushort uid6 = packetReader.ReadUInt16();
						byte vSeat2 = packetReader.ReadByte();
						byte headLights2 = packetReader.ReadByte();
						AIBase.DetachRemotePlayerFromVehicle(networkSession.FindGamerById(gamerId7), uid6, headLights2, vSeat2);
						break;
					}
					case ePacketTypes.VehicleGamerTranslation:
					{
						byte gamerId5 = packetReader.ReadByte();
						ushort uid4 = packetReader.ReadUInt16();
						Vector3 pos = packetReader.ReadVector3();
						byte b14 = packetReader.ReadByte();
						readNB4_0.PackedValue = packetReader.ReadUInt32();
						readPHV2.PackedValue = packetReader.ReadUInt32();
						byte headLights = packetReader.ReadByte();
						readDir = readNB4_0.ToVector4();
						readSpeedSteer = readPHV2.ToVector2();
						direction.X = readDir.X;
						direction.Y = readDir.Y;
						direction.Z = readDir.Z;
						float reverse = ((float)(int)b14 - 127f) * 0.007874f;
						AIBase.VehicleNetworkTranslation(gamerId5, uid4, pos, direction, readSpeedSteer.X, readSpeedSteer.Y, reverse, headLights);
						break;
					}
					case ePacketTypes.VehicleData:
					{
						int vehicleIndex = packetReader.ReadInt32();
						AIBase.UpdateVehicleData(packetReader, vehicleIndex);
						break;
					}
					case ePacketTypes.VehicleSpawn:
					{
						int vehicleIndex2 = packetReader.ReadInt32();
						AIBase.VehicleSpawn(packetReader, vehicleIndex2);
						break;
					}
					case ePacketTypes.SunPosition:
						LevelOutside.SunAngle = packetReader.ReadSingle();
						break;
					case ePacketTypes.ZombieAdd:
						AIBase.HostSendZombieToClient(packetReader, sender);
						break;
					case ePacketTypes.ZombieDamageData:
					{
						byte b3 = packetReader.ReadByte();
						byte b4 = packetReader.ReadByte();
						byte b5 = packetReader.ReadByte();
						if (gamer.Id == b3)
						{
							break;
						}
						NetworkGamer networkGamer2 = networkSession.FindGamerById(b3);
						PlayerBase playerBase2 = ((networkGamer2 != null) ? (networkGamer2.Tag as PlayerBase) : null);
						if (playerBase2 != null && !networkGamer2.IsLocal)
						{
							float num = playerBase2.BloodLevel - (float)(int)b4;
							playerBase2.BloodLevel = ((num < 0f) ? 0f : num);
							if (b5 > 0 && playerBase2.BloodLoss == 0f)
							{
								playerBase2.BloodLoss = 0.01f;
							}
						}
						break;
					}
					case ePacketTypes.DamageData:
					{
						byte b10 = packetReader.ReadByte();
						byte b11 = packetReader.ReadByte();
						byte b12 = packetReader.ReadByte();
						byte b13 = packetReader.ReadByte();
						if (gamer.Id == b11)
						{
							float num5 = ((PlayerBase)gamer.Tag).BloodLevel - (float)(int)b12;
							((PlayerBase)gamer.Tag).BloodLevel = ((num5 < 0f) ? 0f : num5);
							if (b13 > 0 && ((PlayerBase)gamer.Tag).BloodLoss == 0f)
							{
								((PlayerBase)gamer.Tag).BloodLoss = 0.01f;
							}
							if (b10 == 5)
							{
								FacePunchSound.Dispose();
								FacePunchSound = EndGameEngine.SoundBnk.GetCue("FacePunch00");
								FacePunchSound.Play();
							}
						}
						break;
					}
					case ePacketTypes.GamerDeath:
					{
						byte gamerId4 = packetReader.ReadByte();
						NetworkGamer networkGamer5 = networkSession.FindGamerById(gamerId4);
						PlayerBase playerBase5 = ((networkGamer5 != null) ? (networkGamer5.Tag as PlayerBase) : null);
						if (playerBase5 != null && !networkGamer5.IsLocal)
						{
							Vector3 damageDir = Vector3.Zero;
							playerBase5.ProcessDeath(DamegePacketType.None, ref damageDir);
						}
						break;
					}
					case ePacketTypes.GamerSpawned:
					{
						byte gamerId3 = packetReader.ReadByte();
						Vector3 vecPosition = packetReader.ReadVector3();
						Vector3 vecCharacterDir = packetReader.ReadVector3();
						Vector3 angles = packetReader.ReadVector3();
						float bloodLevel = packetReader.ReadSingle();
						float bloodLoss = packetReader.ReadSingle();
						NetworkGamer networkGamer4 = networkSession.FindGamerById(gamerId3);
						if (networkGamer4 != null && !networkGamer4.IsLocal)
						{
							PlayerBase playerBase4 = networkGamer4.Tag as PlayerBase;
							playerBase4.IsAttached0 = false;
							playerBase4.vecPosition = vecPosition;
							playerBase4.vecCharacterDir = vecCharacterDir;
							playerBase4.Angles = angles;
							playerBase4.Spawned = true;
							playerBase4.Health = 100f;
							playerBase4.BloodLevel = bloodLevel;
							playerBase4.BloodLoss = bloodLoss;
						}
						break;
					}
					case ePacketTypes.ZombieDeath:
					{
						byte senderId = packetReader.ReadByte();
						AIBase.ZombieDeath(packetReader, senderId, netBroadcast: false);
						break;
					}
					case ePacketTypes.ZombieDeathConfirm:
					{
						ushort uid3 = packetReader.ReadUInt16();
						ZombiePositionGrid.ConfirmSync(uid3);
						break;
					}
					case ePacketTypes.ZombieUpdatePosition:
						AIBase.ZombieUpdatePosition(packetReader, sender, ePacketTypes.ZombieUpdatePosition);
						break;
					case ePacketTypes.ZombieUpdateRoute:
						AIBase.ZombieUpdatePosition(packetReader, sender, ePacketTypes.ZombieUpdateRoute);
						break;
					case ePacketTypes.ZombieUpdatePathing:
					{
						ushort uid2 = packetReader.ReadUInt16();
						ushort pathingIndex = packetReader.ReadUInt16();
						AIBase.ZombieUpdatePathing(uid2, pathingIndex);
						break;
					}
					case ePacketTypes.ZombieNewState:
					{
						ushort uid = packetReader.ReadUInt16();
						byte anim = packetReader.ReadByte();
						byte state = packetReader.ReadByte();
						byte b9 = packetReader.ReadByte();
						if (gamer.Id != b9)
						{
							AIBase.ZombieNewState(b9, uid, anim, state);
						}
						break;
					}
					case ePacketTypes.ResetWorld:
					{
						byte b8 = packetReader.ReadByte();
						if (gamer.Id == b8)
						{
							HostCreatingWorld = true;
							AIBase.AllWorldItems.Reset();
							AIBase.ResetZombies();
							ApocZSaveDataCls.Reset();
						}
						break;
					}
					case ePacketTypes.PlayerOptionsSync:
					{
						byte gamerId = packetReader.ReadByte();
						byte e = packetReader.ReadByte();
						float si = (int)packetReader.ReadByte();
						float pi = (int)packetReader.ReadByte();
						byte currentBacpPack = packetReader.ReadByte();
						byte b = packetReader.ReadByte();
						byte currentDay = packetReader.ReadByte();
						NetworkGamer networkGamer = networkSession.FindGamerById(gamerId);
						if (networkGamer != null && !networkGamer.IsLocal && networkGamer.Tag is PlayerBase playerBase)
						{
							playerBase.SetCharacter(e, si, pi);
							playerBase.FlashLightOn = b > 0;
							playerBase.CurrentBacpPack = currentBacpPack;
							playerBase.CurrentDay = currentDay;
						}
						break;
					}
					}
				}
			}
		}
		catch (Exception ex)
		{
			MessagePump.AddMessage("ClientReadServerPacket: " + LastPacketRead.ToString() + ", Error: " + ex.Message);
		}
	}

	private static void HookSessionEvents()
	{
		InSessionTimer = 0f;
		HostMigrateTimer = 0f;
		networkSession.AllowHostMigration = true;
		networkSession.AllowJoinInProgress = true;
		networkSession.GamerJoined += GamerJoinedEventHandler;
		networkSession.GamerLeft += GamerLeftEventHandler;
		networkSession.SessionEnded += SessionEndedEventHandler;
		networkSession.HostChanged += SessionHostChanged;
	}

	private static void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
	{
		if (e.Gamer.IsLocal)
		{
			PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
			e.Gamer.Tag = playerBase;
			playerBase.IsAttached0 = false;
			playerBase.NetworkUpdateTimer = (float)EndGameEngine.randGenerator.NextDouble();
			playerBase.NetGamerRef = e.Gamer;
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			packetWriter.Write((byte)135);
			packetWriter.Write(e.Gamer.Id);
			packetWriter.Write(playerBase.CharacterIndex);
			packetWriter.Write((byte)playerBase.ShirtIndex);
			packetWriter.Write((byte)playerBase.PantstIndex);
			packetWriter.Write((byte)playerBase.CurrentBacpPack);
			packetWriter.Write((byte)(playerBase.FlashLightOn ? 1u : 0u));
			packetWriter.Write((byte)playerBase.CurrentDay);
			networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
			try
			{
				if (e.Gamer.IsHost)
				{
					AIBase.HostCreateWorld(e.Gamer);
				}
			}
			catch (Exception ex)
			{
				MessagePump.AddMessage("HostCreateWorld()." + ex.Message);
			}
			AIBase.Clans.JoinSession(e.Gamer);
			return;
		}
		for (int i = 0; i < 15; i++)
		{
			if (NetPlayers[i].NetGamerRef == null)
			{
				NetPlayers[i].SetNetworkPlayer(e.Gamer);
				NetPlayers[i].NetGamerRef = e.Gamer;
				MessagePump.AddGamerMessage(NetPlayers[i].NetGamerRef.Gamertag, " has joined session", "", Color.DarkRed, Color.DarkRed);
				break;
			}
		}
		if (networkSession.IsHost)
		{
			AIBase.GamerJoinedSession(e.Gamer);
		}
		AIBase.Clans.PlayerJoinSession(e.Gamer);
	}

	private static void GamerLeftEventHandler(object sender, GamerLeftEventArgs e)
	{
		if (e.Gamer.IsLocal)
		{
			return;
		}
		if (networkSession != null && networkSession.IsHost)
		{
			AIBase.RemovePlayerFromNetworkEvents(e.Gamer);
		}
		ApocZSaveDataCls.DeletePlayerTents(e.Gamer);
		for (int i = 0; i < 15; i++)
		{
			if (NetPlayers[i].NetGamerRef == e.Gamer)
			{
				MessagePump.AddGamerMessage(NetPlayers[i].NetGamerRef.Gamertag, " has left session", "", Color.DarkRed, Color.DarkRed);
				AIBase.DetachRemotePlayerFromVehicle(NetPlayers[i].NetGamerRef, 0, byte.MaxValue, NetPlayers[i].vehicleSeat);
				NetPlayers[i].mRagdoll.IsValid = false;
				NetPlayers[i].NetGamerId = 0;
				NetPlayers[i].NetGamerRef = null;
				break;
			}
		}
		AIBase.Clans.PlayerLeftSession(e.Gamer);
	}

	private static void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
	{
		errorMessage = e.EndReason.ToString();
		networkSession.Dispose();
		networkSession = null;
	}

	private static void SessionHostChanged(object sender, HostChangedEventArgs e)
	{
		MessagePump.AddGamerMessage("Host Changed: NewHost = ", "", e.NewHost.Gamertag, Color.DarkGreen, Color.DarkGreen);
		MessagePump.AddGamerMessage("OldHost = " + e.OldHost, "", "", Color.DarkRed, Color.DarkRed);
		for (int i = 0; i < 15; i++)
		{
			if (NetPlayers[i].gamerTag == e.NewHost.Gamertag)
			{
				NetPlayers[i].IsHost = true;
			}
			if (NetPlayers[i].gamerTag == e.OldHost.Gamertag)
			{
				NetPlayers[i].IsHost = false;
			}
		}
		HostMigrateTimer = 6f;
		if (networkSession.IsHost)
		{
			if (!ApocZSaveDataCls.SyncingToServer)
			{
				return;
			}
			ApocZSaveDataCls.SyncingToServer = false;
			AIBase.HostCreateWorld(networkSession.Host);
			AIBase.ScheduledWorldDownloads.Clear();
			{
				foreach (NetworkGamer allGamer in networkSession.AllGamers)
				{
					if (!allGamer.IsLocal)
					{
						AIBase.GamerJoinedSession(allGamer);
					}
				}
				return;
			}
		}
		InSessionTimer = 0f;
		if (ApocZSaveDataCls.SyncingToServer)
		{
			packetWriter.Write((byte)146);
			networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder, networkSession.Host);
		}
	}

	public static void ExitSession()
	{
		if (networkSession != null)
		{
			networkSession.Dispose();
			networkSession = null;
			for (int i = 0; i < 15; i++)
			{
				NetPlayers[i].gamerTag = "guest";
				NetPlayers[i].IsHost = false;
				NetPlayers[i].NetGamerId = 0;
				NetPlayers[i].NetGamerRef = null;
			}
		}
	}

	public static void ServerSendToClient(PacketWriter pWriter, NetworkGamer gamer)
	{
		if (networkSession != null && networkSession.IsHost)
		{
			LocalNetworkGamer localNetworkGamer = (LocalNetworkGamer)networkSession.Host;
			localNetworkGamer.SendData(pWriter, SendDataOptions.InOrder);
		}
	}
}
