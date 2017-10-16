using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptDesSteps : Bpt
    {
        public BptDesSteps(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.dessteps";

            this.SqlMaker.dataSourceFieldId = "ds_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "ds_vts";

            this.SqlMaker.dataSourceCondition = 
                $@"exists(select distinct 1 
                            from {SqlMaker.BptProject.Esquema}.testcycl tc 
                            where tc.tc_test_id = ds_test_id)";

            this.SqlMaker.TargetTable = "BPT_Des_Steps";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = "'{Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = "'{Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "ds_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(replace(trim(ds_step_name),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Id", source = "ds_test_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Ordem", source = "ds_step_order" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Descricao", source = "replace(trim(DS_DESCRIPTION),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Resultado_Esperado", source = "replace(trim(DS_expected),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tem_Paramentro", source = "upper(ds_has_params)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Alteracao", source = "substr(ds_vts,9,2) || '-' || substr(ds_vts,6,2) || '-' || substr(ds_vts,3,2) || ' ' || substr(ds_vts,12,8)" });
        }
    }
}

