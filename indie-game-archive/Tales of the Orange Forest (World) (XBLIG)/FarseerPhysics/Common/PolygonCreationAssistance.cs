using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public sealed class PolygonCreationAssistance
{
	private byte _alphaTolerance;

	private uint _alphaToleranceRealValue;

	private int _holeDetectionLineStepSize;

	private float _hullTolerance;

	private uint[] Data { get; set; }

	public int Width { get; private set; }

	public int Height { get; private set; }

	public byte AlphaTolerance
	{
		get
		{
			return _alphaTolerance;
		}
		set
		{
			_alphaTolerance = value;
			_alphaToleranceRealValue = (uint)(value << 24);
		}
	}

	public float HullTolerance
	{
		get
		{
			return _hullTolerance;
		}
		set
		{
			float num = value;
			if (num > 4f)
			{
				num = 4f;
			}
			if (num < 0.9f)
			{
				num = 0.9f;
			}
			_hullTolerance = num;
		}
	}

	public int HoleDetectionLineStepSize
	{
		get
		{
			return _holeDetectionLineStepSize;
		}
		private set
		{
			if (value < 1)
			{
				_holeDetectionLineStepSize = 1;
			}
			else if (value > 10)
			{
				_holeDetectionLineStepSize = 10;
			}
			else
			{
				_holeDetectionLineStepSize = value;
			}
		}
	}

	public bool HoleDetection { get; set; }

	public bool MultipartDetection { get; set; }

	public PolygonCreationAssistance(uint[] data, int width, int height)
	{
		Data = data;
		Width = width;
		Height = height;
		AlphaTolerance = 20;
		HullTolerance = 1.5f;
		HoleDetectionLineStepSize = 1;
		HoleDetection = false;
		MultipartDetection = false;
	}

	public bool IsSolid(Vector2 pixel)
	{
		return IsSolid((int)pixel.X, (int)pixel.Y);
	}

	public bool IsSolid(int x, int y)
	{
		if (x >= 0 && x < Width && y >= 0 && y < Height)
		{
			return (Data[x + y * Width] & 0xFF000000u) >= _alphaToleranceRealValue;
		}
		return false;
	}

	public bool IsSolid(int index)
	{
		if (index >= 0 && index < Width * Height)
		{
			return (Data[index] & 0xFF000000u) >= _alphaToleranceRealValue;
		}
		return false;
	}

	public bool InBounds(Vector2 coord)
	{
		if (coord.X >= 0f && coord.X < (float)Width && coord.Y >= 0f)
		{
			return coord.Y < (float)Height;
		}
		return false;
	}

	public bool IsValid()
	{
		if (Data != null && Data.Length > 0)
		{
			return Data.Length == Width * Height;
		}
		return false;
	}
}
