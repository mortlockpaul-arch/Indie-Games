using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using PropModel;

namespace EGEngine;

public class WorldItemsCls
{
	private static ushort _uid = 0;

	private static int xGridSize = 8192;

	private static int zGridSize = 8192;

	private static int xDim = 131072;

	private static int zDim = 131072;

	private static int nXGrid = xDim / xGridSize;

	private static int nZGrid = zDim / zGridSize;

	private static int xMin = -(xDim / 2);

	private static int zMin = -(zDim / 2);

	private static int xMax = xDim / 2;

	private static int zMax = zDim / 2;

	private static bool displayAccessMenuMsg = false;

	public static bool TentMenuOpen = false;

	public static Vector3 TentPosition = Vector3.Zero;

	private List<AreaItemsCls> WorldItemsList = new List<AreaItemsCls>();

	private static int MaxDrawItems = 200;

	private static int[] CurrentNumDrawItems = new int[2];

	private static ItemCls[,] ItemsDrawList = new ItemCls[2, MaxDrawItems];

	private Color[,] SoundMask;

	public static Cue OceanSound;

	public static Cue ForestDay;

	public static Cue FireLoop;

	public static Cue Radiation;

	private int sndDimHalfx = xDim / 2;

	private int sndDimHalfz = zDim / 2;

	private int sndDivisorX = xDim / 256;

	private int sndDivisorZ = zDim / 256;

	private static bool inRadiation = false;

	private static uint soundBitMask = 4278190080u;

	private static Vector3 tmpUPos = Vector3.Zero;

	private static Vector3 tmpPos = Vector3.Zero;

	private static Vector2 uiPos = Vector2.Zero;

	private static Vector3 tmpDir = Vector3.Zero;

	private static Rectangle tmpRec = Rectangle.Empty;

	public static ushort UniqueId
	{
		get
		{
			_uid++;
			return _uid;
		}
		set
		{
		}
	}

	public void LoadContent()
	{
		Texture2D texture2D = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\SoundMask");
		Color[] array = new Color[texture2D.Width * texture2D.Height];
		texture2D.GetData(array);
		SoundMask = new Color[texture2D.Width, texture2D.Height];
		for (int i = 0; i < texture2D.Width; i++)
		{
			for (int j = 0; j < texture2D.Height; j++)
			{
				int num = i + j * texture2D.Height;
				ref Color reference = ref SoundMask[i, j];
				reference = array[num];
			}
		}
		OceanSound = EndGameEngine.SoundBnk.GetCue("OceanWaves");
		OceanSound.Play();
		OceanSound.SetVariable("Distance", 20000f);
		ForestDay = EndGameEngine.SoundBnk.GetCue("ForestDaySmall");
		ForestDay.Play();
		ForestDay.SetVariable("Distance", 20000f);
		FireLoop = EndGameEngine.SoundBnk.GetCue("FireJetFuel");
		FireLoop.Play();
		FireLoop.SetVariable("Distance", 20000f);
		Radiation = EndGameEngine.SoundBnk.GetCue("GeigerCounterSoundSmall");
		Radiation.Play();
		Radiation.SetVariable("Distance", 20000f);
		for (int k = 0; k < 2; k++)
		{
			for (int l = 0; l < MaxDrawItems; l++)
			{
				ItemsDrawList[k, l] = null;
			}
		}
		WorldItemsList.Clear();
		for (int m = xMin; m < xMax; m += xGridSize)
		{
			for (int n = zMin; n < zMax; n += zGridSize)
			{
				AreaItemsCls areaItemsCls = new AreaItemsCls();
				areaItemsCls.bBox.Min = new Vector3(m, -1000f, n);
				areaItemsCls.bBox.Max = new Vector3(m + xGridSize, 30000f, n + xGridSize);
				WorldItemsList.Add(areaItemsCls);
			}
		}
	}

