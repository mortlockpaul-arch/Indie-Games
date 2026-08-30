using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TechArts
{
	public class ParticleManager
	{
		private List<Particle> parts;

		private List<ParticleReq> preqs;

		public ParticleManager(Texture2D tex)
		{
			Particle.pt = tex;
			parts = new List<Particle>();
			preqs = new List<ParticleReq>();
		}

		public void Entry(Vector2 pos, int n)
		{
			preqs.Add(new ParticleReq(pos, n));
		}

		private void RequestProc()
		{
			int num = 128;
			while (preqs.Count > 0 && num > 0 && parts.Count < 3072)
			{
				int num2 = ((preqs[0].n > num) ? num : preqs[0].n);
				Vector2 pos = preqs[0].pos;
				for (int i = 0; i < num2; i++)
				{
					parts.Add(new Particle(pos));
				}
				num -= num2;
				preqs[0].n -= num2;
				if (preqs[0].n <= 0)
				{
					preqs.RemoveAt(0);
				}
			}
		}

		public void Update()
		{
			RequestProc();
			Queue<Particle> queue = new Queue<Particle>();
			foreach (Particle part in parts)
			{
				part.life -= part.dlife;
				if (part.life <= 0f)
				{
					queue.Enqueue(part);
					continue;
				}
				part.pos += part.dpos;
				part.dpos *= 0.98f;
				part.scl -= 0.01f;
				if (part.scl < 0f)
				{
					part.scl = 0f;
				}
			}
			while (queue.Count > 0)
			{
				parts.Remove(queue.Dequeue());
			}
		}

		public void Draw()
		{
			SpriteBatch spriteBatch = GameEngine.core.spriteBatch;
			foreach (Particle part in parts)
			{
				spriteBatch.Draw(Particle.pt, part.pos, null, new Color(1f, 1f, 1f, part.life), 0f, part.org, part.scl, SpriteEffects.None, 1f);
			}
		}

		public void Reset()
		{
			preqs.Clear();
			parts.Clear();
		}
	}
}
