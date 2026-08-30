using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class PolygonCreationAssistance
{
	private uint[] _data;

	private int _width;

	private int _height;

	private byte _alphaTolerance;

	private uint _alphaToleranceRealValue;

	private float _hullTolerance;

	private int _holeDetectionLineStepSize;

	private bool _holeDetection;

	private bool _multipartDetection;

	public uint[] Data => _data;

	public int Width => _width;

	public int Height => _height;

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
		set
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

	public bool HoleDetection
	{
		get
		{
			return _holeDetection;
		}
		set
		{
			_holeDetection = value;
		}
	}

	public bool MultipartDetection
	{
		get
		{
			return _multipartDetection;
		}
		set
		{
			_multipartDetection = value;
		}
	}

	public PolygonCreationAssistance(uint[] data, int width, int height)
	{
		_data = data;
		_width = width;
		_height = height;
		AlphaTolerance = 20;
		HullTolerance = 1.5f;
		HoleDetectionLineStepSize = 1;
		_holeDetection = false;
		_multipartDetection = false;
	}

	public bool IsSolid(Vector2 pixel)
	{
		return IsSolid((int)pixel.X, (int)pixel.Y);
	}

	public bool IsSolid(int x, int y)
	{
		if (x >= 0 && x < _width && y >= 0 && y < _height)
		{
			return (_data[x + y * _width] & 0xFF000000u) >= _alphaToleranceRealValue;
		}
		return false;
	}

	public bool IsSolid(int index)
	{
		if (index >= 0 && index < _width * _height)
		{
			return (_data[index] & 0xFF000000u) >= _alphaToleranceRealValue;
		}
		return false;
	}

	public bool InBounds(Vector2 coord)
	{
		if (coord.X >= 0f && coord.X < (float)_width && coord.Y >= 0f)
		{
			return coord.Y < (float)_height;
		}
		return false;
	}

	public bool IsValid()
	{
		if (_data != null && _data.Length > 0)
		{
			return _data.Length == _width * _height;
		}
		return false;
	}

	~PolygonCreationAssistance()
	{
		_data = null;
	}
}
