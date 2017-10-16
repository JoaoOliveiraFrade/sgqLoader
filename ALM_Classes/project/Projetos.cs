using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using System.IO;

namespace sgq.alm {
    public class Projetos {
        public alm.Database database { get; set; }

        public SqlMaker2Param sqlMaker2Param { get; set; }

        public TypeUpdate typeUpdate { get; set; }

        public Projetos(alm.Database database, List<Field> fields, TypeUpdate typeUpdate = TypeUpdate.Increment) {
            this.database = database;
            this.typeUpdate = typeUpdate;

            this.sqlMaker2Param = new SqlMaker2Param();
            this.sqlMaker2Param.fields = fields;

            this.sqlMaker2Param.dataSource = $@"
                (
                    select project_id, domain_name, create_from_domain, create_from_project, project_name, db_name, physical_directory, pr_is_active, description,
                        substr(d,4,2) || '-' || substr(d,1,2) || '-' || substr(d,9,2) dtCreate
                    from
                        (
                            select project_id, domain_name, create_from_domain, create_from_project, project_name, db_name, physical_directory, pr_is_active, description,
                                case when substr(d,5,1) = '/' then substr(d,1,3) || '0' || substr(d,4) else d end d
                            from
                                (
                                    select project_id, domain_name, create_from_domain, create_from_project, project_name, db_name, physical_directory, pr_is_active, description,
                                        case when substr(d,2,1) = '/' then '0' || substr(d,1) else d end d
                                    from
                                        (
                                            select project_id, domain_name, create_from_domain, create_from_project, project_name, db_name, physical_directory, pr_is_active, description,
                                                substr(dbms_lob.substr(description, 30, 1),12,9) d
                                            from {this.database.scheme}.projects p  
                                        ) aux1
                                ) aux2
                        ) aux3
                ) aux4
            ";

            this.sqlMaker2Param.dataSourceFilterCondition = $@"
                domain_name = '{this.database.dominio}'
                and substr(project_name,1,3) = 'PRJ' 
                and substr(project_name,12,4) = '_ENT' 
            ";
            this.sqlMaker2Param.targetTable = "alm_projetos";
            this.sqlMaker2Param.targetSqlLastIdInserted = $"select max(id) from alm_projetos where dominio = '{this.database.dominio}'";

            this.sqlMaker2Param.dataSourceFilterConditionInsert = $" project_id > {this.sqlMaker2Param.targetLastIdInserted} ";
            this.sqlMaker2Param.dataSourceFilterConditionUpdate = "";

            this.sqlMaker2Param.typeDB = "ORACLE";
        }

        //public void Carregar_Dt_Ultimo_Update_ALM() {
        //    var projetos_ativos = GetProjects_Ativos<Projeto_Template_05>();

        //    foreach (var projeto in projetos_ativos) {
        //        projeto.Carregar_Dt_Ultimo_Update_ALM();
        //    }
        //}
        
        public void Update() {
            //ALMConnection conn = new ALMConnection(database);

            //OracleDataReader res = conn.Get_DataReader($"select PROJECT_NAME from {database.Scheme}.projects");
            //// Console.WriteLine(res.HasRows);

            //while (res.Read())
            //{
            //    Console.WriteLine(res["PROJECT_NAME"].ToString());
            //}

            //conn.Dispose();

            // this.Carregar_Condicoes_Insert();
            //this.Carregar_Condicoes_Update();

            SqlMaker2 sqlMaker2 = new SqlMaker2 { sqlMaker2Param = this.sqlMaker2Param };

            ALMConnection ALMConn = new ALMConnection(this.database);
            Connection SGQConn = new Connection();

            OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(sqlMaker2.Get_Oracle_Insert());
            if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                SGQConn.Executar(ref DataReader_Insert, 1);
            }

