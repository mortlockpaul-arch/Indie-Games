using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;
using RacingGame.Helpers;

namespace RacingGame.Shaders;

public class ShaderEffect : IDisposable
{
	public static ShaderEffect lineRendering = new ShaderEffect("LineRendering.fx");

	public static ShaderEffect lighting = new ShaderEffect("LightingShader.fx");

	public static ShaderEffect normalMapping = new ShaderEffect("NormalMapping.fx");

	public static ShaderEffect landscapeNormalMapping = new ShaderEffect("LandscapeNormalMapping.fx");

	public static ShadowMapShader shadowMapping = new ShadowMapShader();

	private string shaderContentName;

	protected Effect effect;

	protected EffectParameter worldViewProj;

	protected EffectParameter viewProj;

	protected EffectParameter world;

	protected EffectParameter viewInverse;

	protected EffectParameter projection;

	protected EffectParameter lightDir;

	protected EffectParameter ambientColor;

	protected EffectParameter diffuseColor;

	protected EffectParameter specularColor;

	protected EffectParameter specularPower;

	protected EffectParameter alphaFactor;

	protected EffectParameter scale;

	protected EffectParameter diffuseTexture;

	protected EffectParameter normalTexture;

	protected EffectParameter heightTexture;

	protected EffectParameter reflectionCubeTexture;

	protected EffectParameter detailTexture;

	protected EffectParameter parallaxAmount;

	protected EffectParameter carHueColorChange;

	protected EffectParameter currTime;

	private float time;

	protected Matrix lastUsedWorldViewProjMatrix;

	protected Matrix lastUsedViewProjMatrix;

	protected Matrix lastUsedInverseViewMatrix;

	protected Matrix lastUsedProjectionMatrix;

	protected Vector3 lastUsedLightDir;

	protected Color lastUsedAmbientColor;

	protected Color lastUsedDiffuseColor;

	protected Color lastUsedSpecularColor;

	private float lastUsedSpecularPower;

	private float lastUsedAlphaFactor;

	protected Texture lastUsedDiffuseTexture;

	protected Texture lastUsedNormalTexture;

	protected Texture lastUsedHeightTexture;

	protected TextureCube lastUsedReflectionCubeTexture;

	protected Texture lastUsedDetailTexture;

	protected float lastUsedParallaxAmount;

	protected Color lastUsedCarHueColorChange;

	public bool Valid => effect != null;

	public Effect Effect => effect;

	public int NumberOfTechniques => effect.Techniques.Count;

	public EffectParameter WorldParameter => world;

