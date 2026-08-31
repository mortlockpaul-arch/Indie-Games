namespace _6;

internal class _0002
{
	public const float LightIconSize = 0.01f;

	private float HCB = 10f;

	private float HC_0002 = 10f;

	private bool HC_0012;

	private float HCH = 1f;

	public float IconScale
	{
		get
		{
			return HCH;
		}
		set
		{
			HCH = value;
		}
	}

	public float MoveScale
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	public float RotationScale
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
		}
	}

	public bool UserHandledView
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}
}
