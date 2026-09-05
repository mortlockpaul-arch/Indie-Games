using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2d_house_of_terror;

public class sprite
{
	private Texture2D sheet;

	private uint frame_width;

	private uint frame_height;

	private uint current_state;

	private uint number_of_states;

	private uint anim_counter;

	private uint anim_speed;

	private uint max_anim_counter;

	private float current_radian;

	private float zoom_factor;

	private float current_opacity = 1f;

	private bool animate_backwards;

	public int width => (int)frame_width;

	public int height => (int)frame_height;

	public uint state
	{
		get
		{
			return current_state;
		}
		set
		{
			current_state = ((value >= 0 && value <= number_of_states) ? value : current_state);
		}
	}

	public uint frame
	{
		get
		{
			return anim_counter / anim_speed;
		}
		set
		{
			anim_counter = ((anim_speed * value >= max_anim_counter) ? anim_counter : (anim_speed * value));
		}
	}

	public bool done => anim_counter == max_anim_counter - 1;

	public bool cycle_finished
	{
		get
		{
			if (animate_backwards)
			{
				return anim_counter == 1;
			}
			return false;
		}
	}

	public float radian
	{
		get
		{
			return current_radian;
		}
		set
		{
			current_radian = value;
			current_radian = (float)((current_radian < 0f) ? (Math.PI * 2.0 + (double)current_radian) : (((double)current_radian > Math.PI * 2.0) ? ((double)current_radian - Math.PI * 2.0) : ((double)current_radian)));
		}
	}

	public float opacity
	{
		get
		{
			return current_opacity;
		}
		set
		{
			current_opacity = ((value >= 0f && value <= 1f) ? value : current_opacity);
		}
	}

	public float zoom
	{
		get
		{
			return zoom_factor;
		}
		set
		{
			zoom_factor = value;
		}
	}

	public sprite(Texture2D sheet_img, uint num_of_frames, uint num_of_states, uint fps)
	{
		sheet = sheet_img;
		number_of_states = num_of_states;
		frame_width = (uint)sheet_img.Width / num_of_frames;
		frame_height = (uint)sheet_img.Height / number_of_states;
		anim_counter = 0u;
		anim_speed = 60 / fps;
		anim_speed = ((anim_speed == 0) ? 1u : anim_speed);
		max_anim_counter = anim_speed * num_of_frames;
		zoom = 1f;
	}

	public virtual void animate()
	{
		anim_counter++;
		anim_counter %= max_anim_counter;
	}

	public virtual void animate_cyclic(uint min_frame, uint max_frame)
	{
		max_frame++;
		if (animate_backwards)
		{
			if (anim_counter > min_frame * anim_speed)
			{
				anim_counter--;
			}
			animate_backwards = anim_counter > min_frame * anim_speed;
		}
		else
		{
			if (anim_counter < max_frame * anim_speed - 1)
			{
				anim_counter++;
			}
			animate_backwards = anim_counter >= max_frame * anim_speed - 1;
		}
	}

	public virtual void animate_cyclic()
	{
		animate_cyclic(0u, max_anim_counter / anim_speed - 1);
	}

	public virtual void draw(SpriteBatch spr_batch, int dest_x, int dest_y, bool h_flipped = false)
	{
		int num = (int)((float)frame_width * zoom_factor);
		int num2 = (int)((float)frame_height * zoom_factor);
		spr_batch.Draw(sheet, new Rectangle(dest_x, dest_y, num, num2), new Rectangle((int)(frame_width * (anim_counter / anim_speed)), (int)(frame_height * current_state), (int)frame_width, (int)frame_height), Color.White * current_opacity, current_radian, new Vector2(frame_width / 2, frame_height / 2), h_flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
	}
}
