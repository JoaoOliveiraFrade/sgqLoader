using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptTests : Bpt
    {
        public BptTests(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = 
                $@"(select
                        al_item_id, sys_connect_by_path(al_description, ' \ ') PTH
                    from
                        {SqlMaker.BptProject.Esquema}.all_lists connect by prior al_item_id = al_father_id
                        start with al_father_id = 0 and al_description = 'Subject'
                    ) x
                    inner join {SqlMaker.BptProject.Esquema}.test t 
                    on t.ts_subject = x.al_item_id";

            this.SqlMaker.dataSourceFieldId = "ts_test_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "ts_vts";
            this.SqlMaker.dataSourceCondition = "";

            this.SqlMaker.TargetTable = "BPT_Tests";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "ts_test_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(replace(trim(ts_name),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Descricao", source = "upper(replace(trim(ts_description),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tipo", source = "upper(ts_type)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Qt_Steps", source = "ts_steps" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Path", source = "replace(substr(PTH,14),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status", source = "upper(replace(ts_status,'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status_Execucao", source = "upper(replace(ts_exec_status,'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Resposavel", source = "upper(ts_responsible)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Criacao", source = "to_char(ts_creation_Date,'dd-mm-yy')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Alteracao", source = "substr(ts_vts,9,2) || '-' || substr(ts_vts,6,2) || '-' || substr(ts_vts,3,2) || ' ' || substr(ts_vts,12,8)" });
        }
    }
}

