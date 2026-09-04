using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaLibrary.Graphics;

namespace XnaLibrary.Diagnostics;

public class TimeViewerComponent : DrawableGameComponent
{
	private const float BarSpacing = 1f;

	private readonly Vector2 DrawStartPosition;

	private readonly Rectangle BarSize;

	private readonly Vector3 offsetColor;

	private SpriteBatch spriteBatch;

	private Rectangle workRect;

	public float FrameRate { get; set; }

	public List<TimeWatcher> TimeWatchers { get; set; }

	public DrawHelperComponent DrawHelper => (DrawHelperComponent)((GameComponent)this).Game.Services.GetService(typeof(DrawHelperComponent));

	public TimeViewerComponent(Game game)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		DrawStartPosition = new Vector2(1213f, 36f);
		BarSize = new Rectangle(0, 0, 3, 648);
		offsetColor = new Vector3(0.3f, 0.3f, 0.3f);
		((DrawableGameComponent)this)._002Ector(game);
		TimeWatchers = new List<TimeWatcher>();
		FrameRate = 60f;
	}

	public override void Initialize()
	{
		((DrawableGameComponent)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		spriteBatch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
		((DrawableGameComponent)this).LoadContent();
	}

	protected override void UnloadContent()
	{
		spriteBatch.Dispose();
		((DrawableGameComponent)this).UnloadContent();
	}

	public override void Update(GameTime gameTime)
	{
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.Begin();
		Vector2 drawStartPosition = DrawStartPosition;
		DrawHelper.DrawLineRect(spriteBatch, new Rectangle((int)drawStartPosition.X, (int)drawStartPosition.Y, BarSize.Width, BarSize.Height), Color.Black);
		foreach (TimeWatcher timeWatcher in TimeWatchers)
		{
			Vector2 position = drawStartPosition;
			float num = (float)(timeWatcher.Time.Elapsed.TotalSeconds / (1.0 / (double)FrameRate));
			float num2 = (float)BarSize.Height * num;
			DrawLine(position, num2, timeWatcher.Color);
			position.Y += num2;
			drawStartPosition.Y += num2;
		}
		spriteBatch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}

	private void DrawLine(Vector2 position, float height, Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Color)(ref color)).ToVector3();
		Color color2 = default(Color);
		((Color)(ref color2))._002Ector(val + offsetColor);
		Color color3 = default(Color);
		((Color)(ref color3))._002Ector(val - offsetColor);
		int num = (int)position.X;
		int y = (int)position.Y;
		int height2 = (int)height;
		DrawHelper.SetRectangle(ref workRect, num, y, 1, height2);
		DrawHelper.DrawLineRect(spriteBatch, workRect, color2);
		DrawHelper.SetRectangle(ref workRect, num + 1, y, 1, height2);
		DrawHelper.DrawLineRect(spriteBatch, workRect, color);
		DrawHelper.SetRectangle(ref workRect, num + 2, y, 1, height2);
		DrawHelper.DrawLineRect(spriteBatch, workRect, color3);
	}

	public void Begin(TimeWatcher timeWatcher)
	{
		if (!TimeWatchers.Contains(timeWatcher))
		{
			TimeWatchers.Add(timeWatcher);
		}
		timeWatcher.Time.Reset();
		timeWatcher.Time.Start();
	}

	public void End(TimeWatcher timeWatcher)
	{
		timeWatcher.Time.Stop();
	}
}
