using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SgMotion;
using SgMotion.Controllers;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

public class Player : SceneObject
{
	public SkinnedModel skinnedModel;

	public AnimationController animationController;

	public Vector3 Position;

	public Vector3 Velocity;

	public Vector3 oldvel = Vector3.Zero;

	public Vector3 Rotation;

	public Vector3 dodgedirection;

	public float Scale;

	public float Health = 100f;

	public float nexthealth = 100f;

	public float Stamina = 22f;

	public float totalTime;

	private float leftswim;

	private float rightswim;

	public bool moving;

	public bool movingfast;

	public playerState playerState;

	private GamePadState gpad;

	private GamePadState oldgpad;

	public bool isfirstperson;

	public Vector3 headrotation = Vector3.Zero;

	private bool allthewaydown;

	private bool allthewayup;

	public bool canmove;

	public float nextrotation;

	public Matrix lookat;

	public int dir = -1;

	public float redHealth = 100f;

	private float timertoreach;

	public bool gotogameover;

	public bool alreadyhitted;

	public bool scream;

	public bool splushsfx;

	public bool generatedparticle;

	public watertrail particletrail;

	public PlayerIndex currentplayer;

	public Player(Model model, PlayerIndex pi, Vector3 pos, Vector3 rot, float scale)
		: base(model)
	{
		currentplayer = pi;
		base.UpdateType = UpdateType.Automatic;
		base.Visibility = ObjectVisibility.RenderedAndCastShadows;
		base.World = Matrix.CreateScale(scale) * Matrix.CreateRotationY(MathHelper.ToRadians(rot.Y)) * Matrix.CreateTranslation(pos);
		Rotation = rot;
		Position = pos;
		Scale = scale;
		Position.Y = -10.5f;
		allthewayup = true;
		allthewaydown = false;
		skinnedModel = model.Tag as SkinnedModel;
		animationController = new AnimationController(skinnedModel.SkeletonBones);
		animationController.SwitchToClip(skinnedModel.AnimationClips["Idle"]);
		animationController.LoopEnabled = true;
		animationController.TranslationInterpolation = InterpolationMode.Linear;
		animationController.OrientationInterpolation = InterpolationMode.Linear;
		animationController.Speed = 1f;
	}

