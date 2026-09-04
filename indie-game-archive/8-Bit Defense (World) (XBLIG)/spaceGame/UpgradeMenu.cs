using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spaceGame;

public class UpgradeMenu : Sprite
{
	private int upgradeCost;

	private int upgradeAvailable;

	private int highlighterSelction;

	private bool bActive;

	private bool delay = true;

	private Game1 theGame;

	private Texture2D marker;

	private Texture2D highlighter;

	private Vector2 markerPos;

	private Vector2 highlighterPos;

	private SpriteBatch theSpriteBatch;

	private SpriteFont font;

	private KeyboardState aCurrentKeyboardState;

	private GamePadState gamePadState;

	private KeyboardState oldKeyBoardState;

	private GamePadState oldGamePadState;

	public UpgradeMenu(Game1 getGame)
	{
		theGame = getGame;
		bActive = false;
		Position = new Vector2(450f, 200f);
		highlighterSelction = 0;
		upgradeAvailable = 0;
		upgradeCost = 500;
	}

	public void LoadContent(ContentManager theContentManager)
	{
		highlighter = theContentManager.Load<Texture2D>("UpgradeSelectionSprite");
		marker = theContentManager.Load<Texture2D>("UpgradeMarkerSprite");
		LoadContent(theContentManager, "UpgradeMenuSprite");
		theSpriteBatch = new SpriteBatch(theGame.GraphicsDevice);
		font = theContentManager.Load<SpriteFont>("HighScoreFont");
	}

	public void Update(GameTime theGameTime)
	{
		aCurrentKeyboardState = Keyboard.GetState();
		gamePadState = GamePad.GetState(theGame.ThePlayer);
		UpdateUpgradeMenu(aCurrentKeyboardState, gamePadState);
		delay = false;
		if (aCurrentKeyboardState.IsKeyDown(Keys.B) || GamePad.GetState(theGame.ThePlayer).Buttons.B == ButtonState.Pressed)
		{
			SetActive(num: false);
			delay = true;
		}
		oldKeyBoardState = aCurrentKeyboardState;
		oldGamePadState = gamePadState;
	}

	public void UpdateUpgradeMenu(KeyboardState aCurrentKeyboardState, GamePadState gamePadState)
	{
		if (GamePad.GetState(theGame.ThePlayer).ThumbSticks.Left.X == 1f && oldGamePadState.ThumbSticks.Left.X != 1f)
		{
			theGame.SoundUpgradeSelect();
			switch (highlighterSelction)
			{
			case 0:
				highlighterSelction = 1;
				break;
			case 1:
				highlighterSelction = 2;
				break;
			case 2:
				highlighterSelction = 0;
				break;
			}
		}
		if (GamePad.GetState(theGame.ThePlayer).ThumbSticks.Left.X == -1f && oldGamePadState.ThumbSticks.Left.X != -1f)
		{
			theGame.SoundUpgradeSelect();
			switch (highlighterSelction)
			{
			case 0:
				highlighterSelction = 2;
				break;
			case 1:
				highlighterSelction = 0;
				break;
			case 2:
				highlighterSelction = 1;
				break;
			}
		}
		if (aCurrentKeyboardState.IsKeyDown(Keys.Right) && !oldKeyBoardState.IsKeyDown(Keys.Right))
		{
			theGame.SoundUpgradeSelect();
			switch (highlighterSelction)
			{
			case 0:
				highlighterSelction = 1;
				break;
			case 1:
				highlighterSelction = 2;
				break;
			case 2:
				highlighterSelction = 0;
				break;
			}
		}
		if (aCurrentKeyboardState.IsKeyDown(Keys.Left) && !oldKeyBoardState.IsKeyDown(Keys.Left))
		{
			theGame.SoundUpgradeSelect();
			switch (highlighterSelction)
			{
			case 0:
				highlighterSelction = 2;
				break;
			case 1:
				highlighterSelction = 0;
				break;
			case 2:
				highlighterSelction = 1;
				break;
			}
		}
		if (theGame.mMainShipSprite.GetPoints() >= upgradeCost)
		{
			upgradeAvailable = 1;
		}
		if (delay)
		{
			return;
		}
		if (aCurrentKeyboardState.IsKeyDown(Keys.A) && !oldKeyBoardState.IsKeyDown(Keys.A) && upgradeAvailable == 1)
		{
			switch (highlighterSelction)
			{
			case 0:
				if (theGame.mMainShipSprite.GetMachineGunLevel() < 4)
				{
					ApplyUpgrade(highlighterSelction);
				}
				break;
			case 1:
				if (theGame.mMainShipSprite.GetPenetrationLevel() < 4)
				{
					ApplyUpgrade(highlighterSelction);
				}
				break;
			case 2:
				if (theGame.mMainShipSprite.GetSpreadShotLevel() < 4)
				{
					ApplyUpgrade(highlighterSelction);
				}
				break;
			}
		}
		if (gamePadState.Buttons.A != ButtonState.Pressed || oldGamePadState.Buttons.A == ButtonState.Pressed || upgradeAvailable != 1)
		{
			return;
		}
		switch (highlighterSelction)
		{
		case 0:
			if (theGame.mMainShipSprite.GetMachineGunLevel() < 4)
			{
				ApplyUpgrade(highlighterSelction);
			}
			break;
		case 1:
			if (theGame.mMainShipSprite.GetPenetrationLevel() < 4)
			{
				ApplyUpgrade(highlighterSelction);
			}
			break;
		case 2:
			if (theGame.mMainShipSprite.GetSpreadShotLevel() < 4)
			{
				ApplyUpgrade(highlighterSelction);
			}
			break;
		}
	}

