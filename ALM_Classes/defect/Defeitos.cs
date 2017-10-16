using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.alm
{
    public class Defeitos
    {
        public Projeto projeto { get; set; }

        public List<Field> fields { get; set; }

        public TypeUpdate typeUpdate { get; set; }

        public alm.Database database { get; set; }

        public SqlMaker2Param sqlMaker2Param { get; set; }

        public Defeitos(Projeto projeto, TypeUpdate typeUpdate, alm.Database database) {
            this.projeto = projeto;
            this.typeUpdate = typeUpdate;
            this.database = database;

            sqlMaker2Param = new SqlMaker2Param();
            sqlMaker2Param.fields = new List<Field>();

            sqlMaker2Param.fields.Add(new Field() { target = "Subprojeto", source = "'{Subprojeto}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Entrega", source = "'{Entrega}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Defeito", source = "bg_bug_id", type = "N", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Detectado_Por", source = "Def.bg_detected_by" });
            sqlMaker2Param.fields.Add(new Field() { target = "Nome", source = "upper(replace(Def.bg_summary,'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Release", source = "replace(upper((select r.rel_name from {Esquema}.release_cycles rc, {Esquema}.releases r where rc.rcyc_id = Def.bg_detected_in_rcyc and r.rel_id = rc.rcyc_parent_id)),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Ciclo", source = "upper((select rc.rcyc_name from {Esquema}.release_cycles rc where rc.rcyc_id = Def.bg_detected_in_rcyc))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Severidade", source = "upper(Def.bg_severity)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Prioridade", source = "upper(Def.bg_priority)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Status_Atual", source = "upper(Def.bg_status)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Inicial", source = "to_char(BG_DETECTION_DATE, 'dd-mm-yy')" });
            //sqlMaker2Param.fields.Add(new Field() { target = "Dt_Inicial", source = "to_char(Dt_Inicial_Bug('{Esquema}', Def.bg_bug_id),'dd-mm-yy hh24:mi:ss')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Final", source = "(case when BG_CLOSING_DATE is null then '' else to_char(BG_CLOSING_DATE,'dd-mm-yy') || ' ' || substr(bg_vts,12,8) end)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Tempo_Resolucao_Dias", source = "Def.bg_actual_fix_time", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qtd_Reopen", source = "0", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "CT", source = "(select (case when ln_entity_type = 'TESTCYCL' then (select TC.tc_testcycl_id from {Esquema}.testcycl TC where TC.tc_testcycl_id = L.ln_entity_id) when ln_entity_type = 'RUN' then (select R.rn_testcycl_id from {Esquema}.run R where R.rn_run_id = L.ln_entity_id) when ln_entity_type = 'STEP' then (select R.rn_testcycl_id from {Esquema}.run R where R.rn_run_id = (select S.st_run_id from {Esquema}.step S where S.st_id = L.ln_entity_id)) end) from {Esquema}.Link L where L.ln_bug_id = Def.bg_bug_id and rownum=1)", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Alteracao", source = "substr(bg_vts,9,2) || '-' || substr(bg_vts,6,2) || '-' || substr(bg_vts,3,2) || ' ' || substr(bg_vts,12,8)" });

            sqlMaker2Param.fields.Add(new Field() { target = "Natureza", source = "upper(Def.bg_user_template_01)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Sistema_Defeito", source = "upper(Def.bg_user_template_02)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Sistema_CT", source = "upper(Def.bg_user_template_03)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Prevista_Solucao_Defeito", source = "(case when bg_user_template_04 is not null then substr(bg_user_template_04, 9, 2) || '-' || substr(bg_user_template_04, 6, 2) || '-' || substr(bg_user_template_04, 3, 2) || ' ' || substr(bg_user_template_04, 12, 8) else '' end)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Origem", source = "upper(Def.bg_user_template_05)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Erro_Detectavel_Em_Desenvolvimento", source = "replace(upper(Def.bg_user_template_06),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qtd_Reincidencia", source = "Def.bg_user_template_07", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Ja_Foi_Rejeitado", source = "upper(Def.bg_user_template_08)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Encaminhado_Para", source = "upper(Def.bg_user_template_09)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Motivo_Pendencia", source = "upper(Def.bg_user_template_10)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qtd_CTs_Impactados", source = "Def.bg_user_template_11", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Detalhamento_CR_PKE", source = "Def.bg_user_template_13" });

            if (database.name == "ALM11") {
                sqlMaker2Param.fields.Add(new Field() { target = "Dt_Ultimo_Status", source = "to_char(Dt_Ultimo_Status_Bug('{Esquema}', Def.bg_bug_id, Def.bg_status),'dd-mm-yy hh24:mi:ss')" });
                sqlMaker2Param.fields.Add(new Field() { target = "SLA", source = "SLA(Def.bg_severity)" });
            }

            sqlMaker2Param.dataSource = @"{Esquema}.Bug Def";
            sqlMaker2Param.dataSourceFilterCondition = "";
            sqlMaker2Param.targetTable = "ALM_Defeitos";

            this.sqlMaker2Param.targetSqlLastIdInserted = $"select max(defeito) from alm_defeitos where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";
            this.sqlMaker2Param.targetSqlLastDateUpdate = $"select Defeitos_Incremental_Inicio from alm_projetos where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";

            this.sqlMaker2Param.dataSourceFilterConditionInsert = $" bg_bug_id > {this.sqlMaker2Param.targetLastIdInserted}";
            this.sqlMaker2Param.dataSourceFilterConditionUpdate = $" substr(bg_vts,3,17) > '{this.sqlMaker2Param.targetLastDateUpdate}'";

            sqlMaker2Param.typeDB = "ORACLE"; 
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
                        set Defeitos_Incremental_Inicio='00-00-00 00:00:00',
                            Defeitos_Incremental_Fim='00-00-00 00:00:00',
                            Defeitos_Incremental_Tempo=0
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
                //this.LoadData_Etapa_Final();
                //this.LoadData_Improcedente();

                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Defeitos_Incremental_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Incremental_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Incremental_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");

            } else if (typeUpdate == TypeUpdate.Full) {
                SGQConn.Executar($"delete alm_defeitos where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'");

                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }

                //this.LoadData_Etapa();
                //this.LoadData_Etapa_Final();
                //this.LoadData_Improcedente();

                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Defeitos_Completa_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Completa_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Completa_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");
            }

            SGQConn.Dispose();
        }

        //public void Carregar_Condicoes_Insert()
        //{
        //    if (this.typeUpdate == TypeUpdate.Full)
        //        sqlMaker2Param.dataSourceFilterConditionInsert = "";
        //    else
        //    {
        //        Connection SGQConn = new Connection();
        //        sqlMaker2Param.targetLastIdInserted =
        //            SGQConn.Get_String(string.Format("Select Max(Defeito) from ALM_Defeitos where Subprojeto='{0}' and Entrega='{1}'", projeto.Subprojeto, projeto.Entrega));
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.targetLastIdInserted == "" || sqlMaker2Param.targetLastIdInserted == null)
        //            sqlMaker2Param.targetLastIdInserted = "0";

        //        sqlMaker2Param.dataSourceFilterConditionInsert = " def.bg_bug_id > " + sqlMaker2Param.targetLastIdInserted;
        //    }
        //}
        //public void Carregar_Condicoes_Update()
        //{
        //    if (this.typeUpdate == TypeUpdate.Full)
        //        sqlMaker2Param.dataSourceFilterConditionUpdate = "";
        //    else
        //    {
        //        Connection SGQConn = new Connection();
        //        sqlMaker2Param.Ultima_Atualizacao = SGQConn.Get_String_Por_Id("ALM_Projetos", "Defeitos_Incremental_Inicio", projeto.Id.ToString());
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.Ultima_Atualizacao == "" || sqlMaker2Param.Ultima_Atualizacao == null)
        //            sqlMaker2Param.Ultima_Atualizacao = "00-00-00 00:00:00";
        //        else
        //            sqlMaker2Param.Ultima_Atualizacao = sqlMaker2Param.Ultima_Atualizacao.Substring(6, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(3, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(0, 2) + " " + sqlMaker2Param.Ultima_Atualizacao.Substring(9, 8);

        //        sqlMaker2Param.dataSourceFilterConditionUpdate = "substr(bg_vts,3,17) > '" + sqlMaker2Param.Ultima_Atualizacao + "'";
        //    }
        //}

        public void LoadData_Etapa()
        {
            Connection SGQConn = new Connection();
            String Sql = $@"
                update alm_defeitos
                set alm_defeitos.Etapa = Aux.Etapa
                from
                (
                    select
                        Defeitos1.Subprojeto,
                        Defeitos1.Entrega,
                        Defeitos1.Defeito,
                        (case when cts1.CT <> ''
                            then
            	               case when cts1.Ciclo like '%TI%' then 'TI'
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
            						when cts1.Subprojeto like '%TRG%' then 'TRG'
            						else ''
								end
                            else
            					case 
            						when Defeitos1.Ciclo like '%TI%' then 'TI'
            						when Defeitos1.Ciclo like '%UAT%' then 'UAT-PRESENCIAL'
            						when Defeitos1.Ciclo like '%TRG%' then 'TRG'
            						when Defeitos1.Ciclo like '%TS%' then 'TS'
            						when Defeitos1.Ciclo like '%TP%' then 'TP'
            						when Defeitos1.Subprojeto like '%TRG%' then 'TRG'
            						else ''
            					end 
                        end) as Etapa
                    from 
                        alm_defeitos as Defeitos1 
                        left join alm_cts cts1 
                            on cts1.Subprojeto = Defeitos1.Subprojeto and
                                cts1.Entrega = Defeitos1.Entrega and
                                cts1.CT = Defeitos1.CT
                    where 
                        Defeitos1.subprojeto = '{projeto.Subprojeto}' and Defeitos1.entrega = '{projeto.Entrega}'
                ) Aux	
                where
                    alm_defeitos.Subprojeto = Aux.Subprojeto and
                    alm_defeitos.Entrega = Aux.Entrega and
                    alm_defeitos.Defeito = Aux.Defeito and 
                    (alm_defeitos.Etapa is null or alm_defeitos.Etapa <> Aux.Etapa)
            ";
            SGQConn.Executar(Sql);
            SGQConn.Dispose();
        }

        public void LoadData_Etapa_Final()
        {
            Connection SGQConn = new Connection();
            String Sql = $@"
                update alm_defeitos
                set alm_defeitos.Etapa_Final = Aux.Etapa
                from
                (
	                select
		                df.Subprojeto,
		                df.Entrega,
		                df.Defeito,
		                case when not exists (Select top 1 1
								                From ALM_Historico_Alteracoesfields hac 
								                where hac.Tabela = 'TESTCYCL' and 
									                hac.Campo = '(EVIDÊNCIA) VALIDAÇÃO CLIENTE' and 
									                hac.Novo_Valor = 'LIBERADO PARA VALIDAÇÃO' and 
									                hac.Subprojeto = df.Subprojeto and 
									                hac.Entrega = df.Entrega and 
									                hac.Tabela_Id = df.CT 
								                Order by Dt_Alteracao)
						                then 'TI'
						                else case when (DATEDIFF (day, Convert(Datetime, df.Dt_Inicial, 5), 
													                Convert(Datetime, ISNULL((select Top 1 Dt_Alteracao
																			                from ALM_Historico_Alteracoesfields hac 
																			                where hac.Tabela = 'TESTCYCL' and 
																					                hac.Campo = '(EVIDÊNCIA) VALIDAÇÃO CLIENTE' and 
																					                hac.Novo_Valor = 'LIBERADO PARA VALIDAÇÃO' and 
																					                hac.Subprojeto = df.Subprojeto and 
																					                hac.Entrega = df.Entrega and 
																					                hac.Tabela_Id = df.CT 
																			                order by Dt_Alteracao), ''), 5)
										                )) < 0 
								                then 'UAT'
								                else 'TI'
							                end
		                End as Etapa
	                From ALM_Defeitos df
	                where
                      df.subprojeto = '{projeto.Subprojeto}' and df.entrega = '{projeto.Entrega}' and 
	                  df.Status_Atual = 'CLOSED' and
	                  df.Origem like '%CONSTRUÇÃO%' and
	                  (df.Ciclo like '%TI%' or df.Ciclo like '%UAT%')
                ) Aux	
                where
                    alm_defeitos.Subprojeto = Aux.Subprojeto and
                    alm_defeitos.Entrega = Aux.Entrega and
                    alm_defeitos.Defeito = Aux.Defeito and 
                    (alm_defeitos.Etapa_Final is null or alm_defeitos.Etapa_Final <> Aux.Etapa)
            ";
            SGQConn.Executar(Sql);
            SGQConn.Dispose();
        }

        public void LoadData_Improcedente()
        {
            Connection SGQConn = new Connection();
            String Sql = $@"
                update alm_defeitos
                set alm_defeitos.Improcedente = Aux.Improcedente
                from
                (
	                select
		                df.Subprojeto,
		                df.Entrega,
		                df.Defeito,
		                case when df.origem = 'IMPROCEDENTE' AND df.Natureza IN ('FALHA DO TESTADOR','MASSA DE TESTES', 'PLANO DE TESTES') 
			                then 'SIM'
			                else 'NÃO'
		                end as Improcedente
	                From ALM_Defeitos df
	                where
                      df.subprojeto = '{projeto.Subprojeto}' and df.entrega = '{projeto.Entrega}'
                ) Aux	
                where
                    alm_defeitos.Subprojeto = Aux.Subprojeto and
                    alm_defeitos.Entrega = Aux.Entrega and
                    alm_defeitos.Defeito = Aux.Defeito and 
                    (alm_defeitos.Improcedente is null or alm_defeitos.Improcedente <> Aux.Improcedente)
            ";
            SGQConn.Executar(Sql);
            SGQConn.Dispose();
        }
    }
}

