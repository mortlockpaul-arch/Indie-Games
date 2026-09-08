using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.DataContracts;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Runtime.Serialization;

internal static class Globals
{
	internal const BindingFlags ScanAllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	[CompilerGenerated]
	private static XmlQualifiedName _003CIdQualifiedName_003Ek__BackingField;

	[CompilerGenerated]
	private static XmlQualifiedName _003CRefQualifiedName_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfObject_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfValueType_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfArray_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfString_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfInt_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfULong_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfVoid_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfByteArray_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfTimeSpan_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfGuid_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfDateTimeOffset_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfDateTimeOffsetAdapter_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfDateOnly_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfTimeOnly_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfMemoryStream_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfMemoryStreamAdapter_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfUri_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfTypeEnumerable_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfStreamingContext_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfISerializable_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIDeserializationCallback_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIObjectReference_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlFormatClassWriterDelegate_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlFormatCollectionWriterDelegate_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlFormatClassReaderDelegate_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlFormatCollectionReaderDelegate_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlFormatGetOnlyCollectionReaderDelegate_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfKnownTypeAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfDataContractAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfDataMemberAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfEnumMemberAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfCollectionDataContractAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfOptionalFieldAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfObjectArray_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfOnSerializingAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfOnSerializedAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfOnDeserializingAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfOnDeserializedAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfFlagsAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIXmlSerializable_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlSchemaProviderAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlRootAttribute_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlQualifiedName_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlSchemaType_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIExtensibleDataObject_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfExtensionDataObject_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfISerializableDataNode_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfClassDataNode_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfCollectionDataNode_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlDataNode_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfNullable_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfReflectionPointer_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIDictionaryGeneric_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIDictionary_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIListGeneric_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIList_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfICollectionGeneric_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfICollection_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIEnumerableGeneric_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIEnumerable_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIEnumeratorGeneric_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIEnumerator_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfKeyValuePair_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfKeyValue_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfIDictionaryEnumerator_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfDictionaryEnumerator_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfGenericDictionaryEnumerator_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfDictionaryGeneric_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfHashtable_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlElement_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfXmlNodeArray_003Ek__BackingField;

