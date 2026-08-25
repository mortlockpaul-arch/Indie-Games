using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace DepthAttack.CS;

public class Record(Game game) : DrawableGameComponent(game)
{
	public struct sctScore
	{
		public long lngScore;

		public string strName;
	}

	public const string pcStrContainerName = "DepthAttack00";

	public const string pcStrContainerSaveDataName = "RecordData.dat";

	private SpriteFont font1;

	private SpriteBatch spritesBatch;

	public PlayerIndex playerIndex;

	private GamePadState gamePadState;

	private GamePadState gamePadMaeState;

	public bool pflgRecordEnd = false;

	public int intOneScan;

	public sctScore[] pscoreRecord = new sctScore[10];

	public override void Initialize()
	{
		intOneScan = 0;
		pflgRecordEnd = false;
		base.Initialize();
	}

	public void recordInit()
	{
		for (long num = 0L; num < pscoreRecord.Length; num++)
		{
			pscoreRecord[num].lngScore = (10 - num) * 100000 + 2000000;
			pscoreRecord[num].strName = "AAAAA";
		}
	}

	public void recordRead()
	{
		if (Game1.pStorageDevice == null)
		{
			return;
		}
		IAsyncResult asyncResult = Game1.pStorageDevice.BeginOpenContainer("DepthAttack00", null, null);
		try
		{
			asyncResult.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = Game1.pStorageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			if (!storageContainer.FileExists("RecordData.dat"))
			{
				storageContainer.Dispose();
				return;
			}
			try
			{
				Stream stream = storageContainer.OpenFile("RecordData.dat", FileMode.Open);
				for (int i = 0; i < pscoreRecord.Length; i++)
				{
					byte[] array = new byte[8];
					byte[] array2 = new byte[5];
					char[] array3 = new char[5];
					stream.Read(array, 0, 8);
					pscoreRecord[i].lngScore = BitConverter.ToInt64(array, 0);
					stream.Read(array2, 0, 5);
					array3[0] = (char)array2[0];
					array3[1] = (char)array2[1];
					array3[2] = (char)array2[2];
					array3[3] = (char)array2[3];
					array3[4] = (char)array2[4];
					pscoreRecord[i].strName = array3[0].ToString() + array3[1] + array3[2] + array3[3] + array3[4];
				}
				stream.Close();
			}
			catch
			{
			}
			storageContainer.Dispose();
		}
		catch
		{
			Game1.pStorageDevice = null;
		}
	}

	public void recordSave()
	{
		if (Game1.pStorageDevice == null)
		{
			return;
		}
		IAsyncResult asyncResult = Game1.pStorageDevice.BeginOpenContainer("DepthAttack00", null, null);
		try
		{
			asyncResult.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = Game1.pStorageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			try
			{
				Stream stream = storageContainer.OpenFile("RecordData.dat", FileMode.Create);
				for (int i = 0; i < pscoreRecord.Length; i++)
				{
					byte[] array = new byte[8];
					byte[] array2 = new byte[5];
					char[] array3 = new char[5];
					array = BitConverter.GetBytes(pscoreRecord[i].lngScore);
					stream.Write(array, 0, 8);
					array3 = pscoreRecord[i].strName.ToCharArray();
					array2[0] = (byte)array3[0];
					array2[1] = (byte)array3[1];
					array2[2] = (byte)array3[2];
					array2[3] = (byte)array3[3];
					array2[4] = (byte)array3[4];
					stream.Write(array2, 0, 5);
				}
				stream.Close();
			}
			catch
			{
			}
			storageContainer.Dispose();
		}
		catch
		{
			Game1.pStorageDevice = null;
		}
	}

	protected override void LoadContent()
	{
		spritesBatch = new SpriteBatch(base.GraphicsDevice);
		font1 = base.Game.Content.Load<SpriteFont>("SpriteFont1");
		base.LoadContent();
	}

	public void pRecordMov(GameTime gameTime)
	{
		gamePadState = GamePad.GetState(playerIndex);
		if (intOneScan < 30)
		{
			intOneScan++;
			gamePadMaeState = gamePadState;
			base.Update(gameTime);
			return;
		}
		if ((gamePadState.Buttons.A == ButtonState.Pressed && gamePadMaeState.Buttons.A == ButtonState.Released) || (gamePadState.Buttons.Start == ButtonState.Pressed && gamePadMaeState.Buttons.Start == ButtonState.Released) || (gamePadState.Buttons.B == ButtonState.Pressed && gamePadMaeState.Buttons.B == ButtonState.Released))
		{
			intOneScan = 0;
			pflgRecordEnd = true;
		}
		gamePadMaeState = gamePadState;
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	public void pRecordDraw(SpriteBatch aspriteBatch)
	{
		aspriteBatch.DrawString(font1, "Rank", new Vector2(300f, 80f), Color.White);
		aspriteBatch.DrawString(font1, "SCORE", new Vector2(400f, 80f), Color.White);
		aspriteBatch.DrawString(font1, "NAME", new Vector2(800f, 80f), Color.White);
		for (int i = 0; i < pscoreRecord.Length; i++)
		{
			aspriteBatch.DrawString(font1, (i + 1).ToString(), new Vector2(300f, 130 + i * 50), Color.White);
			aspriteBatch.DrawString(font1, pscoreRecord[i].lngScore.ToString(), new Vector2(400f, 130 + i * 50), Color.White);
			aspriteBatch.DrawString(font1, pscoreRecord[i].strName.ToString(), new Vector2(800f, 130 + i * 50), Color.White);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
