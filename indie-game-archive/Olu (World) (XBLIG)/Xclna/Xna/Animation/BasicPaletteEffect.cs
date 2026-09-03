using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation;

public sealed class BasicPaletteEffect : Effect
{
	public sealed class BasicDirectionalLight
	{
		private BasicPaletteEffect effect;

		private EffectParameter lightDirParam;

		private EffectParameter difColorParam;

		private EffectParameter lightEnabledParam;

		private EffectParameter specColorParam;

		public bool Enabled
		{
			get
			{
				return lightEnabledParam.GetValueBoolean();
			}
			set
			{
				lightEnabledParam.SetValue(value);
			}
		}

		public Vector3 Direction
		{
			get
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				return lightDirParam.GetValueVector3();
			}
			set
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				lightDirParam.SetValue(Vector3.Normalize(value));
			}
		}

		public Vector3 SpecularColor
		{
			get
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				return specColorParam.GetValueVector3();
			}
			set
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				specColorParam.SetValue(value);
			}
		}

		public Vector3 DiffuseColor
		{
			get
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				return difColorParam.GetValueVector3();
			}
			set
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				difColorParam.SetValue(value);
			}
		}

		internal BasicDirectionalLight(BasicPaletteEffect effect, int lightNum)
		{
			this.effect = effect;
			string text = "DirLight" + lightNum;
			lightDirParam = ((Effect)effect).Parameters[text + "Direction"];
			difColorParam = ((Effect)effect).Parameters[text + "DiffuseColor"];
			specColorParam = ((Effect)effect).Parameters[text + "SpecularColor"];
			lightEnabledParam = ((Effect)effect).Parameters[text + "Enable"];
		}
	}

	private EffectParameter worldParam;

	private EffectParameter viewParam;

	private EffectParameter projectionParam;

	private EffectParameter ambientParam;

	private EffectParameter eyeParam;

	private EffectParameter emissiveParam;

	private EffectParameter diffuseParam;

	private EffectParameter lightEnabledParam;

	private EffectParameter alphaParam;

	private EffectParameter specColorParam;

	private EffectParameter specPowerParam;

	private EffectParameter texEnabledParam;

	private EffectParameter texParam;

	private EffectParameter paletteParam;

	private EffectParameter fogEnabledParam;

	private EffectParameter fogStartParam;

	private EffectParameter fogEndParam;

	private EffectParameter fogColorParam;

	private BasicDirectionalLight light0;

	private BasicDirectionalLight light1;

	private BasicDirectionalLight light2;

	private Vector3 eye;

	private static Vector3 zero;

	public readonly int PALETTE_SIZE;

	public Texture2D Texture
	{
		get
		{
			return texParam.GetValueTexture2D();
		}
		set
		{
			texParam.SetValue((Texture)(object)value);
		}
	}

	public float FogStart
	{
		get
		{
			return fogStartParam.GetValueSingle();
		}
		set
		{
			fogStartParam.SetValue(value);
		}
	}

	public float FogEnd
	{
		get
		{
			return fogEndParam.GetValueSingle();
		}
		set
		{
			fogEndParam.SetValue(value);
		}
	}

	public bool FogEnabled
	{
		get
		{
			return fogEnabledParam.GetValueBoolean();
		}
		set
		{
			fogEnabledParam.SetValue(value);
		}
	}

	public Vector3 FogColor
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return fogColorParam.GetValueVector3();
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			fogColorParam.SetValue(value);
		}
	}

	public float Alpha
	{
		get
		{
			return alphaParam.GetValueSingle();
		}
		set
		{
			alphaParam.SetValue(value);
		}
	}

	public bool TextureEnabled
	{
		get
		{
			return texEnabledParam.GetValueBoolean();
		}
		set
		{
			texEnabledParam.SetValue(value);
		}
	}

	public Matrix[] MatrixPalette
	{
		get
		{
			return paletteParam.GetValueMatrixArray(PALETTE_SIZE);
		}
		set
		{
			paletteParam.SetValue(value);
		}
	}

	public BasicDirectionalLight DirectionalLight0 => light0;

	public BasicDirectionalLight DirectionalLight1 => light1;

	public BasicDirectionalLight DirectionalLight2 => light2;

	public Vector3 AmbientLightColor
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return ambientParam.GetValueVector3();
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ambientParam.SetValue(value);
		}
	}

	public Vector3 SpecularColor
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return specColorParam.GetValueVector3();
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			specColorParam.SetValue(value);
		}
	}

	public float SpecularPower
	{
		get
		{
			return specPowerParam.GetValueSingle();
		}
		set
		{
			specPowerParam.SetValue(value);
		}
	}

	public Vector3 DiffuseColor
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return diffuseParam.GetValueVector3();
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			diffuseParam.SetValue(value);
		}
	}

	public bool LightingEnabled
	{
		get
		{
			return lightEnabledParam.GetValueBoolean();
		}
		set
		{
			lightEnabledParam.SetValue(value);
		}
	}

	public Vector3 EmissiveColor
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return emissiveParam.GetValueVector3();
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			emissiveParam.SetValue(value);
		}
	}

	public Matrix World
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return worldParam.GetValueMatrix();
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			worldParam.SetValue(value);
		}
	}

	public Matrix View
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return viewParam.GetValueMatrix();
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			Matrix val = Matrix.Invert(value);
			Vector3.Transform(ref zero, ref val, ref eye);
			viewParam.SetValue(value);
			eyeParam.SetValue(eye);
		}
	}

	public Matrix Projection
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return projectionParam.GetValueMatrix();
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			projectionParam.SetValue(value);
		}
	}

	internal BasicPaletteEffect(GraphicsDevice device, byte[] byteCode, int paletteSize)
		: base(device, byteCode, (CompilerOptions)1024, (EffectPool)null)
	{
		PALETTE_SIZE = paletteSize;
		InitializeParameters();
	}

	internal BasicPaletteEffect(GraphicsDevice device, Effect cloneSource)
		: base(device, cloneSource)
	{
		PALETTE_SIZE = ((BasicPaletteEffect)(object)cloneSource).PALETTE_SIZE;
		InitializeParameters();
	}

	public void EnableDefaultLighting()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		LightingEnabled = true;
		BasicDirectionalLight basicDirectionalLight = light0;
		Color val = Color.White;
		basicDirectionalLight.DiffuseColor = ((Color)(ref val)).ToVector3();
		BasicDirectionalLight basicDirectionalLight2 = light0;
		val = Color.Black;
		basicDirectionalLight2.SpecularColor = ((Color)(ref val)).ToVector3();
		light0.Direction = Vector3.Normalize(new Vector3(-1f, 0f, -1f));
		BasicDirectionalLight basicDirectionalLight3 = light0;
		val = Color.White;
		basicDirectionalLight3.SpecularColor = ((Color)(ref val)).ToVector3();
		BasicDirectionalLight basicDirectionalLight4 = light1;
		val = Color.Black;
		basicDirectionalLight4.DiffuseColor = ((Color)(ref val)).ToVector3();
		BasicDirectionalLight basicDirectionalLight5 = light1;
		val = Color.Black;
		basicDirectionalLight5.SpecularColor = ((Color)(ref val)).ToVector3();
		BasicDirectionalLight basicDirectionalLight6 = light2;
		val = Color.Black;
		basicDirectionalLight6.DiffuseColor = ((Color)(ref val)).ToVector3();
		BasicDirectionalLight basicDirectionalLight7 = light2;
		val = Color.Black;
		basicDirectionalLight7.SpecularColor = ((Color)(ref val)).ToVector3();
		SpecularPower = 8f;
		light0.Enabled = true;
		light1.Enabled = false;
		light2.Enabled = false;
	}

	private void InitializeParameters()
	{
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		paletteParam = ((Effect)this).Parameters["MatrixPalette"];
		texParam = ((Effect)this).Parameters["BasicTexture"];
		texEnabledParam = ((Effect)this).Parameters["TextureEnabled"];
		worldParam = ((Effect)this).Parameters["World"];
		viewParam = ((Effect)this).Parameters["View"];
		projectionParam = ((Effect)this).Parameters["Projection"];
		ambientParam = ((Effect)this).Parameters["AmbientLightColor"];
		eyeParam = ((Effect)this).Parameters["EyePosition"];
		emissiveParam = ((Effect)this).Parameters["EmissiveColor"];
		lightEnabledParam = ((Effect)this).Parameters["LightingEnable"];
		diffuseParam = ((Effect)this).Parameters["DiffuseColor"];
		specColorParam = ((Effect)this).Parameters["SpecularColor"];
		specPowerParam = ((Effect)this).Parameters["SpecularPower"];
		alphaParam = ((Effect)this).Parameters["Alpha"];
		fogColorParam = ((Effect)this).Parameters["FogColor"];
		fogEnabledParam = ((Effect)this).Parameters["FogEnable"];
		fogStartParam = ((Effect)this).Parameters["FogStart"];
		fogEndParam = ((Effect)this).Parameters["FogEnd"];
		light0 = new BasicDirectionalLight(this, 0);
		light1 = new BasicDirectionalLight(this, 1);
		light2 = new BasicDirectionalLight(this, 2);
		FogColor = Vector3.Zero;
		FogStart = 0f;
		FogEnd = 1f;
		FogEnabled = false;
	}

	public override Effect Clone(GraphicsDevice device)
	{
		return (Effect)(object)new BasicPaletteEffect(device, (Effect)(object)this);
	}

	public void SetParamsFromBasicEffect(BasicEffect effect)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		AmbientLightColor = effect.AmbientLightColor;
		DiffuseColor = effect.DiffuseColor;
		LightingEnabled = effect.LightingEnabled;
		Projection = effect.Projection;
		World = effect.World;
		View = effect.View;
		SpecularColor = effect.SpecularColor;
		EmissiveColor = effect.EmissiveColor;
		SpecularPower = effect.SpecularPower;
		Alpha = effect.Alpha;
		Texture = effect.Texture;
		TextureEnabled = effect.TextureEnabled;
		SetParamsFromBasicLight(effect.DirectionalLight0, light0);
		SetParamsFromBasicLight(effect.DirectionalLight1, light1);
		SetParamsFromBasicLight(effect.DirectionalLight2, light2);
	}

	private void SetParamsFromBasicLight(BasicDirectionalLight source, BasicDirectionalLight target)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		target.SpecularColor = source.SpecularColor;
		target.Enabled = source.Enabled;
		target.Direction = source.Direction;
		target.DiffuseColor = source.DiffuseColor;
	}

	static BasicPaletteEffect()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		zero = Vector3.Zero;
	}
}
