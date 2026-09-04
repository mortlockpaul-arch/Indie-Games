using System;
using RuntimeXNA.Application;
using RuntimeXNA.Events;
using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Conditions;

public abstract class CCnd : CEvent
{
	public short evtIdentifier;

	public CCnd()
	{
	}

	public static CCnd create(CRunApp app)
	{
		int filePointer = app.file.getFilePointer();
		short num = app.file.readAShort();
		CCnd cCnd = null;
		int num2 = app.file.readAInt();
		cCnd = num2 switch
		{
			-1638401 => new CND_CHANCE(), 
			-1572865 => new CND_ORLOGICAL(), 
			-1507329 => new CND_OR(), 
			-1441793 => new CND_GROUPSTART(), 
			-1310721 => new CND_ONCLOSE(), 
			-1245185 => new CND_COMPAREGSTRING(), 
			-983041 => new CND_ONLOOP(), 
			-720897 => new CND_GROUPACTIVATED(), 
			-655361 => new CND_ENDGROUP(), 
			-589825 => new CND_GROUP(), 
			-524289 => new CND_REMARK(), 
			-458753 => new CND_COMPAREG(), 
			-393217 => new CND_NOTALWAYS(), 
			-327681 => new CND_ONCE(), 
			-262145 => new CND_REPEAT(), 
			-196609 => new CND_NOMORE(), 
			-131073 => new CND_COMPARE(), 
			-65537 => new CND_NEVER(), 
			-1 => new CND_ALWAYS(), 
			-524290 => new CND_SPCHANNELPAUSED(), 
			-458754 => new CND_NOSPCHANNELPLAYING(), 
			-327682 => new CND_SPSAMPAUSED(), 
			-131074 => new CND_NOSAMPLAYING(), 
			-2 => new CND_NOSPSAMPLAYING(), 
			-458755 => new CND_ENDOFPAUSE(), 
			-327683 => new CND_ISLADDER(), 
			-262147 => new CND_ISOBSTACLE(), 
			-196611 => new CND_QUITAPPLICATION(), 
			-131075 => new CND_LEVEL(), 
			-65539 => new CND_END(), 
			-3 => new CND_START(), 
			-458756 => new CND_EVERY2(), 
			-393220 => new CND_TIMER2(), 
			-262148 => new CND_TIMEOUT(), 
			-196612 => new CND_EVERY(), 
			-131076 => new CND_TIMER(), 
			-65540 => new CND_TIMERINF(), 
			-4 => new CND_TIMERSUP(), 
			-720902 => new CND_ONMOUSEWHEELDOWN(), 
			-655366 => new CND_ONMOUSEWHEELUP(), 
			-589830 => new CND_MOUSEON(), 
			-524294 => new CND_ANYKEY(), 
			-458758 => new CND_MKEYDEPRESSED(), 
			-393222 => new CND_MCLICKONOBJECT(), 
			-327686 => new CND_MCLICKINZONE(), 
			-262150 => new CND_MCLICK(), 
			-196614 => new CND_MONOBJECT(), 
			-131078 => new CND_MINZONE(), 
			-65542 => new CND_KBKEYDEPRESSED(), 
			-6 => new CND_KBPRESSKEY(), 
			-327687 => new CND_JOYPUSHED(), 
			-262151 => new CND_NOMORELIVE(), 
			-196615 => new CND_JOYPRESSED(), 
			-131079 => new CND_LIVE(), 
			-65543 => new CND_SCORE(), 
			-7 => new CND_PLAYERPLAYING(), 
			-1441797 => new CND_CHOOSEALLINLINE(), 
			-1376261 => new CND_CHOOSEFLAGRESET(), 
			-1310725 => new CND_CHOOSEFLAGSET(), 
			-1245189 => new CND_CHOOSEVALUE(), 
			-1179653 => new CND_PICKFROMID(), 
			-1114117 => new CND_CHOOSEALLINZONE(), 
			-1048581 => new CND_CHOOSEALL(), 
			-983045 => new CND_CHOOSEZONE(), 
			-917509 => new CND_NUMOFALLOBJECT(), 
			-851973 => new CND_NUMOFALLZONE(), 
			-786437 => new CND_NOMOREALLZONE(), 
			-720901 => new CND_CHOOSEFLAGRESET_OLD(), 
			-655365 => new CND_CHOOSEFLAGSET_OLD(), 
			-458757 => new CND_CHOOSEVALUE_OLD(), 
			-393221 => new CND_PICKFROMID_OLD(), 
			-327685 => new CND_CHOOSEALLINZONE_OLD(), 
			-262149 => new CND_CHOOSEALL_OLD(), 
			-196613 => new CND_CHOOSEZONE_OLD(), 
			-131077 => new CND_NUMOFALLOBJECT_OLD(), 
			-65541 => new CND_NUMOFALLZONE_OLD(), 
			-5 => new CND_NOMOREALLZONE_OLD(), 
			-5308414 => new CND_SPRCLICK(), 
			-5308409 => new CND_CCOUNTER(), 
			-5439484 => new CND_QEQUAL(), 
			-5373948 => new CND_QFALSE(), 
			-5308412 => new CND_QEXACT(), 
			-5505015 => new CND_CCAISPAUSED(), 
			-5439479 => new CND_CCAISVISIBLE(), 
			-5373943 => new CND_CCAAPPFINISHED(), 
			-5308407 => new CND_CCAFRAMECHANGED(), 
			_ => (num2 & -65536) switch
			{
				-2490368 => new CND_EXTISITALIC(), 
				-2424832 => new CND_EXTISBOLD(), 
				-2359296 => new CND_EXTCMPVARSTRING(), 
				-2293760 => new CND_EXTPATHNODENAME(), 
				-2228224 => new CND_EXTCHOOSE(), 
				-2162688 => new CND_EXTNOMOREOBJECT(), 
				-2097152 => new CND_EXTNUMOFOBJECT(), 
				-2031616 => new CND_EXTNOMOREZONE(), 
				-1966080 => new CND_EXTNUMBERZONE(), 
				-1900544 => new CND_EXTSHOWN(), 
				-1835008 => new CND_EXTHIDDEN(), 
				-1769472 => new CND_EXTCMPVAR(), 
				-1703936 => new CND_EXTCMPVARFIXED(), 
				-1638400 => new CND_EXTFLAGSET(), 
				-1572864 => new CND_EXTFLAGRESET(), 
				-1507328 => new CND_EXTISCOLBACK(), 
				-1441792 => new CND_EXTNEARBORDERS(), 
				-1376256 => new CND_EXTENDPATH(), 
				-1310720 => new CND_EXTPATHNODE(), 
				-1245184 => new CND_EXTCMPACC(), 
				-1179648 => new CND_EXTCMPDEC(), 
				-1114112 => new CND_EXTCMPX(), 
				-1048576 => new CND_EXTCMPY(), 
				-983040 => new CND_EXTCMPSPEED(), 
				-917504 => new CND_EXTCOLLISION(), 
				-851968 => new CND_EXTCOLBACK(), 
				-786432 => new CND_EXTOUTPLAYFIELD(), 
				-720896 => new CND_EXTINPLAYFIELD(), 
				-655360 => new CND_EXTISOUT(), 
				-589824 => new CND_EXTISIN(), 
				-524288 => new CND_EXTFACING(), 
				-458752 => new CND_EXTSTOPPED(), 
				-393216 => new CND_EXTBOUNCING(), 
				-327680 => new CND_EXTREVERSED(), 
				-262144 => new CND_EXTISCOLLIDING(), 
				-196608 => new CND_EXTANIMPLAYING(), 
				-131072 => new CND_EXTANIMENDOF(), 
				-65536 => new CND_EXTCMPFRAME(), 
				_ => new CCndExtension(), 
			}, 
		};
		if (cCnd != null)
		{
			cCnd.evtCode = num2;
			cCnd.evtOi = app.file.readAShort();
			cCnd.evtOiList = app.file.readAShort();
			cCnd.evtFlags = app.file.readByte();
			cCnd.evtFlags2 = app.file.readByte();
			cCnd.evtNParams = app.file.readByte();
			cCnd.evtDefType = app.file.readByte();
			cCnd.evtIdentifier = app.file.readAShort();
			if (cCnd.evtNParams > 0)
			{
				cCnd.evtParams = new CParam[cCnd.evtNParams];
				for (int i = 0; i < cCnd.evtNParams; i++)
				{
					cCnd.evtParams[i] = CParam.create(app);
				}
			}
		}
		else
		{
			Console.Out.WriteLine("*** Missing condition!");
		}
		app.file.seek(filePointer + num);
		return cCnd;
	}

