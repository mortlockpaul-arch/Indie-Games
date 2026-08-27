using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class WeaponHalographicUI : PropModelBase
{
	private static Vector4 uvOffsets = Vector4.Zero;

	public override void Update(float eTime, int qIndex)
	{
	}

	public void Draw(ref Matrix matVP, int qIndex, PlayerBase playerRef)
	{
		float num = 0.1024f;
		int bulletsInMag = playerRef.fpsWeapon.CurrentWeapon.BulletsInMag;
		int bulletsTotal = playerRef.fpsWeapon.CurrentWeapon.BulletsTotal;
		float num2 = bulletsInMag / 10;
		float num3 = bulletsInMag % 10;
		float num4 = bulletsTotal / 100;
		float num5 = (bulletsTotal - (int)num4 * 100) / 10;
		float num6 = (bulletsTotal - (int)num4 * 100) % 10;
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			PropModelBase.drawMesh = propModel.Meshes[i];
			for (int j = 0; j < PropModelBase.drawMesh.MeshParts.Count; j++)
			{
				PropModelBase.drawMeshPart = PropModelBase.drawMesh.MeshParts[j];
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(PropModelBase.drawMeshPart.VertexBuffer, PropModelBase.drawMeshPart.VertexOffset);
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.Indices = PropModelBase.drawMeshPart.IndexBuffer;
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).eyePosition.SetValue(playerRef.mDataQueue[qIndex].eyePosition);
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).matWorld.SetValue(propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]);
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).matViewProj.SetValue(matVP);
				uvOffsets.X = 0f;
				uvOffsets.Y = 0f;
				uvOffsets.Z = (playerRef.Sighted ? 0.5f : 1f);
				uvOffsets.W = 1f;
				switch (i)
				{
				case 2:
					uvOffsets.Y = num2 * num;
					break;
				case 1:
					uvOffsets.Y = num3 * num;
					break;
				case 5:
					uvOffsets.Y = num4 * num;
					break;
				case 3:
					uvOffsets.Y = num5 * num;
					break;
				case 4:
					uvOffsets.Y = num6 * num;
					break;
				}
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).vecUVOffset.SetValue(uvOffsets);
				PropModelBase.drawMeshPart.Effect.CurrentTechnique.Passes[12].Apply();
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, PropModelBase.drawMeshPart.NumVertices, PropModelBase.drawMeshPart.StartIndex, PropModelBase.drawMeshPart.PrimitiveCount);
			}
		}
	}
}
