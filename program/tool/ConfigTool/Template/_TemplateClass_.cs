using System;
using System.Collections;
using System.Collections.Generic;


namespace AutoConfig
{
#if _TableKeyEnum_
	public enum _TemplateClassEnum_
	{
		_TableKeyEnumContent_
	}
#endif

    public class _TemplateClass_
	{
		public _TemplateClass_() { }

#if _ClassField_
		// _Comment_
		public _FieldType_ _FieldName_; 
#endif

	}


	//可以对Base进行一些操作，比如解析json
	public class _TemplateClass_Ext
	{
#if _ExtField_
		public _FieldType_ _FieldName_ { get {return Base._FieldName_;} set {Base._FieldName_=value;}}
#endif

		public _TemplateClass_ Base { get; private set; }

		public _TemplateClass_Ext()
		{
			Base = new _TemplateClass_();
		}

		public _TemplateClass_Ext(_TemplateClass_ info)
		{
			Base = info;
		}

	}


	public class _TemplateClass_ExtTable 
	{

#if _ExtTable_
		private static Dictionary<_KeyType_, _TemplateClass_Ext> key2ConfigDict = new Dictionary<_KeyType_, _TemplateClass_Ext>();
#endif
        static _TemplateClass_ExtTable()
		{
			InitData();
		}

		private static void InitData()
		{
#if _ExtTableData_

			var item_Index_ = new _TemplateClass_() {
				_InitField_
			};
            key2ConfigDict.Add(item_Index_._Key_, new _TemplateClass_Ext(item_Index_) );

#endif
        }


#if _ExtTable_
		public static _TemplateClass_Ext GetData(_KeyType_ key)
		{
			return key2ConfigDict[key];
		}
#endif

    }
}

