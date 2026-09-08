using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class LevelSegment
{
	private ModelMesh sourceData;

	public OOBB origin = default(OOBB);

	public List<DoorObject> Doors = new List<DoorObject>();

	public List<OOBB> Physics = new List<OOBB>();

	public List<ModelMesh> PhysicsGeom = new List<ModelMesh>();

	public List<ModelMesh> Geometry = new List<ModelMesh>();

	public List<WorldObject> WorldObjectList = new List<WorldObject>();

	private static Ray tmpRay;

	private static CollisionStruct tmpCollision;

	public void SetFromMesh(ModelMesh mesh, VertexType v)
	{
		origin.SetFromMesh(mesh, ((CustomContent)mesh.Tag).transform, v);
		sourceData = mesh;
	}

	public void AddWorldObject(WorldObject obj)
	{
		WorldObjectList.Add(obj);
	}

	public void AddChildren(Model model, Matrix[] transforms, ContentManager contMgr, string pathStr)
	{
		ModelBoneCollection.Enumerator enumerator = sourceData.ParentBone.Children.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ModelBone current = enumerator.Current;
				ModelMeshCollection.Enumerator enumerator2 = model.Meshes.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ModelMesh current2 = enumerator2.Current;
						if (!current2.Name.Equals(current.Name))
						{
							continue;
						}
						CustomContent customContent = current2.Tag as CustomContent;
						for (int i = 0; i < 3; i++)
						{
							if (current2.Name.Contains(DoorObject.idString[i]))
							{
								DoorObject doorObject = new DoorObject();
								doorObject.Set(i, customContent.transform);
								Doors.Add(doorObject);
								break;
							}
						}
						if (current2.Name.Contains("oobb"))
						{
							OOBB item = default(OOBB);
							item.SetFromMesh(current2, customContent.transform, VertexType.BakedLight);
							Physics.Add(item);
						}
						else if (!current2.Name.Contains("segment"))
						{
							CustomContent customContent2 = current2.Tag as CustomContent;
							TextureBase.GetMaterialsTextureByName(contMgr, pathStr + customContent2.textureName, out customContent2.DiffuseMap, out customContent2.NormalMap);
							customContent2.SetPhysics(current2, customContent.transform, VertexType.BakedLight);
							Geometry.Add(current2);
						}
						break;
					}
				}
				finally
				{
					enumerator2.Dispose();
				}
			}
		}
		finally
		{
			enumerator.Dispose();
		}
	}

	public void UpdateEditor(Matrix transform)
	{
		for (int i = 0; i < Physics.Count; i++)
		{
			_ = Physics[i];
		}
	}

	public bool IntersectPhysicsSphere(ref BoundingSphere e, ref CollisionStruct c)
	{
		bool result = false;
		for (int i = 0; i < Physics.Count; i++)
		{
			if (Physics[i].CollisionSphere(ref e, ref c))
			{
				result = true;
			}
		}
		return result;
	}

	public MaterialType RayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, ref Vector3 hitPos, ref Vector3 hitNorm)
	{
		MaterialType result = MaterialType.Undefined;
		tmpRay.Position = origin;
		tmpRay.Direction = direction;
		for (int i = 0; i < Physics.Count; i++)
		{
			if (Physics[i].CollisionRay(ref tmpRay, ref tmpCollision))
			{
				hitNorm = tmpCollision.hitNormal;
				hitPos = tmpCollision.hitPosition;
				return MaterialType.Concrete;
			}
		}
		return result;
	}

	public bool IntersectSphere(ref BoundingSphere sphere)
	{
		return false;
	}

	static LevelSegment()
	{
		tmpRay = default(Ray);
		tmpCollision = default(CollisionStruct);
	}
}
