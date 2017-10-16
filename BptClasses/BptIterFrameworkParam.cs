using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptFrameworkParam : Bpt
    {
        public BptFrameworkParam(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.framework_param";

            this.SqlMaker.dataSourceFieldId = "fp_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Framework_Param";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "fp_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Component_Id", source = "fp_component_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(replace((fp_name),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Descricao", source = "upper(replace((fp_desc),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Valor", source = "upper(replace((fp_value),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Saida", source = "upper(replace((fp_is_out),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Escopo", source = "upper(replace((fp_scope),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Data_Center_List_Id", source = "fp_datacenter_list_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Ordem", source = "fp_order" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tipo", source = "upper(replace((fp_value_type),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Contator_Referencia", source = "fp_ref_count" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "BPTA_Valor", source = "upper(replace((fp_bpta_long_value),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Source_Param_Id", source = "fp_source_param_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Meta_Data", source = "upper(replace((fp_metadata),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Usuario_Checkout", source = "upper(replace((fp_vc_checkout_user_name),'''',''))" });
        }
    }
}
