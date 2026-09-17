using System;
using GKEngine.Utils;

namespace GKEngine.Animation;

public class Tween
{
	public enum TweenMethod
	{
		Linear,
		EaseIn,
		EaseOut,
		EaseOutIn,
		EaseInOut,
		BounceOne
	}

	private const float PI = (float)Math.PI;

	private const float HALFPI = (float)Math.PI / 2f;

	private const float DOUBLEPI = (float)Math.PI * 2f;

	public TweenMethod tween;

	public static float Lerp(float xRatio, TweenMethod xMethod)
	{
		float result = xRatio;
		switch (xMethod)
		{
		case TweenMethod.EaseIn:
			result = EaseIn(xRatio);
			break;
		case TweenMethod.EaseOut:
			result = EaseOut(xRatio);
			break;
		case TweenMethod.EaseOutIn:
			result = EaseOutIn(xRatio);
			break;
		case TweenMethod.EaseInOut:
			result = EaseInOut(xRatio);
			break;
		}
		return result;
	}

	public static float EaseIn(float xRatio)
	{
		return MathUtils.SinLow(xRatio * ((float)Math.PI / 2f));
	}

	public static float EaseOut(float xRatio)
	{
		return 1f + MathUtils.CosLow((float)Math.PI + (float)Math.PI / 2f * xRatio);
	}

	public static float EaseOutIn(float xRatio)
	{
		if (xRatio > 0.5f)
		{
			xRatio = (xRatio - 0.5f) * 2f;
			return 0.5f + (MathUtils.SinLow(4.712389f + (float)Math.PI / 2f * xRatio) + 1f) * 0.5f;
		}
		xRatio *= 2f;
		return MathUtils.SinLow((float)Math.PI / 2f * xRatio) * 0.5f;
	}

	public static float EaseInOut(float xRatio)
	{
		return (MathUtils.CosLow((float)Math.PI + (float)Math.PI * xRatio) + 1f) * 0.5f;
	}

	public static float EaseInBounce(float xRatio, float xTimes)
	{
		return Math.Abs(MathUtils.SinLow(xRatio * (float)Math.PI * (xTimes - 0.5f))) * (1f - xRatio) + MathUtils.SinLow(xRatio * ((float)Math.PI / 2f));
	}

	public static float EaseIn_Circ(float time, float start, float change, float duration)
	{
		return (0f - change) * ((float)Math.Sqrt(1f - (time /= duration) * time) - 1f) + start;
	}

	public static float EaseOut_Circ(float time, float start, float change, float duration)
	{
		return change * (float)Math.Sqrt(1f - (time = time / duration - 1f) * time) + start;
	}

	public static float EaseInOut_Circ(float time, float start, float change, float duration)
	{
		if ((time /= duration / 2f) < 1f)
		{
			return (0f - change) / 2f * ((float)Math.Sqrt(1f - time * time) - 1f) + start;
		}
		return change / 2f * ((float)Math.Sqrt(1f - (time -= 2f) * time) + 1f) + start;
	}

	public static float EaseIn_Exp(float time, float start, float change, float duration)
	{
		if (time != 0f)
		{
			return change * (float)Math.Pow(2.0, 10f * (time / duration - 1f)) + start;
		}
		return start;
	}

	public static float EaseOut_Exp(float time, float start, float change, float duration)
	{
		if (time != duration)
		{
			return change * (0f - (float)Math.Pow(2.0, -10f * time / duration) + 1f) + start;
		}
		return start + change;
	}

	public static float EaseInOut_Exp(float time, float start, float change, float duration)
	{
		if (time == 0f)
		{
			return start;
		}
		if (time == duration)
		{
			return start + change;
		}
		if ((time /= duration / 2f) < 1f)
		{
			return change / 2f * (float)Math.Pow(2.0, 10f * (time - 1f)) + start;
		}
		return change / 2f * (0f - (float)Math.Pow(2.0, -10f * --time) + 2f) + start;
	}
}