	private int GetGridIndex(ref Vector3 pos)
	{
		tmpPos.Y = pos.Y;
		tmpPos.X = pos.X + (float)(xDim / 2) + 1f;
		tmpPos.Z = pos.Z + (float)(zDim / 2) + 1f;
		int num = (int)(tmpPos.X / (float)xGridSize);
		int num2 = (int)(tmpPos.Z / (float)zGridSize);
		num = ((num > 0) ? num : 0);
		num2 = ((num2 > 0) ? num2 : 0);
		num = ((num < nXGrid) ? num : (nXGrid - 1));
		num2 = ((num2 < nZGrid) ? num2 : (nZGrid - 1));
		return num * nZGrid + num2;
	}

	public virtual void Reset()
	{
		inRadiation = false;
		for (int i = 0; i < WorldItemsList.Count; i++)
		{
			WorldItemsList[i].items.Clear();
		}
	}

	public virtual void Update(float eTime, int qIndex, PlayerBase playerRef)
	{
		bool flag = false;
		bool tentMenuOpen = false;
		float num = 19600f;
		float num2 = 25000000f;
		float num3 = 640000f;
		CurrentNumDrawItems[qIndex] = 0;
		tmpPos.Y = playerRef.vecPosition.Y;
		tmpPos.X = playerRef.vecPosition.X + (float)sndDimHalfx;
		tmpPos.Z = playerRef.vecPosition.Z + (float)sndDimHalfz;
		int num4 = (int)tmpPos.X / sndDivisorX;
		int num5 = (int)tmpPos.Z / sndDivisorZ;
		num4 = ((num4 >= 0) ? ((num4 < 256) ? num4 : 255) : 0);
		num5 = ((num5 >= 0) ? ((num5 < 256) ? num5 : 255) : 0);
		float num6 = (float)(int)SoundMask[num4, num5].A / 256f;
		OceanSound.SetVariable("Distance", (1f - num6) * 20000f);
		num6 = (float)(int)SoundMask[num4, num5].B / 256f;
		ForestDay.SetVariable("Distance", (1f - num6) * 20000f);
		num6 = (float)(int)SoundMask[num4, num5].G / 256f;
		FireLoop.SetVariable("Distance", (1f - num6) * 20000f);
		num6 = (float)(int)SoundMask[num4, num5].R / 255f;
		Radiation.SetVariable("Distance", (1f - num6 * 1.22f) * 20000f);
		PostProcessEffects.pNoiseBlend = num6;
		if (num6 > 0.85f)
		{
			float num7 = playerRef.BloodLevel - 0.25f;
			playerRef.BloodLevel = ((num7 < 0f) ? 0f : num7);
			playerRef.BloodLoss = 0.005f;
			inRadiation = true;
		}
		if (num6 > 0.8f)
		{
			float num8 = playerRef.BloodLevel - 0.1f;
			playerRef.BloodLevel = ((num8 < 0f) ? 0f : num8);
			inRadiation = true;
		}
		else if (num6 > 0.65f)
		{
			playerRef.BloodLevel -= 0.025f;
			inRadiation = true;
		}
		else if (num6 > 0.45f)
		{
			inRadiation = true;
		}
		else
		{
			inRadiation = false;
		}
		InventoryCls.VacinityItemList[qIndex].Clear();
		tmpPos.Y = playerRef.vecPosition.Y;
		tmpPos.X = playerRef.vecPosition.X + (float)(xDim / 2) + 1f;
		tmpPos.Z = playerRef.vecPosition.Z + (float)(zDim / 2) + 1f;
		int num9 = (int)(tmpPos.X / (float)xGridSize) - 2;
		int num10 = (int)(tmpPos.Z / (float)zGridSize) - 2;
		num9 = ((num9 > 0) ? num9 : 0);
		num10 = ((num10 > 0) ? num10 : 0);
		int num11 = num9 + 6;
		int num12 = num10 + 6;
		num11 = ((num11 < nXGrid) ? num11 : (nXGrid - 1));
		num12 = ((num12 < nZGrid) ? num12 : (nZGrid - 1));
		for (int i = num9; i < num11; i++)
		{
			for (int j = num10; j < num12; j++)
			{
				int index = i * nZGrid + j;
				int count = WorldItemsList[index].items.Count;
				for (int k = 0; k < count; k++)
				{
					ItemCls itemCls = WorldItemsList[index].items[k];
					if (!itemCls.IsValid || itemCls.IsVehicle)
					{
						continue;
					}
					tmpDir = itemCls.pos - playerRef.vecPosition;
					float num13 = tmpDir.LengthSquared();
					if (!(num13 < num2))
					{
						continue;
					}
					tmpDir = itemCls.pos - playerRef.vecHeadPosition[qIndex];
					float num14 = Vector3.Dot(tmpDir, playerRef.CameraDirection);
					if (CurrentNumDrawItems[qIndex] < MaxDrawItems && (num14 > 0f || num13 < num3) && !itemCls.IsInTent)
					{
						ItemsDrawList[qIndex, CurrentNumDrawItems[qIndex]] = itemCls;
						CurrentNumDrawItems[qIndex]++;
					}
					if (num13 < num)
					{
						InventoryItemCls inventoryItemCls = new InventoryItemCls();
						if (TentMenuOpen)
						{
							if (itemCls.IsEquipment && itemCls.ItemType == 1)
							{
								TentPosition = itemCls.pos;
							}
							if (itemCls.IsInTent || (itemCls.IsEquipment && itemCls.ItemType == 1))
							{
								inventoryItemCls.desc = itemCls.desc;
								inventoryItemCls.item = itemCls;
								InventoryCls.VacinityItemList[qIndex].Add(inventoryItemCls);
							}
						}
						else if (!itemCls.IsInTent && (!itemCls.IsEquipment || itemCls.ItemType != 1))
						{
							inventoryItemCls.desc = itemCls.desc;
							inventoryItemCls.item = itemCls;
							InventoryCls.VacinityItemList[qIndex].Add(inventoryItemCls);
						}
					}
					if (!(num14 > 0f) || !(num13 < num))
					{
						continue;
					}
					tmpDir.Normalize();
					num14 = Vector3.Dot(tmpDir, playerRef.CameraDirection);
					if (num14 > 0.8f)
					{
						if (itemCls.IsEquipment && itemCls.ItemType == 1)
						{
							tentMenuOpen = true;
							TentMenuOpen = true;
						}
						if (!InventoryCls.InventoryOpen && !flag)
						{
							flag = true;
						}
					}
				}
			}
		}
		if (playerRef.IsAttached0)
		{
			flag = false;
		}
		if (flag && InventoryCls.VacinityItemList[qIndex].Count == 0)
		{
			flag = false;
		}
		playerRef.OverrideButtonX = flag;
		displayAccessMenuMsg = flag;
		if (TentMenuOpen)
		{
			TentMenuOpen = tentMenuOpen;
		}
	}