	public void inputQTE()
	{
		if (dir == -1)
		{
			if (gpad.IsButtonDown(Buttons.LeftThumbstickUp) && oldgpad.IsButtonUp(Buttons.LeftThumbstickUp))
			{
				dir = 0;
			}
			if (gpad.IsButtonDown(Buttons.LeftThumbstickLeft) && oldgpad.IsButtonUp(Buttons.LeftThumbstickLeft))
			{
				dir = 1;
			}
			if (gpad.IsButtonDown(Buttons.LeftThumbstickRight) && oldgpad.IsButtonUp(Buttons.LeftThumbstickRight))
			{
				dir = 2;
			}
			if (gpad.IsButtonDown(Buttons.LeftThumbstickDown) && oldgpad.IsButtonUp(Buttons.LeftThumbstickDown))
			{
				dir = 3;
			}
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (nexthealth < 0f)
		{
			nexthealth = 0f;
		}
		if (nexthealth < Health)
		{
			Health = nexthealth;
		}
		if (Health < nexthealth)
		{
			Health = nexthealth;
		}
		if (nexthealth == Health && Health < redHealth)
		{
			redHealth = Health;
		}
		if (playerState != playerState.dragged || playerState != playerState.goingup)
		{
			if (Stamina > 0f)
			{
				Stamina -= (float)gameTime.ElapsedGameTime.Milliseconds * 0.0001f;
			}
			else
			{
				Stamina = 0f;
			}
		}
		gpad = GamePad.GetState(currentplayer);
		if (playerState == playerState.dragged && animationController.AnimationClip == skinnedModel.AnimationClips["Pulled"])
		{
			if (animationController.HasFinished)
			{
				animationController.Speed = 1f;
				if (Stamina >= 1f)
				{
					playerState = playerState.goingup;
					animationController.CrossFade(skinnedModel.AnimationClips["Drowning"], new TimeSpan(0, 0, 0, 0, 250));
					animationController.LoopEnabled = false;
					timertoreach = 30f;
					return;
				}
				playerState = playerState.drown;
				animationController.CrossFade(skinnedModel.AnimationClips["Drowning"], new TimeSpan(0, 0, 0, 0, 250));
				animationController.LoopEnabled = false;
			}
			else
			{
				Position.Y -= (float)gameTime.ElapsedGameTime.Milliseconds * 0.01f;
			}
		}
		if (playerState == playerState.goingup)
		{
			if (Stamina > 0f)
			{
				if (Velocity.Y > 0f)
				{
					Velocity.Y -= (float)gameTime.ElapsedGameTime.Milliseconds * 0.002f;
					if (Velocity.Y <= 0f)
					{
						Velocity.Y = 0f;
					}
				}
				if (leftswim == 0f && gpad.IsButtonDown(Buttons.LeftTrigger) && oldgpad.IsButtonUp(Buttons.LeftTrigger))
				{
					Velocity.Y += (float)gameTime.ElapsedGameTime.Milliseconds * 0.1f;
					leftswim = 1f;
					rightswim = 0f;
				}
				if (rightswim == 0f && gpad.IsButtonDown(Buttons.RightTrigger) && oldgpad.IsButtonUp(Buttons.RightTrigger))
				{
					Velocity.Y += (float)gameTime.ElapsedGameTime.Milliseconds * 0.1f;
					leftswim = 0f;
					rightswim = 1f;
				}
				Velocity.Y = MathHelper.Clamp(Velocity.Y, -5f, 5f);
				if (timertoreach <= 0f)
				{
					if (Position.Y < -10.5f)
					{
						Velocity = Vector3.Zero;
						playerState = playerState.drown;
						if (animationController.AnimationClip != skinnedModel.AnimationClips["Drowning"])
						{
							animationController.CrossFade(skinnedModel.AnimationClips["Drowning"], new TimeSpan(0, 0, 0, 0, 250));
							animationController.LoopEnabled = false;
						}
						moving = false;
					}
				}
				else
				{
					timertoreach -= (float)gameTime.ElapsedGameTime.Milliseconds * 0.01f;
				}
				if (Velocity.Y == 0f)
				{
					if (moving)
					{
						animationController.CrossFade(skinnedModel.AnimationClips["Drowning"], new TimeSpan(0, 0, 0, 0, 250));
						animationController.LoopEnabled = false;
						moving = false;
					}
				}
				else if (!moving)
				{
					if (animationController.AnimationClip == skinnedModel.AnimationClips["Drowning"])
					{
						animationController.LoopEnabled = true;
						animationController.CrossFade(skinnedModel.AnimationClips["SwimUp"], new TimeSpan(0, 0, 0, 0, 250));
						moving = true;
					}
					if (animationController.AnimationClip == skinnedModel.AnimationClips["Pulled"])
					{
						animationController.LoopEnabled = true;
						animationController.CrossFade(skinnedModel.AnimationClips["SwimUp"], new TimeSpan(0, 0, 0, 0, 250));
						moving = true;
					}
				}
				if (Position.Y >= -10.5f)
				{
					Velocity = Vector3.Zero;
					moving = false;
					playerState = playerState.game;
					Health = 33f;
					nexthealth = 33f;
					redHealth = 33f;
					animationController.CrossFade(skinnedModel.AnimationClips["Idle"], new TimeSpan(0, 0, 0, 0, 100));
					animationController.LoopEnabled = true;
				}
				Position += Velocity / 20f;
			}
			else
			{
				Stamina = 0f;
				moving = false;
				Velocity = Vector3.Zero;
				if (animationController.AnimationClip == skinnedModel.AnimationClips["Drowning"])
				{
					if (animationController.HasFinished)
					{
						playerState = playerState.dead;
					}
				}
				else
				{
					Velocity = Vector3.Zero;
					playerState = playerState.drown;
					animationController.CrossFade(skinnedModel.AnimationClips["Drowning"], new TimeSpan(0, 0, 0, 0, 100));
					animationController.LoopEnabled = false;
					moving = false;
				}
			}
		}
		if (playerState == playerState.drown && animationController.AnimationClip == skinnedModel.AnimationClips["Drowning"] && animationController.HasFinished)
		{
			playerState = playerState.dead;
			gotogameover = true;
		}
		if (playerState == playerState.game)
		{
			if (nexthealth == 0f || Stamina == 0f)
			{
				scream = true;
				playerState = playerState.dragged;
				animationController.CrossFade(skinnedModel.AnimationClips["Pulled"], new TimeSpan(0, 0, 0, 0, 250));
				animationController.Speed = 0.5f;
				animationController.LoopEnabled = false;
				return;
			}
			if (Velocity.Z == 0f && Velocity.X == 0f)
			{
				if (moving)
				{
					animationController.CrossFade(skinnedModel.AnimationClips["Idle"], new TimeSpan(0, 0, 0, 0, 250));
				}
				moving = false;
			}
			else if (Math.Abs(Velocity.Z) >= 4.5f || Math.Abs(Velocity.X) >= 4.5f)
			{
				if (!movingfast && animationController.AnimationClip == skinnedModel.AnimationClips["Swim"])
				{
					animationController.CrossFade(skinnedModel.AnimationClips["SwimFast"], new TimeSpan(0, 0, 0, 0, 250));
					movingfast = true;
				}
			}
			else if (movingfast)
			{
				if (animationController.AnimationClip == skinnedModel.AnimationClips["SwimFast"])
				{
					animationController.CrossFade(skinnedModel.AnimationClips["Swim"], new TimeSpan(0, 0, 0, 0, 250));
					movingfast = false;
				}
			}
			else if (!moving && animationController.AnimationClip == skinnedModel.AnimationClips["Idle"])
			{
				animationController.CrossFade(skinnedModel.AnimationClips["Swim"], new TimeSpan(0, 0, 0, 0, 250));
				moving = true;
			}
			Rotation.Y -= gpad.ThumbSticks.Left.X * (float)gameTime.ElapsedGameTime.Milliseconds * 0.1f;
			if (isfirstperson)
			{
				headrotation.Y = Rotation.Y;
				headrotation.X -= gpad.ThumbSticks.Left.Y * (float)gameTime.ElapsedGameTime.Milliseconds * 0.1f;
				headrotation.X = MathHelper.Clamp(headrotation.X, -45f, 80f);
			}
			Vector3 vector = -Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)).Forward;
			if (Velocity.X > 0f)
			{
				Velocity.X -= (float)gameTime.ElapsedGameTime.Milliseconds * 0.002f;
				if (Velocity.X <= 0f)
				{
					Velocity.X = 0f;
				}
			}
			else if (Velocity.X < 0f)
			{
				Velocity.X += (float)gameTime.ElapsedGameTime.Milliseconds * 0.002f;
				if (Velocity.X >= 0f)
				{
					Velocity.X = 0f;
				}
			}
			if (Velocity.Z > 0f)
			{
				Velocity.Z -= (float)gameTime.ElapsedGameTime.Milliseconds * 0.002f;
				if (Velocity.Z <= 0f)
				{
					Velocity.Z = 0f;
				}
			}
			else if (Velocity.Z < 0f)
			{
				Velocity.Z += (float)gameTime.ElapsedGameTime.Milliseconds * 0.002f;
				if (Velocity.Z >= 0f)
				{
					Velocity.Z = 0f;
				}
			}
			if (!isfirstperson)
			{
				if (!allthewayup)
				{
					if (Position.Y < -10.5f)
					{
						Position.Y += (float)gameTime.ElapsedGameTime.Milliseconds * 0.01f;
					}
					else
					{
						allthewayup = true;
						splushsfx = true;
					}
				}
				else
				{
					if (leftswim == 0f && gpad.IsButtonDown(Buttons.LeftTrigger) && oldgpad.IsButtonUp(Buttons.LeftTrigger))
					{
						Velocity += vector * gameTime.ElapsedGameTime.Milliseconds * 0.05f;
						leftswim = 1f;
						rightswim = 0f;
					}
					if (rightswim == 0f && gpad.IsButtonDown(Buttons.RightTrigger) && oldgpad.IsButtonUp(Buttons.RightTrigger))
					{
						Velocity += vector * gameTime.ElapsedGameTime.Milliseconds * 0.05f;
						leftswim = 0f;
						rightswim = 1f;
					}
					Position.Y = -10.5f - (float)(Math.Sin(totalTime * 16f + Position.Z) + Math.Sin(totalTime * 16f + Position.X)) * 0.1f;
				}
			}
			else if (!allthewaydown)
			{
				if (Position.Y > -14.5f)
				{
					Position.Y -= (float)gameTime.ElapsedGameTime.Milliseconds * 0.01f;
				}
				else
				{
					allthewaydown = true;
				}
			}
			else
			{
				Position.Y = -14.5f + (float)(Math.Sin(totalTime * 16f + Position.Z) + Math.Sin(totalTime * 16f + Position.X)) * 0.1f;
			}
			Velocity.X = MathHelper.Clamp(Velocity.X, -5f, 5f);
			Velocity.Z = MathHelper.Clamp(Velocity.Z, -5f, 5f);
			Position += Velocity / 20f;
			if (allthewaydown || allthewayup)
			{
				if (gpad.IsButtonDown(Buttons.Y) && oldgpad.IsButtonUp(Buttons.Y) && !isfirstperson)
				{
					allthewaydown = false;
					allthewayup = false;
					isfirstperson = true;
				}
				if (gpad.IsButtonUp(Buttons.Y) && oldgpad.IsButtonDown(Buttons.Y) && isfirstperson)
				{
					allthewaydown = false;
					allthewayup = false;
					isfirstperson = false;
				}
				if (gpad.IsButtonDown(Buttons.Y))
				{
					isfirstperson = true;
				}
				else
				{
					isfirstperson = false;
				}
			}
		}
		if (playerState == playerState.waitingqte)
		{
			allthewaydown = false;
			allthewayup = true;
			Position.Y = -10.5f + (float)Math.Sin((double)totalTime * 0.0001) * 0.3f;
			faceshark();
			rotatecam(gameTime);
		}
		if (playerState == playerState.qte)
		{
			allthewaydown = false;
			allthewayup = true;
			inputQTE();
			Position.Y = -10.5f + (float)Math.Sin((double)totalTime * 0.0001) * 0.3f;
			faceshark();
			rotateme(gameTime);
		}
		if (playerState == playerState.Dodge || playerState == playerState.Hit)
		{
			if (animationController.HasFinished)
			{
				if (animationController.AnimationClip == skinnedModel.AnimationClips["Dodge"] || animationController.AnimationClip == skinnedModel.AnimationClips["Hit"])
				{
					animationController.LoopEnabled = true;
					animationController.CrossFade(skinnedModel.AnimationClips["Idle"], new TimeSpan(0, 0, 0, 0, 250));
					playerState = playerState.game;
				}
			}
			else
			{
				Position += Velocity / 20f;
			}
		}
		Position.X = MathHelper.Clamp(Position.X, -1300f, 1300f);
		Position.Z = MathHelper.Clamp(Position.Z, -1000f, 0f);
		base.World = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)) * Matrix.CreateTranslation(Position);
		animationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
		base.SkinBones = animationController.SkinnedBoneTransforms;
		oldvel = Velocity;
		oldgpad = gpad;
		base.Update(gameTime);
	}

	private void faceshark()
	{
		float num = Position.X - lookat.Translation.X;
		float num2 = Position.Z - lookat.Translation.Z;
		float radians = (float)Math.Atan2(0f - num, 0f - num2);
		nextrotation = MathHelper.ToDegrees(radians);
	}

	private void rotatecam(GameTime gameTime)
	{
		if (nextrotation < 0f)
		{
			nextrotation += 360f;
		}
		if (nextrotation > 360f)
		{
			nextrotation -= 360f;
		}
		if (nextrotation > Rotation.Y)
		{
			if (nextrotation - Rotation.Y > 180f)
			{
				Rotation.Y += 360f;
			}
		}
		else if (Rotation.Y - nextrotation > 180f)
		{
			Rotation.Y -= 360f;
		}
	}

	private void rotateme(GameTime gameTime)
	{
		rotatecam(gameTime);
		if (nextrotation == Rotation.Y)
		{
			return;
		}
		if (nextrotation > Rotation.Y)
		{
			Rotation.Y += (float)gameTime.ElapsedGameTime.Milliseconds * 0.5f;
			Rotation.Y = MathHelper.Clamp(Rotation.Y, Rotation.Y, nextrotation);
			if (nextrotation < Rotation.Y)
			{
				Rotation.Y = nextrotation;
			}
		}
		if (nextrotation < Rotation.Y)
		{
			Rotation.Y -= (float)gameTime.ElapsedGameTime.Milliseconds * 0.5f;
			Rotation.Y = MathHelper.Clamp(Rotation.Y, nextrotation, Rotation.Y);
			if (nextrotation > Rotation.Y)
			{
				Rotation.Y = nextrotation;
			}
		}
	}
}
