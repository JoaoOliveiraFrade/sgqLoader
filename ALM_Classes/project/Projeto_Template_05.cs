using System;
using Oracle.DataAccess.Client;
using System.Collections.Generic;
using sgq;

namespace sgq.alm
{
    public class Projeto_Template_05 : Projeto
    {
        public Projeto_Template_05()
        {
        }

        public Projeto_Template_05(string _Subprojeto, string _Entrega)
        {
            Connection Conn_SGQ = new Connection();

            var projeto_Template_05 =
                Conn_SGQ.Executar<Projeto_Template_05>(
                    string.Format(@"select 
                                        Id, 
                                        Nome, 
                                        Dominio, 
                                        Subprojeto, 
                                        Entrega, 
                                        Template,
                                        Esquema, 
                                        Ativo, 
                                        (case when
                                            Subprojeto + Entrega in 
                                            (
	                                            select Subprojeto + Entrega as Chave from SGQ_Releases_Entregas where Release in (select id from SGQ_Releases where Status = 2)
	                                            union all
	                                            select Subprojeto + Entrega as Chave from SGQ_Releases_Entregas_Somente_Exec_Teste where Release in (select id from SGQ_Releases where Status = 2)
                                            )
                                            then 'SIM'
                                            else 'NÃO'
                                        end) Em_Andamento
                                    from alm_projetos 
                                    where Subprojeto = '{0}' and Entrega = '{1}' ", _Subprojeto, _Entrega)
                 );

            Conn_SGQ.Dispose();

            this.Id = projeto_Template_05[0].Id;
            this.Nome = projeto_Template_05[0].Nome;
            this.Dominio = projeto_Template_05[0].Dominio;
            this.Subprojeto = projeto_Template_05[0].Subprojeto;
            this.Entrega = projeto_Template_05[0].Entrega;
            //this.Template = projeto_Template_05[0].Template;
            this.Esquema = projeto_Template_05[0].Esquema;
            //this.Ativo = projeto_Template_05[0].Ativo;
        }

        //public override void LoadData_Testes(TypeUpdate typeUpdate)
        //{
        //    var oTestes_Template_05 = new Testes_Template_05(projeto: this, typeUpdate: typeUpdate);
        //    oTestes_Template_05.LoadData();
        //}

        //public override void LoadData_Steps(TypeUpdate typeUpdate)
        //{
        //    var oSteps_Template_05 = new Steps_Template_05(projeto: this, typeUpdate: typeUpdate);
        //    oSteps_Template_05.LoadData();
        //}

        //public override void LoadData_CTs(TypeUpdate typeUpdate)
        //{
        //    var oCTs_Template_05 = new CTs_Template_05(projeto: this, typeUpdate: typeUpdate);
        //    oCTs_Template_05.LoadData();
        //}

        //public override void LoadData_Execucoes(TypeUpdate typeUpdate)
        //{
        //    var oExecucoes_Template_05 = new Execucoes_Template_05(projeto: this, typeUpdate: typeUpdate);
        //    oExecucoes_Template_05.LoadData();
        //}

        //public override void LoadData_Defeitos(TypeUpdate typeUpdate)
        //{
        //    var defeitos_Template_05 = new Defeitos_Template_05(projeto: this, TypeUpdate: typeUpdate);
        //    defeitos_Template_05.LoadData();
        //}

        //public override void LoadData_Defeitos_Links(TypeUpdate typeUpdate)
        //{
        //    var defeitos_Links_Template_05 = new Defeitos_Links_Template_05(projeto: this, typeUpdate: typeUpdate);
        //    defeitos_Links_Template_05.LoadData();
        //}

        public override void LoadData_Defeitos_Tempos(TypeUpdate typeUpdate)
        {
            DateTime Dt_Inicio_Geral = DateTime.Now;

            // Defeitos_Tempos.LoadData(project, typeUpdate, alm.Database); já alterado ?????????? 

            Connection SGQConn = new Connection();
            SGQConn.Executar(
                @"update ALM_Defeitos
                    set Qtd_Reopen = 
                        (select count(*)
                            from  
                            (select distinct Dt_Ate from ALM_Defeitos_Tempos t 
                                where t.Subprojeto = '{Subprojeto}' and t.Entrega = '{Entrega}' and t.Defeito = ALM_Defeitos.Defeito and t.Status = 'REOPEN') x
                        ) ".Replace("{Subprojeto}", this.Subprojeto).Replace("{Entrega}", this.Entrega));

            SGQConn.Dispose();

            DateTime Dt_Fim_Geral = DateTime.Now;

            if (typeUpdate == TypeUpdate.Full)
            {
                Gerais.Enviar_Email_Atualizacao_Projetos(
                    @"SGQ - ALM - Projeto " + this.Nome +
                    " Atualizado - Tipo Atualização: " + typeUpdate +
                    ", Escopo de Dados: Defeitos Tempos",
                    new List<String> { this.Nome },
                    Dt_Inicio_Geral,
                    Dt_Fim_Geral
                );
            }
        }

        //public override void LoadData_Historicos(TypeUpdate typeUpdate)
        //{
        //    var oHistoricos_Template_05 = new Historicos_Template_05(projeto: this, typeUpdate: typeUpdate);
        //    oHistoricos_Template_05.LoadData();
        //}
    }
}
