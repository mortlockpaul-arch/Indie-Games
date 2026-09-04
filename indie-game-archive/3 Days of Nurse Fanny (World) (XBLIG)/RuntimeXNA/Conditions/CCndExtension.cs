using Microsoft.Xna.Framework.Input;
using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Conditions;

public class CCndExtension : CCnd
{
	public override bool eva1(CRun rhPtr, CObject pHo)
	{
		if (pHo == null)
		{
			return eva2(rhPtr);
		}
		CExtension cExtension = (CExtension)pHo;
		pHo.hoFlags |= 2;
		int num = -(short)((evtCode >> 16) & 0xFFFF) - 80 - 1;
		if (cExtension.condition(num, this))
		{
			rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
			return true;
		}
		return false;
	}

	public override bool eva2(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
		int num = rhPtr.rhEvtProg.evtNSelectedObjects;
		int num2 = -(short)((evtCode >> 16) & 0xFFFF) - 80 - 1;
		while (cObject != null)
		{
			CExtension cExtension = (CExtension)cObject;
			cObject.hoFlags &= -3;
			if (cExtension.condition(num2, this))
			{
				if ((evtFlags2 & 1) != 0)
				{
					num--;
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
				}
			}
			else if ((evtFlags2 & 1) == 0)
			{
				num--;
				rhPtr.rhEvtProg.evt_DeleteCurrentObject();
			}
			cObject = rhPtr.rhEvtProg.evt_NextObject();
		}
		if (num != 0)
		{
			return true;
		}
		return false;
	}

	public virtual PARAM_OBJECT getParamObject(CRun rhPtr, int num)
	{
		return (PARAM_OBJECT)evtParams[num];
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

	public virtual short getParamAltValue(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
	}

	public virtual short getParamDirection(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
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

	public virtual PARAM_POSITION getParamPosition(CRun rhPtr, int num)
	{
		return (PARAM_POSITION)evtParams[num];
	}

	public virtual short getParamJoyDirection(CRun rhPtr, int num)
	{
		return ((PARAM_SHORT)evtParams[num]).value;
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
		return CServices.swapRGB(rhPtr.get_EventExpressionInt((CParamExpression)evtParams[num]));
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

	public virtual string getParamFilename2(CRun rhPtr, int num)
	{
		if (evtParams[num].code == 63)
		{
			return ((PARAM_STRING)evtParams[num]).pString;
		}
		return rhPtr.get_EventExpressionString((CParamExpression)evtParams[num]);
	}

	public virtual bool compareValues(CRun rhPtr, int num, CValue value_Renamed)
	{
		CValue pValue = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[num]);
		short comparaison = ((CParamExpression)evtParams[num]).comparaison;
		return CRun.compareTo(value_Renamed, pValue, comparaison);
	}

	public virtual bool compareTime(CRun rhPtr, int num, int t)
	{
		PARAM_CMPTIME pARAM_CMPTIME = (PARAM_CMPTIME)evtParams[num];
		CValue pValue = new CValue(pARAM_CMPTIME.timer);
		short comparaison = pARAM_CMPTIME.comparaison;
		CValue pValue2 = new CValue(t);
		return CRun.compareTo(pValue2, pValue, comparaison);
	}
}
