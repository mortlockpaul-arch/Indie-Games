using System;
using Microsoft.Xna.Framework;
using Renderer;

namespace PlayObjects;

public class AwardPopup
{
	private const int QUIET_TIME = 1000;

	private const int ENTER_TIME = 200;

	private const int EXIT_TIME = 200;

	private SpriteInstance m_spr;

	private int m_iTimer;

	private Vector2 m_vStartScale;

	private static int sm_iNumSpawned;

	public AwardPopup(PropType type)
	{
		m_spr = GetSpriteType(type);
		m_spr.Depth = 5000 + sm_iNumSpawned;
		m_vStartScale = m_spr.SurfaceScale;
		sm_iNumSpawned++;
	}

	public static SpriteInstance GetSpriteType(PropType type)
	{
		switch (type)
		{
		case PropType.NURSE:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(0, 0, 204, 270), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.OTHERMOTHER:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(218, 16, 192, 284), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.DIAPER_PILE:
		case PropType.CHANGING_TABLE:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(417, 18, 328, 286), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.BEAR_FATHER:
		case PropType.TRIAL_BEAR:
		case PropType.TOY_BEAR:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(741, 15, 265, 227), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.RECEPTION:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(1, 286, 282, 312), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.SLEEPING_FATHER:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(533, 339, 231, 277), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.WHEELCHAIR:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(813, 260, 180, 270), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.TV:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(8, 600, 252, 247), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.XRAY_FULL:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(257, 741, 211, 275), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.XRAY_DOUBLE:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(482, 700, 296, 324), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.FLOWER_TABLE:
			return TextureContainer.GetSprite("images/awards1", new Rectangle(792, 757, 219, 249), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.PROCTOL_PATIENT:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(8, 13, 244, 259), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.HEAD_TRAUMA:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(277, 9, 185, 297), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.SURGERY_PATIENT:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(520, 14, 292, 263), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.DOCTOR:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(851, 11, 164, 309), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.TISSUES:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(3, 279, 275, 260), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.DIRECTOR:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(303, 335, 237, 307), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.DIAG_BOARD:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(641, 334, 312, 358), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.CRASHCART:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(36, 567, 203, 162), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.BOTTLE_TABLE:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(19, 752, 212, 234), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.MRI_MACHINE:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(251, 709, 288, 283), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.CURTAIN:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(550, 731, 215, 269), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.BODYCAST:
			return TextureContainer.GetSprite("images/awards2", new Rectangle(772, 700, 252, 317), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.DOG:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(11, 11, 217, 273), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.PICNICTABLE2FAT:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(250, 17, 193, 231), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.BBQ:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(491, 20, 256, 310), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.PICNIC_FLOOR:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(751, 10, 265, 317), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.STATUE:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(9, 302, 222, 244), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.BOUNCY_DRAGON:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(242, 262, 252, 290), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.ICE_CREAM:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(505, 352, 224, 293), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.PIPE:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(752, 362, 258, 177), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.BIKE:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(19, 551, 215, 252), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.TIRE:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(284, 578, 217, 256), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.VOLLEYBALL:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(505, 674, 256, 256), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.GRASS_CUTTER:
			return TextureContainer.GetSprite("images/awards3", new Rectangle(787, 554, 185, 294), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.SANITIZER:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(7, 16, 182, 223), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.CELL_BOOTH:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(227, 47, 152, 230), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.CAMERA_DESK:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(433, 28, 206, 214), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.FLOPPY_DESK:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(710, 23, 238, 208), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.COFFEECHAIRTABLE2:
		case PropType.COFFEESTOOLTABLE2:
		case PropType.COFFEECOUCHTABLE2:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(9, 254, 228, 232), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.ATM:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(272, 289, 189, 248), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.PEARL_NECKLACE:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(496, 250, 227, 285), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.BLOCK_STACK:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(745, 244, 166, 209), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.DUMMY:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(34, 494, 203, 303), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.RUNNER1:
		case PropType.RUNNER2:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(232, 538, 269, 313), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.CLOWN:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(518, 565, 220, 298), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.SKATEBOARDER:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(746, 469, 229, 224), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.PAINTER:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(760, 700, 188, 314), default(Vector2), 5000 + sm_iNumSpawned);
		case PropType.GLASS_PANEL:
			return TextureContainer.GetSprite("images/awards4", new Rectangle(3, 796, 244, 228), default(Vector2), 5000 + sm_iNumSpawned);
		default:
		{
			SpriteInstance sprite = TextureContainer.GetSprite("images/awards3", new Rectangle(58, 835, 173, 173), default(Vector2), 5000 + sm_iNumSpawned);
			sprite.FlatColor = true;
			sprite.Alpha = 0f;
			return sprite;
		}
		}
	}

	public void Update(TimeTracker gameTime)
	{
		float cameraZoom = SceneRenderer.GetCameraZoom();
		m_iTimer += gameTime.ElapsedMilli;
		if (m_iTimer < 200)
		{
			m_spr.SurfaceScale = m_vStartScale * (float)Math.Sin(2f * ((float)m_iTimer / 200f)) / cameraZoom;
		}
		else if (m_iTimer < 1200)
		{
			m_spr.SurfaceScale = m_vStartScale / cameraZoom;
		}
		else
		{
			m_spr.SurfaceScale = m_vStartScale * (float)Math.Cos(2f * ((float)(m_iTimer - 1200) / 200f)) / cameraZoom;
		}
		m_spr.Position = SceneRenderer.GetCameraPosition() + (new Vector2(0f, 200f) + new Vector2(SceneRenderer.GetRand(-5f, 5f), SceneRenderer.GetRand(-5f, 5f))) / cameraZoom;
		m_spr.Rotation = SceneRenderer.GetRand(-0.05f, 0.05f);
	}

	public void Draw(TimeTracker gameTime)
	{
		m_spr.Draw(gameTime);
	}

	public void ForceExit()
	{
		if (m_iTimer < 1200)
		{
			m_iTimer = 1200;
		}
	}

	public bool IsActive()
	{
		return m_iTimer < 1400;
	}
}
