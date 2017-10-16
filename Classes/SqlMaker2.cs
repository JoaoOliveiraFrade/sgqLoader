using System.Text;

namespace sgq
{
    public class SqlMaker2
    {
        public SqlMaker2Param sqlMaker2Param { get; set; }

        public string Get_Oracle_Insert()
        {
            StringBuilder csql = new StringBuilder();
            csql.Append("select 'insert into " + sqlMaker2Param.targetTable + " (");

            string Tem_Virgula = "";
            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                csql.Append(sqlMaker2Param.fields[i].target + Tem_Virgula);
            }
            csql.Append(") select ' || ");

            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                if (sqlMaker2Param.fields[i].type == "A")
                    csql.Append(" '''' || NVL(" + sqlMaker2Param.fields[i].source + ",'') || '''" + Tem_Virgula + "' || ");
                else
                    csql.Append(" cast(NVL(" + sqlMaker2Param.fields[i].source + ",0) as varchar(30)) || '" + Tem_Virgula + "' || ");
            }


            if (sqlMaker2Param.keys.Count > 0)
            {
                csql.Append("'' || ' + '''");

                csql.Append("' where not exists (select 1 from " + sqlMaker2Param.targetTable + " x where ");

                for (int i = 0; i < sqlMaker2Param.keys.Count; i++)
                {
                    if (sqlMaker2Param.keys[i].type == "A")
                        csql.Append("x." + sqlMaker2Param.keys[i].target + "= ''' || " + sqlMaker2Param.keys[i].source + " || '''");
                    else
                        csql.Append("x." + sqlMaker2Param.keys[i].target + "=' || " + sqlMaker2Param.keys[i].source + " || '");

                    if (i < sqlMaker2Param.keys.Count - 1)
                        csql.Append(" and ");
                }
                csql.Append(")' ");
            }
            else
                csql.Append("' || ' + ''''''");

            csql.Append(" as Conteudo ");
            csql.Append("from " + sqlMaker2Param.dataSource + " ");

            if ((sqlMaker2Param.dataSourceFilterCondition != null ? sqlMaker2Param.dataSourceFilterCondition : "") != "" || sqlMaker2Param.dataSourceFilterConditionInsert != "")
            {
                csql.Append("where ");

                if (sqlMaker2Param.dataSourceFilterCondition != "")
                {
                    csql.Append(sqlMaker2Param.dataSourceFilterCondition);

                    if (sqlMaker2Param.dataSourceFilterConditionInsert != "")
                        csql.Append(" and ");
                }

                if (sqlMaker2Param.dataSourceFilterConditionInsert != "")
                    csql.Append(sqlMaker2Param.dataSourceFilterConditionInsert);
            }
            string Operador_Concatenacao = "||";
            string Se_Null = "NVL";

            if (sqlMaker2Param.typeDB != "ORACLE")
            {
                Operador_Concatenacao = "+";
                Se_Null = "ISNULL";
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        }

