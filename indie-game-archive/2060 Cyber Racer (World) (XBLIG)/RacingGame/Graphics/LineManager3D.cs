using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Shaders;

namespace RacingGame.Graphics;

internal class LineManager3D : IDisposable
{
	private struct Line
	{
		public Vector3 startPoint;

		public Vector3 endPoint;

		public Color startColor;

		public Color endColor;

		public Line(Vector3 setStartPoint, Color setStartColor, Vector3 setEndPoint, Color setEndColor)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			startPoint = setStartPoint;
			startColor = setStartColor;
			endPoint = setEndPoint;
			endColor = setEndColor;
		}

		public static bool operator ==(Line a, Line b)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			if (a.startPoint == b.startPoint && a.endPoint == b.endPoint && a.startColor == b.startColor)
			{
				return a.endColor == b.endColor;
			}
			return false;
		}

		public static bool operator !=(Line a, Line b)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			if (!(a.startPoint != b.startPoint) && !(a.endPoint != b.endPoint) && !(a.startColor != b.startColor))
			{
				return a.endColor != b.endColor;
			}
			return true;
		}

		public override bool Equals(object a)
		{
			if (a is Line)
			{
				return (Line)a == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}

	protected const int MaxNumOfLines = 4096;

	private int numOfLines;

	private List<Line> lines;

	private bool buildVertexBuffer;

	private VertexPositionColor[] lineVertices;

	private int numOfPrimitives;

	private VertexDeclaration decl;

	public LineManager3D()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		lines = new List<Line>();
		lineVertices = (VertexPositionColor[])(object)new VertexPositionColor[8192];
		base._002Ector();
		if (BaseGame.Device == null)
		{
			throw new ArgumentNullException("XNA device is not initialized, can't init line manager.");
		}
		decl = new VertexDeclaration(BaseGame.Device, VertexPositionColor.VertexElements);
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
			decl.Dispose();
		}
	}

	public void AddLine(Vector3 startPoint, Color startColor, Vector3 endPoint, Color endColor)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (numOfLines >= 4096)
		{
			return;
		}
		Line line = new Line(startPoint, startColor, endPoint, endColor);
		if (lines.Count > numOfLines)
		{
			if (lines[numOfLines] != line)
			{
				lines[numOfLines] = line;
				buildVertexBuffer = true;
			}
		}
		else
		{
			lines.Add(line);
			buildVertexBuffer = true;
		}
		numOfLines++;
	}

	public void AddLine(Vector3 startPoint, Vector3 endPoint, Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		AddLine(startPoint, color, endPoint, color);
	}

	protected void UpdateVertexBuffer()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (numOfLines == 0 || lines.Count < numOfLines)
		{
			numOfPrimitives = 0;
			return;
		}
		for (int i = 0; i < numOfLines; i++)
		{
			Line line = lines[i];
			ref VertexPositionColor reference = ref lineVertices[i * 2];
			reference = new VertexPositionColor(line.startPoint, line.startColor);
			ref VertexPositionColor reference2 = ref lineVertices[i * 2 + 1];
			reference2 = new VertexPositionColor(line.endPoint, line.endColor);
		}
		numOfPrimitives = numOfLines;
		buildVertexBuffer = false;
	}

	public void Render()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (buildVertexBuffer || numOfPrimitives != numOfLines)
		{
			UpdateVertexBuffer();
		}
		if (numOfPrimitives > 0)
		{
			BaseGame.WorldMatrix = Matrix.Identity;
			ShaderEffect.lineRendering.Render("LineRendering3D", delegate
			{
				BaseGame.SetAlphaBlendingEnabled(value: true);
				BaseGame.Device.VertexDeclaration = decl;
				BaseGame.Device.DrawUserPrimitives<VertexPositionColor>((PrimitiveType)2, lineVertices, 0, numOfPrimitives);
			});
		}
		numOfLines = 0;
	}
}
