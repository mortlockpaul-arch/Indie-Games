using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaLibrary.Graphics;

public class BasicModel : ModelData
{
	protected float alpha;

	protected Vector3 ambientLightColor;

	protected Vector3 diffuseColor;

	protected Vector3 emissiveColor;

	private bool fogEnabled;

	private Vector3 fogColor;

	private float fogStart;

	private float fogEnd;

	private bool lightingEnabled;

	private bool preferPerPixelLighting;

	private Vector3 specularColor;

	private float specularPower;

	private DirectionalLight directionalLight0;

	private DirectionalLight directionalLight1;

	private DirectionalLight directionalLight2;

	public Vector3 AmbientLightColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return ambientLightColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			ambientLightColor = value;
		}
	}

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

	public Vector3 EmissiveColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return emissiveColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			emissiveColor = value;
		}
	}

	public float Alpha
	{
		get
		{
			return alpha;
		}
		set
		{
			alpha = value;
		}
	}

	public bool FogEnabled
	{
		get
		{
			return fogEnabled;
		}
		set
		{
			fogEnabled = value;
		}
	}

	public Vector3 FogColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return fogColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			fogColor = value;
		}
	}

	public float FogStart
	{
		get
		{
			return fogStart;
		}
		set
		{
			fogStart = value;
		}
	}

	public float FogEnd
	{
		get
		{
			return fogEnd;
		}
		set
		{
			fogEnd = value;
		}
	}

	public bool LightingEnabled
	{
		get
		{
			return lightingEnabled;
		}
		set
		{
			lightingEnabled = value;
		}
	}

	public bool PreferPerPixelLighting
	{
		get
		{
			return preferPerPixelLighting;
		}
		set
		{
			preferPerPixelLighting = value;
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

	public float SpecularPower
	{
		get
		{
			return specularPower;
		}
		set
		{
			specularPower = value;
		}
	}

	public DirectionalLight DirectionalLight0
	{
		get
		{
			return directionalLight0;
		}
		set
		{
			directionalLight0 = value;
		}
	}

	public DirectionalLight DirectionalLight1
	{
		get
		{
			return directionalLight1;
		}
		set
		{
			directionalLight1 = value;
		}
	}

	public DirectionalLight DirectionalLight2
	{
		get
		{
			return directionalLight2;
		}
		set
		{
			directionalLight2 = value;
		}
	}

	public BasicModel(Model model)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		alpha = 1f;
		ambientLightColor = new Vector3(0f, 0f, 0f);
		diffuseColor = new Vector3(0.5882353f, 0.5882353f, 0.5882353f);
		emissiveColor = new Vector3(0.3f, 0.3f, 0.3f);
		fogColor = new Vector3(0f, 0f, 0f);
		fogEnd = 1f;
		specularColor = new Vector3(0f, 0f, 0f);
		specularPower = (float)Math.PI * 226f / 355f;
		directionalLight0 = new DirectionalLight();
		directionalLight1 = new DirectionalLight();
		directionalLight2 = new DirectionalLight();
		base._002Ector(model);
	}

	public virtual void Draw(Matrix view, Matrix projection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Draw(view, projection, Matrix.Identity);
	}

	public unsafe override void Draw(Matrix view, Matrix projection, Matrix world)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = base.Model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.World = GetWorldMatrix(current, world);
						val.View = view;
						val.Projection = projection;
						val.Alpha = Alpha;
						val.LightingEnabled = LightingEnabled;
						val.PreferPerPixelLighting = PreferPerPixelLighting;
						val.AmbientLightColor = AmbientLightColor;
						val.DiffuseColor = DiffuseColor;
						val.EmissiveColor = EmissiveColor;
						val.SpecularColor = SpecularColor;
						val.SpecularPower = SpecularPower;
						SetDirectionalLight(val.DirectionalLight0, DirectionalLight0);
						SetDirectionalLight(val.DirectionalLight1, DirectionalLight1);
						SetDirectionalLight(val.DirectionalLight2, DirectionalLight2);
						val.FogEnabled = FogEnabled;
						val.FogColor = FogColor;
						val.FogStart = FogStart;
						val.FogEnd = FogEnd;
						if (base.Texture != null)
						{
							val.Texture = base.Texture;
						}
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void SetDirectionalLight(BasicDirectionalLight basicDirectionalLight, DirectionalLight directionalLight)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		basicDirectionalLight.Enabled = directionalLight.Enabled;
		basicDirectionalLight.DiffuseColor = directionalLight.DiffuseColor;
		basicDirectionalLight.Direction = directionalLight.Direction;
		basicDirectionalLight.SpecularColor = directionalLight.SpecularColor;
	}
}
