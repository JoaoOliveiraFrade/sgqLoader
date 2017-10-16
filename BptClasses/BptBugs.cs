using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptBugs : Bpt
    {
        public BptBugs(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource = $"{SqlMaker.BptProject.Esquema}.Bug";
            this.SqlMaker.dataSourceFieldId = "bg_bug_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "bg_vts";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Bugs";

            //bg_dev_comments, Comentarios
            //bg_description, Descricao
            //bg_detected_in_rel, Detectado Na Release
            //bg_detection_date Detectado No Dia
            //bg_detection_version, Detected In Version
            //bg_responsible, Enviado Para
            //bg_estimated_fix_time, Estimated Fix Time
            //bg_planned_closing_ver, Planned Closing Version
            //bg_project, Project
            //bg_reproducible, Reproducible
            //bg_subject, Subject
            //bg_target_rcyc, Target Cycle
            //bg_target_rel, Target Release

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "bg_bug_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(replace(bg_summary,'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Detectado_Por", source = "bg_detected_by" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Release", source = $"replace(upper((select r.rel_name from {SqlMaker.BptProject.Esquema}.release_cycles rc, {SqlMaker.BptProject.Esquema}.releases r where rc.rcyc_id = bg_detected_in_rcyc and r.rel_id = rc.rcyc_parent_id)),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Ciclo", source = $"upper((select rc.rcyc_name from {SqlMaker.BptProject.Esquema}.release_cycles rc where rc.rcyc_id = bg_detected_in_rcyc))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Prioridade", source = "upper(bg_priority)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Severidade", source = "upper(bg_severity)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Inicial", source = "to_char(BG_DETECTION_DATE, 'dd-mm-yy')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Final", source = "(case when BG_CLOSING_DATE is null then '' else to_char(BG_CLOSING_DATE,'dd-mm-yy') || ' ' || substr(bg_vts,12,8) end)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Prevista_Solucao_Defeito", source = "(case when bg_user_template_04 is not null then substr(bg_user_template_04, 9, 2) || '-' || substr(bg_user_template_04, 6, 2) || '-' || substr(bg_user_template_04, 3, 2) || ' ' || substr(bg_user_template_04, 12, 8) else '' end)" });
            //this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Ultimo_Status", source = $"to_char(Dt_Ultimo_Status_Bug('{SqlMaker.BptProject.Esquema}', bg_bug_id, bg_status),'dd-mm-yy hh24:mi:ss')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status", source = "upper(bg_status)" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Tempo_Resolucao_Dias", source = "bg_actual_fix_time" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Detalhamento_CR_PKE", source = "bg_user_template_13" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Encaminhado_Para", source = "upper(bg_user_template_09)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Erro_Detectavel_Em_Desenvolvimento", source = "replace(upper(bg_user_template_06),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Ja_Foi_Rejeitado", source = "upper(bg_user_template_08)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Motivo_Pendencia", source = "upper(bg_user_template_10)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Natureza", source = "upper(bg_user_template_01)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Origem", source = "upper(bg_user_template_05)" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Qtd_CTs_Impactados", source = "bg_user_template_11" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Qtd_Reincidencia", source = "bg_user_template_07" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Sistema_CT", source = "upper(bg_user_template_03)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Sistema_Defeito", source = "upper(bg_user_template_02)" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Cycle_Id", source = $"(select (case when ln_entity_type = 'TESTCYCL' then (select TC.tc_testcycl_id from {SqlMaker.BptProject.Esquema}.testcycl TC where TC.tc_testcycl_id = L.ln_entity_id) when ln_entity_type = 'RUN' then (select R.rn_testcycl_id from {SqlMaker.BptProject.Esquema}.run R where R.rn_run_id = L.ln_entity_id) when ln_entity_type = 'STEP' then (select R.rn_testcycl_id from {SqlMaker.BptProject.Esquema}.run R where R.rn_run_id = (select S.st_run_id from {SqlMaker.BptProject.Esquema}.step S where S.st_id = L.ln_entity_id)) end) from {SqlMaker.BptProject.Esquema}.Link L where L.ln_bug_id = bg_bug_id and rownum=1)" });
            //this.SqlMaker.fields.Add(new Field() { type = "n", target = "SLA", source = "SLA(bg_severity)" });
            this.SqlMaker.fields.Add(new Field() { type = "n", target = "Qtd_Reopen", source = "0" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Alteracao", source = "substr(bg_vts,9,2) || '-' || substr(bg_vts,6,2) || '-' || substr(bg_vts,3,2) || ' ' || substr(bg_vts,12,8)" });
        }
    }
}

