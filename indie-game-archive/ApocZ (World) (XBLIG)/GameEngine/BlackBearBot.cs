using System;
using DataContent;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine;

public class BlackBearBot : BaseData
{
	public new const int MaxBaseModels = 1;

	public bool InUse;

	public new int ModelIndex;

	public float TransitionTime;

	public Matrix[] WorldTransform = new Matrix[2];

	public new Ragdoll mRagdoll = new Ragdoll();

	public new bool[] Render = new bool[2];

	public int BodyPartDamage;

	public static BotPhysics basePhysics;

	public static Model[] baseModel = new Model[1];

	public static SkinnedInstanceEffectParams[] baseModelEffects = new SkinnedInstanceEffectParams[1];

	private static string[] baseModelNames = new string[1] { "BlackBear" };

	private new static Random RandGenerator = new Random();

	private static Animation tmpPlayer = new Animation();

	private static bool IsInitialized = false;

	private static int animCounter = 0;

	private static BoundingSphere tmpSphere = default(BoundingSphere);

	private static BoundingSphere tmpModelSphere = new BoundingSphere(Vector3.Zero, 120f);

	private static Vector4 animBoneFrame = Vector4.Zero;

	private static ModelMesh drawMesh = null;

	private static ModelMeshPart drawMeshPart = null;

	private static Matrix matScale = Matrix.CreateScale(0.7f);

	public static Vector3 RayCastHitPosition = Vector3.Zero;

	private static Vector3 tmpOrigin = Vector3.Zero;

	private static Vector3 tmpDirection = Vector3.UnitX;

	private static Matrix tmpInverse = Matrix.Identity;

	private static Matrix tmpInverseRotate = Matrix.Identity;

	private static Vector3 CoOpOffset = new Vector3(0f, 0f, 0f);

	private static Matrix drawtmpMatrix = Matrix.Identity;

