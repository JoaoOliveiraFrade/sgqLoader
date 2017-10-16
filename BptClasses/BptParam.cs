using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptIterParam : Bpt
    {
        public BptIterParam(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.bp_iter_param  ";

            this.SqlMaker.dataSourceFieldId = "bpip_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Iter_Param";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "bpip_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "BPP_Id", source = "bpip_bpp_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "BPI_Id", source = "bpip_bpi_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Valor", source = "upper(replace((bpip_value),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Usuario_Checkout", source = "upper(replace((bpip_vc_checkout_user_name),'''',''))" });
        }
    }
}
