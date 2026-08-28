using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CubeBuilder;

public class ManualCube
{
	private const int NUM_TRIANGLES = 12;

	private const int NUM_VERTICES = 36;

	private VertexPositionColor[] _vertices;

	private VertexBuffer _shapeBuffer;

	private bool _isConstructed;

	public Vector3 Size { get; set; }

	public Vector3 Position { get; set; }

	public ManualCube(Vector3 size, Vector3 position)
	{
		Size = size;
		Position = position;
	}

	public void RenderToDevice(GraphicsDevice device, Color color)
	{
		if (!_isConstructed)
		{
			ConstructCube();
		}
		for (int i = 0; i < _vertices.Length; i++)
		{
			_vertices[i].Color = color;
		}
		device.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 12);
	}

	private void ConstructCube()
	{
		_vertices = new VertexPositionColor[36];
		Vector3 position = Position + new Vector3(-1f, 1f, -1f) * Size;
		Vector3 position2 = Position + new Vector3(-1f, 1f, 1f) * Size;
		Vector3 position3 = Position + new Vector3(1f, 1f, -1f) * Size;
		Vector3 position4 = Position + new Vector3(1f, 1f, 1f) * Size;
		Vector3 position5 = Position + new Vector3(-1f, -1f, -1f) * Size;
		Vector3 position6 = Position + new Vector3(-1f, -1f, 1f) * Size;
		Vector3 position7 = Position + new Vector3(1f, -1f, -1f) * Size;
		Vector3 position8 = Position + new Vector3(1f, -1f, 1f) * Size;
		ref VertexPositionColor reference = ref _vertices[0];
		reference = new VertexPositionColor(position, Color.White);
		ref VertexPositionColor reference2 = ref _vertices[1];
		reference2 = new VertexPositionColor(position5, Color.White);
		ref VertexPositionColor reference3 = ref _vertices[2];
		reference3 = new VertexPositionColor(position3, Color.White);
		ref VertexPositionColor reference4 = ref _vertices[3];
		reference4 = new VertexPositionColor(position5, Color.White);
		ref VertexPositionColor reference5 = ref _vertices[4];
		reference5 = new VertexPositionColor(position7, Color.White);
		ref VertexPositionColor reference6 = ref _vertices[5];
		reference6 = new VertexPositionColor(position3, Color.White);
		ref VertexPositionColor reference7 = ref _vertices[6];
		reference7 = new VertexPositionColor(position2, Color.White);
		ref VertexPositionColor reference8 = ref _vertices[7];
		reference8 = new VertexPositionColor(position4, Color.White);
		ref VertexPositionColor reference9 = ref _vertices[8];
		reference9 = new VertexPositionColor(position6, Color.White);
		ref VertexPositionColor reference10 = ref _vertices[9];
		reference10 = new VertexPositionColor(position6, Color.White);
		ref VertexPositionColor reference11 = ref _vertices[10];
		reference11 = new VertexPositionColor(position4, Color.White);
		ref VertexPositionColor reference12 = ref _vertices[11];
		reference12 = new VertexPositionColor(position8, Color.White);
		ref VertexPositionColor reference13 = ref _vertices[12];
		reference13 = new VertexPositionColor(position, Color.White);
		ref VertexPositionColor reference14 = ref _vertices[13];
		reference14 = new VertexPositionColor(position4, Color.White);
		ref VertexPositionColor reference15 = ref _vertices[14];
		reference15 = new VertexPositionColor(position2, Color.White);
		ref VertexPositionColor reference16 = ref _vertices[15];
		reference16 = new VertexPositionColor(position, Color.White);
		ref VertexPositionColor reference17 = ref _vertices[16];
		reference17 = new VertexPositionColor(position3, Color.White);
		ref VertexPositionColor reference18 = ref _vertices[17];
		reference18 = new VertexPositionColor(position4, Color.White);
		ref VertexPositionColor reference19 = ref _vertices[18];
		reference19 = new VertexPositionColor(position5, Color.White);
		ref VertexPositionColor reference20 = ref _vertices[19];
		reference20 = new VertexPositionColor(position6, Color.White);
		ref VertexPositionColor reference21 = ref _vertices[20];
		reference21 = new VertexPositionColor(position8, Color.White);
		ref VertexPositionColor reference22 = ref _vertices[21];
		reference22 = new VertexPositionColor(position5, Color.White);
		ref VertexPositionColor reference23 = ref _vertices[22];
		reference23 = new VertexPositionColor(position8, Color.White);
		ref VertexPositionColor reference24 = ref _vertices[23];
		reference24 = new VertexPositionColor(position7, Color.White);
		ref VertexPositionColor reference25 = ref _vertices[24];
		reference25 = new VertexPositionColor(position, Color.White);
		ref VertexPositionColor reference26 = ref _vertices[25];
		reference26 = new VertexPositionColor(position6, Color.White);
		ref VertexPositionColor reference27 = ref _vertices[26];
		reference27 = new VertexPositionColor(position5, Color.White);
		ref VertexPositionColor reference28 = ref _vertices[27];
		reference28 = new VertexPositionColor(position2, Color.White);
		ref VertexPositionColor reference29 = ref _vertices[28];
		reference29 = new VertexPositionColor(position6, Color.White);
		ref VertexPositionColor reference30 = ref _vertices[29];
		reference30 = new VertexPositionColor(position, Color.White);
		ref VertexPositionColor reference31 = ref _vertices[30];
		reference31 = new VertexPositionColor(position3, Color.White);
		ref VertexPositionColor reference32 = ref _vertices[31];
		reference32 = new VertexPositionColor(position7, Color.White);
		ref VertexPositionColor reference33 = ref _vertices[32];
		reference33 = new VertexPositionColor(position8, Color.White);
		ref VertexPositionColor reference34 = ref _vertices[33];
		reference34 = new VertexPositionColor(position4, Color.White);
		ref VertexPositionColor reference35 = ref _vertices[34];
		reference35 = new VertexPositionColor(position3, Color.White);
		ref VertexPositionColor reference36 = ref _vertices[35];
		reference36 = new VertexPositionColor(position8, Color.White);
		_isConstructed = true;
	}
}
