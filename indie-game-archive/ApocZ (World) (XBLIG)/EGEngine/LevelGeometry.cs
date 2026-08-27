using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class LevelGeometry
{
	private const int numOfObjects = 4;

	private const int BaseId = 1000;

	private static ObjectDefinition[] objectDefs = new ObjectDefinition[4]
	{
		new ObjectDefinition("cellar00", ObjectTypes.Prop),
		new ObjectDefinition("main_f1", ObjectTypes.Prop),
		new ObjectDefinition("floor3_attic", ObjectTypes.Prop),
		new ObjectDefinition("house_outside", ObjectTypes.Prop)
	};

	private static int objIds = 1000;

	private static bool isInitialized = false;

	public static List<LevelGeometry> assetList = new List<LevelGeometry>();

	public static List<LevelGeometry> levelSegmentList = new List<LevelGeometry>();

	public int id;

	public int index;

	public string name = "";

	public Matrix worldTransform;

	public Matrix inverseTransform;

	public ObjectTypes objType;

	public Model model;

	public List<LevelSegment> segments = new List<LevelSegment>();

	public static void LoadContent(ContentManager contMgr)
	{
		if (!isInitialized)
		{
			isInitialized = true;
			for (int i = 0; i < 4; i++)
			{
				LevelGeometry levelGeometry = new LevelGeometry();
				levelGeometry.id = -1;
				levelGeometry.index = i;
				levelGeometry.name = objectDefs[i].objName;
				levelGeometry.objType = objectDefs[i].objType;
				levelGeometry.model = contMgr.Load<Model>("models\\" + ObjStrings.Paths[(int)levelGeometry.objType] + levelGeometry.name);
				Matrix[] array = new Matrix[levelGeometry.model.Bones.Count];
				levelGeometry.model.CopyAbsoluteBoneTransformsTo(array);
				levelGeometry.SetSegments(levelGeometry.model, array, contMgr, ObjStrings.Paths[(int)levelGeometry.objType]);
				assetList.Add(levelGeometry);
			}
		}
	}

	public virtual LevelGeometry Create(LevelGeometry e, string name, Matrix transform)
	{
		foreach (LevelGeometry asset in assetList)
		{
			if (asset.name == name)
			{
				e.id = objIds++;
				e.index = asset.index;
				e.name = asset.name;
				e.objType = asset.objType;
				e.worldTransform = transform;
				e.inverseTransform = Matrix.Invert(transform);
				e.model = asset.model;
				e.segments = asset.segments;
				levelSegmentList.Add(e);
			}
		}
		return e;
	}

	public virtual void PrevObject()
	{
		index--;
		if (index < 0)
		{
			index = assetList.Count - 1;
		}
		name = assetList[index].name;
		objType = assetList[index].objType;
		model = assetList[index].model;
		segments = assetList[index].segments;
	}

	public virtual void NextObject()
	{
		index++;
		if (index >= assetList.Count)
		{
			index = 0;
		}
		name = assetList[index].name;
		objType = assetList[index].objType;
		model = assetList[index].model;
		segments = assetList[index].segments;
	}

	public virtual void Update(GameTime gameTime)
	{
		_ = gameTime.ElapsedGameTime.Milliseconds;
	}

	public virtual void UpdateEditor(GameTime gameTime)
	{
		_ = gameTime.ElapsedGameTime.Milliseconds;
		for (int i = 0; i < segments.Count; i++)
		{
			_ = segments[i];
		}
	}

	public virtual void Draw(int qIndex, RenderPass pass)
	{
		Effect materialEffect = EndGameEngine.MaterialEffect;
		GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		materialEffect.CurrentTechnique = materialParams.T_PropObject;
		Vector4[] value = new Vector4[4];
		Vector3[] value2 = new Vector3[4];
		foreach (LevelSegment segment in segments)
		{
			int num = 0;
			materialParams.numberLights.SetValue(num);
			if (num > 0)
			{
				materialParams.vecLightPositions.SetValue(value);
				materialParams.vecLightColors.SetValue(value2);
			}
			foreach (ModelMesh item in segment.Geometry)
			{
				materialEffect.CurrentTechnique = materialParams.T_PropObject;
				CustomContent customContent = item.Tag as CustomContent;
				if (customContent.textureName.Contains("null"))
				{
					continue;
				}
				Matrix value3 = customContent.transform * worldTransform;
				materialParams.matWorld.SetValue(value3);
				foreach (ModelMeshPart meshPart in item.MeshParts)
				{
					graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
					graphicsDevice.Indices = meshPart.IndexBuffer;
					materialParams.propDiffuse1.SetValue(customContent.DiffuseMap);
					materialParams.propNormal1.SetValue(customContent.NormalMap);
					switch (num)
					{
					case 1:
						materialEffect.CurrentTechnique.Passes[1].Apply();
						break;
					case 2:
						materialEffect.CurrentTechnique.Passes[2].Apply();
						break;
					case 3:
						materialEffect.CurrentTechnique.Passes[3].Apply();
						break;
					default:
						materialEffect.CurrentTechnique.Passes[0].Apply();
						break;
					}
					graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
				}
			}
			foreach (WorldObject worldObject in segment.WorldObjectList)
			{
				worldObject.Draw(qIndex, pass);
			}
			foreach (DoorObject door in segment.Doors)
			{
				door.Draw(qIndex, pass);
			}
		}
		_ = objType;
	}

	public virtual void DrawEditor(ref Vector2 textPos, float scale, float fontHeight)
	{
		textPos.Y += fontHeight;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "ObjectName: " + name.ToString(), textPos, new Color(255, 255, 255, 255), 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
		textPos.Y += fontHeight;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "ObjectType: " + objType, textPos, new Color(255, 255, 255, 255), 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
	}

	public virtual MaterialType RayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, ref Vector3 hitPos, ref Vector3 hitNorm)
	{
		MaterialType materialType = MaterialType.Undefined;
		for (int i = 0; i < segments.Count; i++)
		{
			LevelSegment levelSegment = segments[i];
			materialType = levelSegment.RayCast(qIndex, ref origin, ref direction, ref hitPos, ref hitNorm);
			if (materialType != MaterialType.Undefined)
			{
				return materialType;
			}
		}
		return materialType;
	}

	public virtual bool AddWorldObject(WorldObject obj)
	{
		Vector3 p = obj.worldTransform.Translation;
		foreach (LevelSegment segment in segments)
		{
			if (segment.origin.ContainsPoint(ref p))
			{
				segment.AddWorldObject(obj);
				return true;
			}
		}
		return false;
	}

	public void SetSegments(Model m, Matrix[] t, ContentManager contMgr, string pathStr)
	{
		foreach (ModelMesh mesh in m.Meshes)
		{
			CustomContent customContent = mesh.Tag as CustomContent;
			customContent.transform = t[mesh.ParentBone.Index];
		}
		foreach (ModelMesh mesh2 in m.Meshes)
		{
			if (mesh2.Name.Contains("segment"))
			{
				LevelSegment levelSegment = new LevelSegment();
				levelSegment.SetFromMesh(mesh2, VertexType.BakedLight);
				levelSegment.AddChildren(m, t, contMgr, pathStr);
				segments.Add(levelSegment);
			}
		}
	}

	public bool IntersectPhysicsSphere(ref BoundingSphere sphere, ref CollisionStruct collision)
	{
		for (int i = 0; i < segments.Count; i++)
		{
			LevelSegment levelSegment = segments[i];
			if (levelSegment.IntersectSphere(ref sphere))
			{
				levelSegment.IntersectPhysicsSphere(ref sphere, ref collision);
			}
		}
		return false;
	}
}
