using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class HPBar : DrawableGameComponent
{
	private const string cstrPlayerHp = "PNG\\System\\PlayerHP00";

	private const string cstrPlayerHpWaku = "PNG\\System\\PlayerHPWaku00";

	private const string cstrCPUBOSSHp = "PNG\\System\\CPUBossHP00";

	private const string cstrCPUBOSSHpWaku = "PNG\\System\\CPUBossHPWaku00";

	private SpriteFont font1;

	private Texture2D imgPlayerHp;

	private Texture2D imgPlayerHpWaku;

	private Texture2D imgCPUBOSSHp;

	private Texture2D imgCPUBOSSHpWaku;

	public HPBar(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		font1 = base.Game.Content.Load<SpriteFont>("SpriteFont1");
		imgPlayerHp = base.Game.Content.Load<Texture2D>("PNG\\System\\PlayerHP00");
		imgPlayerHpWaku = base.Game.Content.Load<Texture2D>("PNG\\System\\PlayerHPWaku00");
		imgCPUBOSSHp = base.Game.Content.Load<Texture2D>("PNG\\System\\CPUBossHP00");
		imgCPUBOSSHpWaku = base.Game.Content.Load<Texture2D>("PNG\\System\\CPUBossHPWaku00");
		base.LoadContent();
	}

	public void PlayerHpDraw(SpriteBatch aspritesBatch, int intHp)
	{
		if (imgPlayerHp != null)
		{
			aspritesBatch.Draw(imgPlayerHp, new Vector2(640 - imgPlayerHp.Width / 2, 630f), new Rectangle(0, 0, intHp, imgPlayerHp.Height), new Color(200, 200, 200, 200), MathHelper.ToRadians(0f), new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 1f);
			aspritesBatch.Draw(imgPlayerHpWaku, new Vector2(640 - imgPlayerHp.Width / 2 - 2, 628f), null, Color.White, MathHelper.ToRadians(0f), new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 1f);
			aspritesBatch.DrawString(font1, "Player", new Vector2(640 - imgPlayerHp.Width / 2 + 5, 620f), Color.White);
		}
	}

	public void CPUBOSSHpDraw(SpriteBatch aspritesBatch, int intHp)
	{
		if (imgCPUBOSSHp != null)
		{
			if (intHp <= 1000)
			{
				aspritesBatch.Draw(imgCPUBOSSHp, new Vector2(640 - imgCPUBOSSHp.Width / 2, 660f), new Rectangle(0, 0, intHp, imgCPUBOSSHp.Height), new Color(200, 200, 200, 200), MathHelper.ToRadians(0f), new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 1f);
			}
			else
			{
				aspritesBatch.Draw(imgCPUBOSSHp, new Vector2(640 - imgCPUBOSSHp.Width / 2, 660f), new Rectangle(0, 0, 1000, imgCPUBOSSHp.Height), new Color(200, 200, 200, 200), MathHelper.ToRadians(0f), new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 1f);
			}
			aspritesBatch.Draw(imgCPUBOSSHpWaku, new Vector2(640 - imgCPUBOSSHp.Width / 2 - 2, 658f), null, Color.White, MathHelper.ToRadians(0f), new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 1f);
			aspritesBatch.DrawString(font1, "Boss", new Vector2(640 - imgPlayerHp.Width / 2 + 5, 666f), Color.White);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
