using System.Text;
using Oracle.DataAccess.Client;
using System.Collections.Generic;
using sgq;
using System;

namespace sgq.bpt
{
    public class SqlMaker
    {
        public BptConnection bptConnection { get; set; } 

        public Connection Connection { get; set; } 

        public TypeUpdate typeUpdate { get; set; } 

        public List<Field> fields { get; set; } 

        public List<Field> Keys
        {
            get
            {
                var keys = new List<Field>();
                foreach (var field in this.fields)
                {
                    if (field.key)
                        keys.Add(field);
                }

                return keys;
            }
        } 

        public TypeDatabase typeTargetDatabase { get; set; } 

        public string dataSource { get; set; } 

        public string dataSourceFieldId { get; set; }

        public string dataSourceFieldDateUpdade { get; set; }

        public string dataSourceCondition { get; set; }

        public string TargetTable { get; set; } 

        public string GetSqlInsert()
        {
            StringBuilder csql = new StringBuilder();
            csql.Append("select 'insert into " + this.TargetTable + " (");

            string Tem_Virgula = "";
            for (int i = 0; i < this.fields.Count; i++)
            {
                Tem_Virgula = ((i != this.fields.Count - 1) ? "," : "");
                csql.Append(this.fields[i].target + Tem_Virgula);
            }
            csql.Append(") select ' || ");

            for (int i = 0; i < this.fields.Count; i++)
            {
                Tem_Virgula = ((i != this.fields.Count - 1) ? "," : "");
                if (this.fields[i].type == "A")
                    csql.Append(" '''' || NVL(" + this.fields[i].source + ",'') || '''" + Tem_Virgula + "' || ");
                else
                    csql.Append(" cast(NVL(" + this.fields[i].source + ",0) as varchar(30)) || '" + Tem_Virgula + "' || ");
            }

            if (this.Keys.Count > 0)
            {
                csql.Append("'' || ' + '''");

                csql.Append("' where not exists (select 1 from " + this.TargetTable + " x where ");

                for (int i = 0; i < this.Keys.Count; i++)
                {
                    if (this.Keys[i].type == "A")
                        csql.Append("x." + this.Keys[i].target + "= ''' || " + this.Keys[i].source + " || '''");
                    else
                        csql.Append("x." + this.Keys[i].target + "=' || " + this.Keys[i].source + " || '");

                    if (i < this.Keys.Count - 1)
                        csql.Append(" and ");
                }
                csql.Append(")' ");
            }
            else
                csql.Append("' || ' + ''''''");

            csql.Append(" as Conteudo ");
            csql.Append("from " + this.dataSource + " ");

            if ((this.dataSourceCondition != null ? this.dataSourceCondition : "") != "" || this.ConditionsDataSourceInsert != "")
            {
                csql.Append("where ");

                if (this.dataSourceCondition != "")
                {
                    csql.Append(this.dataSourceCondition);

                    if (this.ConditionsDataSourceInsert != "")
                        csql.Append(" and ");
                }

                if (this.ConditionsDataSourceInsert != "")
                    csql.Append(this.ConditionsDataSourceInsert);
            }

            string Operador_Concatenacao;
            string Se_Null;
            if (this.typeTargetDatabase == TypeDatabase.Oracle)
            {
                Operador_Concatenacao = "||";
                Se_Null = "NVL";
            }
            else
            {
                Operador_Concatenacao = "+";
                Se_Null = "ISNULL";
            }

            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        } 

