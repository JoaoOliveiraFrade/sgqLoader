using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptComponents : Bpt
    {
        public BptComponents(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.component";
            this.SqlMaker.dataSourceFieldId = "co_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "co_vts";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Components";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "co_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(co_name)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status", source = "upper(co_status)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tipo", source = "upper(co_script_type)" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Folder_Id", source = "co_folder_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Path_Id", source = "co_physical_path" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Src_Id", source = "co_src_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Complexidade_Acc", source = "replace(upper(CO_USER_TEMPLATE_03),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Complexidade_Link", source = "replace(upper(CO_USER_TEMPLATE_04),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Componente_Automatizado", source = "replace(upper(CO_USER_TEMPLATE_09),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Fabrica_Desenvolvimento", source = "replace(upper(CO_USER_TEMPLATE_08),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Fabrica_Teste", source = "replace(upper(CO_USER_TEMPLATE_07),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Fornecedor", source = "replace(upper(CO_USER_TEMPLATE_02),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Geracao_Massa", source = "replace(upper(CO_USER_TEMPLATE_06),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Motivo_Exec_Manual", source = "replace(upper(CO_USER_TEMPLATE_11),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tipo_Execucao", source = "replace(upper(CO_USER_TEMPLATE_10),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Sistema", source = "replace(upper(CO_USER_TEMPLATE_01),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "UAT", source = "replace(upper(CO_USER_TEMPLATE_05),'''','')" });

            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Plano_Validacao_Tecnica", source = "replace(upper(co_user_template_08),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Plano_Motivo_Rejeicao_Tecnica", source = "replace(upper(co_user_template_09),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Complexidade_Acc", source = "replace(upper(co_user_template_01),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Complexidade_Link", source = "replace(upper(co_user_template_02),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Componente_Automatizado", source = "replace(upper(co_user_template_06),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Fornecedor", source = "replace(upper(co_user_template_03),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Geracao_Massa", source = "replace(upper(co_user_template_05),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Projeto", source = "replace(upper(co_user_template_07),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Sistema", source = "replace(upper(co_user_template_04),'''','')" });

            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Criacao", source = "to_char(co_creation_Date,'dd-mm-yy')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Alteracao", source = "substr(co_vts,9,2) || '-' || substr(co_vts,6,2) || '-' || substr(co_vts,3,2) || ' ' || substr(co_vts,12,8)" });
        }
    }
}

