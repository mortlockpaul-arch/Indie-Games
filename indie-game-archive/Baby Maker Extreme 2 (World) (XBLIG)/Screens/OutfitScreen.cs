using System;
using System.Collections.Generic;
using System.Linq;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlayObjects;
using Renderer;

namespace Screens;

public class OutfitScreen : Screen
{
	private List<OutfitPiece> m_outfitPiecesEquipped;

	private List<SpriteInstance> m_baby;

	private List<List<SpriteInstance>> m_SelectableBabies;

	private List<List<SpriteInstance>> m_pieces;

	private List<List<string>> m_pieceDescriptions;

	private List<string> m_powerupDescriptions;

	private SpriteInstance m_bg;

	private RenderLight m_light;

	private RenderLight m_light2;

	private int m_iSelectedIndexX;

	private int m_iSelectedIndexY;

	private SpriteInstance m_highLight;

	private float m_fTimer;

	private Player m_player;

	private float m_fScale;

	private int m_iBodyType;

	private SpriteInstance m_nextPageButton;

	private string m_nextPageText;

	private SpriteInstance m_sel;

	private string m_notAvatar;

	public OutfitScreen(Player p)
		: base(updateParent: false, drawParent: false, inputParent: false)
	{
		m_iBodyType = (int)p.BabyType;
		m_player = p;
		m_baby = new List<SpriteInstance>();
		List<SpriteInstance> firstOutfitSprites = p.GetFirstOutfitSprites();
		m_fScale = firstOutfitSprites[0].WidthScale / firstOutfitSprites[0].GetSpriteImage().Width;
		for (int i = 0; i < firstOutfitSprites.Count; i++)
		{
			m_baby.Add(new SpriteInstance(firstOutfitSprites[i].GetSpriteImage(), SceneRenderer.GetCameraPosition() + new Vector2(255f, 40f), firstOutfitSprites[i].Depth));
			m_baby.Last().Origin = firstOutfitSprites[i].Origin;
			m_baby.Last().WidthScale *= m_fScale;
			m_baby.Last().Position += new Vector2(0f, 100f);
		}
		m_baby[2].Position = m_baby[0].Position + new Vector2(44f, 10f) * m_fScale;
		m_baby[3].Position = m_baby[0].Position + new Vector2(12f, -2f) * m_fScale;
		m_baby[1].Position = m_baby[0].Position + new Vector2(3f, -47f) * m_fScale;
		m_SelectableBabies = new List<List<SpriteInstance>>();
		AddBaby(0.75f, m_baby, Color.White, 0);
		AddBaby(0.75f, m_baby, new Color(1f, 1f, 1f, 0f), 0);
		AddBaby(0.55f, m_baby, Color.White, 0);
		AddBaby(0.75f, m_baby, Color.White, 4);
		AddBaby(0.75f, m_baby, Color.Cyan, 3);
		AddBaby(1.1f, m_baby, Color.White, 0);
		AddBaby(0.6f, m_baby, Color.Red, 1);
		AddBaby(0.75f, m_baby, Color.White, 2);
		m_outfitPiecesEquipped = new List<OutfitPiece>();
		m_pieces = new List<List<SpriteInstance>>();
		for (int j = 0; j < 4; j++)
		{
			m_pieces.Add(new List<SpriteInstance>());
		}
		for (int k = 0; k < 25; k++)
		{
			Rectangle pageCoords = default(Rectangle);
			int num = 0;
			if (k < 5)
			{
				int num2 = 0;
				if (k > 1)
				{
					num2 += 4;
				}
				pageCoords.X = k * 96 + num2;
				pageCoords.Y = 0;
				pageCoords.Width = 96;
				if (k == 1)
				{
					pageCoords.Width += 4;
				}
				pageCoords.Height = 100;
				num = 2;
			}
			else if (k < 12)
			{
				pageCoords.X = (k - 5) * 104;
				pageCoords.Y = 100;
				pageCoords.Width = 104;
				pageCoords.Height = 118;
				num = 0;
			}
			else if (k < 16)
			{
				pageCoords.X = (k - 5 - 7) * 105;
				pageCoords.Y = 218;
				pageCoords.Width = 105;
				pageCoords.Height = 91;
				num = 3;
			}
			else
			{
				int num3 = 0;
				if (k == 17 || k == 18 || k == 21 || k == 24)
				{
					num3 = 18;
				}
				pageCoords.X = (k - 5 - 7 - 4) * 98;
				pageCoords.Y = 309;
				pageCoords.Width = 98;
				pageCoords.Height = 104 + num3 * 2;
				num = 1;
			}
			m_pieces[num].Add(TextureContainer.GetSprite("images/Spritesheets/outfitPieces", pageCoords, SceneRenderer.GetCameraPosition() + new Vector2(-240 + (m_pieces[num].Count % 5 - 1) * 100, 50 + (num - 1) * 150), 10f));
			switch (num)
			{
			case 2:
				m_pieces[num].Last().Position = m_pieces[num].Last().Position - new Vector2(0f, 80f);
				break;
			case 3:
				m_pieces[num].Last().Position = m_pieces[num].Last().Position - new Vector2(0f, 100f);
				break;
			}
		}
		GeneratePieceDescriptions();
		m_bg = TextureContainer.GetSprite("images/outfitShelfs", SceneRenderer.GetCameraPosition(), -10f);
		m_bg.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/outfitShelfsNorm");
		m_bg.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_light = new RenderLight(new Vector3(0f, 0f, 0f), 1f, 100, Color.White);
		m_highLight = TextureContainer.GetSprite("images/particle", SceneRenderer.GetCameraPosition(), -1f);
		m_highLight.WidthScale = 140f;
		m_highLight.Alpha = 0.7f;
		m_highLight.Additive = true;
		m_iSelectedIndexX = 0;
		m_iSelectedIndexY = 0;
		m_light2 = new RenderLight(default(Vector3), 0f, 1300, Color.White);
		m_light2.pos = new Vector3(SceneRenderer.GetCameraPosition().X + 300f, 0f - SceneRenderer.GetCameraPosition().Y + 250f, 500f);
		m_light2.falloff = 0.2f;
		m_light2.range = 1200;
		m_light2.color = new Color(0.6f, 0.6f, 0.3f);
		m_fTimer = 0f;
		m_nextPageButton = TextureContainer.GetSprite("images/Buttons/abxy", new Rectangle(0, 0, 50, 47), m_bg.Position + new Vector2(270f, 280f), m_bg.Depth + 2f);
		m_nextPageButton.SurfaceScale *= 0.65f;
		m_nextPageText = "Go Back";
		m_sel = TextureContainer.GetSprite("images/ball", default(Vector2), m_bg.Depth + 0.0001f);
		m_sel.Additive = true;
		m_sel.WidthScale = 130f;
		m_sel.Alpha = 0.3f;
		m_sel.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/ballNorm");
		EquipPiece(-1, m_iBodyType);
		List<OutfitPiece> outfitPieces = m_player.GetOutfitPieces();
		for (int l = 0; l < outfitPieces.Count; l++)
		{
			m_outfitPiecesEquipped.Add(outfitPieces[l]);
			m_outfitPiecesEquipped[l].AttachedTo = m_baby[m_player.GetProp().GetOutfit().GetSprites()
				.IndexOf(m_outfitPiecesEquipped[l].AttachedTo)];
		}
		m_notAvatar = "Not for Avatars";
	}

