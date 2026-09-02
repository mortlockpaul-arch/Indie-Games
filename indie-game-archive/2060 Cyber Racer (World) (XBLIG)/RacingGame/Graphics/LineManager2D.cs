using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Helpers;
using RacingGame.Shaders;

namespace RacingGame.Graphics;

public class LineManager2D : IDisposable
{
	private struct Line
	{
		public Point startPoint;

		public Point endPoint;

		public Color color;

		public Line(Point setStartPoint, Point setEndPoint, Color setColor)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			startPoint = setStartPoint;
			endPoint = setEndPoint;
			color = setColor;
		}

		public static bool operator ==(Line a, Line b)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (a.startPoint == b.startPoint && a.endPoint == b.endPoint)
			{
				return a.color == b.color;
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
			if (!(a.startPoint != b.startPoint) && !(a.endPoint != b.endPoint))
			{
				return a.color != b.color;
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

	private const int MaxNumOfLines = 64;

	private int numOfLines;

	private List<Line> lines;

	private bool buildVertexBuffer;

	private VertexPositionColor[] lineVertices;

	private int numOfPrimitives;

	private VertexDeclaration decl;

	public LineManager2D()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		lines = new List<Line>();
		lineVertices = (VertexPositionColor[])(object)new VertexPositionColor[128];
		base._002Ector();
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

	private void UpdateVertexBuffer()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (numOfLines == 0 || lines.Count < numOfLines)
		{
			numOfPrimitives = 0;
			return;
		}
		for (int i = 0; i < numOfLines; i++)
		{
			Line line = lines[i];
			ref VertexPositionColor reference = ref lineVertices[i * 2];
			reference = new VertexPositionColor(new Vector3(-1f + 2f * (float)line.startPoint.X / (float)BaseGame.Width, 0f - (-1f + 2f * (float)line.startPoint.Y / (float)BaseGame.Height), 0f), line.color);
			ref VertexPositionColor reference2 = ref lineVertices[i * 2 + 1];
			reference2 = new VertexPositionColor(new Vector3(-1f + 2f * (float)line.endPoint.X / (float)BaseGame.Width, 0f - (-1f + 2f * (float)line.endPoint.Y / (float)BaseGame.Height), 0f), line.color);
		}
		numOfPrimitives = numOfLines;
		buildVertexBuffer = false;
	}

	public void AddLine(Point startPoint, Point endPoint, Color color)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (numOfLines >= 64)
		{
			Log.Write("Too many lines requested in LineManager2D. Max lines = " + 64);
			return;
		}
		Line line = new Line(startPoint, endPoint, color);
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

	public void AddLineWithShadow(Point startPoint, Point endPoint, Color color)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		AddLine(new Point(startPoint.X, startPoint.Y + 1), new Point(endPoint.X, endPoint.Y + 1), Color.Black);
		AddLine(startPoint, endPoint, color);
	}

	public virtual void Render()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (buildVertexBuffer || numOfPrimitives != numOfLines)
		{
			UpdateVertexBuffer();
		}
		if (numOfPrimitives > 0)
		{
			BaseGame.SetAlphaBlendingEnabled(value: true);
			BaseGame.WorldMatrix = Matrix.Identity;
			ShaderEffect.lineRendering.Render("LineRendering2D", delegate
			{
				BaseGame.Device.VertexDeclaration = decl;
				BaseGame.Device.DrawUserPrimitives<VertexPositionColor>((PrimitiveType)2, lineVertices, 0, numOfPrimitives);
			});
		}
		numOfLines = 0;
	}
}
