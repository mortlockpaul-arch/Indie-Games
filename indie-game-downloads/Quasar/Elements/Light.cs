using Microsoft.Xna.Framework;

namespace Quasar.Elements;

public class Light : Element
{
	private Vector3 ambient = new Vector3(0.5f, 0.5f, 0.5f);

	private Vector3 diffuse = new Vector3(0.5f, 0.5f, 0.5f);

	private Vector3 specular = new Vector3(0.5f, 0.5f, 0.5f);

	private Vector3 attenuation = new Vector3(1f, 0f, 0f);

	public Vector3 Ambient
	{
		get
		{
			return ambient;
		}
		set
		{
			ambient = value;
		}
	}

	public Vector3 Diffuse
	{
		get
		{
			return diffuse;
		}
		set
		{
			diffuse = value;
		}
	}

	public Vector3 Specular
	{
		get
		{
			return specular;
		}
		set
		{
			specular = value;
		}
	}

	public Vector3 Attenuation
	{
		get
		{
			return attenuation;
		}
		set
		{
			attenuation = value;
		}
	}

	public Light()
	{
	}

	public Light(Vector3 color)
	{
		ambient = Vector3.Zero;
		diffuse = color;
		specular = color;
	}

	public void UpdateRotationFromTranslation()
	{
		base.Transform.Rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(new Vector3(base.Transform.Translation.X, 0f - base.Transform.Translation.Y, 0f - base.Transform.Translation.Z), Vector3.Zero, Vector3.Up));
	}
}
