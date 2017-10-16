using System;
using System.Collections.Generic;
using sgq;

namespace sgq.bpt
{
    public class BptProject
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Dominio { get; set; }
        public string Subprojeto { get; set; }
        public string Entrega { get; set; }
        public string Esquema { get; set; }
        public string Ativo { get; set; }

        public override string ToString()
        {
            return
                "Id:" + this.Id.ToString() + "\n\r" +
                "Nome:" + this.Nome + "\n\r" +
                "Dominio:" + this.Dominio + "\n\r" +
                "Subprojeto:" + this.Subprojeto + "\n\r" +
                "Entrega:" + this.Entrega + "\n\r" +
                "Esquema:" + this.Esquema + "\n\r" +
                "Ativo:" + this.Ativo;
        }

        public BptProject()
        {

        }

        public BptProject(string Subprojeto, string Entrega)
        {
            string sql = $@"
                select
                    p.Id, 
	                p.Nome, 
	                p.Dominio, 
	                p.Subprojeto, 
	                p.Entrega, 
	                p.Esquema, 
	                p.Ativo
                from
                    ALMA_Projetos p
                where Subprojeto = '{Subprojeto}' and Entrega = '{Entrega}'
                ";

            Connection Conn_SGQ = new Connection();
            var ALMA_Projeto = Conn_SGQ.Executar<BptProject>(sql);
            Conn_SGQ.Dispose();

            this.Id = ALMA_Projeto[0].Id;
            this.Nome = ALMA_Projeto[0].Nome;
            this.Dominio = ALMA_Projeto[0].Dominio;
            this.Subprojeto = ALMA_Projeto[0].Subprojeto;
            this.Entrega = ALMA_Projeto[0].Entrega;
            this.Esquema = ALMA_Projeto[0].Esquema;
            this.Ativo = ALMA_Projeto[0].Ativo;
        }

        public void LoadDataComponents(SqlMakerFurther sqlMaker)
        {
            var bptComponents = new BptComponents(sqlMaker);
            bptComponents.LoadData();
        }

        public void LoadDataComponentsSteps(SqlMakerFurther sqlMaker)
        {
            var bptComponentsSteps = new BptComponentsSteps(sqlMaker);
            bptComponentsSteps.LoadData();
        }

        public void LoadDataTestCycle(SqlMakerFurther sqlMaker)
        {
            var bptTestsCycle = new BptTestsCycle(sqlMaker);
            bptTestsCycle.LoadData();
        }

        public void LoadDataBugs(SqlMakerFurther sqlMaker)
        {
            var bptBugs = new BptBugs(sqlMaker);
            bptBugs.LoadData();
        }

        public void LoadDataRuns(SqlMakerFurther sqlMaker)
        {
            var bptRuns = new BptRuns(sqlMaker);
            bptRuns.LoadData();
        }

        public void LoadDataSteps(SqlMakerFurther sqlMaker)
        {
            var bptSteps = new BptSteps(sqlMaker);
            bptSteps.LoadData();
        }

        public void LoadDataRunsCriteria(SqlMakerFurther sqlMaker)
        {
            var bptRunsCriteria = new BptRunsCriteria(sqlMaker);
            bptRunsCriteria.LoadData();
        }

        public void LoadDataTests(SqlMakerFurther sqlMaker)
        {
            var bptTests = new BptTests(sqlMaker);
            bptTests.LoadData();
        }

        public void LoadDataDesSteps(SqlMakerFurther sqlMaker)
        {
            var bptDesSteps = new BptDesSteps(sqlMaker);
            bptDesSteps.LoadData();
        }

        public void LoadDataTestsToComponents(SqlMakerFurther sqlMaker)
        {
            var bptTestsToComponents = new BptTestsToComponents(sqlMaker);
            bptTestsToComponents.LoadData();
        }

        public void LoadDataTestsConfigs(SqlMakerFurther sqlMaker)
        {
            var bptTestsConfigs = new BptTestsConfigs(sqlMaker);
            bptTestsConfigs.LoadData();
        }

