using System;
using DataContent;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace GameEngine;

public class ZombieBot : BaseData
{
	public bool InUse;

	public float TransitionTime;

	public int CollisionIndex;

	public Vector3 SpawnPosition = Vector3.Zero;

	public Matrix[] WorldTransform = new Matrix[2];

	public int BodyPartDamage;

	public static BotPhysics basePhysics;

	public static Model[] baseModel = new Model[5];

	public static SkinnedInstanceEffectParams[] baseModelEffects = new SkinnedInstanceEffectParams[5];

	private static string[] baseModelNames = new string[5] { "Zombie00", "Zombie01", "Zombie02", "Zombie03", "Zombie04" };

	private static Model ZombieShadow;

	private static SkinnedInstanceEffectParams ZombieShadowEffect;

	private new static Random RandGenerator = new Random();

	private static Animation tmpPlayer = new Animation();

	private static bool IsInitialized = false;

	private static int animCounter = 0;

	private Vector3 tmpNorm = Vector3.UnitY;

	private static BoundingSphere tmpSphere = default(BoundingSphere);

	private static BoundingSphere tmpModelSphere = new BoundingSphere(Vector3.Zero, 120f);

	private static Vector4 animBoneFrame = Vector4.Zero;

	private static ModelMeshPart drawMeshPart = null;

	private static Matrix matScale = Matrix.CreateScale(0.72f);

	private static Vector3 vecDrawVec = Vector3.Zero;

	private static Matrix tmpMatDrawMatrix = Matrix.Identity;

	public static Vector3 RayCastHitPosition = Vector3.Zero;

	private static Vector3 tmpOrigin = Vector3.Zero;

	private static Vector3 tmpDirection = Vector3.UnitX;

	private static Matrix tmpInverse = Matrix.Identity;

	private static Matrix tmpInverseRotate = Matrix.Identity;

	private static Vector3 CoOpOffset = new Vector3(0f, 0f, 0f);

	private static Matrix drawtmpMatrix = Matrix.Identity;

	public ZombieBot(bool useWeapon)
		: base(useWeapon)
	{
		InUse = false;
		ModelIndex = RandGenerator.Next(5);
		for (int i = 0; i < 2; i++)
		{
			Render[i] = false;
			ref Matrix reference = ref WorldTransform[i];
			reference = Matrix.CreateScale(0.7f);
		}
		ApplyGravity = true;
		TimeSpan elapsedGameTime = new TimeSpan(166670L);
		Matrix transform = Matrix.Identity;
		if (!IsInitialized)
		{
			IsInitialized = true;
			for (int j = 0; j < 5; j++)
			{
				baseModel[j] = EndGameEngine.GameAssetMgr.Load<Model>("models\\characters\\" + baseModelNames[j]);
				for (int k = 0; k < baseModel[j].Meshes.Count; k++)
				{
					ModelMesh modelMesh = baseModel[j].Meshes[k];
					for (int l = 0; l < modelMesh.MeshParts.Count; l++)
					{
						baseModelEffects[j] = new SkinnedInstanceEffectParams(modelMesh.MeshParts[l].Effect);
					}
				}
			}
			basePhysics = new BotPhysics(EndGameEngine.GameAssetMgr.Load<Model>("models\\characters\\char_physics"));
			ZombieShadow = EndGameEngine.GameAssetMgr.Load<Model>("models\\characters\\ZombieShadow");
			for (int m = 0; m < ZombieShadow.Meshes.Count; m++)
			{
				ModelMesh modelMesh2 = ZombieShadow.Meshes[m];
				for (int n = 0; n < modelMesh2.MeshParts.Count; n++)
				{
					ZombieShadowEffect = new SkinnedInstanceEffectParams(modelMesh2.MeshParts[n].Effect);
				}
			}
		}
		AnimPlayer.Initialize(baseModel[ModelIndex], 0);
		AnimPlayer.SetBaseAnimation(WeaponAnim.ZombieAttack0);
		AnimPlayer.Update(elapsedGameTime, ref transform, 0, 1f);
		if (EndGameEngine.GameSettings.GameName.Contains("Tower Defense"))
		{
			CurrentAnimation = WeaponAnim.ZombieRun;
		}
		else
		{
			CurrentAnimation = WeaponAnim.CoOpIdle;
		}
		AnimTexture = base.CurrentAnimationState.AnimationTexture;
		for (int num = 0; num < baseModel[ModelIndex].Meshes.Count; num++)
		{
			ModelMesh modelMesh3 = baseModel[ModelIndex].Meshes[num];
			for (int num2 = 0; num2 < modelMesh3.MeshParts.Count; num2++)
			{
				_ = modelMesh3.MeshParts[num2];
				baseModelEffects[ModelIndex].AnimationTextureMap.SetValue(AnimTexture);
			}
		}
		mRagdoll.SetSkinData(baseModel[ModelIndex], 1f);
		BotState = AIStateMachine.allStates[14];
		BotState.CurrentState(this, 0);
	}

