using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptResources : Bpt
    {
        public BptResources(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.resources";

            this.SqlMaker.dataSourceFieldId = "rsc_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "rsc_vts";
            this.SqlMaker.dataSourceCondition = "";

            this.SqlMaker.TargetTable = "BPT_Resources";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "rsc_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(replace((rsc_name),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome_Arquivo", source = "upper(replace((rsc_file_name),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tipo_Localizacao", source = "upper(replace((rsc_location_type),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tipo", source = "upper(replace((rsc_type),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Descricao", source = "upper(replace((rsc_description),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Parent_Id", source = "rsc_parent_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status", source = "upper(replace((rsc_vc_status),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Usuario_Checkout", source = "upper(replace((rsc_vc_checkin_user_name),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Criacao", source = "to_char(rsc_creation_date,'dd-mm-yy')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Alteracao", source = "substr(rsc_vts,9,2) || '-' || substr(rsc_vts,6,2) || '-' || substr(rsc_vts,3,2) || ' ' || substr(rsc_vts,12,8)" });
        }
    }
}
