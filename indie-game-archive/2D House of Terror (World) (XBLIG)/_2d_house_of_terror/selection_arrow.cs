using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2d_house_of_terror;

public class selection_arrow
{
	private Texture2D image;

	private double pos_x;

	private double pos_y;

	private double mov_x;

	private double mov_y;

	private double max_x;

	private double max_y;

	public selection_arrow(Texture2D img, double mx, double my, double x_max, double y_max)
	{
		image = img;
		mov_x = mx;
		mov_y = my;
		max_x = x_max;
		max_y = y_max;
	}

	public int width()
	{
		return image.Width;
	}

	public int height()
	{
		return image.Height;
	}

	public void update()
	{
		pos_x += mov_x;
		pos_y += mov_y;
		mov_x = ((pos_x < 0.0 || pos_x > max_x) ? (0.0 - mov_x) : mov_x);
		mov_y = ((pos_y < 0.0 || pos_y > max_y) ? (0.0 - mov_y) : mov_y);
	}

	public void draw(SpriteBatch spr_batch, float x, float y, bool swap_direction = false)
	{
		spr_batch.Draw(image, new Vector2(x + (float)(swap_direction ? (0.0 - pos_x) : pos_x), y + (float)(swap_direction ? (0.0 - pos_y) : pos_y)), Color.White);
	}
}
