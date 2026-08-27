using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class TriggerPoints
{
	public static List<TriggerStruct> list = new List<TriggerStruct>();

	public static List<TriggerStruct> Ailist = new List<TriggerStruct>();

	public static Vector3[] PositionList;

	public static void LoadContent(Model m)
	{
		bool flag = false;
		ModelCollision modelCollision = default(ModelCollision);
		if (m.Tag != null)
		{
			flag = true;
			modelCollision = (ModelCollision)m.Tag;
			PositionList = modelCollision.PostionList.ToArray();
		}
		Matrix[] array = new Matrix[m.Bones.Count];
		m.CopyAbsoluteBoneTransformsTo(array);
		foreach (ModelMesh mesh in m.Meshes)
		{
			if (mesh.Name.Contains("trigger"))
			{
				TriggerStruct item = new TriggerStruct
				{
					flag = TriggerFlags.Clear
				};
				if (mesh.Name.Contains("building"))
				{
					item.flag = TriggerFlags.Building;
				}
				else if (mesh.Name.Contains("TRGS"))
				{
					item.flag = TriggerFlags.Target;
				}
				item.oobb = new OOBB(MeshTools.GetPositionsFromMesh(mesh, VertexType.Unknown), array[mesh.ParentBone.Index]);
				list.Add(item);
			}
			if (!flag)
			{
				continue;
			}
			if (mesh.Name.Contains("AI_Climb"))
			{
				foreach (MeshPartCollision modelPart in modelCollision.ModelPartList)
				{
					if (mesh.Name == modelPart.MeshName)
					{
						TriggerStruct item2 = new TriggerStruct
						{
							flag = TriggerFlags.AIClimb,
							oobb = new OOBB(MeshTools.GetPositionsFromMesh(mesh, VertexType.Unknown), array[mesh.ParentBone.Index]),
							mesh = modelPart.triangleData.ToArray()
						};
						Ailist.Add(item2);
					}
				}
			}
			if (mesh.Name.Contains("AI_Window"))
			{
				foreach (MeshPartCollision modelPart2 in modelCollision.ModelPartList)
				{
					if (mesh.Name == modelPart2.MeshName)
					{
						TriggerStruct item3 = new TriggerStruct
						{
							flag = TriggerFlags.AIWindow,
							oobb = new OOBB(MeshTools.GetPositionsFromMesh(mesh, VertexType.Unknown), array[mesh.ParentBone.Index]),
							mesh = modelPart2.triangleData.ToArray()
						};
						Ailist.Add(item3);
					}
				}
			}
			if (!mesh.Name.Contains("AI_Survival_Safe_Zone"))
			{
				continue;
			}
			foreach (MeshPartCollision modelPart3 in modelCollision.ModelPartList)
			{
				if (mesh.Name == modelPart3.MeshName)
				{
					TriggerStruct item4 = new TriggerStruct
					{
						flag = TriggerFlags.AISafeHouse,
						oobb = new OOBB(MeshTools.GetPositionsFromMesh(mesh, VertexType.Unknown), array[mesh.ParentBone.Index]),
						mesh = modelPart3.triangleData.ToArray()
					};
					Ailist.Add(item4);
				}
			}
		}
	}

	public static void Update(GameTime gameTime)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].oobb.ContainsPoint(ref LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition))
			{
				_ = list[i];
			}
		}
		float disSqr = float.MaxValue;
		Vector3 closestPoint = Vector3.Zero;
		for (int j = 0; j < BaseData.AllBotsList.Count; j++)
		{
			foreach (TriggerStruct item in Ailist)
			{
				if ((BaseData.AllBotsList[j].triggerFlags & item.flag) <= TriggerFlags.Clear)
				{
					continue;
				}
				for (int k = 0; k < item.mesh.Length; k++)
				{
					if (MyMath.IntersectSphereTriangle(ref BaseData.AllBotsList[j].Position, 2304f, ref item.mesh[k], ref closestPoint, ref disSqr, PositionList))
					{
						BaseData.AllBotsList[j].TriggerHit(item.flag);
						break;
					}
				}
			}
		}
		for (int l = 0; l < 4; l++)
		{
			if (!LevelBaseMenu.Players[l].IsValid || !LevelBaseMenu.Players[l].Spawned)
			{
				continue;
			}
			bool flag = false;
			foreach (TriggerStruct item2 in Ailist)
			{
				if ((LevelBaseMenu.Players[l].triggerFlags & item2.flag) > TriggerFlags.Clear)
				{
					OOBB oobb = item2.oobb;
					if (oobb.ContainsPoint(ref LevelBaseMenu.Players[l].vecPosition))
					{
						flag = true;
					}
				}
			}
			if (!flag && (LevelBaseMenu.Players[l].triggerFlags & TriggerFlags.AISafeHouse) > TriggerFlags.Clear)
			{
				AIBase.PlayerOutTrigger(TriggerFlags.AISafeHouse, LevelBaseMenu.Players[l]);
			}
		}
	}

	public static void Draw()
	{
	}
}
