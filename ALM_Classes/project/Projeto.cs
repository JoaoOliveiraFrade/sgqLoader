using System;
using System.Collections.Generic;
using sgq;

namespace sgq.alm
{
    public class Projeto
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Dominio { get; set; }

        public string Subprojeto { get; set; }

        public string Entrega { get; set; }

        // public int Template { get; set; }

        public string Esquema { get; set; }

        //public string Ativo { get; set; }
        
        //public String Em_Andamento { get; set; }

        public override string ToString()
        {
            return
                "Id:" + this.Id.ToString() + "\n\r" +
                "Nome:" + this.Nome + "\n\r" +
                "Dominio:" + this.Dominio + "\n\r" +
                "Subprojeto:" + this.Subprojeto + "\n\r" +
                "Entrega:" + this.Entrega + "\n\r" +
                // "Template:" + this.Template.ToString() + "\n\r" +
                "Esquema:" + this.Esquema + "\n\r";
                // "Ativo:" + this.Ativo;
        }

        public Projeto()
        {
        }

        //public void LoadData_Projeto(Projeto projeto, Sql sql) {
        //    DateTime Dt_Inicio = DateTime.Now;

        //    string Origem_Condicoes_Insert = "";
        //    string Origem_Condicoes_Update = "";
        //    Connection SGQConn = new Connection();
        //    if (typeUpdate == TypeUpdate.Increment) {
        //        string Dt_Leitura = SGQConn.Get_String_Por_Id("ALM_Projetos", "CTs_Incremental_Inicio", projeto.Id.ToString());
        //        if (Dt_Leitura == "" || Dt_Leitura == null)
        //            Dt_Leitura = "00-00-00 00:00:00";
        //        else
        //            Dt_Leitura = Dt_Leitura.Substring(6, 2) + "-" + Dt_Leitura.Substring(3, 2) + "-" + Dt_Leitura.Substring(0, 2) + " " + Dt_Leitura.Substring(9, 8);

        //        Origem_Condicoes_Update = "substr(tc.tc_vts,3,17) > '" + Dt_Leitura + "'";

        //        string Ultimo_Id = SGQConn.Get_String("select max(CT) from ALM_CTs where Subprojeto = '{Subprojeto}' and Entrega = '{Entrega}'".Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega));
        //        if (Ultimo_Id == "" || Ultimo_Id == null)
        //            Ultimo_Id = "0";

        //        Origem_Condicoes_Insert = " tc.tc_testcycl_id > {Ultimo_Id}".Replace("{Ultimo_Id}", Ultimo_Id);
        //    }

        //    Sql sql = new Sql() {
        //        dataSource = @"{Esquema}.testcycl tc",
        //        dataSourceFilterCondition = "",
        //        dataSourceFilterConditionInsert = Origem_Condicoes_Insert,
        //        dataSourceFilterConditionUpdate = Origem_Condicoes_Update,
        //        targetTable = "ALM_CTs",
        //        fields = Campos,
        //        typeDB = "ORACLE"
        //    };
        //    sql.Carregarkeys();

        //    string Sql_Insert = sql.Get_Oracle_Insert().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
        //    string Sql_Update = sql.Get_Oracle_Update().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);

        //    ALMConnection ALMConn = new ALMConnection(alm.Databases.ALM11);
        //    OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
        //    OracleDataReader DataReader_Update = ALMConn.Get_DataReader(Sql_Update);

        //    if (DataReader_Insert != null && DataReader_Insert.HasRows == true)
        //        SGQConn.Executar(ref DataReader_Insert, 1);

        //    if (DataReader_Update != null && DataReader_Update.HasRows == true)
        //        SGQConn.Executar(ref DataReader_Update, 1);

        //    DateTime Dt_Fim = DateTime.Now;

        //    if (typeUpdate == TypeUpdate.Increment) {
        //        SGQConn.Setfields_Por_Id("ALM_Projetos", projeto.Id.ToString(), "CTs_Incremental_Inicio", "'" + Dt_Inicio.ToString("dd-MM-yy HH:mm:ss") + "'");
        //        SGQConn.Setfields_Por_Id("ALM_Projetos", projeto.Id.ToString(), "CTs_Incremental_Fim", "'" + Dt_Fim.ToString("dd-MM-yy HH:mm:ss") + "'");
        //        SGQConn.Setfields_Por_Id("ALM_Projetos", projeto.Id.ToString(), "CTs_Incremental_Tempo", DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim).ToString());
        //    } else {
        //        SGQConn.Setfields_Por_Id("ALM_Projetos", projeto.Id.ToString(), "CTs_Completa_Inicio", "'" + Dt_Inicio.ToString("dd-MM-yy HH:mm:ss") + "'");
        //        SGQConn.Setfields_Por_Id("ALM_Projetos", projeto.Id.ToString(), "CTs_Completa_Fim", "'" + Dt_Fim.ToString("dd-MM-yy HH:mm:ss") + "'");
        //        SGQConn.Setfields_Por_Id("ALM_Projetos", projeto.Id.ToString(), "CTs_Completa_Tempo", DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim).ToString());
        //    }

        //    SGQConn.Dispose();

        //    //Gerais.Enviar_Email_Atualizacao_Tabela("ALM", "ALM_CTs - " + projeto.Subprojeto + " - " + projeto.Entrega, Dt_Inicio, Dt_Fim);
        //}

        public virtual void LoadData_Testes(TypeUpdate typeUpdate) {
        }

        public virtual void LoadData_Steps(TypeUpdate typeUpdate) {
        }

