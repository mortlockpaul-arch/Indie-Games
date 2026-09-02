using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using RacingGame.Helpers;
using RacingGame.Landscapes;

namespace RacingGame.Tracks;

public class TrackCombiModels
{
	[Serializable]
	public class CombiObject
	{
		public string modelName;

		public Matrix matrix;

		public CombiObject()
		{
		}

		public CombiObject(string setModelName, Matrix setMatrix)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector();
			modelName = setModelName;
			matrix = setMatrix;
		}
	}

	public const string Directory = "Content";

	public const string Extension = "CombiModel";

	private List<CombiObject> objects = new List<CombiObject>();

	private string name = "";

	private float size = 10f;

	public string Name => name;

	public float Size => size;

	public TrackCombiModels(string filename)
	{
		StreamReader streamReader = new StreamReader(FileHelper.LoadGameContentFile("Content\\" + filename + ".CombiModel"));
		objects = (List<CombiObject>)new XmlSerializer(typeof(List<CombiObject>)).Deserialize(streamReader.BaseStream);
		streamReader.Close();
		name = Path.GetFileNameWithoutExtension(filename);
		size = ((Name == "CombiPalms" || Name == "CombiPalms2" || Name == "CombiRuins" || Name == "CombiRuins2" || Name == "CombiStones" || Name == "CombiStones2") ? 10 : 50);
	}

	public void AddAllModels(Landscape landscape, Matrix parentMatrix)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		foreach (CombiObject @object in objects)
		{
			landscape.AddObjectToRender(@object.modelName, @object.matrix * parentMatrix, isNearTrackForShadowGeneration: false);
		}
	}
}
