using System.Collections.Generic;
using System.Text;
using sgq;

namespace sgq {
    public class Sql
    {
        public List<Field> fields { get; set; }

        public List<Field> keys {
            get {
                var keys = new List<Field>();
                foreach (var field in this.fields) {
                    if (field.key) {
                        keys.Add(field);
                    }
                }
                return keys;
            }
        }


        public string dataSource { get; set; }

        public string dataSourceFilterCondition { get; set; }

        public string dataSourceFilterConditionInsert { get; set; }

        public string dataSourceFilterConditionUpdate { get; set; }


        public string targetTable { get; set; }

        public string targetSqlLastIdInserted { get; set; }

        public string targetLastIdInserted {
            get {
                Connection conn = new Connection();
                string result = conn.Get_String(this.targetSqlLastIdInserted);
                if (result == "" || result == null) {
                    result = "0";
                }

                return result;
            }
        }

        public string targetSqlLastDateUpdate { get; set; }

        public string targetLastDateUpdate {
            get {
                Connection conn = new Connection();
                string result = conn.Get_String(this.targetSqlLastDateUpdate);
                if (result == "" || result == null) {
                    result = "00-00-00 00:00:00";
                } else {
                    result = result.Substring(6, 2) + "-" + result.Substring(3, 2) + "-" + result.Substring(0, 2) + " " + result.Substring(9, 8);
                }

                return result;
            }
        }

        public Sql() {
        }

        public Sql(string dataSource, string Origem_where, string targetTable, List<Field> fields, string dataSourceFilterConditionInsert, string dataSourceFilterConditionUpdate) {
            this.dataSource = dataSource;
            this.dataSourceFilterCondition = Origem_where;
            this.targetTable = targetTable;
            this.fields = fields;
            this.dataSourceFilterConditionInsert = dataSourceFilterConditionInsert;
            this.dataSourceFilterConditionUpdate = dataSourceFilterConditionUpdate;
        }

