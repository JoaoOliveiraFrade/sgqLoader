using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptLinks : Bpt
    {
        public BptLinks(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.Link";
            this.SqlMaker.dataSourceFieldId = "ln_bug_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "to_char(ln_creation_date,'yy-mm-dd')";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Links";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = "'{Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = "'{Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "ln_link_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Bug_Id", source = "ln_bug_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tabela", source = "ln_entity_type" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Tabela_Id", source = "ln_entity_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Atualizador", source = "upper(ln_created_by)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Alteracao", source = "to_char(ln_creation_date,'dd-mm-yy')" });
        }
    }
}

