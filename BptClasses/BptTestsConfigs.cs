using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptTestsConfigs : Bpt
    {
        public BptTestsConfigs(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.test_configs";
            this.SqlMaker.dataSourceFieldId = "tsc_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "tsc_vts";

            this.SqlMaker.dataSourceCondition =
                $@"exists(select distinct 1 
                    from {SqlMaker.BptProject.Esquema}.testcycl tc 
                    where tc.tc_test_id = tsc_test_id)";

            this.SqlMaker.TargetTable = "BPT_Tests_Configs";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "tsc_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(tsc_name)" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Id", source = "tsc_test_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status_Execucao", source = "upper(tsc_exec_status)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Criacao", source = "to_char(tsc_creation_date,'dd-mm-yy')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Alteracao", source = "substr(tsc_vts,9,2) || '-' || substr(tsc_vts,6,2) || '-' || substr(tsc_vts,3,2) || ' ' || substr(tsc_vts,12,8)" });
        }
    }
}