        public virtual void LoadData_CTs(TypeUpdate typeUpdate) {
        }

        public virtual void LoadData_Execucoes(TypeUpdate typeUpdate) {
        }

        //public virtual void LoadData_Defeitos(TypeUpdate typeUpdate) {
        //    var defeitos_Template_05 = new Defeitos_Template_05(projeto: this, TypeUpdate: typeUpdate);
        //    defeitos_Template_05.LoadData();
        //}

        public virtual void LoadData_Defeitos_Links(TypeUpdate typeUpdate) {
        }
        public virtual void LoadData_Defeitos_Tempos(TypeUpdate typeUpdate) {
        }

        public virtual void LoadData_Historicos(TypeUpdate typeUpdate) {
        }

        //public void LoadData_Tudo(TypeUpdate typeUpdate) {
        //    this.LoadData_Testes(typeUpdate);
        //    this.LoadData_Steps(typeUpdate);
        //    this.LoadData_CTs(typeUpdate);
        //    this.LoadData_Execucoes(typeUpdate);
        //    //this.LoadData_Defeitos(typeUpdate);
        //    this.LoadData_Defeitos_Links(typeUpdate);
        //    this.LoadData_Defeitos_Tempos(typeUpdate);
        //    this.LoadData_Historicos(typeUpdate);
        //}

        //select
        //    p.Id, 
        //    convert(varchar, cast(substring(p.subprojeto,4,8) as int)) + ' ' + convert(varchar,cast(substring(p.entrega,8,8) as int)) as Nome, 
        //    p.Dominio as Dominio, 
        //    p.Subprojeto as Subprojeto, 
        //    p.Entrega as Entrega, 
        //    p.template as Template, 
        //    p.Esquema as Esquema, 
        //    p.Ativo as Ativo
        //	--right('00'+ convert(varchar,r.release_mes),2) + '/' + convert(varchar,r.release_ano) as release,
        //	--(select count(*) from alm_cts cts where cts.subprojeto = p.subprojeto and cts.entrega = p.entrega) as qtyCts
        //from 
        //	ALM_Projetos p
        //	left join sgq_projects
        //		on sgq_projects.subproject = p.subprojeto and
        //		    sgq_projects.delivery = p.entrega
        //where 
        //    p.Ativo = 'Y' and 
        //    p.Entrega not in ('ENTREGA_UNIF', 'ENTREGA_UNIF2') and
        //	(
        //		p.subprojeto like 'TRG%'
        //        or
        //		right('0000' + convert(varchar(4), isnull(currentReleaseYear,0)),4) + right('00' + convert(varchar(2), isnull(currentReleaseMonth,0)),2) >= right('0000' + convert(varchar(4), datepart(yyyy, dateadd(m, -1, getdate()))),4) + right('00' + convert(varchar(2), datepart(m, dateadd(m, -1, getdate()))),2)
        //		or
        //		right('0000' + convert(varchar(4), isnull(clarityReleaseYear,0)),4) + right('00' + convert(varchar(2), isnull(clarityReleaseMonth,0)),2) >= right('0000' + convert(varchar(4), datepart(yyyy, dateadd(m, -1, getdate()))),4) + right('00' + convert(varchar(2), datepart(m, dateadd(m, -1, getdate()))),2)
        //	)
        //	--Dt_Ultimo_Update_ALM is not null and
        //	--convert(datetime, Dt_Ultimo_Update_ALM, 5) > (getdate() - 7) 
        //order by 
        //	    (select count(*) from alm_cts cts where cts.subprojeto = p.subprojeto and cts.entrega = p.entrega)

        public string Get_Dt_Ultimo_Update_ALM()
        {
            ALMConnection ALMConn = new ALMConnection(this.Esquema);

            string sql = $@"
                select max(Dt_Ultimo_Update)
                from
                  (
                    select max(ts_vts) as Dt_Ultimo_Update from {this.Esquema}.test
                    union all
                    select max(ds_vts) as Dt_Ultimo_Update from {this.Esquema}.DesSteps
                    union all
                    select max(tc_vts) as Dt_Ultimo_Update from {this.Esquema}.testcycl
                    union all
                    select max(bg_vts) as Dt_Ultimo_Update from {this.Esquema}.bug
                    union all
                    select max(rn_vts) as Dt_Ultimo_Update from {this.Esquema}.run
                    union all
                    select max(cast(to_char(au_time, 'yyyy-mm-dd hh24:mi:ss') as char(20))) as Dt_Ultimo_Update from {this.Esquema}.audit_log
                  )
                ";

            string Dt_Ultimo_Update = ALMConn.Get_String_(sql);

            ALMConn.Dispose();

            if (Dt_Ultimo_Update != "")
            {
                Dt_Ultimo_Update =
                    Dt_Ultimo_Update.Substring(8, 2) + "-" +
                    Dt_Ultimo_Update.Substring(5, 2) + "-" +
                    Dt_Ultimo_Update.Substring(2, 2) + " " +
                    Dt_Ultimo_Update.Substring(11, 8);
            }
            return Dt_Ultimo_Update;
        }

        public void Carregar_Dt_Ultimo_Update_ALM()
        {
            Connection Conn_SGQ = new Connection();

            Conn_SGQ.Executar($"update ALM_Projetos set Dt_Ultimo_Update_ALM = '{Get_Dt_Ultimo_Update_ALM()}' where ALM_Projetos.Id={this.Id}");

            Conn_SGQ.Dispose();
        }
    }
}
