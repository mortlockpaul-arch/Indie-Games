using System;

namespace SgMotion.Controllers;

public interface IBlendController : IBlendable
{
	BlendMode BlendMode { get; set; }

	InterpolationMode TranslationInterpolation { get; set; }

	InterpolationMode OrientationInterpolation { get; set; }

	InterpolationMode ScaleInterpolation { get; set; }

	void SetBlendLayer(IBlendable blendable, BlendLayer blendLayer);

	void SetWeightInterpolation(float value, float desiredValue, TimeSpan time, BlendLayer blendLayer);

	void UpdateBlend();
}
