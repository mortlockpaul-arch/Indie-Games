using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;
using RacingGame.Landscapes;
using RacingGame.Shaders;

namespace RacingGame.Tracks;

internal class TrackColumns : IDisposable
{
	private const float ColumnsDistance = 33f;

	private const float ColumnGroundHeight = 1f;

	private const float MinimumColumnHeight = 2.5f;

	private const float TopColumnSubHeight = 0.55f;

	private readonly TangentVertex[] BaseColumnVertices;

	private List<Vector3> columnPositions;

	private TangentVertex[] columnVertices;

	private VertexBuffer columnVb;

	private IndexBuffer columnIb;

	public TrackColumns(List<TrackVertex> points, Landscape landscape)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Expected O, but got Unknown
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Expected O, but got Unknown
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		BaseColumnVertices = new TangentVertex[7]
		{
			new TangentVertex(new Vector3(1f, 0f, 0f), new Vector2(0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f)),
			new TangentVertex(new Vector3(0.5f, 0.866025f, 0f), new Vector2(1f / 6f, 0f), new Vector3(0.5f, 0.866025f, 0f), new Vector3(0f, 0f, -1f)),
			new TangentVertex(new Vector3(-0.5f, 0.866025f, 0f), new Vector2(1f / 3f, 0f), new Vector3(-0.5f, 0.866025f, 0f), new Vector3(0f, 0f, -1f)),
			new TangentVertex(new Vector3(-1f, 0f, 0f), new Vector2(0.5f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(0f, 0f, -1f)),
			new TangentVertex(new Vector3(-0.5f, -0.866025f, 0f), new Vector2(2f / 3f, 0f), new Vector3(-0.5f, -0.866025f, 0f), new Vector3(0f, 0f, -1f)),
			new TangentVertex(new Vector3(0.5f, -0.866025f, 0f), new Vector2(5f / 6f, 0f), new Vector3(0.5f, -0.866025f, 0f), new Vector3(0f, 0f, -1f)),
			new TangentVertex(new Vector3(1f, 0f, 0f), new Vector2(1f, 0f), new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f))
		};
		columnPositions = new List<Vector3>();
		base._002Ector();
		if (landscape == null)
		{
			return;
		}
		float num = 33f;
		List<Matrix> list = new List<Matrix>();
		List<Matrix> list2 = new List<Matrix>();
		Vector3 val = default(Vector3);
		for (int i = 0; i < points.Count; i++)
		{
			float num2 = Vector3.Distance(points[(i + 1) % points.Count].pos, points[i].pos);
			if (num - num2 <= 0f)
			{
				Vector3 pos = points[(i - 1 < 0) ? (points.Count - 1) : (i - 1)].pos;
				Vector3 pos2 = points[i].pos;
				Vector3 pos3 = points[(i + 1) % points.Count].pos;
				Vector3 pos4 = points[(i + 2) % points.Count].pos;
				Vector3 item = Vector3.CatmullRom(pos, pos2, pos3, pos4, num / num2);
				float num3 = Vector3.Dot(points[i].up, new Vector3(0f, 0f, 1f));
				float num4 = item.Z - landscape.GetMapHeight(item.X, item.Y);
				if (num3 > 0.3f && num4 > 2.5f)
				{
					columnPositions.Add(item);
					Vector3 right = points[i].right;
					Vector3 dir = points[i].dir;
					Vector3 up = points[i].up;
					Matrix identity = Matrix.Identity;
					identity.M11 = right.X;
					identity.M12 = right.Y;
					identity.M13 = right.Z;
					identity.M21 = dir.X;
					identity.M22 = dir.Y;
					identity.M23 = dir.Z;
					identity.M31 = up.X;
					identity.M32 = up.Y;
					identity.M33 = up.Z;
					list.Add(identity);
					identity = Matrix.Identity;
					((Vector3)(ref val))._002Ector(0f, 0f, 1f);
					Vector3 val2 = Vector3.Cross(dir, val);
					identity.M11 = val2.X;
					identity.M12 = val2.Y;
					identity.M13 = val2.Z;
					identity.M21 = dir.X;
					identity.M22 = dir.Y;
					identity.M23 = dir.Z;
					list2.Add(identity);
				}
				num += 33f;
			}
			num -= num2;
		}
		columnVertices = new TangentVertex[columnPositions.Count * BaseColumnVertices.Length * 2];
		Vector3 val4 = default(Vector3);
		Vector3 val5 = default(Vector3);
		for (int j = 0; j < columnPositions.Count; j++)
		{
			Vector3 val3 = columnPositions[j];
			((Vector3)(ref val4))._002Ector(val3.X, val3.Y, landscape.GetMapHeight(val3.X, val3.Y) + 1f);
			((Vector3)(ref val5))._002Ector(val3.X, val3.Y, val3.Z - 0.55f);
			float num5 = Vector3.Distance(val5, val4) / ((float)Math.PI * 2f);
			for (int k = 0; k < 2; k++)
			{
				for (int l = 0; l < BaseColumnVertices.Length; l++)
				{
					int num6 = j * BaseColumnVertices.Length * 2 + k * BaseColumnVertices.Length + l;
					Matrix val6 = ((k == 0) ? list2[j] : list[j]);
					ref TangentVertex reference = ref columnVertices[num6];
					reference = new TangentVertex(((k == 0) ? val4 : val5) + Vector3.Transform(BaseColumnVertices[l].pos, val6), BaseColumnVertices[l].U, (k == 0) ? 0f : num5, Vector3.Transform(BaseColumnVertices[l].normal, val6), Vector3.Transform(-BaseColumnVertices[l].tangent, val6));
				}
			}
			if (landscape != null && BaseGame.HighDetail)
			{
				landscape.AddObjectToRender("RoadColumnSegment", new Vector3(val4.X, val4.Y, val4.Z - 1f));
			}
		}
		columnVb = new VertexBuffer(BaseGame.Device, typeof(TangentVertex), columnVertices.Length, (BufferUsage)8);
		columnVb.SetData<TangentVertex>(columnVertices);
		int num7 = BaseColumnVertices.Length - 1;
		int[] array = new int[6 * num7 * columnPositions.Count];
		int num8 = 0;
		int num9 = 0;
		for (int m = 0; m < columnPositions.Count; m++)
		{
			for (int n = 0; n < num7; n++)
			{
				num9 = 6 * (m * num7 + n);
				array[num9] = num8 + n;
				array[num9 + 1] = num8 + 1 + BaseColumnVertices.Length + n;
				array[num9 + 2] = num8 + 1 + n;
				array[num9 + 3] = array[num9 + 1];
				array[num9 + 4] = array[num9];
				array[num9 + 5] = num8 + BaseColumnVertices.Length + n;
			}
			num8 += BaseColumnVertices.Length * 2;
		}
		columnIb = new IndexBuffer(BaseGame.Device, typeof(int), array.Length, (BufferUsage)8);
		columnIb.SetData<int>(array);
	}

	public void Dispose()
	{
		((GraphicsResource)columnVb).Dispose();
		((GraphicsResource)columnIb).Dispose();
	}

	public void Render(Material columnMaterial)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Device.VertexDeclaration = TangentVertex.VertexDeclaration;
		BaseGame.WorldMatrix = Matrix.Identity;
		ShaderEffect.normalMapping.Render(columnMaterial, "Specular20", RenderColumnVertices);
		BaseGame.WorldMatrix = Matrix.Identity;
	}

	private void RenderColumnVertices()
	{
		if (columnVertices != null)
		{
			BaseGame.Device.Vertices[0].SetSource(columnVb, 0, TangentVertex.SizeInBytes);
			BaseGame.Device.Indices = columnIb;
			BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, 0, 0, columnVertices.Length, 0, (BaseColumnVertices.Length - 1) * columnPositions.Count * 2);
		}
	}
}
