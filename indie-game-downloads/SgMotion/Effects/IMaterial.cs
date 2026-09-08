using Microsoft.Xna.Framework;

namespace SgMotion.Effects;

public interface IMaterial
{
	Vector3 EmissiveColor { get; set; }

	Vector3 DiffuseColor { get; set; }

	Vector3 SpecularColor { get; set; }

	float SpecularPower { get; set; }
}