	public void GetPlayerTents(byte ownerId, List<ItemCls> t)
	{
		for (int i = 0; i < WorldItemsList.Count; i++)
		{
			int count = WorldItemsList[i].items.Count;
			for (int j = 0; j < count; j++)
			{
				if (WorldItemsList[i].items[j].ownerNetId == ownerId && WorldItemsList[i].items[j].IsEquipment && WorldItemsList[i].items[j].ItemType == 1)
				{
					t.Add(WorldItemsList[i].items[j]);
				}
			}
		}
	}

	public void GetItemAtPosition(ref Vector3 p, List<ItemCls> l)
	{
		tmpPos.Y = p.Y;
		tmpPos.X = p.X + (float)(xDim / 2) + 1f;
		tmpPos.Z = p.Z + (float)(zDim / 2) + 1f;
		int num = (int)(tmpPos.X / (float)xGridSize) - 1;
		int num2 = (int)(tmpPos.Z / (float)zGridSize) - 1;
		num = ((num > 0) ? num : 0);
		num2 = ((num2 > 0) ? num2 : 0);
		int num3 = num + 3;
		int num4 = num2 + 3;
		num3 = ((num3 < nXGrid) ? num3 : (nXGrid - 1));
		num4 = ((num4 < nZGrid) ? num4 : (nZGrid - 1));
		for (int i = num; i < num3; i++)
		{
			for (int j = num2; j < num4; j++)
			{
				int index = i * nZGrid + j;
				int count = WorldItemsList[index].items.Count;
				for (int k = 0; k < count; k++)
				{
					ItemCls itemCls = WorldItemsList[index].items[k];
					if (itemCls.IsValid && !itemCls.IsVehicle && itemCls.IsInTent && (itemCls.pos - p).LengthSquared() < 256f)
					{
						l.Add(itemCls);
					}
				}
			}
		}
	}

