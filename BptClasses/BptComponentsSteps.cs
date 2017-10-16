using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptComponentsSteps : Bpt
    {
        public BptComponentsSteps(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.component_step";
            this.SqlMaker.dataSourceFieldId = "cs_step_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Components_Steps";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "cs_step_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(replace(trim(cs_step_name),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Component_Id", source = "cs_component_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Ordem", source = "cs_step_order" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Descricao", source = "replace(trim(cs_description),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Resultado_Esperado", source = "replace(trim(cs_expected),'''','')" });
        }
    }
}

