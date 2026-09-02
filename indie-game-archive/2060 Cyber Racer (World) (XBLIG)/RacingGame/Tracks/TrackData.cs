using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using RacingGame.Helpers;

namespace RacingGame.Tracks;

public class TrackData
{
	[Serializable]
	public class WidthHelper
	{
		public Vector3 pos;

		public float scale;

		public WidthHelper()
		{
		}

		public WidthHelper(Vector3 setPos, float setScale)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector();
			pos = setPos;
			scale = setScale;
		}
	}

	[Serializable]
	public class RoadHelper
	{
		public enum HelperType
		{
			Tunnel,
			Palms,
			Laterns,
			Reset
		}

		public HelperType type;

		public Vector3 pos;

		public RoadHelper()
		{
		}

		public RoadHelper(HelperType setType, Vector3 setPos)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector();
			type = setType;
			pos = setPos;
		}
	}

	[Serializable]
	public class NeutralObject
	{
		public string modelName;

		public Matrix matrix;

		public NeutralObject()
		{
		}

		public NeutralObject(string setModelName, Matrix setMatrix)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector();
			modelName = setModelName;
			matrix = setMatrix;
		}
	}

	public const string Directory = "Content";

	public const string Extension = "Track";

	private List<Vector3> trackPoints = new List<Vector3>();

	private List<WidthHelper> widthHelpers = new List<WidthHelper>();

	private List<RoadHelper> roadHelpers = new List<RoadHelper>();

	private List<NeutralObject> objects = new List<NeutralObject>();

	public List<Vector3> TrackPoints => trackPoints;

	public List<WidthHelper> WidthHelpers => widthHelpers;

	public List<RoadHelper> RoadHelpers => roadHelpers;

	public List<NeutralObject> NeutralsObjects => objects;

	public TrackData()
	{
	}

	public TrackData(List<Vector3> setTrackPoints, List<WidthHelper> setWidthHelpers, List<RoadHelper> setRoadHelpers, List<NeutralObject> setObjects)
	{
		trackPoints = setTrackPoints;
		widthHelpers = setWidthHelpers;
		roadHelpers = setRoadHelpers;
		objects = setObjects;
	}

	public static TrackData Load(string setFilename)
	{
		StreamReader streamReader = new StreamReader(FileHelper.LoadGameContentFile("Content\\" + setFilename + ".Track"));
		TrackData result = (TrackData)new XmlSerializer(typeof(TrackData)).Deserialize(streamReader.BaseStream);
		streamReader.Close();
		return result;
	}
}