            OracleDataReader DataReader_Update = ALMConn.Get_DataReader(sqlMaker2.Get_Oracle_Update());
            if (DataReader_Update != null && DataReader_Update.HasRows == true) {
                SGQConn.Executar(ref DataReader_Update, 1);
            }

            SGQConn.Dispose();
        }

        public void loadData() {
            DateTime Dt_Inicio_Geral = DateTime.Now;

            var SGQConn = new Connection();

            var List_Nome_Projetos = new List<String>();

            List<Projeto_Template_05> projectList = this.GetProjects<Projeto_Template_05>();
            foreach (var project in projectList) {
                DateTime Dt_Inicio = DateTime.Now;

                var tests = new Tests(project, typeUpdate, database);
                tests.LoadData();

                var steps = new Steps(project, typeUpdate, database);
                steps.LoadData();

                var cts = new CTs(project, typeUpdate, database);
                cts.LoadData();

                var execucoes = new Execucoes(project, typeUpdate, database);
                execucoes.LoadData();

                var defeitos = new Defeitos(project, typeUpdate, database);
                defeitos.LoadData();

                var defeitosLinks = new Defeitos_Links(project, typeUpdate, database);
                defeitosLinks.LoadData();

                // Defeitos_Tempos.LoadData(project, typeUpdate, alm.Database);

                var historicos = new Historicos(project, typeUpdate, database);
                historicos.LoadData();

                DateTime Dt_Fim = DateTime.Now;

                long Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim);
                string Nome_Projeto_BITI = SGQConn.Get_String($"select Nome from biti_subprojetos where id = '{project.Subprojeto}'");
                List_Nome_Projetos.Add(Tempo.ToString().PadLeft(4, '0') + " seg, " + project.Nome + " - " + Nome_Projeto_BITI);
            }

            SGQConn.Dispose();

            DateTime Dt_Fim_Geral = DateTime.Now;