	[CompilerGenerated]
	private static Type _003CTypeOfDBNull_003Ek__BackingField;

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)]
	private static Type s_typeOfSchemaDefinedType;

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)]
	private static Type s_typeOfSchemaDefinedEnum;

	[CompilerGenerated]
	private static MemberInfo _003CSchemaMemberInfoPlaceholder_003Ek__BackingField;

	[CompilerGenerated]
	private static Uri _003CDataContractXsdBaseNamespaceUri_003Ek__BackingField;

	public const bool DefaultIsRequired = false;

	public const bool DefaultEmitDefaultValue = true;

	public const int DefaultOrder = 0;

	public const bool DefaultIsReference = false;

	public static readonly string NewObjectId = string.Empty;

	public const string NullObjectId = null;

	public const string FullSRSInternalsVisiblePattern = "^[\\s]*System\\.Runtime\\.Serialization[\\s]*,[\\s]*PublicKey[\\s]*=[\\s]*(?i:00240000048000009400000006020000002400005253413100040000010001008d56c76f9e8649383049f383c44be0ec204181822a6c31cf5eb7ef486944d032188ea1d3920763712ccb12d75fb77e9811149e6148e5d32fbaab37611c1878ddc19e20ef135d0cb2cff2bfec3d115810c3d9069638fe4be215dbf795861920e5ab6f7db2e2ceef136ac23d5dd2bf031700aec232f6c6b1c785b4305c123b37ab)[\\s]*$";

	public const char SpaceChar = ' ';

	public const char OpenBracketChar = '[';

	public const char CloseBracketChar = ']';

	public const char CommaChar = ',';

	public const string Space = " ";

	public const string XsiPrefix = "i";

	public const string XsdPrefix = "x";

	public const string SerPrefix = "z";

	public const string SerPrefixForSchema = "ser";

	public const string ElementPrefix = "q";

	public const string DataContractXsdBaseNamespace = "http://schemas.datacontract.org/2004/07/";

	public const string DataContractXmlNamespace = "http://schemas.datacontract.org/2004/07/System.Xml";

	public const string SchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

	public const string SchemaNamespace = "http://www.w3.org/2001/XMLSchema";

	public const string XsiNilLocalName = "nil";

	public const string XsiTypeLocalName = "type";

	public const string TnsPrefix = "tns";

	public const string OccursUnbounded = "unbounded";

	public const string AnyTypeLocalName = "anyType";

	public const string StringLocalName = "string";

	public const string IntLocalName = "int";

	public const string True = "true";

	public const string False = "false";

	public const string ArrayPrefix = "ArrayOf";

	public const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";

	public const string XmlnsPrefix = "xmlns";

	public const string SchemaLocalName = "schema";

	public const string CollectionsNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

	public const string DefaultClrNamespace = "GeneratedNamespace";

	public const string DefaultTypeName = "GeneratedType";

	public const string DefaultGeneratedMember = "GeneratedMember";

	public const string DefaultFieldSuffix = "Field";

	public const string DefaultPropertySuffix = "Property";

	public const string DefaultMemberSuffix = "Member";

	public const string NameProperty = "Name";

	public const string NamespaceProperty = "Namespace";

	public const string OrderProperty = "Order";

	public const string IsReferenceProperty = "IsReference";

	public const string IsRequiredProperty = "IsRequired";

	public const string EmitDefaultValueProperty = "EmitDefaultValue";

	public const string ClrNamespaceProperty = "ClrNamespace";

	public const string ItemNameProperty = "ItemName";

	public const string KeyNameProperty = "KeyName";

	public const string ValueNameProperty = "ValueName";

	public const string SerializationInfoPropertyName = "SerializationInfo";

	public const string SerializationInfoFieldName = "info";

	public const string NodeArrayPropertyName = "Nodes";

	public const string NodeArrayFieldName = "nodesField";

	public const string ExportSchemaMethod = "ExportSchema";

	public const string IsAnyProperty = "IsAny";

	public const string ContextFieldName = "context";

	public const string GetObjectDataMethodName = "GetObjectData";

	public const string GetEnumeratorMethodName = "GetEnumerator";

	public const string MoveNextMethodName = "MoveNext";

	public const string AddValueMethodName = "AddValue";

	public const string CurrentPropertyName = "Current";

	public const string ValueProperty = "Value";

	public const string EnumeratorFieldName = "enumerator";

	public const string SerializationEntryFieldName = "entry";

	public const string ExtensionDataSetMethod = "set_ExtensionData";

	public const string ExtensionDataSetExplicitMethod = "System.Runtime.Serialization.IExtensibleDataObject.set_ExtensionData";

	public const string ExtensionDataObjectPropertyName = "ExtensionData";

	public const string ExtensionDataObjectFieldName = "extensionDataField";

	public const string AddMethodName = "Add";

	public const string GetCurrentMethodName = "get_Current";

	public const string SerializationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";

	public const string ClrTypeLocalName = "Type";

	public const string ClrAssemblyLocalName = "Assembly";

	public const string IsValueTypeLocalName = "IsValueType";

	public const string EnumerationValueLocalName = "EnumerationValue";

	public const string SurrogateDataLocalName = "Surrogate";

	public const string GenericTypeLocalName = "GenericType";

	public const string GenericParameterLocalName = "GenericParameter";

	public const string GenericNameAttribute = "Name";

	public const string GenericNamespaceAttribute = "Namespace";

	public const string GenericParameterNestedLevelAttribute = "NestedLevel";

	public const string IsDictionaryLocalName = "IsDictionary";

	public const string ActualTypeLocalName = "ActualType";

	public const string ActualTypeNameAttribute = "Name";

	public const string ActualTypeNamespaceAttribute = "Namespace";

	public const string DefaultValueLocalName = "DefaultValue";

	public const string EmitDefaultValueAttribute = "EmitDefaultValue";

	public const string IdLocalName = "Id";

	public const string RefLocalName = "Ref";

	public const string ArraySizeLocalName = "Size";

	public const string KeyLocalName = "Key";

	public const string ValueLocalName = "Value";

	public const string MscorlibAssemblyName = "0";

	public const string ParseMethodName = "Parse";

	public const string SafeSerializationManagerName = "SafeSerializationManager";

	public const string SafeSerializationManagerNamespace = "http://schemas.datacontract.org/2004/07/System.Runtime.Serialization";

	public const string ISerializableFactoryTypeLocalName = "FactoryType";

	public const string SerializationSchema = "<?xml version='1.0' encoding='utf-8'?>\r\n<xs:schema elementFormDefault='qualified' attributeFormDefault='qualified' xmlns:tns='http://schemas.microsoft.com/2003/10/Serialization/' targetNamespace='http://schemas.microsoft.com/2003/10/Serialization/' xmlns:xs='http://www.w3.org/2001/XMLSchema'>\r\n  <xs:element name='anyType' nillable='true' type='xs:anyType' />\r\n  <xs:element name='anyURI' nillable='true' type='xs:anyURI' />\r\n  <xs:element name='base64Binary' nillable='true' type='xs:base64Binary' />\r\n  <xs:element name='boolean' nillable='true' type='xs:boolean' />\r\n  <xs:element name='byte' nillable='true' type='xs:byte' />\r\n  <xs:element name='dateTime' nillable='true' type='xs:dateTime' />\r\n  <xs:element name='decimal' nillable='true' type='xs:decimal' />\r\n  <xs:element name='double' nillable='true' type='xs:double' />\r\n  <xs:element name='float' nillable='true' type='xs:float' />\r\n  <xs:element name='int' nillable='true' type='xs:int' />\r\n  <xs:element name='long' nillable='true' type='xs:long' />\r\n  <xs:element name='QName' nillable='true' type='xs:QName' />\r\n  <xs:element name='short' nillable='true' type='xs:short' />\r\n  <xs:element name='string' nillable='true' type='xs:string' />\r\n  <xs:element name='unsignedByte' nillable='true' type='xs:unsignedByte' />\r\n  <xs:element name='unsignedInt' nillable='true' type='xs:unsignedInt' />\r\n  <xs:element name='unsignedLong' nillable='true' type='xs:unsignedLong' />\r\n  <xs:element name='unsignedShort' nillable='true' type='xs:unsignedShort' />\r\n  <xs:element name='char' nillable='true' type='tns:char' />\r\n  <xs:simpleType name='char'>\r\n    <xs:restriction base='xs:int'/>\r\n  </xs:simpleType>\r\n  <xs:element name='duration' nillable='true' type='tns:duration' />\r\n  <xs:simpleType name='duration'>\r\n    <xs:restriction base='xs:duration'>\r\n      <xs:pattern value='\\-?P(\\d*D)?(T(\\d*H)?(\\d*M)?(\\d*(\\.\\d*)?S)?)?' />\r\n      <xs:minInclusive value='-P10675199DT2H48M5.4775808S' />\r\n      <xs:maxInclusive value='P10675199DT2H48M5.4775807S' />\r\n    </xs:restriction>\r\n  </xs:simpleType>\r\n  <xs:element name='guid' nillable='true' type='tns:guid' />\r\n  <xs:simpleType name='guid'>\r\n    <xs:restriction base='xs:string'>\r\n      <xs:pattern value='[\\da-fA-F]{8}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{12}' />\r\n    </xs:restriction>\r\n  </xs:simpleType>\r\n  <xs:attribute name='FactoryType' type='xs:QName' />\r\n  <xs:attribute name='Id' type='xs:ID' />\r\n  <xs:attribute name='Ref' type='xs:IDREF' />\r\n  <xs:simpleType name='dateOnly'>\r\n    <xs:restriction base='xs:date'>\r\n      <xs:pattern value='([0-9]{4})-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])' />\r\n    </xs:restriction>\r\n  </xs:simpleType>\r\n  <xs:simpleType name='timeOnly'>\r\n    <xs:restriction base='xs:time'>\r\n      <xs:pattern value='([01][0-9]|2[0-3]):([0-5][0-9])(:([0-5][0-9])(\\.[0-9]{1,7})?)?' />\r\n    </xs:restriction>\r\n  </xs:simpleType>\r\n</xs:schema>\r\n";

	internal static XmlQualifiedName IdQualifiedName => _003CIdQualifiedName_003Ek__BackingField ?? (_003CIdQualifiedName_003Ek__BackingField = new XmlQualifiedName("Id", "http://schemas.microsoft.com/2003/10/Serialization/"));

	internal static XmlQualifiedName RefQualifiedName => _003CRefQualifiedName_003Ek__BackingField ?? (_003CRefQualifiedName_003Ek__BackingField = new XmlQualifiedName("Ref", "http://schemas.microsoft.com/2003/10/Serialization/"));

	internal static Type TypeOfObject => _003CTypeOfObject_003Ek__BackingField ?? (_003CTypeOfObject_003Ek__BackingField = typeof(object));

	internal static Type TypeOfValueType => _003CTypeOfValueType_003Ek__BackingField ?? (_003CTypeOfValueType_003Ek__BackingField = typeof(ValueType));

	internal static Type TypeOfArray => _003CTypeOfArray_003Ek__BackingField ?? (_003CTypeOfArray_003Ek__BackingField = typeof(Array));

	internal static Type TypeOfString => _003CTypeOfString_003Ek__BackingField ?? (_003CTypeOfString_003Ek__BackingField = typeof(string));

	internal static Type TypeOfInt => _003CTypeOfInt_003Ek__BackingField ?? (_003CTypeOfInt_003Ek__BackingField = typeof(int));

	internal static Type TypeOfULong => _003CTypeOfULong_003Ek__BackingField ?? (_003CTypeOfULong_003Ek__BackingField = typeof(ulong));

	internal static Type TypeOfVoid => _003CTypeOfVoid_003Ek__BackingField ?? (_003CTypeOfVoid_003Ek__BackingField = typeof(void));

	internal static Type TypeOfByteArray => _003CTypeOfByteArray_003Ek__BackingField ?? (_003CTypeOfByteArray_003Ek__BackingField = typeof(byte[]));

	internal static Type TypeOfTimeSpan => _003CTypeOfTimeSpan_003Ek__BackingField ?? (_003CTypeOfTimeSpan_003Ek__BackingField = typeof(TimeSpan));

	internal static Type TypeOfGuid => _003CTypeOfGuid_003Ek__BackingField ?? (_003CTypeOfGuid_003Ek__BackingField = typeof(Guid));

	internal static Type TypeOfDateTimeOffset => _003CTypeOfDateTimeOffset_003Ek__BackingField ?? (_003CTypeOfDateTimeOffset_003Ek__BackingField = typeof(DateTimeOffset));

	internal static Type TypeOfDateTimeOffsetAdapter => _003CTypeOfDateTimeOffsetAdapter_003Ek__BackingField ?? (_003CTypeOfDateTimeOffsetAdapter_003Ek__BackingField = typeof(DateTimeOffsetAdapter));

	internal static Type TypeOfDateOnly => _003CTypeOfDateOnly_003Ek__BackingField ?? (_003CTypeOfDateOnly_003Ek__BackingField = typeof(DateOnly));

	internal static Type TypeOfTimeOnly => _003CTypeOfTimeOnly_003Ek__BackingField ?? (_003CTypeOfTimeOnly_003Ek__BackingField = typeof(TimeOnly));

	internal static Type TypeOfMemoryStream => _003CTypeOfMemoryStream_003Ek__BackingField ?? (_003CTypeOfMemoryStream_003Ek__BackingField = typeof(MemoryStream));

	internal static Type TypeOfMemoryStreamAdapter => _003CTypeOfMemoryStreamAdapter_003Ek__BackingField ?? (_003CTypeOfMemoryStreamAdapter_003Ek__BackingField = typeof(MemoryStreamAdapter));

	internal static Type TypeOfUri => _003CTypeOfUri_003Ek__BackingField ?? (_003CTypeOfUri_003Ek__BackingField = typeof(Uri));

	internal static Type TypeOfTypeEnumerable => _003CTypeOfTypeEnumerable_003Ek__BackingField ?? (_003CTypeOfTypeEnumerable_003Ek__BackingField = typeof(IEnumerable<Type>));

	internal static Type TypeOfStreamingContext => _003CTypeOfStreamingContext_003Ek__BackingField ?? (_003CTypeOfStreamingContext_003Ek__BackingField = typeof(StreamingContext));

	internal static Type TypeOfISerializable => _003CTypeOfISerializable_003Ek__BackingField ?? (_003CTypeOfISerializable_003Ek__BackingField = typeof(ISerializable));

	internal static Type TypeOfIDeserializationCallback => _003CTypeOfIDeserializationCallback_003Ek__BackingField ?? (_003CTypeOfIDeserializationCallback_003Ek__BackingField = typeof(IDeserializationCallback));

	internal static Type TypeOfIObjectReference => _003CTypeOfIObjectReference_003Ek__BackingField ?? (_003CTypeOfIObjectReference_003Ek__BackingField = typeof(IObjectReference));

	internal static Type TypeOfXmlFormatClassWriterDelegate => _003CTypeOfXmlFormatClassWriterDelegate_003Ek__BackingField ?? (_003CTypeOfXmlFormatClassWriterDelegate_003Ek__BackingField = typeof(XmlFormatClassWriterDelegate));

	internal static Type TypeOfXmlFormatCollectionWriterDelegate => _003CTypeOfXmlFormatCollectionWriterDelegate_003Ek__BackingField ?? (_003CTypeOfXmlFormatCollectionWriterDelegate_003Ek__BackingField = typeof(XmlFormatCollectionWriterDelegate));

	internal static Type TypeOfXmlFormatClassReaderDelegate => _003CTypeOfXmlFormatClassReaderDelegate_003Ek__BackingField ?? (_003CTypeOfXmlFormatClassReaderDelegate_003Ek__BackingField = typeof(XmlFormatClassReaderDelegate));

	internal static Type TypeOfXmlFormatCollectionReaderDelegate => _003CTypeOfXmlFormatCollectionReaderDelegate_003Ek__BackingField ?? (_003CTypeOfXmlFormatCollectionReaderDelegate_003Ek__BackingField = typeof(XmlFormatCollectionReaderDelegate));

	internal static Type TypeOfXmlFormatGetOnlyCollectionReaderDelegate => _003CTypeOfXmlFormatGetOnlyCollectionReaderDelegate_003Ek__BackingField ?? (_003CTypeOfXmlFormatGetOnlyCollectionReaderDelegate_003Ek__BackingField = typeof(XmlFormatGetOnlyCollectionReaderDelegate));

	internal static Type TypeOfKnownTypeAttribute => _003CTypeOfKnownTypeAttribute_003Ek__BackingField ?? (_003CTypeOfKnownTypeAttribute_003Ek__BackingField = typeof(KnownTypeAttribute));

	internal static Type TypeOfDataContractAttribute => _003CTypeOfDataContractAttribute_003Ek__BackingField ?? (_003CTypeOfDataContractAttribute_003Ek__BackingField = typeof(DataContractAttribute));

	internal static Type TypeOfDataMemberAttribute => _003CTypeOfDataMemberAttribute_003Ek__BackingField ?? (_003CTypeOfDataMemberAttribute_003Ek__BackingField = typeof(DataMemberAttribute));

	internal static Type TypeOfEnumMemberAttribute => _003CTypeOfEnumMemberAttribute_003Ek__BackingField ?? (_003CTypeOfEnumMemberAttribute_003Ek__BackingField = typeof(EnumMemberAttribute));

	internal static Type TypeOfCollectionDataContractAttribute => _003CTypeOfCollectionDataContractAttribute_003Ek__BackingField ?? (_003CTypeOfCollectionDataContractAttribute_003Ek__BackingField = typeof(CollectionDataContractAttribute));

	internal static Type TypeOfOptionalFieldAttribute => _003CTypeOfOptionalFieldAttribute_003Ek__BackingField ?? (_003CTypeOfOptionalFieldAttribute_003Ek__BackingField = typeof(OptionalFieldAttribute));

	internal static Type TypeOfObjectArray => _003CTypeOfObjectArray_003Ek__BackingField ?? (_003CTypeOfObjectArray_003Ek__BackingField = typeof(object[]));

	internal static Type TypeOfOnSerializingAttribute => _003CTypeOfOnSerializingAttribute_003Ek__BackingField ?? (_003CTypeOfOnSerializingAttribute_003Ek__BackingField = typeof(OnSerializingAttribute));

	internal static Type TypeOfOnSerializedAttribute => _003CTypeOfOnSerializedAttribute_003Ek__BackingField ?? (_003CTypeOfOnSerializedAttribute_003Ek__BackingField = typeof(OnSerializedAttribute));

	internal static Type TypeOfOnDeserializingAttribute => _003CTypeOfOnDeserializingAttribute_003Ek__BackingField ?? (_003CTypeOfOnDeserializingAttribute_003Ek__BackingField = typeof(OnDeserializingAttribute));

	internal static Type TypeOfOnDeserializedAttribute => _003CTypeOfOnDeserializedAttribute_003Ek__BackingField ?? (_003CTypeOfOnDeserializedAttribute_003Ek__BackingField = typeof(OnDeserializedAttribute));

	internal static Type TypeOfFlagsAttribute => _003CTypeOfFlagsAttribute_003Ek__BackingField ?? (_003CTypeOfFlagsAttribute_003Ek__BackingField = typeof(FlagsAttribute));

	internal static Type TypeOfIXmlSerializable => _003CTypeOfIXmlSerializable_003Ek__BackingField ?? (_003CTypeOfIXmlSerializable_003Ek__BackingField = typeof(IXmlSerializable));

	internal static Type TypeOfXmlSchemaProviderAttribute => _003CTypeOfXmlSchemaProviderAttribute_003Ek__BackingField ?? (_003CTypeOfXmlSchemaProviderAttribute_003Ek__BackingField = typeof(XmlSchemaProviderAttribute));

	internal static Type TypeOfXmlRootAttribute => _003CTypeOfXmlRootAttribute_003Ek__BackingField ?? (_003CTypeOfXmlRootAttribute_003Ek__BackingField = typeof(XmlRootAttribute));

	internal static Type TypeOfXmlQualifiedName => _003CTypeOfXmlQualifiedName_003Ek__BackingField ?? (_003CTypeOfXmlQualifiedName_003Ek__BackingField = typeof(XmlQualifiedName));

	internal static Type TypeOfXmlSchemaType => _003CTypeOfXmlSchemaType_003Ek__BackingField ?? (_003CTypeOfXmlSchemaType_003Ek__BackingField = typeof(XmlSchemaType));

	internal static Type TypeOfIExtensibleDataObject => _003CTypeOfIExtensibleDataObject_003Ek__BackingField ?? (_003CTypeOfIExtensibleDataObject_003Ek__BackingField = typeof(IExtensibleDataObject));

	internal static Type TypeOfExtensionDataObject => _003CTypeOfExtensionDataObject_003Ek__BackingField ?? (_003CTypeOfExtensionDataObject_003Ek__BackingField = typeof(ExtensionDataObject));

	internal static Type TypeOfISerializableDataNode => _003CTypeOfISerializableDataNode_003Ek__BackingField ?? (_003CTypeOfISerializableDataNode_003Ek__BackingField = typeof(ISerializableDataNode));

	internal static Type TypeOfClassDataNode => _003CTypeOfClassDataNode_003Ek__BackingField ?? (_003CTypeOfClassDataNode_003Ek__BackingField = typeof(ClassDataNode));

	internal static Type TypeOfCollectionDataNode => _003CTypeOfCollectionDataNode_003Ek__BackingField ?? (_003CTypeOfCollectionDataNode_003Ek__BackingField = typeof(CollectionDataNode));

	internal static Type TypeOfXmlDataNode => _003CTypeOfXmlDataNode_003Ek__BackingField ?? (_003CTypeOfXmlDataNode_003Ek__BackingField = typeof(XmlDataNode));

	internal static Type TypeOfNullable => _003CTypeOfNullable_003Ek__BackingField ?? (_003CTypeOfNullable_003Ek__BackingField = typeof(Nullable<>));

	internal static Type TypeOfReflectionPointer => _003CTypeOfReflectionPointer_003Ek__BackingField ?? (_003CTypeOfReflectionPointer_003Ek__BackingField = typeof(Pointer));

	internal static Type TypeOfIDictionaryGeneric => _003CTypeOfIDictionaryGeneric_003Ek__BackingField ?? (_003CTypeOfIDictionaryGeneric_003Ek__BackingField = typeof(IDictionary<, >));

	internal static Type TypeOfIDictionary => _003CTypeOfIDictionary_003Ek__BackingField ?? (_003CTypeOfIDictionary_003Ek__BackingField = typeof(IDictionary));

	internal static Type TypeOfIListGeneric => _003CTypeOfIListGeneric_003Ek__BackingField ?? (_003CTypeOfIListGeneric_003Ek__BackingField = typeof(IList<>));

	internal static Type TypeOfIList => _003CTypeOfIList_003Ek__BackingField ?? (_003CTypeOfIList_003Ek__BackingField = typeof(IList));

	internal static Type TypeOfICollectionGeneric => _003CTypeOfICollectionGeneric_003Ek__BackingField ?? (_003CTypeOfICollectionGeneric_003Ek__BackingField = typeof(ICollection<>));

	internal static Type TypeOfICollection => _003CTypeOfICollection_003Ek__BackingField ?? (_003CTypeOfICollection_003Ek__BackingField = typeof(ICollection));

	internal static Type TypeOfIEnumerableGeneric => _003CTypeOfIEnumerableGeneric_003Ek__BackingField ?? (_003CTypeOfIEnumerableGeneric_003Ek__BackingField = typeof(IEnumerable<>));

	internal static Type TypeOfIEnumerable => _003CTypeOfIEnumerable_003Ek__BackingField ?? (_003CTypeOfIEnumerable_003Ek__BackingField = typeof(IEnumerable));

	internal static Type TypeOfIEnumeratorGeneric => _003CTypeOfIEnumeratorGeneric_003Ek__BackingField ?? (_003CTypeOfIEnumeratorGeneric_003Ek__BackingField = typeof(IEnumerator<>));

	internal static Type TypeOfIEnumerator => _003CTypeOfIEnumerator_003Ek__BackingField ?? (_003CTypeOfIEnumerator_003Ek__BackingField = typeof(IEnumerator));

	internal static Type TypeOfKeyValuePair => _003CTypeOfKeyValuePair_003Ek__BackingField ?? (_003CTypeOfKeyValuePair_003Ek__BackingField = typeof(KeyValuePair<, >));

	internal static Type TypeOfKeyValue => _003CTypeOfKeyValue_003Ek__BackingField ?? (_003CTypeOfKeyValue_003Ek__BackingField = typeof(KeyValue<, >));

	internal static Type TypeOfIDictionaryEnumerator => _003CTypeOfIDictionaryEnumerator_003Ek__BackingField ?? (_003CTypeOfIDictionaryEnumerator_003Ek__BackingField = typeof(IDictionaryEnumerator));

	internal static Type TypeOfDictionaryEnumerator => _003CTypeOfDictionaryEnumerator_003Ek__BackingField ?? (_003CTypeOfDictionaryEnumerator_003Ek__BackingField = typeof(CollectionDataContract.DictionaryEnumerator));

	internal static Type TypeOfGenericDictionaryEnumerator => _003CTypeOfGenericDictionaryEnumerator_003Ek__BackingField ?? (_003CTypeOfGenericDictionaryEnumerator_003Ek__BackingField = typeof(CollectionDataContract.GenericDictionaryEnumerator<, >));

	internal static Type TypeOfDictionaryGeneric => _003CTypeOfDictionaryGeneric_003Ek__BackingField ?? (_003CTypeOfDictionaryGeneric_003Ek__BackingField = typeof(Dictionary<, >));

	internal static Type TypeOfHashtable
	{
		[RequiresDynamicCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed.")]
		[RequiresUnreferencedCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed. Make sure all of the required types are preserved.")]
		get
		{
			return _003CTypeOfHashtable_003Ek__BackingField ?? (_003CTypeOfHashtable_003Ek__BackingField = TypeOfDictionaryGeneric.MakeGenericType(TypeOfObject, TypeOfObject));
		}
	}

	internal static Type TypeOfXmlElement => _003CTypeOfXmlElement_003Ek__BackingField ?? (_003CTypeOfXmlElement_003Ek__BackingField = typeof(XmlElement));

	internal static Type TypeOfXmlNodeArray => _003CTypeOfXmlNodeArray_003Ek__BackingField ?? (_003CTypeOfXmlNodeArray_003Ek__BackingField = typeof(XmlNode[]));

	internal static Type TypeOfDBNull => _003CTypeOfDBNull_003Ek__BackingField ?? (_003CTypeOfDBNull_003Ek__BackingField = typeof(DBNull));

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)]
	internal static Type TypeOfSchemaDefinedType => s_typeOfSchemaDefinedType ?? (s_typeOfSchemaDefinedType = typeof(SchemaDefinedType));

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)]
	internal static Type TypeOfSchemaDefinedEnum => s_typeOfSchemaDefinedEnum ?? (s_typeOfSchemaDefinedEnum = typeof(SchemaDefinedEnum));

	internal static MemberInfo SchemaMemberInfoPlaceholder => _003CSchemaMemberInfoPlaceholder_003Ek__BackingField ?? (_003CSchemaMemberInfoPlaceholder_003Ek__BackingField = TypeOfSchemaDefinedType.GetField("_xmlName", BindingFlags.Instance | BindingFlags.NonPublic));

	internal static Uri DataContractXsdBaseNamespaceUri => _003CDataContractXsdBaseNamespaceUri_003Ek__BackingField ?? (_003CDataContractXsdBaseNamespaceUri_003Ek__BackingField = new Uri("http://schemas.datacontract.org/2004/07/"));

	[GeneratedRegex("^[\\s]*System\\.Runtime\\.Serialization[\\s]*,[\\s]*PublicKey[\\s]*=[\\s]*(?i:00240000048000009400000006020000002400005253413100040000010001008d56c76f9e8649383049f383c44be0ec204181822a6c31cf5eb7ef486944d032188ea1d3920763712ccb12d75fb77e9811149e6148e5d32fbaab37611c1878ddc19e20ef135d0cb2cff2bfec3d115810c3d9069638fe4be215dbf795861920e5ab6f7db2e2ceef136ac23d5dd2bf031700aec232f6c6b1c785b4305c123b37ab)[\\s]*$")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
	public static Regex FullSRSInternalsVisibleRegex => _003CRegexGenerator_g_003EF3846317665D1ABF3801AE2FE0960EF8ACBF58466C53082BD1467D37A02DDB4B6__FullSRSInternalsVisibleRegex_0.Instance;
}
