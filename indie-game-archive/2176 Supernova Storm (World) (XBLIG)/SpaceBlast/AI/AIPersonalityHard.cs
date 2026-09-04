namespace SpaceBlast.AI;

internal class AIPersonalityHard : AIPersonality
{
	public AIPersonalityHard()
	{
		VisualRange = 60000;
		ShieldRetreatThreshold = 50;
		StrengthRetreatThreshold = 50;
		Accuracy = 0.8f;
	}
}