	public virtual void Draw(PlayerBase viewer, int qIndex)
	{
		for (int i = 0; i < CurrentNumDrawItems[qIndex]; i++)
		{
			ItemCls itemCls = ItemsDrawList[qIndex, i];
			if ((itemCls.desc & 0x100) > 0)
			{
				int itemType = itemCls.ItemType;
				ref Matrix reference = ref ConsumableCls.itemsModels[itemType].matWorld[qIndex];
				reference = Matrix.Identity;
				ConsumableCls.itemsModels[itemType].matWorld[qIndex].Translation = itemCls.pos;
				ConsumableCls.itemsModels[itemType].DrawCameraSpace(viewer, qIndex, 1f);
			}
			else if ((itemCls.desc & 0x200) > 0)
			{
				int itemType2 = itemCls.ItemType;
				ref Matrix reference2 = ref AIBase.AllWeapons.matWorld[qIndex];
				reference2 = Matrix.Identity;
				AIBase.AllWeapons.matWorld[qIndex].Translation = itemCls.pos;
				AIBase.AllWeapons.DrawCameraSpace(viewer, qIndex, itemType2);
			}
			else if ((itemCls.desc & 0x400) > 0)
			{
				int itemType3 = itemCls.ItemType;
				ref Matrix reference3 = ref EquipmentCls.itemsModels[itemType3].matWorld[qIndex];
				reference3 = Matrix.Identity;
				EquipmentCls.itemsModels[itemType3].matWorld[qIndex].Translation = itemCls.pos;
				EquipmentCls.itemsModels[itemType3].DrawCameraSpace(viewer, qIndex, 1f);
			}
			else
			{
				_ = itemCls.desc & 0x800;
				_ = 0;
			}
		}
	}

	public virtual void DrawShadowMap(PlayerBase viewer, ref Matrix LightViewProj, ref Vector3 lightPos, int qIndex)
	{
		for (int i = 0; i < CurrentNumDrawItems[qIndex]; i++)
		{
			ItemCls itemCls = ItemsDrawList[qIndex, i];
			if ((itemCls.desc & 0x100) > 0)
			{
				int itemType = itemCls.ItemType;
				ref Matrix reference = ref ConsumableCls.itemsModels[itemType].matWorld[qIndex];
				reference = Matrix.Identity;
				ConsumableCls.itemsModels[itemType].matWorld[qIndex].Translation = itemCls.pos;
				ConsumableCls.itemsModels[itemType].DrawShadowMap(viewer, ref LightViewProj, ref lightPos, qIndex, lod: false);
			}
			else if ((itemCls.desc & 0x200) > 0)
			{
				_ = itemCls.ItemType;
				ref Matrix reference2 = ref AIBase.AllWeapons.matWorld[qIndex];
				reference2 = Matrix.Identity;
				AIBase.AllWeapons.matWorld[qIndex].Translation = itemCls.pos;
			}
			else if ((itemCls.desc & 0x400) > 0)
			{
				int itemType2 = itemCls.ItemType;
				ref Matrix reference3 = ref EquipmentCls.itemsModels[itemType2].matWorld[qIndex];
				reference3 = Matrix.Identity;
				EquipmentCls.itemsModels[itemType2].matWorld[qIndex].Translation = itemCls.pos;
				EquipmentCls.itemsModels[itemType2].DrawShadowMap(viewer, ref LightViewProj, ref lightPos, qIndex, lod: false);
			}
			else
			{
				_ = itemCls.desc & 0x800;
				_ = 0;
			}
		}
	}

