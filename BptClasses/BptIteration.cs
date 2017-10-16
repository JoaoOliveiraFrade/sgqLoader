using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptIteration : Bpt
    {
        public BptIteration(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.bp_iteration";

            this.SqlMaker.dataSourceFieldId = "bpi_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Iteration";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "bpi_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "BPC_Id", source = "bpi_bpc_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Ordem", source = "bpi_order" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Usuario_Checkout", source = "upper(replace((bpi_vc_checkout_user_name),'''',''))" });
        }
    }
}

