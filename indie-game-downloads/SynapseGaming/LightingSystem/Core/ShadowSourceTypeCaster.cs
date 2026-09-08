using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Core;

/// <summary />
public class ShadowSourceTypeCaster : ITypeCaster<IShadowSource>
{
	/// <summary />
	public IPointSource PointSource;

	/// <summary />
	public ISpotSource SpotSource;

	/// <summary />
	public IDirectionalSource DirectionalSource;

	/// <summary />
	public void Set(IShadowSource light)
	{
		PointSource = light as IPointSource;
		SpotSource = light as ISpotSource;
		DirectionalSource = light as IDirectionalSource;
	}
}