	public BlackBearBot(bool useWeapon)
		: base(useWeapon)
	{
		InUse = false;
		ModelIndex = 0;
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
			for (int j = 0; j < 1; j++)
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
		}
		AIStateMachine.allStates[6] = new ZombieHuntPlayer(AIBotStates.ZombieHuntPlayer);
		AnimPlayer.Initialize(baseModel[ModelIndex], 0);
		AnimPlayer.SetBaseAnimation(WeaponAnim.ZombieKeepWalk);
		AnimPlayer.Update(elapsedGameTime, ref transform, 0, 1f);
		if (EndGameEngine.GameSettings.GameName.Contains("Tower Defense"))
		{
			CurrentAnimation = WeaponAnim.ZombieKeepWalk;
		}
		else
		{
			CurrentAnimation = WeaponAnim.ZombieKeepWalk;
		}
		AnimTexture = base.CurrentAnimationState.AnimationTexture;
		for (int m = 0; m < baseModel[ModelIndex].Meshes.Count; m++)
		{
			ModelMesh modelMesh2 = baseModel[ModelIndex].Meshes[m];
			for (int n = 0; n < modelMesh2.MeshParts.Count; n++)
			{
				_ = modelMesh2.MeshParts[n];
			}
		}
		mRagdoll.SetSkinData(baseModel[ModelIndex], 1f);
		BotState = AIStateMachine.allStates[21];
		BotState.CurrentState(this, 0);
	}

	public new void Update(float etime, int qIndex, bool getCollision)
	{
		if (Health > 0)
		{
			GetCollision = getCollision;
			BotState.CurrentState(this, qIndex);
			ref Matrix reference = ref WorldTransform[qIndex];
			reference = Matrix.Identity;
			WorldTransform[qIndex].Forward = Direction * -1f;
			WorldTransform[qIndex].Right = Vector3.Cross(Direction * -1f, Vector3.UnitY);
			WorldTransform[qIndex].Up = Vector3.UnitY;
			ref Matrix reference2 = ref WorldTransform[qIndex];
			reference2 = WorldTransform[qIndex] * matScale;
			WorldTransform[qIndex].Translation = Position;
			tmpModelSphere.Center = Position;
			Matrix transform = Matrix.Identity;
			AnimPlayer.Update(EndGameEngine.currentEleapsedTime.ElapsedGameTime, ref transform, qIndex, 1f);
			ContainmentType containmentType = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].bFrustum[qIndex].Contains(tmpModelSphere);
			if (containmentType == ContainmentType.Contains || containmentType == ContainmentType.Intersects)
			{
				Render[qIndex] = true;
				InFrustum = true;
			}
			else
			{
				Render[qIndex] = false;
				InFrustum = false;
			}
			Render[qIndex] = true;
			InFrustum = true;
		}
		else
		{
			Render[qIndex] = false;
			InFrustum = false;
		}
		SetRagdoll(qIndex);
	}

	public void PreDraw(int qIndex, int mIndex)
	{
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		Vector3 value = Vector3.Transform(-playerBase.mDataQueue[qIndex].view.Translation, Matrix.Transpose(playerBase.mDataQueue[qIndex].view));
		for (int i = 0; i < baseModel[mIndex].Meshes.Count; i++)
		{
			ModelMesh modelMesh = baseModel[mIndex].Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				drawMeshPart = modelMesh.MeshParts[j];
				baseModelEffects[ModelIndex].AnimationTextureMap.SetValue(AnimTexture);
				animBoneFrame.X = 1f / (float)AnimTexture.Width;
				animBoneFrame.Y = 1f / (float)AnimTexture.Height;
				animBoneFrame.Z = animBoneFrame.X / 2f;
				animBoneFrame.W = animBoneFrame.Y / 2f;
				baseModelEffects[ModelIndex].animationBoneFrame.SetValue(animBoneFrame);
				baseModelEffects[ModelIndex].vecEyePosition.SetValue(value);
				baseModelEffects[ModelIndex].matViewProj.SetValue(playerBase.mDataQueue[qIndex].view * playerBase.mDataQueue[qIndex].projection);
				baseModelEffects[ModelIndex].TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
				baseModelEffects[ModelIndex].matTexProj.SetValue(playerBase.mDataQueue[qIndex].lightView * playerBase.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj);
				graphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				graphicsDevice.Indices = drawMeshPart.IndexBuffer;
			}
		}
	}

	public void Draw(int qIndex, PlayerBase viewer)
	{
		if (!Render[qIndex])
		{
			return;
		}
		drawtmpMatrix = Matrix.Identity;
		Vector3 position = Vector3.Zero;
		Vector3 normal = Vector3.Zero;
		position.X = 88480f;
		position.Z = 214200f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal);
		position.X -= viewer.vecHeadPosition[qIndex].X;
		position.Z -= viewer.vecHeadPosition[qIndex].Z;
		drawtmpMatrix.Translation = position;
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		for (int i = 0; i < baseModel[ModelIndex].Meshes.Count; i++)
		{
			drawMesh = baseModel[ModelIndex].Meshes[i];
			for (int j = 0; j < drawMesh.MeshParts.Count; j++)
			{
				drawMeshPart = drawMesh.MeshParts[j];
				Vector3 value = Vector3.Transform(-viewer.mDataQueue[qIndex].view.Translation, Matrix.Transpose(viewer.mDataQueue[qIndex].view));
				drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(value);
				baseModelEffects[ModelIndex].matBones.SetValue(AnimPlayer.GetSkinTransforms(qIndex));
				baseModelEffects[ModelIndex].matSkinnedWorldTransform.SetValue(drawtmpMatrix);
				baseModelEffects[ModelIndex].matView.SetValue(viewer.mDataQueue[qIndex].view);
				baseModelEffects[ModelIndex].matProj.SetValue(viewer.mDataQueue[qIndex].projection);
				baseModelEffects[ModelIndex].TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
				baseModelEffects[ModelIndex].matTexProj.SetValue(viewer.mDataQueue[qIndex].lightView * viewer.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj);
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				drawMeshPart.Effect.CurrentTechnique.Passes[4].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
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

	public virtual int RayCast(int qIndex, ref Vector3 origin, ref Vector3 direction)
	{
		int num = basePhysics.RayCast(ref origin, ref direction, ref RayCastHitPosition, ref WorldTransform[qIndex], AnimPlayer.GetSkinTransforms(qIndex), 1f);
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
			Matrix world = WorldTransform[qIndex];
			world.Translation += CoOpOffset;
			mRagdoll.ResetSkinData(baseModel[ModelIndex], ModelIndex);
			mRagdoll.Spawn(world, AnimPlayer.GetBoneTransforms(0), AnimPlayer.GetBoneTransforms(0));
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
		for (int i = 0; i < mRagdoll.currentCharacter.Meshes.Count; i++)
		{
			ModelMesh modelMesh = mRagdoll.currentCharacter.Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				drawMeshPart = modelMesh.MeshParts[j];
				baseModelEffects[currentCharacterIndex].matBones.SetValue(mRagdoll.RagdollSkinPose);
				baseModelEffects[currentCharacterIndex].matSkinnedWorldTransform.SetValue(drawtmpMatrix);
				baseModelEffects[currentCharacterIndex].matViewProj.SetValue(viewer.mDataQueue[qIndex].view * viewer.mDataQueue[qIndex].projection);
				baseModelEffects[currentCharacterIndex].TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
				baseModelEffects[currentCharacterIndex].matTexProj.SetValue(viewer.mDataQueue[qIndex].lightView * viewer.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj);
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				drawMeshPart.Effect.CurrentTechnique.Passes[2].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public new void KillZombie(ref Vector3 direction)
	{
		Health = 0;
		mRagdoll.SetRagdoll = true;
		mRagdoll.DamageType = DamegePacketType.Body;
		mRagdoll.DamageDirection = direction * 14000f;
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
