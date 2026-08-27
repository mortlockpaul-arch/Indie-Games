using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class ClanCls
{
	private const float TimeforMessage = 12f;

	private List<string> ClanGamerTags = new List<string>();

	private float requestTimer;

	public List<NetworkGamer> PendingInviteReuest = new List<NetworkGamer>();

	public List<NetworkGamer> BlockedGamerReuest = new List<NetworkGamer>();

	public List<ClanEntry> ClanMemebers = new List<ClanEntry>();

	private PlayerBase localPlayer;

	public Texture2D diamondIcon;

	public Texture2D clanBlockIcon;

	private static Vector3 ProjDir = Vector3.Zero;

	private static Vector3 projectedPosition = Vector3.Zero;

	private static Rectangle tRec = default(Rectangle);

	public void Clans()
	{
	}

	public void LoadContent()
	{
		diamondIcon = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\TeamDiamond");
		clanBlockIcon = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\ClanBlockX");
	}

	public virtual void AddPlayerToClan(NetworkGamer e)
	{
		if (!IsBlocked(e.Gamertag) && !PendingInviteReuest.Contains(e))
		{
			requestTimer = 12f;
			PendingInviteReuest.Add(e);
		}
	}

	public virtual void AddPlayerToClan(NetworkGamer e, bool accept)
	{
		for (int i = 0; i < ClanMemebers.Count; i++)
		{
			if (ClanMemebers[i].gamerTag == e.Gamertag)
			{
				return;
			}
		}
		ClanMemebers.Add(new ClanEntry(e));
	}

	public virtual void SilentAddPlayerToClan(NetworkGamer e)
	{
		if (!ClanGamerTags.Contains(e.Gamertag))
		{
			return;
		}
		for (int i = 0; i < ClanMemebers.Count; i++)
		{
			if (ClanMemebers[i].gamerTag == e.Gamertag)
			{
				return;
			}
		}
		ClanMemebers.Add(new ClanEntry(e));
		EGENetWorkNext.packetWriter.Write((byte)152);
		EGENetWorkNext.networkSession.LocalGamers[0].SendData(EGENetWorkNext.packetWriter, SendDataOptions.ReliableInOrder, e);
	}

	public virtual void DeleteFromClan(NetworkGamer e)
	{
		for (int i = 0; i < ClanMemebers.Count; i++)
		{
			if (ClanMemebers[i].gamerTag == e.Gamertag)
			{
				ClanMemebers.RemoveAt(i);
				break;
			}
		}
	}

	public virtual void ToggleBlockFromInvites(NetworkGamer e)
	{
		for (int i = 0; i < BlockedGamerReuest.Count; i++)
		{
			if (BlockedGamerReuest[i] != null && BlockedGamerReuest[i].Gamertag == e.Gamertag)
			{
				BlockedGamerReuest.RemoveAt(i);
				return;
			}
		}
		if (PendingInviteReuest.Contains(e))
		{
			requestTimer = 0.9f;
			PendingInviteReuest.Remove(e);
		}
		DeleteFromClan(e);
		BlockedGamerReuest.Add(e);
	}

	public virtual void JoinSession(NetworkGamer e)
	{
		localPlayer = e.Tag as PlayerBase;
		ClanMemebers.Clear();
		PendingInviteReuest.Clear();
		BlockedGamerReuest.Clear();
	}

	public virtual void PlayerJoinSession(NetworkGamer e)
	{
	}

	public virtual void PlayerLeftSession(NetworkGamer e)
	{
		for (int i = 0; i < ClanMemebers.Count; i++)
		{
			if (ClanMemebers[i].gamerTag == e.Gamertag)
			{
				ClanMemebers.RemoveAt(i);
				break;
			}
		}
	}

	public virtual bool IsInClan(string gt)
	{
		for (int i = 0; i < ClanMemebers.Count; i++)
		{
			if (ClanMemebers[i].gamerTag == gt)
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool IsBlocked(string gt)
	{
		for (int i = 0; i < BlockedGamerReuest.Count; i++)
		{
			if (BlockedGamerReuest[i] != null && BlockedGamerReuest[i].Gamertag == gt)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void Update(float eTime, int qIndex)
	{
		if (EGENetWorkNext.networkSession == null)
		{
			return;
		}
		if (PendingInviteReuest.Count > 0 && !InventoryCls.InventoryOpen && !VehicleCls.VehicleMenuOpen)
		{
			requestTimer -= 0.03334f;
			if (requestTimer > 1f && requestTimer < 11f && localPlayer.currentGamePadState.IsButtonDown(Buttons.DPadLeft))
			{
				AddPlayerToClan(PendingInviteReuest[0], accept: true);
				ClanGamerTags.Add(PendingInviteReuest[0].Gamertag);
				EGENetWorkNext.packetWriter.Write((byte)152);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(EGENetWorkNext.packetWriter, SendDataOptions.ReliableInOrder, PendingInviteReuest[0]);
				PendingInviteReuest.RemoveAt(0);
				if (PendingInviteReuest.Count > 0)
				{
					requestTimer = 12f;
				}
			}
			if (requestTimer < 1f)
			{
				PendingInviteReuest.RemoveAt(0);
				if (PendingInviteReuest.Count > 0)
				{
					requestTimer = 12f;
				}
			}
		}
		for (int i = 0; i < ClanMemebers.Count; i++)
		{
			if (ClanMemebers[i].gamer != null && ClanMemebers[i].gamer.Tag != null)
			{
				PlayerBase playerBase = ClanMemebers[i].gamer.Tag as PlayerBase;
				ClanMemebers[i].render[qIndex] = false;
				ProjDir.X = playerBase.vecPosition.X - localPlayer.vecPosition.X;
				ProjDir.Y = playerBase.vecPosition.Z - localPlayer.vecPosition.Z;
				float num = ProjDir.X * localPlayer.CameraDirection.X + ProjDir.Y * localPlayer.CameraDirection.Z;
				if (num > 0f)
				{
					ClanMemebers[i].render[qIndex] = true;
					projectedPosition = playerBase.vecPosition;
					projectedPosition.X -= localPlayer.vecHeadPosition[qIndex].X;
					projectedPosition.Z -= localPlayer.vecHeadPosition[qIndex].Z;
					projectedPosition.Y += 40f;
					projectedPosition = localPlayer.vpViewPort.Project(projectedPosition, localPlayer.mDataQueue[qIndex].projection, localPlayer.mDataQueue[qIndex].view, Matrix.Identity);
					ClanMemebers[i].screenPos[qIndex].X = projectedPosition.X - (float)localPlayer.vpViewPort.X;
					ClanMemebers[i].screenPos[qIndex].Y = projectedPosition.Y - (float)localPlayer.vpViewPort.Y;
				}
			}
		}
	}

	public virtual void Draw(int qIndex, PlayerBase viewer)
	{
		_ = EGENetWorkNext.networkSession;
	}

	public virtual void DrawPost(int qIndex)
	{
		if (EGENetWorkNext.networkSession == null)
		{
			return;
		}
		Menu.spriteBatch.Begin();
		if (PendingInviteReuest.Count > 0 && !InventoryCls.InventoryOpen && !VehicleCls.VehicleMenuOpen && requestTimer > 1f && requestTimer < 11f)
		{
			tRec.X = 360;
			tRec.Y = 120;
			tRec.Width = 48;
			tRec.Height = 48;
			Menu.DrawButton(tRec, Buttons.DPadLeft, Color.LightGray);
			Vector2 p = new Vector2(tRec.X + 58, 120f);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Accept Clan Invite From " + PendingInviteReuest[0].Gamertag, p, Color.Black);
			p.X -= 2f;
			p.Y -= 2f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Accept Clan Invite From " + PendingInviteReuest[0].Gamertag, p, Color.LightGray);
		}
		for (int i = 0; i < ClanMemebers.Count; i++)
		{
			if (ClanMemebers[i].render[qIndex])
			{
				tRec.X = (int)ClanMemebers[i].screenPos[qIndex].X - 12;
				tRec.Y = (int)ClanMemebers[i].screenPos[qIndex].Y - 12;
				tRec.Width = 24;
				tRec.Height = 24;
				Menu.spriteBatch.Draw(diamondIcon, tRec, Color.White);
			}
		}
		Menu.spriteBatch.End();
	}
}
