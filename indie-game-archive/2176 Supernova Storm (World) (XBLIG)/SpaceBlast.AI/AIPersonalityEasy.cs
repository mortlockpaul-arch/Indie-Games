namespace SpaceBlast.AI;

internal class AIPersonalityEasy : AIPersonality
{
	public AIPersonalityEasy()
	{
		VisualRange = 40000;
		ShieldRetreatThreshold = 70;
		StrengthRetreatThreshold = 70;
		Accuracy = 0.3f;
	}
}
