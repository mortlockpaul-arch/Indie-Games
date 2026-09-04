namespace SpaceBlast.AI;

internal class AIPersonalityVeryHard : AIPersonality
{
	public AIPersonalityVeryHard()
	{
		VisualRange = 70000;
		ShieldRetreatThreshold = 30;
		StrengthRetreatThreshold = 30;
		Accuracy = 1f;
	}
}