        public void LoadDataTestsCriteria(SqlMakerFurther sqlMaker)
        {
            var bptTestsCriteria = new BptTestsCriteria(sqlMaker);
            bptTestsCriteria.LoadData();
        }

        public void LoadDataIteration(SqlMakerFurther sqlMaker)
        {
            var BptIteration = new BptIteration(sqlMaker);
            BptIteration.LoadData();
        }

        public void LoadDataIterParam(SqlMakerFurther sqlMaker)
        {
            var BptIterParam = new BptIterParam(sqlMaker);
            BptIterParam.LoadData();
        }

        public void LoadDataParam(SqlMakerFurther sqlMaker)
        {
            var BptParam = new BptParam(sqlMaker);
            BptParam.LoadData();
        }

        public void LoadDataFrameworkParam(SqlMakerFurther sqlMaker)
        {
            var BptFrameworkParam = new BptFrameworkParam(sqlMaker);
            BptFrameworkParam.LoadData();
        }

        public void LoadDataAssetRelations(SqlMakerFurther sqlMaker)
        {
            var BptAssetRelations = new BptAssetRelations(sqlMaker);
            BptAssetRelations.LoadData();
        }

        public void LoadDataResources(SqlMakerFurther sqlMaker)
        {
            var BptResources = new BptResources(sqlMaker);
            BptResources.LoadData();
        }

        public void Atualizar_Tudo(TypeUpdate typeUpdate)
        {
            //this.Atualizar_Testes(typeUpdate);
            //this.Atualizar_Steps(typeUpdate);
            //this.Atualizar_CTs(typeUpdate);
            //this.Atualizar_Execucoes(typeUpdate);
            //this.Atualizar_Defeitos(typeUpdate);
            //this.Atualizar_Defeitos_Links(typeUpdate);
            //this.Atualizar_Defeitos_Tempos(typeUpdate);
            //this.Atualizar_Historicos(typeUpdate);
        }

        public string Get_Dt_Ultimo_Update_ALM()
        {
            //ALMConnection ALMConn = new ALMConnection(this.Esquema);

            //string sql = $@"
            //    select max(Dt_Ultimo_Update)
            //    from
            //      (
            //        select max(ts_vts) as Dt_Ultimo_Update from {this.Esquema}.test
            //        union all
            //        select max(ds_vts) as Dt_Ultimo_Update from {this.Esquema}.DesSteps
            //        union all
            //        select max(tc_vts) as Dt_Ultimo_Update from {this.Esquema}.testcycl
            //        union all
            //        select max(bg_vts) as Dt_Ultimo_Update from {this.Esquema}.bug
            //        union all
            //        select max(rn_vts) as Dt_Ultimo_Update from {this.Esquema}.run
            //        union all
            //        select max(cast(to_char(au_time, 'yyyy-mm-dd hh24:mi:ss') as char(20))) as Dt_Ultimo_Update from {this.Esquema}.audit_log
            //      )
            //    ";

            //string Dt_Ultimo_Update = ALMConn.Get_String_(sql);

            //ALMConn.Dispose();

            //if (Dt_Ultimo_Update != "")
            //{
            //    Dt_Ultimo_Update =
            //        Dt_Ultimo_Update.Substring(8, 2) + "-" +
            //        Dt_Ultimo_Update.Substring(5, 2) + "-" +
            //        Dt_Ultimo_Update.Substring(2, 2) + " " +
            //        Dt_Ultimo_Update.Substring(11, 8);
            //}
            //return Dt_Ultimo_Update;
            return "";
        }

        public void Carregar_Dt_Ultimo_Update_ALM()
        {
            Connection Conn_SGQ = new Connection();

            Conn_SGQ.Executar($"update ALMA_Projetos set Dt_Ultimo_Update_ALM = '{Get_Dt_Ultimo_Update_ALM()}' where ALMA_Projetos.Id={this.Id}");

            Conn_SGQ.Dispose();
        }
    }
}
