using System;
using System.Data;
using Oracle.DataAccess.Client;
using System.Data.SqlClient;
using System.Collections.Generic;
using sgq;

namespace sgq.bpt
{
    public class BptProjects
    {
        private SqlMakerProjects SqlMakerProjects { get; set; }

        public BptProjects(SqlMakerProjects sqlMakerProjects = null)
        {
            if (sqlMakerProjects != null)
                SqlMakerProjects = sqlMakerProjects;
            else
                throw new ArgumentNullException("sqlMakerProjects", "O parâmetro 'sqlMakerProjects' não pode ser null");

            this.SqlMakerProjects.TargetTable = "BPT_Projects";
            this.SqlMakerProjects.ConditionsDataSourceUpdate = "";
            this.SqlMakerProjects.dataSourceFieldId = "Project_Id";

            if (SqlMakerProjects.bptConnection.Ambiente == Ambientes.PRODUCAO)
            {
                this.SqlMakerProjects.dataSource = "qcsiteadmin_db2.projects";
                /*
                this.SqlMakerProjects.dataSourceCondition =
                        @"domain_name in('PRJ') 
                              and substr(PROJECT_NAME,1,3) = 'PRJ' 
                              and substr(PROJECT_NAME,12,4) = '_ENT' 
                              and INSTR(PROJECT_NAME,'TESTE') = 0";
                */
                this.SqlMakerProjects.dataSourceCondition = "domain_name in('AUXILIARES') and project_name in('AUTOM_LINK')";
            }
            else
            {
                this.SqlMakerProjects.dataSource = "qcsiteadmin_db.projects";
                // this.SqlMakerProjects.dataSourceCondition = "domain_name in('NEW_FEATURES') and project_name in('T705_2')";
                // this.SqlMakerProjects.dataSourceCondition = "domain_name in('NEW_FEATURES') and project_name in('AUTOM_LINK')";
                this.SqlMakerProjects.dataSourceCondition = "domain_name in('NEW_FEATURES') and project_name in('AUTOM_LINK2')";
            }

            this.SqlMakerProjects.fields = new List<Field>();
            this.SqlMakerProjects.fields.Add(new Field() { key = true, type = "N", source = "Project_Id", target = "Id" });
            this.SqlMakerProjects.fields.Add(new Field() { source = "upper(rtrim(substr(Project_Name,1,11)))", target = "Subprojeto" });
            this.SqlMakerProjects.fields.Add(new Field() { source = "upper(rtrim('ENTREGA' || substr(Project_Name,16,8)))", target = "Entrega" });
            this.SqlMakerProjects.fields.Add(new Field() { source = "upper(rtrim(Project_Name))", target = "Nome" });
            this.SqlMakerProjects.fields.Add(new Field() { source = "upper(rtrim(Create_from_Project))", target = "Origem" });
            this.SqlMakerProjects.fields.Add(new Field() { source = "upper(rtrim(Domain_Name))", target = "Dominio" });
            this.SqlMakerProjects.fields.Add(new Field() { source = "Db_Name", target = "Esquema" });
            this.SqlMakerProjects.fields.Add(new Field() { source = "upper(rtrim(Pr_Is_Active))", target = "Ativo" });
            this.SqlMakerProjects.fields.Add(new Field() { source = "substr(description, 20, 2) || '-' || substr(description, 17, 2) || '-' || substr(description, 14, 2) || ' ' || substr(description, 23, 8)", target = "Dt_Criacao" });
        }

        public Period insert()
        {
            var period = new Period();

            string sqlInsert = this.SqlMakerProjects.GetSqlInsert();
            OracleDataReader oracleDataReader = this.SqlMakerProjects.bptConnection.Get_DataReader(sqlInsert);
            if (oracleDataReader != null && oracleDataReader.HasRows == true)
            {
                this.SqlMakerProjects.Connection.Executar(ref oracleDataReader, 1);
            }

            period.End = DateTime.Now;

            return period;
        }

        public Period Update()
        {
            var period = new Period();

            string SqlUpdate = this.SqlMakerProjects.GetSqlUpdate();
            OracleDataReader OracleDataReaderUpdate = this.SqlMakerProjects.bptConnection.Get_DataReader(SqlUpdate);
            if (OracleDataReaderUpdate != null && OracleDataReaderUpdate.HasRows == true)
            {
                this.SqlMakerProjects.Connection.Executar(ref OracleDataReaderUpdate, 1);
            }

            period.End = DateTime.Now;

            return period;
        }

        public Period delete()
        {
            var period = new Period();

            this.SqlMakerProjects.Connection.Executar(this.SqlMakerProjects.getSqlDelete());

            period.End = DateTime.Now;

            return period;
        }

        public Period clearKeys()
        {
            var period = new Period();

            this.SqlMakerProjects.Connection.Executar(this.SqlMakerProjects.getSqlDeleteKeys());

            period.End = DateTime.Now;

            return period;
        }

