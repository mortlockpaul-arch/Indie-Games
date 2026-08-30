using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class RoundLine
{
	private Vector2 p0;

	private Vector2 p1;

	private float rho;

	private float theta;

	public Vector2 P0
	{
		get
		{
			return p0;
		}
		set
		{
			p0 = value;
			RecalcRhoTheta();
		}
	}

	public Vector2 P1
	{
		get
		{
			return p1;
		}
		set
		{
			p1 = value;
			RecalcRhoTheta();
		}
	}

	public float Rho => rho;

	public float Theta => theta;

	public void Reset(Vector2 P0, Vector2 P1)
	{
		p0 = P0;
		p1 = P1;
		RecalcRhoTheta();
	}

	public RoundLine(Vector2 p0, Vector2 p1)
	{
		this.p0 = p0;
		this.p1 = p1;
		RecalcRhoTheta();
	}

	public RoundLine(float x0, float y0, float x1, float y1)
	{
		p0 = new Vector2(x0, y0);
		p1 = new Vector2(x1, y1);
		RecalcRhoTheta();
	}

	protected void RecalcRhoTheta()
	{
		Vector2 vector = P1 - P0;
		rho = vector.Length();
		theta = (float)Math.Atan2(vector.Y, vector.X);
	}

	public static RoundLine ToScreenCoordinates(int height, RoundLine r)
	{
		r.P0 = new Vector2(r.P0.X, (float)height - r.P0.Y);
		r.P1 = new Vector2(r.P1.X, (float)height - r.P1.Y);
		return r;
	}

	public static List<RoundLine> ToScreenCoordinates(int height, List<RoundLine> l)
	{
		List<RoundLine> list = new List<RoundLine>();
		foreach (RoundLine item in l)
		{
			list.Add(ToScreenCoordinates(height, item));
		}
		return list;
	}
}
