using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace _2d_house_of_terror;

public class award_ceremony
{
	private const int max_counter = 480;

	private const int max_zoom_counter = 600;

	private SpriteBatch spr_batch;

	private fade_effect fade;

	private Texture2D bg;

	private Texture2D[][] players;

	private Vector2[] positions;

	private int counter;

	public award_ceremony(ContentManager con_mgr, SpriteBatch sprite_batch)
	{
		spr_batch = sprite_batch;
		fade = new fade_effect(spr_batch.GraphicsDevice);
		fade.to_color(240u, Color.Black, fadein: true);
		bg = con_mgr.Load<Texture2D>("menu/award_ceremony/bg");
		con_mgr.Load<SoundEffect>("sfx/cheering").Play();
		players = new Texture2D[4][]
		{
			new Texture2D[3]
			{
				con_mgr.Load<Texture2D>("menu/award_ceremony/jimmy_happy"),
				con_mgr.Load<Texture2D>("menu/award_ceremony/jimmy_normal"),
				con_mgr.Load<Texture2D>("menu/award_ceremony/jimmy_sad")
			},
			new Texture2D[3]
			{
				con_mgr.Load<Texture2D>("menu/award_ceremony/sam_happy"),
				con_mgr.Load<Texture2D>("menu/award_ceremony/sam_normal"),
				con_mgr.Load<Texture2D>("menu/award_ceremony/sam_sad")
			},
			new Texture2D[3]
			{
				con_mgr.Load<Texture2D>("menu/award_ceremony/erik_happy"),
				con_mgr.Load<Texture2D>("menu/award_ceremony/erik_normal"),
				con_mgr.Load<Texture2D>("menu/award_ceremony/erik_sad")
			},
			new Texture2D[3]
			{
				con_mgr.Load<Texture2D>("menu/award_ceremony/billy_happy"),
				con_mgr.Load<Texture2D>("menu/award_ceremony/billy_normal"),
				con_mgr.Load<Texture2D>("menu/award_ceremony/billy_sad")
			}
		};
		positions = new Vector2[4]
		{
			new Vector2(338f, 323f),
			new Vector2(249f, 348f),
			new Vector2(424f, 348f),
			new Vector2(540f, 400f)
		};
	}

	~award_ceremony()
	{
		free();
	}

	private void free()
	{
		MediaPlayer.Stop();
	}

	public bool update()
	{
		counter++;
		if (counter < 580)
		{
			fade.update();
		}
		if (counter == 480)
		{
			fade.weird(150u, Color.Black);
		}
		return counter > 600;
	}

	public void draw()
	{
		float num = 1f + (float)(counter - 480) / 120f;
		num = ((num < 1f) ? 1f : num);
		Vector2 vector = new Vector2((num - 1f) * -760f, (num - 1f) * -1f * 470f);
		spr_batch.Begin();
		spr_batch.Draw(bg, new Rectangle((int)vector.X, (int)vector.Y, (int)((float)bg.Width * num), (int)((float)bg.Height * num)), new Rectangle(0, 0, bg.Width, bg.Height), Color.White);
		for (int i = 0; i < 4; i++)
		{
			Texture2D texture2D = players[game_mgr.char_ids[i]][(game_mgr.ranking[i] != 0) ? ((game_mgr.ranking[i] != 3) ? 1 : 2) : 0];
			spr_batch.Draw(texture2D, new Rectangle((int)vector.X + (int)(num * (positions[game_mgr.ranking[i]].X - (float)(texture2D.Width / 2))), (int)vector.Y + (int)(num * (positions[game_mgr.ranking[i]].Y - (float)texture2D.Height)), (int)((float)texture2D.Width * num), (int)((float)texture2D.Height * num)), new Rectangle(0, 0, texture2D.Width, texture2D.Height), Color.White);
		}
		fade.draw(spr_batch);
		spr_batch.End();
	}
}