	public virtual bool negaTRUE()
	{
		if ((evtFlags2 & 1) != 0)
		{
			return false;
		}
		return true;
	}

	public virtual bool negaFALSE()
	{
		if ((evtFlags2 & 1) != 0)
		{
			return true;
		}
		return false;
	}

	public virtual bool compute_GlobalNoRepeat(CRun rhPtr)
	{
		CEventGroup rhEventGroup = rhPtr.rhEvtProg.rhEventGroup;
		int evgInhibit = rhEventGroup.evgInhibit;
		rhEventGroup.evgInhibit = (ushort)rhPtr.rhLoopCount;
		int rhLoopCount = rhPtr.rhLoopCount;
		if (rhLoopCount == evgInhibit)
		{
			return false;
		}
		rhLoopCount--;
		if (rhLoopCount == evgInhibit)
		{
			return false;
		}
		return true;
	}

	public bool compute_NoRepeatCol(int identifier, CObject pHo)
	{
		CArrayList cArrayList = pHo.hoBaseNoRepeat;
		int num;
		if (cArrayList == null)
		{
			cArrayList = (pHo.hoBaseNoRepeat = new CArrayList());
		}
		else
		{
			for (int i = 0; i < cArrayList.size(); i++)
			{
				num = (int)cArrayList.get(i);
				if (num == identifier)
				{
					return false;
				}
			}
		}
		num = identifier;
		cArrayList.add(num);
		cArrayList = pHo.hoPrevNoRepeat;
		if (cArrayList == null)
		{
			return true;
		}
		for (int i = 0; i < cArrayList.size(); i++)
		{
			num = (int)cArrayList.get(i);
			if (num == identifier)
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool compute_NoRepeat(CObject pHo)
	{
		return compute_NoRepeatCol(evtIdentifier, pHo);
	}

	public virtual bool evaChooseValueOld(CRun rhPtr, IChooseValue pRoutine)
	{
		int num = 0;
		for (CObject cObject = rhPtr.rhEvtProg.evt_FirstObjectFromType(2); cObject != null; cObject = rhPtr.rhEvtProg.evt_NextObjectFromType())
		{
			num++;
			int v = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			if (!pRoutine.evaluate(cObject, v))
			{
				num--;
				rhPtr.rhEvtProg.evt_DeleteCurrentObject();
			}
		}
		if (num != 0)
		{
			return true;
		}
		return false;
	}

	public virtual bool evaChooseValue(CRun rhPtr, IChooseValue pRoutine)
	{
		int num = 0;
		for (CObject cObject = rhPtr.rhEvtProg.evt_FirstObjectFromType(-1); cObject != null; cObject = rhPtr.rhEvtProg.evt_NextObjectFromType())
		{
			num++;
			int v = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			if (!pRoutine.evaluate(cObject, v))
			{
				num--;
				rhPtr.rhEvtProg.evt_DeleteCurrentObject();
			}
		}
		if (num != 0)
		{
			return true;
		}
		return false;
	}

	public virtual bool evaExpObject(CRun rhPtr, IEvaExpObject pRoutine)
	{
		CObject cObject = rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
		int num = rhPtr.rhEvtProg.evtNSelectedObjects;
		CParamExpression cParamExpression = (CParamExpression)evtParams[0];
		while (cObject != null)
		{
			int v = rhPtr.get_EventExpressionInt(cParamExpression);
			if (!pRoutine.evaExpRoutine(cObject, v, cParamExpression.comparaison))
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

	public virtual bool evaObject(CRun rhPtr, IEvaObject pRoutine)
	{
		CObject cObject = rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
		int num = rhPtr.rhEvtProg.evtNSelectedObjects;
		while (cObject != null)
		{
			if (!pRoutine.evaObjectRoutine(cObject))
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

	public virtual bool compareCondition(CRun rhPtr, int param, int v)
	{
		CValue pValue = rhPtr.get_EventExpressionAny((CParamExpression)evtParams[param]);
		short comparaison = ((CParamExpression)evtParams[param]).comparaison;
		CValue pValue2 = new CValue(v);
		return CRun.compareTo(pValue2, pValue, comparaison);
	}

	public virtual bool checkMark(CRun rhPtr, int mark)
	{
		if (mark == 0)
		{
			return false;
		}
		if (mark == rhPtr.rhLoopCount)
		{
			return true;
		}
		if (mark == rhPtr.rhLoopCount - 1)
		{
			return true;
		}
		return false;
	}

	public bool isColliding(CRun rhPtr)
	{
		if (rhPtr.rhEvtProg.rh4ConditionsFalse)
		{
			rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
			rhPtr.rhEvtProg.evt_FirstObject(((PARAM_OBJECT)evtParams[0]).oiList);
			return false;
		}
		bool flag = false;
		if ((evtFlags2 & 1) != 0)
		{
			flag = true;
		}
		CObject cObject = rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
		if (cObject == null)
		{
			return negaFALSE();
		}
		int num = rhPtr.rhEvtProg.evtNSelectedObjects;
		int num2 = num;
		short oi = ((PARAM_OBJECT)evtParams[0]).oi;
		short[] pOiColList;
		if (oi >= 0)
		{
			rhPtr.isColArray[0] = oi;
			rhPtr.isColArray[1] = ((PARAM_OBJECT)evtParams[0]).oiList;
			pOiColList = rhPtr.isColArray;
		}
		else
		{
			CQualToOiList cQualToOiList = rhPtr.rhEvtProg.qualToOiList[((PARAM_OBJECT)evtParams[0]).oiList & 0x7FFF];
			pOiColList = cQualToOiList.qoiList;
		}
		bool flag2 = false;
		CArrayList cArrayList = new CArrayList();
		do
		{
			CArrayList cArrayList2 = rhPtr.objectAllCol_IXY(cObject, cObject.roc.rcImage, cObject.roc.rcAngle, cObject.roc.rcScaleX, cObject.roc.rcScaleY, cObject.hoX, cObject.hoY, pOiColList);
			if (cArrayList2 == null)
			{
				if (!flag)
				{
					num--;
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
				}
			}
			else
			{
				flag2 = false;
				for (int i = 0; i < cArrayList2.size(); i++)
				{
					CObject cObject2 = (CObject)cArrayList2.get(i);
					if ((cObject2.hoFlags & 1) == 0)
					{
						cArrayList.add(cObject2);
						flag2 = true;
					}
				}
				if (flag)
				{
					if (flag2)
					{
						num--;
						rhPtr.rhEvtProg.evt_DeleteCurrentObject();
					}
				}
				else if (!flag2)
				{
					num--;
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
				}
			}
			cObject = rhPtr.rhEvtProg.evt_NextObject();
		}
		while (cObject != null);
		if (!flag)
		{
			if (num == 0)
			{
				return false;
			}
		}
		else if (num < num2)
		{
			return false;
		}
		cObject = rhPtr.rhEvtProg.evt_FirstObject(((PARAM_OBJECT)evtParams[0]).oiList);
		if (cObject == null)
		{
			return false;
		}
		num = rhPtr.rhEvtProg.evtNSelectedObjects;
		if (!flag)
		{
			do
			{
				int i;
				for (i = 0; i < cArrayList.size(); i++)
				{
					CObject cObject2 = (CObject)cArrayList.get(i);
					if (cObject == cObject2)
					{
						break;
					}
				}
				if (i == cArrayList.size())
				{
					num--;
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
				}
				cObject = rhPtr.rhEvtProg.evt_NextObject();
			}
			while (cObject != null);
			if (num != 0)
			{
				return true;
			}
			return false;
		}
		do
		{
			for (int i = 0; i < cArrayList.size(); i++)
			{
				CObject cObject2 = (CObject)cArrayList.get(i);
				if (cObject == cObject2)
				{
					num--;
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
					break;
				}
			}
			cObject = rhPtr.rhEvtProg.evt_NextObject();
		}
		while (cObject != null);
		if (num != 0)
		{
			return true;
		}
		return false;
	}

	public abstract bool eva1(CRun rhPtr, CObject hoPtr);

	public abstract bool eva2(CRun rhPtr);
}
