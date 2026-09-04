using Microsoft.Xna.Framework;

namespace XnaLibrary.Graphics;

public class DirectionalLight
{
	private Vector3 diffuseColor;

	private Vector3 direction;

	private bool enabled;

	private Vector3 specularColor;

	public Vector3 DiffuseColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return diffuseColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			diffuseColor = value;
		}
	}

	public Vector3 Direction
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return direction;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			direction = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public Vector3 SpecularColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return specularColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			specularColor = value;
		}
	}

	public DirectionalLight()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		diffuseColor = Vector3.Zero;
		direction = Vector3.Up;
		specularColor = Vector3.Zero;
		base._002Ector();
	}
}
