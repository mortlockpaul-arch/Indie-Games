using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class ApocZSaveDataCls
{
	public static bool DeployingTentsToServer = false;

	public static bool SyncingToServer = false;

	public static float DelayForSaveTimer = 0f;

	private static List<ItemCls> LocalPlayerTents = new List<ItemCls>();

	private static List<ItemCls> LocalTentContents = new List<ItemCls>();

	private static List<Vector3> LocalLootedTents = new List<Vector3>();

	private static List<ItemCls> AllTentContents = new List<ItemCls>();

	private static int LoadTentIndex = 0;

	private static int LoadContentsIndex = 0;

	public static bool scheduleWorldItemLoad = false;

	private static bool justPurchased = false;

	private static bool TentDeployRegulatorToggle = true;

	private static float msgLootTimer = 0f;

	private static float confirmDeployTimer = 0f;

	private static int syncticker = 0;

	private static float contentsExceedTimer = 0f;

	private static int maxLocalTents = 16;

	private static ItemCls[] removeTentIndices = new ItemCls[maxLocalTents];

	public static bool ScheduleWorldItemLoad
	{
		get
		{
			return scheduleWorldItemLoad;
		}
		set
		{
			scheduleWorldItemLoad = value;
		}
	}

	public static void Reset()
	{
		LocalPlayerTents.Clear();
		LocalTentContents.Clear();
		LocalLootedTents.Clear();
		LoadTentIndex = 0;
		LoadContentsIndex = 0;
		DeployingTentsToServer = false;
	}

	public static void SpawnTestEquipment()
	{
	}

	public static void Save()
	{
		if (!(EGENetWorkNext.HostMigrateTimer > 0f) && !SyncingToServer)
		{
			Storage.SavePlayerWorldItems(LocalPlayerTents, LocalTentContents);
		}
	}

	public static bool Load()
	{
		if (!DataEncoder.DataBufferIsLoaded)
		{
			return false;
		}
		LocalPlayerTents.Clear();
		LocalTentContents.Clear();
		Storage.LoadPlayerWorldItems(LocalPlayerTents, LocalTentContents);
		byte ownerNetId = (byte)((EGENetWorkNext.networkSession != null) ? EGENetWorkNext.networkSession.LocalGamers[0].Id : 0);
		if (LocalPlayerTents != null)
		{
			for (int i = 0; i < LocalPlayerTents.Count; i++)
			{
				LocalPlayerTents[i].uid = 0;
				LocalPlayerTents[i].ownerNetId = ownerNetId;
			}
		}
		if (LocalTentContents != null)
		{
			for (int j = 0; j < LocalTentContents.Count; j++)
			{
				LocalTentContents[j].uid = 0;
				LocalTentContents[j].ownerNetId = ownerNetId;
			}
		}
		LocalLootedTents.Clear();
		LoadTentIndex = 0;
		LoadContentsIndex = 0;
		DeployingTentsToServer = true;
		return true;
	}

	public static void Update()
	{
		if (ScheduleWorldItemLoad && Load())
		{
			ScheduleWorldItemLoad = false;
		}
		DelayForSaveTimer -= 0.03f;
		msgLootTimer -= 0.03f;
		confirmDeployTimer -= 0.03f;
		contentsExceedTimer -= 0.03f;
		if (LevelBaseMenu.isTrialMode && !justPurchased && !Guide.IsTrialMode)
		{
			string gamerTag = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag;
			Storage.PlayerCharacterFilename = gamerTag + "_Character";
			Storage.PlayerStatisFilename = gamerTag + "_OnlineStatis";
			Storage.PlayerInventoryFilename = gamerTag + "_OnlineInventory";
			Storage.PlayerTentsFilename = gamerTag + "_OnlineTents";
			Storage.NewSaveInventory();
			Save();
			justPurchased = true;
		}
		if ((LocalPlayerTents != null && LoadTentIndex < LocalPlayerTents.Count) || (LocalTentContents != null && LoadContentsIndex < LocalTentContents.Count))
		{
			DeployingTentsToServer = true;
			TentDeployRegulatorToggle = !TentDeployRegulatorToggle;
			if (TentDeployRegulatorToggle)
			{
				if (EGENetWorkNext.networkSession != null)
				{
					_ = EGENetWorkNext.networkSession.LocalGamers[0].Id;
				}
				if (LoadTentIndex < LocalPlayerTents.Count)
				{
					AIBase.AllWorldItems.DropItemInworld(LocalPlayerTents[LoadTentIndex].pos, LocalPlayerTents[LoadTentIndex]);
					LoadTentIndex++;
				}
				if (LoadContentsIndex < LocalTentContents.Count)
				{
					AIBase.AllWorldItems.DropItemInworld(LocalTentContents[LoadContentsIndex].pos, LocalTentContents[LoadContentsIndex]);
					LoadContentsIndex++;
				}
			}
			if (LoadTentIndex >= LocalPlayerTents.Count && LoadContentsIndex >= LocalTentContents.Count)
			{
				DeployingTentsToServer = false;
				if (EGENetWorkNext.networkSession != null && EGENetWorkNext.networkSession.IsHost)
				{
					SyncingToServer = false;
				}
			}
		}
		else
		{
			if (!(confirmDeployTimer < 0f))
			{
				return;
			}
			confirmDeployTimer = 2f;
			bool flag = false;
			if (!DeployingTentsToServer && LocalPlayerTents != null)
			{
				for (int i = 0; i < LocalPlayerTents.Count; i++)
				{
					if (LocalPlayerTents[i].uid == 0)
					{
						flag = true;
						break;
					}
				}
			}
			if (!DeployingTentsToServer && LocalTentContents != null)
			{
				for (int j = 0; j < LocalTentContents.Count; j++)
				{
					if (LocalTentContents[j].uid == 0)
					{
						flag = true;
						break;
					}
				}
			}
			if (!SyncingToServer)
			{
				DeployingTentsToServer = flag;
			}
			else if (flag)
			{
				DeployingTentsToServer = true;
			}
		}
	}

	public static void DrawPost(int qIndex)
	{
		if (SyncingToServer)
		{
			Vector2 zero = Vector2.Zero;
			Vector2 vector = new Vector2(2f, 2f);
			string text = "";
			text = ((!DeployingTentsToServer) ? "Syncing With Server" : "Deploying Tents");
			zero.X = (float)(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Width / 2) - Menu.defaultFont.MeasureString(text).X / 2f;
			zero.Y = (float)EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Bounds.Center.Y + 64f;
			syncticker = ((syncticker < 60) ? (syncticker + 1) : 0);
			for (int i = 0; i < syncticker / 10; i++)
			{
				text += ".";
			}
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero + vector, Color.Black);
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.DarkRed);
			text = "Items Cannot Be Dropped Or Picked Up Until Syncing Is Done";
			zero.X = (float)(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Width / 2) - Menu.defaultFont.MeasureString(text).X / 2f;
			zero.Y += 32f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero + vector, Color.Black);
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.DarkRed);
			Menu.spriteBatch.End();
		}
		else if (DeployingTentsToServer)
		{
			Vector2 zero2 = Vector2.Zero;
			Vector2 vector2 = new Vector2(2f, 2f);
			string text2 = "Deploying Tents";
			zero2.X = (float)(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Width / 2) - Menu.defaultFont.MeasureString(text2).X / 2f;
			zero2.Y = (float)EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Bounds.Center.Y + 64f;
			syncticker = ((syncticker < 60) ? (syncticker + 1) : 0);
			for (int j = 0; j < syncticker / 10; j++)
			{
				text2 += ".";
			}
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.DrawString(Menu.defaultFont, text2, zero2 + vector2, Color.Black);
			Menu.spriteBatch.DrawString(Menu.defaultFont, text2, zero2, Color.DarkRed);
			Menu.spriteBatch.End();
		}
	}

	public static void AddTentOrContents(ItemCls e)
	{
		if (e.IsEquipment && e.ItemType == 1)
		{
			if (LocalPlayerTents == null)
			{
				LocalPlayerTents = new List<ItemCls>();
			}
			e.uid = 0;
			LocalPlayerTents.Add(e);
			LoadTentIndex = LocalPlayerTents.Count;
			if (LoadTentIndex >= 8)
			{
				MessagePump.AddMessage("..Max tents exceeded");
			}
		}
		else
		{
			if (!e.IsInTent || LocalPlayerTents == null)
			{
				return;
			}
			for (int i = 0; i < LocalPlayerTents.Count; i++)
			{
				if ((LocalPlayerTents[i].pos - e.pos).LengthSquared() < 256f)
				{
					if (LocalTentContents == null)
					{
						LocalTentContents = new List<ItemCls>();
					}
					e.uid = 0;
					LocalTentContents.Add(e);
					LoadContentsIndex = LocalTentContents.Count;
					if (LoadContentsIndex >= 192 && contentsExceedTimer < 0f)
					{
						contentsExceedTimer = 10f;
						MessagePump.AddMessage("..Max tents contents exceeded");
					}
				}
			}
		}
	}

	public static void NetworkTestTentOrContentsDropped(ItemCls e)
	{
		if (e.IsEquipment && e.ItemType == 1)
		{
			for (int i = 0; i < LocalPlayerTents.Count; i++)
			{
				if (LocalPlayerTents[i].uid == e.uid)
				{
					return;
				}
			}
			if (!LevelBaseMenu.isTrialMode && !LevelBaseMenu.isLocalMode && (EGENetWorkNext.networkSession == null || EGENetWorkNext.networkSession.LocalGamers[0].Id != e.ownerNetId))
			{
				return;
			}
			for (int j = 0; j < LocalPlayerTents.Count; j++)
			{
				if (LocalPlayerTents[j].uid == 0 && LocalPlayerTents[j].desc == e.desc && LocalPlayerTents[j].reserved0 == e.reserved0 && (LocalPlayerTents[j].pos - e.pos).LengthSquared() < 256f)
				{
					LocalPlayerTents[j].uid = e.uid;
					LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].saveStatusTimer = 0f;
					break;
				}
			}
		}
		else
		{
			if (!e.IsInTent)
			{
				return;
			}
			for (int k = 0; k < LocalTentContents.Count; k++)
			{
				if (LocalTentContents[k].uid == e.uid)
				{
					return;
				}
			}
			if (LevelBaseMenu.isTrialMode || LevelBaseMenu.isLocalMode || (EGENetWorkNext.networkSession != null && EGENetWorkNext.networkSession.LocalGamers[0].Id == e.ownerNetId))
			{
				for (int l = 0; l < LocalTentContents.Count; l++)
				{
					if (LocalTentContents[l].uid == 0 && LocalTentContents[l].desc == e.desc && LocalTentContents[l].reserved0 == e.reserved0 && (LocalTentContents[l].pos - e.pos).LengthSquared() < 256f)
					{
						LocalTentContents[l].uid = e.uid;
						LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].saveStatusTimer = 0f;
						return;
					}
				}
			}
			if (EGENetWorkNext.networkSession == null || EGENetWorkNext.networkSession.LocalGamers[0].Id == e.ownerNetId)
			{
				return;
			}
			for (int m = 0; m < LocalPlayerTents.Count; m++)
			{
				if ((LocalPlayerTents[m].pos - e.pos).LengthSquared() < 256f)
				{
					e.ownerNetId = EGENetWorkNext.networkSession.LocalGamers[0].Id;
					LocalTentContents.Add(e);
					LoadContentsIndex++;
					LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].saveStatusTimer = 0f;
					Storage.SavePlayerWorldItems(LocalPlayerTents, LocalTentContents);
					break;
				}
			}
		}
	}

	public static void RemoveLocalPlayerTent(ItemCls e)
	{
		bool flag = false;
		if (!e.IsInTent && ((e.desc & 0x400) <= 0 || e.ItemType != 11))
		{
			return;
		}
		bool flag2 = false;
		for (int i = 0; i < LocalPlayerTents.Count; i++)
		{
			if (LocalPlayerTents[i].uid == e.uid)
			{
				LocalPlayerTents.RemoveAt(i);
				flag = true;
				flag2 = true;
				break;
			}
		}
		if (!flag && !flag2)
		{
			for (int j = 0; j < LocalTentContents.Count; j++)
			{
				if (LocalTentContents[j].uid == e.uid)
				{
					flag2 = true;
					LocalTentContents.RemoveAt(j);
					break;
				}
			}
		}
		if (flag2 && msgLootTimer < 0f)
		{
			msgLootTimer = 4f;
			MessagePump.AddMessage("Tent Has Been Looted...");
			Storage.SavePlayerWorldItems(LocalPlayerTents, LocalTentContents);
		}
	}

	public static void DeletePlayerTents(NetworkGamer netGamer)
	{
		List<ItemCls> list = new List<ItemCls>();
		AIBase.AllWorldItems.GetPlayerTents(netGamer.Id, list);
		List<ItemCls> list2 = new List<ItemCls>();
		for (int i = 0; i < list.Count; i++)
		{
			AIBase.AllWorldItems.GetItemAtPosition(ref list[i].pos, list2);
		}
		foreach (ItemCls item in list)
		{
			AIBase.AllWorldItems.DirectDeleteItem(item);
		}
		if (EGENetWorkNext.networkSession != null)
		{
			_ = EGENetWorkNext.networkSession.LocalGamers[0].Id;
		}
		foreach (ItemCls item2 in list2)
		{
			AIBase.AllWorldItems.DirectDeleteItem(item2);
		}
	}
}
