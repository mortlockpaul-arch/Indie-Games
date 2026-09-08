using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class EGENetWorkNext
{
	public const int maxNetGamers = 15;

	public const int maxLocalGamers = 1;

	public static bool inviteToGameScheduled;

	public static NetworkSession networkSession;

	public static PacketWriter packetWriter;

	public static PacketReader packetReader;

	public static ePacketTypes LastPacketRead;

	public static IAsyncResult asyncResultJoin;

	public static IAsyncResult asyncResultFind;

	public static bool NetPlayersInitialized;

	public static PlayerBase[] NetPlayers;

	private static int currentValidListIndex;

	private static List<MyNetworkSessionEntry>[] MyAvailableSessions;

	private static AvailableNetworkSessionCollection availableSessions;

	private static NetworkSessionProperties sessionProperties;

	private static string errorMessage;

	public static float HostMigrateTimer;

	public static float InSessionTimer;

	public static bool HostCreatingWorld;

	private static HalfVector2 readPHV2;

	private static HalfVector4 readPHV4;

	private static NormalizedByte4 readNB4_0;

	private static NormalizedByte4 readNB4_1;

	private static Vector2 readUnpackerV2;

	private static Vector4 readUnpackerV4;

	private static Cue FacePunchSound;

	private static int readDataBufferSize;

	private static byte[] readDataBuffer;

	private static bool initialized;

	public static bool JoinInviteInProgress;

	public static float NetworkCurrentPing;

	private static float NetworkPingTimer;

	private static float NetworkUpdateTimeStep;

	private static float LocalPlayerUpdateTimer;

	private static float ServerTransmitUpdateTimer;

	private static int ClientUpdateTimer;

	private static int ServerUpdateTimer;

	private static Vector4 readDir;

	private static Vector2 readSpeedSteer;

	private static Vector3 direction;

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
			Texture2D texBlack = LevelBaseMenu.texBlack;
			Menu.spriteBatch.Draw(texBlack, EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Bounds, black);
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
				if (e < 0 || e >= ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions).Count)
				{
					MessagePump.AddGamerMessage("Join Session Error...", "", "", Color.DarkRed, Color.DarkRed);
					MessagePump.AddMessage("Invalid Session Or Session ended...");
					return NetSessionRetCodes.Error;
				}
				asyncResultJoin = NetworkSession.BeginJoin(((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[e], null, null);
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
				LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerId = ((ReadOnlyCollection<LocalNetworkGamer>)networkSession.LocalGamers)[0].Id;
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
			if (((ReadOnlyCollection<AvailableNetworkSession>)availableSessions).Count == 0)
			{
				errorMessage = "No network sessions found.";
			}
			MyAvailableSessions[num].Clear();
			for (int i = 0; i < ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions).Count; i++)
			{
				for (int j = 0; j < MyAvailableSessions[currentValidListIndex].Count; j++)
				{
					if (MyAvailableSessions[currentValidListIndex][j].HostGamertag == ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[i].HostGamertag)
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
			for (int k = 0; k < ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions).Count; k++)
			{
				bool flag = false;
				for (int l = 0; l < MyAvailableSessions[num].Count; l++)
				{
					if (MyAvailableSessions[num][l].HostGamertag == ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[k].HostGamertag)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					MyNetworkSessionEntry myNetworkSessionEntry2 = new MyNetworkSessionEntry();
					myNetworkSessionEntry2.HostGamertag = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[k].HostGamertag;
					myNetworkSessionEntry2.CurrentGamerCount = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[k].CurrentGamerCount;
					myNetworkSessionEntry2.OpenPublicGamerSlots = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[k].OpenPublicGamerSlots;
					if (((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[k].QualityOfService.IsAvailable)
					{
						myNetworkSessionEntry2.BytesPerSecondDownstream = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[k].QualityOfService.BytesPerSecondDownstream;
						myNetworkSessionEntry2.BytesPerSecondUpstream = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[k].QualityOfService.BytesPerSecondUpstream;
						myNetworkSessionEntry2.Ping = (int)((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[k].QualityOfService.AverageRoundtripTime.TotalMilliseconds;
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
			for (int i = 0; i < ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions).Count; i++)
			{
				if (MyAvailableSessions[currentValidListIndex] == null || i >= MyAvailableSessions[currentValidListIndex].Count)
				{
					continue;
				}
				try
				{
					MyAvailableSessions[currentValidListIndex][i].HostGamertag = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[i].HostGamertag;
					MyAvailableSessions[currentValidListIndex][i].CurrentGamerCount = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[i].CurrentGamerCount;
					MyAvailableSessions[currentValidListIndex][i].OpenPublicGamerSlots = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[i].OpenPublicGamerSlots;
					if (((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[i].QualityOfService.IsAvailable)
					{
						MyAvailableSessions[currentValidListIndex][i].BytesPerSecondDownstream = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[i].QualityOfService.BytesPerSecondDownstream;
						MyAvailableSessions[currentValidListIndex][i].BytesPerSecondUpstream = ((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[i].QualityOfService.BytesPerSecondUpstream;
						MyAvailableSessions[currentValidListIndex][i].Ping = (int)((ReadOnlyCollection<AvailableNetworkSession>)availableSessions)[i].QualityOfService.AverageRoundtripTime.TotalMilliseconds;
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
		GamerCollection<LocalNetworkGamer>.GamerCollectionEnumerator enumerator = networkSession.LocalGamers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				LocalNetworkGamer current = enumerator.Current;
				UpdateLocalGamer(current);
			}
		}
		finally
		{
			enumerator.Dispose();
		}
		GamerCollection<NetworkGamer>.GamerCollectionEnumerator enumerator2 = networkSession.AllGamers.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				NetworkGamer current2 = enumerator2.Current;
				if (current2.IsLocal)
				{
					continue;
				}
				PlayerBase playerBase = current2.Tag as PlayerBase;
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
		}
		finally
		{
			enumerator2.Dispose();
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
		GamerCollection<LocalNetworkGamer>.GamerCollectionEnumerator enumerator3 = networkSession.LocalGamers.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				LocalNetworkGamer current3 = enumerator3.Current;
				if (current3.IsHost)
				{
					ServerReadInputFromClients(current3);
				}
				else
				{
					ClientReadGameStateFromServer(current3);
				}
			}
		}
		finally
		{
			enumerator3.Dispose();
		}
		if (networkSession != null && !networkSession.IsHost)
		{
			NetworkPingTimer += 0.0333334f;
			if (NetworkPingTimer > 10f)
			{
				NetworkPingTimer = 0f;
				((BinaryWriter)packetWriter).Write((byte)149);
				((ReadOnlyCollection<LocalNetworkGamer>)networkSession.LocalGamers)[0].SendData(packetWriter, SendDataOptions.InOrder, networkSession.Host);
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
		GamerCollection<NetworkGamer>.GamerCollectionEnumerator enumerator = networkSession.AllGamers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NetworkGamer current = enumerator.Current;
				if (!current.IsLocal)
				{
					PlayerNetWorkPacket.WriteLocalGamer(packetWriter, gamer);
					if (packetWriter.Length > 0)
					{
						gamer.SendData(packetWriter, SendDataOptions.InOrder, current);
					}
				}
			}
		}
		finally
		{
			enumerator.Dispose();
		}
	}

	private static void UpdateServer()
	{
		ServerUpdateTimer--;
		if (ServerUpdateTimer <= 0)
		{
			ServerUpdateTimer = 8;
			LocalNetworkGamer localNetworkGamer = (LocalNetworkGamer)networkSession.Host;
			GamerCollection<NetworkGamer>.GamerCollectionEnumerator enumerator = networkSession.AllGamers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NetworkGamer current = enumerator.Current;
					if (!current.IsLocal)
					{
						PlayerNetWorkPacket.WriteLocalGamer(packetWriter, localNetworkGamer);
						if (packetWriter.Length > 0)
						{
							localNetworkGamer.SendData(packetWriter, SendDataOptions.InOrder, current);
						}
					}
				}
			}
			finally
			{
				enumerator.Dispose();
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
			NetworkGamer sender = null;
			while (gamer.IsDataAvailable)
			{
				gamer.ReceiveData(packetReader, out sender);
				if (sender.IsLocal)
				{
					continue;
				}
				switch (LastPacketRead = (ePacketTypes)((BinaryReader)packetReader).ReadByte())
				{
				case ePacketTypes.PingHost:
					((BinaryWriter)EGENetWorkNext.packetWriter).Write((byte)149);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(sender.Id);
					((ReadOnlyCollection<LocalNetworkGamer>)networkSession.LocalGamers)[0].SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
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
					float num10 = (playerBase6.vecPosition - playerBase6.vecTargetPosition).LengthSquared();
					if (num10 > 160000f)
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
					ushort num5 = ((BinaryReader)packetReader).ReadUInt16();
					byte b8 = ((BinaryReader)packetReader).ReadByte();
					if (AIBase.CanPlayerAttachToVehicle(sender, num5, b8))
					{
						((BinaryWriter)EGENetWorkNext.packetWriter).Write((byte)114);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(sender.Id);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(num5);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(b8);
						gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.ReliableInOrder);
					}
					break;
				}
				case ePacketTypes.VehicleGamerDetach:
				{
					ushort num3 = ((BinaryReader)packetReader).ReadUInt16();
					byte b6 = ((BinaryReader)packetReader).ReadByte();
					byte b7 = ((BinaryReader)packetReader).ReadByte();
					((BinaryWriter)EGENetWorkNext.packetWriter).Write((byte)115);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(sender.Id);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(num3);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(b6);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(b7);
					gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.ReliableInOrder);
					AIBase.DetachRemotePlayerFromVehicle(sender, num3, b7, b6);
					break;
				}
				case ePacketTypes.VehicleGamerTranslation:
				{
					ushort num11 = ((BinaryReader)packetReader).ReadUInt16();
					Vector3 vector4 = packetReader.ReadVector3();
					byte b17 = ((BinaryReader)packetReader).ReadByte();
					readNB4_0.PackedValue = ((BinaryReader)packetReader).ReadUInt32();
					readPHV2.PackedValue = ((BinaryReader)packetReader).ReadUInt32();
					byte b18 = ((BinaryReader)packetReader).ReadByte();
					readDir = readNB4_0.ToVector4();
					readSpeedSteer = readPHV2.ToVector2();
					direction.X = readDir.X;
					direction.Y = readDir.Y;
					direction.Z = readDir.Z;
					float reverse = ((float)(int)b17 - 127f) * 0.007874f;
					AIBase.VehicleNetworkTranslation(sender.Id, num11, vector4, direction, readSpeedSteer.X, readSpeedSteer.Y, reverse, b18);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write((byte)116);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(sender.Id);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(num11);
					EGENetWorkNext.packetWriter.Write(vector4);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(b17);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(readNB4_0.PackedValue);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(readPHV2.PackedValue);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(b18);
					gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					break;
				}
				case ePacketTypes.VehicleData:
				{
					int num4 = ((BinaryReader)packetReader).ReadInt32();
					AIBase.UpdateVehicleData(packetReader, num4);
					AIBase.AllVehicles[num4].SendVehicleDataPacket(EGENetWorkNext.packetWriter, num4, isHost: true);
					break;
				}
				case ePacketTypes.ZombieDamageData:
				{
					byte b10 = ((BinaryReader)packetReader).ReadByte();
					byte b11 = ((BinaryReader)packetReader).ReadByte();
					byte b12 = ((BinaryReader)packetReader).ReadByte();
					NetworkGamer networkGamer3 = networkSession.FindGamerById(b10);
					PlayerBase playerBase4 = ((networkGamer3 != null) ? (networkGamer3.Tag as PlayerBase) : null);
					if (playerBase4 != null)
					{
						float num8 = playerBase4.BloodLevel - (float)(int)b11;
						playerBase4.BloodLevel = ((num8 < 0f) ? 0f : num8);
						playerBase4.BloodLoss = ((b12 > 0) ? 1 : 0);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write((byte)129);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(b10);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(b11);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(b12);
						gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					}
					break;
				}
				case ePacketTypes.DamageData:
				{
					byte b13 = ((BinaryReader)packetReader).ReadByte();
					byte b14 = ((BinaryReader)packetReader).ReadByte();
					byte b15 = ((BinaryReader)packetReader).ReadByte();
					byte b16 = ((BinaryReader)packetReader).ReadByte();
					NetworkGamer networkGamer4 = networkSession.FindGamerById(b14);
					PlayerBase playerBase5 = ((networkGamer4 != null) ? (networkGamer4.Tag as PlayerBase) : null);
					if (playerBase5 != null)
					{
						float num9 = playerBase5.BloodLevel - (float)(int)b15;
						playerBase5.BloodLevel = ((num9 < 0f) ? 0f : num9);
						playerBase5.BloodLoss = ((b16 > 0) ? 1 : 0);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write((byte)130);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(b13);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(b14);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(b15);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(b16);
						gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
						if (networkGamer4.IsLocal && b13 == 5)
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
					byte b9 = ((BinaryReader)packetReader).ReadByte();
					NetworkGamer networkGamer2 = networkSession.FindGamerById(b9);
					PlayerBase playerBase3 = ((networkGamer2 != null) ? (networkGamer2.Tag as PlayerBase) : null);
					if (playerBase3 != null && !networkGamer2.IsLocal)
					{
						Vector3 damageDir = Vector3.Zero;
						playerBase3.ProcessDeath(DamegePacketType.None, ref damageDir);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write((byte)132);
						((BinaryWriter)EGENetWorkNext.packetWriter).Write(b9);
						gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					}
					break;
				}
				case ePacketTypes.GamerSpawned:
				{
					Vector3 vector = packetReader.ReadVector3();
					Vector3 vector2 = packetReader.ReadVector3();
					Vector3 vector3 = packetReader.ReadVector3();
					float num6 = ((BinaryReader)packetReader).ReadSingle();
					float num7 = ((BinaryReader)packetReader).ReadSingle();
					((BinaryWriter)EGENetWorkNext.packetWriter).Write((byte)133);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(sender.Id);
					EGENetWorkNext.packetWriter.Write(vector);
					EGENetWorkNext.packetWriter.Write(vector2);
					EGENetWorkNext.packetWriter.Write(vector3);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(num6);
					((BinaryWriter)EGENetWorkNext.packetWriter).Write(num7);
					gamer.SendData(EGENetWorkNext.packetWriter, SendDataOptions.InOrder);
					PlayerBase playerBase2 = sender.Tag as PlayerBase;
					playerBase2.IsAttached0 = false;
					playerBase2.vecPosition = vector;
					playerBase2.vecCharacterDir = vector2;
					playerBase2.Angles = vector3;
					playerBase2.Spawned = true;
					playerBase2.Health = 100f;
					playerBase2.BloodLevel = num6;
					playerBase2.BloodLoss = num7;
					break;
				}
				case ePacketTypes.ZombieDeath:
				{
					byte senderId = ((BinaryReader)packetReader).ReadByte();
					AIBase.ZombieDeath(packetReader, senderId, netBroadcast: true);
					break;
				}
				case ePacketTypes.ZombieDeathConfirm:
				{
					ushort uid3 = ((BinaryReader)packetReader).ReadUInt16();
					ZombiePositionGrid.ConfirmSync(uid3);
					break;
				}
				case ePacketTypes.ZombieLineOfSight:
				{
					ushort uid2 = ((BinaryReader)packetReader).ReadUInt16();
					byte disQuant = ((BinaryReader)packetReader).ReadByte();
					AIBase.ZombieSetLOS(uid2, disQuant, sender);
					break;
				}
				case ePacketTypes.ZombieClientRequestNewState:
				{
					ushort uid = ((BinaryReader)packetReader).ReadUInt16();
					byte anim = ((BinaryReader)packetReader).ReadByte();
					byte state = ((BinaryReader)packetReader).ReadByte();
					AIBase.ZombieNewState(sender.Id, uid, anim, state);
					break;
				}
				case ePacketTypes.PlayerOptionsSync:
				{
					byte b = ((BinaryReader)packetReader).ReadByte();
					byte b2 = ((BinaryReader)packetReader).ReadByte();
					float num = (int)((BinaryReader)packetReader).ReadByte();
					float num2 = (int)((BinaryReader)packetReader).ReadByte();
					byte b3 = ((BinaryReader)packetReader).ReadByte();
					byte b4 = ((BinaryReader)packetReader).ReadByte();
					byte b5 = ((BinaryReader)packetReader).ReadByte();
					NetworkGamer networkGamer = networkSession.FindGamerById(b);
					if (networkGamer != null && !networkGamer.IsLocal && networkGamer.Tag is PlayerBase playerBase)
					{
						playerBase.SetCharacter(b2, num, num2);
						playerBase.FlashLightOn = b4 > 0;
						playerBase.CurrentBacpPack = b3;
						playerBase.CurrentDay = b5;
						PacketWriter packetWriter = EGENetWorkNext.packetWriter;
						((BinaryWriter)packetWriter).Write((byte)135);
						((BinaryWriter)packetWriter).Write(b);
						((BinaryWriter)packetWriter).Write(b2);
						((BinaryWriter)packetWriter).Write((byte)num);
						((BinaryWriter)packetWriter).Write((byte)num2);
						((BinaryWriter)packetWriter).Write(b3);
						((BinaryWriter)packetWriter).Write(b4);
						((BinaryWriter)packetWriter).Write(b5);
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
			NetworkGamer sender = null;
			while (gamer.IsDataAvailable)
			{
				gamer.ReceiveData(packetReader, out sender);
				while (packetReader.Position < packetReader.Length)
				{
					switch (LastPacketRead = (ePacketTypes)((BinaryReader)packetReader).ReadByte())
					{
					case ePacketTypes.PingHost:
					{
						byte b9 = ((BinaryReader)packetReader).ReadByte();
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
						PlayerBase playerBase5 = sender.Tag as PlayerBase;
						playerBase5.vecTargetPosition = vecTargetPosition;
						float num3 = (playerBase5.vecPosition - playerBase5.vecTargetPosition).LengthSquared();
						if (num3 > 160000f)
						{
							playerBase5.vecPosition = playerBase5.vecTargetPosition;
						}
						break;
					}
					case ePacketTypes.PlayerDamage:
					{
						byte gameId2 = ((BinaryReader)packetReader).ReadByte();
						int num = ((BinaryReader)packetReader).ReadByte();
						int num2 = ((BinaryReader)packetReader).ReadByte();
						NetworkGamer networkGamer2 = networkSession.FindGamerById(gameId2);
						if (networkGamer2 != null && networkGamer2.IsLocal)
						{
							PlayerBase playerBase2 = networkGamer2.Tag as PlayerBase;
							playerBase2.Health -= num;
							playerBase2.BloodLoss = ((playerBase2.BloodLoss > (float)num2) ? playerBase2.BloodLoss : ((float)num2));
						}
						break;
					}
					case ePacketTypes.WorldItemCreate:
						AIBase.AllWorldItems.ReadCreateFromServer(packetReader, sender);
						break;
					case ePacketTypes.WorldItemUpdate:
					{
						byte b16 = ((BinaryReader)packetReader).ReadByte();
						ItemCls itemCls3 = new ItemCls();
						itemCls3.NetworkRead(packetReader);
						AIBase.AllWorldItems.PickupItem(itemCls3, gamer.Id == b16);
						break;
					}
					case ePacketTypes.WorldItemUpdateDesc:
					{
						ItemCls itemCls = new ItemCls();
						itemCls.NetworkRead(packetReader);
						AIBase.AllWorldItems.ClientUpdateItem(itemCls);
						break;
					}
					case ePacketTypes.WorldItemAdd:
					{
						byte b15 = ((BinaryReader)packetReader).ReadByte();
						ItemCls itemCls2 = new ItemCls();
						itemCls2.NetworkRead(packetReader);
						AIBase.AllWorldItems.AddItemToList(itemCls2.pos, itemCls2);
						if (gamer.Id == b15)
						{
						}
						break;
					}
					case ePacketTypes.WorldCreateLoadTents:
					{
						byte b10 = ((BinaryReader)packetReader).ReadByte();
						if (gamer.Id == b10)
						{
							ApocZSaveDataCls.ScheduleWorldItemLoad = true;
						}
						break;
					}
					case ePacketTypes.WorldCreateDone:
					{
						byte b11 = ((BinaryReader)packetReader).ReadByte();
						if (gamer.Id == b11)
						{
							ApocZSaveDataCls.SyncingToServer = false;
						}
						break;
					}
					case ePacketTypes.VehicleGamerAttach:
					{
						byte gamerId = ((BinaryReader)packetReader).ReadByte();
						ushort uid5 = ((BinaryReader)packetReader).ReadUInt16();
						byte vSeat2 = ((BinaryReader)packetReader).ReadByte();
						AIBase.AttachPlayerToVehicle(gamerId, uid5, vSeat2);
						break;
					}
					case ePacketTypes.VehicleGamerDetach:
					{
						byte gameId5 = ((BinaryReader)packetReader).ReadByte();
						ushort uid4 = ((BinaryReader)packetReader).ReadUInt16();
						byte vSeat = ((BinaryReader)packetReader).ReadByte();
						byte headLights = ((BinaryReader)packetReader).ReadByte();
						AIBase.DetachRemotePlayerFromVehicle(networkSession.FindGamerById(gameId5), uid4, headLights, vSeat);
						break;
					}
					case ePacketTypes.VehicleGamerTranslation:
					{
						byte gamerId2 = ((BinaryReader)packetReader).ReadByte();
						ushort uid6 = ((BinaryReader)packetReader).ReadUInt16();
						Vector3 pos = packetReader.ReadVector3();
						byte b8 = ((BinaryReader)packetReader).ReadByte();
						readNB4_0.PackedValue = ((BinaryReader)packetReader).ReadUInt32();
						readPHV2.PackedValue = ((BinaryReader)packetReader).ReadUInt32();
						byte headLights2 = ((BinaryReader)packetReader).ReadByte();
						readDir = readNB4_0.ToVector4();
						readSpeedSteer = readPHV2.ToVector2();
						direction.X = readDir.X;
						direction.Y = readDir.Y;
						direction.Z = readDir.Z;
						float reverse = ((float)(int)b8 - 127f) * 0.007874f;
						AIBase.VehicleNetworkTranslation(gamerId2, uid6, pos, direction, readSpeedSteer.X, readSpeedSteer.Y, reverse, headLights2);
						break;
					}
					case ePacketTypes.VehicleData:
					{
						int vehicleIndex2 = ((BinaryReader)packetReader).ReadInt32();
						AIBase.UpdateVehicleData(packetReader, vehicleIndex2);
						break;
					}
					case ePacketTypes.VehicleSpawn:
					{
						int vehicleIndex = ((BinaryReader)packetReader).ReadInt32();
						AIBase.VehicleSpawn(packetReader, vehicleIndex);
						break;
					}
					case ePacketTypes.SunPosition:
						LevelOutside.SunAngle = ((BinaryReader)packetReader).ReadSingle();
						break;
					case ePacketTypes.ZombieAdd:
						AIBase.HostSendZombieToClient(packetReader, sender);
						break;
					case ePacketTypes.ZombieDamageData:
					{
						byte b12 = ((BinaryReader)packetReader).ReadByte();
						byte b13 = ((BinaryReader)packetReader).ReadByte();
						byte b14 = ((BinaryReader)packetReader).ReadByte();
						if (gamer.Id == b12)
						{
							break;
						}
						NetworkGamer networkGamer5 = networkSession.FindGamerById(b12);
						PlayerBase playerBase6 = ((networkGamer5 != null) ? (networkGamer5.Tag as PlayerBase) : null);
						if (playerBase6 != null && !networkGamer5.IsLocal)
						{
							float num5 = playerBase6.BloodLevel - (float)(int)b13;
							playerBase6.BloodLevel = ((num5 < 0f) ? 0f : num5);
							if (b14 > 0 && playerBase6.BloodLoss == 0f)
							{
								playerBase6.BloodLoss = 0.01f;
							}
						}
						break;
					}
					case ePacketTypes.DamageData:
					{
						byte b4 = ((BinaryReader)packetReader).ReadByte();
						byte b5 = ((BinaryReader)packetReader).ReadByte();
						byte b6 = ((BinaryReader)packetReader).ReadByte();
						byte b7 = ((BinaryReader)packetReader).ReadByte();
						if (gamer.Id == b5)
						{
							float num4 = ((PlayerBase)gamer.Tag).BloodLevel - (float)(int)b6;
							((PlayerBase)gamer.Tag).BloodLevel = ((num4 < 0f) ? 0f : num4);
							if (b7 > 0 && ((PlayerBase)gamer.Tag).BloodLoss == 0f)
							{
								((PlayerBase)gamer.Tag).BloodLoss = 0.01f;
							}
							if (b4 == 5)
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
						byte gameId4 = ((BinaryReader)packetReader).ReadByte();
						NetworkGamer networkGamer4 = networkSession.FindGamerById(gameId4);
						PlayerBase playerBase4 = ((networkGamer4 != null) ? (networkGamer4.Tag as PlayerBase) : null);
						if (playerBase4 != null && !networkGamer4.IsLocal)
						{
							Vector3 damageDir = Vector3.Zero;
							playerBase4.ProcessDeath(DamegePacketType.None, ref damageDir);
						}
						break;
					}
					case ePacketTypes.GamerSpawned:
					{
						byte gameId3 = ((BinaryReader)packetReader).ReadByte();
						Vector3 vecPosition = packetReader.ReadVector3();
						Vector3 vecCharacterDir = packetReader.ReadVector3();
						Vector3 angles = packetReader.ReadVector3();
						float bloodLevel = ((BinaryReader)packetReader).ReadSingle();
						float bloodLoss = ((BinaryReader)packetReader).ReadSingle();
						NetworkGamer networkGamer3 = networkSession.FindGamerById(gameId3);
						if (networkGamer3 != null && !networkGamer3.IsLocal)
						{
							PlayerBase playerBase3 = networkGamer3.Tag as PlayerBase;
							playerBase3.IsAttached0 = false;
							playerBase3.vecPosition = vecPosition;
							playerBase3.vecCharacterDir = vecCharacterDir;
							playerBase3.Angles = angles;
							playerBase3.Spawned = true;
							playerBase3.Health = 100f;
							playerBase3.BloodLevel = bloodLevel;
							playerBase3.BloodLoss = bloodLoss;
						}
						break;
					}
					case ePacketTypes.ZombieDeath:
					{
						byte senderId = ((BinaryReader)packetReader).ReadByte();
						AIBase.ZombieDeath(packetReader, senderId, netBroadcast: false);
						break;
					}
					case ePacketTypes.ZombieDeathConfirm:
					{
						ushort uid3 = ((BinaryReader)packetReader).ReadUInt16();
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
						ushort uid2 = ((BinaryReader)packetReader).ReadUInt16();
						ushort pathingIndex = ((BinaryReader)packetReader).ReadUInt16();
						AIBase.ZombieUpdatePathing(uid2, pathingIndex);
						break;
					}
					case ePacketTypes.ZombieNewState:
					{
						ushort uid = ((BinaryReader)packetReader).ReadUInt16();
						byte anim = ((BinaryReader)packetReader).ReadByte();
						byte state = ((BinaryReader)packetReader).ReadByte();
						byte b3 = ((BinaryReader)packetReader).ReadByte();
						if (gamer.Id != b3)
						{
							AIBase.ZombieNewState(b3, uid, anim, state);
						}
						break;
					}
					case ePacketTypes.ResetWorld:
					{
						byte b2 = ((BinaryReader)packetReader).ReadByte();
						if (gamer.Id == b2)
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
						byte gameId = ((BinaryReader)packetReader).ReadByte();
						byte e = ((BinaryReader)packetReader).ReadByte();
						float si = (int)((BinaryReader)packetReader).ReadByte();
						float pi = (int)((BinaryReader)packetReader).ReadByte();
						byte currentBacpPack = ((BinaryReader)packetReader).ReadByte();
						byte b = ((BinaryReader)packetReader).ReadByte();
						byte currentDay = ((BinaryReader)packetReader).ReadByte();
						NetworkGamer networkGamer = networkSession.FindGamerById(gameId);
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
			((BinaryWriter)packetWriter).Write((byte)135);
			((BinaryWriter)packetWriter).Write(e.Gamer.Id);
			((BinaryWriter)packetWriter).Write(playerBase.CharacterIndex);
			((BinaryWriter)packetWriter).Write((byte)playerBase.ShirtIndex);
			((BinaryWriter)packetWriter).Write((byte)playerBase.PantstIndex);
			((BinaryWriter)packetWriter).Write((byte)playerBase.CurrentBacpPack);
			((BinaryWriter)packetWriter).Write((byte)(playerBase.FlashLightOn ? 1u : 0u));
			((BinaryWriter)packetWriter).Write((byte)playerBase.CurrentDay);
			((ReadOnlyCollection<LocalNetworkGamer>)networkSession.LocalGamers)[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
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
			GamerCollection<NetworkGamer>.GamerCollectionEnumerator enumerator = networkSession.AllGamers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NetworkGamer current = enumerator.Current;
					if (!current.IsLocal)
					{
						AIBase.GamerJoinedSession(current);
					}
				}
				return;
			}
			finally
			{
				enumerator.Dispose();
			}
		}
		InSessionTimer = 0f;
		if (ApocZSaveDataCls.SyncingToServer)
		{
			((BinaryWriter)packetWriter).Write((byte)146);
			((ReadOnlyCollection<LocalNetworkGamer>)networkSession.LocalGamers)[0].SendData(packetWriter, SendDataOptions.ReliableInOrder, networkSession.Host);
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

	static EGENetWorkNext()
	{
		inviteToGameScheduled = false;
		networkSession = null;
		packetWriter = new PacketWriter();
		packetReader = new PacketReader();
		LastPacketRead = ePacketTypes.DamageData;
		asyncResultJoin = null;
		asyncResultFind = null;
		NetPlayersInitialized = false;
		NetPlayers = new PlayerBase[15];
		currentValidListIndex = 0;
		sessionProperties = new NetworkSessionProperties();
		HostMigrateTimer = 0f;
		InSessionTimer = 0f;
		HostCreatingWorld = false;
		readPHV2 = default(HalfVector2);
		readPHV4 = default(HalfVector4);
		readNB4_0 = default(NormalizedByte4);
		readNB4_1 = default(NormalizedByte4);
		readUnpackerV2 = Vector2.Zero;
		readUnpackerV4 = Vector4.Zero;
		readDataBufferSize = 32768;
		readDataBuffer = new byte[readDataBufferSize];
		initialized = false;
		JoinInviteInProgress = false;
		NetworkCurrentPing = 1000f;
		NetworkPingTimer = 0f;
		NetworkUpdateTimeStep = 0.5f;
		LocalPlayerUpdateTimer = 0f;
		ServerTransmitUpdateTimer = 0f;
		ClientUpdateTimer = 0;
		ServerUpdateTimer = 0;
		readDir = Vector4.Zero;
		readSpeedSteer = Vector2.Zero;
		direction = Vector3.Zero;
	}
}
