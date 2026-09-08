using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal abstract class ExprVisitorBase
{
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected Expr Visit(Expr pExpr)
	{
		if (pExpr != null)
		{
			return Dispatch(pExpr);
		}
		return null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr Dispatch(Expr pExpr)
	{
		return pExpr.Kind switch
		{
			ExpressionKind.BinaryOp => VisitBINOP(pExpr as ExprBinOp), 
			ExpressionKind.UnaryOp => VisitUNARYOP(pExpr as ExprUnaryOp), 
			ExpressionKind.Assignment => VisitASSIGNMENT(pExpr as ExprAssignment), 
			ExpressionKind.List => VisitLIST(pExpr as ExprList), 
			ExpressionKind.ArrayIndex => VisitARRAYINDEX(pExpr as ExprArrayIndex), 
			ExpressionKind.Call => VisitCALL(pExpr as ExprCall), 
			ExpressionKind.Field => VisitFIELD(pExpr as ExprField), 
			ExpressionKind.Local => VisitLOCAL(pExpr as ExprLocal), 
			ExpressionKind.Constant => VisitCONSTANT(pExpr as ExprConstant), 
			ExpressionKind.Class => pExpr, 
			ExpressionKind.Property => VisitPROP(pExpr as ExprProperty), 
			ExpressionKind.Multi => VisitMULTI(pExpr as ExprMulti), 
			ExpressionKind.MultiGet => VisitMULTIGET(pExpr as ExprMultiGet), 
			ExpressionKind.Wrap => VisitWRAP(pExpr as ExprWrap), 
			ExpressionKind.Concat => VisitCONCAT(pExpr as ExprConcat), 
			ExpressionKind.ArrayInit => VisitARRINIT(pExpr as ExprArrayInit), 
			ExpressionKind.Cast => VisitCAST(pExpr as ExprCast), 
			ExpressionKind.UserDefinedConversion => VisitUSERDEFINEDCONVERSION(pExpr as ExprUserDefinedConversion), 
			ExpressionKind.TypeOf => VisitTYPEOF(pExpr as ExprTypeOf), 
			ExpressionKind.ZeroInit => VisitZEROINIT(pExpr as ExprZeroInit), 
			ExpressionKind.UserLogicalOp => VisitUSERLOGOP(pExpr as ExprUserLogicalOp), 
			ExpressionKind.MemberGroup => VisitMEMGRP(pExpr as ExprMemberGroup), 
			ExpressionKind.FieldInfo => VisitFIELDINFO(pExpr as ExprFieldInfo), 
			ExpressionKind.MethodInfo => VisitMETHODINFO(pExpr as ExprMethodInfo), 
			ExpressionKind.EqualsParam => VisitEQUALS(pExpr as ExprBinOp), 
			ExpressionKind.Compare => VisitCOMPARE(pExpr as ExprBinOp), 
			ExpressionKind.NotEq => VisitNE(pExpr as ExprBinOp), 
			ExpressionKind.LessThan => VisitLT(pExpr as ExprBinOp), 
			ExpressionKind.LessThanOrEqual => VisitLE(pExpr as ExprBinOp), 
			ExpressionKind.GreaterThan => VisitGT(pExpr as ExprBinOp), 
			ExpressionKind.GreaterThanOrEqual => VisitGE(pExpr as ExprBinOp), 
			ExpressionKind.Add => VisitADD(pExpr as ExprBinOp), 
			ExpressionKind.Subtract => VisitSUB(pExpr as ExprBinOp), 
			ExpressionKind.Multiply => VisitMUL(pExpr as ExprBinOp), 
			ExpressionKind.Divide => VisitDIV(pExpr as ExprBinOp), 
			ExpressionKind.Modulo => VisitMOD(pExpr as ExprBinOp), 
			ExpressionKind.BitwiseAnd => VisitBITAND(pExpr as ExprBinOp), 
			ExpressionKind.BitwiseOr => VisitBITOR(pExpr as ExprBinOp), 
			ExpressionKind.BitwiseExclusiveOr => VisitBITXOR(pExpr as ExprBinOp), 
			ExpressionKind.LeftShirt => VisitLSHIFT(pExpr as ExprBinOp), 
			ExpressionKind.RightShift => VisitRSHIFT(pExpr as ExprBinOp), 
			ExpressionKind.LogicalAnd => VisitLOGAND(pExpr as ExprBinOp), 
			ExpressionKind.LogicalOr => VisitLOGOR(pExpr as ExprBinOp), 
			ExpressionKind.Sequence => VisitSEQUENCE(pExpr as ExprBinOp), 
			ExpressionKind.Save => VisitSAVE(pExpr as ExprBinOp), 
			ExpressionKind.Swap => VisitSWAP(pExpr as ExprBinOp), 
			ExpressionKind.Indir => VisitINDIR(pExpr as ExprBinOp), 
			ExpressionKind.StringEq => VisitSTRINGEQ(pExpr as ExprBinOp), 
			ExpressionKind.StringNotEq => VisitSTRINGNE(pExpr as ExprBinOp), 
			ExpressionKind.DelegateEq => VisitDELEGATEEQ(pExpr as ExprBinOp), 
			ExpressionKind.DelegateNotEq => VisitDELEGATENE(pExpr as ExprBinOp), 
			ExpressionKind.DelegateAdd => VisitDELEGATEADD(pExpr as ExprBinOp), 
			ExpressionKind.DelegateSubtract => VisitDELEGATESUB(pExpr as ExprBinOp), 
			ExpressionKind.Eq => VisitEQ(pExpr as ExprBinOp), 
			ExpressionKind.True => VisitTRUE(pExpr as ExprUnaryOp), 
			ExpressionKind.False => VisitFALSE(pExpr as ExprUnaryOp), 
			ExpressionKind.Inc => VisitINC(pExpr as ExprUnaryOp), 
			ExpressionKind.Dec => VisitDEC(pExpr as ExprUnaryOp), 
			ExpressionKind.LogicalNot => VisitLOGNOT(pExpr as ExprUnaryOp), 
			ExpressionKind.Negate => VisitNEG(pExpr as ExprUnaryOp), 
			ExpressionKind.UnaryPlus => VisitUPLUS(pExpr as ExprUnaryOp), 
			ExpressionKind.BitwiseNot => VisitBITNOT(pExpr as ExprUnaryOp), 
			ExpressionKind.Addr => VisitADDR(pExpr as ExprUnaryOp), 
			ExpressionKind.DecimalNegate => VisitDECIMALNEG(pExpr as ExprUnaryOp), 
			ExpressionKind.DecimalInc => VisitDECIMALINC(pExpr as ExprUnaryOp), 
			ExpressionKind.DecimalDec => VisitDECIMALDEC(pExpr as ExprUnaryOp), 
			_ => throw Error.InternalCompilerError(), 
		};
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private void VisitChildren(Expr pExpr)
	{
		switch (pExpr.Kind)
		{
		case ExpressionKind.List:
		{
			ExprList exprList = (ExprList)pExpr;
			Expr optionalNextListNode;
			while (true)
			{
				exprList.OptionalElement = Visit(exprList.OptionalElement);
				optionalNextListNode = exprList.OptionalNextListNode;
				if (optionalNextListNode == null)
				{
					return;
				}
				if (!(optionalNextListNode is ExprList exprList2))
				{
					break;
				}
				exprList = exprList2;
			}
			exprList.OptionalNextListNode = Visit(optionalNextListNode);
			break;
		}
		case ExpressionKind.Assignment:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprAssignment).LHS);
			(pExpr as ExprAssignment).LHS = optionalLeftChild;
			optionalLeftChild = Visit((pExpr as ExprAssignment).RHS);
			(pExpr as ExprAssignment).RHS = optionalLeftChild;
			break;
		}
		case ExpressionKind.ArrayIndex:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprArrayIndex).Array);
			(pExpr as ExprArrayIndex).Array = optionalLeftChild;
			optionalLeftChild = Visit((pExpr as ExprArrayIndex).Index);
			(pExpr as ExprArrayIndex).Index = optionalLeftChild;
			break;
		}
		case ExpressionKind.UnaryOp:
		case ExpressionKind.True:
		case ExpressionKind.False:
		case ExpressionKind.Inc:
		case ExpressionKind.Dec:
		case ExpressionKind.LogicalNot:
		case ExpressionKind.Negate:
		case ExpressionKind.UnaryPlus:
		case ExpressionKind.BitwiseNot:
		case ExpressionKind.Addr:
		case ExpressionKind.DecimalNegate:
		case ExpressionKind.DecimalInc:
		case ExpressionKind.DecimalDec:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprUnaryOp).Child);
			(pExpr as ExprUnaryOp).Child = optionalLeftChild;
			break;
		}
		case ExpressionKind.UserLogicalOp:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprUserLogicalOp).TrueFalseCall);
			(pExpr as ExprUserLogicalOp).TrueFalseCall = optionalLeftChild;
			optionalLeftChild = Visit((pExpr as ExprUserLogicalOp).OperatorCall);
			(pExpr as ExprUserLogicalOp).OperatorCall = optionalLeftChild as ExprCall;
			optionalLeftChild = Visit((pExpr as ExprUserLogicalOp).FirstOperandToExamine);
			(pExpr as ExprUserLogicalOp).FirstOperandToExamine = optionalLeftChild;
			break;
		}
		case ExpressionKind.Cast:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprCast).Argument);
			(pExpr as ExprCast).Argument = optionalLeftChild;
			break;
		}
		case ExpressionKind.UserDefinedConversion:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprUserDefinedConversion).UserDefinedCall);
			(pExpr as ExprUserDefinedConversion).UserDefinedCall = optionalLeftChild;
			break;
		}
		case ExpressionKind.MemberGroup:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprMemberGroup).OptionalObject);
			(pExpr as ExprMemberGroup).OptionalObject = optionalLeftChild;
			break;
		}
		case ExpressionKind.Call:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprCall).OptionalArguments);
			(pExpr as ExprCall).OptionalArguments = optionalLeftChild;
			optionalLeftChild = Visit((pExpr as ExprCall).MemberGroup);
			(pExpr as ExprCall).MemberGroup = optionalLeftChild as ExprMemberGroup;
			break;
		}
		case ExpressionKind.Property:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprProperty).OptionalArguments);
			(pExpr as ExprProperty).OptionalArguments = optionalLeftChild;
			optionalLeftChild = Visit((pExpr as ExprProperty).MemberGroup);
			(pExpr as ExprProperty).MemberGroup = optionalLeftChild as ExprMemberGroup;
			break;
		}
		case ExpressionKind.Field:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprField).OptionalObject);
			(pExpr as ExprField).OptionalObject = optionalLeftChild;
			break;
		}
		case ExpressionKind.Constant:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprConstant).OptionalConstructorCall);
			(pExpr as ExprConstant).OptionalConstructorCall = optionalLeftChild;
			break;
		}
		case ExpressionKind.Multi:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprMulti).Left);
			(pExpr as ExprMulti).Left = optionalLeftChild;
			optionalLeftChild = Visit((pExpr as ExprMulti).Operator);
			(pExpr as ExprMulti).Operator = optionalLeftChild;
			break;
		}
		case ExpressionKind.Concat:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprConcat).FirstArgument);
			(pExpr as ExprConcat).FirstArgument = optionalLeftChild;
			optionalLeftChild = Visit((pExpr as ExprConcat).SecondArgument);
			(pExpr as ExprConcat).SecondArgument = optionalLeftChild;
			break;
		}
		case ExpressionKind.ArrayInit:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprArrayInit).OptionalArguments);
			(pExpr as ExprArrayInit).OptionalArguments = optionalLeftChild;
			optionalLeftChild = Visit((pExpr as ExprArrayInit).OptionalArgumentDimensions);
			(pExpr as ExprArrayInit).OptionalArgumentDimensions = optionalLeftChild;
			break;
		}
		default:
		{
			Expr optionalLeftChild = Visit((pExpr as ExprBinOp).OptionalLeftChild);
			(pExpr as ExprBinOp).OptionalLeftChild = optionalLeftChild;
			optionalLeftChild = Visit((pExpr as ExprBinOp).OptionalRightChild);
			(pExpr as ExprBinOp).OptionalRightChild = optionalLeftChild;
			break;
		}
		case ExpressionKind.NoOp:
		case ExpressionKind.Local:
		case ExpressionKind.Class:
		case ExpressionKind.MultiGet:
		case ExpressionKind.Wrap:
		case ExpressionKind.TypeOf:
		case ExpressionKind.ZeroInit:
		case ExpressionKind.FieldInfo:
		case ExpressionKind.MethodInfo:
			break;
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitEXPR(Expr pExpr)
	{
		VisitChildren(pExpr);
		return pExpr;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitBINOP(ExprBinOp pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitLIST(ExprList pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitASSIGNMENT(ExprAssignment pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitARRAYINDEX(ExprArrayIndex pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitUNARYOP(ExprUnaryOp pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitUSERLOGOP(ExprUserLogicalOp pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitTYPEOF(ExprTypeOf pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitCAST(ExprCast pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitUSERDEFINEDCONVERSION(ExprUserDefinedConversion pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitZEROINIT(ExprZeroInit pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitMEMGRP(ExprMemberGroup pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitCALL(ExprCall pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitPROP(ExprProperty pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitFIELD(ExprField pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitLOCAL(ExprLocal pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitCONSTANT(ExprConstant pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitMULTIGET(ExprMultiGet pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitMULTI(ExprMulti pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitWRAP(ExprWrap pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitCONCAT(ExprConcat pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitARRINIT(ExprArrayInit pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitFIELDINFO(ExprFieldInfo pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitMETHODINFO(ExprMethodInfo pExpr)
	{
		return VisitEXPR(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitEQUALS(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitCOMPARE(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitEQ(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitNE(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitLE(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitGE(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitADD(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitSUB(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitDIV(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitBITAND(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitBITOR(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitLSHIFT(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitLOGAND(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitSEQUENCE(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitSAVE(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitINDIR(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitSTRINGEQ(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitDELEGATEEQ(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitDELEGATEADD(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitLT(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitMUL(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitBITXOR(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitRSHIFT(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitLOGOR(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitSTRINGNE(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitDELEGATENE(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitGT(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitMOD(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitSWAP(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitDELEGATESUB(ExprBinOp pExpr)
	{
		return VisitBINOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitTRUE(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitINC(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitLOGNOT(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitNEG(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitBITNOT(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitADDR(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitDECIMALNEG(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitDECIMALDEC(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitFALSE(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitDEC(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitUPLUS(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected virtual Expr VisitDECIMALINC(ExprUnaryOp pExpr)
	{
		return VisitUNARYOP(pExpr);
	}
}
