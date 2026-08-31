using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// The Default Effect provided by DPSF.
/// </summary>
public class DPSFDefaultEffect : Effect
{
	/// <summary>
	/// The list of valid DPSF Default Effect configurations
	/// </summary>
	public enum DPSFDefaultEffectConfigurations
	{
		/// <summary>
		/// Windows HiDef configuration.
		/// </summary>
		WindowsHiDef,
		/// <summary>
		/// Windows Reach configuration.
		/// </summary>
		WindowsReach,
		/// <summary>
		/// Xbox 360 HiDef configuration.
		/// </summary>
		Xbox360HiDef
	}

	private EffectParameter _colorBlendAmountParameter;

	private EffectParameter _textureParameter;

	private EffectParameter _worldParameter;

	private EffectParameter _viewParameter;

	private EffectParameter _projectionParameter;

	/// <summary>
	/// Holds this effects configuration in case we need to clone it.
	/// </summary>
	private DPSFDefaultEffectConfigurations _configuration;

	/// <summary>
	/// How much of the vertex Color should be blended in with the Texture's Color.
	/// <para>0.0 = use Texture's color, 1.0 = use specified color. Default is 0.5.</para>
	/// </summary>
	public float ColorBlendAmount
	{
		get
		{
			return _colorBlendAmountParameter.GetValueSingle();
		}
		set
		{
			float num = value;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			_colorBlendAmountParameter.SetValue(num);
		}
	}

	/// <summary>
	/// The texture to use to draw the particles.
	/// </summary>
	public Texture2D Texture
	{
		get
		{
			return _textureParameter.GetValueTexture2D();
		}
		set
		{
			_textureParameter.SetValue(value);
		}
	}

	/// <summary>
	/// The World matrix.
	/// </summary>
	public Matrix World
	{
		get
		{
			return _worldParameter.GetValueMatrix();
		}
		set
		{
			_worldParameter.SetValue(value);
		}
	}

	/// <summary>
	/// The View matrix.
	/// </summary>
	public Matrix View
	{
		get
		{
			return _viewParameter.GetValueMatrix();
		}
		set
		{
			_viewParameter.SetValue(value);
		}
	}

	/// <summary>
	/// The Projection matrix.
	/// </summary>
	public Matrix Projection
	{
		get
		{
			return _projectionParameter.GetValueMatrix();
		}
		set
		{
			_projectionParameter.SetValue(value);
		}
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="graphicsDevice">The Graphics Device to load the effect with.</param>
	/// <param name="configuration">The effect configuration to load (i.e. Windows HiDef, Xbox 360 Reach, etc.)</param>
	public DPSFDefaultEffect(GraphicsDevice graphicsDevice, DPSFDefaultEffectConfigurations configuration)
		: base(graphicsDevice, configuration switch
		{
			DPSFDefaultEffectConfigurations.WindowsReach => DPSFResources.DPSFDefaultEffectWindowsReach, 
			DPSFDefaultEffectConfigurations.WindowsHiDef => DPSFResources.DPSFDefaultEffectWindowsHiDef, 
			_ => DPSFResources.DPSFDefaultEffectXbox360HiDef, 
		})
	{
		_colorBlendAmountParameter = base.Parameters["xColorBlendAmount"];
		_textureParameter = base.Parameters["xTexture"];
		_worldParameter = base.Parameters["xWorld"];
		_viewParameter = base.Parameters["xView"];
		_projectionParameter = base.Parameters["xProjection"];
		_configuration = configuration;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:DPSF.DPSFDefaultEffect" /> class.
	/// </summary>
	/// <param name="effectToClone">The effect to clone.</param>
	public DPSFDefaultEffect(DPSFDefaultEffect effectToClone)
		: this(effectToClone.GraphicsDevice, effectToClone._configuration)
	{
		ColorBlendAmount = effectToClone.ColorBlendAmount;
		Texture = effectToClone.Texture;
		World = effectToClone.World;
		View = effectToClone.View;
		Projection = effectToClone.Projection;
	}

	/// <summary>
	/// Creates and returns a clone of this DPSFDefaultEffect instance.
	/// </summary>
	public override Effect Clone()
	{
		return new DPSFDefaultEffect(this);
	}
}
