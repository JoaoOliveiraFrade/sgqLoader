using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.bpt
{
    public class BptTestsCycle : Bpt
    {
        public BptTestsCycle(SqlMakerFurther sqlMaker)
        {
            if (sqlMaker != null)
                this.SqlMaker = sqlMaker;
            else
                throw new ArgumentNullException("sqlMaker", "O parâmetro 'sqlMaker' não pode ser null");

            this.SqlMaker.dataSource =
                    $@"{SqlMaker.BptProject.Esquema}.testcycl tc
                        JOIN {SqlMaker.BptProject.Esquema}.cycle ts
                        ON ts.cy_cycle_id = tc.tc_cycle_id
                        left JOIN {SqlMaker.BptProject.Esquema}.cycl_fold tsfolder
                        ON tsfolder.cf_item_id = ts.cy_folder_id";

            this.SqlMaker.dataSourceFieldId = "tc_testcycl_id";
            this.SqlMaker.dataSourceFieldDateUpdade = "tc_vts";
            this.SqlMaker.dataSourceCondition = "";
            this.SqlMaker.TargetTable = "BPT_Tests_Cycle";

            this.SqlMaker.fields = new List<Field>();
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Subprojeto", source = $"'{SqlMaker.BptProject.Subprojeto}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "A", target = "Entrega", source = $"'{SqlMaker.BptProject.Entrega}'" });
            this.SqlMaker.fields.Add(new Field() { key = true, type = "N", target = "Id", source = "tc.tc_testcycl_id" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Config_Id", source = "tc.tc_test_Config_id" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Nome", source = "upper(substr((select t.ts_name from {SqlMaker.BptProject.Esquema}.test t where t.ts_test_id=tc.tc_test_id),0,199))" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Test_Id", source = "tc.tc_test_id" });

            this.SqlMaker.fields.Add(new Field()
            {
                target = "Path",
                source = @"
                    substr(
                        (select tablepath.pth from
                        (select in_cf.cf_item_id, sys_connect_by_path (in_cf.CF_ITEM_NAME, ' \ ') pth 
                            from {SqlMaker.BptProject.Esquema}.cycl_fold in_cf connect by prior in_cf.cf_item_id = in_cf.cf_father_id
                            start with in_cf.cf_father_id = 0
                        ) tablepath

                        left join {SqlMaker.BptProject.Esquema}.cycle in_cy 
                            on (in_cy.cy_folder_id = tablepath.cf_item_id)

                        where in_cy.cy_cycle_id = ts.cy_cycle_id

                        ),4)
                    "
            });

            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Release", source = "replace(upper((select r.rel_name from {SqlMaker.BptProject.Esquema}.release_cycles rc, {SqlMaker.BptProject.Esquema}.releases r where rc.rcyc_id = tc.tc_assign_rcyc and r.rel_id = rc.rcyc_parent_id)),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Ciclo", source = "upper((select distinct rc.rcyc_name from {SqlMaker.BptProject.Esquema}.release_cycles rc where rc.rcyc_id=tc.tc_assign_rcyc))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Tipo", source = "upper(tc.tc_subtype_id)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Iterations", source = "tc.tc_iterations" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Instanciador", source = "tc.tc_tester_name" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Testador", source = "(case when tc.tc_actual_tester is not null or tc.tc_actual_tester <> '' then tc.tc_actual_tester else tc.tc_tester_name end)" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Planejamento", source = "to_char(tc.tc_plan_scheduling_date,'dd-mm-yy')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Execucao", source = "case when tc.tc_exec_date is not null then to_char(tc.tc_exec_date,'dd-mm-yy') || ' ' || tc.tc_exec_time else '' end" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Status_Execucao", source = "upper(tc.tc_status)" });

            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Causa_Blocked", source = "replace(upper(tc.tc_user_template_06),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Detalhe_Blocked", source = "replace(upper(tc.tc_user_template_07),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Motivo_Cancelamento_CT", source = "replace(upper(tc.tc_user_template_23),'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Nro_Cenario", source = "replace(tc.tc_user_template_05,'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "N", target = "Previcao_Desbloqueio", source = "replace(tc.tc_user_template_03,'''','')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Priorizacao", source = "upper(replace(tc_user_template_24,'''',''))" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Variante", source = "replace(upper(tc.tc_user_template_31),'''','')" });

            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Criacao", source = "to_char((select min(l.au_time) from {SqlMaker.BptProject.Esquema}.audit_log l where l.au_entity_type = 'testcycl' group by TO_NUMBER(l.au_entity_id) having TO_NUMBER(l.au_entity_id) = TO_NUMBER(tc.tc_testcycl_id)),'dd-mm-yy hh:mm:ss')" });
            this.SqlMaker.fields.Add(new Field() { type = "A", target = "Dt_Alteracao", source = "substr(tc.tc_vts,9,2) || '-' || substr(tc.tc_vts,6,2) || '-' || substr(tc.tc_vts,3,2) || ' ' || substr(tc.tc_vts,12,8)" });
        }
    }
}

