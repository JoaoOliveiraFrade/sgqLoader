using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.alm
{
    public class CTs
    {
        public Projeto projeto { get; set; }

        public List<Field> fields { get; set; }

        public TypeUpdate typeUpdate { get; set; }

        public alm.Database database { get; set; }

        public SqlMaker2Param sqlMaker2Param { get; set; }

        public CTs(Projeto projeto, TypeUpdate typeUpdate, alm.Database database) {
            this.projeto = projeto;
            this.typeUpdate = typeUpdate;
            this.database = database;

            sqlMaker2Param = new SqlMaker2Param();

            sqlMaker2Param.fields = new List<Field>();

            sqlMaker2Param.fields.Add(new Field() { target = "Subprojeto", source = "'{Subprojeto}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Entrega", source = "'{Entrega}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "CT", source = "tc.tc_testcycl_id", type = "N", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Teste", source = "tc.tc_test_id", type = "N" });

            sqlMaker2Param.fields.Add(new Field() { target = "Path",
                source = @"
                    substr(
                        (select TABLEPATH.PTH from
                        (select in_cf.CF_ITEM_ID, sys_connect_by_path (in_cf.CF_ITEM_NAME, ' \ ') PTH 
                            from {Esquema}.CYCL_FOLD in_cf connect by prior in_cf.CF_ITEM_ID = in_cf.CF_FATHER_ID
                            start with in_cf.CF_FATHER_ID = 0
                        ) TABLEPATH

                        left join {Esquema}.CYCLE in_cy 
                            on (in_cy.CY_FOLDER_ID = TABLEPATH.CF_ITEM_ID)

                        where in_cy.CY_CYCLE_ID = ts.CY_CYCLE_ID

                        ),4)
                "
            });

            sqlMaker2Param.fields.Add(new Field() { target = "Nome", source = "upper(substr((select t.ts_name from {Esquema}.test t where t.ts_test_id=tc.tc_test_id),0,199))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Tipo", source = "upper((select t.ts_type from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Iterations", source = "tc.tc_iterations" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Base", source = "''" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Planejamento", source = "to_char(tc.tc_plan_scheduling_date,'dd-mm-yy')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Execucao", source = "case when tc.tc_exec_date is not null then to_char(tc.tc_exec_date,'dd-mm-yy') || ' ' || tc.tc_exec_time else '' end" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Deteccao_Primeiro_Anexo", source = "(case when exists (select 1 from {Esquema}.cros_ref where cr_entity = 'TESTCYCL' and cr_ref_type = 'File' and cr_key_1 = tc.tc_testcycl_id) then to_char(sysdate,'dd-mm-yy hh:mm:ss') else '' end)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Release", source = "replace(upper((select r.rel_name from {Esquema}.release_cycles rc, {Esquema}.releases r where rc.rcyc_id = tc.tc_assign_rcyc and r.rel_id = rc.rcyc_parent_id)),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Ciclo", source = "upper((select distinct rc.rcyc_name from {Esquema}.release_cycles rc where rc.rcyc_id=tc.tc_assign_rcyc))" });
            //sqlMaker2Param.fields.Add(new Field() { target = "Plano_Teste", source = "upper(replace((select c.cy_cycle from {Esquema}.cycle c where c.cy_cycle_id = tc.tc_cycle_id),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Plano_Teste", source = "upper(replace(ts.cy_cycle,'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qt_Steps", source = "(select t.ts_steps from {Esquema}.test t where t.ts_test_id=tc.tc_test_id)", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Status_Exec_Teste", source = "upper((select t.ts_exec_Status from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Status_Exec_CT", source = "upper(tc.tc_status)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Instanciador", source = "tc.tc_tester_name" });
            sqlMaker2Param.fields.Add(new Field() { target = "Testador", source = "(case when tc.tc_actual_tester is not null or tc.tc_actual_tester <> '' then tc.tc_actual_tester else tc.tc_tester_name end)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qt_Blocked", source = "(select count(*) from {Esquema}.run where rn_status='Blocked' and run.rn_testcycl_id=tc.tc_testcycl_id)", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qt_Cancelled", source = "(select count(*) from {Esquema}.run where rn_status='Cancelled' and run.rn_testcycl_id=tc.tc_testcycl_id)", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qt_Failed", source = "(select count(*) from {Esquema}.run where rn_status='Failed' and run.rn_testcycl_id=tc.tc_testcycl_id)", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qt_NoRun", source = "(select count(*) from {Esquema}.run where rn_status='No Run' and run.rn_testcycl_id=tc.tc_testcycl_id)", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qt_NotCompleted", source = "(select count(*) from {Esquema}.run where rn_status='Not Completed' and run.rn_testcycl_id=tc.tc_testcycl_id)", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qt_Passed", source = "(select count(*) from {Esquema}.run where rn_status='Passed' and run.rn_testcycl_id=tc.tc_testcycl_id)", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qt_N_Def", source = "(select count(*) from {Esquema}.run where rn_status is null and run.rn_testcycl_id=tc.tc_testcycl_id)", type = "N" });
            //sqlMaker2Param.fields.Add(new Field() { target = "Dt_Criacao", source = "to_char((select min(au_time) from {Esquema}.audit_log where au_entity_type = 'TESTCYCL' and au_entity_id = ''' || tc.tc_testcycl_id || '''),'dd-mm-yy hh:mm:ss')" });
            //sqlMaker2Param.fields.Add(new Field() { target = "Dt_Criacao", source = "to_char((select min(au_time) from {Esquema}.audit_log where au_entity_type = 'TESTCYCL' and TO_NUMBER(au_entity_id) = TO_NUMBER(tc.tc_testcycl_id)),'dd-mm-yy hh:mm:ss')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Criacao", source = "to_char((select min(l.au_time) from {Esquema}.audit_log l where l.au_entity_type = 'TESTCYCL' group by TO_NUMBER(l.au_entity_id) having TO_NUMBER(l.au_entity_id) = TO_NUMBER(tc.tc_testcycl_id)),'dd-mm-yy hh:mm:ss')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Alteracao", source = "substr(tc.tc_vts,9,2) || '-' || substr(tc.tc_vts,6,2) || '-' || substr(tc.tc_vts,3,2) || ' ' || substr(tc.tc_vts,12,8)" });

            sqlMaker2Param.fields.Add(new Field() { target = "UAT", source = "upper(tc.tc_user_template_02)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Previsao_Desbloqueio", source = "to_char(tc_user_template_03,'dd-mm-yy')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Causa_Blocked", source = "replace(upper(tc.tc_user_template_06),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Detalhe_Blocked", source = "replace(upper(tc.tc_user_template_07),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Plano_Validacao_Tecnica", source = "replace(upper(tc.tc_user_template_15),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Plano_Comentario_Tecnica", source = "replace(upper(tc.tc_user_template_19),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Plano_Aprovador_Tecnico", source = "upper(tc.tc_user_template_21)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Detalhamento_Funcional", source = "upper(replace(tc_user_template_25,'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Pre_Requisito", source = "replace(upper(tc.tc_user_template_26),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "CT_Sucessor", source = "upper(replace(tc_user_template_27,'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Execucao_Automatica", source = "upper(replace(tc_user_template_28,'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Variante", source = "replace(upper(tc.tc_user_template_31),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Motivo_Execucao_Manual", source = "upper(replace(tc_user_template_32,'''',''))" });

            if (database.name == "ALM11") {
                sqlMaker2Param.fields.Add(new Field() { target = "Sistema", source = "upper((select t.ts_user_template_01 from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });
                sqlMaker2Param.fields.Add(new Field() { target = "Macrocenario", source = "upper((select t.ts_user_template_02 from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });
                sqlMaker2Param.fields.Add(new Field() { target = "Cenario", source = "upper((select t.ts_user_template_03 from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });
                sqlMaker2Param.fields.Add(new Field() { target = "Fornecedor", source = "upper((select t.ts_user_template_05 from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });
                sqlMaker2Param.fields.Add(new Field() { target = "Complexidade", source = "upper((select t.ts_user_template_24 from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });
                sqlMaker2Param.fields.Add(new Field() { target = "Complexidade_Link", source = "upper((select t.ts_user_template_28 from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });
                //sqlMaker2Param.fields.Add(new Field() { target = "Resp_Execucao", source = "upper((select t.ts_user_template_19 from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });
                sqlMaker2Param.fields.Add(new Field() { target = "Prioridade_Execucao", source = "upper((select t.ts_user_template_20 from {Esquema}.test t where t.ts_test_id=tc.tc_test_id))" });

                sqlMaker2Param.fields.Add(new Field() { target = "Nro_CT", source = "tc.tc_user_template_01", type = "N" });
                sqlMaker2Param.fields.Add(new Field() { target = "Evidencia_Aprovador_Tecnico", source = "upper(tc.tc_user_template_04)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Nro_Cenario", source = "replace(tc.tc_user_template_05,'''','')", type = "N" });
                sqlMaker2Param.fields.Add(new Field() { target = "Evidencia_Validacao_Tecnica", source = "replace(upper(tc.tc_user_template_08),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Evidencia_Validacao_Cliente", source = "replace(upper(tc.tc_user_template_09),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Evidencia_Motivo_Rejeicao_Tecnica", source = "replace(upper(tc.tc_user_template_10),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Evidencia_Motivo_Rejeicao_Cliente", source = "replace(upper(tc.tc_user_template_11),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Evidencia_Comentario_Tecnica", source = "replace(upper(tc.tc_user_template_12),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Evidencia_Comentario_Cliente", source = "replace(upper(tc.tc_user_template_13),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Evidencia_Aprovador_Cliente", source = "upper(tc.tc_user_template_14)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Plano_Validacao_Cliente", source = "replace(upper(tc.tc_user_template_16),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Plano_Motivo_Rejeicao_Tecnica", source = "replace(upper(tc.tc_user_template_17),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Plano_Motivo_Rejeicao_Cliente", source = "replace(upper(tc.tc_user_template_18),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Plano_Comentario_Cliente", source = "replace(upper(tc.tc_user_template_20),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Plano_Aprovador_Cliente", source = "upper(tc.tc_user_template_22)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Motivo_Cancelamento_CT", source = "replace(upper(tc.tc_user_template_23),'''','')" });
                sqlMaker2Param.fields.Add(new Field() { target = "Demanda_Sinergia", source = "upper(replace(tc_user_template_24,'''',''))" });
                sqlMaker2Param.fields.Add(new Field() { target = "Massa_Teste", source = "upper(tc.tc_user_template_29)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Pre_Condicao", source = "replace(upper(tc.tc_user_template_30),'''','')" });
            }

            //sqlMaker2Param.dataSource = @"{Esquema}.testcycl tc";
            sqlMaker2Param.dataSource =
                @"{Esquema}.TESTCYCL tc
                    JOIN {Esquema}.CYCLE ts
                    ON ts.CY_CYCLE_ID = tc.TC_CYCLE_ID
                    JOIN {Esquema}.CYCL_FOLD tsfolder
                    ON tsfolder.CF_ITEM_ID = ts.CY_FOLDER_ID
                ";

            sqlMaker2Param.dataSourceFilterCondition = "";
            sqlMaker2Param.targetTable = "ALM_CTs";

            this.sqlMaker2Param.targetSqlLastIdInserted = $"select max(ct) from alm_cts where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";
            this.sqlMaker2Param.targetSqlLastDateUpdate = $"select CTs_Incremental_Inicio from alm_projetos where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";

            this.sqlMaker2Param.dataSourceFilterConditionInsert = $" tc_testcycl_id > {this.sqlMaker2Param.targetLastIdInserted}";
            this.sqlMaker2Param.dataSourceFilterConditionUpdate = $" substr(tc_vts,3,17) > '{this.sqlMaker2Param.targetLastDateUpdate}'";

            sqlMaker2Param.typeDB = "ORACLE";

            // - Instanciador : tc.tc_tester_name (ALM Label: Responsible Tester)
            // - Testador    :(case when tc.tc_actual_tester <> '' then tc.tc_actual_tester else tc.tc_tester_name end) (ALM Label: Tester)

        }

        public List<Field> keys {
            get {
                var keys = new List<Field>();

                foreach (var field in this.fields) {
                    if (field.key) {
                        keys.Add(field);
                    }
                }

                return keys;
            }
        }

        //public void Carregarkeys()
        //{
        //    sqlMaker2Param.keys = new List<Field>();
        //    for (int i = 0; i < this.sqlMaker2Param.fields.Count; i++)
        //    {
        //        if (sqlMaker2Param.fields[i].key)
        //            sqlMaker2Param.keys.Add(sqlMaker2Param.fields[i]);
        //    }
        //}
        //public void Carregar_Condicoes_Insert()
        //{
        //    if (this.typeUpdate == TypeUpdate.Full)
        //        sqlMaker2Param.dataSourceFilterConditionInsert = "";
        //    else
        //    {
        //        Connection SGQConn = new Connection();
        //        sqlMaker2Param.targetLastIdInserted =
        //            SGQConn.Get_String(string.Format("select max(CT) from ALM_CTs where Subprojeto='{0}' and Entrega='{1}'", projeto.Subprojeto, projeto.Entrega));
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.targetLastIdInserted == "" || sqlMaker2Param.targetLastIdInserted == null)
        //            sqlMaker2Param.targetLastIdInserted = "0";

        //        sqlMaker2Param.dataSourceFilterConditionInsert = " tc.tc_testcycl_id > " + sqlMaker2Param.targetLastIdInserted;
        //    }
        //}
        //public void Carregar_Condicoes_Update()
        //{
        //    if (this.typeUpdate == TypeUpdate.Full)
        //        sqlMaker2Param.dataSourceFilterConditionUpdate = "";
        //    else
        //    {
        //        Connection SGQConn = new Connection();
        //        sqlMaker2Param.Ultima_Atualizacao = SGQConn.Get_String_Por_Id("ALM_Projetos", "CTs_Incremental_Inicio", projeto.Id.ToString());
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.Ultima_Atualizacao == "" || sqlMaker2Param.Ultima_Atualizacao == null)
        //            sqlMaker2Param.Ultima_Atualizacao = "00-00-00 00:00:00";
        //        else
        //            sqlMaker2Param.Ultima_Atualizacao = sqlMaker2Param.Ultima_Atualizacao.Substring(6, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(3, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(0, 2) + " " + sqlMaker2Param.Ultima_Atualizacao.Substring(9, 8);

        //        sqlMaker2Param.dataSourceFilterConditionUpdate = "substr(tc.tc_vts,3,17) > '" + sqlMaker2Param.Ultima_Atualizacao + "'";
        //    }
        //}


        public void LoadData() {
            DateTime Dt_Inicio = DateTime.Now;

            ALMConnection ALMConn = new ALMConnection(this.database);
            Connection SGQConn = new Connection();

            SqlMaker2 sqlMaker2 = new SqlMaker2() { sqlMaker2Param = this.sqlMaker2Param };

            if (typeUpdate == TypeUpdate.Increment || typeUpdate == TypeUpdate.IncrementFullUpdate) {
                if (typeUpdate == TypeUpdate.IncrementFullUpdate) {
                    SGQConn.Executar($@"
                        update 
                            alm_projetos 
                        set CTs_Incremental_Inicio='00-00-00 00:00:00',
                            CTs_Incremental_Fim='00-00-00 00:00:00',
                            CTs_Incremental_Tempo=0
                        where 
                            subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                    ");
                }

                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }

                string Sql_Update = sqlMaker2.Get_Oracle_Update().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                OracleDataReader DataReader_Update = ALMConn.Get_DataReader(Sql_Update);
                if (DataReader_Update != null && DataReader_Update.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Update, 1);
                }

                //this.LoadData_Etapa();
                //this.LoadData_Fabrica_Teste(So_Nulos: true);
                //this.LoadData_Fabrica_Desenvolvimento(So_Nulos: true);

                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set CTs_Incremental_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        CTs_Incremental_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        CTs_Incremental_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");

            } else if (typeUpdate == TypeUpdate.Full) {
                SGQConn.Executar($"delete alm_cts where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'");

                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }

                //this.LoadData_Etapa();
                //this.LoadData_Fabrica_Teste(So_Nulos: false);
                //this.LoadData_Fabrica_Desenvolvimento(So_Nulos: false);

                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set CTs_Completa_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        CTs_Completa_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        CTs_Completa_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");
            }

            SGQConn.Dispose();
        }

        public void LoadData_Etapa()
        {
            String Sql = @"
                update alm_cts 
                set alm_cts.Etapa = Aux.Etapa
                from
                (
	                select
                        cts1.Subprojeto,
                        cts1.Entrega,
                        cts1.CT,
                        case 
                            when cts1.Ciclo like '%TI%' and UAT = 'SIM' then 'TI (UAT)'
	                        when cts1.Ciclo like '%TI%' then 'TI'
	                        when cts1.Ciclo like '%UAT%' then 'UAT-PRESENCIAL'
	                        when cts1.Ciclo like '%TRG%' then 'TRG'
	                        when cts1.Ciclo like '%TS%' then 'TS'
	                        when cts1.Ciclo like '%TP%' then 'TP'
                            when cts1.Plano_Teste like '%TI%' and cts1.UAT = 'SIM' then 'TI (UAT)'
					        when cts1.Plano_Teste like '%TI%' then 'TI'
					        when cts1.Plano_Teste like '%UAT%' then 'UAT-PRESENCIAL'
					        when cts1.Plano_Teste like '%TRG%' then 'TRG'
					        when cts1.Plano_Teste like '%TS%' then 'TS'
					        when cts1.Plano_Teste like '%TP%' then 'TP'
                			when cts1.subprojeto like '%TRG%' then 'TRG'
					        else ''
                        end as Etapa
	                from alm_cts as cts1
                    where cts1.subprojeto = '" + projeto.Subprojeto + "' and cts1.entrega = '" + projeto.Entrega + @"'
                ) Aux	
                where
	                alm_cts.Subprojeto = Aux.Subprojeto and
	                alm_cts.Entrega = Aux.Entrega and
	                alm_cts.CT = Aux.CT and 
	                (alm_cts.Etapa is null or alm_cts.Etapa <> Aux.Etapa)
                ";

            Connection SGQConn = new Connection();
            SGQConn.Executar(Sql);
            SGQConn.Dispose();
        }

        public void LoadData_Fabrica_Teste(bool So_Nulos)
        {
            String Sql =
                @"update ALM_CTs 
                    set Fabrica_Teste = 
	                    case when Testador <> ''
			                    then case when (select fornecedor from ALM_Usuarios where Login = ALM_CTs.Testador) <> '' and
						                       (select fornecedor from ALM_Usuarios where Login = ALM_CTs.Testador) <> 'OI' and
						                       (select fornecedor from ALM_Usuarios where Login = ALM_CTs.Testador) <> '.'
					                    then replace(replace((select fornecedor from ALM_Usuarios where Login = ALM_CTs.Testador),char(10),''),char(13),'')
					                    else ALM_CTs.fornecedor
				                    end
			                    else ALM_CTs.fornecedor
	                    end
                where ALM_CTs.subprojeto = '" + projeto.Subprojeto + "' and ALM_CTs.entrega = '" + projeto.Entrega + "'";

            if (!So_Nulos)
            {
                Sql = Sql + " and fabrica_teste is null";
            }

            Connection SGQConn = new Connection();
            SGQConn.Executar(Sql);
            SGQConn.Executar("update ALM_CTs set Fabrica_Teste = 'LINK CONSULTING' where Fabrica_Teste like '%LINK%'");
            SGQConn.Executar("update ALM_CTs set Fabrica_Teste = 'SONDA' where Fabrica_Teste like '%SONDA%'");
            SGQConn.Executar("update ALM_CTs set Fabrica_Teste = 'TRIAD SYSTEM' where Fabrica_Teste like '%TRIAD%'");
            SGQConn.Dispose();
        }

        public void LoadData_Fabrica_Desenvolvimento(bool So_Nulos)
        {
            String Sql =
                @"update ALM_CTs 
                set 
	                Fabrica_Desenvolvimento = 
	                (case when (select top 1 Fabrica_Desenvolvimento_Nome from SGQ_Sistemas_Arquitetura where Nome like ALM_CTs.Sistema + '%' ) <> '' 
		                then (select top 1 Fabrica_Desenvolvimento_Nome from SGQ_Sistemas_Arquitetura where Nome like ALM_CTs.Sistema + '%' )
		                else ALM_CTs.Sistema
	                end)
                where ALM_CTs.subprojeto = '" + projeto.Subprojeto + "' and ALM_CTs.entrega = '" + projeto.Entrega + "'";

            if (!So_Nulos)
            {
                Sql = Sql + " and Fabrica_Desenvolvimento is null";
            }

            Connection SGQConn = new Connection();
            SGQConn.Executar(Sql);
            SGQConn.Executar("update ALM_CTs set Fabrica_Desenvolvimento = 'SAC' where Fabrica_Desenvolvimento like '%SAC%'");
            SGQConn.Dispose();
        }

    }
}
