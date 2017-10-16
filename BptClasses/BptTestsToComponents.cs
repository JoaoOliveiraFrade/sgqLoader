using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptTestsToComponents : Bpt
    {
        public BptTestsToComponents(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.bptest_to_components";
            this.SqlMaker.dataSourceFieldId = "bc_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";

            this.SqlMaker.dataSourceCondition = 
                $@"exists(select distinct 1 
                            from {SqlMaker.BptProject.Esquema}.testcycl tc 
                            where tc.tc_test_id = bc_bpt_id)";

            this.SqlMaker.TargetTable = "BPT_Tests_To_Components";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "bc_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Ordem", source = "bc_order" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Id", source = "bc_bpt_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Component_Id", source = "bc_co_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Criteria_Id", source = "bc_criterion_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Condicao_Falha", source = "upper(bc_fail_cond)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Pai", source = "upper(bc_Parent_type)" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Pai_Id", source = "bc_Parent_id" });
        }
    }
}