	private void AddBaby(float scale, List<SpriteInstance> sprites, Color c, int special)
	{
		m_SelectableBabies.Add(new List<SpriteInstance>());
		Vector2 vector = SceneRenderer.GetCameraPosition() + new Vector2(-270 + (m_SelectableBabies.Count - 2) * 100, -200f);
		float num = sprites[0].WidthScale / sprites[0].GetSpriteImage().Width;
		for (int i = 0; i < sprites.Count; i++)
		{
			m_SelectableBabies.Last().Add(new SpriteInstance(sprites[i].GetSpriteImage(), default(Vector2), sprites[i].Depth));
			m_SelectableBabies.Last().Last().WidthScale *= scale;
			m_SelectableBabies.Last().Last().Position = vector + scale * ((sprites[i].Position - sprites[0].Position) / num);
			m_SelectableBabies.Last().Last().Color = c;
			if (c.A == 0)
			{
				m_SelectableBabies.Last().Last().Alpha = 0f;
			}
			m_SelectableBabies.Last().Last().Origin = scale * (sprites[i].Origin / num);
			m_SelectableBabies.Last().Last().Depth += m_SelectableBabies.Count;
		}
		if (special == 1)
		{
			m_SelectableBabies.Last().Add(TextureContainer.GetSprite("images/spritesheets/outfitPieces", new Rectangle(695, 813, 102, 74), default(Vector2), sprites.Last().Depth - 0.5f));
			m_SelectableBabies.Last().Last().WidthScale *= 0.5f;
			m_SelectableBabies.Last().Last().Position = vector - new Vector2(20f, 0f);
			m_SelectableBabies.Last().Last().Depth += m_SelectableBabies.Count;
		}
		if (special == 2)
		{
			m_SelectableBabies.Last().Add(TextureContainer.GetSprite("images/spritesheets/outfitPieces", new Rectangle(841, 785, 82, 114), default(Vector2), sprites[0].Depth + 1E-05f));
			m_SelectableBabies.Last().Last().WidthScale *= 0.7f;
			m_SelectableBabies.Last().Last().Position = vector - new Vector2(20f, -10f);
			m_SelectableBabies.Last().Last().Depth += m_SelectableBabies.Count;
		}
		if (special == 3)
		{
			SpriteInstance spriteInstance = new SpriteInstance(new SpriteImage(m_SelectableBabies.Last()[1].GetSpriteImage().GetSpritePage(), m_SelectableBabies.Last()[1].GetSpriteImage().GetPageRect()), m_SelectableBabies.Last()[1].Position, 0f);
			spriteInstance.WidthScale = m_SelectableBabies.Last()[1].WidthScale;
			spriteInstance.Color = m_SelectableBabies.Last()[1].Color;
			spriteInstance.Origin = m_SelectableBabies.Last()[1].Origin;
			spriteInstance.Depth = m_SelectableBabies.Last()[1].Depth;
			spriteInstance.GetSpriteImage().X += 300;
			spriteInstance.RecalcTexCoordinates();
			m_SelectableBabies.Last()[1] = spriteInstance;
		}
		if (special == 4)
		{
			m_SelectableBabies.Last().Add(TextureContainer.GetSprite("images/spritesheets/outfitPieces", new Rectangle(710, 918, 95, 61), default(Vector2), sprites[0].Depth + 0.0001f));
			m_SelectableBabies.Last().Last().Position = vector;
			m_SelectableBabies.Last().Last().Rotation = (float)Math.PI;
			m_SelectableBabies.Last().Last().Depth += m_SelectableBabies.Count;
		}
	}

