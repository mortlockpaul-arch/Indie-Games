using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class WorldObject
{
	private const int numOfObjects = 1;

	private const int BaseId = 1000;

	private static ObjectDefinition[] objectDefs = new ObjectDefinition[1]
	{
		new ObjectDefinition("box", ObjectTypes.Object)
	};

	private static int objIds = 1000;

	private static bool isInitialized = false;

	public static List<WorldObject> assetList = new List<WorldObject>();

	public static List<WorldObject> worldObjectList = new List<WorldObject>();

	public int id;

	public int index;

	public string name = "";

	public Matrix worldTransform;

	public Matrix inverseTransform;

	public ObjectTypes objType;

	public Model model;

	public static void LoadContent(ContentManager contMgr)
	{
		if (isInitialized)
		{
			return;
		}
		isInitialized = true;
		for (int i = 0; i < 1; i++)
		{
			WorldObject worldObject = new WorldObject();
			worldObject.id = -1;
			worldObject.index = i;
			worldObject.name = objectDefs[i].objName;
			worldObject.objType = objectDefs[i].objType;
			worldObject.model = contMgr.Load<Model>("models\\" + ObjStrings.Paths[(int)worldObject.objType] + worldObject.name);
			Matrix[] array = new Matrix[worldObject.model.Bones.Count];
			worldObject.model.CopyAbsoluteBoneTransformsTo(array);
			foreach (ModelMesh mesh in worldObject.model.Meshes)
			{
				CustomContent customContent = mesh.Tag as CustomContent;
				customContent.renderType = 1;
				customContent.transform = array[mesh.ParentBone.Index];
				if (mesh.Name.Contains("oobb"))
				{
					customContent.oobb = default(OOBB);
					((OOBB)customContent.oobb).SetFromMesh(mesh, customContent.transform, VertexType.Basic);
				}
				else if (customContent.textureName != "null")
				{
					customContent.renderType = 0;
					TextureBase.GetMaterialsTextureByName(contMgr, ObjStrings.Paths[(int)worldObject.objType] + customContent.textureName, out customContent.DiffuseMap, out customContent.NormalMap);
					customContent.SetPhysics(mesh, array[mesh.ParentBone.Index], VertexType.Basic);
				}
			}
			assetList.Add(worldObject);
		}
	}

	public virtual WorldObject Create(WorldObject e, string name, Matrix transform)
	{
		foreach (WorldObject asset in assetList)
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
				worldObjectList.Add(e);
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
	}

	public virtual void Update(GameTime gameTime)
	{
		_ = gameTime.ElapsedGameTime.Milliseconds;
	}

	public virtual void UpdateEditor(GameTime gameTime)
	{
		inverseTransform = Matrix.Invert(worldTransform);
	}

	public virtual void Draw(int qIndex, RenderPass pass)
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
		int num = 0;
		foreach (ModelMesh mesh in model.Meshes)
		{
			CustomContent customContent = mesh.Tag as CustomContent;
			if (customContent.renderType == 0)
			{
				Matrix value = customContent.transform * worldTransform;
				materialParams.matWorld.SetValue(value);
				graphicsDevice.SetVertexBuffer(mesh.MeshParts[0].VertexBuffer, mesh.MeshParts[0].VertexOffset);
				graphicsDevice.Indices = mesh.MeshParts[0].IndexBuffer;
				materialParams.propDiffuse1.SetValue(customContent.DiffuseMap);
				materialParams.propNormal1.SetValue(customContent.NormalMap);
				switch (num)
				{
				case 2:
					materialEffect.CurrentTechnique.Passes[1].Apply();
					break;
				case 3:
					materialEffect.CurrentTechnique.Passes[2].Apply();
					break;
				case 4:
					materialEffect.CurrentTechnique.Passes[3].Apply();
					break;
				default:
					materialEffect.CurrentTechnique.Passes[0].Apply();
					break;
				}
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.MeshParts[0].NumVertices, mesh.MeshParts[0].StartIndex, mesh.MeshParts[0].PrimitiveCount);
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
		return MaterialType.Undefined;
	}
}
