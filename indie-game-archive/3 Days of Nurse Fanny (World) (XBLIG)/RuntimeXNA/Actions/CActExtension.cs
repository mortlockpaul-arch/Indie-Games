using Microsoft.Xna.Framework.Input;
using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

public class CActExtension : CAct
{
	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject != null)
		{
			int num = ((evtCode >> 16) & 0xFFFF) - 80;
			CExtension cExtension = (CExtension)cObject;
			cExtension.action(num, this);
		}
	}

	public virtual CObject getParamObject(CRun rhPtr, int num)
	{
		return rhPtr.rhEvtProg.get_ParamActionObjects(((PARAM_OBJECT)evtParams[num]).oiList, this);
	}

	public virtual int getParamTime(CRun rhPtr, int num)
	{
		if (evtParams[num].code == 2)
		{
			return ((PARAM_TIME)evtParams[num]).timer;
		}
		return rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
	}

	public virtual short getParamBorder(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
	}

	public virtual short getParamShort(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
	}

	public virtual short getParamAltValue(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
	}

	public virtual short getParamDirection(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
	}

	public virtual PARAM_CREATE getParamCreate(CRun rhPtr, int num)
	{
		return (PARAM_CREATE)evtParams[num];
	}

	public virtual int getParamAnimation(CRun rhPtr, int num)
	{
		if (evtParams[num].code == 10)
		{
			return ((PARAM_SHORT)evtParams[num]).value;
		}
		return rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
	}

	public virtual short getParamPlayer(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
	}

	public virtual PARAM_EVERY getParamEvery(CRun rhPtr, int num)
	{
		return (PARAM_EVERY)evtParams[num];
	}

	public virtual Keys getParamKey(CRun rhPtr, int num)
	{
		return ((PARAM_KEY)evtParams[num]).key;
	}

	public virtual int getParamSpeed(CRun rhPtr, int num)
	{
		return rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
	}

	public virtual CPositionInfo getParamPosition(CRun rhPtr, int num)
	{
		CPosition cPosition = (CPosition)evtParams[num];
		CPositionInfo cPositionInfo = new CPositionInfo();
		cPosition.read_Position(rhPtr, 0, cPositionInfo);
		return cPositionInfo;
	}

	public virtual short getParamJoyDirection(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
	}

	public virtual PARAM_SHOOT getParamShoot(CRun rhPtr, int num)
	{
		return (PARAM_SHOOT)evtParams[num];
	}

	public virtual PARAM_ZONE getParamZone(CRun rhPtr, int num)
	{
		return (PARAM_ZONE)evtParams[num];
	}

	public virtual int getParamExpression(CRun rhPtr, int num)
	{
		return rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
	}

	public virtual int getParamColour(CRun rhPtr, int num)
	{
		if (evtParams[num].code == 24)
		{
			return ((PARAM_COLOUR)evtParams[num]).color;
		}
		int rgb = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
		return CServices.swapRGB(rgb);
	}

	public virtual short getParamFrame(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
	}

	public virtual int getParamNewDirection(CRun rhPtr, int num)
	{
		if (evtParams[num].code == 29)
		{
			return ((PARAM_SHORT)evtParams[num]).value;
		}
		return rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]);
	}

	public virtual short getParamClick(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
	}

	public virtual PARAM_PROGRAM getParamProgram(CRun rhPtr, int num)
	{
		return (PARAM_PROGRAM)evtParams[num];
	}

	public virtual string getParamFilename(CRun rhPtr, int num)
	{
		if (evtParams[num].code == 40)
		{
			return ((PARAM_STRING)evtParams[num]).pString;
		}
		return rhPtr.get_EventExpressionString((CParamExpression)evtParams[num]);
	}

	public virtual string getParamExpString(CRun rhPtr, int num)
	{
		return rhPtr.get_EventExpressionString((CParamExpression)evtParams[num]);
	}

	public virtual double getParamExpDouble(CRun rhPtr, int num)
	{
		CValue cValue = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[num]);
		return cValue.getDouble();
	}

	public virtual string getParamFilename2(CRun rhPtr, int num)
	{
		if (evtParams[num].code == 63)
		{
			return ((PARAM_STRING)evtParams[num]).pString;
		}
		return rhPtr.get_EventExpressionString((CParamExpression)evtParams[num]);
	}

	public virtual CFile getParamExtension(CRun rhPtr, int num)
	{
		return null;
	}
}
