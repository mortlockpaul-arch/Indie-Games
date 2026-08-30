using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class RoadTrack : ObjDrawUpdate
{
	public enum BankingCenter
	{
		Center,
		Left,
		Right
	}

	public class Area
	{
		public readonly Vector3 PLow0;

		public readonly Vector3 PLow1;

		public readonly Vector3 PTop0;

		public readonly Vector3 PTop1;

		public readonly Vector3 Normal0;

		public readonly Vector3 Normal1;

		private readonly Vector3 mean0;

		private readonly Vector3 mean1;

		private readonly Vector3 meanLow;

		private readonly Vector3 meanTop;

		private readonly float distance0to1;

		private readonly float distanceLowToTop;

		private readonly MyMath.Triangle triangleA;

		private readonly MyMath.Triangle triangleB;

		private float ratio0;

		private float ratioLow;

		private static int counter;

		public readonly int Id;

		public List<int> barrierIndexes;

		public float Ratio0 => ratio0;

		public float RatioLow => ratioLow;

		public Area(Vector3 low0, Vector3 low1, Vector3 top0, Vector3 top1, Vector3 normal0, Vector3 normal1)
		{
			PLow0 = low0;
			PLow1 = low1;
			PTop0 = top0;
			PTop1 = top1;
			Normal0 = normal0;
			Normal1 = normal1;
			Id = counter++;
			ratioLow = 0f;
			ratio0 = 0f;
			triangleA = new MyMath.Triangle(low0, low1, top1);
			triangleB = new MyMath.Triangle(low0, top0, top1);
			mean0 = (low0 + top0) / 2f;
			mean1 = (low1 + top1) / 2f;
			meanTop = (top1 + top0) / 2f;
			meanLow = (low1 + low0) / 2f;
			distance0to1 = Vector3.Distance(mean0, mean1);
			distanceLowToTop = Vector3.Distance(meanLow, meanTop);
			barrierIndexes = new List<int>();
		}

		public bool Contains(Vector3 pos, out Vector3 interPoint)
		{
			if (!triangleA.IntersectLine(pos, pos + triangleA.Normal, out interPoint))
			{
				return triangleB.IntersectLine(pos, pos + triangleB.Normal, out interPoint);
			}
			return true;
		}

		public Vector3 GetNormal(Vector3 pos)
		{
			float num = Vector3.Distance(MyMath.Project3D(mean0, mean1, pos), mean0);
			ratio0 = num / distance0to1;
			float num2 = Vector3.Distance(MyMath.Project3D(meanLow, meanTop, pos), meanLow);
			ratioLow = num2 / distanceLowToTop;
			return Utils.LerpVector3(Normal0, Normal1, ratio0);
		}

		public void AddBarrierIndex(int barrierLow, int barrierTop)
		{
			barrierIndexes.Add(barrierLow);
			barrierIndexes.Add(barrierTop);
		}

		public void DrawBarriersBox()
		{
			foreach (int barrierIndex in barrierIndexes)
			{
				if ((barrierIndex % 2 == 0) ? (RatioLow < 0.5f) : (RatioLow >= 0.5f))
				{
					Instance.barrierBoxDebug.Draw(Instance.BarrierCollisionMatrixes[barrierIndex]);
				}
			}
		}

		public void Draw()
		{
		}
	}

	private const string NameBoneBarrier = "Barrier";

	private Texture2D roadTex;

	private BasicEffect effect;

	private VertexPositionNormalTexture[] trackVertices;

	private float TotalLength;

	private SplineTraj baseSpline;

	private Model barrierModel;

	private readonly float barrierLength;

	private readonly float barrierWidth;

	private Matrix[] BarrierDisplayMatrixes;

	public Drawing3D_V2.BoundingBoxTransformable barrierBox;

	public Matrix[] BarrierCollisionMatrixes;

	public readonly BankingCenter bankingCenter;

	public CurveRelationshipSplineControl bankingValues;

	private Debug3D.LineList barrierBoxDebug;

	private Area[] Areas;

	private readonly float areaLen;

	public static RoadTrack Instance;

	private bool HasBarrier => barrierModel != null;

	public int AreasCount => Areas.Length;

	public RoadTrack(Texture2D roadTex, List<Vector3> trackPoints, float halfTrackWidth, List<Vector2> banking, BankingCenter bankingCenter, Model barrierModel, float areaLen)
		: this(roadTex, trackPoints, halfTrackWidth, banking, bankingCenter, barrierModel, areaLen, useAutoDraw: true, Drawing3D_V2.NewDefaultEffect)
	{
	}

	public RoadTrack(Texture2D roadTex, List<Vector3> trackPoints, float halfTrackWidth, List<Vector2> banking, BankingCenter bankingCenter, Model barrierModel, float areaLen, bool useAutoDraw, BasicEffect effect)
		: base(useAutoDraw, useAutoDraw: true)
	{
		if (Instance != null)
		{
			throw new Exception("only one instance supported");
		}
		Instance = this;
		this.roadTex = roadTex;
		this.effect = effect;
		this.effect.World = Matrix.Identity;
		this.effect.TextureEnabled = true;
		this.effect.Texture = this.roadTex;
		this.bankingCenter = bankingCenter;
		this.barrierModel = barrierModel;
		this.areaLen = areaLen;
		barrierBox = new Drawing3D_V2.BoundingBoxTransformable(barrierModel.Meshes["Barrier"]);
		barrierLength = Math.Abs(barrierBox.OriginalBox.Min.Y - barrierBox.OriginalBox.Max.Y);
		barrierWidth = Math.Abs(barrierBox.OriginalBox.Min.X - barrierBox.OriginalBox.Max.X);
		barrierBoxDebug = new Debug3D.LineList(barrierBox.DrawingData(showDiagonals: true), Color.Gold);
		Reset(trackPoints, halfTrackWidth, banking);
	}

	public void Reset(List<Vector3> trackPoints, float halfTrackWidth, List<Vector2> banking)
	{
		GenerateTrackPoints(trackPoints);
		trackVertices = GenerateTrackVertices(halfTrackWidth, banking);
	}

	public override void Update(GameTime gameTime)
	{
	}

	private void GenerateTrackPoints(List<Vector3> basePoints)
	{
		basePoints.Add(basePoints[0]);
		basePoints.Add(basePoints[1]);
		basePoints.Add(basePoints[2]);
		baseSpline = new SplineTraj(basePoints);
		TotalLength = baseSpline.Length;
	}

	private float SideOffset(bool isInnerPoint)
	{
		return bankingCenter switch
		{
			BankingCenter.Center => (!isInnerPoint) ? 1 : (-1), 
			BankingCenter.Left => isInnerPoint ? (-2) : 0, 
			BankingCenter.Right => (!isInnerPoint) ? 2 : 0, 
			_ => throw new NotImplementedException(), 
		};
	}

	private VertexPositionNormalTexture[] GenerateTrackVertices(float halfTrackWidth, List<Vector2> banking)
	{
		bankingValues = new CurveRelationshipSplineControl(CurveRelationshipSplineControl.Mode.SmoothStep, banking);
		int num = (int)(TotalLength / areaLen);
		List<VertexPositionNormalTexture> list = new List<VertexPositionNormalTexture>();
		float num2 = halfTrackWidth * 2f * (float)roadTex.Width / (float)roadTex.Height;
		float num3 = 0f;
		Areas = new Area[num];
		int num4 = 0;
		for (int i = 0; i < num + 1; i++)
		{
			float num5 = (float)i / (float)num;
			float ratio = (float)(i + 1) / (float)num % 1f;
			float num6 = (float)(i - 1) / (float)num;
			if (num6 < 0f)
			{
				num6++;
			}
			Vector3 byRatio = baseSpline.GetByRatio(num5);
			Vector3 vector = baseSpline.GetByRatio(ratio) - byRatio;
			Vector3 vector2 = byRatio - baseSpline.GetByRatio(num6);
			vector2.Normalize();
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			Vector3.Cross(vector, vector3);
			Vector3 vector4 = Utils.LerpVector3(Vector3.Up, Vector3.Cross(Vector3.Up, Vector3.Normalize(vector)), bankingValues.Value(num5));
			Vector3 vector5 = Vector3.Cross(vector4, Vector3.Normalize(vector));
			Vector3 position = byRatio + vector5 * halfTrackWidth * SideOffset(isInnerPoint: true);
			Vector3 position2 = byRatio + vector5 * halfTrackWidth * SideOffset(isInnerPoint: false);
			VertexPositionNormalTexture item = new VertexPositionNormalTexture(position, vector4, new Vector2(0f, num3 / num2));
			list.Add(item);
			item = new VertexPositionNormalTexture(position2, vector4, new Vector2(1f, num3 / num2));
			list.Add(item);
			num3 += vector.Length();
			if (i > 0)
			{
				int num7 = list.Count - 1;
				Areas[num4++] = new Area(list[num7 - 2].Position, list[num7].Position, list[num7 - 3].Position, list[num7 - 1].Position, list[num7 - 3].Normal, list[num7].Normal);
			}
		}
		int num8 = Areas.Length;
		List<Matrix> list2 = new List<Matrix>();
		List<Matrix> list3 = new List<Matrix>();
		for (int j = 0; j < num8; j++)
		{
			if (j > 0 && j % 25 == 0)
			{
				Area area = Areas[(int)((float)(j - 25) / (float)num8 * (float)Areas.Length)];
				Area area2 = Areas[(int)((float)j / (float)num8 * (float)Areas.Length) % Areas.Length];
				bool flag = j + 25 >= num8;
				if (flag)
				{
					area2 = Areas[0];
				}
				list3.AddRange(BarrierTransform(area, area2));
				for (int k = area.Id; k < (flag ? Areas.Length : area2.Id); k++)
				{
					Areas[k].AddBarrierIndex(list3.Count - 2, list3.Count - 1);
					if (k > (area.Id + (flag ? Areas.Length : area2.Id)) / 2)
					{
						Areas[k].AddBarrierIndex(list3.Count, list3.Count + 1);
					}
					else
					{
						Areas[k].AddBarrierIndex(list3.Count - 3, list3.Count - 4);
					}
				}
			}
			Area areaForBarrier = Areas[(int)((float)j / (float)num8 * (float)Areas.Length)];
			Area areaForBarrier2 = Areas[(int)((float)(j + 1) / (float)num8 * (float)Areas.Length) % Areas.Length];
			list2.AddRange(BarrierTransform(areaForBarrier, areaForBarrier2));
		}
		BarrierDisplayMatrixes = list2.ToArray();
		BarrierCollisionMatrixes = list3.ToArray();
		Area[] areas = Areas;
		foreach (Area area3 in areas)
		{
			for (int m = 0; m < area3.barrierIndexes.Count; m++)
			{
				int num9 = area3.barrierIndexes[m] % BarrierCollisionMatrixes.Length;
				if (num9 < 0)
				{
					num9 += BarrierCollisionMatrixes.Length;
				}
				area3.barrierIndexes[m] = num9;
			}
		}
		return list.ToArray();
	}

	public List<Matrix> BarrierTransform(Area AreaForBarrier0, Area AreaForBarrier1)
	{
		List<Matrix> list = new List<Matrix>();
		Vector3 pLow = AreaForBarrier0.PLow0;
		Vector3 pLow2 = AreaForBarrier1.PLow0;
		Vector3 vector = (AreaForBarrier0.Normal0 + AreaForBarrier1.Normal0) / 2f;
		Vector3 vector2 = pLow2 - pLow;
		Vector3.Cross(vector, Vector3.Normalize(vector2));
		Matrix identity = Matrix.Identity;
		identity.Up = vector;
		identity.Forward = vector2;
		identity.Left = Vector3.Cross(identity.Up, Vector3.Normalize(identity.Forward));
		list.Add(identity * Matrix.CreateTranslation(pLow + identity.Forward / 2f));
		pLow = AreaForBarrier0.PTop0;
		pLow2 = AreaForBarrier1.PTop0;
		vector = (AreaForBarrier0.Normal0 + AreaForBarrier1.Normal0) / 2f;
		vector2 = pLow2 - pLow;
		Vector3.Cross(vector, Vector3.Normalize(vector2));
		identity = Matrix.Identity;
		identity.Up = vector;
		identity.Backward = vector2;
		identity.Left = Vector3.Cross(identity.Up, Vector3.Normalize(identity.Forward));
		list.Add(identity * Matrix.CreateTranslation(pLow - identity.Forward / 2f));
		return list;
	}

	public Area FindArea(Vector3 pos, int previousAreaIndex, out Vector3 intersectionPoint)
	{
		for (int i = 0; i < Areas.Length; i++)
		{
			Area area = Areas[(i + previousAreaIndex) % Areas.Length];
			if (area.Contains(pos, out intersectionPoint))
			{
				return area;
			}
		}
		intersectionPoint = pos;
		return null;
	}

	public static VertexPositionColor[] VerticesFromVector3List(List<Vector3> pointList, Color color)
	{
		VertexPositionColor[] array = new VertexPositionColor[pointList.Count];
		int num = 0;
		foreach (Vector3 point in pointList)
		{
			ref VertexPositionColor reference = ref array[num++];
			reference = new VertexPositionColor(point, color);
		}
		return array;
	}

	public override void Draw(GameTime gameTime)
	{
		Drawing3D_V2.DrawModelHWInstances(barrierModel, BarrierDisplayMatrixes);
		effect.View = MaximinusGame.Instance.Camera.View;
		effect.Projection = MaximinusGame.Instance.Camera.Proj;
		foreach (EffectPass pass in effect.CurrentTechnique.Passes)
		{
			pass.Apply();
			MaximinusGame.Draw2D.Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, trackVertices, 0, trackVertices.Length - 2);
		}
	}
}
