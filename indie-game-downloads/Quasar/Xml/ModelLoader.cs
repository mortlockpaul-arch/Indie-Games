using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.ContentPipeline;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;

namespace Quasar.Xml;

public class ModelLoader
{
	public static RenderItem LoadMeshHierarchy(string filename, bool optimize, bool resetMaterialColor)
	{
		try
		{
			Model model = Engine.ContentManager.Load<Model>("Models/" + filename);
			Dictionary<int, RenderItem> dictionary = new Dictionary<int, RenderItem>(10);
			RenderItem renderItem = new RenderItem();
			FillBoneHierarchy(model.Root, dictionary, renderItem);
			foreach (ModelMesh mesh in model.Meshes)
			{
				XMesh xMesh = new XMesh(mesh);
				if (resetMaterialColor)
				{
					foreach (Material material in xMesh.Materials)
					{
						material.Diffuse = Vector3.One;
						material.Ambient = Vector3.One;
					}
				}
				if (dictionary.TryGetValue(mesh.ParentBone.Index, out var value))
				{
					value.addMesh(xMesh);
				}
				else
				{
					renderItem.addMesh(xMesh);
				}
			}
			if (optimize)
			{
				OptimizeHierarchy(renderItem);
			}
			return renderItem;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static void FillBoneHierarchy(ModelBone bone, Dictionary<int, RenderItem> dictionary, RenderItem item)
	{
		item.Transform.Matrix = bone.Transform;
		dictionary.Add(bone.Index, item);
		foreach (ModelBone child in bone.Children)
		{
			RenderItem renderItem = new RenderItem();
			FillBoneHierarchy(child, dictionary, renderItem);
			item.addChild(renderItem);
		}
	}

	private static void OptimizeHierarchy(RenderItem ri)
	{
		for (int i = 0; i < ri.Children.Count; i++)
		{
			RenderItem renderItem = ri.Children[i] as RenderItem;
			OptimizeHierarchy(renderItem);
			if (renderItem.Meshes.Count == 0 && renderItem.Children.Count == 0)
			{
				ri.removeChild(renderItem);
				i--;
			}
		}
		if (ri.Children.Count != 1 || ri.Meshes.Count != 0)
		{
			return;
		}
		RenderItem renderItem2 = ri.Children[0] as RenderItem;
		ri.Transform.Matrix = renderItem2.Transform.Matrix * ri.Transform.Matrix;
		foreach (Mesh mesh in renderItem2.Meshes)
		{
			ri.addMesh(mesh);
		}
		foreach (RenderItem child in renderItem2.Children)
		{
			ri.addChild(child);
		}
		ri.removeChild(renderItem2);
	}

	public static RenderItem LoadModelDefinition(string filename)
	{
		RenderItem renderItem = new RenderItem();
		LoadModelDefinition(filename, renderItem);
		return renderItem;
	}

	public static void LoadModelDefinition(string filename, RenderItem root)
	{
		try
		{
			string text = Engine.ProcessPath("Models/", filename);
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>(text);
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			string text2 = Path.GetDirectoryName(text).Replace('\\', '/') + '/';
			string text3 = Engine.ProcessPath(text2, XDocHelper.GetAttribute(xDocument.Root, "source"));
			bool flag = XDocHelper.ParseBoolAttribute(xDocument.Root, "isAnimated", defaultValue: false);
			bool flag2 = XDocHelper.ParseBoolAttribute(xDocument.Root, "isInstanced", defaultValue: false);
			float num = XDocHelper.ParseFloatAttribute(xDocument.Root, "timeScale", 1f);
			float num2 = XDocHelper.ParseFloatAttribute(xDocument.Root, "timeScaleVariation", 0f);
			num += GameMath.Random.NextFloat(0f - num2, num2);
			Model model = Engine.ContentManager.Load<Model>(text3);
			Dictionary<int, RenderItem> dictionary = new Dictionary<int, RenderItem>(10);
			FillBoneHierarchy(model.Root, dictionary, root);
			int num3 = 0;
			foreach (XElement item in xDocument.Root.Elements("Mesh"))
			{
				if (num3 >= model.Meshes.Count)
				{
					break;
				}
				ModelMesh modelMesh = model.Meshes[num3];
				Mesh mesh = null;
				if (!flag && !flag2)
				{
					mesh = new XMesh(modelMesh);
				}
				else if (flag)
				{
					bool interpolate = XDocHelper.ParseBoolAttribute(item, "interpolate", defaultValue: false);
					mesh = SkinnedMesh.LoadSkinnedMesh(text3, interpolate);
					(mesh as SkinnedMesh).SetAnimation("Default");
					(mesh as SkinnedMesh).Loop = true;
					(mesh as SkinnedMesh).SetTimeScale(num);
				}
				else if (flag2)
				{
					mesh = ((!XDocHelper.HasAttribute(item, "maxInstances")) ? new InstancedMesh(text3) : new InstancedMesh(text3, XDocHelper.ParseIntAttribute(item, "maxInstances")));
				}
				if (XDocHelper.HasAttribute(item, "shader"))
				{
					mesh.Shader = ShaderManager.Shaders[XDocHelper.GetAttribute(item, "shader")];
				}
				int num4 = 0;
				foreach (XElement item2 in item.Elements("Material"))
				{
					if (num4 < mesh.Materials.Count)
					{
						Material material = mesh.Materials[num4];
						material.FromXml(item2, text2);
						num4++;
						continue;
					}
					break;
				}
				if (dictionary.TryGetValue(modelMesh.ParentBone.Index, out var value))
				{
					value.addMesh(mesh);
				}
				else
				{
					root.addMesh(mesh);
				}
				num3++;
			}
			OptimizeHierarchy(root);
		}
		catch (Exception)
		{
		}
	}
}