        public string GetSqlUpdate()
        {
            StringBuilder csql = new StringBuilder();
            string Tem_Virgula = "";
            string Tem_Or = "";
            string Tem_Apostrofo = "";

            if (this.typeTargetDatabase != TypeDatabase.Oracle)
                csql.Append("select CAST('update " + this.TargetTable + " set ");
            else
                csql.Append("select 'update " + this.TargetTable + " set ");

            for (int i = 0; i < this.fields.Count; i++)
            {
                if (!this.fields[i].key)
                {
                    Tem_Virgula = ((i != this.fields.Count - 1) ? "," : "");
                    Tem_Apostrofo = (this.fields[i].type == "A" ? "''" : "");

                    if (this.fields[i].type == "A")
                        csql.Append(this.fields[i].target + "=" + Tem_Apostrofo + "' || NVL(" + this.fields[i].source + ",'') || '" + Tem_Apostrofo + Tem_Virgula);
                    else
                        csql.Append(this.fields[i].target + "=" + Tem_Apostrofo + "' || cast(NVL(" + this.fields[i].source + ",0) as varchar(20)) || '" + Tem_Apostrofo + Tem_Virgula);
                }
            }

            if (this.typeTargetDatabase != TypeDatabase.Oracle)
                csql.Append("' AS VARCHAR(MAX)) || ' where ");
            else
                csql.Append(" where ");

            for (int i = 0; i < this.Keys.Count; i++)
            {
                Tem_Virgula = ((i != this.Keys.Count - 1) ? "," : "");
                Tem_Apostrofo = (this.fields[i].type == "A" ? "''" : "");

                csql.Append(this.Keys[i].target + "=" + Tem_Apostrofo + "' || " + this.Keys[i].source + " || '" + Tem_Apostrofo + " and ");
            }
            csql.Append("(");
            for (int i = 0; i < this.fields.Count; i++)
            {
                if (!this.fields[i].key)
                {
                    Tem_Or = ((i != this.fields.Count - 1) ? " or " : "");
                    Tem_Apostrofo = (this.fields[i].type == "A" ? "''" : "");

                    if (this.fields[i].type == "A")
                        csql.Append(this.fields[i].target + "<>" + Tem_Apostrofo + "' || NVL(" + this.fields[i].source + ",'') || '" + Tem_Apostrofo + Tem_Or);
                    else
                        csql.Append(this.fields[i].target + "<>" + Tem_Apostrofo + "' || cast(NVL(" + this.fields[i].source + ",0) as varchar(20)) || '" + Tem_Apostrofo + Tem_Or);
                }
            }
            csql.Append(")' as Conteudo ");

            csql.Append("from " + this.dataSource + " ");

            if ((this.dataSourceCondition != null ? this.dataSourceCondition : "") != "" || this.ConditionsDataSourceUpdate != "")
            {
                csql.Append("where ");

                if (this.dataSourceCondition != "")
                {
                    csql.Append(this.dataSourceCondition);

                    if (this.ConditionsDataSourceUpdate != "")
                        csql.Append(" and ");
                }

                if (this.ConditionsDataSourceUpdate != "")
                    csql.Append(this.ConditionsDataSourceUpdate);
            }
            string Operador_Concatenacao = "||";
            string Se_Null = "NVL";

            if (this.typeTargetDatabase != TypeDatabase.Oracle)
            {
                Operador_Concatenacao = "+";
                Se_Null = "ISNULL";
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        } 

        public string getSqlDelete() 
        {
            StringBuilder ConditionsKeys = new StringBuilder();
            foreach (var item in this.Keys)
            {
                ConditionsKeys.Append($"{this.TargetTable}_Keys.{item.target} = { this.TargetTable}.{item.target} and ");
            }
            int index = ConditionsKeys.ToString().LastIndexOf("and");
            if (index >= 0)
                ConditionsKeys.Remove(index - 1, 5);

            return $"delete {this.TargetTable} where not exists (select 1 from {this.TargetTable}_Keys where {ConditionsKeys})";
        }

        public string GetSqlTruncate() 
        {
            return $"truncate table {this.TargetTable}";
        }

        public string getSqlInsertKeys() 
        {
            StringBuilder keyFieldsName = new StringBuilder();
            foreach (var item in this.Keys)
            {
                keyFieldsName.Append(item.target + ", ");
            }
            var index = keyFieldsName.ToString().LastIndexOf(',');
            if (index >= 0)
                keyFieldsName.Remove(index, 1);


            StringBuilder keyFields = new StringBuilder();
            foreach (var item in this.Keys)
            {
                if (item.type == "A")
                    keyFields.Append($"'''' || {item.source} ||'''' as {item.target}, ");
                else
                    keyFields.Append($"{item.source} as {item.target}, ");
            }
            index = keyFields.ToString().LastIndexOf(',');
            if (index >= 0)
                keyFields.Remove(index, 1);


            string sqlValueFields = "";
            if (this.dataSourceCondition != "")
                sqlValueFields = $"select {keyFields} from {this.dataSource} where {this.dataSourceCondition}";
            else
                sqlValueFields = $"select {keyFields} from {this.dataSource}";

            OracleDataReader OracleDataReader = this.bptConnection.Get_DataReader(sqlValueFields);

            StringBuilder valueFields = new StringBuilder();
            while (OracleDataReader.Read())
            {
                valueFields.Append("select ");

                foreach (var item in this.Keys)
                    valueFields.Append(OracleDataReader[item.target].ToString() + ",");

                index = valueFields.ToString().LastIndexOf(',');
                if (index >= 0)
                    valueFields.Remove(index, 1);

                valueFields.Append(" union all ");
            }
            index = valueFields.ToString().LastIndexOf("union all");
            if (index >= 0)
                valueFields.Remove(index, 10);

            string sql = $"insert into {this.TargetTable}_Keys ({keyFieldsName}) {valueFields}";
            return sql;
        }

        public string GetInsert()
        {
            StringBuilder csql = new StringBuilder();
            csql.Append("select ");
            csql.Append("'insert into " + this.TargetTable + " (");

            string Tem_Virgula = "";

            for (int i = 0; i < this.fields.Count; i++)
            {
                Tem_Virgula = ((i != this.fields.Count - 1) ? "," : "");
                csql.Append(this.fields[i].target + Tem_Virgula);
            }
            csql.Append(") select ' || ");

            for (int i = 0; i < this.fields.Count; i++)
            {
                Tem_Virgula = ((i != this.fields.Count - 1) ? "," : "");
                if (this.fields[i].type == "A")
                    csql.Append("'''' || NVL(" + this.fields[i].source + ",'') || '''" + Tem_Virgula + "' || ");
                else
                    csql.Append(" cast(NVL(" + this.fields[i].source + ",0) as varchar(30)) || '" + Tem_Virgula + "' || ");
                //csql.Append(" || cast(NVL(" + this.fields[i].source + ",0) as varchar(30)) || '" + Tem_Virgula + "' || ");
            }

            csql.Append("' where not exists (select 1 from " + this.TargetTable + " x where ");

            csql.Append(")' as Conteudo ");

            csql.Append("from " + this.dataSource + " ");

            if (this.dataSourceCondition != "" || this.ConditionsDataSourceInsert != "")
            {
                csql.Append("where ");

                if (this.dataSourceCondition != "")
                {
                    csql.Append(this.dataSourceCondition);

                    if (this.ConditionsDataSourceInsert != "")
                        csql.Append(" and ");
                }

                if (this.ConditionsDataSourceInsert != "")
                    csql.Append(this.ConditionsDataSourceInsert);
            }
            string Operador_Concatenacao = "||";
            string Se_Null = "NVL";

            if (this.typeTargetDatabase != TypeDatabase.Oracle)
            {
                Operador_Concatenacao = "+";
                Se_Null = "ISNULL";
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("||", Operador_Concatenacao).Replace("NVL", Se_Null);
        } 

        public string GetUpdateNull() 
        {
            StringBuilder csql = new StringBuilder();
            for (int i = 0; i < this.fields.Count; i++)
            {
                if (this.fields[i].type == "A")
                    csql.Append("update " + this.TargetTable + " set " + this.fields[i].target + " = '' where " + this.fields[i].target + " is null;");
                else
                    csql.Append("update " + this.TargetTable + " set " + this.fields[i].target + " = 0 where " + this.fields[i].target + " is null;");
            }
            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " "); ;
        }

        public string GetUpdate() 
        {
            StringBuilder csql = new StringBuilder();
            csql.Append("select ");
            csql.Append("to_clob('update " + this.TargetTable + " set ");

            string Tem_Virgula = "";

            csql.Append(" from (select ') || ");

            for (int i = 0; i < this.fields.Count; i++)
            {
                Tem_Virgula = ((i != this.fields.Count - 1) ? "," : "");
                if (this.fields[i].type == "A")
                    csql.Append("'''' || " + this.fields[i].source + " || ''' _" + this.fields[i].target + Tem_Virgula + "' || ");
                else
                    csql.Append(" cast(NVL(" + this.fields[i].source + " || ',0) as varchar) _" + this.fields[i].target + Tem_Virgula + "' || ");

            }
            csql.Append("') as Aux ");
            csql.Append("where ");

            string Tem_Or = "";
            for (int i = 0; i < this.fields.Count; i++)
            {
                bool EChave = false;

                if (!EChave)
                {
                    csql.Append("'");
                    Tem_Or = ((i != this.fields.Count - 1) ? " or " : "");
                    csql.Append(this.fields[i].target + "<>_" + this.fields[i].target + " or ");
                    csql.Append("(" + this.fields[i].target + " is null and _" + this.fields[i].target + " is not null)" + " or ");
                    csql.Append("(" + this.fields[i].target + " is not null and _" + this.fields[i].target + " is null)" + Tem_Or);
                    csql.Append("' || ");
                }
            }
            csql.Append(" ')' as Conteudo ");

            csql.Append("from " + this.dataSource + " ");

            if (this.ConditionsDataSourceUpdate != "")
                csql.Append("where " + this.ConditionsDataSourceUpdate);

            return csql.ToString().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\"", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
        }


        public virtual int GetCountRowsTarget { get; } // DIFERENTE

        public virtual string LastInsertKey { get; } // DIFERENTE

        public virtual string LastUpdate { get; }

        public virtual string ConditionsDataSourceInsert { get; } // DIFERENTE

        public virtual string ConditionsDataSourceUpdate { get; set; } // DIFERENTE

        public virtual string getSqlDeleteKeys() // DIFERENTE
        {
            return "";
        }
    }
}
