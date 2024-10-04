using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.Gen
{

    internal class TableInitDataDecorator : IDisposable
    {
        private string type;
        private StringBuilder stringBuilder;
        public TableInitDataDecorator(string type, StringBuilder stringBuilder)
        {
            this.type = type;
            this.stringBuilder = stringBuilder;

            Init();
        }

        private void Init()
        {
            if (IsContainerType(type))
            {
                stringBuilder.Append("new ").Append(type).Append("(){");
            }

            if (IsText(type))
            {
                stringBuilder.Append("\"");
            }


        }

        public void Dispose()
        {
            if (IsContainerType(type))
                stringBuilder.Append("}");

            if (IsText(type))
            {
                stringBuilder.Append("\"");
            }

            if (IsFloat(type))
            {
                stringBuilder.Append("f");
            }

        }




        private bool IsContainerType(string type)
        {
            if (type.Contains("List"))
                return true;

            return false;

        }

        private bool IsText(string type)
        {
            if (type.ToLower().Equals("string"))
                return true;

            return false;
        }

        private bool IsFloat(string type)
        {
            if (type.ToLower().Equals("float") || type.ToLower().Equals("double"))
                return true;

            return false;
        }

    }
}