        public Period deleteAllData()
        {
            var Period = new Period();

            var Connection = new Connection();

            Connection.Executar(@"
                truncate table BPT_Des_Steps
                truncate table BPT_Components_Steps
                truncate table BPT_Tests_To_Components
                truncate table BPT_Runs_Criteria
                truncate table BPT_Steps
                delete BPT_Components
                delete BPT_Tests_Criteria
                delete BPT_Runs
                delete BPT_Tests_Cycle
                delete BPT_Tests_Configs
                delete BPT_Tests
                delete BPT_Bugs
                delete BPT_Iteration
                delete BPT_Iter_Param
                delete BPT_Param
                delete BPT_Framework_Param
                delete BPT_Asset_Relations
                delete BPT_Resources

                truncate table BPT_Des_Steps_Keys
                truncate table BPT_Components_Steps_Keys
                truncate table BPT_Tests_To_Components_Keys
                truncate table BPT_Steps_Keys
                truncate table BPT_Runs_Criteria_Keys
                truncate table BPT_Components_Keys
                truncate table BPT_Tests_Criteria_Keys
                truncate table BPT_Runs_Keys
                truncate table BPT_Tests_Cycle_Keys
                truncate table BPT_Tests_Configs_Keys
                truncate table BPT_Tests_Keys
                truncate table BPT_Bugs_Keys
                truncate table BPT_Iteration_Keys
                truncate table BPT_Iter_Param_Keys
                truncate table BPT_Param_Keys
                truncate table BPT_Framework_Param_Keys
                truncate table BPT_Asset_Relations_Keys
                truncate table BPT_Resources_Keys
                "
            );

            Connection.Dispose();

            Period.End = DateTime.Now;

            return Period;
        }

        public Period loadkeys()
        {
            var period = new Period();

            this.SqlMakerProjects.Connection.Executar(this.SqlMakerProjects.getSqlDeleteKeys());

            this.SqlMakerProjects.Connection.Executar(this.SqlMakerProjects.getSqlInsertKeys());

            period.End = DateTime.Now;

            return period;
        }

        public Period loadOwnData()
        {
            var period = new Period();

            if (this.SqlMakerProjects.GetCountRowsTarget > 0)
            {
                this.loadkeys();
                this.delete();
                this.clearKeys();
                this.Update();
                this.insert();
            }
            else
                this.insert();

            period.End = DateTime.Now;

            return period;
        }

        public Period loadFurtherData()
        {
            var Period = new Period();
            //var bptProjectsActiveNames = new List<String>();
            var bptProjectsActive = this.GetActive();
            foreach (var bptProject in bptProjectsActive)
            {
                var period = new Period();

                var sqlMaker = new SqlMakerFurther(this.SqlMakerProjects.bptConnection, this.SqlMakerProjects.Connection, this.SqlMakerProjects.typeUpdate, bptProject);

                bptProject.LoadDataTests(sqlMaker);
                bptProject.LoadDataTestsConfigs(sqlMaker);
                bptProject.LoadDataTestCycle(sqlMaker);
                bptProject.LoadDataRuns(sqlMaker);
                bptProject.LoadDataSteps(sqlMaker);
                bptProject.LoadDataTestsCriteria(sqlMaker);
                bptProject.LoadDataComponents(sqlMaker);
                bptProject.LoadDataComponentsSteps(sqlMaker);
                bptProject.LoadDataDesSteps(sqlMaker);
                bptProject.LoadDataRunsCriteria(sqlMaker);
                bptProject.LoadDataTestsToComponents(sqlMaker);
                bptProject.LoadDataBugs(sqlMaker);
                bptProject.LoadDataIteration(sqlMaker);
                bptProject.LoadDataIterParam(sqlMaker);
                bptProject.LoadDataParam(sqlMaker);
                bptProject.LoadDataFrameworkParam(sqlMaker);
                bptProject.LoadDataAssetRelations(sqlMaker);
                bptProject.LoadDataResources(sqlMaker);

                period.End = DateTime.Now;

                var Tempo = (int)period.End.Subtract(period.Start).Seconds;
                
                //string Nome_Projeto_BITI = Conn_SGQ.Get_String(string.Format("select Nome from biti_subprojetos where id = '{0}'", item.Subprojeto));
                //projectsNames.Add(Tempo.ToString().PadLeft(4, '0') + " seg, " + item.Nome + " - " + Nome_Projeto_BITI);
            }

            //Conn_SGQ.Dispose();

            //DateTime Dt_Fim_Geral = DateTime.Now;

            //if (Projetos.Count > 0)
            //{
            //    Gerais.Enviar_Email_Atualizacao_Projetos(
            //        Assunto: string.Format("SGQ-ALMA: Atualizou projetos - Tipo Atualização: INCREMENTAL"),
            //        projectList: List_Nome_Projetos,
            //        Dt_Inicio: Dt_Inicio_Geral,
            //        Dt_Fim: Dt_Fim_Geral
            //    );
            //}

            Period.End = DateTime.Now;

            return Period;
        }

        public List<BptProject> GetActive()
        {
            string sql = @"
                select 
                    Id, 
                    Nome, 
                    Dominio, 
                    Subprojeto, 
                    Entrega, 
                    Esquema, 
                    Ativo
                from BPT_Projects 
                where 
                    Ativo='Y'
                order by convert(datetime, CTs_Completa_Fim, 5) desc
            ";

            List<BptProject> BptProjects = this.SqlMakerProjects.Connection.Executar<BptProject>(sql);

            //foreach (var project in Projects)
            //{
            //    project.SqlMaker = new SqlMaker(this.SqlMakerProjects.bptConnection, this.SqlMakerProjects.Connection, this.SqlMakerProjects.typeUpdate, project);
            //}

            return BptProjects;
        }

    }
}