	protected Matrix WorldViewProjMatrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lastUsedWorldViewProjMatrix;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetValue(worldViewProj, ref lastUsedWorldViewProjMatrix, value);
		}
	}

	protected Matrix ViewProjMatrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lastUsedViewProjMatrix;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetValue(viewProj, ref lastUsedViewProjMatrix, value);
		}
	}

	public Matrix WorldMatrix
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Matrix.Identity;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			world.SetValue(value);
		}
	}

	protected Matrix InverseViewMatrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lastUsedInverseViewMatrix;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetValue(viewInverse, ref lastUsedInverseViewMatrix, value);
		}
	}

	protected Matrix ProjectionMatrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lastUsedProjectionMatrix;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetValue(projection, ref lastUsedProjectionMatrix, value);
		}
	}

	protected Vector3 LightDir
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lastUsedLightDir;
		}
		set
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			((Vector3)(ref value)).Normalize();
			SetValue(lightDir, ref lastUsedLightDir, -value);
		}
	}

	public Color AmbientColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lastUsedAmbientColor;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetValue(ambientColor, ref lastUsedAmbientColor, value);
		}
	}

	public Color DiffuseColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lastUsedDiffuseColor;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetValue(diffuseColor, ref lastUsedDiffuseColor, value);
		}
	}

	public Color SpecularColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lastUsedSpecularColor;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetValue(specularColor, ref lastUsedSpecularColor, value);
		}
	}

	public float SpecularPower
	{
		get
		{
			return lastUsedSpecularPower;
		}
		set
		{
			SetValue(specularPower, ref lastUsedSpecularPower, value);
		}
	}

	public float AlphaFactor
	{
		get
		{
			return lastUsedAlphaFactor;
		}
		set
		{
			SetValue(alphaFactor, ref lastUsedAlphaFactor, value);
		}
	}

	public Texture DiffuseTexture
	{
		get
		{
			return null;
		}
		set
		{
			SetValue(diffuseTexture, ref lastUsedDiffuseTexture, (Texture)(object)value?.XnaTexture);
		}
	}

	public Texture NormalTexture
	{
		get
		{
			return null;
		}
		set
		{
			SetValue(normalTexture, ref lastUsedNormalTexture, (Texture)(object)value?.XnaTexture);
		}
	}

	public Texture HeightTexture
	{
		get
		{
			return null;
		}
		set
		{
			SetValue(heightTexture, ref lastUsedHeightTexture, (Texture)(object)value?.XnaTexture);
		}
	}

	public TextureCube ReflectionCubeTexture
	{
		get
		{
			return lastUsedReflectionCubeTexture;
		}
		set
		{
			if (reflectionCubeTexture != null && lastUsedReflectionCubeTexture != value)
			{
				lastUsedReflectionCubeTexture = value;
				reflectionCubeTexture.SetValue((Texture)(object)value);
			}
		}
	}

	public Texture DetailTexture
	{
		get
		{
			return null;
		}
		set
		{
			SetValue(detailTexture, ref lastUsedDetailTexture, (Texture)(object)value?.XnaTexture);
		}
	}

	public float ParallaxAmount
	{
		get
		{
			return lastUsedParallaxAmount;
		}
		set
		{
			SetValue(parallaxAmount, ref lastUsedParallaxAmount, value);
		}
	}

	public Color CarHueColorChange
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lastUsedCarHueColorChange;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetValue(carHueColorChange, ref lastUsedCarHueColorChange, value);
		}
	}

	public EffectTechnique GetTechnique(string techniqueName)
	{
		return effect.Techniques[techniqueName];
	}

	private static void SetValue(EffectParameter param, ref Matrix lastUsedMatrix, Matrix newMatrix)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		lastUsedMatrix = newMatrix;
		param.SetValue(newMatrix);
	}

	private static void SetValue(EffectParameter param, ref Vector3 lastUsedVector, Vector3 newVector)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (param != null && lastUsedVector != newVector)
		{
			lastUsedVector = newVector;
			param.SetValue(newVector);
		}
	}

	private static void SetValue(EffectParameter param, ref Color lastUsedColor, Color newColor)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (param != null && ((Color)(ref lastUsedColor)).PackedValue != ((Color)(ref newColor)).PackedValue)
		{
			lastUsedColor = newColor;
			param.SetValue(((Color)(ref newColor)).ToVector4());
		}
	}

	private static void SetValue(EffectParameter param, ref float lastUsedValue, float newValue)
	{
		if (param != null && lastUsedValue != newValue)
		{
			lastUsedValue = newValue;
			param.SetValue(newValue);
		}
	}

	private static void SetValue(EffectParameter param, ref Texture lastUsedValue, Texture newValue)
	{
		if (param != null && lastUsedValue != newValue)
		{
			lastUsedValue = newValue;
			param.SetValue(newValue);
		}
	}

	public ShaderEffect(string shaderName)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		shaderContentName = "";
		lastUsedWorldViewProjMatrix = Matrix.Identity;
		lastUsedViewProjMatrix = Matrix.Identity;
		lastUsedInverseViewMatrix = Matrix.Identity;
		lastUsedProjectionMatrix = Matrix.Identity;
		lastUsedLightDir = Vector3.Zero;
		lastUsedAmbientColor = ColorHelper.Empty;
		lastUsedDiffuseColor = ColorHelper.Empty;
		lastUsedSpecularColor = ColorHelper.Empty;
		lastUsedParallaxAmount = -1f;
		lastUsedCarHueColorChange = ColorHelper.Empty;
		base._002Ector();
		if (BaseGame.Device == null)
		{
			throw new InvalidOperationException("XNA device is not initialized, can't create ShaderEffect.");
		}
		shaderContentName = Path.GetFileNameWithoutExtension(shaderName);
		Reload();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && effect != null)
		{
			effect.Dispose();
		}
	}

	public void Reload()
	{
		effect = BaseGame.Content.Load<Effect>(Path.Combine(Directories.ContentDirectory + "\\shaders", shaderContentName));
		ResetParameters();
		GetParameters();
	}

	protected virtual void ResetParameters()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		lastUsedInverseViewMatrix = Matrix.Identity;
		lastUsedAmbientColor = ColorHelper.Empty;
		lastUsedDiffuseTexture = null;
	}

	protected virtual void GetParameters()
	{
		worldViewProj = effect.Parameters["worldViewProj"];
		viewProj = effect.Parameters["viewProj"];
		world = effect.Parameters["world"];
		viewInverse = effect.Parameters["viewInverse"];
		projection = effect.Parameters["projection"];
		lightDir = effect.Parameters["lightDir"];
		ambientColor = effect.Parameters["ambientColor"];
		diffuseColor = effect.Parameters["diffuseColor"];
		specularColor = effect.Parameters["specularColor"];
		specularPower = effect.Parameters["specularPower"];
		alphaFactor = effect.Parameters["alphaFactor"];
		AlphaFactor = 1f;
		scale = effect.Parameters["scale"];
		diffuseTexture = effect.Parameters["diffuseTexture"];
		normalTexture = effect.Parameters["normalTexture"];
		heightTexture = effect.Parameters["heightTexture"];
		reflectionCubeTexture = effect.Parameters["reflectionCubeTexture"];
		detailTexture = effect.Parameters["detailTexture"];
		parallaxAmount = effect.Parameters["parallaxAmount"];
		carHueColorChange = effect.Parameters["carHueColorChange"];
		currTime = effect.Parameters["time"];
	}

	public virtual void SetParameters(Material setMat)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (worldViewProj != null)
		{
			worldViewProj.SetValue(BaseGame.WorldViewProjectionMatrix);
		}
		if (viewProj != null)
		{
			viewProj.SetValue(BaseGame.ViewProjectionMatrix);
		}
		if (world != null)
		{
			world.SetValue(BaseGame.WorldMatrix);
		}
		if (viewInverse != null)
		{
			viewInverse.SetValue(BaseGame.InverseViewMatrix);
		}
		if (lightDir != null)
		{
			lightDir.SetValue(BaseGame.LightDirection);
		}
		if (time <= 1f)
		{
			time += 0.0003f;
		}
		else
		{
			time = 0f;
		}
		if (currTime != null)
		{
			currTime.SetValue(time);
		}
		if (lastUsedReflectionCubeTexture == null && reflectionCubeTexture != null)
		{
			ReflectionCubeTexture = BaseGame.UI.SkyCubeMapTexture;
		}
		if (setMat != null)
		{
			AmbientColor = setMat.ambientColor;
			DiffuseColor = setMat.diffuseColor;
			SpecularColor = setMat.specularColor;
			SpecularPower = setMat.specularPower;
			DiffuseTexture = setMat.diffuseTexture;
			NormalTexture = setMat.normalTexture;
			HeightTexture = setMat.heightTexture;
			ParallaxAmount = setMat.parallaxAmount;
			DetailTexture = setMat.detailTexture;
		}
	}

	public virtual void SetParameters()
	{
		SetParameters(null);
	}

	public virtual void SetParametersOptimizedGeneral()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (worldViewProj != null)
		{
			worldViewProj.SetValue(BaseGame.WorldViewProjectionMatrix);
		}
		if (viewProj != null)
		{
			viewProj.SetValue(BaseGame.ViewProjectionMatrix);
		}
		if (world != null)
		{
			world.SetValue(BaseGame.WorldMatrix);
		}
		if (viewInverse != null)
		{
			viewInverse.SetValue(BaseGame.InverseViewMatrix);
		}
		if (lightDir != null)
		{
			lightDir.SetValue(BaseGame.LightDirection);
		}
		if (lastUsedReflectionCubeTexture == null && reflectionCubeTexture != null)
		{
			ReflectionCubeTexture = BaseGame.UI.SkyCubeMapTexture;
		}
		lastUsedAmbientColor = ColorHelper.Empty;
		lastUsedDiffuseColor = ColorHelper.Empty;
		lastUsedSpecularColor = ColorHelper.Empty;
		lastUsedDiffuseTexture = null;
		lastUsedNormalTexture = null;
	}

	public virtual void SetParametersOptimized(Material setMat)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (setMat == null)
		{
			throw new ArgumentNullException("setMat");
		}
		ambientColor.SetValue(((Color)(ref setMat.ambientColor)).ToVector4());
		diffuseColor.SetValue(((Color)(ref setMat.diffuseColor)).ToVector4());
		specularColor.SetValue(((Color)(ref setMat.specularColor)).ToVector4());
		if (setMat.diffuseTexture != null)
		{
			diffuseTexture.SetValue((Texture)(object)setMat.diffuseTexture.XnaTexture);
		}
		if (setMat.normalTexture != null)
		{
			normalTexture.SetValue((Texture)(object)setMat.normalTexture.XnaTexture);
		}
	}

	public void Update()
	{
		effect.CommitChanges();
	}

	public void Render(Material setMat, string techniqueName, BaseGame.RenderHandler renderCode)
	{
		if (techniqueName == null)
		{
			throw new ArgumentNullException("techniqueName");
		}
		if (renderCode == null)
		{
			throw new ArgumentNullException("renderCode");
		}
		SetParameters(setMat);
		effect.CurrentTechnique = effect.Techniques[techniqueName];
		try
		{
			effect.Begin((SaveStateMode)0);
			for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
			{
				EffectPass val = effect.CurrentTechnique.Passes[i];
				val.Begin();
				renderCode();
				val.End();
			}
		}
		finally
		{
			effect.End();
		}
	}

	public void Render(string techniqueName, BaseGame.RenderHandler renderDelegate)
	{
		Render(null, techniqueName, renderDelegate);
	}

	public void RenderSinglePassShader(BaseGame.RenderHandler renderCode)
	{
		if (renderCode == null)
		{
			throw new ArgumentNullException("renderCode");
		}
		try
		{
			effect.Begin((SaveStateMode)0);
			effect.CurrentTechnique.Passes[0].Begin();
			renderCode();
			effect.CurrentTechnique.Passes[0].End();
		}
		finally
		{
			effect.End();
		}
	}
}
