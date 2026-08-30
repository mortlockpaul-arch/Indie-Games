using Microsoft.Xna.Framework;

namespace TechArts
{
	public class ParticleReq
	{
		public int n;

		public Vector2 pos;

		public ParticleReq(Vector2 p, int q)
		{
			pos = p;
			n = q;
		}
	}
}
