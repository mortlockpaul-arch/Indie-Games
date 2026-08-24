using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine;

public class InstancedModel
{
	private GraphicsDevice device;

	private DepthStencilState ds;

	private Model model;

	private Matrix[] modelBoneTransforms;

	private DynamicVertexBuffer vertexBuffer;

	public Vector4 AmbientLightColor;

	public Vector4 SpecularLightColor;

	public Vector3 LightPosition;

	public Vector3 DiffuseLightColor;

	public float SpecularPower;

	public float SpecularIntensity;

	public InstancedModel(Model model)
	{
		device = EngineManager.GetGraphicsDevice;
		ds = new DepthStencilState();
		this.model = model;
		modelBoneTransforms = new Matrix[model.Bones.Count];
		model.CopyAbsoluteBoneTransformsTo(modelBoneTransforms);
		AmbientLightColor = new Vector4(0.1f, 0.1f, 0.2f, 1f);
		SpecularLightColor = Vector4.One;
		LightPosition = new Vector3(20f, 20f, 20f);
		DiffuseLightColor = Color.White.ToVector3();
		SpecularPower = 1f;
		SpecularIntensity = 10f;
	}

	public void DrawInstances(DrawTechnique technique, ref VertexColorInstanceWorld[] transforms, Camera camera)
	{
		if (transforms.Length == 0)
		{
			return;
		}
		device.DepthStencilState = ds;
		if (vertexBuffer == null || transforms.Length > vertexBuffer.VertexCount)
		{
			if (vertexBuffer != null)
			{
				vertexBuffer.Dispose();
			}
			vertexBuffer = new DynamicVertexBuffer(device, VertexColorInstanceWorld.VertexDeclaration, transforms.Length, BufferUsage.WriteOnly);
		}
		vertexBuffer.SetData(transforms, 0, transforms.Length, SetDataOptions.Discard);
		foreach (ModelMesh mesh in model.Meshes)
		{
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				device.SetVertexBuffers(new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0), new VertexBufferBinding(vertexBuffer, 0, 1));
				device.Indices = meshPart.IndexBuffer;
				Effect effect = meshPart.Effect;
				effect.CurrentTechnique = effect.Techniques[technique.ToString()];
				effect.Parameters["World"].SetValue(modelBoneTransforms[mesh.ParentBone.Index]);
				effect.Parameters["View"].SetValue(camera.View);
				effect.Parameters["Projection"].SetValue(camera.Projection);
				effect.Parameters["DiffuseLightColor"].SetValue(DiffuseLightColor);
				if (technique == DrawTechnique.PhongShading)
				{
					effect.Parameters["CameraPosition"].SetValue(camera.Position);
					effect.Parameters["LightPosition"].SetValue(LightPosition);
					effect.Parameters["AmbientLightColor"].SetValue(AmbientLightColor);
					effect.Parameters["SpecularLightColor"].SetValue(SpecularLightColor);
					effect.Parameters["SpecularPower"].SetValue(SpecularPower);
					effect.Parameters["SpecularIntensity"].SetValue(SpecularIntensity);
				}
				foreach (EffectPass pass in effect.CurrentTechnique.Passes)
				{
					pass.Apply();
					device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount, transforms.Length);
				}
			}
		}
		device.Indices = null;
		device.SetVertexBuffers((VertexBufferBinding[])null);
	}
}