	private void GeneratePieceDescriptions()
	{
		m_pieceDescriptions = new List<List<string>>();
		m_powerupDescriptions = new List<string>();
		int num = 0;
		for (int i = 0; i < m_pieces.Count; i++)
		{
			m_pieceDescriptions.Add(new List<string>());
			for (int j = 0; j < m_pieces[i].Count; j++)
			{
				string outfitName = MasterOfUnlocking.GetOutfitName(num);
				if (!MasterOfUnlocking.IsOutfitAvail(num))
				{
					m_pieces[i][j].Color = Color.Black;
					m_pieces[i][j].FlatColor = true;
					outfitName = outfitName + "\nLocked\n\nTo Unlock:\n" + MasterOfUnlocking.GetOutfitConditionString(num);
				}
				else
				{
					outfitName = outfitName + "\n\n" + MasterOfUnlocking.GetOutfitDescription(num);
				}
				m_pieceDescriptions[i].Add(outfitName);
				num++;
			}
		}
		for (int k = 0; k < m_SelectableBabies.Count; k++)
		{
			string powerupName = MasterOfUnlocking.GetPowerupName(k);
			if (!MasterOfUnlocking.IsPowerupAvail(k))
			{
				for (int l = 0; l < m_SelectableBabies[k].Count; l++)
				{
					m_SelectableBabies[k][l].Color = Color.Black;
					m_SelectableBabies[k][l].FlatColor = true;
				}
				powerupName = powerupName + "\nLocked\n\nUnlock Req:\n" + MasterOfUnlocking.GetPowerupConditionString(k);
			}
			else
			{
				powerupName = powerupName + "\n\n" + MasterOfUnlocking.GetPowerupDescription(k);
			}
			m_powerupDescriptions.Add(powerupName);
		}
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_bg.Draw(gameTime);
		for (int i = 0; i < m_baby.Count; i++)
		{
			m_baby[i].Draw(gameTime);
		}
		if (m_baby[0].Color.A == 0)
		{
			Vector2 position = SceneRenderer.GetCameraPosition() - m_baby[0].Position;
			SceneRenderer.Avatar.SetRotations(m_baby[1].Rotation - m_baby[0].Rotation, m_baby[3].Rotation - m_baby[0].Rotation, m_baby[2].Rotation - m_baby[0].Rotation, m_baby[0].Rotation, position, 1001f, 1f);
		}
		else
		{
			Vector2 position2 = SceneRenderer.GetCameraPosition() - m_SelectableBabies[1][0].Position;
			SceneRenderer.Avatar.SetRotations(m_baby[1].Rotation - m_baby[0].Rotation, m_baby[3].Rotation - m_baby[0].Rotation, m_baby[2].Rotation - m_baby[0].Rotation, m_baby[0].Rotation, position2, 1001f, 1f);
		}
		SceneRenderer.Avatar.ShouldDraw = true;
		if (m_baby[0].Color.A != 0)
		{
			for (int j = 0; j < m_outfitPiecesEquipped.Count; j++)
			{
				m_outfitPiecesEquipped[j].Draw(gameTime);
			}
		}
		for (int k = 0; k < m_pieces.Count; k++)
		{
			for (int l = 0; l < m_pieces[k].Count; l++)
			{
				if (l < 5 && (m_iSelectedIndexX < 5 || m_iSelectedIndexY != k))
				{
					m_pieces[k][l].Draw(gameTime);
				}
				else if (l >= 5 && m_iSelectedIndexX >= 5 && m_iSelectedIndexY == k)
				{
					m_pieces[k][l].Draw(gameTime);
				}
			}
		}
		for (int m = 0; m < m_SelectableBabies.Count; m++)
		{
			for (int n = 0; n < m_SelectableBabies[m].Count; n++)
			{
				m_SelectableBabies[m][n].Draw(gameTime);
			}
		}
		if (m_iSelectedIndexY >= 0)
		{
			SceneRenderer.DrawString(fonts.BASE_FONT, m_pieceDescriptions[m_iSelectedIndexY][m_iSelectedIndexX], m_baby[0].Position - new Vector2(110f, 260f), Color.Black, new Vector2(0.8f), 100f);
			if (m_baby[0].Color.A == 0 && m_pieces[m_iSelectedIndexY][m_iSelectedIndexX].Color != Color.Black)
			{
				SceneRenderer.DrawString(fonts.BASE_FONT, m_notAvatar, m_baby[0].Position - new Vector2(110f, 230f), Color.Black, new Vector2(0.8f), 100f);
			}
		}
		else
		{
			SceneRenderer.DrawString(fonts.BASE_FONT, m_powerupDescriptions[m_iSelectedIndexX], m_baby[0].Position - new Vector2(110f, 260f), Color.Black, new Vector2(0.8f), 100f);
		}
		m_nextPageButton.Draw(gameTime);
		SceneRenderer.DrawString(fonts.BASE_FONT, m_nextPageText, m_nextPageButton.Position + new Vector2(30f, -15f), Color.Black, 100f);
		m_light.Draw(gameTime);
		m_light2.Draw(gameTime);
		m_highLight.Draw(gameTime);
		m_sel.Position = m_highLight.Position;
		m_sel.Draw(gameTime);
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.PressedLeft(ControlManager.ActiveMenuIndex))
		{
			m_iSelectedIndexX--;
		}
		if (ControlManager.PressedRight(ControlManager.ActiveMenuIndex))
		{
			m_iSelectedIndexX++;
		}
		if (m_iSelectedIndexY >= 0)
		{
			if (m_iSelectedIndexX >= m_pieces[m_iSelectedIndexY].Count)
			{
				m_iSelectedIndexX = 0;
			}
			else if (m_iSelectedIndexX < 0)
			{
				m_iSelectedIndexX = m_pieces[m_iSelectedIndexY].Count - 1;
			}
		}
		else if (m_iSelectedIndexX >= m_SelectableBabies.Count)
		{
			m_iSelectedIndexX = 0;
		}
		else if (m_iSelectedIndexX < 0)
		{
			m_iSelectedIndexX = m_SelectableBabies.Count - 1;
		}
		if (ControlManager.PressedUp(ControlManager.ActiveMenuIndex))
		{
			m_iSelectedIndexY--;
			m_iSelectedIndexX = 0;
		}
		if (ControlManager.PressedDown(ControlManager.ActiveMenuIndex))
		{
			m_iSelectedIndexY++;
			m_iSelectedIndexX = 0;
		}
		if (m_iSelectedIndexY < -1)
		{
			m_iSelectedIndexY = m_pieces.Count - 1;
		}
		else if (m_iSelectedIndexY >= m_pieces.Count)
		{
			m_iSelectedIndexY = -1;
		}
		if (ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
		{
			EquipPiece(m_iSelectedIndexY, m_iSelectedIndexX);
		}
		if (ControlManager.PressedBackButton(ControlManager.ActiveMenuIndex) || ControlManager.PressedStart(ControlManager.ActiveMenuIndex) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
		{
			m_player.ClearOutfit();
			m_player.SetBody((PropType)m_iBodyType);
			for (int i = 0; i < m_outfitPiecesEquipped.Count; i++)
			{
				m_outfitPiecesEquipped[i].AttachedTo = m_player.GetProp().GetOutfit().GetSprites()[m_baby.IndexOf(m_outfitPiecesEquipped[i].AttachedTo)];
				m_player.AddOutfitPiece(m_outfitPiecesEquipped[i]);
			}
			ScreenStorage.PopScreen("");
		}
		if (m_iSelectedIndexY >= 0)
		{
			m_highLight.Position = m_pieces[m_iSelectedIndexY][m_iSelectedIndexX].Position;
		}
		else
		{
			m_highLight.Position = m_SelectableBabies[m_iSelectedIndexX][0].Position;
		}
		m_light.pos = new Vector3(m_highLight.Position.X, 0f - m_highLight.Position.Y, 300f);
	}

	private void EquipPiece(int type, int index)
	{
		if (type >= 0)
		{
			if (m_pieces[type][index].Color == Color.Black && m_pieces[type][index].FlatColor)
			{
				return;
			}
			for (int num = m_outfitPiecesEquipped.Count - 1; num >= 0; num--)
			{
				if (m_outfitPiecesEquipped[num].Slot == type)
				{
					m_outfitPiecesEquipped.RemoveAt(num);
				}
			}
			SpriteInstance spriteInstance = new SpriteInstance(m_pieces[type][index].GetSpriteImage(), default(Vector2), 0f);
			spriteInstance.Color = m_pieces[type][index].Color;
			spriteInstance.SurfaceScale = m_pieces[type][index].SurfaceScale * m_fScale;
			spriteInstance.Origin = m_pieces[type][index].Origin * m_fScale;
			spriteInstance.Origin = m_baby[type].Origin;
			m_outfitPiecesEquipped.Add(new OutfitPiece(spriteInstance, m_baby[type], type));
		}
		else if (!(m_SelectableBabies[index][0].Color == Color.Black) || !m_SelectableBabies[index][0].FlatColor)
		{
			Vector2 position = m_baby[0].Position;
			m_baby = new List<SpriteInstance>();
			for (int i = 0; i < m_SelectableBabies[index].Count; i++)
			{
				m_baby.Add(new SpriteInstance(m_SelectableBabies[index][i].GetSpriteImage(), default(Vector2), m_SelectableBabies[index][i].Depth));
				m_baby.Last().Color = m_SelectableBabies[index][i].Color;
				m_baby.Last().WidthScale = m_SelectableBabies[index][i].WidthScale;
				m_baby.Last().Position = position + (m_SelectableBabies[index][i].Position - m_SelectableBabies[index][0].Position);
				m_baby.Last().Origin = m_SelectableBabies[index][i].Origin;
				m_baby.Last().Alpha = m_SelectableBabies[index][i].Alpha;
				m_baby.Last().Rotation = m_SelectableBabies[index][i].Rotation;
			}
			m_fScale = m_baby[0].WidthScale / m_baby[0].GetSpriteImage().Width;
			m_outfitPiecesEquipped.Clear();
			m_iBodyType = index;
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		for (int i = 0; i < m_outfitPiecesEquipped.Count; i++)
		{
			m_outfitPiecesEquipped[i].Update(gameTime);
		}
		m_fTimer += gameTime.FractionOfSecond;
		m_light.color.A = (byte)(80 + (byte)(15f * (float)Math.Sin(3f * m_fTimer)));
		m_light.range = 1300;
		m_light.falloff = 0.3f;
		m_highLight.Alpha = 0.4f + 0.2f * (float)Math.Sin(3f * m_fTimer);
	}
}
