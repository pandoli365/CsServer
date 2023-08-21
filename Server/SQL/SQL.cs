using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Server.System;

namespace Server.SQL
{
    public class SQL<T>
    {
        string className;
        Regex regex = new Regex(STATICS.PATTERN);

        public SQL()
        {
            className = typeof(T).Name;
        }

        public string sqlInsert(T instance)
        {
            List<string> names = new List<string>();
            List<string> values = new List<string>();
            foreach (FieldInfo field in typeof(T).GetFields())
            {
                object value = field.GetValue(instance);
                if (value == null)
                    continue;
                names.Add(field.Name);
                values.Add(value.ToString());
            }

            StringBuilder qurry = new StringBuilder();

            qurry.Append($"INSERT INTO {className} (");

            int n = 0;
            int count = names.Count;

            for(; n < count; n++)
            {
                qurry.Append(names[n]);
                if(n != count - 1)
                {
                    qurry.Append(", ");
                }
            }

            qurry.Append(") VALUES (");
            n = 0;
            for (; n < count; n++)
            {
                qurry.Append(values[n]);
                if (n != count - 1)
                {
                    qurry.Append(", ");
                }
            }
            qurry.Append(");");

            return qurry.ToString();
        }

        public string sqlUpdate(string[] names, object[] values, string[] wnames, object[] wvalues)
        {
            StringBuilder qurry = new StringBuilder();
            qurry.Append($"UPDATE {className} SET ");
            for(int n = 0; n < names.Length; n++)
            {
                qurry.Append($"{names[n]} = {values[n]}");
                if(n < names.Length - 1)
                {
                    qurry.Append(", ");
                }
            }
            qurry.Append(" WHERE ");
            for (int n = 0; n < wnames.Length; n++)
            {
                qurry.Append($"{wnames[n]} = {wvalues[n]}");
                if (n < wnames.Length - 1)
                {
                    qurry.Append(", ");
                }
            }
            return qurry.ToString();
        }

        public string sqlSelect(string[] names, string[] wnames, object[] wvalues)
        {
            StringBuilder qurry = new StringBuilder();

            if(names == null)
            {
                qurry.Append($"SELECT * FROM {className} ");
            }
            else
            {
                qurry.Append("SELECT ");
                for(int n = 0; n < names.Length; n++)
                {
                    qurry.Append(names[n]);
                    if (n < names.Length - 1)
                    {
                        qurry.Append(", ");
                    }
                }
                qurry.Append($" FROM {className} ");
            }

            qurry.Append(" WHERE ");
            for (int n = 0; n < wnames.Length; n++)
            {
                qurry.Append($"{wnames[n]} = {wvalues[n]}");
                if (n < wnames.Length - 1)
                {
                    qurry.Append(", ");
                }
            }
            return qurry.ToString();
        }

        public string sqlDelete(string[] wnames, object[] wvalues)
        {
            StringBuilder qurry = new StringBuilder();
            qurry.Append($"DELETE FROM {className}");

            qurry.Append(" WHERE ");
            for (int n = 0; n < wnames.Length; n++)
            {
                qurry.Append($"{wnames[n]} = {wvalues[n]}");
                if (n < wnames.Length - 1)
                {
                    qurry.Append(", ");
                }
            }
            return qurry.ToString();
        }
        public string Injection(string data)
        {
            return regex.Replace(data, "");
        }
    }
}

