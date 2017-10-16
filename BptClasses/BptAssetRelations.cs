using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptAssetRelations : Bpt
    {
        public BptAssetRelations(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.asset_relations";

            this.SqlMaker.dataSourceFieldId = "fp_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Asset_Relations";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "asr_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Owner_Id", source = "asr_owner_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Owner_type", source = "upper(replace((asr_owner_type),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Related_Id", source = "asr_related_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Related_type", source = "upper(replace((asr_related_type),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Ordem", source = "asr_order" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Condicao", source = "upper(replace((asr_condition),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Mapeamento_Dados", source = "upper(replace((asr_data_mapping),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Usuario_Checkout", source = "upper(replace((asr_vc_checkout_user_name),'''',''))" });
        }
    }
}
