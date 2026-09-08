using Microsoft.Xna.Framework;

namespace SgMotion.Effects;

public interface IPointLight : ILight
{
	Vector3 Position { get; set; }
}
