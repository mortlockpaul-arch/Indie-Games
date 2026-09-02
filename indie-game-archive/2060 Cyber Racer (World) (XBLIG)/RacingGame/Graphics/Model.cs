using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Helpers;
using RacingGame.Shaders;

namespace RacingGame.Graphics;

public class Model : IDisposable
{
	private string name;

	private Model xnaModel;

	private static readonly Matrix objectMatrix;

	private Matrix[] transforms;

	private float realScaling;

	private float scaling;

	private bool hasAlpha;

	private bool isCar;

	private ModelMesh animatedMesh;

	private List<EffectParameter> cachedEffectParameters;

	private List<bool> cachedIsReflectionSpecularTechnique;

	private Dictionary<ModelMeshPart, MeshRenderManager.RenderableMesh> renderableMeshes;

	private float time;

	private static int maxViewDistance;

	public string Name => name;

	public float Size => realScaling;

	public int NumOfMeshParts
	{
		get
		{
			int num = 0;
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes).Count; i++)
			{
				num += ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes)[i].MeshParts).Count;
			}
			return num;
		}
	}

	public static int MaxViewDistance
	{
		get
		{
			return maxViewDistance;
		}
		set
		{
			if (value < maxViewDistance)
			{
				maxViewDistance = value;
			}
		}
	}

	public Model(string setModelName)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		name = "";
		realScaling = 1f;
		scaling = 1f;
		cachedEffectParameters = new List<EffectParameter>();
		cachedIsReflectionSpecularTechnique = new List<bool>();
		renderableMeshes = new Dictionary<ModelMeshPart, MeshRenderManager.RenderableMesh>();
		base._002Ector();
		name = setModelName;
		xnaModel = BaseGame.Content.Load<Model>("Content\\models\\" + name);
		if (xnaModel != null)
		{
			transforms = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)xnaModel.Bones).Count];
			xnaModel.CopyAbsoluteBoneTransformsTo(transforms);
			if (((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes).Count > 0)
			{
				float radius = ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes)[0].BoundingSphere.Radius;
				Vector3 right = ((Matrix)(ref transforms[0])).Right;
				realScaling = (scaling = radius * ((Vector3)(ref right)).Length());
			}
			if (name.ToLower() == "alphapalm" || name.ToLower() == "alphapalm2" || name.ToLower() == "alphapalm3" || name.ToLower() == "roadcolumnsegment")
			{
				scaling *= 0.75f;
			}
			if (name.ToLower() == "hotel01" || name.ToLower() == "hotel02" || name.ToLower() == "casino01" || name.ToLower() == "windmill")
			{
				scaling *= 5f;
			}
			else if (scaling > 3f)
			{
				scaling = 3f;
			}
		}
		hasAlpha = name.ToLower().StartsWith("alpha");
		bool flag = name.ToLower().StartsWith("sign") || name.ToLower().StartsWith("banner") || name.ToLower().StartsWith("windmill");
		isCar = name.ToLower() == "car";
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes).Count; i++)
		{
			ModelMesh val = ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes)[i];
			int num = 0;
			string text = val.Name;
			if (name.ToLower() == "windmill" && text.ToLower().StartsWith("windmill_wings"))
			{
				animatedMesh = val;
			}
			for (int j = 0; j < ((ReadOnlyCollection<Effect>)(object)val.Effects).Count; j++)
			{
				Effect val2 = ((ReadOnlyCollection<Effect>)(object)val.Effects)[j];
				cachedEffectParameters.Add(val2.Parameters["diffuseTexture"]);
				cachedEffectParameters.Add(val2.Parameters["ambientColor"]);
				cachedEffectParameters.Add(val2.Parameters["diffuseColor"]);
				cachedEffectParameters.Add(val2.Parameters["world"]);
				cachedEffectParameters.Add(val2.Parameters["viewProj"]);
				cachedEffectParameters.Add(val2.Parameters["viewInverse"]);
				cachedEffectParameters.Add(val2.Parameters["lightDir"]);
				cachedIsReflectionSpecularTechnique.Add(val2.CurrentTechnique.Name.Contains("ReflectionSpecular"));
				if (flag)
				{
					EffectParameter obj = val2.Parameters["ambientColor"];
					Color val3 = new Color((byte)128, (byte)128, (byte)128);
					obj.SetValue(((Color)(ref val3)).ToVector4());
				}
				if (isCar && !val.Name.StartsWith("glass"))
				{
					val2.Parameters["UseAlpha"].SetValue(false);
				}
				int num2 = -1;
				if (text.Length > num)
				{
					string value = text.Substring(text.Length - (1 + num), 1);
					try
					{
						num2 = Convert.ToInt32(value);
					}
					catch
					{
					}
				}
				if (num2 < 0 || num2 >= val2.Techniques.Count)
				{
					num2 = val2.Techniques.Count - 1;
					if (val2.Techniques[num2].Name.Contains("SpecularWithReflection"))
					{
						num2 -= 2;
					}
					if (val2.Techniques[num2].Name.Contains("ReflectionSpecular"))
					{
						num2 -= 4;
					}
				}
				val2.CurrentTechnique = val2.Techniques[num2];
				num++;
			}
			for (int k = 0; k < ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts).Count; k++)
			{
				ModelMeshPart val4 = ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts)[k];
				renderableMeshes.Add(val4, BaseGame.MeshRenderManager.Add(val.VertexBuffer, val.IndexBuffer, val4, val4.Effect));
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			name = "";
			xnaModel = null;
			transforms = null;
			animatedMesh = null;
		}
	}

	public void Render(Matrix renderMatrix)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)maxViewDistance * scaling;
		float num2 = Vector3.DistanceSquared(BaseGame.CameraPos, ((Matrix)(ref renderMatrix)).Translation);
		if (num2 > num * num)
		{
			return;
		}
		if (num2 > 400f && num2 > 10f * scaling * (10f * scaling))
		{
			Vector3 vec = Vector3.Normalize(BaseGame.CameraPos - ((Matrix)(ref renderMatrix)).Translation);
			float angleBetweenVectors = Vector3Helper.GetAngleBetweenVectors(BaseGame.CameraRotation, vec);
			if (angleBetweenVectors > (float)Math.PI * 4f / 9f)
			{
				return;
			}
		}
		renderMatrix = objectMatrix * renderMatrix;
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes).Count; i++)
		{
			ModelMesh val = ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes)[i];
			Matrix item = transforms[val.ParentBone.Index] * renderMatrix;
			if (animatedMesh == val)
			{
				Vector3 translation = ((Matrix)(ref renderMatrix)).Translation;
				item = Matrix.CreateRotationZ(((Vector3)(ref translation)).Length() * 3f + ((Matrix)(ref renderMatrix)).Determinant() * 5f + (1f + (float)((int)(renderMatrix.M42 * 33.3f) % 100) * 0.00123f) * BaseGame.TotalTime / 0.654f) * transforms[val.ParentBone.Index] * renderMatrix;
			}
			for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts).Count; j++)
			{
				renderableMeshes[((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts)[j]].renderMatrices.Add(item);
			}
		}
	}

	public void Render(Vector3 renderPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Render(Matrix.CreateTranslation(renderPos));
	}

	public void RenderCar(int carNumber, Color carColor, bool shadowCarMode, Matrix renderMatrix)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		renderMatrix = objectMatrix * renderMatrix;
		if (shadowCarMode)
		{
			ShaderEffect simpleShader = ShaderEffect.lighting;
			simpleShader.Render("ShadowCar", delegate
			{
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				int num7 = 0;
				for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes).Count; i++)
				{
					ModelMesh val5 = ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes)[i];
					Matrix val6 = transforms[val5.ParentBone.Index];
					if (((ReadOnlyCollection<ModelMeshPart>)(object)val5.MeshParts).Count == 2)
					{
						num7++;
						val6 = Matrix.CreateRotationX((float)((num7 == 2 || num7 == 4) ? 1 : (-1)) * RacingGameManager.Player.CarWheelPos) * val6;
					}
					BaseGame.WorldMatrix = val6 * renderMatrix;
					simpleShader.SetParameters();
					simpleShader.Update();
					for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)(object)val5.MeshParts).Count; j++)
					{
						ModelMeshPart val7 = ((ReadOnlyCollection<ModelMeshPart>)(object)val5.MeshParts)[j];
						BaseGame.Device.VertexDeclaration = val7.VertexDeclaration;
						BaseGame.Device.Vertices[0].SetSource(val5.VertexBuffer, val7.StreamOffset, val7.VertexStride);
						BaseGame.Device.Indices = val5.IndexBuffer;
						BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, val7.BaseVertex, 0, val7.NumVertices, val7.StartIndex, val7.PrimitiveCount);
					}
				}
			});
			return;
		}
		Color defaultAmbientColor = Material.DefaultAmbientColor;
		Color defaultDiffuseColor = Material.DefaultDiffuseColor;
		EffectTechnique val = null;
		for (int num = 0; num < 2; num++)
		{
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int num5 = 0; num5 < ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes).Count; num5++)
			{
				ModelMesh val2 = ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes)[num5];
				bool flag = false;
				for (int num6 = 0; num6 < ((ReadOnlyCollection<Effect>)(object)val2.Effects).Count; num6++)
				{
					Effect val3 = ((ReadOnlyCollection<Effect>)(object)val2.Effects)[num6];
					if (num6 == 0)
					{
						val = val3.CurrentTechnique;
					}
					if (time <= 1f)
					{
						time += 0.0001f;
					}
					else
					{
						time = 0f;
					}
					if (val3.Parameters["time"] != null)
					{
						val3.Parameters["time"].SetValue(time);
					}
					if (cachedIsReflectionSpecularTechnique[num4++])
					{
						if (num == 0)
						{
							flag = true;
							num3 += 7;
							break;
						}
						num3 += 3;
					}
					else
					{
						if (num == 1)
						{
							flag = true;
							num3 += 7;
							break;
						}
						cachedEffectParameters[num3++].SetValue((Texture)(object)RacingGameManager.CarTexture(carNumber).XnaTexture);
						cachedEffectParameters[num3++].SetValue(((Color)(ref defaultAmbientColor)).ToVector4());
						cachedEffectParameters[num3++].SetValue(((Color)(ref defaultDiffuseColor)).ToVector4());
						if (RacingGameManager.currentCarColor != 0 && num6 == 0)
						{
							val3.CurrentTechnique = val3.Techniques["SpecularWithReflectionForCar20"];
							val3.Parameters["carHueColor"].SetValue(((Color)(ref carColor)).ToVector3());
						}
					}
					Matrix val4 = transforms[val2.ParentBone.Index];
					if (((ReadOnlyCollection<ModelMeshPart>)(object)val2.MeshParts).Count == 2)
					{
						num2++;
						val4 = Matrix.CreateRotationX((float)((num2 == 2 || num2 == 4) ? 1 : (-1)) * RacingGameManager.Player.CarWheelPos) * val4;
					}
					BaseGame.WorldMatrix = val4 * renderMatrix;
					cachedEffectParameters[num3++].SetValue(BaseGame.WorldMatrix);
					cachedEffectParameters[num3++].SetValue(BaseGame.ViewProjectionMatrix);
					cachedEffectParameters[num3++].SetValue(BaseGame.InverseViewMatrix);
					cachedEffectParameters[num3++].SetValue(BaseGame.LightDirection);
				}
				if (!flag)
				{
					val2.Draw();
				}
				if (RacingGameManager.currentCarColor != 0 && val != null)
				{
					((ReadOnlyCollection<Effect>)(object)val2.Effects)[0].CurrentTechnique = val;
				}
			}
		}
	}

	public void GenerateShadow(Matrix renderMatrix)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		float num = scaling / 2.5f + 1.015f * ShaderEffect.shadowMapping.ShadowDistance;
		if (Vector3.DistanceSquared(ShaderEffect.shadowMapping.ShadowLightPos, ((Matrix)(ref renderMatrix)).Translation) > num * num)
		{
			return;
		}
		renderMatrix = objectMatrix * renderMatrix;
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes).Count; i++)
		{
			ModelMesh val = ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes)[i];
			ShaderEffect.shadowMapping.UpdateGenerateShadowWorldMatrix(transforms[val.ParentBone.Index] * renderMatrix);
			if (animatedMesh == val)
			{
				ShadowMapShader shadowMapping = ShaderEffect.shadowMapping;
				Vector3 translation = ((Matrix)(ref renderMatrix)).Translation;
				shadowMapping.UpdateGenerateShadowWorldMatrix(Matrix.CreateRotationZ(((Vector3)(ref translation)).Length() * 3f + ((Matrix)(ref renderMatrix)).Determinant() * 5f + (1f + (float)((int)(renderMatrix.M42 * 33.3f) % 100) * 0.00123f) * BaseGame.TotalTime / 0.654f) * transforms[val.ParentBone.Index] * renderMatrix);
			}
			for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts).Count; j++)
			{
				ModelMeshPart val2 = ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts)[j];
				BaseGame.Device.VertexDeclaration = val2.VertexDeclaration;
				BaseGame.Device.Vertices[0].SetSource(val.VertexBuffer, val2.StreamOffset, val2.VertexStride);
				BaseGame.Device.Indices = val.IndexBuffer;
				BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, val2.BaseVertex, 0, val2.NumVertices, val2.StartIndex, val2.PrimitiveCount);
			}
		}
	}

	public void UseShadow(Matrix renderMatrix)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (hasAlpha)
		{
			return;
		}
		float num = 1.015f * ShaderEffect.shadowMapping.ShadowDistance;
		if (Vector3.DistanceSquared(ShaderEffect.shadowMapping.ShadowLightPos, ((Matrix)(ref renderMatrix)).Translation) > num * num)
		{
			return;
		}
		renderMatrix = objectMatrix * renderMatrix;
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes).Count; i++)
		{
			ModelMesh val = ((ReadOnlyCollection<ModelMesh>)(object)xnaModel.Meshes)[i];
			ShaderEffect.shadowMapping.UpdateCalcShadowWorldMatrix(transforms[val.ParentBone.Index] * renderMatrix);
			if (animatedMesh == val)
			{
				ShadowMapShader shadowMapping = ShaderEffect.shadowMapping;
				Vector3 translation = ((Matrix)(ref renderMatrix)).Translation;
				shadowMapping.UpdateCalcShadowWorldMatrix(Matrix.CreateRotationZ(((Vector3)(ref translation)).Length() * 3f + ((Matrix)(ref renderMatrix)).Determinant() * 5f + (1f + (float)((int)(renderMatrix.M42 * 33.3f) % 100) * 0.00123f) * BaseGame.TotalTime / 0.654f) * transforms[val.ParentBone.Index] * renderMatrix);
			}
			for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts).Count; j++)
			{
				ModelMeshPart val2 = ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts)[j];
				BaseGame.Device.VertexDeclaration = val2.VertexDeclaration;
				BaseGame.Device.Vertices[0].SetSource(val.VertexBuffer, val2.StreamOffset, val2.VertexStride);
				BaseGame.Device.Indices = val.IndexBuffer;
				BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, val2.BaseVertex, 0, val2.NumVertices, val2.StartIndex, val2.PrimitiveCount);
			}
		}
	}

	static Model()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		objectMatrix = Matrix.CreateRotationX((float)Math.PI / 2f);
		maxViewDistance = 200;
	}
}
