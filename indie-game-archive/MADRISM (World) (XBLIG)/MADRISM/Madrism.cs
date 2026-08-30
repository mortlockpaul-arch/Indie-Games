using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TechArts;

namespace MADRISM
{
	public class Madrism : GameEngine
	{
		public Madrism()
		{
			MediaPlayer.Volume = 0.35f;
			SoundEffect.MasterVolume = 0.5f;
		}

		protected override void LoadContent()
		{
			base.LoadContent();
			tasks.Entry(new GlobalState());
		}
	}
}
