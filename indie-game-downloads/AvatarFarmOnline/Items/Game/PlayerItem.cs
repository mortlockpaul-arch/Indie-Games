using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Quasar;
using Quasar.Audios;
using Quasar.Behaviors;
using Quasar.ContentPipeline;
using Quasar.GameUtils.Tasks;
using Quasar.GameUtils.XBLIG.Avatar;
using Quasar.Global;
using Quasar.Items._2D;
using Quasar.Particles;
using Quasar.Shaders;
using Quasar.Textures;
using XnaToFna.StubXDK.GamerServices;

namespace AvatarFarmOnline.Items.Game;

internal class PlayerItem : RenderItem
{
	private const int stepTime = 490;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private AvatarFarmOnline.Logic.Stage.Player player;

	private bool isLocal;

	private Avatar avatar;

	private float rotation;

	private List<AvatarColladaAnimation> animations = new List<AvatarColladaAnimation>(5);

	private AvatarAnimation standAnimation;

	private AvatarAnimation clapAnimation;

	private SpatializedAudio[] stepSound;

	private int stepTempTimer;

	private bool isWalking;

	private ParticleSystem winterParticles;

	private ParticleSystem autumnParticles;

	private ParticleSystem summerParticles;

	private ParticleSystem springParticles;

	private ParticleSystem walkingParticles;

	private RenderItem meshItem;

	private Avatar loadedAvatar;

	public AvatarFarmOnline.Logic.Stage.Player Player => player;

