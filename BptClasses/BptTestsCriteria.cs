using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptTestsCriteria : Bpt
    {
        public BptTestsCriteria(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.test_criteria";
            this.SqlMaker.dataSourceFieldId = "tcr_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";

            this.SqlMaker.dataSourceCondition =
                    $@"exists(select distinct 1 
                        from {SqlMaker.BptProject.Esquema}.testcycl tc 
                        where tc.tc_test_id = tcr_test_id)";

            this.SqlMaker.TargetTable = "BPT_Tests_Criteria";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "tcr_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(tcr_name)" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Id", source = "tcr_test_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status", source = "upper(tcr_execution_status)" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Main", source = "tcr_is_main_criterion" });
            this.SqlMaker.fields.Add(new Field() { target = "Dt_Criacao", source = "to_char(tcr_creation_Date,'dd-mm-yy')" });
        }
    }
}

