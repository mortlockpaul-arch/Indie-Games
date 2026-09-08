using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class LineMesh : UserVertexMesh<Quasar.Global.Vertex.VertexPositionColor>
{
	public struct Line(Vector3 from, Vector3 to, Color color)
	{
		public Vector3 from = from;

		public Vector3 to = to;

		public Color color = color;
	}

	private int lineCount;

	private Line[] lineList;

	private bool dirty;

	public int Count => lineCount;

	public Vector3 LastLineEnd
	{
		get
		{
			if (lineCount > 0)
			{
				return lineList[lineCount - 1].to;
			}
			return Vector3.Zero;
		}
	}

	public Line? this[int index]
	{
		get
		{
			if (index >= 0 && index < lineCount)
			{
				return lineList[index];
			}
			return null;
		}
	}

	public void SetColor(Color c)
	{
		for (int i = 0; i < lineList.Length; i++)
		{
			lineList[i].color = c;
		}
		dirty = true;
	}

	public LineMesh(int lineCount)
	{
		base.PrimitiveType = PrimitiveType.LineList;
		base.Materials.Add(new Material());
		shader = ShaderManager.Shaders["VertexColor"];
		if (lineCount <= 0)
		{
			lineCount = 256;
		}
		lineList = new Line[lineCount];
		initMesh(lineCount * 2);
	}

	public LineMesh()
		: this(512)
	{
	}

	public void PushLine(Vector3 from, Vector3 to)
	{
		PushLine(from, to, Color.Green);
	}

	public void PushLine(Vector3 to)
	{
		PushLine(to, Color.Green);
	}

	public void PushLine(Vector3 to, Color color)
	{
		if (lineCount > 0)
		{
			PushLine(lineList[lineCount - 1].to, to, color);
		}
	}

	public void PushLine(Vector3 from, Vector3 to, Color color)
	{
		if (lineCount < lineList.Length)
		{
			ref Line reference = ref lineList[lineCount++];
			reference = new Line(from, to, color);
			dirty = true;
		}
	}

	public void PopLine()
	{
		if (lineCount > 0)
		{
			lineCount--;
			dirty = true;
		}
	}

	public void ClearLines()
	{
		lineCount = 0;
		dirty = true;
	}

	private void updateMesh()
	{
		if (lineCount > 0)
		{
			for (int i = 0; i < lineCount; i++)
			{
				Line line = lineList[i];
				ref Quasar.Global.Vertex.VertexPositionColor reference = ref verticesBuffer[i * 2];
				reference = new Quasar.Global.Vertex.VertexPositionColor(line.from, line.color);
				ref Quasar.Global.Vertex.VertexPositionColor reference2 = ref verticesBuffer[i * 2 + 1];
				reference2 = new Quasar.Global.Vertex.VertexPositionColor(line.to, line.color);
			}
		}
		dirty = false;
		base.PrimitiveCount = lineCount;
	}

	public override void Render(Transform motion)
	{
		if (dirty)
		{
			updateMesh();
		}
		base.Render(motion);
	}
}