	public PlayerItem(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.Player player)
	{
		this.stage = stage;
		this.player = player;
		Quasar.Items._2D.Rectangle rectangle = new Quasar.Items._2D.Rectangle(TextureManager.Textures["blobShadow"], new Vector2(1f));
		rectangle.Mesh.Shader = ShaderManager.Shaders["SimpleMultiply"];
		rectangle.Mesh.FirstMaterial.SetForcedAlpha(alpha: true);
		rectangle.Alpha = 0.75f;
		rectangle.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -(float)Math.PI / 2f);
		rectangle.Transform.Translation = new Vector3(0f, 0.02f, 0f);
		addChild(rectangle);
		meshItem = new RenderItem();
		meshItem.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, -(float)Math.PI / 2f);
		addChild(meshItem);
		TaskManager.Post(LoadAvatar, null, OnLoadAvatarFinished);
		SpatializedAudio.DistanceScale = 10f;
		SpatializedAudioBehavior spatializedAudioBehavior = new SpatializedAudioBehavior();
		stepSound = new SpatializedAudio[3];
		stepSound[0] = new SpatializedAudio(SoundEffectManager.SoundEffects["Step1"]);
		stepSound[1] = new SpatializedAudio(SoundEffectManager.SoundEffects["Step2"]);
		stepSound[2] = new SpatializedAudio(SoundEffectManager.SoundEffects["Step3"]);
		for (int i = 0; i < stepSound.Length; i++)
		{
			spatializedAudioBehavior.AddAudio(stepSound[i]);
		}
		addBehavior(spatializedAudioBehavior);
		isLocal = Player is AvatarFarmOnline.Logic.Stage.LocalPlayer;
		if (isLocal)
		{
			winterParticles = ParticleSystem.Load("SeasonWinter");
			addChild(winterParticles);
			autumnParticles = ParticleSystem.Load("SeasonAutumn");
			addChild(autumnParticles);
			summerParticles = ParticleSystem.Load("SeasonSummer");
			addChild(summerParticles);
			springParticles = ParticleSystem.Load("SeasonSpring");
			addChild(springParticles);
		}
		else
		{
			addChild(new AvatarFarmOnline.Items.Game.PlayerNameItem(player));
		}
		walkingParticles = ParticleSystem.Load("Walking");
		addChild(walkingParticles);
	}

	private void LoadAvatar(object parameters)
	{
		loadedAvatar = new Avatar(player.Selection.Avatar);
		loadedAvatar.Timer = stage.Timer;
		loadedAvatar.Ambient = new Vector3(0.85f);
		animations.Add(new AvatarColladaAnimation(Engine.ContentManager.Load<AvatarColladaAnimation>("Animations/Walk")));
		animations.Add(new AvatarColladaAnimation(Engine.ContentManager.Load<AvatarColladaAnimation>("Animations/Run2")));
		try
		{
			standAnimation = new AvatarAnimation((AvatarAnimationPreset)2);
			clapAnimation = new AvatarAnimation((AvatarAnimationPreset)8);
		}
		catch (Exception)
		{
		}
	}

	private void OnLoadAvatarFinished(object parameters)
	{
		avatar = loadedAvatar;
		if (avatar != null)
		{
			if (meshItem != null)
			{
				meshItem.addMesh(avatar);
			}
			if (standAnimation != null)
			{
				avatar.StartAnimation((AvatarAnimationPreset)2, standAnimation, loopAnimation: true, 0f, blend: false);
			}
		}
	}

	protected override void DoUpdate()
	{
		if (isLocal)
		{
			winterParticles.EnableEmitters = stage.FarmData.CurrentSeason == AvatarFarmOnline.Logic.Seasons.Winter;
			autumnParticles.EnableEmitters = stage.FarmData.CurrentSeason == AvatarFarmOnline.Logic.Seasons.Fall;
			summerParticles.EnableEmitters = stage.FarmData.CurrentSeason == AvatarFarmOnline.Logic.Seasons.Summer;
			springParticles.EnableEmitters = stage.FarmData.CurrentSeason == AvatarFarmOnline.Logic.Seasons.Spring;
		}
		Transform.Translation = player.WorldPosition;
		rotation = GameMath.AngleDamping(rotation, player.Rotation, 0.008f, Timer.DefaultTimer.LastIntervalSeconds);
		Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, rotation);
		isWalking = false;
		if (avatar != null)
		{
			if (player.State == AvatarFarmOnline.Logic.Stage.Player.PlayerState.Idle)
			{
				float num = player.Speed.Length();
				if (num > 0.2f)
				{
					isWalking = true;
					int index = (player.IsRunning ? 1 : 0);
					if (avatar.CurrentAnimationType != Avatar.AnimationType.Collada || avatar.CurrentColladaAnimation != animations[index])
					{
						avatar.StartAnimation(animations[index], loopAnimation: true, 0f, cloneAnimation: false, blend: true);
					}
					if (!player.IsRunning)
					{
						avatar.AnimationSpeed = 0.25f + 0.95f * num / 2.75f;
					}
					else
					{
						avatar.AnimationSpeed = 0.25f + 0.95f * num / 5f;
					}
				}
				else
				{
					if (standAnimation != null && (avatar.CurrentAnimationType != Avatar.AnimationType.BuiltIn || avatar.CurrentAnimationPreset != (AvatarAnimationPreset)2))
					{
						avatar.StartAnimation((AvatarAnimationPreset)2, standAnimation, loopAnimation: true, 0f, blend: true);
					}
					avatar.AnimationSpeed = 1f;
				}
				AvatarExpression expression = default(AvatarExpression);
				AvatarEyebrow leftEyebrow = (expression.RightEyebrow = AvatarEyebrow.Angry);
				expression.LeftEyebrow = leftEyebrow;
				expression.Mouth = AvatarMouth.Angry;
				AvatarEye leftEye = (expression.RightEye = AvatarEye.Angry);
				expression.LeftEye = leftEye;
				switch (Timer.DefaultTimer.TotalTime / 1000 % 100)
				{
				case 1L:
				{
					AvatarEye leftEye7 = (expression.RightEye = AvatarEye.LookLeft);
					expression.LeftEye = leftEye7;
					AvatarEyebrow leftEyebrow6 = (expression.RightEyebrow = AvatarEyebrow.Sad);
					expression.LeftEyebrow = leftEyebrow6;
					break;
				}
				case 35L:
				{
					AvatarEye leftEye6 = (expression.RightEye = AvatarEye.Sleeping);
					expression.LeftEye = leftEye6;
					break;
				}
				case 75L:
				{
					AvatarEye leftEye5 = (expression.RightEye = AvatarEye.Laughing);
					expression.LeftEye = leftEye5;
					AvatarEyebrow leftEyebrow5 = (expression.RightEyebrow = AvatarEyebrow.Sad);
					expression.LeftEyebrow = leftEyebrow5;
					break;
				}
				case 56L:
				{
					AvatarEye leftEye4 = (expression.RightEye = AvatarEye.Shocked);
					expression.LeftEye = leftEye4;
					AvatarEyebrow leftEyebrow4 = (expression.RightEyebrow = AvatarEyebrow.Sad);
					expression.LeftEyebrow = leftEyebrow4;
					break;
				}
				case 82L:
				{
					AvatarEye leftEye3 = (expression.RightEye = AvatarEye.Neutral);
					expression.LeftEye = leftEye3;
					AvatarEyebrow leftEyebrow3 = (expression.RightEyebrow = AvatarEyebrow.Sad);
					expression.LeftEyebrow = leftEyebrow3;
					expression.Mouth = AvatarMouth.PhoneticDth;
					break;
				}
				case 20L:
				{
					AvatarEye leftEye2 = (expression.RightEye = AvatarEye.Sad);
					expression.LeftEye = leftEye2;
					AvatarEyebrow leftEyebrow2 = (expression.RightEyebrow = AvatarEyebrow.Neutral);
					expression.LeftEyebrow = leftEyebrow2;
					expression.Mouth = AvatarMouth.Happy;
					break;
				}
				}
				if (Timer.DefaultTimer.TotalTime % 6000 > 5800)
				{
					AvatarEye leftEye8 = (expression.RightEye = AvatarEye.Yawning);
					expression.LeftEye = leftEye8;
				}
				expression.Mouth = AvatarMouth.Angry;
				avatar.Expression = expression;
			}
			else
			{
				avatar.AnimationSpeed = 1f;
				if (clapAnimation != null && (avatar.CurrentAnimationType != Avatar.AnimationType.BuiltIn || avatar.CurrentAnimationPreset != (AvatarAnimationPreset)8))
				{
					avatar.StartAnimation((AvatarAnimationPreset)8, clapAnimation, loopAnimation: true, 0.4f, blend: true);
				}
				AvatarExpression expression = default(AvatarExpression);
				AvatarEye leftEye9 = (expression.RightEye = AvatarEye.LookLeft);
				expression.LeftEye = leftEye9;
				AvatarEyebrow leftEyebrow7 = (expression.RightEyebrow = AvatarEyebrow.Angry);
				expression.LeftEyebrow = leftEyebrow7;
				expression.Mouth = AvatarMouth.PhoneticDth;
				avatar.Expression = expression;
			}
		}
		if (isWalking && !stage.IsPaused && !stage.IsOnShop && stage.IsInPlayableState)
		{
			stepTempTimer += (int)((float)stage.Timer.LastInterval * avatar.AnimationSpeed);
			if ((float)stepTempTimer >= ((!player.IsRunning) ? 490f : 362.6f))
			{
				int num2 = GameMath.Random.Next(0, stepSound.Length);
				if (!isLocal)
				{
					stepSound[num2].Volume = 0.5f;
				}
				else
				{
					stepSound[num2].Volume = ((!player.IsRunning) ? 0.35f : 0.65f);
				}
				stepSound[num2].Start(0.15f);
				walkingParticles.Burst();
				stepTempTimer = 0;
			}
		}
		else
		{
			stepTempTimer = 0;
		}
		if (avatar != null)
		{
			avatar.Update();
		}
		base.DoUpdate();
	}

	public override void Dispose()
	{
		if (stepSound != null)
		{
			for (int i = 0; i < stepSound.Length; i++)
			{
				if (stepSound[i] != null)
				{
					stepSound[i].Dispose();
					stepSound[i] = null;
				}
			}
		}
		base.Dispose();
	}
}
