using Maximinus;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class Obj : Drawing3D.ModelAlphaBones
{
	public enum IDenum
	{
		TablePlan,
		RepositionWBall,
		Cue,
		Floor,
		Ball0,
		Ball1,
		Ball2,
		Ball3,
		Ball4,
		Ball5,
		Ball6,
		Ball7,
		Ball8,
		Ball9,
		Ball10,
		Ball11,
		Ball12,
		Ball13,
		Ball14,
		Ball15
	}

	public readonly IDenum id;

	public Obj(IDenum id, Model model)
		: base(model)
	{
		this.id = id;
	}

	public static bool IsBall(IDenum i)
	{
		if (i >= IDenum.Ball0)
		{
			return i < (IDenum)32;
		}
		return false;
	}
}
