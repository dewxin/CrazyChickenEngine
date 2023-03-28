using System;
using FluentNHibernate.Mapping;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ProjectCommon.MySql;


namespace SqlDataCommon
{
	public class _TemplateClass_
	{
		public _TemplateClass_() { }

#if _ClassField_
		// _Comment_
		public virtual _FieldType_ _FieldName_ {get; set;}
#endif

	}

	public class _TemplateClass_Map : ClassMap<_TemplateClass_>
	{
		public _TemplateClass_Map()
		{
#if _Map_
#endif
		}
	}

	public class _TemplateClass_Ext
	{
#if _ExtField_
		public _FieldType_ _FieldName_ { get {return Base._FieldName_;} set {Base._FieldName_=value;}}
#endif

#if _ExtJson_
		[JsonIgnore]
		public _ContainerType_<_ContainerTrick__FieldType_> _FieldName__ContainerType_ = new();
		private void Parse_FieldName_Ext()
		{
			if (string.IsNullOrEmpty(_FieldName_)) return;
			_FieldName__ContainerType_ = JsonConvert.DeserializeObject<_ContainerType_<_ContainerTrick__FieldType_>>(_FieldName_);
		}

#endif
		public _TemplateClass_ Base { get; private set; }

		public _TemplateClass_Ext()
		{
			Base = new _TemplateClass_();
		}

		public _TemplateClass_Ext(_TemplateClass_ info)
		{
			Base = info;
#if _ExtJson_
			Parse_FieldName_Ext();
#endif
		}

		public void StoreJson()
		{
#if _ExtJson_
			_FieldName_ = JsonConvert.SerializeObject(_FieldName__ContainerType_);
#endif
		}
	}


	public class _TemplateClass_ExtTable : SqlConfigBase<_TemplateClass_, _TemplateClass_Ext>
	{
		public _TemplateClass_ExtTable()
		{
			TableName = nameof(_TemplateClass_);
		}

		public override void ParseConfigExtToDictAfterLoadFromDb(IList<_TemplateClass_> list)
		{
			foreach (var it in list)
			{
#if _ExtTable_
				if (!extTableAsDict.ContainsKey(it._Key_))
				{
					extTableAsDict.Add(it._Key_, new _TemplateClass_Ext(it));
				}
#endif
			}
		}
	}
}

