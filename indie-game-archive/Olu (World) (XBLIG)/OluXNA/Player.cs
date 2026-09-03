using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Player
{
	public ModelWrapper mPlayer;

	public ModelWrapper mGrid;

	public float playerRot;

	public float playerRate;

	public float scaleAmount;

	public float scaleCooldown;

	public float cooldownRate;

	public Dictionary<ModelBone, int> playerBones;

	public ModelOluAnimator playerAnim;

	public AnimationController idle;

	public AnimationController spin;

	public int level;

	public PlaneDetachColl pdColl;

	public PlayerNull pn;

	public Player()
	{
		playerRate = 0.5f;
		cooldownRate = 2f;
		playerRot = (scaleCooldown = 0f);
		scaleAmount = 0.2f;
		level = 1;
		pn = new PlayerNull();
	}

	public void Update(GameTime gametime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Zero;
		KeyboardState curKey = BaseGame.Get().input.curKey;
		if (((KeyboardState)(ref curKey)).IsKeyDown((Keys)38))
		{
			val += new Vector3(0f, -1f, 0f);
		}
		KeyboardState curKey2 = BaseGame.Get().input.curKey;
		if (((KeyboardState)(ref curKey2)).IsKeyDown((Keys)40))
		{
			val += new Vector3(0f, 1f, 0f);
		}
		KeyboardState curKey3 = BaseGame.Get().input.curKey;
		if (((KeyboardState)(ref curKey3)).IsKeyDown((Keys)37))
		{
			val += new Vector3(-1f, 0f, 0f);
		}
		KeyboardState curKey4 = BaseGame.Get().input.curKey;
		if (((KeyboardState)(ref curKey4)).IsKeyDown((Keys)39))
		{
			val += new Vector3(1f, 0f, 0f);
		}
		GamePadState curPad = BaseGame.Get().input.curPad;
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref curPad)).ThumbSticks;
		Vector2 left = ((GamePadThumbSticks)(ref thumbSticks)).Left;
		val += new Vector3(left.X, left.Y, 0f);
		scaleCooldown = Math.Max(BaseGame.Get().channels[28], BaseGame.Get().channels[29]);
		playerRot += playerRate * (float)gametime.ElapsedGameTime.TotalSeconds;
		if (playerRot > 3.14159f)
		{
			playerRot -= 3.141529f;
		}
		((GameComponent)spin).Update(gametime);
		((GameComponent)playerAnim).Update(gametime);
		pdColl.act(gametime);
		if (BaseGame.Get().invert)
		{
			val.Y *= -1f;
		}
		BaseGame.Get().MoveCursor(val);
	}

	public void DrawPlayer(GameTime gametime)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Side");
		BaseGame.Get().fogEffect.Parameters["xDoubleSided"].SetValue(true);
		BaseGame.Get().fogEffect.Parameters["SideSplice"].SetValue((BaseGame.Get().weaponMode - 0.5f) * -3f);
		BaseGame.Get().fogEffect.Parameters["SideShowLeft"].SetValue(false);
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(BaseGame.MapObjectToSystem(Vector3.Zero, BaseGame.Get().playerDir, BaseGame.Get().playerUp));
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f + scaleAmount * scaleCooldown));
		BaseGame.Get().DrawModel(ref mPlayer);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().fogEffect.Parameters["SideShowLeft"].SetValue(true);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().DrawModel(ref mPlayer);
		BaseGame.Get().matStack.PopMatrix();
		pdColl.draw(gametime);
		BaseGame.Get().SwitchEffectTechnique("Textured");
		if (BaseGame.Get().weaponMode > 0.1f && BaseGame.Get().weaponMode < 0.9f)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(new Vector3((BaseGame.Get().weaponMode - 0.5f) * 3f, 0f, 0f)));
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f));
			BaseGame.Get().DrawModel(ref mGrid);
			BaseGame.Get().matStack.PopMatrix();
		}
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
	}

	public void PlayOnDown()
	{
		BaseGame.Get().PlayCue("oneClap");
	}

	public void PlayOnLock(float volume)
	{
		if (BaseGame.Get().numTargeted <= 8)
		{
			BaseGame.Get().PlayCue("lockon0" + BaseGame.Get().numTargeted, volume);
		}
		else
		{
			BaseGame.Get().PlayCue("lockon01", volume);
		}
	}

	public void PlayOnUp()
	{
	}
}