	public virtual void DrawPost(PlayerBase e, int qIndex)
	{
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		Menu.spriteBatch.Begin();
		if (inRadiation)
		{
			uiPos.X = (float)viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString("Deadly Radiation Zone").X * 0.5f;
			uiPos.Y = viewport.TitleSafeArea.Top + 128;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Deadly Radiation Zone", uiPos + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Deadly Radiation Zone", uiPos, Color.Red, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
		}
		if (displayAccessMenuMsg && !VehicleCls.VehicleMenuOpen)
		{
			tmpRec.X = 520;
			tmpRec.Y = 462;
			tmpRec.Width = 48;
			tmpRec.Height = 48;
			Menu.spriteBatch.Draw(Menu.backButton, tmpRec, Color.White);
			uiPos.X = 564f;
			uiPos.Y = 460f;
			if (TentMenuOpen)
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, " Open Tent", uiPos + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, " Open Tent", uiPos, Color.LightGray, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
			}
			else
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, " View Items", uiPos + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, " View Items", uiPos, Color.LightGray, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
			}
		}
		Menu.spriteBatch.End();
	}

	public virtual void Initialize(WorldAreaCls world)
	{
	}

	public virtual void Setup(NetworkGamer gamer, WorldAreaCls world)
	{
		if (world == null)
		{
			return;
		}
		bool flag = false;
		if (gamer == null)
		{
			flag = true;
		}
		else if (gamer.IsHost)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		Vector3 zero = Vector3.Zero;
		ushort num = 0;
		byte b = 0;
		SpawnPositionData e = new SpawnPositionData();
		int getNumOfItemPoints = world.GetNumOfItemPoints;
		for (int i = 0; i < getNumOfItemPoints; i++)
		{
			int num2 = EndGameEngine.randGenerator.Next(2, 5);
			for (int j = 0; j < num2; j++)
			{
				bool flag2 = false;
				world.GetItemSpawnAtIndex(ref e, i);
				zero = e.spawmPosition;
				num = e.spawnType;
				b = e.itemRange;
				int gridIndex = GetGridIndex(ref zero);
				if (num == 512 && EndGameEngine.randGenerator.Next(0, 100) > 20)
				{
					flag2 = true;
					num = 1024;
				}
				switch (num)
				{
				case 256:
				{
					ItemCls itemCls3 = new ItemCls();
					itemCls3.uid = UniqueId;
					itemCls3.pos = zero;
					itemCls3.desc = (ushort)(0x100 | (ushort)ConsumableCls.CreateRandom(i, b));
					WorldItemsList[gridIndex].items.Add(itemCls3);
					break;
				}
				case 1024:
				{
					ItemCls itemCls2 = new ItemCls();
					itemCls2.uid = UniqueId;
					itemCls2.pos = zero;
					if (flag2)
					{
						itemCls2.desc = (ushort)(0x400 | (ushort)EquipmentCls.CreateRandomAmmo(i, b));
						itemCls2.reserved0 = EquipmentCls.Reservedbyte0[itemCls2.ItemType];
					}
					else
					{
						itemCls2.desc = (ushort)(0x400 | (ushort)EquipmentCls.CreateRandom(i, b));
					}
					WorldItemsList[gridIndex].items.Add(itemCls2);
					break;
				}
				case 512:
				{
					ItemCls itemCls = new ItemCls();
					itemCls.uid = UniqueId;
					itemCls.pos = zero;
					itemCls.desc = (ushort)(0x200 | WeaponsCls.CreateRandom(i, b));
					WorldItemsList[gridIndex].items.Add(itemCls);
					break;
				}
				}
			}
		}
	}

	public void AddVehicle(NetworkGamer gamer, ItemCls item)
	{
		bool flag = false;
		if (gamer == null)
		{
			flag = true;
		}
		else if (gamer.IsHost)
		{
			flag = true;
		}
		if (flag)
		{
			int gridIndex = GetGridIndex(ref item.pos);
			if (gridIndex < WorldItemsList.Count)
			{
				WorldItemsList[gridIndex].items.Add(item);
			}
		}
	}

	public bool UpdateWorldToClient(NetworkGamer gamer, ref int areaIndex, ref int itemIndex)
	{
		if (areaIndex >= WorldItemsList.Count)
		{
			return true;
		}
		if (itemIndex >= WorldItemsList[areaIndex].items.Count)
		{
			areaIndex++;
			if (areaIndex >= WorldItemsList.Count)
			{
				return true;
			}
			itemIndex = 0;
		}
		PacketWriter packetWriter = EGENetWorkNext.packetWriter;
		int num = 0;
		while (areaIndex < WorldItemsList.Count)
		{
			while (itemIndex < WorldItemsList[areaIndex].items.Count)
			{
				WriteCreateToClients(packetWriter, WorldItemsList[areaIndex].items[itemIndex], gamer);
				itemIndex++;
				num++;
				if (num > 1)
				{
					if (packetWriter.Length > 0)
					{
						EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
					}
					return false;
				}
			}
			itemIndex = 0;
			areaIndex++;
		}
		if (packetWriter.Length > 0)
		{
			EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
		}
		return false;
	}

	private void WriteCreateToClients(PacketWriter pWriter, ItemCls item, NetworkGamer gamer)
	{
		_ = gamer.Tag;
		pWriter.Write((byte)105);
		pWriter.Write(gamer.Id);
		item.NetworkWrite(pWriter);
	}

	public void ReadCreateFromServer(PacketReader pReader, NetworkGamer sender)
	{
		byte gamerId = pReader.ReadByte();
		ItemCls itemCls = new ItemCls();
		itemCls.NetworkRead(pReader);
		NetworkGamer networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId);
		if (networkGamer != null && networkGamer.IsLocal)
		{
			WorldItemsList[GetGridIndex(ref itemCls.pos)].items.Add(itemCls);
			ApocZSaveDataCls.NetworkTestTentOrContentsDropped(itemCls);
			SpecialCreateProcess(networkGamer, itemCls);
		}
	}

	private void SpecialCreateProcess(NetworkGamer gamer, ItemCls item)
	{
	}

	private void CreateWorldItem(NetworkGamer gamer, ItemCls item)
	{
		if ((item.desc & 0x800) > 0)
		{
			AIBase.CreateVehicleByType(gamer, item);
		}
		else
		{
			WorldItemsList[GetGridIndex(ref item.pos)].items.Add(item);
		}
	}

	public void RequestItem(ItemCls e)
	{
		if (!CanPickUpItem(e))
		{
			return;
		}
		if (EGENetWorkNext.networkSession != null)
		{
			if (EGENetWorkNext.networkSession.IsHost)
			{
				if (ServerRequestPickupItem(e))
				{
					if ((e.desc & 0x400) > 0 && e.ItemType == 1)
					{
						e.desc = 1035;
					}
					InventoryItemCls inventoryItemCls = new InventoryItemCls();
					inventoryItemCls.desc = e.desc;
					inventoryItemCls.item = new ItemCls(e);
					AIBase.PlayerInventory.AddItem(InventorySlot.Pockets, inventoryItemCls);
					ServerUpdateItemToClients(e, EGENetWorkNext.networkSession.LocalGamers[0].Id);
				}
			}
			else
			{
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)106);
				e.NetworkWrite(packetWriter);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder, EGENetWorkNext.networkSession.Host);
			}
		}
		else if (ServerRequestPickupItem(e))
		{
			if ((e.desc & 0x400) > 0 && e.ItemType == 1)
			{
				e.desc = 1035;
			}
			InventoryItemCls inventoryItemCls2 = new InventoryItemCls();
			inventoryItemCls2.desc = e.desc;
			inventoryItemCls2.item = e;
			AIBase.PlayerInventory.AddItem(InventorySlot.Pockets, inventoryItemCls2);
		}
	}

	public void ServerUpdateItemToClients(ItemCls e, byte senderId)
	{
		PacketWriter packetWriter = EGENetWorkNext.packetWriter;
		packetWriter.Write((byte)107);
		packetWriter.Write(senderId);
		e.NetworkWrite(packetWriter);
		EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
	}

	public bool ServerRequestPickupItem(ItemCls e)
	{
		bool flag = false;
		if (EGENetWorkNext.networkSession == null)
		{
			flag = true;
		}
		else if (EGENetWorkNext.networkSession.IsHost)
		{
			flag = true;
		}
		if (flag)
		{
			int gridIndex = GetGridIndex(ref e.pos);
			int count = WorldItemsList[gridIndex].items.Count;
			for (int i = 0; i < count; i++)
			{
				if (WorldItemsList[gridIndex].items[i].uid != e.uid || WorldItemsList[gridIndex].items[i].desc == 0)
				{
					continue;
				}
				ApocZSaveDataCls.RemoveLocalPlayerTent(e);
				if (e.IsEquipment && e.ItemType == 1)
				{
					e.desc &= 32767;
					for (int j = 0; j < count; j++)
					{
						if (WorldItemsList[gridIndex].items[j].IsInTent && (e.pos - WorldItemsList[gridIndex].items[j].pos).LengthSquared() < 256f)
						{
							ApocZSaveDataCls.RemoveLocalPlayerTent(WorldItemsList[gridIndex].items[j]);
							WorldItemsList[gridIndex].items[j].desc &= 32767;
							WorldItemsList[gridIndex].items[j].pos += WorldAreaCls.GetItemPlaceRandOffset();
							PacketWriter packetWriter = EGENetWorkNext.packetWriter;
							packetWriter.Write((byte)110);
							WorldItemsList[gridIndex].items[j].NetworkWrite(packetWriter);
						}
					}
				}
				WorldItemsList[gridIndex].items[i].desc = 0;
				return true;
			}
		}
		return false;
	}

	public void PickupItem(ItemCls e, bool receiver)
	{
		int gridIndex = GetGridIndex(ref e.pos);
		int count = WorldItemsList[gridIndex].items.Count;
		for (int i = 0; i < count; i++)
		{
			if (WorldItemsList[gridIndex].items[i].uid != e.uid || WorldItemsList[gridIndex].items[i].desc == 0)
			{
				continue;
			}
			if (receiver)
			{
				InventoryItemCls inventoryItemCls = new InventoryItemCls();
				if ((e.desc & 0x400) > 0 && e.ItemType == 1)
				{
					e.desc = 1035;
				}
				inventoryItemCls.desc = e.desc;
				inventoryItemCls.item = e;
				AIBase.PlayerInventory.AddItem(InventorySlot.Pockets, inventoryItemCls);
				WorldItemsList[gridIndex].items[i].desc = 0;
			}
			else
			{
				ApocZSaveDataCls.RemoveLocalPlayerTent(e);
				WorldItemsList[gridIndex].items[i].desc = 0;
			}
			break;
		}
	}

	public void ClientUpdateItem(ItemCls e)
	{
		int gridIndex = GetGridIndex(ref e.pos);
		int count = WorldItemsList[gridIndex].items.Count;
		for (int i = 0; i < count; i++)
		{
			if (WorldItemsList[gridIndex].items[i].uid == e.uid && WorldItemsList[gridIndex].items[i].desc != 0)
			{
				WorldItemsList[gridIndex].items[i].desc = e.desc;
				WorldItemsList[gridIndex].items[i].ownerNetId = e.ownerNetId;
				WorldItemsList[gridIndex].items[i].pos = e.pos;
				WorldItemsList[gridIndex].items[i].reserved0 = e.reserved0;
				break;
			}
		}
	}

	private static bool CanPickUpItem(ItemCls e)
	{
		return AIBase.PlayerInventory.CanPickUpItem(e);
	}

	public void DropItemInworld(Vector3 pos, ItemCls e)
	{
		if (EGENetWorkNext.networkSession != null)
		{
			if (EGENetWorkNext.networkSession.IsHost)
			{
				ServerDropItem(pos, e, EGENetWorkNext.networkSession.Host.Id);
				return;
			}
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			packetWriter.Write((byte)108);
			e.NetworkWrite(packetWriter);
			if (EGENetWorkNext.networkSession != null && packetWriter.Length > 0)
			{
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder, EGENetWorkNext.networkSession.Host);
			}
		}
		else
		{
			ServerDropItem(pos, e, 0);
		}
	}

	public void ServerDropItem(Vector3 pos, ItemCls e, byte gamerId)
	{
		ItemCls itemCls = new ItemCls();
		itemCls.uid = UniqueId;
		itemCls.desc = e.desc;
		itemCls.pos = pos;
		itemCls.ownerNetId = e.ownerNetId;
		itemCls.reserved0 = e.reserved0;
		WorldItemsList[GetGridIndex(ref itemCls.pos)].items.Add(itemCls);
		ServerAddItemClient(pos, itemCls, gamerId);
		ApocZSaveDataCls.NetworkTestTentOrContentsDropped(itemCls);
	}

	public void AddItemToList(Vector3 pos, ItemCls e)
	{
		ItemCls itemCls = new ItemCls();
		itemCls.uid = e.uid;
		itemCls.desc = e.desc;
		itemCls.pos = pos;
		itemCls.ownerNetId = e.ownerNetId;
		itemCls.reserved0 = e.reserved0;
		WorldItemsList[GetGridIndex(ref itemCls.pos)].items.Add(itemCls);
		ApocZSaveDataCls.NetworkTestTentOrContentsDropped(itemCls);
	}

	public void ServerAddItemClient(Vector3 pos, ItemCls e, byte gamerId)
	{
		if (EGENetWorkNext.networkSession != null)
		{
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			packetWriter.Write((byte)109);
			packetWriter.Write(gamerId);
			e.NetworkWrite(packetWriter);
			EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
		}
	}

	public void DirectDeleteItem(ItemCls e)
	{
		int gridIndex = GetGridIndex(ref e.pos);
		int count = WorldItemsList[gridIndex].items.Count;
		for (int i = 0; i < count; i++)
		{
			if (WorldItemsList[gridIndex].items[i].uid == e.uid && WorldItemsList[gridIndex].items[i].desc != 0)
			{
				WorldItemsList[gridIndex].items.RemoveAt(i);
				break;
			}
		}
	}

	public string GetItemStringName(ItemCls item)
	{
		if ((item.desc & 0x100) > 0)
		{
			return ConsumableCls.ConsumableItemsDesc[item.ItemType];
		}
		if ((item.desc & 0x200) > 0)
		{
			return WeaponsCls.WeaponsItemsDesc[item.ItemType];
		}
		if ((item.desc & 0x400) > 0)
		{
			return EquipmentCls.EquipmentItemDesc[item.ItemType];
		}
		if ((item.desc & 0x800) > 0)
		{
			return "Vehicle";
		}
		return "null";
	}
}
