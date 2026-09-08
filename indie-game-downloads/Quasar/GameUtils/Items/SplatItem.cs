using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Items;

public class SplatItem : RenderItem
{
	private const int DURATION = -1;

	private const int SPLAT_DURATION = 125;

	private int splatDuration = 125;

	private int duration = -1;

	private TextMesh text;

	private Sized2DRectangleMesh bgMesh;

	private bool doSplat;

	private long showTime = 1L;

	public int SplatDuration
	{
		get
		{
			return splatDuration;
		}
		set
		{
			splatDuration = value;
		}
	}

	public int Duration
	{
		get
		{
			return duration;
		}
		set
		{
			duration = value;
		}
	}

	public float TextScale
	{
		get
		{
			return text.Scale;
		}
		set
		{
			text.Scale = value;
		}
	}

	public string Text
	{
		set
		{
			text.Text = value;
		}
	}

	public SplatItem(Vector2 size, BitmapFont textFont, Texture2D texture)
	{
		bgMesh = new Sized2DRectangleMesh(size, texture);
		addMesh(bgMesh);
		text = new TextMesh(textFont, new TextDrawProperties(HorizontalAlignment.Center), 40, useStringBuilder: false);
		addMesh(text);
	}

	public void DoSplat()
	{
		DoSplat(0);
	}

	public void DoSplat(int timeOffset)
	{
		Visible = false;
		bgMesh.Rotation = GameMath.Random.NextFloat(-0.15f, 0.15f);
		showTime = Timer.DefaultTimer.TotalTime + timeOffset;
		doSplat = true;
	}

	protected override void DoUpdate()
	{
		long totalTime = Timer.DefaultTimer.TotalTime;
		if (doSplat)
		{
			if (totalTime >= showTime)
			{
				Visible = true;
				float num = (float)(totalTime - showTime) / (float)splatDuration;
				Transform.Scale = new Vector3(GameMath.Clamp(1f, 3f, GameMath.Interpolate(3f, 1f, num)));
				if (num >= 1f)
				{
					doSplat = false;
				}
			}
			else
			{
				Visible = false;
			}
		}
		if (Visible && duration != -1 && totalTime > showTime + duration)
		{
			Visible = false;
		}
		base.DoUpdate();
	}
}