        public string Get_Oracle_Update() {
            StringBuilder csql = new StringBuilder();
            string Tem_Virgula = "";
            string Tem_Or = "";
            string Tem_Apostrofo = "";

            if (sqlMaker2Param.typeDB != "ORACLE")
                csql.Append("select CAST('update " + sqlMaker2Param.targetTable + " set ");
            else
                csql.Append("select 'update " + sqlMaker2Param.targetTable + " set ");

            for (int i = 0; i < sqlMaker2Param.fields.Count; i++) {
                if (!sqlMaker2Param.fields[i].key) {
                    Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                    Tem_Apostrofo = (sqlMaker2Param.fields[i].type == "A" ? "''" : "");

                    if (sqlMaker2Param.fields[i].type == "A")
                        csql.Append(sqlMaker2Param.fields[i].target + "=" + Tem_Apostrofo + "' || NVL(" + sqlMaker2Param.fields[i].source + ",'') || '" + Tem_Apostrofo + Tem_Virgula);
                    else
                        csql.Append(sqlMaker2Param.fields[i].target + "=" + Tem_Apostrofo + "' || cast(NVL(" + sqlMaker2Param.fields[i].source + ",0) as varchar(20)) || '" + Tem_Apostrofo + Tem_Virgula);
                }
            }

            if (sqlMaker2Param.typeDB != "ORACLE")
                csql.Append("' AS VARCHAR(MAX)) || ' where ");
            else
                csql.Append(" where ");

            for (int i = 0; i < sqlMaker2Param.keys.Count; i++) {
                Tem_Virgula = ((i != sqlMaker2Param.keys.Count - 1) ? "," : "");
                Tem_Apostrofo = (sqlMaker2Param.fields[i].type == "A" ? "''" : "");

                csql.Append(sqlMaker2Param.keys[i].target + "=" + Tem_Apostrofo + "' || " + sqlMaker2Param.keys[i].source + " || '" + Tem_Apostrofo + " and ");
            }
            csql.Append("(");
            for (int i = 0; i < sqlMaker2Param.fields.Count; i++) {
                if (!sqlMaker2Param.fields[i].key) {
                    Tem_Or = ((i != sqlMaker2Param.fields.Count - 1) ? " or " : "");
                    Tem_Apostrofo = (sqlMaker2Param.fields[i].type == "A" ? "''" : "");

                    if (sqlMaker2Param.fields[i].type == "A")
                        csql.Append(sqlMaker2Param.fields[i].target + "<>" + Tem_Apostrofo + "' || NVL(" + sqlMaker2Param.fields[i].source + ",'') || '" + Tem_Apostrofo + Tem_Or);
                    else
                        csql.Append(sqlMaker2Param.fields[i].target + "<>" + Tem_Apostrofo + "' || cast(NVL(" + sqlMaker2Param.fields[i].source + ",0) as varchar(20)) || '" + Tem_Apostrofo + Tem_Or);
                }
            }
            csql.Append(")' as Conteudo ");

            csql.Append("from " + sqlMaker2Param.dataSource + " ");

            if ((sqlMaker2Param.dataSourceFilterCondition != null ? sqlMaker2Param.dataSourceFilterCondition : "") != "" || sqlMaker2Param.dataSourceFilterConditionUpdate != "") {
                csql.Append("where ");

                if (sqlMaker2Param.dataSourceFilterCondition != "") {
                    csql.Append(sqlMaker2Param.dataSourceFilterCondition);

                    if (sqlMaker2Param.dataSourceFilterConditionUpdate != "")
                        csql.Append(" and ");
                }

                if (sqlMaker2Param.dataSourceFilterConditionUpdate != "")
                    csql.Append(sqlMaker2Param.dataSourceFilterConditionUpdate);
            }
            string Operador_Concatenacao = "||";
            string Se_Null = "NVL";

            if (sqlMaker2Param.typeDB != "ORACLE") {
                Operador_Concatenacao = "+";
                Se_Null = "ISNULL";
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        }


        public string Get_Sql_Insert()
        {
            StringBuilder csql = new StringBuilder();
            csql.Append("select cast('insert into " + sqlMaker2Param.targetTable + " (");

            string Tem_Virgula = "";
            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                csql.Append(sqlMaker2Param.fields[i].target + Tem_Virgula);
            }
            csql.Append(") select ' as varchar(MAX)) || cast(");

            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                if (sqlMaker2Param.fields[i].type == "A")
                    csql.Append(" '''' || NVL(" + sqlMaker2Param.fields[i].source + ",'') || '''" + Tem_Virgula + "' || ");
                else
                    csql.Append(" cast(NVL(" + sqlMaker2Param.fields[i].source + ",0) as varchar(30)) || '" + Tem_Virgula + "' || ");
            }


