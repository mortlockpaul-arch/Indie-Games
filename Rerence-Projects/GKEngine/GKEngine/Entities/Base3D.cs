using System;
using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class Base3D : IEquatable<Base3D>
{
	private Vector3 _calc_scale = new Vector3(1f, 1f, 1f);

	public Vector3 _position;

	public Quaternion _rotation;

	public Vector3 _scale;

	protected Matrix _matrix = Matrix.Identity;

	protected Matrix _matrixScale = default(Matrix);

	protected Matrix _matrixRotation = default(Matrix);

	protected Matrix _matrixPosition = default(Matrix);

	protected bool _change_position = true;

	protected bool _change_rotation = true;

	protected bool _change_scale = true;

	public float camDepth;

	public Matrix rotationMatrix => Matrix.CreateFromQuaternion(_rotation);

	public virtual float X
	{
		get
		{
			return _position.X;
		}
		set
		{
			_position.X = value;
			_change_position = true;
		}
	}

	public virtual float Y
	{
		get
		{
			return _position.Y;
		}
		set
		{
			_position.Y = value;
			_change_position = true;
		}
	}

	public virtual float Z
	{
		get
		{
			return _position.Z;
		}
		set
		{
			_position.Z = value;
			_change_position = true;
		}
	}

	public virtual Vector3 position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
			_change_position = true;
		}
	}

	public virtual Quaternion rotation
	{
		get
		{
			return _rotation;
		}
		set
		{
			_rotation = value;
			_change_rotation = true;
		}
	}

	public virtual Vector3 scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
			_change_scale = true;
		}
	}

	public virtual float scaleX
	{
		get
		{
			return _scale.X;
		}
		set
		{
			_scale.X = value;
			_change_scale = true;
		}
	}

	public virtual float scaleY
	{
		get
		{
			return _scale.Y;
		}
		set
		{
			_scale.Y = value;
			_change_scale = true;
		}
	}

	public virtual float scaleZ
	{
		get
		{
			return _scale.Z;
		}
		set
		{
			_scale.Z = value;
			_change_scale = true;
		}
	}

	public virtual float scaleAll
	{
		get
		{
			return (_scale.Z + _scale.Y + _scale.Z) / 3f;
		}
		set
		{
			_scale.X = value;
			_scale.Y = value;
			_scale.Z = value;
			_change_scale = true;
		}
	}

	public virtual Matrix matrix
	{
		get
		{
			if (_change_position || _change_rotation || _change_scale)
			{
				Matrix.CreateScale(ref _scale, out _matrixScale);
				Matrix.CreateFromQuaternion(ref _rotation, out _matrixRotation);
				Matrix.CreateTranslation(ref _position, out _matrixPosition);
				Matrix.Multiply(ref _matrixScale, ref _matrixRotation, out _matrix);
				Matrix.Multiply(ref _matrix, ref _matrixPosition, out _matrix);
				_change_position = false;
				_change_rotation = false;
				_change_scale = false;
			}
			return _matrix;
		}
		set
		{
			_matrix = value;
			value.Decompose(out _scale, out _rotation, out _position);
			_change_position = false;
			_change_rotation = false;
			_change_scale = false;
		}
	}

	public virtual Vector3 unit => Vector3.Transform(Vector3.Forward, Matrix.CreateFromQuaternion(_rotation));

	public Base3D()
	{
		Set(Vector3.Zero, Quaternion.Identity, Vector3.One);
	}

	public Base3D(Vector3 oPosition, Quaternion oRotation, Vector3 oScale)
	{
		Set(oPosition, oRotation, oScale);
	}

	public Base3D(Matrix oMatrix)
	{
		Set(oMatrix);
	}

	public void Set(Vector3 oPosition, Quaternion oRotation, Vector3 oScale)
	{
		position = oPosition;
		rotation = oRotation;
		scale = oScale;
	}

	public void Set(Matrix oMatrix)
	{
		matrix = oMatrix;
	}

	public void Set(Base3D oBase)
	{
		Set(oBase.matrix);
	}

	public virtual void SetPosition(Vector3 vPosition)
	{
		_position = vPosition;
		_change_position = true;
	}

	public virtual void SetRotation(Quaternion oRotation)
	{
		_rotation = oRotation;
		_change_rotation = true;
	}

	public virtual void SetScale(Vector3 vScale)
	{
		_scale = vScale;
		_change_scale = true;
	}

	public virtual void RoundMatrix()
	{
		_calc_scale = _scale;
		scale = Vector3.One;
		_matrix.Left = new Vector3((float)Math.Round(matrix.Left.X), (float)Math.Round(matrix.Left.Y), (float)Math.Round(matrix.Left.Z));
		_matrix.Forward = new Vector3((float)Math.Round(matrix.Forward.X), (float)Math.Round(matrix.Forward.Y), (float)Math.Round(matrix.Forward.Z));
		_matrix.Up = new Vector3((float)Math.Round(matrix.Up.X), (float)Math.Round(matrix.Up.Y), (float)Math.Round(matrix.Up.Z));
		scale = _calc_scale;
		_matrix = matrix;
	}

	public virtual void Translate(Vector3 xDistance)
	{
		position += Vector3.Transform(xDistance, Matrix.CreateFromQuaternion(rotation));
	}

	public virtual void PivotLocal(Vector3 xPivot, Quaternion xRotation)
	{
		position -= xPivot;
		rotation *= xRotation;
		position += Vector3.Transform(xPivot, rotation);
	}

	public bool Equals(Base3D other)
	{
		if (_position == other._position && _rotation == other._rotation)
		{
			return _scale == other._scale;
		}
		return false;
	}

	public static Vector3 Translate(Vector3 xPosition, float xDistance, Matrix mRotation)
	{
		return xPosition + Vector3.Transform(xDistance * Vector3.Forward, mRotation);
	}

	public override string ToString()
	{
		return "Base3D: Pos:" + position.ToString();
	}
}
