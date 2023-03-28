using Newtonsoft.Json;
using ProjectCommon.MySql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.Gen
{

    class MySqlGen : GenBase
    {
        public MySqlGen()
        {
        }

        Assembly SqlAssembly { get; set; }
        private MySqlBase mySql = new MySqlBase();

        protected override void GenerateImpl(string outPath)
        {
            SqlAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(outPath); 
            if (SqlAssembly == null)
            {
                Console.WriteLine("LoadSqlDll fail");
                return;
            }

            var mystr = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            mySql.InitMySqlCreateTableFromAssembly(mystr, SqlAssembly);

            foreach (var st in SqlTableDict)
            {
                UpdateSql(st.Value);
            }
        }

        void UpdateSql(SqlTable sqlTable)
        {
            try
            {
                var instance = SqlAssembly.CreateInstance("SqlDataCommon." + sqlTable.Name); // 创建类的实例                 
                if (instance == null)
                {
                    Console.WriteLine("assembly error: SqlDataCommon." + sqlTable.Name);
                    return;
                }

                var objectType = instance.GetType();

                var rowlist = sqlTable.Data.Rows;
                if (rowlist.Count <= 3)
                    return;//空表

                using (var session = mySql.OpenStatelesSession())
                {
                    using (var trans = session.BeginTransaction())
                    {
                        for (int i = 3; i < rowlist.Count; ++i)
                        {
                            if (string.IsNullOrEmpty(rowlist[i][0].ToString()))
                                break;

                            var entity = SqlAssembly.CreateInstance(objectType.FullName);
                            //object find = null;

                            for (int j = 0; j < sqlTable.Fields.Count; ++j)
                            {
                                var sqlField = sqlTable.Fields[j];
                                var field = objectType.GetProperty(sqlField.Name);
                                string valueStr = rowlist[i][j].ToString();

                                // update 查找原来的项
                                //if (sqlField.Attributes.ContainsKey("id"))
                                //{
                                //    string hql = string.Format("from {0} tmp where tmp.{1}=:wkey", st.Name, sqlField.Name);
                                //    var cq = session.CreateQuery(hql);
                                //    cq.SetParameter("wkey", ConvertFieldType(sqlField, valueStr));
                                //    var list = cq.List();
                                //    if (list.Count > 0)
                                //        find = list[0];
                                //}

                                //if (find != null)
                                //    field.SetValue(find, ConvertFieldType(sf, fv));
                                //else
                                    field.SetValue(entity, ConvertFieldType(sqlField, valueStr));
                            }

                            //if (find == null)
                                session.Insert(entity);
                            //else if (Update)
                            //    session.Update(find);
                        }

                        trans.Commit();
                        Console.WriteLine("UpdateSql complete: " + sqlTable.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateSql " + sqlTable.Name + " error: " + ex.Message);
            }
        }

        object ConvertFieldType(SqlField sqlField, string v)
        {
            switch (sqlField.Type)
            {
                case "short":
                    {
                        return short.Parse(v);
                    }
                case "ushort":
                    {
                        return ushort.Parse(v);
                    }
                case "int":
                    {
                        return int.Parse(v);
                    }
                case "uint":
                    {
                        return uint.Parse(v);
                    }
                case "int64":
                    {
                        return Int64.Parse(v);
                    }
                case "uint64":
                    {
                        return UInt64.Parse(v);
                    }
                case "bool":
                    {
                        return bool.Parse(v);
                    }
                case "float":
                    {
                        return float.Parse(v);
                    }
                case "double":
                    {
                        return double.Parse(v);
                    }
                case "string":
                    {
                        if (!sqlField.Attributes.ContainsKey("index"))
                        {
                            if (sqlField.Attributes.ContainsKey("list"))
                            {
                                var list = new string[0];
                                if (!string.IsNullOrEmpty(v))
                                    list = v.Split(',');
                                v = JsonConvert.SerializeObject(list);
                            }
                            else if (sqlField.Attributes.ContainsKey("map") || sqlField.Attributes.ContainsKey("smap"))
                            {
                                var list = v.Split(',');
                                Dictionary<string, string> map = new Dictionary<string, string>();
                                foreach (var i in list)
                                {
                                    var sp = i.Split('&');
                                    if (sp.Length == 2)
                                    {
                                        map.Add(sp[0], sp[1]);
                                    }
                                }
                                v = JsonConvert.SerializeObject(map);
                            }
                        }
                        return v;
                    }
                case "date":
                    {
                        return DateTime.Parse(v);
                    }
                case "guid":
                    {
                        return Guid.Parse(v);
                    }
                default: return null;
            }
        }

    }
}
