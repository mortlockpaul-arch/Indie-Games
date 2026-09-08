using Microsoft.Xna.Framework.Graphics;

namespace Quasar.GameUtils.Game;

public interface IGamer
{
	string Gamertag { get; }

	Texture2D GetTexture();
}
