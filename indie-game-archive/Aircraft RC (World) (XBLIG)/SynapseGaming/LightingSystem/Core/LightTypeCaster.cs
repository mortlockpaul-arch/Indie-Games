using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Core;

/// <summary />
public class LightTypeCaster : ITypeCaster<BaseLight>
{
	/// <summary />
	public IPointSource PointSource;

	/// <summary />
	public ISpotSource SpotSource;

	/// <summary />
	public IDirectionalSource DirectionalSource;

	/// <summary />
	public IAmbientSource AmbientSource;

	/// <summary />
	public IShadowSource ShadowSource;

	/// <summary />
	public void Set(BaseLight light)
	{
		PointSource = light as IPointSource;
		SpotSource = light as ISpotSource;
		DirectionalSource = light as IDirectionalSource;
		AmbientSource = light as IAmbientSource;
		ShadowSource = light as IShadowSource;
	}
}
