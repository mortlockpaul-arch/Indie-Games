using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class OluModel
{
	public string pathname;

	private VertexPositionColor[] vertices;

	private List<int[]> triangles;

	private List<int[]> rectangles;

	private List<Vector3> normals;

	private List<Vector3> facetnorms;

	private List<PlaneEffect> faces;

	private Vector3 position;

	public OluModel()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Initialize();
		pathname = "";
		position = default(Vector3);
	}

	public OluModel(string filename)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(filename, default(Vector3), Color.White);
	}

	public OluModel(string filename, Vector3 _pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(filename, _pos, Color.White);
	}

	public OluModel(string filename, Color _col)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(filename, default(Vector3), _col);
	}

	public OluModel(string filename, Vector3 _pos, Color _col)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Initialize();
		pathname = filename;
		position = _pos;
		readOBJ(filename, _col);
	}

	private void Initialize()
	{
		triangles = new List<int[]>();
		rectangles = new List<int[]>();
		normals = new List<Vector3>();
		facetnorms = new List<Vector3>();
	}

	public void readOBJ(string filename, Color _col)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		StreamReader streamReader = File.OpenText(filename);
		pathname = filename;
		secondPass(streamReader, _col);
		streamReader.Close();
	}

	private void secondPass(StreamReader file, Color _col)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		List<VertexPositionColor> list = new List<VertexPositionColor>();
		list.Add(new VertexPositionColor(default(Vector3), Color.White));
		string text;
		while ((text = file.ReadLine()) != null)
		{
			switch (text[0])
			{
			case 'v':
				if (text[1] == ' ')
				{
					string[] array = text.Split(' ');
					list.Add(new VertexPositionColor(new Vector3(float.Parse(array[1], CultureInfo.InvariantCulture), float.Parse(array[2], CultureInfo.InvariantCulture), float.Parse(array[3], CultureInfo.InvariantCulture)), _col));
				}
				break;
			case 'f':
			{
				string[] array = text.Split(' ');
				int num = int.Parse(array[1]);
				int num2 = int.Parse(array[2]);
				int num3 = int.Parse(array[3]);
				triangles.Add(new int[4] { num, num2, num3, num });
				if (array.Length == 5)
				{
					triangles.RemoveAt(0);
					rectangles.Add(new int[5]
					{
						num,
						num2,
						num3,
						int.Parse(array[4]),
						num
					});
				}
				break;
			}
			}
		}
		vertices = list.ToArray();
	}

	public void drawModel(int mode)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		switch (mode)
		{
		case 1:
		{
			BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
			if (faces.Count == 0)
			{
				GenerateSimpleFaceEffects(2, 0.3f, 0.1f, 0.2f, 0.15f, Color.Yellow);
			}
			for (int k = 0; k < faces.Count; k++)
			{
				faces[k].draw();
			}
			break;
		}
		case 2:
		{
			BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
			for (int i = 0; i < triangles.Count; i++)
			{
				BaseGame.Get().graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType)3, vertices, 0, vertices.Length, triangles[i], 0, 3);
			}
			for (int j = 0; j < rectangles.Count; j++)
			{
				BaseGame.Get().graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType)3, vertices, 0, vertices.Length, rectangles[j], 0, 4);
			}
			break;
		}
		}
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
	}

	public void GenerateSimpleFaceEffects(int density, float vel, float velRand, float side, float sideRand, Color colorCoord)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		Random random = new Random();
		faces = new List<PlaneEffect>();
		for (int i = 0; i < triangles.Count; i++)
		{
			PlaneEffect planeEffect = new PlaneEffect();
			for (int j = 0; j < density; j++)
			{
				TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, vel, velRand, side, sideRand);
				treeNode.branchTree = false;
				treeNode.setColor(colorCoord);
				planeEffect.addNode(treeNode);
			}
			ref Vector3 reference = ref planeEffect.cornerNodes[0];
			reference = vertices[triangles[i][0]].Position;
			ref Vector3 reference2 = ref planeEffect.cornerNodes[1];
			reference2 = vertices[triangles[i][1]].Position;
			ref Vector3 reference3 = ref planeEffect.cornerNodes[2];
			reference3 = vertices[triangles[i][2]].Position;
			ref Vector3 reference4 = ref planeEffect.cornerNodes[3];
			reference4 = vertices[triangles[i][2]].Position;
			planeEffect.iteratePlane();
			planeEffect.FinalizeEffect();
			faces.Add(planeEffect);
		}
		for (int k = 0; k < rectangles.Count; k++)
		{
			PlaneEffect planeEffect = new PlaneEffect();
			for (int l = 0; l < density; l++)
			{
				TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, vel, velRand, side, sideRand);
				treeNode.branchTree = false;
				treeNode.setColor(colorCoord);
				planeEffect.addNode(treeNode);
			}
			ref Vector3 reference5 = ref planeEffect.cornerNodes[0];
			reference5 = vertices[rectangles[k][0]].Position;
			ref Vector3 reference6 = ref planeEffect.cornerNodes[1];
			reference6 = vertices[rectangles[k][1]].Position;
			ref Vector3 reference7 = ref planeEffect.cornerNodes[2];
			reference7 = vertices[rectangles[k][3]].Position;
			ref Vector3 reference8 = ref planeEffect.cornerNodes[3];
			reference8 = vertices[rectangles[k][2]].Position;
			planeEffect.iteratePlane();
			planeEffect.FinalizeEffect();
			faces.Add(planeEffect);
		}
	}
}
