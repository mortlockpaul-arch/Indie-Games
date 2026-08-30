using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class DigitTexture
{
	private Color color;

	private int nbDigits;

	private bool overflow;

	private float valueFloat;

	private Texture2D[] texFinals;

	private static Texture2D[] texDigits;

	public static Texture2D texDot;

	public static Texture2D texPlus;

	private float height;

	private bool isShort;

	private float timerStartSeconds;

	private float timerLengthSeconds;

	private bool DetailsIfLessThan10;

	private static bool initialized = false;

	private float pauseTimeStart;

	private float pauseTimeValue;

	public Vector2 TotalSize => new Vector2((float)nbDigits * height, height);

	public float Height => height;

	public Texture2D[] Textures => texFinals;

	public Color Color => color;

	public bool DrawDot => isShort;

	public float DotOffset_X => (float)(nbDigits - 2) * height;

	public Vector2 DotSize => new Vector2(2f * height, height);

	public bool IsTimerStarted => timerStartSeconds != -1f;

	private int MaxValue => (int)Math.Pow(10.0, nbDigits) - 1;

	public bool Paused => pauseTimeStart != 0f;

	public static void Initialize(Texture2D[] texDigitsValue, Texture2D texDotValue, Texture2D texPlusValue)
	{
		texDigits = texDigitsValue;
		texDot = texDotValue;
		texPlus = texPlusValue;
		initialized = true;
	}

	public DigitTexture(Color color, int nbDigits, float height)
		: this(color, nbDigits, height, DetailsIfLessThan10: true)
	{
	}

	public DigitTexture(Color color, int nbDigits, float height, bool DetailsIfLessThan10)
	{
		if (!initialized)
		{
			throw new Exception("not initialized");
		}
		if (nbDigits < 1)
		{
			throw new Exception("not enough digits : " + nbDigits);
		}
		this.DetailsIfLessThan10 = DetailsIfLessThan10;
		this.color = color;
		this.nbDigits = nbDigits;
		this.height = height;
		valueFloat = -1f;
		texFinals = new Texture2D[nbDigits];
		timerStartSeconds = -1f;
		Update(0f);
	}

	public void Update(float valueFloatNoCheck)
	{
		float num = Math.Min(valueFloatNoCheck, (float)Math.Pow(10.0, nbDigits) - 1f);
		int num2 = -1;
		isShort = DetailsIfLessThan10 && num < 10f;
		if (!isShort)
		{
			num2 = (int)num;
			if (num2 == (int)valueFloat)
			{
				return;
			}
		}
		else if ((int)(valueFloat * 10f) == (int)(num * 10f))
		{
			return;
		}
		valueFloat = num;
		overflow = num > (float)(MaxValue + 1);
		int[] array = new int[nbDigits];
		if (overflow)
		{
			return;
		}
		if (!isShort)
		{
			int num3 = 0;
			int num4 = num2;
			for (int num5 = nbDigits - 1; num5 >= 0; num5--)
			{
				int num6 = (int)Math.Pow(10.0, num5);
				int num7 = num4 / num6;
				array[num3++] = num7;
				num4 -= num7 * num6;
			}
		}
		else
		{
			array[0] = (int)num;
			array[1] = (int)((num - (float)array[0]) * 10f);
		}
		for (int i = 0; i < nbDigits; i++)
		{
			texFinals[i] = texDigits[array[i]];
		}
	}

	public bool IsTimerFinished(GameTime gameTime)
	{
		return timerLengthSeconds + pauseTimeValue - ((float)gameTime.TotalGameTime.TotalSeconds - timerStartSeconds) <= 0f;
	}

	public void InitTimer(GameTime gameTime, float lengthSeconds)
	{
		timerStartSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
		timerLengthSeconds = lengthSeconds;
		pauseTimeStart = 0f;
		pauseTimeValue = 0f;
	}

	public void UpdateTimer(GameTime gameTime)
	{
		Update(MathHelper.Max(0f, timerLengthSeconds + pauseTimeValue - ((float)gameTime.TotalGameTime.TotalSeconds - timerStartSeconds)));
	}

	public void StopTimer()
	{
		timerStartSeconds = -1f;
	}

	public void PauseTimer(GameTime pauseTime)
	{
		pauseTimeStart = (float)pauseTime.TotalGameTime.TotalSeconds;
	}

	public void UnpauseTimer(GameTime unpauseTime)
	{
		pauseTimeValue += (float)unpauseTime.TotalGameTime.TotalSeconds - pauseTimeStart;
		pauseTimeStart = 0f;
	}

	public double RemainingSeconds(GameTime gameTime)
	{
		return timerLengthSeconds + pauseTimeValue - ((float)gameTime.TotalGameTime.TotalSeconds - timerStartSeconds);
	}
}
