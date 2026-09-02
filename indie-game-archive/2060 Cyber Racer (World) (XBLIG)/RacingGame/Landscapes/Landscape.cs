using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameLogic;
using RacingGame.GameScreens;
using RacingGame.Graphics;
using RacingGame.Helpers;
using RacingGame.Shaders;
using RacingGame.Sounds;
using RacingGame.Tracks;

namespace RacingGame.Landscapes;

public class Landscape : IDisposable
{
	public class LandscapeObject
	{
		private Model model;

		private Matrix matrix;

		private bool isBanner;

		public bool IsBigBuilding
		{
			get
			{
				if (!model.Name.ToLower().Contains("hotel"))
				{
					return model.Name.ToLower().Contains("building");
				}
				return true;
			}
		}

		public bool IsBanner => isBanner;

		public Vector3 Position
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return ((Matrix)(ref matrix)).Translation;
			}
		}

		public float Size => model.Size;

		public void ChangeModel(Model setNewModel)
		{
			model = setNewModel;
		}

		public LandscapeObject(Model setModel, Matrix setMatrix)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector();
			if (setModel == null)
			{
				throw new ArgumentNullException("setModel");
			}
			model = setModel;
			matrix = setMatrix;
			isBanner = model.Name.ToLower().Contains("banner") || model.Name.ToLower().Contains("sign");
		}

		public void Render()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			model.Render(matrix);
		}

		public void GenerateShadows()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			model.GenerateShadow(matrix);
		}

		public void UseShadows()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			model.UseShadow(matrix);
		}
	}

	private const int GridWidth = 257;

	private const int GridHeight = 257;

	private const float MapWidthFactor = 10f;

	private const float MapHeightFactor = 10f;

	private const float MapZScale = 300f;

	private const int MaxBrakeTrackVertices = 840;

	private const float RaiseBreakTracksAmount = 0.2f;

	private List<LandscapeObject> landscapeObjects;

	private List<LandscapeObject> nearTrackObjects;

	private LandscapeObject startLightObject;

	private Model[] landscapeModels;

	private TrackCombiModels[] combos;

	internal string[] autoGenerationNames;

	private RacingGameManager.Level level;

	private TangentVertex[] vertices;

	private Material mat;

	private Material cityMat;

	private PlaneRenderer cityPlane;

	private VertexBuffer vertexBuffer;

	private IndexBuffer indexBuffer;

	private float[,] mapHeights;

	private Track track;

	private Replay bestReplay;

	private Replay newReplay;

	private List<TangentVertex> brakeTracksVertices;

	private TangentVertex[] brakeTracksVerticesArray;

	private Vector3 lastAddedTrackPos;

	public Material CityMaterial => cityMat;

	public Replay NewReplay => newReplay;

	public string CurrentTrackName => level.ToString();

	public float TrackLength => track.Length;

	public List<int> CheckpointSegmentPositions => track.CheckpointSegmentPositions;

	public Replay BestReplay => bestReplay;

	public void ReplaceStartLightObject(int number)
	{
		if (number < 0 || number >= 3)
		{
			number = 0;
		}
		if (startLightObject != null)
		{
			if (number == 2)
			{
				Sound.Play(Sound.Sounds.Bleep);
			}
			else
			{
				Sound.Play(Sound.Sounds.Beep);
			}
			startLightObject.ChangeModel(landscapeModels[number]);
		}
	}

	public void KillAllLoadedObjects()
	{
		landscapeObjects.Clear();
		nearTrackObjects.Clear();
		startLightObject = null;
	}

	public void AddObjectToRender(string modelName, Matrix renderMatrix, bool isNearTrackForShadowGeneration)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		switch (modelName)
		{
		case "OilWell":
			modelName = "OilPump";
			break;
		case "PalmSmall":
			modelName = "AlphaPalmSmall";
			break;
		case "AlphaPalm4":
			modelName = "AlphaPalmSmall";
			break;
		case "Palm":
			modelName = "AlphaPalm";
			break;
		case "Casino":
			modelName = "Casino01";
			break;
		case "Combi":
			modelName = "CombiPalms";
			break;
		}
		if (modelName.ToLower() == "windmill" || modelName.ToLower().Contains("hotel") || modelName.ToLower().Contains("building") || modelName.ToLower().Contains("casino01"))
		{
			isNearTrackForShadowGeneration = true;
		}
		for (int i = 0; i < combos.Length; i++)
		{
			TrackCombiModels trackCombiModels = combos[i];
			if (trackCombiModels.Name == modelName)
			{
				trackCombiModels.AddAllModels(this, renderMatrix);
				return;
			}
		}
		Model model = null;
		for (int j = 0; j < landscapeModels.Length; j++)
		{
			Model model2 = landscapeModels[j];
			if (model2.Name == modelName)
			{
				model = model2;
				break;
			}
		}
		if (model == null)
		{
			return;
		}
		Vector3 translation = ((Matrix)(ref renderMatrix)).Translation;
		float mapHeight = GetMapHeight(translation.X, translation.Y);
		if (translation.Z < mapHeight)
		{
			translation.Z = mapHeight;
			((Matrix)(ref renderMatrix)).Translation = translation;
		}
		if (!modelName.StartsWith("Banner") && !modelName.StartsWith("Sign") && !modelName.StartsWith("StartLight"))
		{
			for (int k = 0; k < landscapeObjects.Count; k++)
			{
				if (Vector3.DistanceSquared(landscapeObjects[k].Position, translation) < model.Size * model.Size / 4f)
				{
					return;
				}
			}
		}
		LandscapeObject item = new LandscapeObject(model, Matrix.CreateScale(1.2f) * renderMatrix);
		landscapeObjects.Add(item);
		if (isNearTrackForShadowGeneration)
		{
			nearTrackObjects.Add(item);
		}
		if (modelName.StartsWith("StartLight"))
		{
			startLightObject = item;
		}
	}

	public void AddObjectToRender(string modelName, float rotation, Vector3 trackPos, Vector3 trackRight, float distance)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		for (int i = 0; i < combos.Length; i++)
		{
			TrackCombiModels trackCombiModels = combos[i];
			if (trackCombiModels.Name == modelName)
			{
				num = trackCombiModels.Size;
				break;
			}
		}
		for (int j = 0; j < landscapeModels.Length; j++)
		{
			Model model = landscapeModels[j];
			if (model.Name == modelName)
			{
				num = model.Size;
				break;
			}
		}
		if (distance > 0f && distance - 10f < num)
		{
			distance += num;
		}
		if (distance < 0f && distance + 10f > 0f - num)
		{
			distance -= num;
		}
		AddObjectToRender(modelName, Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(trackPos + trackRight * distance + new Vector3(0f, 0f, -100f)), isNearTrackForShadowGeneration: false);
	}

	public void AddObjectToRender(string modelName, Vector3 renderPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		AddObjectToRender(modelName, Matrix.CreateTranslation(renderPos), isNearTrackForShadowGeneration: false);
	}

	public int CompareCheckpointTime(int checkpointNum)
	{
		if (bestReplay == null || checkpointNum >= bestReplay.CheckpointTimes.Count)
		{
			return 0;
		}
		float num = RacingGameManager.Player.GameTimeMilliseconds - bestReplay.CheckpointTimes[checkpointNum] * 1000f;
		return (int)num;
	}

	public void StartNewLap()
	{
		float num = RacingGameManager.Player.GameTimeMilliseconds / 1000f;
		Highscores.SubmitHighscore((int)level, (int)RacingGameManager.Player.GameTimeMilliseconds);
		RacingGameManager.Player.AddLapTime(num);
		if (num < bestReplay.LapTime)
		{
			RacingGameManager.Landscape.NewReplay.CheckpointTimes.Add(num);
			newReplay.LapTime = num;
			ThreadPool.QueueUserWorkItem(SaveReplay, (Replay)newReplay.Clone());
			bestReplay = newReplay;
		}
		newReplay = new Replay((int)level, createNew: true, track);
	}

	private void SaveReplay(object replay)
	{
		((Replay)replay).Save();
	}

	public float GetMapHeight(int x, int y)
	{
		if (x < 0)
		{
			x = 0;
		}
		if (y < 0)
		{
			y = 0;
		}
		if (x >= 257)
		{
			x = 256;
		}
		if (y >= 257)
		{
			y = 256;
		}
		return mapHeights[x, y];
	}

	private static int ModulateValueInRange(float val, int max)
	{
		if (val < 0f)
		{
			return max - 1 - (int)(0f - val) % max;
		}
		return (int)val % max;
	}

	public float GetMapHeight(float x, float y)
	{
		x /= 10f;
		y /= 10f;
		int num = ModulateValueInRange(x, 256);
		int num2 = ModulateValueInRange(y, 256);
		float num3 = x - (float)(int)x;
		float num4 = y - (float)(int)y;
		int num5 = (num + 1) % 256;
		int num6 = (num2 + 1) % 256;
		if (num3 + num4 < 1f)
		{
			return mapHeights[num, num2] + num3 * (mapHeights[num5, num2] - mapHeights[num, num2]) + num4 * (mapHeights[num, num6] - mapHeights[num, num2]);
		}
		return mapHeights[num5, num6] + (1f - num4) * (mapHeights[num5, num2] - mapHeights[num5, num6]) + (1f - num3) * (mapHeights[num, num6] - mapHeights[num5, num6]);
	}

	internal Landscape(RacingGameManager.Level setLevel)
	{
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0902: Expected O, but got Unknown
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Expected O, but got Unknown
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
		landscapeObjects = new List<LandscapeObject>();
		nearTrackObjects = new List<LandscapeObject>();
		landscapeModels = new Model[53]
		{
			new Model("StartLight"),
			new Model("StartLight2"),
			new Model("StartLight3"),
			new Model("Blockade"),
			new Model("Blockade2"),
			new Model("Hydrant"),
			new Model("Kaktus"),
			new Model("Kaktus2"),
			new Model("KaktusBenny"),
			new Model("KaktusSeg"),
			new Model("AlphaDeadTree"),
			new Model("AlphaPalm"),
			new Model("AlphaPalm2"),
			new Model("AlphaPalm3"),
			new Model("AlphaPalmSmall"),
			new Model("Laterne"),
			new Model("Laterne2Sides"),
			new Model("Trashcan"),
			new Model("Roadsign"),
			new Model("Roadsign2"),
			new Model("Goal"),
			new Model("Building"),
			new Model("Building2"),
			new Model("Building3"),
			new Model("Building4"),
			new Model("Building5"),
			new Model("OilPump"),
			new Model("OilTanks"),
			new Model("RoadColumnSegment"),
			new Model("Windmill"),
			new Model("Ruin"),
			new Model("RuinHouse"),
			new Model("SandCastle"),
			new Model("Banner"),
			new Model("Banner2"),
			new Model("Banner3"),
			new Model("Banner4"),
			new Model("Banner5"),
			new Model("Banner6"),
			new Model("Sign"),
			new Model("Sign2"),
			new Model("SignWarning"),
			new Model("SignCurveLeft"),
			new Model("SignCurveRight"),
			new Model("SharpRock"),
			new Model("SharpRock2"),
			new Model("Stone4"),
			new Model("Stone5"),
			new Model("AlphaTrain"),
			new Model("GuardRailHolder"),
			new Model("Hotel01"),
			new Model("Hotel02"),
			new Model("Casino01")
		};
		combos = new TrackCombiModels[10]
		{
			new TrackCombiModels("CombiPalms"),
			new TrackCombiModels("CombiPalms2"),
			new TrackCombiModels("CombiRuins"),
			new TrackCombiModels("CombiRuins2"),
			new TrackCombiModels("CombiStones"),
			new TrackCombiModels("CombiStones2"),
			new TrackCombiModels("CombiOilTanks"),
			new TrackCombiModels("CombiSandCastle"),
			new TrackCombiModels("CombiBuildings"),
			new TrackCombiModels("CombiHotels")
		};
		autoGenerationNames = new string[30]
		{
			"CombiPalms", "CombiPalms2", "CombiRuins", "CombiRuins2", "CombiStones", "CombiStones2", "Kaktus", "Kaktus2", "KaktusBenny", "KaktusSeg",
			"AlphaDeadTree", "AlphaPalm", "AlphaPalm2", "AlphaPalm3", "AlphaPalmSmall", "Laterne2Sides", "Trashcan", "OilPump", "OilTanks", "RoadColumnSegment",
			"Windmill", "Ruin", "RuinHouse", "Sign", "Sign2", "SharpRock", "SharpRock2", "Stone4", "Stone5", "Casino01"
		};
		vertices = new TangentVertex[66049];
		mat = new Material(new Color((byte)88, (byte)88, (byte)88), new Color((byte)234, (byte)234, (byte)234), new Color((byte)33, (byte)33, (byte)33), "Landscape", "LandscapeNormal", "", "LandscapeDetail");
		cityMat = new Material(new Color((byte)32, (byte)32, (byte)32), new Color((byte)200, (byte)200, (byte)200), new Color((byte)128, (byte)128, (byte)128), "CityGround", "CityGroundNormal", "", "");
		brakeTracksVertices = new List<TangentVertex>();
		lastAddedTrackPos = new Vector3(-1000f, -1000f, -1000f);
		base._002Ector();
		FileStream fileStream = FileHelper.LoadGameContentFile("Content\\LandscapeHeights.data");
		byte[] array = new byte[66049];
		fileStream.Read(array, 0, 66049);
		fileStream.Close();
		mapHeights = new float[257, 257];
		for (int i = 0; i < 257; i++)
		{
			for (int j = 0; j < 257; j++)
			{
				int num = i + j * 257;
				Vector3 val = CalcLandscapePos(i, j, array);
				mapHeights[i, j] = val.Z;
				vertices[num].pos = val;
				Vector3 val2 = val - CalcLandscapePos(i, j + 1, array);
				Vector3 val3 = val - CalcLandscapePos(i + 1, j, array);
				Vector3 val4 = val - CalcLandscapePos(i - 1, j + 1, array);
				Vector3 val5 = val - CalcLandscapePos(i + 1, j + 1, array);
				Vector3 val6 = val - CalcLandscapePos(i - 1, j - 1, array);
				vertices[num].normal = Vector3.Normalize(Vector3.Cross(val3, val2) + Vector3.Cross(val5, val4) + Vector3.Cross(val4, val6));
				vertices[num].tangent = Vector3.Normalize(val2);
				vertices[num].uv = new Vector2((float)j / 256f, (float)i / 256f);
			}
		}
		Vector3[,] array2 = new Vector3[257, 257];
		for (int k = 0; k < 257; k++)
		{
			for (int l = 0; l < 257; l++)
			{
				int num2 = k + l * 257;
				ref Vector3 reference = ref array2[k, l];
				reference = vertices[num2].normal;
			}
		}
		for (int m = 1; m < 256; m++)
		{
			for (int n = 1; n < 256; n++)
			{
				int num3 = m + n * 257;
				Vector3 val7 = vertices[num3].normal * 4f;
				for (int num4 = -1; num4 <= 1; num4++)
				{
					for (int num5 = -1; num5 <= 1; num5++)
					{
						val7 += array2[m + num4, n + num5];
					}
				}
				vertices[num3].normal = Vector3.Normalize(val7);
				Vector3 val8 = Vector3.Cross(vertices[num3].normal, vertices[num3].tangent);
				vertices[num3].tangent = Vector3.Cross(val8, vertices[num3].normal);
			}
		}
		vertexBuffer = new VertexBuffer(BaseGame.Device, typeof(TangentVertex), vertices.Length, (BufferUsage)8);
		vertexBuffer.SetData<TangentVertex>(vertices);
		uint[] array3 = new uint[393216];
		int num6 = 0;
		for (int num7 = 0; num7 < 256; num7++)
		{
			for (int num8 = 0; num8 < 256; num8++)
			{
				array3[num6] = (uint)(num7 * 257 + num8);
				array3[num6 + 2] = (uint)((num7 + 1) * 257 + (num8 + 1));
				array3[num6 + 1] = (uint)((num7 + 1) * 257 + num8);
				array3[num6 + 3] = (uint)((num7 + 1) * 257 + (num8 + 1));
				array3[num6 + 5] = (uint)(num7 * 257 + num8);
				array3[num6 + 4] = (uint)(num7 * 257 + (num8 + 1));
				num6 += 6;
			}
		}
		indexBuffer = new IndexBuffer(BaseGame.Device, typeof(uint), 393216, (BufferUsage)8);
		indexBuffer.SetData<uint>(array3);
		ReloadLevel(setLevel);
		foreach (LandscapeObject landscapeObject in landscapeObjects)
		{
			if (landscapeObject.IsBigBuilding)
			{
				cityPlane = new PlaneRenderer(landscapeObject.Position, new Plane(new Vector3(0f, 0f, 1f), 0.1f), cityMat, Math.Min(landscapeObject.Position.X, landscapeObject.Position.Y));
				break;
			}
		}
	}

	internal void ReloadLevel(RacingGameManager.Level setLevel)
	{
		level = setLevel;
		if (track == null)
		{
			track = new Track("Track" + level, this);
		}
		else
		{
			track.Reload("Track" + level, this);
		}
		bestReplay = new Replay((int)level, createNew: false, track);
		newReplay = new Replay((int)level, createNew: true, track);
		brakeTracksVertices.Clear();
		brakeTracksVerticesArray = null;
		SetCarToStartPosition();
		startLightObject.ChangeModel(landscapeModels[0]);
	}

	private static Vector3 CalcLandscapePos(int x, int y, byte[] heights)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		int num = ((x >= 0) ? ((x >= 257) ? 256 : x) : 0);
		int num2 = ((y >= 0) ? ((y >= 257) ? 256 : y) : 0);
		float num3 = (float)(int)heights[num + num2 * 257] / 255f;
		return new Vector3((float)x * 10f, (float)y * 10f, num3 * 300f);
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
			for (int i = 0; i < landscapeModels.Length; i++)
			{
				landscapeModels[i].Dispose();
			}
			mat.Dispose();
			cityMat.Dispose();
			((GraphicsResource)vertexBuffer).Dispose();
			((GraphicsResource)indexBuffer).Dispose();
			track.Dispose();
		}
	}

	public void SetCarToStartPosition()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		RacingGameManager.Player.SetCarPosition(track.StartPosition, track.StartDirection, track.StartUpVector);
	}

	public void Render()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Device.RenderState.DepthBufferEnable = true;
		BaseGame.Device.RenderState.DepthBufferWriteEnable = true;
		BaseGame.WorldMatrix = Matrix.Identity;
		ShaderEffect.landscapeNormalMapping.Render(mat, "DiffuseWithDetail20", RenderLandscapeVertices);
		cityPlane.Render();
		track.Render();
		for (int i = 0; i < landscapeObjects.Count; i++)
		{
			landscapeObjects[i].Render();
		}
		RenderBrakeTracks();
	}

	private void RenderLandscapeVertices()
	{
		BaseGame.Device.VertexDeclaration = TangentVertex.VertexDeclaration;
		BaseGame.Device.Vertices[0].SetSource(vertexBuffer, 0, TangentVertex.SizeInBytes);
		BaseGame.Device.Indices = indexBuffer;
		BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, 0, 0, 66049, 0, 131072);
	}

	public void GenerateShadow()
	{
		track.GenerateShadow();
		for (int i = 0; i < nearTrackObjects.Count; i++)
		{
			nearTrackObjects[i].GenerateShadows();
		}
	}

	public void UseShadow()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ShaderEffect.shadowMapping.UpdateCalcShadowWorldMatrix(Matrix.Identity);
		RenderLandscapeVertices();
		if (BaseGame.HighDetail)
		{
			for (int i = 0; i < nearTrackObjects.Count; i++)
			{
				if (!nearTrackObjects[i].IsBanner)
				{
					nearTrackObjects[i].UseShadows();
				}
			}
		}
		track.UseShadow();
	}

	public Matrix GetTrackPositionMatrix(float carTrackPos, out float roadWidth, out float nextRoadWidth)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return track.GetTrackPositionMatrix(carTrackPos, out roadWidth, out nextRoadWidth);
	}

	public Matrix GetTrackPositionMatrix(int trackSegmentNum, float trackSegmentPercent, out float roadWidth, out float nextRoadWidth)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return track.GetTrackPositionMatrix(trackSegmentNum, trackSegmentPercent, out roadWidth, out nextRoadWidth);
	}

	public void UpdateCarTrackPosition(Vector3 carPos, ref int trackSegmentNumber, ref float trackPositionPercent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		track.UpdateCarTrackPosition(carPos, ref trackSegmentNumber, ref trackPositionPercent);
	}

	public void AddBrakeTrack(CarPhysics car)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = car.CarPosition + car.CarDirection * 1.25f;
		if (Vector3.DistanceSquared(val, lastAddedTrackPos) < 0.024f || brakeTracksVertices.Count > 840)
		{
			return;
		}
		lastAddedTrackPos = val;
		float num = (float)Math.Sqrt(26.01000045776368) / 2f - 0.35f;
		for (int i = 0; i < brakeTracksVertices.Count; i++)
		{
			if (Vector3.DistanceSquared(brakeTracksVertices[i].pos, val) < num * num)
			{
				return;
			}
		}
		val += Vector3.Normalize(car.CarUpVector) * 0.2f;
		TangentVertex[] collection = new TangentVertex[6]
		{
			new TangentVertex(val - car.CarRight * 2.4f / 2f - car.CarDirection * 4.5f / 2f, 0f, 0f, car.CarUpVector, car.CarRight),
			new TangentVertex(val - car.CarRight * 2.4f / 2f + car.CarDirection * 4.5f / 2f, 0f, 5f, car.CarUpVector, car.CarRight),
			new TangentVertex(val + car.CarRight * 2.4f / 2f + car.CarDirection * 4.5f / 2f, 1f, 5f, car.CarUpVector, car.CarRight),
			new TangentVertex(val - car.CarRight * 2.4f / 2f - car.CarDirection * 4.5f / 2f, 0f, 0f, car.CarUpVector, car.CarRight),
			new TangentVertex(val + car.CarRight * 2.4f / 2f + car.CarDirection * 4.5f / 2f, 1f, 5f, car.CarUpVector, car.CarRight),
			new TangentVertex(val + car.CarRight * 2.4f / 2f - car.CarDirection * 4.5f / 2f, 1f, 0f, car.CarUpVector, car.CarRight)
		};
		brakeTracksVertices.AddRange(collection);
		brakeTracksVerticesArray = brakeTracksVertices.ToArray();
	}

	public void RenderBrakeTracks()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (brakeTracksVerticesArray != null)
		{
			BaseGame.SetAlphaBlendingEnabled(value: true);
			BaseGame.WorldMatrix = Matrix.Identity;
			BaseGame.Device.VertexDeclaration = TangentVertex.VertexDeclaration;
			ShaderEffect.lighting.Render(RacingGameManager.BrakeTrackMaterial, "Diffuse20", delegate
			{
				BaseGame.Device.DrawUserPrimitives<TangentVertex>((PrimitiveType)4, brakeTracksVerticesArray, 0, brakeTracksVerticesArray.Length / 3);
			});
		}
	}
}