            if (sqlMaker2Param.keys.Count > 0)
            {
                csql.Append("'' || ' + '''");

                csql.Append("' where not exists (select 1 from " + sqlMaker2Param.targetTable + " x where ");

                for (int i = 0; i < sqlMaker2Param.keys.Count; i++)
                {
                    if (sqlMaker2Param.keys[i].type == "A")
                        csql.Append("x." + sqlMaker2Param.keys[i].target + "= ''' || " + sqlMaker2Param.keys[i].source + " || '''");
                    else
                        csql.Append("x." + sqlMaker2Param.keys[i].target + "=' || " + sqlMaker2Param.keys[i].source + " || '");

                    if (i < sqlMaker2Param.keys.Count - 1)
                        csql.Append(" and ");
                }
                csql.Append(")' ");
            }
            else
                csql.Append("' || ' + ''''''");

            csql.Append(" as varchar(max)) as Conteudo ");
            csql.Append("from " + sqlMaker2Param.dataSource + " ");

            if ((sqlMaker2Param.dataSourceFilterCondition != null ? sqlMaker2Param.dataSourceFilterCondition : "") != "" || sqlMaker2Param.dataSourceFilterConditionInsert != "")
            {
                csql.Append("where ");

                if (sqlMaker2Param.dataSourceFilterCondition != "")
                {
                    csql.Append(sqlMaker2Param.dataSourceFilterCondition);

                    if (sqlMaker2Param.dataSourceFilterConditionInsert != "")
                        csql.Append(" and ");
                }

                if (sqlMaker2Param.dataSourceFilterConditionInsert != "")
                    csql.Append(sqlMaker2Param.dataSourceFilterConditionInsert);
            }
            string Operador_Concatenacao = "||";
            string Se_Null = "NVL";

            if (sqlMaker2Param.typeDB != "ORACLE")
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

