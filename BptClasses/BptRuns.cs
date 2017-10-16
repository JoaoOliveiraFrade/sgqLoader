using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptRuns : Bpt
    {
        public BptRuns(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.run";

            this.SqlMaker.dataSourceFieldId = "rn_run_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "rn_vts";
            this.SqlMaker.dataSourceCondition =
                    $@"exists(select distinct 1 
                        from {SqlMaker.BptProject.Esquema}.testcycl tc 
                        where rn_test_id = rn_test_id)";

            this.SqlMaker.TargetTable = "BPT_Runs";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "rn_run_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(replace(rn_run_name,'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Id", source = "rn_test_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Config_Id", source = "rn_test_config_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Cycle_Id", source = "rn_testcycl_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Execucao", source = "to_char(rn_execution_date, 'dd-mm-yy') || ' ' || rn_execution_time" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Duracao", source = "rn_duration" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status", source = "upper(replace((rn_status),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Testador", source = "upper(replace((rn_tester_name),'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tem_Anexo", source = "upper(rn_attachment)" });

            //this.SqlMaker.fields.Add(new Field() { target = "Evidencia_Comentario_Tecnica", source = "replace(upper(rn_user_template_05),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { target = "Evidencia_Comentario_Cliente", source = "replace(upper(rn_user_template_10),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { target = "Evidencia_Motivo_Rejeicao_Tecnica", source = "replace(upper(rn_user_template_04),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { target = "Evidencia_Motivo_Rejeicao_Cliente", source = "replace(upper(rn_user_template_08),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { target = "Evidencia_Aprovador_Tecnico", source = "upper(rn_user_template_06)" });
            //this.SqlMaker.fields.Add(new Field() { target = "Evidencia_Aprovador_Cliente", source = "upper(rn_user_template_11)" });
            //this.SqlMaker.fields.Add(new Field() { target = "Evidencia_Validacao_Tecnica", source = "replace(upper(rn_user_template_03),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { target = "Evidencia_Validacao_Cliente", source = "replace(upper(rn_user_template_07),'''','')" });
            //this.SqlMaker.fields.Add(new Field() { target = "Execucao_Automatica", source = "upper(replace(rn_user_template_01,'''',''))" });
            //this.SqlMaker.fields.Add(new Field() { target = "Motivo_Execucao_Manual", source = "upper(replace(rn_user_template_02,'''',''))" });

            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Alteracao", source = "substr(rn_vts,9,2) || '-' || substr(rn_vts,6,2) || '-' || substr(rn_vts,3,2) || ' ' || substr(rn_vts,12,8)" });
        }
    }
}