            if (projectList.Count > 0) {
                List_Nome_Projetos.Sort();
                Gerais.Enviar_Email_Atualizacao_Projetos(
                    Assunto: string.Format($"[SGQLoader]{database.name} - projetos - {this.typeUpdate}"),
                    projectList: List_Nome_Projetos,
                    Dt_Inicio: Dt_Inicio_Geral,
                    Dt_Fim: Dt_Fim_Geral
                );
            }
        }


        public void loadDefectsTimes() {
            DateTime Dt_Inicio_Geral = DateTime.Now;

            var SGQConn = new Connection();

            var List_Nome_Projetos = new List<String>();

            List<Projeto_Template_05> projectList = this.GetProjects<Projeto_Template_05>();
            foreach (var project in projectList) {
                DateTime Dt_Inicio = DateTime.Now;

                Defeitos_Tempos.LoadData(project, typeUpdate, database);

                DateTime Dt_Fim = DateTime.Now;

                long Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim);
                string Nome_Projeto_BITI = SGQConn.Get_String($"select Nome from biti_subprojetos where id = '{project.Subprojeto}'");
                List_Nome_Projetos.Add(Tempo.ToString().PadLeft(4, '0') + " seg, " + project.Nome + " - " + Nome_Projeto_BITI);
            }

            SGQConn.Dispose();

            DateTime Dt_Fim_Geral = DateTime.Now;

            if (projectList.Count > 0) {
                List_Nome_Projetos.Sort();
                Gerais.Enviar_Email_Atualizacao_Projetos(
                    Assunto: string.Format($"[SGQLoader]{database.name} - Defeitos Tempos - {this.typeUpdate}"),
                    projectList: List_Nome_Projetos,
                    Dt_Inicio: Dt_Inicio_Geral,
                    Dt_Fim: Dt_Fim_Geral
                );
            }
        }

        public void LoadData_Projetos_CTs() {
            //int Template = 5;

            //DateTime Dt_Inicio_Geral = DateTime.Now;

            //Connection SGQConn = new Connection();

            //var List_Nome_Projetos = new List<String>();
            //var projectList = this.GetProjects<Projeto_Template_05>();
            //foreach (var projeto_Template_05 in projectList) {
            //    DateTime Dt_Inicio = DateTime.Now;

            //    projeto_Template_05.LoadData_CTs(this.typeUpdate);

            //    DateTime Dt_Fim = DateTime.Now;

            //    long Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim);
            //    string Nome_Projeto_BITI = SGQConn.Get_String(string.Format("select Nome from biti_subprojetos where id = '{0}'", projeto_Template_05.Subprojeto));
            //    List_Nome_Projetos.Add(Tempo.ToString().PadLeft(4, '0') + " seg, " + projeto_Template_05.Nome + " - " + Nome_Projeto_BITI);
            //}

            //SGQConn.Dispose();

            //DateTime Dt_Fim_Geral = DateTime.Now;

            //if (projectList.Count > 0) {
            //    Gerais.Enviar_Email_Atualizacao_Projetos(
            //        Assunto: string.Format($"SGQ-ALM: Atualizou CTS de projetos - Tipo Atualização: {this.typeUpdate}"),
            //        projectList: List_Nome_Projetos,
            //        Dt_Inicio: Dt_Inicio_Geral,
            //        Dt_Fim: Dt_Fim_Geral
            //    );
            //}
        }

        public void Limpar_Dados_Projetos_Excluidos() {
            Connection Conn_SGQ = new Connection();
            Conn_SGQ.Executar(
                @"
                delete ALM_Execucoes where subprojeto + Entrega not in (select subprojeto + Entrega from alm_projetos where ativo = 'Y')
                delete ALM_CTs where subprojeto + Entrega not in (select subprojeto + Entrega from alm_projetos where ativo = 'Y')
                delete ALM_Defeitos_Links where subprojeto + Entrega not in (select subprojeto + Entrega from alm_projetos where ativo = 'Y')
                delete ALM_Defeitos_Tempos where subprojeto + Entrega not in (select subprojeto + Entrega from alm_projetos where ativo = 'Y')
                delete ALM_Defeitos where subprojeto + Entrega not in (select subprojeto + Entrega from alm_projetos where ativo = 'Y')
                delete ALM_Historico_Alteracoesfields where subprojeto + Entrega not in (select subprojeto + Entrega from alm_projetos where ativo = 'Y')
                delete ALM_Steps where subprojeto + Entrega not in (select subprojeto + Entrega from alm_projetos where ativo = 'Y')
                delete ALM_Testes where subprojeto + Entrega not in (select subprojeto + Entrega from alm_projetos where ativo = 'Y')
                "
            );
            Conn_SGQ.Dispose();
        }

        public List<T> GetProjects_Ativos<T>() {
            Connection SGQConn = new Connection();

            List<T> projetos = SGQConn.Executar<T>(
                @"select 
                    Id, 
                    Nome, 
                    Dominio, 
                    Subprojeto, 
                    Entrega, 
                    Template, 
                    Esquema, 
                    Ativo
                from ALM_Projetos 
                where 
                    Entrega not in ('ENTREGA_UNIF', 'ENTREGA_UNIF2') and
                    Ativo='Y'
                order by convert(datetime, CTs_Completa_Fim, 5) desc"
                );

            SGQConn.Dispose();

            return projetos;
        }

        public List<T> GetProjects<T>() {
            //BD oBD = new BD(Lib.SGQ());
            // oBD.Executar(File.ReadAllText(Gerais.Caminho_App() + @"\SQLs\Insert_SGQ_Historico_Valores_Execucao_Teste.sql"));
            //oBD.Close();

            string sql = File.ReadAllText(Gerais.Caminho_App() + $@"\sql\{database.sqlListProjects}", System.Text.Encoding.Default);

            Connection SGQConn = new Connection();
            List<T> projetos = SGQConn.Executar<T>(sql);

            SGQConn.Dispose();

            return projetos;
        }
    }
}

