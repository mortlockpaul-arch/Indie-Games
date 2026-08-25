using System;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace SpaceBlast;

internal class Utils
{
	private const float constCollisionDamageMultiplier = 0.75f;

	public static Vector3 StringToVector3(string str)
	{
		string[] array = str.Split(',');
		if (array.GetLength(0) != 3)
		{
			return new Vector3(-1f, -1f, -1f);
		}
		return new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
	}

	public static Vector2 StringToVector2(string str)
	{
		string[] array = str.Split(',');
		if (array.GetLength(0) != 2)
		{
			return new Vector2(-1f, -1f);
		}
		return new Vector2(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]));
	}

	public static void LoadModelFile(string modelName, out Model model, out Texture2D texture, out Matrix[] transforms, ref BoundingSphere[] collisionSpheres)
	{
		modelName = "Content/ModelInfo/" + modelName + ".model";
		string path = Path.Combine(StorageContainer.TitleLocation, modelName);
		FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream);
		string xml = streamReader.ReadToEnd();
		fileStream.Close();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		XmlNode documentElement = xmlDocument.DocumentElement;
		XmlNode xmlNode = documentElement.SelectSingleNode("Model");
		string value = xmlNode.Attributes["model"].Value;
		string value2 = xmlNode.Attributes["texture"].Value;
		XmlNodeList xmlNodeList = documentElement.SelectNodes("CollisionSpheres/ColSphere");
		int count = xmlNodeList.Count;
		if (count > 0)
		{
			collisionSpheres = new BoundingSphere[count];
			int num = 0;
			foreach (XmlNode item in xmlNodeList)
			{
				Vector3 center = StringToVector3(item.Attributes["position"].Value);
				float radius = Convert.ToSingle(item.Attributes["radius"].Value);
				ref BoundingSphere reference = ref collisionSpheres[num++];
				reference = new BoundingSphere(center, radius);
			}
		}
		model = MainGame.ContentMan.Load<Model>("Models/" + value);
		texture = MainGame.ContentMan.Load<Texture2D>("Textures/" + value2);
		transforms = new Matrix[model.Bones.Count];
		model.CopyAbsoluteBoneTransformsTo(transforms);
	}

	public static void LoadModelFile(string modelName, out Model model, out Matrix[] transforms, ref BoundingSphere[] collisionSpheres)
	{
		modelName = "Content/ModelInfo/" + modelName + ".model";
		string path = Path.Combine(StorageContainer.TitleLocation, modelName);
		FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream);
		string xml = streamReader.ReadToEnd();
		fileStream.Close();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		XmlNode documentElement = xmlDocument.DocumentElement;
		XmlNode xmlNode = documentElement.SelectSingleNode("Model");
		string value = xmlNode.Attributes["model"].Value;
		model = MainGame.ContentMan.Load<Model>("Models/" + value);
		XmlNodeList xmlNodeList = documentElement.SelectNodes("CollisionSpheres/ColSphere");
		int count = xmlNodeList.Count;
		if (count > 0)
		{
			collisionSpheres = new BoundingSphere[count];
			int num = 0;
			foreach (XmlNode item in xmlNodeList)
			{
				Vector3 center = StringToVector3(item.Attributes["position"].Value);
				float radius = Convert.ToSingle(item.Attributes["radius"].Value);
				ref BoundingSphere reference = ref collisionSpheres[num++];
				reference = new BoundingSphere(center, radius);
			}
		}
		transforms = new Matrix[model.Bones.Count];
		model.CopyAbsoluteBoneTransformsTo(transforms);
	}

	public static void RemapModelEffects(Model model, Effect effect, Texture2D texture)
	{
		foreach (ModelMesh mesh in model.Meshes)
		{
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				meshPart.Effect = effect;
				meshPart.Effect.Parameters["Projection"].SetValue(MainGame.ProjectionMatrix);
				meshPart.Effect.Parameters["MeshTexture"].SetValue(texture);
			}
		}
	}

	public static void RemapModelEffects(Model model, Effect effect, Texture2D texture, Texture2D detailTexture)
	{
		foreach (ModelMesh mesh in model.Meshes)
		{
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				meshPart.Effect = effect;
				meshPart.Effect.Parameters["Projection"].SetValue(MainGame.ProjectionMatrix);
				meshPart.Effect.Parameters["MeshTexture"].SetValue(texture);
				meshPart.Effect.Parameters["DetailTexture"].SetValue(detailTexture);
			}
		}
	}

	public static void PrepareBasicEffectModel(Model model)
	{
		foreach (ModelMesh mesh in model.Meshes)
		{
			foreach (BasicEffect effect in mesh.Effects)
			{
				effect.EnableDefaultLighting();
				effect.PreferPerPixelLighting = true;
				effect.Projection = MainGame.ProjectionMatrix;
				effect.View = MainGame.ViewMatrix;
			}
		}
	}

	public static float NormaliseAngle(float angle)
	{
		while (angle >= (float)Math.PI * 2f)
		{
			angle -= (float)Math.PI * 2f;
		}
		while (angle < 0f)
		{
			angle += (float)Math.PI * 2f;
		}
		return angle;
	}

	public static void NormaliseAngle(ref float angle)
	{
		while (angle >= (float)Math.PI * 2f)
		{
			angle -= (float)Math.PI * 2f;
		}
		while (angle < 0f)
		{
			angle += (float)Math.PI * 2f;
		}
	}

	public static float AngleFromVector(ref Vector3 vec)
	{
		return (float)Math.Atan2(vec.Y, vec.X);
	}

	public static float AngleFromVector(ref Vector2 vec)
	{
		return (float)Math.Atan2(vec.Y, vec.X);
	}

	public static void ElasticCollision(ref Vector3 pos1, ref Vector3 v1, ref Vector3 pos2, ref Vector3 v2, out float energy1, out float energy2)
	{
		Vector3 vector = pos2 - pos1;
		vector.Normalize();
		Vector3 vector2 = new Vector3(0f - vector.Y, vector.X, 0f);
		float num = Vector3.Dot(vector, v1);
		float num2 = Vector3.Dot(vector2, v1);
		float num3 = Vector3.Dot(vector, v2);
		float num4 = Vector3.Dot(vector2, v2);
		float num5 = num2;
		float num6 = num4;
		float num7 = num3;
		float num8 = num;
		Vector3 vector3 = num7 * vector;
		Vector3 vector4 = num8 * vector;
		Vector3 vector5 = num5 * vector2;
		Vector3 vector6 = num6 * vector2;
		Vector3 vector7 = vector3 + vector5;
		Vector3 vector8 = vector4 + vector6;
		energy1 = (v1 - vector7).Length() * 0.75f;
		energy2 = (v2 - vector8).Length() * 0.75f;
		v1 = vector7;
		v2 = vector8;
		v1.Z = 0f;
		v2.Z = 0f;
	}

	public static void SetRichPresence(GamerPresenceMode mode, int? value)
	{
		foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
		{
			signedInGamer.Presence.PresenceMode = mode;
			if (value.HasValue)
			{
				signedInGamer.Presence.PresenceValue = value.Value;
			}
		}
	}

	public static GamerPresenceMode GetRichPresenceMode()
	{
		if (Gamer.SignedInGamers.Count > 0)
		{
			return Gamer.SignedInGamers[PlayerIndex.One].Presence.PresenceMode;
		}
		return GamerPresenceMode.None;
	}

	public static void AdjustVector(ref Vector2 vec, float angle)
	{
		float num = vec.Length();
		float num2 = AngleFromVector(ref vec);
		float num3 = num2 + angle;
		vec.Y = (float)Math.Sin(num3) * num;
		vec.X = (float)Math.Cos(num3) * num;
	}

	public static void AdjustVector(ref Vector3 vec, float angle)
	{
		float num = vec.Length();
		float num2 = AngleFromVector(ref vec);
		float num3 = num2 + angle;
		vec.Y = (float)Math.Sin(num3) * num;
		vec.X = (float)Math.Cos(num3) * num;
	}
}
