using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptParam : Bpt
    {
        public BptParam(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O par�metro 'sqlMaker' n�o pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.bp_param  ";

            this.SqlMaker.dataSourceFieldId = "bpp_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Param";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "bpp_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "BPC_Id", source = "bpp_bpc_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Param_Id", source = "bpp_param_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tipo", source = "upper(replace((bpp_type),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Ref_Id", source = "bpp_ref_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Usuario_Checkout", source = "upper(replace((bpp_vc_checkout_user_name),'''',''))" });
        }
    }
}
