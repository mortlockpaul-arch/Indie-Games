using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class FPSScopeSightsBase
{
	public bool FoldSightsDown;

	public Matrix[] matRearSightTransform = new Matrix[2];

	public Matrix[] matFrontSightTransform = new Matrix[2];

	public Matrix[] matScopeTransform = new Matrix[2];

	public Vector3[] vecFPSLightColor = new Vector3[2];

	public Vector4[] vecFPSLightPosition = new Vector4[2];

	public static Model scope;

	private static Matrix[] scopeTransforms;

	public static Model sights;

	private static Matrix[] sightsTransforms;

	public static Model eotechSight;

	private static Matrix[] eotechSightTransforms;

	public static Model reddotSight;

	private static Matrix[] reddotSightTransforms;

	public static string[] WeaponAttachmentName = new string[6] { "Empty", "Iron Sights", "Sniper Scope", "RedDot Sight", "Holographic Sight", "Nade Luancher" };

	private static bool IsInitialized = false;

	private static Matrix updateMatWorld = Matrix.Identity;

	private static ModelMesh drawMesh;

	private static ModelMeshPart drawMeshPart;

	private static Vector4 lightColor = new Vector4(1f, 1f, 1f, 1f);

	private static Vector4 ambientColor = new Vector4(0.6f, 0.6f, 0.64f, 1f);

	private static Matrix tmpSight = Matrix.Identity;

	private static Matrix drawRearSight = Matrix.CreateRotationX(MathHelper.ToRadians(90f));

	private static Matrix drawFrontSight = Matrix.CreateRotationX(MathHelper.ToRadians(-90f));

	private static Effect drawEffect;

	private static Effect drawPartEffect;

	public Matrix GetScopeCrossHairsTransform => WeaponClass.GetBoneTransform(scope, scopeTransforms, WeaponPart.CrossHairs);

	public virtual void LoadContent()
	{
		if (IsInitialized)
		{
			return;
		}
		IsInitialized = true;
		if (!(EndGameEngine.GameSettings.GameName != "_AvR_") || !(EndGameEngine.GameSettings.GameName != "ToyPlane"))
		{
			return;
		}
		scope = EndGameEngine.GameAssetMgr.Load<Model>("models\\weapons\\m107scope");
		scopeTransforms = new Matrix[scope.Bones.Count];
		scope.CopyAbsoluteBoneTransformsTo(scopeTransforms);
		int num = 0;
		foreach (ModelMesh mesh in scope.Meshes)
		{
			mesh.Tag = FPSWeaponBase.SetWeaponPart(mesh.Name, num++);
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				meshPart.Tag = new WeaponEffectParams(meshPart.Effect, null);
			}
		}
		sights = EndGameEngine.GameAssetMgr.Load<Model>("models\\weapons\\sights");
		sightsTransforms = new Matrix[sights.Bones.Count];
		sights.CopyAbsoluteBoneTransformsTo(sightsTransforms);
		num = 0;
		foreach (ModelMesh mesh2 in sights.Meshes)
		{
			mesh2.Tag = FPSWeaponBase.SetWeaponPart(mesh2.Name, num++);
			foreach (ModelMeshPart meshPart2 in mesh2.MeshParts)
			{
				meshPart2.Tag = new WeaponEffectParams(meshPart2.Effect, null);
			}
		}
		eotechSight = EndGameEngine.GameAssetMgr.Load<Model>("models\\weapons\\eotechsight");
		eotechSightTransforms = new Matrix[eotechSight.Bones.Count];
		eotechSight.CopyAbsoluteBoneTransformsTo(eotechSightTransforms);
		num = 0;
		foreach (ModelMesh mesh3 in eotechSight.Meshes)
		{
			mesh3.Tag = FPSWeaponBase.SetWeaponPart(mesh3.Name, num++);
			foreach (ModelMeshPart meshPart3 in mesh3.MeshParts)
			{
				meshPart3.Tag = new WeaponEffectParams(meshPart3.Effect, null);
			}
		}
		reddotSight = EndGameEngine.GameAssetMgr.Load<Model>("models\\weapons\\reddotsight");
		reddotSightTransforms = new Matrix[reddotSight.Bones.Count];
		reddotSight.CopyAbsoluteBoneTransformsTo(reddotSightTransforms);
		num = 0;
		foreach (ModelMesh mesh4 in reddotSight.Meshes)
		{
			mesh4.Tag = FPSWeaponBase.SetWeaponPart(mesh4.Name, num++);
			foreach (ModelMeshPart meshPart4 in mesh4.MeshParts)
			{
				meshPart4.Tag = new WeaponEffectParams(meshPart4.Effect, null);
			}
		}
	}

	public virtual void Update(int qIndex, FPSWeaponBase owner)
	{
		updateMatWorld = owner.CurrentWeapon.GetBoneTransform(WeaponPart.RearSight);
		math.RemoveScaling(ref updateMatWorld);
		ref Matrix reference = ref matRearSightTransform[qIndex];
		reference = updateMatWorld * owner.matWeaponTransform[qIndex];
		updateMatWorld = owner.CurrentWeapon.GetBoneTransform(WeaponPart.FrontSight);
		math.RemoveScaling(ref updateMatWorld);
		ref Matrix reference2 = ref matFrontSightTransform[qIndex];
		reference2 = updateMatWorld * owner.matWeaponTransform[qIndex];
		updateMatWorld = owner.CurrentWeapon.GetBoneTransform(WeaponPart.Scope);
		if (owner.CurrentWeapon.WepType != WeaponType.FiftyCal)
		{
			math.RemoveScaling(ref updateMatWorld);
		}
		ref Matrix reference3 = ref matScopeTransform[qIndex];
		reference3 = updateMatWorld * owner.matWeaponTransform[qIndex];
	}

	public virtual void Draw(int qIndex, FPSWeaponBase owner, ref Matrix view, ref Matrix projection, ref Matrix texProjection, bool isMenu)
	{
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		_ = EndGameEngine.MaterialEffect;
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		Vector3 vector = Vector3.Zero;
		if (isMenu)
		{
			vector = new Vector3(-2000f, 2000f, 5000f);
		}
		else
		{
			vector.X = LevelOutside.SunPosition.X;
			vector.Y = LevelOutside.SunPosition.Y;
			vector.Z = LevelOutside.SunPosition.Z;
		}
		materialParams.vecLightPosition.SetValue(vector);
		materialParams.fSpecularPower.SetValue(2f);
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		if (owner.CurrentWeapon.Attachment == WeaponAttachment.IronSights)
		{
			for (int i = 0; i < sights.Meshes.Count; i++)
			{
				drawMesh = sights.Meshes[i];
				if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.FrontSight && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.FS_Pivot && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.RearSight && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.RS_Pivot)
				{
					continue;
				}
				for (int j = 0; j < drawMesh.MeshParts.Count; j++)
				{
					drawMeshPart = drawMesh.MeshParts[j];
					drawEffect = drawMeshPart.Effect;
					drawEffect.GraphicsDevice.BlendState = BlendState.Opaque;
					drawEffect.GraphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
					drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
					Vector3 value = Vector3.Transform(-view.Translation, Matrix.Transpose(view));
					drawEffect.Parameters["vecEyePosition"].SetValue(value);
					((WeaponEffectParams)drawMeshPart.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
					((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
					((WeaponEffectParams)drawMeshPart.Tag).vecLightColor.SetValue(lightColor);
					((WeaponEffectParams)drawMeshPart.Tag).vecAmbientLightColor.SetValue(ambientColor);
					((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(texProjection);
					((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(view);
					((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(projection);
					if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.FrontSight || ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.FS_Pivot)
					{
						if (FoldSightsDown && ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.FS_Pivot)
						{
							tmpSight = sightsTransforms[drawMesh.ParentBone.Index];
							tmpSight *= drawFrontSight;
							tmpSight.Translation = sightsTransforms[drawMesh.ParentBone.Index].Translation;
							((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpSight * matFrontSightTransform[qIndex]);
						}
						else
						{
							((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(sightsTransforms[drawMesh.ParentBone.Index] * matFrontSightTransform[qIndex]);
						}
					}
					else if (FoldSightsDown && ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.RS_Pivot)
					{
						tmpSight = sightsTransforms[drawMesh.ParentBone.Index];
						tmpSight *= drawRearSight;
						tmpSight.Translation = sightsTransforms[drawMesh.ParentBone.Index].Translation;
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpSight * matRearSightTransform[qIndex]);
					}
					else
					{
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(sightsTransforms[drawMesh.ParentBone.Index] * matRearSightTransform[qIndex]);
					}
					if (isMenu)
					{
						drawEffect.CurrentTechnique.Passes[9].Apply();
					}
					else
					{
						drawEffect.CurrentTechnique.Passes[10].Apply();
					}
					drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
		else if (owner.CurrentWeapon.Attachment == WeaponAttachment.HoloGraphicSight)
		{
			for (int k = 0; k < eotechSight.Meshes.Count; k++)
			{
				drawMesh = eotechSight.Meshes[k];
				if (isMenu)
				{
					if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Lens && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.RedDot)
					{
						DrawMeshPart(drawMesh.MeshParts[0], vector, 9, drawMesh.ParentBone.Index, view, projection, eotechSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					}
				}
				else if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Lens)
				{
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
					EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthNoWrite;
					DrawMeshPart(drawMesh.MeshParts[0], vector, 5, drawMesh.ParentBone.Index, view, projection, eotechSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
				}
				else if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.RedDot)
				{
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = EndGameEngine.BlendPreAlphaNoWriteAlpha;
					DrawMeshPart(drawMesh.MeshParts[0], vector, 6, drawMesh.ParentBone.Index, view, projection, eotechSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
					DrawMeshPart(drawMesh.MeshParts[0], vector, 7, drawMesh.ParentBone.Index, view, projection, eotechSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
				}
				else
				{
					DrawMeshPart(drawMesh.MeshParts[0], vector, 0, drawMesh.ParentBone.Index, view, projection, eotechSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
				}
			}
		}
		else if (owner.CurrentWeapon.Attachment == WeaponAttachment.RedDotSight)
		{
			for (int l = 0; l < reddotSight.Meshes.Count; l++)
			{
				drawMesh = reddotSight.Meshes[l];
				if (isMenu)
				{
					if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Lens && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.RedDot)
					{
						DrawMeshPart(drawMesh.MeshParts[0], vector, 9, drawMesh.ParentBone.Index, view, projection, reddotSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					}
				}
				else if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Lens)
				{
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
					EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthNoWrite;
					DrawMeshPart(drawMesh.MeshParts[0], vector, 5, drawMesh.ParentBone.Index, view, projection, reddotSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
				}
				else if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.RedDot)
				{
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
					DrawMeshPart(drawMesh.MeshParts[0], vector, 11, drawMesh.ParentBone.Index, view, projection, reddotSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
				}
				else
				{
					DrawMeshPart(drawMesh.MeshParts[0], vector, 0, drawMesh.ParentBone.Index, view, projection, reddotSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
				}
			}
		}
		else
		{
			if (owner.CurrentWeapon.Attachment != WeaponAttachment.SniperScope)
			{
				return;
			}
			if (owner.WeaponZoom < 0.76624215f)
			{
				for (int m = 0; m < scope.Meshes.Count; m++)
				{
					drawMesh = scope.Meshes[m];
					if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Magnify)
					{
						EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
						DrawMeshPart(drawMesh.MeshParts[0], vector, 1, drawMesh.ParentBone.Index, view, projection, scopeTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
						EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
					}
				}
				for (int n = 0; n < scope.Meshes.Count; n++)
				{
					drawMesh = scope.Meshes[n];
					if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Scope)
					{
						DrawMeshPart(drawMesh.MeshParts[0], vector, 0, drawMesh.ParentBone.Index, view, projection, scopeTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					}
				}
				return;
			}
			if (isMenu)
			{
				for (int num = 0; num < scope.Meshes.Count; num++)
				{
					drawMesh = scope.Meshes[num];
					if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Scope)
					{
						DrawMeshPart(drawMesh.MeshParts[0], vector, 9, drawMesh.ParentBone.Index, view, projection, scopeTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					}
				}
				return;
			}
			for (int num2 = 0; num2 < scope.Meshes.Count; num2++)
			{
				drawMesh = scope.Meshes[num2];
				if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Scope)
				{
					DrawMeshPart(drawMesh.MeshParts[0], vector, 0, drawMesh.ParentBone.Index, view, projection, scopeTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
				}
			}
			for (int num3 = 0; num3 < scope.Meshes.Count; num3++)
			{
				drawMesh = scope.Meshes[num3];
				if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Lens)
				{
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
					DrawMeshPart(drawMesh.MeshParts[0], vector, 4, drawMesh.ParentBone.Index, view, projection, scopeTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
					EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
				}
			}
		}
	}

	public virtual void DrawDepth(int qIndex, FPSWeaponBase owner, ref Matrix view, ref Matrix projection)
	{
		_ = EndGameEngine.GraphicMgr.GraphicsDevice;
		if (owner.CurrentWeapon.Attachment == WeaponAttachment.IronSights)
		{
			for (int i = 0; i < sights.Meshes.Count; i++)
			{
				drawMesh = sights.Meshes[i];
				if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.FrontSight && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.FS_Pivot && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.RearSight && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.RS_Pivot)
				{
					continue;
				}
				for (int j = 0; j < drawMesh.MeshParts.Count; j++)
				{
					drawMeshPart = drawMesh.MeshParts[j];
					drawEffect = drawMeshPart.Effect;
					drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(view);
					((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(projection);
					if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.FrontSight || ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.FS_Pivot)
					{
						if (FoldSightsDown && ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.FS_Pivot)
						{
							tmpSight = sightsTransforms[drawMesh.ParentBone.Index];
							tmpSight *= drawFrontSight;
							tmpSight.Translation = sightsTransforms[drawMesh.ParentBone.Index].Translation;
							((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpSight * matFrontSightTransform[qIndex]);
						}
						else
						{
							((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(sightsTransforms[drawMesh.ParentBone.Index] * matFrontSightTransform[qIndex]);
						}
					}
					else if (FoldSightsDown && ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.RS_Pivot)
					{
						tmpSight = sightsTransforms[drawMesh.ParentBone.Index];
						tmpSight *= drawRearSight;
						tmpSight.Translation = sightsTransforms[drawMesh.ParentBone.Index].Translation;
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpSight * matRearSightTransform[qIndex]);
					}
					else
					{
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(sightsTransforms[drawMesh.ParentBone.Index] * matRearSightTransform[qIndex]);
					}
					drawEffect.CurrentTechnique.Passes[8].Apply();
					drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
		else if (owner.CurrentWeapon.Attachment == WeaponAttachment.HoloGraphicSight)
		{
			for (int k = 0; k < eotechSight.Meshes.Count; k++)
			{
				drawMesh = eotechSight.Meshes[k];
				if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Lens && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.RedDot)
				{
					drawMeshPart = drawMesh.MeshParts[0];
					drawPartEffect = drawMeshPart.Effect;
					drawPartEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawPartEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(view);
					((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(projection);
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(eotechSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex]);
					drawPartEffect.CurrentTechnique.Passes[8].Apply();
					drawPartEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
		else if (owner.CurrentWeapon.Attachment == WeaponAttachment.RedDotSight)
		{
			for (int l = 0; l < reddotSight.Meshes.Count; l++)
			{
				drawMesh = reddotSight.Meshes[l];
				if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Lens && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.RedDot)
				{
					drawMeshPart = drawMesh.MeshParts[0];
					drawPartEffect = drawMeshPart.Effect;
					drawPartEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawPartEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(view);
					((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(projection);
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(reddotSightTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex]);
					drawPartEffect.CurrentTechnique.Passes[8].Apply();
					drawPartEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
		else
		{
			if (owner.CurrentWeapon.Attachment != WeaponAttachment.SniperScope || !(owner.WeaponZoom >= 0.76624215f))
			{
				return;
			}
			for (int m = 0; m < scope.Meshes.Count; m++)
			{
				drawMesh = scope.Meshes[m];
				if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Scope)
				{
					drawMeshPart = drawMesh.MeshParts[0];
					drawPartEffect = drawMeshPart.Effect;
					drawPartEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawPartEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(view);
					((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(projection);
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(scopeTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex]);
					drawPartEffect.CurrentTechnique.Passes[8].Apply();
					drawPartEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
	}

	public void PostDrawScope(int qIndex, PlayerBase player, FPSWeaponBase owner, ref Matrix projection, ref Matrix texProjection, Texture2D scene, Texture2D bloom)
	{
		player.SetViewPortForPass(PlayerBase.RenderPass.PostScopePass, qIndex);
		Matrix view = player.mDataQueue[qIndex].view;
		Vector3 zero = Vector3.Zero;
		zero.X = LevelOutside.SunPosition.X;
		zero.Y = LevelOutside.SunPosition.Y;
		zero.Z = LevelOutside.SunPosition.Z;
		if (!(owner.WeaponZoom < 0.76624215f))
		{
			return;
		}
		for (int i = 0; i < scope.Meshes.Count; i++)
		{
			drawMesh = scope.Meshes[i];
			if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Magnify)
			{
				((WeaponEffectParams)drawMesh.MeshParts[0].Tag).texOffset.SetValue(player.UVDisplacement);
				((WeaponEffectParams)drawMesh.MeshParts[0].Tag).fpsScene.SetValue(scene);
				((WeaponEffectParams)drawMesh.MeshParts[0].Tag).fpsBloom.SetValue(bloom);
				DrawMeshPart(drawMesh.MeshParts[0], zero, 2, drawMesh.ParentBone.Index, view, projection, scopeTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
			}
		}
		for (int j = 0; j < scope.Meshes.Count; j++)
		{
			drawMesh = scope.Meshes[j];
			if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.CrossHairs)
			{
				DrawMeshPart(drawMesh.MeshParts[0], zero, 3, drawMesh.ParentBone.Index, view, projection, scopeTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
			}
		}
		for (int k = 0; k < scope.Meshes.Count; k++)
		{
			drawMesh = scope.Meshes[k];
			if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Lens)
			{
				EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
				DrawMeshPart(drawMesh.MeshParts[0], zero, 4, drawMesh.ParentBone.Index, view, projection, scopeTransforms[drawMesh.ParentBone.Index] * matScopeTransform[qIndex], ref texProjection);
				EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
			}
		}
	}

	private void DrawMeshPart(ModelMeshPart part, Vector3 lp, int pass, int bone, Matrix view, Matrix projection, Matrix transform, ref Matrix texProjection)
	{
		drawPartEffect = part.Effect;
		drawPartEffect.GraphicsDevice.SetVertexBuffer(part.VertexBuffer, part.VertexOffset);
		drawPartEffect.GraphicsDevice.Indices = part.IndexBuffer;
		((WeaponEffectParams)part.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
		((WeaponEffectParams)part.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
		((WeaponEffectParams)part.Tag).vecLightColor.SetValue(lightColor);
		((WeaponEffectParams)part.Tag).vecAmbientLightColor.SetValue(ambientColor);
		((WeaponEffectParams)part.Tag).vecLightPosition.SetValue(lp);
		((WeaponEffectParams)part.Tag).fSpecularPower.SetValue(32f);
		((WeaponEffectParams)part.Tag).fReflectiveness.SetValue(0.075f);
		((WeaponEffectParams)part.Tag).matTexProj.SetValue(texProjection);
		((WeaponEffectParams)part.Tag).matView.SetValue(view);
		((WeaponEffectParams)part.Tag).matProj.SetValue(projection);
		((WeaponEffectParams)part.Tag).matWorld.SetValue(transform);
		((WeaponEffectParams)part.Tag).vecFPSLightPos.SetValue(vecFPSLightPosition[LevelBaseMenu.DataQueueRender]);
		((WeaponEffectParams)part.Tag).vecFPSLightColor.SetValue(vecFPSLightColor[LevelBaseMenu.DataQueueRender]);
		((WeaponEffectParams)part.Tag).vecMuzzleFlash.SetValue(particles.MuzzleFlash());
		drawPartEffect.CurrentTechnique.Passes[pass].Apply();
		drawPartEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
	}
}