            if (sqlMaker2Param.typeDB != "ORACLE")
                csql.Append("select CAST('update " + sqlMaker2Param.targetTable + " set ");
            else
                csql.Append("select 'update " + sqlMaker2Param.targetTable + " set ");

            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                if (!sqlMaker2Param.fields[i].key)
                {
                    Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                    Tem_Apostrofo = (sqlMaker2Param.fields[i].type == "A" ? "''" : "");

                    if (sqlMaker2Param.fields[i].type == "A")
                        csql.Append(sqlMaker2Param.fields[i].target + "=" + Tem_Apostrofo + "' || cast(NVL(" + sqlMaker2Param.fields[i].source + ",'') as varchar(MAX)) || '" + Tem_Apostrofo + Tem_Virgula);
                    else
                        csql.Append(sqlMaker2Param.fields[i].target + "=" + Tem_Apostrofo + "' || cast(NVL(" + sqlMaker2Param.fields[i].source + ",0) as varchar(20)) || '" + Tem_Apostrofo + Tem_Virgula);
                }
            }

            if (sqlMaker2Param.typeDB != "ORACLE")
                csql.Append("' AS VARCHAR(MAX)) || ' where ");
            else
                csql.Append(" where ");

            for (int i = 0; i < sqlMaker2Param.keys.Count; i++)
            {
                Tem_Virgula = ((i != sqlMaker2Param.keys.Count - 1) ? "," : "");
                Tem_Apostrofo = (sqlMaker2Param.fields[i].type == "A" ? "''" : "");

                csql.Append(sqlMaker2Param.keys[i].target + "=" + Tem_Apostrofo + "' || " + sqlMaker2Param.keys[i].source + " || '" + Tem_Apostrofo + " and ");
            }
            csql.Append("(");
            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                if (!sqlMaker2Param.fields[i].key)
                {
                    Tem_Or = ((i != sqlMaker2Param.fields.Count - 1) ? " or " : "");
                    Tem_Apostrofo = (sqlMaker2Param.fields[i].type == "A" ? "''" : "");

                    if (sqlMaker2Param.fields[i].type == "A")
                        csql.Append(sqlMaker2Param.fields[i].target + "<>" + Tem_Apostrofo + "' || NVL(" + sqlMaker2Param.fields[i].source + ",'') || '" + Tem_Apostrofo + Tem_Or);
                    else
                        csql.Append(sqlMaker2Param.fields[i].target + "<>" + Tem_Apostrofo + "' || cast(NVL(" + sqlMaker2Param.fields[i].source + ",0) as varchar(20)) || '" + Tem_Apostrofo + Tem_Or);
                }
            }
            csql.Append(")' as Conteudo ");

            csql.Append("from " + sqlMaker2Param.dataSource + " ");

            if ((sqlMaker2Param.dataSourceFilterCondition != null ? sqlMaker2Param.dataSourceFilterCondition : "") != "" || sqlMaker2Param.dataSourceFilterConditionUpdate != "")
            {
                csql.Append("where ");

                if (sqlMaker2Param.dataSourceFilterCondition != "")
                {
                    csql.Append(sqlMaker2Param.dataSourceFilterCondition);

                    if (sqlMaker2Param.dataSourceFilterConditionUpdate != "")
                        csql.Append(" and ");
                }

                if (sqlMaker2Param.dataSourceFilterConditionUpdate != "")
                    csql.Append(sqlMaker2Param.dataSourceFilterConditionUpdate);
            }
            string Operador_Concatenacao = "||";
            string Se_Null = "NVL";

            if (sqlMaker2Param.typeDB != "ORACLE")
            {
                Operador_Concatenacao = "+";
                Se_Null = "ISNULL";
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        }


        public string GetInsert()
        {
            StringBuilder csql = new StringBuilder();
            csql.Append("select ");
            csql.Append("'insert into " + sqlMaker2Param.targetTable + " (");

            string Tem_Virgula = "";

            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                csql.Append(sqlMaker2Param.fields[i].target + Tem_Virgula);
            }
            csql.Append(") select ' || ");

            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                if (sqlMaker2Param.fields[i].type == "A")
                    csql.Append("'''' || NVL(" + sqlMaker2Param.fields[i].source + ",'') || '''" + Tem_Virgula + "' || ");
                else
                    csql.Append(" cast(NVL(" + sqlMaker2Param.fields[i].source + ",0) as varchar(30)) || '" + Tem_Virgula + "' || ");
                //csql.Append(" || cast(NVL(" + sqlMaker2Param.fields[i].source + ",0) as varchar(30)) || '" + Tem_Virgula + "' || ");
            }

            csql.Append("' where not exists (select 1 from " + sqlMaker2Param.targetTable + " x where ");

            //if (sqlMaker2Param.dataSource_Id != null)
            //    csql.Append("x." + sqlMaker2Param.targetTable_Id + "=''' || " + sqlMaker2Param.dataSource_Id + " || '''");
            //else
            //{
            //    for (int i = 0; i < sqlMaker2Param.dataSource_Ids.Count; i++)
            //    {

            //        csql.Append("x." + sqlMaker2Param.targetTable_Ids[i] + "=' || " + sqlMaker2Param.dataSource_Ids[i] + " || '");
            //        if (i < sqlMaker2Param.dataSource_Ids.Count - 1)
            //            csql.Append(" and ");
            //    }
            //}

            csql.Append(")' as Conteudo ");

            csql.Append("from " + sqlMaker2Param.dataSource + " ");

            if (sqlMaker2Param.dataSourceFilterCondition != "" || sqlMaker2Param.dataSourceFilterConditionInsert != "")
            {
                csql.Append("where ");

                if (sqlMaker2Param.dataSourceFilterCondition != "")
                {
                    csql.Append(sqlMaker2Param.dataSourceFilterCondition);

                    if (sqlMaker2Param.dataSourceFilterConditionInsert != "")
                        csql.Append(" and ");
                }

                if (sqlMaker2Param.dataSourceFilterConditionInsert != "")
                    csql.Append(sqlMaker2Param.dataSourceFilterConditionInsert);
            }
            string Operador_Concatenacao = "||";
            string Se_Null = "NVL";

            if (sqlMaker2Param.typeDB != "ORACLE")
            {
                Operador_Concatenacao = "+";
                Se_Null = "ISNULL";
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        }

        public string GetUpdateNull()
        {
            StringBuilder csql = new StringBuilder();
            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                if (sqlMaker2Param.fields[i].type == "A")
                    csql.Append("update " + sqlMaker2Param.targetTable + " set " + sqlMaker2Param.fields[i].target + " = '' where " + sqlMaker2Param.fields[i].target + " is null;");
                else
                    csql.Append("update " + sqlMaker2Param.targetTable + " set " + sqlMaker2Param.fields[i].target + " = 0 where " + sqlMaker2Param.fields[i].target + " is null;");
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " "); ;
        }

        public string GetUpdate()
        {
            StringBuilder csql = new StringBuilder();
            csql.Append("select ");
            csql.Append("to_clob('update " + sqlMaker2Param.targetTable + " set ");

            string Tem_Virgula = "";

            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                //if (sqlMaker2Param.fields[i].target != sqlMaker2Param.targetTable_Id)
                //{
                //    Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                //    csql.Append(sqlMaker2Param.fields[i].target + "=_" + sqlMaker2Param.fields[i].target + Tem_Virgula);
                //}
            }

            csql.Append(" from (select ') || ");

            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                Tem_Virgula = ((i != sqlMaker2Param.fields.Count - 1) ? "," : "");
                if (sqlMaker2Param.fields[i].type == "A")
                    csql.Append("'''' || " + sqlMaker2Param.fields[i].source + " || ''' _" + sqlMaker2Param.fields[i].target + Tem_Virgula + "' || ");
                else
                    csql.Append(" cast(NVL(" + sqlMaker2Param.fields[i].source + " || ',0) as varchar) _" + sqlMaker2Param.fields[i].target + Tem_Virgula + "' || ");

                //if (sqlMaker2Param.fields[i].type == "A")
                //    csql.Append("'''' || NVL(" + sqlMaker2Param.fields[i].source + ",'') || ''' _" + sqlMaker2Param.fields[i].target + Tem_Virgula + "' || ");
                //else
                //    csql.Append("'0' || NVL(" + sqlMaker2Param.fields[i].source + ",'0') || ' _" + sqlMaker2Param.fields[i].target + Tem_Virgula + "' || ");
            }
            csql.Append("') as Aux ");
            csql.Append("where ");

            //if (sqlMaker2Param.dataSource_Id != null)
            //    csql.Append("(" + sqlMaker2Param.targetTable_Id + "=_" + sqlMaker2Param.targetTable_Id + ") and (' || ");
            //else
            //{
            //    for (int i = 0; i < sqlMaker2Param.dataSource_Ids.Count; i++)
            //        csql.Append("(" + sqlMaker2Param.targetTable_Ids[i] + "=_" + sqlMaker2Param.targetTable_Ids[i] + ") and ");

            //    csql.Append("(' || ");
            //}

            string Tem_Or = "";
            for (int i = 0; i < sqlMaker2Param.fields.Count; i++)
            {
                bool EChave = false;
                //if (sqlMaker2Param.dataSource_Id != null)
                //    EChave = sqlMaker2Param.fields[i].target == sqlMaker2Param.targetTable_Id;
                //else
                //    for (int x = 0; x < sqlMaker2Param.targetTable_Ids.Count; x++)
                //        if (sqlMaker2Param.fields[i].target == sqlMaker2Param.targetTable_Ids[x])
                //            EChave = true;

                if (!EChave)
                {
                    csql.Append("'");
                    Tem_Or = ((i != sqlMaker2Param.fields.Count - 1) ? " or " : "");
                    csql.Append(sqlMaker2Param.fields[i].target + "<>_" + sqlMaker2Param.fields[i].target + " or ");
                    csql.Append("(" + sqlMaker2Param.fields[i].target + " is null and _" + sqlMaker2Param.fields[i].target + " is not null)" + " or ");
                    csql.Append("(" + sqlMaker2Param.fields[i].target + " is not null and _" + sqlMaker2Param.fields[i].target + " is null)" + Tem_Or);
                    csql.Append("' || ");
                }
            }
            csql.Append(" ')' as Conteudo ");

            csql.Append("from " + sqlMaker2Param.dataSource + " ");

            if (sqlMaker2Param.dataSourceFilterConditionUpdate != "")
                csql.Append("where " + sqlMaker2Param.dataSourceFilterConditionUpdate);

            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
        }
        
    }

}
