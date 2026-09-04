using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Conditions;

public interface ICndEval
{
	bool eval(CRun rhPtr, CObject hoPtr);
}
