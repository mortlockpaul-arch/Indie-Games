using System;
using System.Collections.ObjectModel;
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
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		string[] array = str.Split(',');
		if (array.GetLength(0) != 3)
		{
			return new Vector3(-1f, -1f, -1f);
		}
		return new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
	}

	public static Vector2 StringToVector2(string str)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		string[] array = str.Split(',');
		if (array.GetLength(0) != 2)
		{
			return new Vector2(-1f, -1f);
		}
		return new Vector2(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]));
	}

	public static void LoadModelFile(string modelName, out Model model, out Texture2D texture, out Matrix[] transforms, ref BoundingSphere[] collisionSpheres)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
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
			collisionSpheres = (BoundingSphere[])(object)new BoundingSphere[count];
			int num = 0;
			foreach (XmlNode item in xmlNodeList)
			{
				Vector3 val = StringToVector3(item.Attributes["position"].Value);
				float num2 = Convert.ToSingle(item.Attributes["radius"].Value);
				ref BoundingSphere reference = ref collisionSpheres[num++];
				reference = new BoundingSphere(val, num2);
			}
		}
		model = MainGame.ContentMan.Load<Model>("Models/" + value);
		texture = MainGame.ContentMan.Load<Texture2D>("Textures/" + value2);
		transforms = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)model.Bones).Count];
		model.CopyAbsoluteBoneTransformsTo(transforms);
	}

	public static void LoadModelFile(string modelName, out Model model, out Matrix[] transforms, ref BoundingSphere[] collisionSpheres)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
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
			collisionSpheres = (BoundingSphere[])(object)new BoundingSphere[count];
			int num = 0;
			foreach (XmlNode item in xmlNodeList)
			{
				Vector3 val = StringToVector3(item.Attributes["position"].Value);
				float num2 = Convert.ToSingle(item.Attributes["radius"].Value);
				ref BoundingSphere reference = ref collisionSpheres[num++];
				reference = new BoundingSphere(val, num2);
			}
		}
		transforms = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)model.Bones).Count];
		model.CopyAbsoluteBoneTransformsTo(transforms);
	}

	public unsafe static void RemapModelEffects(Model model, Effect effect, Texture2D texture)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.MeshParts.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						ModelMeshPart current2 = ((Enumerator)(ref enumerator2)).Current;
						current2.Effect = effect;
						current2.Effect.Parameters["Projection"].SetValue(MainGame.ProjectionMatrix);
						current2.Effect.Parameters["MeshTexture"].SetValue((Texture)(object)texture);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public unsafe static void RemapModelEffects(Model model, Effect effect, Texture2D texture, Texture2D detailTexture)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.MeshParts.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						ModelMeshPart current2 = ((Enumerator)(ref enumerator2)).Current;
						current2.Effect = effect;
						current2.Effect.Parameters["Projection"].SetValue(MainGame.ProjectionMatrix);
						current2.Effect.Parameters["MeshTexture"].SetValue((Texture)(object)texture);
						current2.Effect.Parameters["DetailTexture"].SetValue((Texture)(object)detailTexture);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public unsafe static void PrepareBasicEffectModel(Model model)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.PreferPerPixelLighting = true;
						val.Projection = MainGame.ProjectionMatrix;
						val.View = MainGame.ViewMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos2 - pos1;
		((Vector3)(ref val)).Normalize();
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(0f - val.Y, val.X, 0f);
		float num = Vector3.Dot(val, v1);
		float num2 = Vector3.Dot(val2, v1);
		float num3 = Vector3.Dot(val, v2);
		float num4 = Vector3.Dot(val2, v2);
		float num5 = num2;
		float num6 = num4;
		float num7 = num3;
		float num8 = num;
		Vector3 val3 = num7 * val;
		Vector3 val4 = num8 * val;
		Vector3 val5 = num5 * val2;
		Vector3 val6 = num6 * val2;
		Vector3 val7 = val3 + val5;
		Vector3 val8 = val4 + val6;
		Vector3 val9 = v1 - val7;
		energy1 = ((Vector3)(ref val9)).Length() * 0.75f;
		val9 = v2 - val8;
		energy2 = ((Vector3)(ref val9)).Length() * 0.75f;
		v1 = val7;
		v2 = val8;
		v1.Z = 0f;
		v2.Z = 0f;
	}

	public static void SetRichPresence(GamerPresenceMode mode, int? value)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				current.Presence.PresenceMode = mode;
				if (value.HasValue)
				{
					current.Presence.PresenceValue = value.Value;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public static GamerPresenceMode GetRichPresenceMode()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 0)
		{
			return Gamer.SignedInGamers[(PlayerIndex)0].Presence.PresenceMode;
		}
		return (GamerPresenceMode)0;
	}

	public static void AdjustVector(ref Vector2 vec, float angle)
	{
		float num = ((Vector2)(ref vec)).Length();
		float num2 = AngleFromVector(ref vec);
		float num3 = num2 + angle;
		vec.Y = (float)Math.Sin(num3) * num;
		vec.X = (float)Math.Cos(num3) * num;
	}

	public static void AdjustVector(ref Vector3 vec, float angle)
	{
		float num = ((Vector3)(ref vec)).Length();
		float num2 = AngleFromVector(ref vec);
		float num3 = num2 + angle;
		vec.Y = (float)Math.Sin(num3) * num;
		vec.X = (float)Math.Cos(num3) * num;
	}
}
