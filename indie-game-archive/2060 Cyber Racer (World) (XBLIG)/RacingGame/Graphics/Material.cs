using System;
using Microsoft.Xna.Framework.Graphics;

namespace RacingGame.Graphics;

public class Material : IDisposable
{
	private const float DefaultSpecularPower = 24f;

	public const float DefaultParallaxAmount = 0.04f;

	public static readonly Color DefaultAmbientColor;

	public static readonly Color DefaultDiffuseColor;

	public static readonly Color DefaultSpecularColor;

	public Color diffuseColor;

	public Color ambientColor;

	public Color specularColor;

	public float specularPower;

	public Texture diffuseTexture;

	public Texture normalTexture;

	public Texture heightTexture;

	public Texture detailTexture;

	public float parallaxAmount;

	public bool HasAlpha
	{
		get
		{
			if (diffuseTexture != null)
			{
				return diffuseTexture.HasAlphaPixels;
			}
			return false;
		}
	}

	public Material()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		diffuseColor = DefaultDiffuseColor;
		ambientColor = DefaultAmbientColor;
		specularColor = DefaultSpecularColor;
		specularPower = 24f;
		parallaxAmount = 0.04f;
		base._002Ector();
	}

	public Material(string setDiffuseTexture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		diffuseColor = DefaultDiffuseColor;
		ambientColor = DefaultAmbientColor;
		specularColor = DefaultSpecularColor;
		specularPower = 24f;
		parallaxAmount = 0.04f;
		base._002Ector();
		diffuseTexture = new Texture(setDiffuseTexture);
	}

	public Material(Color setAmbientColor, Color setDiffuseColor, string setDiffuseTexture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		diffuseColor = DefaultDiffuseColor;
		ambientColor = DefaultAmbientColor;
		specularColor = DefaultSpecularColor;
		specularPower = 24f;
		parallaxAmount = 0.04f;
		base._002Ector();
		ambientColor = setAmbientColor;
		diffuseColor = setDiffuseColor;
		diffuseTexture = new Texture(setDiffuseTexture);
	}

	public Material(Color setAmbientColor, Color setDiffuseColor, Texture setDiffuseTexture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		diffuseColor = DefaultDiffuseColor;
		ambientColor = DefaultAmbientColor;
		specularColor = DefaultSpecularColor;
		specularPower = 24f;
		parallaxAmount = 0.04f;
		base._002Ector();
		ambientColor = setAmbientColor;
		diffuseColor = setDiffuseColor;
		diffuseTexture = setDiffuseTexture;
	}

	public Material(string setDiffuseTexture, string setNormalTexture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		diffuseColor = DefaultDiffuseColor;
		ambientColor = DefaultAmbientColor;
		specularColor = DefaultSpecularColor;
		specularPower = 24f;
		parallaxAmount = 0.04f;
		base._002Ector();
		diffuseTexture = new Texture(setDiffuseTexture);
		normalTexture = new Texture(setNormalTexture);
	}

	public Material(string setDiffuseTexture, string setNormalTexture, string setHeightTexture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		diffuseColor = DefaultDiffuseColor;
		ambientColor = DefaultAmbientColor;
		specularColor = DefaultSpecularColor;
		specularPower = 24f;
		parallaxAmount = 0.04f;
		base._002Ector();
		diffuseTexture = new Texture(setDiffuseTexture);
		normalTexture = new Texture(setNormalTexture);
		heightTexture = new Texture(setHeightTexture);
	}

	public Material(Color setAmbientColor, Color setDiffuseColor, Color setSpecularColor, string setDiffuseTexture, string setNormalTexture, string setHeightTexture, string setDetailTexture)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		diffuseColor = DefaultDiffuseColor;
		ambientColor = DefaultAmbientColor;
		specularColor = DefaultSpecularColor;
		specularPower = 24f;
		parallaxAmount = 0.04f;
		base._002Ector();
		ambientColor = setAmbientColor;
		diffuseColor = setDiffuseColor;
		specularColor = setSpecularColor;
		diffuseTexture = new Texture(setDiffuseTexture);
		if (!string.IsNullOrEmpty(setNormalTexture))
		{
			normalTexture = new Texture(setNormalTexture);
		}
		if (!string.IsNullOrEmpty(setHeightTexture))
		{
			heightTexture = new Texture(setHeightTexture);
		}
		if (!string.IsNullOrEmpty(setDetailTexture))
		{
			detailTexture = new Texture(setDetailTexture);
		}
	}

	public Material(Effect effect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		diffuseColor = DefaultDiffuseColor;
		ambientColor = DefaultAmbientColor;
		specularColor = DefaultSpecularColor;
		specularPower = 24f;
		parallaxAmount = 0.04f;
		base._002Ector();
		if (effect == null)
		{
			throw new ArgumentNullException("effect");
		}
		EffectParameter val = effect.Parameters["diffuseTexture"];
		if (val != null)
		{
			diffuseTexture = new Texture(val.GetValueTexture2D());
		}
		EffectParameter val2 = effect.Parameters["normalTexture"];
		if (val2 != null)
		{
			normalTexture = new Texture(val2.GetValueTexture2D());
		}
		EffectParameter val3 = effect.Parameters["diffuseColor"];
		if (val3 != null)
		{
			diffuseColor = new Color(val3.GetValueVector4());
		}
		EffectParameter val4 = effect.Parameters["ambientColor"];
		if (val4 != null)
		{
			ambientColor = new Color(val4.GetValueVector4());
		}
		EffectParameter val5 = effect.Parameters["specularColor"];
		if (val5 != null)
		{
			specularColor = new Color(val5.GetValueVector4());
		}
		EffectParameter val6 = effect.Parameters["specularPower"];
		if (val6 != null)
		{
			specularPower = val6.GetValueSingle();
		}
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
			if (diffuseTexture != null)
			{
				diffuseTexture.Dispose();
			}
			if (normalTexture != null)
			{
				normalTexture.Dispose();
			}
			if (heightTexture != null)
			{
				heightTexture.Dispose();
			}
			if (detailTexture != null)
			{
				detailTexture.Dispose();
			}
		}
	}

	static Material()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		DefaultAmbientColor = new Color((byte)40, (byte)140, (byte)40);
		DefaultDiffuseColor = new Color((byte)210, (byte)210, (byte)210);
		DefaultSpecularColor = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
	}
}
