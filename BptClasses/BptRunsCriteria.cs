using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptRunsCriteria : Bpt
    {
        public BptRunsCriteria(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.run_criteria";

            this.SqlMaker.dataSourceFieldId = "rcr_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";
            this.SqlMaker.dataSourceCondition = "";

            this.SqlMaker.TargetTable = "BPT_Runs_Criteria";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "rcr_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(replace(rcr_name,'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Run_Id", source = "rcr_run_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Criteria_Id", source = "rcr_criterion_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Config_Id", source = "rcr_configuration_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status_Execucao", source = "upper(rcr_status)" });
        }
    }
}

