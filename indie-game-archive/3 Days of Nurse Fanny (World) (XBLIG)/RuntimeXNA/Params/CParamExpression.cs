using RuntimeXNA.Application;
using RuntimeXNA.Expressions;
using RuntimeXNA.Services;

namespace RuntimeXNA.Params;

public abstract class CParamExpression : CParam
{
	public CExp[] tokens;

	public short comparaison;

	public virtual void load(CFile file)
	{
		long num = file.getFilePointer();
		int num2 = 0;
		while (true)
		{
			num2++;
			if (file.readAInt() == 0)
			{
				break;
			}
			short num3 = file.readAShort();
			if (num3 > 6)
			{
				file.skipBytes(num3 - 6);
			}
		}
		file.seek((int)num);
		tokens = new CExp[num2];
		for (int i = 0; i < num2; i++)
		{
			tokens[i] = CExp.create(file);
		}
	}

	public abstract override void load(CRunApp app);
}
