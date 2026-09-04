namespace SpaceBlast.AI;

internal class AIPersonalityVeryEasy : AIPersonality
{
	public AIPersonalityVeryEasy()
	{
		VisualRange = 30000;
		ShieldRetreatThreshold = 80;
		StrengthRetreatThreshold = 80;
		Accuracy = 0.1f;
	}
}
