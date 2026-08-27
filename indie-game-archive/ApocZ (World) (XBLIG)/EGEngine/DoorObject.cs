using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class DoorObject
{
	public const int numDoorTypes = 3;

	private static bool Initialized = false;

	public static string[] idString = new string[3] { "dooraobj", "doorbobj", "doorcobj" };

	private static Model[] renderGeometry = new Model[3];

	public int idIndex;

	public Matrix worldTransform;

	public DoorObject()
	{
		if (Initialized)
		{
			return;
		}
		Initialized = true;
		for (int i = 0; i < 3; i++)
		{
			renderGeometry[i] = EndGameEngine.GameAssetMgr.Load<Model>("models\\props\\" + idString[i]);
			Matrix[] array = new Matrix[renderGeometry[i].Bones.Count];
			renderGeometry[i].CopyAbsoluteBoneTransformsTo(array);
			foreach (ModelMesh mesh in renderGeometry[i].Meshes)
			{
				CustomContent customContent = mesh.Tag as CustomContent;
				customContent.transform = array[mesh.ParentBone.Index];
				TextureBase.GetMaterialsTextureByName(EndGameEngine.GameAssetMgr, "props\\" + customContent.textureName, out customContent.DiffuseMap, out customContent.NormalMap);
			}
		}
	}

	public void Set(int index, Matrix transform)
	{
		idIndex = index;
		Matrix matrix = transform;
		Vector4 vector = new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14);
		Vector4 vector2 = new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24);
		Vector4 vector3 = new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34);
		vector.Normalize();
		vector2.Normalize();
		vector3.Normalize();
		matrix.M11 = vector.X;
		matrix.M12 = vector.Y;
		matrix.M13 = vector.Z;
		matrix.M14 = vector.W;
		matrix.M21 = vector2.X;
		matrix.M22 = vector2.Y;
		matrix.M23 = vector2.Z;
		matrix.M24 = vector2.W;
		matrix.M31 = vector3.X;
		matrix.M32 = vector3.Y;
		matrix.M33 = vector3.Z;
		matrix.M34 = vector3.W;
		worldTransform = matrix;
	}

	public void Draw(int qIndex, RenderPass pass)
	{
		Effect materialEffect = EndGameEngine.MaterialEffect;
		GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		DepthStencilState depthStencilState = new DepthStencilState();
		depthStencilState.DepthBufferEnable = true;
		depthStencilState.DepthBufferWriteEnable = true;
		graphicsDevice.DepthStencilState = depthStencilState;
		new SamplerState();
		RasterizerState rasterizerState = new RasterizerState();
		rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
		rasterizerState.FillMode = FillMode.Solid;
		graphicsDevice.RasterizerState = rasterizerState;
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		materialEffect.CurrentTechnique = materialParams.T_WorldObject;
		foreach (ModelMesh mesh in renderGeometry[idIndex].Meshes)
		{
			CustomContent customContent = mesh.Tag as CustomContent;
			Matrix value = customContent.transform * worldTransform;
			materialParams.matWorld.SetValue(value);
			graphicsDevice.SetVertexBuffer(mesh.MeshParts[0].VertexBuffer, mesh.MeshParts[0].VertexOffset);
			graphicsDevice.Indices = mesh.MeshParts[0].IndexBuffer;
			materialParams.propDiffuse1.SetValue(customContent.DiffuseMap);
			materialParams.propNormal1.SetValue(customContent.NormalMap);
			materialEffect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.MeshParts[0].NumVertices, mesh.MeshParts[0].StartIndex, mesh.MeshParts[0].PrimitiveCount);
		}
	}
}