        public string Get_Sql_Insert()
        {
            StringBuilder csql = new StringBuilder();
            csql.Append("select cast('insert into " + targetTable + " (");

            string Tem_Virgula = "";
            for (int i = 0; i < fields.Count; i++)
            {
                Tem_Virgula = ((i != fields.Count - 1) ? "," : "");
                csql.Append(fields[i].target + Tem_Virgula);
            }
            csql.Append(") select ' as varchar(MAX)) || cast(");

            for (int i = 0; i < fields.Count; i++)
            {
                Tem_Virgula = ((i != fields.Count - 1) ? "," : "");
                if (fields[i].type == "A")
                    csql.Append(" '''' || NVL(" + fields[i].source + ",'') || '''" + Tem_Virgula + "' || ");
                else
                    csql.Append(" cast(NVL(" + fields[i].source + ",0) as varchar(30)) || '" + Tem_Virgula + "' || ");
            }


            if (this.keys.Count > 0)
            {
                csql.Append("'' || ' + '''");

                csql.Append("' where not exists (select 1 from " + targetTable + " x where ");

                for (int i = 0; i < this.keys.Count; i++)
                {
                    if (this.keys[i].type == "A")
                        csql.Append("x." + keys[i].target + "= ''' || " + this.keys[i].source + " || '''");
                    else
                        csql.Append("x." + keys[i].target + "=' || " + this.keys[i].source + " || '");

                    if (i < this.keys.Count - 1)
                        csql.Append(" and ");
                }
                csql.Append(")' ");
            }
            else
                csql.Append("' || ' + ''''''");

            csql.Append(" as varchar(max)) as Conteudo ");
            csql.Append("from " + dataSource + " ");

            if ((dataSourceFilterCondition != null ? dataSourceFilterCondition : "") != "" || dataSourceFilterConditionInsert != "")
            {
                csql.Append("where ");

                if (dataSourceFilterCondition != "")
                {
                    csql.Append(dataSourceFilterCondition);

                    if (dataSourceFilterConditionInsert != "")
                        csql.Append(" and ");
                }

                if (dataSourceFilterConditionInsert != "")
                    csql.Append(dataSourceFilterConditionInsert);
            }
            string Operador_Concatenacao = "||";
            string Se_Null = "NVL";

            if (typeDB != "ORACLE")
            {
                Operador_Concatenacao = "+";
                Se_Null = "ISNULL";
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        }

        public string Get_Sql_Update()
        {
            StringBuilder csql = new StringBuilder();
            string Tem_Virgula = "";
            string Tem_Or = "";
            string Tem_Apostrofo = "";

            if (typeDB != "ORACLE")
                csql.Append("select CAST('update " + targetTable + " set ");
            else
                csql.Append("select 'update " + targetTable + " set ");

            for (int i = 0; i < fields.Count; i++)
            {
                if (!fields[i].key)
                {
                    Tem_Virgula = ((i != fields.Count - 1) ? "," : "");
                    Tem_Apostrofo = (fields[i].type == "A" ? "''" : "");

                    if (fields[i].type == "A")
                        csql.Append(fields[i].target + "=" + Tem_Apostrofo + "' || cast(NVL(" + fields[i].source + ",'') as varchar(MAX)) || '" + Tem_Apostrofo + Tem_Virgula);
                    else
                        csql.Append(fields[i].target + "=" + Tem_Apostrofo + "' || cast(NVL(" + fields[i].source + ",0) as varchar(20)) || '" + Tem_Apostrofo + Tem_Virgula);
                }
            }

            if (typeDB != "ORACLE")
                csql.Append("' AS VARCHAR(MAX)) || ' where ");
            else
                csql.Append(" where ");

            for (int i = 0; i < keys.Count; i++)
            {
                Tem_Virgula = ((i != keys.Count - 1) ? "," : "");
                Tem_Apostrofo = (fields[i].type == "A" ? "''" : "");

                csql.Append(keys[i].target + "=" + Tem_Apostrofo + "' || " + keys[i].source + " || '" + Tem_Apostrofo + " and ");
            }
            csql.Append("(");
            for (int i = 0; i < fields.Count; i++)
            {
                if (!fields[i].key)
                {
                    Tem_Or = ((i != fields.Count - 1) ? " or " : "");
                    Tem_Apostrofo = (fields[i].type == "A" ? "''" : "");

                    if (fields[i].type == "A")
                        csql.Append(fields[i].target + "<>" + Tem_Apostrofo + "' || NVL(" + fields[i].source + ",'') || '" + Tem_Apostrofo + Tem_Or);
                    else
                        csql.Append(fields[i].target + "<>" + Tem_Apostrofo + "' || cast(NVL(" + fields[i].source + ",0) as varchar(20)) || '" + Tem_Apostrofo + Tem_Or);
                }
            }
            csql.Append(")' as Conteudo ");

            csql.Append("from " + dataSource + " ");

            if ((dataSourceFilterCondition != null ? dataSourceFilterCondition : "") != "" || dataSourceFilterConditionUpdate != "")
            {
                csql.Append("where ");

                if (dataSourceFilterCondition != "")
                {
                    csql.Append(dataSourceFilterCondition);

                    if (dataSourceFilterConditionUpdate != "")
                        csql.Append(" and ");
                }

                if (dataSourceFilterConditionUpdate != "")
                    csql.Append(dataSourceFilterConditionUpdate);
            }
            string Operador_Concatenacao = "||";
            string Se_Null = "NVL";

            if (typeDB != "ORACLE")
            {
                Operador_Concatenacao = "+";
                Se_Null = "ISNULL";
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        }

        //public string GetInsert()
        //{
        //    StringBuilder csql = new StringBuilder();
        //    csql.Append("select ");
        //    csql.Append("'insert into " + targetTable + " (");

        //    string Tem_Virgula = "";

        //    for (int i = 0; i < fields.Count; i++)
        //    {
        //        Tem_Virgula = ((i != fields.Count - 1) ? "," : "");
        //        csql.Append(fields[i].target + Tem_Virgula);
        //    }
        //    csql.Append(") select ' || ");

        //    for (int i = 0; i < fields.Count; i++)
        //    {
        //        Tem_Virgula = ((i != fields.Count - 1) ? "," : "");
        //        if (fields[i].type == "A")
        //            csql.Append("'''' || NVL(" + fields[i].source + ",'') || '''" + Tem_Virgula + "' || ");
        //        else
        //            csql.Append(" cast(NVL(" + fields[i].source + ",0) as varchar(30)) || '" + Tem_Virgula + "' || ");
        //    }

        //    csql.Append("' where not exists (select 1 from " + targetTable + " x where ");

        //    csql.Append(")' as Conteudo ");

        //    csql.Append("from " + dataSource + " ");

        //    if (dataSourceFilterCondition != "" || dataSourceFilterConditionInsert != "")
        //    {
        //        csql.Append("where ");

        //        if (dataSourceFilterCondition != "")
        //        {
        //            csql.Append(dataSourceFilterCondition);

        //            if (dataSourceFilterConditionInsert != "")
        //                csql.Append(" and ");
        //        }

        //        if (dataSourceFilterConditionInsert != "")
        //            csql.Append(dataSourceFilterConditionInsert);
        //    }
        //    string Operador_Concatenacao = "||";
        //    string Se_Null = "NVL";

        //    if (typeDB != "ORACLE")
        //    {
        //        Operador_Concatenacao = "+";
        //        Se_Null = "ISNULL";
        //    }
        //    return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        //}

        //public string GetUpdateNull()
        //{
        //    StringBuilder csql = new StringBuilder();
        //    for (int i = 0; i < fields.Count; i++)
        //    {
        //        if (fields[i].type == "A")
        //            csql.Append("update " + targetTable + " set " + fields[i].target + " = '' where " + fields[i].target + " is null;");
        //        else
        //            csql.Append("update " + targetTable + " set " + fields[i].target + " = 0 where " + fields[i].target + " is null;");
        //    }
        //    return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " "); ;
        //}

        //public string GetUpdate()
        //{
        //    StringBuilder csql = new StringBuilder();
        //    csql.Append("select ");
        //    csql.Append("to_clob('update " + targetTable + " set ");

        //    string Tem_Virgula = "";

        //    csql.Append(" from (select ') || ");

        //    for (int i = 0; i < fields.Count; i++)
        //    {
        //        Tem_Virgula = ((i != fields.Count - 1) ? "," : "");
        //        if (fields[i].type == "A")
        //            csql.Append("'''' || " + fields[i].source + " || ''' _" + fields[i].target + Tem_Virgula + "' || ");
        //        else
        //            csql.Append(" cast(NVL(" + fields[i].source + " || ',0) as varchar) _" + fields[i].target + Tem_Virgula + "' || ");

        //    }
        //    csql.Append("') as Aux ");
        //    csql.Append("where ");

        //    string Tem_Or = "";
        //    for (int i = 0; i < fields.Count; i++)
        //    {
        //        bool EChave = false;
        //        if (!EChave)
        //        {
        //            csql.Append("'");
        //            Tem_Or = ((i != fields.Count - 1) ? " or " : "");
        //            csql.Append(fields[i].target + "<>_" + fields[i].target + " or ");
        //            csql.Append("(" + fields[i].target + " is null and _" + fields[i].target + " is not null)" + " or ");
        //            csql.Append("(" + fields[i].target + " is not null and _" + fields[i].target + " is null)" + Tem_Or);
        //            csql.Append("' || ");
        //        }
        //    }
        //    csql.Append(" ')' as Conteudo ");

        //    csql.Append("from " + dataSource + " ");

        //    if (dataSourceFilterConditionUpdate != "")
        //        csql.Append("where " + dataSourceFilterConditionUpdate);

        //    return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
        //}

        public string typeDB { get; set; }
    }

}