	public new void Update(float etime, int qIndex, bool getCollision)
	{
		if (Health > 0)
		{
			DistanceScalar = PlayerDistanceSqr[qIndex] / 1000000f;
			DistanceScalar = ((DistanceScalar < 1f) ? DistanceScalar : 1f);
			UpdateNetworkTimer += etime;
			UpdateNetworkTimerTripped = false;
			if (UpdateNetworkTimer >= UpdateNetworkTimeStep)
			{
				UpdateNetworkTimer -= UpdateNetworkTimeStep;
				UpdateNetworkTimerTripped = true;
				if (AIBase.ZombieRayCastToPlayer(this, qIndex, directionTest: true, null))
				{
					if (EGENetWorkNext.networkSession != null)
					{
						if (EGENetWorkNext.networkSession.IsHost)
						{
							if (EGENetWorkNext.networkSession.LocalGamers[0] != null && EGENetWorkNext.networkSession.LocalGamers[0].Tag != null)
							{
								byte b = (byte)((((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).vecPosition - BotHordeRef.pos).Length() / 16f);
								for (int i = 0; i < 16; i++)
								{
									if (((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).PlayerLineOfSight[i] == 0 || ((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).PlayerLineOfSight[i] == BotHordeRef._uid)
									{
										((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).PlayerLineOfSight[i] = BotHordeRef._uid;
										((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).PlayerDistanceQuant[i] = b;
										break;
									}
									if ((BotHordeRef.zFlags & 0x80) == 0 || (BotHordeRef.zFlags & 0x40) == 0)
									{
										((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).PlayerLineOfSight[i] = BotHordeRef._uid;
										((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).PlayerDistanceQuant[i] = b;
										break;
									}
									if (((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).PlayerDistanceQuant[i] > b)
									{
										((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).PlayerLineOfSight[i] = BotHordeRef._uid;
										((PlayerBase)EGENetWorkNext.networkSession.LocalGamers[0].Tag).PlayerDistanceQuant[i] = b;
										break;
									}
								}
							}
						}
						else
						{
							byte value = (byte)((LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - BotHordeRef.pos).Length() / 16f);
							PacketWriter packetWriter = EGENetWorkNext.packetWriter;
							packetWriter.Write((byte)125);
							packetWriter.Write(BotHordeRef._uid);
							packetWriter.Write(value);
						}
					}
					else
					{
						byte b2 = (byte)((LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - BotHordeRef.pos).Length() / 16f);
						for (int j = 0; j < 16; j++)
						{
							if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerLineOfSight[j] == 0 || LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerLineOfSight[j] == BotHordeRef._uid)
							{
								LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerLineOfSight[j] = BotHordeRef._uid;
								LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerDistanceQuant[j] = b2;
								break;
							}
							if ((BotHordeRef.zFlags & 0x80) == 0 || (BotHordeRef.zFlags & 0x40) == 0)
							{
								LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerLineOfSight[j] = BotHordeRef._uid;
								LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerDistanceQuant[j] = b2;
								break;
							}
							if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerDistanceQuant[j] > b2)
							{
								LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerLineOfSight[j] = BotHordeRef._uid;
								LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerDistanceQuant[j] = b2;
								break;
							}
						}
					}
				}
			}
			GetCollision = getCollision;
			BotState.CurrentState(this, qIndex);
			int treePositions = LevelBaseMenu.tmpTerrainVegitation.GetTreePositions(ref Position, qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value]);
			for (int k = 0; k < treePositions; k++)
			{
				tmpNorm = Position - TerrainVegetation.GetTreePosList[k];
				tmpNorm.Y = 0f;
				if (tmpNorm.LengthSquared() < 6400f)
				{
					Position.X += tmpNorm.X * (1f - tmpNorm.LengthSquared() / 6400f);
					Position.Z += tmpNorm.Z * (1f - tmpNorm.LengthSquared() / 6400f);
				}
			}
			ref Matrix reference = ref WorldTransform[qIndex];
			reference = Matrix.Identity;
			WorldTransform[qIndex].Forward = Direction * -1f;
			WorldTransform[qIndex].Right = Vector3.Cross(Direction * -1f, Vector3.UnitY);
			WorldTransform[qIndex].Up = Vector3.UnitY;
			ref Matrix reference2 = ref WorldTransform[qIndex];
			reference2 = WorldTransform[qIndex] * matScale;
			WorldTransform[qIndex].Translation = Position;
			tmpModelSphere.Center = Position;
			tmpModelSphere.Center.X -= LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].X;
			tmpModelSphere.Center.Z -= LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].Z;
			tmpModelSphere.Center.Y = 0f;
			if (tmpModelSphere.Center.LengthSquared() < 64000000f)
			{
				tmpModelSphere.Center.Y = Position.Y;
				ContainmentType containmentType = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].bFrustum[qIndex].Contains(tmpModelSphere);
				if (containmentType == ContainmentType.Contains || containmentType == ContainmentType.Intersects)
				{
					Render[qIndex] = true;
					InFrustum = true;
					ZombieLODEntry botHordeRef = BotHordeRef;
					if (botHordeRef != null && botHordeRef.FrameIndex[qIndex] == 0f)
					{
						AnimPlayer.ReStartCurrentClip();
					}
					AnimPlayer.UpdateTimeStep();
				}
				else
				{
					Render[qIndex] = false;
					InFrustum = false;
				}
			}
			else
			{
				Render[qIndex] = false;
				InFrustum = false;
			}
		}
		else
		{
			Render[qIndex] = false;
			InFrustum = false;
		}
		AnimPlayer.UpdateOnlyTransitionTime(EndGameEngine.currentEleapsedTime.ElapsedGameTime);
		SetRagdoll(qIndex);
	}

	public void PreDraw(PlayerBase viewer, int qIndex, int mIndex)
	{
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		for (int i = 0; i < baseModel[mIndex].Meshes.Count; i++)
		{
			ModelMesh modelMesh = baseModel[mIndex].Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				drawMeshPart = modelMesh.MeshParts[j];
				baseModelEffects[mIndex].AnimationTextureMap.SetValue(AnimTexture);
				graphicsDevice.VertexSamplerStates[0] = SamplerState.PointWrap;
				graphicsDevice.VertexSamplerStates[1] = SamplerState.PointWrap;
				graphicsDevice.SamplerStates[4] = SamplerState.PointWrap;
				graphicsDevice.SamplerStates[5] = SamplerState.PointWrap;
				animBoneFrame.X = 1f / (float)AnimTexture.Width;
				animBoneFrame.Y = 1f / (float)AnimTexture.Height;
				animBoneFrame.Z = animBoneFrame.X / 2f;
				animBoneFrame.W = animBoneFrame.Y / 2f;
				baseModelEffects[mIndex].animationBoneFrame.SetValue(animBoneFrame);
				baseModelEffects[mIndex].vecEyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
				baseModelEffects[mIndex].matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
				graphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				graphicsDevice.Indices = drawMeshPart.IndexBuffer;
			}
		}
	}

	public void Draw(PlayerBase playerRef, int qIndex, int mIndex)
	{
		if (Render[qIndex])
		{
			GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
			vecDrawVec.Y = 0f;
			vecDrawVec.X = 0f - playerRef.vecHeadPosition[qIndex].X;
			vecDrawVec.Z = 0f - playerRef.vecHeadPosition[qIndex].Z;
			tmpMatDrawMatrix = WorldTransform[qIndex];
			tmpMatDrawMatrix.Translation += vecDrawVec;
			for (int i = 0; i < baseModel[mIndex].Meshes.Count; i++)
			{
				ModelMesh modelMesh = baseModel[mIndex].Meshes[i];
				for (int j = 0; j < modelMesh.MeshParts.Count; j++)
				{
					drawMeshPart = modelMesh.MeshParts[j];
					baseModelEffects[mIndex].matSkinnedWorldTransform.SetValue(tmpMatDrawMatrix);
					Vector2 zero = Vector2.Zero;
					zero.X = (int)BotHordeRef.FrameIndex[qIndex];
					zero.Y = BotHordeRef.FrameIndex[qIndex] % 1f;
					baseModelEffects[mIndex].currentFrame.SetValue(zero);
					float animationBlendTime = AnimPlayer.GetAnimationBlendTime();
					if (animationBlendTime < 1f)
					{
						Texture2D animationTexture = base.PreviosAnimationState.AnimationTexture;
						drawMeshPart.Effect.Parameters["AnimationTextureMap2"].SetValue(animationTexture);
						drawMeshPart.Effect.Parameters["animationBlendTime"].SetValue(animationBlendTime);
						animBoneFrame.X = 1f / (float)animationTexture.Width;
						animBoneFrame.Y = 1f / (float)animationTexture.Height;
						animBoneFrame.Z = animBoneFrame.X / 2f;
						animBoneFrame.W = animBoneFrame.Y / 2f;
						drawMeshPart.Effect.Parameters["animationBoneFrame2"].SetValue(animBoneFrame);
						Vector2 zero2 = Vector2.Zero;
						zero2.X = (int)PrevFrameIndex;
						zero2.Y = PrevFrameIndex % 1f;
						drawMeshPart.Effect.Parameters["currentFrame2"].SetValue(zero2);
						drawMeshPart.Effect.CurrentTechnique.Passes[11].Apply();
					}
					else
					{
						drawMeshPart.Effect.CurrentTechnique.Passes[1].Apply();
					}
					graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
		if (mRagdoll.IsValid)
		{
			DrawRagdoll(qIndex, playerRef);
		}
	}

	public void PreDrawShadow(PlayerBase viewer, ref Matrix lightViewProj, ref Vector3 lightPos, int qIndex)
	{
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		for (int i = 0; i < ZombieShadow.Meshes.Count; i++)
		{
			ModelMesh modelMesh = ZombieShadow.Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				drawMeshPart = modelMesh.MeshParts[j];
				ZombieShadowEffect.AnimationTextureMap.SetValue(AnimTexture);
				graphicsDevice.VertexSamplerStates[0] = SamplerState.PointWrap;
				graphicsDevice.VertexSamplerStates[1] = SamplerState.PointWrap;
				graphicsDevice.SamplerStates[4] = SamplerState.PointWrap;
				graphicsDevice.SamplerStates[5] = SamplerState.PointWrap;
				animBoneFrame.X = 1f / (float)AnimTexture.Width;
				animBoneFrame.Y = 1f / (float)AnimTexture.Height;
				animBoneFrame.Z = animBoneFrame.X / 2f;
				animBoneFrame.W = animBoneFrame.Y / 2f;
				ZombieShadowEffect.animationBoneFrame.SetValue(animBoneFrame);
				ZombieShadowEffect.vecEyePosition.SetValue(lightPos);
				ZombieShadowEffect.matViewProj.SetValue(lightViewProj);
				graphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				graphicsDevice.Indices = drawMeshPart.IndexBuffer;
			}
		}
	}

	public void DrawShadow(PlayerBase playerRef, int qIndex)
	{
		if (!Render[qIndex])
		{
			return;
		}
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		vecDrawVec.Y = 0f;
		vecDrawVec.X = 0f - playerRef.vecHeadPosition[qIndex].X;
		vecDrawVec.Z = 0f - playerRef.vecHeadPosition[qIndex].Z;
		tmpMatDrawMatrix = WorldTransform[qIndex];
		tmpMatDrawMatrix.Translation += vecDrawVec;
		for (int i = 0; i < ZombieShadow.Meshes.Count; i++)
		{
			ModelMesh modelMesh = ZombieShadow.Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				drawMeshPart = modelMesh.MeshParts[j];
				ZombieShadowEffect.matSkinnedWorldTransform.SetValue(tmpMatDrawMatrix);
				Vector2 zero = Vector2.Zero;
				zero.X = (int)BotHordeRef.FrameIndex[qIndex];
				zero.Y = BotHordeRef.FrameIndex[qIndex] % 1f;
				ZombieShadowEffect.currentFrame.SetValue(zero);
				drawMeshPart.Effect.CurrentTechnique.Passes[10].Apply();
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public void DrawWeapon(int qIndex)
	{
		if (Weapon != null && Render[qIndex])
		{
			_ = EndGameEngine.GraphicMgr.GraphicsDevice;
			Matrix transform = Matrix.Identity;
			Vector3 direction = Direction;
			direction.Normalize();
			transform.Forward = direction;
			transform.Up = Vector3.UnitY;
			transform.Right = Vector3.Cross(transform.Forward, transform.Up);
			transform *= Matrix.CreateFromAxisAngle(transform.Right, MathHelper.ToRadians(-90f));
			transform *= Matrix.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(180f));
			Weapon.WeaponPosition = Position + new Vector3(0f, 90f, 0f) + transform.Down * 20f + transform.Right * -8f;
			transform.Translation = Weapon.WeaponPosition;
			Weapon.Draw(qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], ref transform, Vector2.Zero);
		}
	}

	public override int RayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, WeaponClass weapon)
	{
		return RayCast(qIndex, ref origin, ref direction, ref WorldTransform[qIndex], weapon);
	}

	public int RayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, ref Matrix worldTran, WeaponClass weapon)
	{
		tmpOrigin = worldTran.Translation;
		tmpOrigin.Y += 60f;
		if (!math.TestRaySphere(ref origin, ref direction, ref tmpOrigin, 120f))
		{
			return 0;
		}
		AnimPlayer.UpdateJustBoneTransforms(ref worldTran, qIndex, applyToModelTransforms: false);
		AnimPlayer.UpdateTopAnim(ref worldTran, qIndex);
		int num = basePhysics.RayCast(ref origin, ref direction, ref RayCastHitPosition, ref worldTran, AnimPlayer.GetSkinTransforms(qIndex), (weapon.WepType == WeaponType.Shotgun) ? 2f : 1f);
		if (num > 0)
		{
			BodyPartDamage = BotPhysics.LastHitBodyPart;
		}
		return num;
	}

	public void SetRagdoll(int qIndex)
	{
		if (mRagdoll.IsValid)
		{
			mRagdoll.Update();
			tmpModelSphere.Center = mRagdoll.RagdollSkinPose[0].Translation;
			tmpModelSphere.Center.X -= LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].X;
			tmpModelSphere.Center.Z -= LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].Z;
			ContainmentType containmentType = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].bFrustum[qIndex].Contains(tmpModelSphere);
			if (containmentType == ContainmentType.Contains || containmentType == ContainmentType.Intersects)
			{
				mRagdoll.Render[qIndex] = true;
			}
			else
			{
				mRagdoll.Render[qIndex] = false;
			}
		}
		if (mRagdoll.SetRagdoll)
		{
			Health = 0;
			Matrix rootTransform = WorldTransform[qIndex];
			rootTransform.Translation += CoOpOffset;
			AnimPlayer.UpdateJustBoneTransforms(ref rootTransform, qIndex, applyToModelTransforms: true);
			mRagdoll.ResetSkinData(baseModel[ModelIndex], ModelIndex);
			mRagdoll.Spawn(rootTransform, AnimPlayer.GetBoneTransforms(qIndex), AnimPlayer.GetBoneTransforms(qIndex));
			if (BodyPartDamage == 10)
			{
				mRagdoll.DamageType = DamegePacketType.HeadShot;
			}
			else if (BodyPartDamage >= 0 && BodyPartDamage <= 4)
			{
				mRagdoll.DamageType = DamegePacketType.Legs;
			}
			else if (BodyPartDamage >= 5)
			{
				mRagdoll.DamageType = DamegePacketType.Body;
			}
			else if (BodyPartDamage == -1)
			{
				mRagdoll.DamageType = DamegePacketType.Grenade;
			}
		}
	}

	public void DrawRagdoll(int qIndex, PlayerBase viewer)
	{
		if (!mRagdoll.Render[qIndex])
		{
			return;
		}
		int currentCharacterIndex = mRagdoll.currentCharacterIndex;
		drawtmpMatrix = Matrix.Identity;
		if (PlayerBase.ApocalypseZ_Hack)
		{
			vecDrawVec.Y = 0f;
			vecDrawVec.X = 0f - viewer.vecHeadPosition[qIndex].X;
			vecDrawVec.Z = 0f - viewer.vecHeadPosition[qIndex].Z;
			drawtmpMatrix.Translation = vecDrawVec;
		}
		for (int i = 0; i < mRagdoll.currentCharacter.Meshes.Count; i++)
		{
			ModelMesh modelMesh = mRagdoll.currentCharacter.Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				drawMeshPart = modelMesh.MeshParts[j];
				baseModelEffects[currentCharacterIndex].matBones.SetValue(mRagdoll.RagdollSkinPose);
				baseModelEffects[currentCharacterIndex].matSkinnedWorldTransform.SetValue(drawtmpMatrix);
				baseModelEffects[currentCharacterIndex].matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				drawMeshPart.Effect.CurrentTechnique.Passes[2].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public override void KillZombie(ref Vector3 direction)
	{
		Health = 0;
		mRagdoll.SetRagdoll = true;
		mRagdoll.DamageType = DamegePacketType.Body;
		mRagdoll.DamageDirection = direction * 1000f;
		AIStateMachine.SetAttackPlayerEnable(e: true);
	}

	public void NetworkKillZombie(ref Vector3 direction)
	{
		Health = 0;
		mRagdoll.SetRagdoll = true;
		mRagdoll.DamageType = DamegePacketType.Body;
		mRagdoll.DamageDirection = direction * 1000f;
		AIStateMachine.SetAttackPlayerEnable(e: true);
	}

	public new void Reset()
	{
		Health = 0;
		mRagdoll.SetRagdoll = false;
		mRagdoll.IsValid = false;
		if (Weapon != null)
		{
			Weapon.Reset();
		}
	}
}