	public void ApplyUpgrade(int upgradeType)
	{
		theGame.SoundUpgradeMove();
		theGame.mMainShipSprite.ChangePoints(-upgradeCost);
		upgradeCost *= 2;
		upgradeAvailable--;
		switch (upgradeType)
		{
		case 0:
			theGame.mMainShipSprite.ChangeMachineGunLevel();
			break;
		case 1:
			theGame.mMainShipSprite.ChangePenetrationLevel();
			break;
		case 2:
			theGame.mMainShipSprite.ChangeSpreadShotLevel();
			break;
		}
	}

	public void SetActive(bool num)
	{
		bActive = num;
	}

	public bool GetActive()
	{
		return bActive;
	}

	public override void Draw(SpriteBatch theSpriteBatch)
	{
		base.Draw(theSpriteBatch);
		switch (highlighterSelction)
		{
		case 0:
			highlighterPos = new Vector2(462f, 212f);
			theSpriteBatch.Draw(highlighter, highlighterPos, Color.White);
			break;
		case 1:
			highlighterPos = new Vector2(652f, 212f);
			theSpriteBatch.Draw(highlighter, highlighterPos, Color.White);
			break;
		case 2:
			highlighterPos = new Vector2(462f, 302f);
			theSpriteBatch.Draw(highlighter, highlighterPos, Color.White);
			break;
		}
		switch (theGame.mMainShipSprite.GetMachineGunLevel())
		{
		case 1:
			markerPos = new Vector2(556f, 235f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			break;
		case 2:
			markerPos = new Vector2(582f, 235f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			goto case 1;
		case 3:
			markerPos = new Vector2(608f, 235f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			goto case 2;
		case 4:
			markerPos = new Vector2(634f, 235f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			goto case 3;
		}
		switch (theGame.mMainShipSprite.GetPenetrationLevel())
		{
		case 1:
			markerPos = new Vector2(746f, 235f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			break;
		case 2:
			markerPos = new Vector2(772f, 235f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			goto case 1;
		case 3:
			markerPos = new Vector2(798f, 235f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			goto case 2;
		case 4:
			markerPos = new Vector2(824f, 235f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			goto case 3;
		}
		switch (theGame.mMainShipSprite.GetSpreadShotLevel())
		{
		case 1:
			markerPos = new Vector2(556f, 325f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			break;
		case 2:
			markerPos = new Vector2(582f, 325f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			goto case 1;
		case 3:
			markerPos = new Vector2(608f, 325f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			goto case 2;
		case 4:
			markerPos = new Vector2(634f, 325f);
			theSpriteBatch.Draw(marker, markerPos, Color.White);
			goto case 3;
		}
		theSpriteBatch.DrawString(font, string.Concat(upgradeCost), new Vector2(726f, 313f), Color.White);
		theSpriteBatch.DrawString(font, string.Concat(theGame.mMainShipSprite.GetPoints()), new Vector2(726f, 354f), Color.White);
	}
}
