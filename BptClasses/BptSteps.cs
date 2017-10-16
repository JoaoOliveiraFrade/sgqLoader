using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptSteps : Bpt
    {
        public BptSteps(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.step";
            this.SqlMaker.dataSourceFieldId = "st_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Steps";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "st_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Run_Id", source = "st_run_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Teste_Id", source = "st_test_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(replace(trim(st_step_name),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status", source = "upper(replace(trim(st_status),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Execucao", source = "to_char(st_execution_date,'dd-mm-yy')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Hora_Execucao", source = "st_execution_time" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Descricao", source = "replace(trim(st_description),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Resultado_Esperado", source = "replace(trim(st_expected),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Ordem", source = "st_step_order" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Nivel_Id", source = "ST_OBJ_ID" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Nivel", source = "ST_LEVEL" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Id_Step_Pai", source = "ST_PARENT_ID" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Id_Objeto_Relacionado", source = "ST_REL_OBJ_ID" });
        }
    }
}
