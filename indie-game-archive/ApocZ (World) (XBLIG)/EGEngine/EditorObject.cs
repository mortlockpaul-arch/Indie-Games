using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class EditorObject
{
	public int uniqueId;

	public float scale;

	public Model model;

	public Matrix[] worldTransform;

	public Matrix[] worldScale;

	public Texture2D texture;

	public List<EditorObject> objList = new List<EditorObject>();

	private static string[] gimbalNames = new string[10] { "xAxis", "yAxis", "zAxis", "xRotate", "yRotate", "zRotate", "xScale", "yScale", "zScale", "ScaleAll" };

	public EditorObject()
	{
	}

	public EditorObject(ContentManager cntMgr, int id, string modelName, string textureName)
	{
		uniqueId = id;
		worldTransform = new Matrix[2];
		worldScale = new Matrix[2];
		model = cntMgr.Load<Model>(modelName);
		TextureBase.GetTexture2DByName(cntMgr, textureName, out texture);
		SetPhysics();
		objList.Add(this);
	}

	public virtual void SetPhysics()
	{
		Matrix[] destinationBoneTransforms = new Matrix[model.Bones.Count];
		model.CopyAbsoluteBoneTransformsTo(destinationBoneTransforms);
		foreach (ModelMesh mesh in model.Meshes)
		{
			int vertexStride = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
			EditorVerticeStruct[] data = new EditorVerticeStruct[mesh.MeshParts[0].NumVertices];
			mesh.MeshParts[0].VertexBuffer.GetData(mesh.MeshParts[0].VertexOffset * vertexStride, data, 0, mesh.MeshParts[0].NumVertices, vertexStride);
		}
	}

	public virtual GimbalType RayCast(int qIndex, Vector3 pos, Vector3 dir)
	{
		return GimbalType.NumOfTypes;
	}

	public virtual void Update(int qIndex, Matrix matWorld, Vector3 cameraPos, bool Gimbal)
	{
		if (Gimbal)
		{
			scale = (cameraPos - worldTransform[qIndex].Translation).Length() / 60f;
			ref Matrix reference = ref worldScale[qIndex];
			reference = Matrix.CreateScale(scale);
			if (uniqueId == 1)
			{
				ref Matrix reference2 = ref worldTransform[qIndex];
				reference2 = Matrix.Identity;
				worldTransform[qIndex].Translation = matWorld.Translation;
				return;
			}
			worldTransform[qIndex] = matWorld;
			Vector4 vector = new Vector4(matWorld.M11, matWorld.M12, matWorld.M13, matWorld.M14);
			Vector4 vector2 = new Vector4(matWorld.M21, matWorld.M22, matWorld.M23, matWorld.M24);
			Vector4 vector3 = new Vector4(matWorld.M31, matWorld.M32, matWorld.M33, matWorld.M34);
			vector.Normalize();
			vector2.Normalize();
			vector3.Normalize();
			worldTransform[qIndex].M11 = vector.X;
			worldTransform[qIndex].M12 = vector.Y;
			worldTransform[qIndex].M13 = vector.Z;
			worldTransform[qIndex].M14 = vector.W;
			worldTransform[qIndex].M21 = vector2.X;
			worldTransform[qIndex].M22 = vector2.Y;
			worldTransform[qIndex].M23 = vector2.Z;
			worldTransform[qIndex].M24 = vector2.W;
			worldTransform[qIndex].M31 = vector3.X;
			worldTransform[qIndex].M32 = vector3.Y;
			worldTransform[qIndex].M33 = vector3.Z;
			worldTransform[qIndex].M34 = vector3.W;
		}
		else
		{
			worldTransform[qIndex] = matWorld;
			ref Matrix reference3 = ref worldScale[qIndex];
			reference3 = Matrix.CreateScale(10f);
		}
	}

	public virtual void Draw(int qIndex, bool WireFrame)
	{
		Effect materialEffect = EndGameEngine.MaterialEffect;
		GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		DepthStencilState depthStencilState = new DepthStencilState();
		depthStencilState.DepthBufferEnable = false;
		depthStencilState.DepthBufferWriteEnable = false;
		DepthStencilState depthStencilState2 = new DepthStencilState();
		depthStencilState2.DepthBufferEnable = true;
		depthStencilState2.DepthBufferWriteEnable = true;
		DepthStencilState depthStencilState3 = new DepthStencilState();
		depthStencilState3.DepthBufferEnable = true;
		depthStencilState3.DepthBufferWriteEnable = true;
		graphicsDevice.DepthStencilState = depthStencilState3;
		SamplerState samplerState = new SamplerState();
		samplerState.Filter = TextureFilter.Point;
		samplerState.AddressU = TextureAddressMode.Wrap;
		samplerState.AddressV = TextureAddressMode.Wrap;
		samplerState.AddressW = TextureAddressMode.Wrap;
		RasterizerState rasterizerState = graphicsDevice.RasterizerState;
		RasterizerState rasterizerState2 = new RasterizerState();
		rasterizerState2.CullMode = CullMode.None;
		if (WireFrame)
		{
			rasterizerState2.FillMode = FillMode.WireFrame;
		}
		else
		{
			rasterizerState2.FillMode = FillMode.Solid;
		}
		graphicsDevice.RasterizerState = rasterizerState2;
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		materialEffect.CurrentTechnique = materialParams.T_EditObject;
		materialParams.editTexture.SetValue(texture);
		graphicsDevice.RasterizerState = rasterizerState;
	}
}
