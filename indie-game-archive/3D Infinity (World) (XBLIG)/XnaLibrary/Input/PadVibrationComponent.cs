using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaLibrary.Input;

public class PadVibrationComponent : GameComponent
{
	private float[] volume;

	public float Reduction { get; set; }

	public float this[PlayerIndex index]
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return volume[index];
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			volume[index] = value;
		}
	}

	public PadVibrationComponent(Game game)
		: base(game)
	{
		volume = new float[4];
		Reduction = 0.2f;
	}

	protected override void Dispose(bool disposing)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		for (PlayerIndex val = (PlayerIndex)0; (int)val <= 3; val = (PlayerIndex)(val + 1))
		{
			GamePad.SetVibration(val, 0f, 0f);
		}
		((GameComponent)this).Dispose(disposing);
	}

	public override void Update(GameTime gameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		for (PlayerIndex val = (PlayerIndex)0; (int)val <= 3; val = (PlayerIndex)(val + 1))
		{
			float num = Math.Min(volume[val], 1f);
			GamePad.SetVibration(val, num, num);
		}
		for (int i = 0; i < volume.Length; i++)
		{
			volume[i] = Math.Max(volume[i] - Reduction, 0f);
		}
		((GameComponent)this).Update(gameTime);
	}
}
