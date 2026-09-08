using System;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Quasar.GameUtils.Game;
using Quasar.Global;

namespace Quasar.GameUtils.XBLIG.Game;

public class XBLIGGamer : IGamer
{
	private static readonly string ProfilePicturePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AvatarAssets", "profile", "tile_64.png");

	protected readonly Gamer gamer;

	public virtual string Gamertag => gamer.Gamertag;

	public XBLIGGamer(Gamer gamer)
	{
		this.gamer = gamer ?? new Gamer("Player1");
	}

	public virtual Texture2D GetTexture()
	{
		using FileStream stream = File.OpenRead(ProfilePicturePath);
		return Texture2D.FromStream(Engine.Device, stream);
	}
}
