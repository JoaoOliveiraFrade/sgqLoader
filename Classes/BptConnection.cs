using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Reflection;
using sgq;
using Oracle.DataAccess.Client;

namespace sgq
{
    public class BptConnection : IDisposable
    {
        public OracleConnection OracleConnection { get; set; }
        public string ConnectionString { get; set; }
        public string Sid { get; set; }
        public Ambientes Ambiente { get; set; }
        
        public BptConnection(Ambientes ambiente = Ambientes.PRODUCAO, string schema = "")
        {
            this.Ambiente = ambiente;

            if (this.Ambiente == Ambientes.PRODUCAO)
            {
                this.ConnectionString = ConfigurationManager.ConnectionStrings["connectionStringAlmPrd"].ConnectionString;
                this.Sid = "QC11PRD1";
            }
            else
            {
                this.ConnectionString = ConfigurationManager.ConnectionStrings["connectionStringAlmHml"].ConnectionString;
                this.Sid = "QC11HML1";
            }

            if (schema != "")
            {
                var Parametros = this.ConnectionString.Split(';');
                Parametros[2] = $"User ID={schema}";
                Parametros[3] = $"Password=tdtdtd";
                this.ConnectionString = string.Join(";", Parametros);
            }

            if (OracleConnection == null)
                OracleConnection = new OracleConnection(this.ConnectionString);

            if (OracleConnection.State == ConnectionState.Closed)
                OracleConnection.Open();
        }

        public List<T> Executar<T>(string sql)
        {
            OracleDataReader DataReader = this.Get_DataReader(sql);
            List<T> Lista = this.DataReaderMapToList<T>(DataReader);

            if (DataReader != null)
                DataReader.Dispose();

            return Lista;
        }
        public void Dispose()
        {
            if (OracleConnection.State == ConnectionState.Open)
                OracleConnection.Close();
        }
        public OracleDataReader Get_DataReader(string sql)
        {
            try
            {
                OracleCommand oOracleCommand = new OracleCommand($"{this.Sid}.GET_QCALM11SQL_V2", OracleConnection);
                oOracleCommand.CommandType = CommandType.StoredProcedure;
                oOracleCommand.Parameters.Add(new OracleParameter("QCALM11SQL", OracleDbType.Clob));
                oOracleCommand.Parameters.Add("RS", OracleDbType.RefCursor);
                oOracleCommand.Parameters[0].Value = sql;
                oOracleCommand.Parameters[1].Direction = ParameterDirection.Output;
                OracleDataReader DataReader = oOracleCommand.ExecuteReader();
                oOracleCommand.Dispose();
                return DataReader;
            }
            catch (Exception oEX)
            {
                //throw new Exception(oEX.Message);
                Gerais.Enviar_Email_Para_Administradores("Open QC11PRD1.GET_QCALM11SQL_V2", oEX.Message.ToString() + "<br/><br/>" + sql);
                return null;
            }
        }

        private List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();

            if (dr == null)
                return list;

            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();

                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        public string Get_String(string sql)
        {
            try
            {
                OracleCommand oOracleCommand = new OracleCommand("QC11PRD1.GET_QCALM11SQL_V2", OracleConnection);
                oOracleCommand.CommandType = CommandType.StoredProcedure;
                oOracleCommand.Parameters.Add(new OracleParameter("QCALM11SQL", OracleDbType.Clob));
                oOracleCommand.Parameters.Add("RS", OracleDbType.RefCursor);
                oOracleCommand.Parameters[0].Value = sql;
                oOracleCommand.Parameters[1].Direction = ParameterDirection.Output;
                OracleDataReader DataReader = oOracleCommand.ExecuteReader();
                oOracleCommand.Dispose();

                DataReader.Read();

                return DataReader["Valor"].ToString();
            }
            catch (Exception oEX)
            {
                //throw new Exception(oEX.Message);
                Gerais.Enviar_Email_Para_Administradores("Open QC11PRD1.GET_QCALM11SQL_V2", oEX.Message.ToString());
                return null;
            }
        }
        public string Get_String_(string sql)
        {
            try
            {
                OracleCommand oOracleCommand = new OracleCommand(sql, OracleConnection);
                OracleDataReader DataReader = oOracleCommand.ExecuteReader();
                oOracleCommand.Dispose();

                DataReader.Read();

                return DataReader[0].ToString();
            }
            catch (Exception oEX)
            {
                //throw new Exception(oEX.Message);
                Gerais.Enviar_Email_Para_Administradores("Get_String_", oEX.Message.ToString());
                return null;
            }
        }

    }
}

//select 
//    'insert into ALM_Defeitos (Subprojeto,Entrega,Defeito,Sistema,Sistema_Defeito,Detectado_Por,Nome,Fase,Etapa,Severidade,Prioridade,Status_Atual,CT,Dt_Inicial,Dt_Final,Dt_Ultimo_Status,SLA,Agente_Atual,Natureza,Origem,Qtd_Reopen,Qtd_Reincidencia,Ja_Foi_Rejeitado,Motivo_Pendencia,Qtd_CTs_Impactados,Dt_Alteracao) select ' || 
//    '''' || NVL('PRJ00000591','') || ''',' || 
//    '''' || NVL('ENTREGA00000245','') || ''',' || 
//    cast(NVL(Def.bg_bug_id,0) as varchar(30)) || ',' || 
//    '''' || NVL(upper(Def.bg_user_template_03),'') || ''',' || 
//    '''' || NVL(upper(Def.bg_user_template_02),'') || ''',' || 
//    '''' || NVL(Def.bg_detected_by,'') || ''',' || 
//    '''' || NVL(upper(replace(Def.bg_summary,'''','')),'') || ''',' || 
//    '''' || NVL(replace(upper((select r.rel_name from solicitacao_83728_db.release_cycles rc, solicitacao_83728_db.releases r where rc.rcyc_id = Def.bg_detected_in_rcyc and r.rel_id = rc.rcyc_parent_id)),'''',''),'') || ''',' || 
//    '''' || NVL(upper((select rc.rcyc_name from solicitacao_83728_db.release_cycles rc where rc.rcyc_id = Def.bg_detected_in_rcyc)),'') || ''',' || 
//    '''' || NVL(upper(Def.bg_severity),'') || ''',' || 
//    '''' || NVL(upper(Def.bg_priority),'') || ''',' || 
//    '''' || NVL(upper(Def.bg_status),'') || ''',' || 
//    cast(NVL((select (case when ln_entity_type = 'TESTCYCL' then (select TC.tc_testcycl_id from solicitacao_83728_db.testcycl TC where TC.tc_testcycl_id = L.ln_entity_id) when ln_entity_type = 'RUN' then (select R.rn_testcycl_id from solicitacao_83728_db.run R where R.rn_run_id = L.ln_entity_id) when ln_entity_type = 'STEP' then (select R.rn_testcycl_id from solicitacao_83728_db.run R where R.rn_run_id = (select S.st_run_id from solicitacao_83728_db.step S where S.st_id = L.ln_entity_id)) end) from solicitacao_83728_db.Link L where L.ln_bug_id = Def.bg_bug_id and rownum=1),0) as varchar(30)) || ',' || 
//    '''' || NVL(to_char(Dt_Inicial_Bug('solicitacao_83728_db', Def.bg_bug_id),'dd-mm-yy hh24:mi:ss'),'') || ''',' || '''' || NVL(to_char(Dt_Final_Bug('solicitacao_83728_db', Def.bg_bug_id, Def.bg_status),'dd-mm-yy hh24:mi:ss'),'') || ''',' || '''' || NVL(to_char(Dt_Ultimo_Status_Bug('solicitacao_83728_db', Def.bg_bug_id, Def.bg_status),'dd-mm-yy hh24:mi:ss'),'') || ''',' || cast(NVL(SLA(Def.bg_severity),0) as varchar(30)) || ',' || '''' || NVL(upper(Def.bg_user_template_01),'') || ''',' || '''' || NVL(upper(Def.bg_user_template_01),'') || ''',' || '''' || NVL(upper(Def.bg_user_template_05),'') || ''',' || cast(NVL(0,0) as varchar(30)) || ',' || cast(NVL('(case ISNUMERIC(''' || '0' || Def.bg_user_template_07 || ''') when 1 then ''0' || Def.bg_user_template_07 || ''' else 0 end),0) as varchar(30)) || ',' || '''' || NVL(upper(Def.bg_user_template_08),'') || ''',' || '''' || NVL(upper(Def.bg_user_template_10),'') || ''',' || cast(NVL(Def.bg_user_template_11,0) as varchar(30)) || ',' || '''' || NVL((select to_char(max(AU_TIME),'dd-mm-yy hh24:mi:ss') from solicitacao_83728_db.AUDIT_LOG where (AU_ENTITY_TYPE = 'BUG') and (au_action = 'UPDATE') and (AU_ENTITY_ID = to_char(Def.bg_bug_id))),'') || '''' || ' where not exists (select 1 from ALM_Defeitos x where x.Subprojeto= ''' || 'PRJ00000591' || ''' and x.Entrega= ''' || 'ENTREGA00000245' || ''' and x.Defeito=' || Def.bg_bug_id || ')' as Conteudo from solicitacao_83728_db.Bug Def 

//public DataTable Executar(string sql)
//{
//    OracleCommand cmInserts = new OracleCommand("QC11PRD1.GET_QCALM11SQL_V2", OracleConnection);
//    cmInserts.CommandType = CommandType.StoredProcedure;
//    cmInserts.Parameters.Add(new OracleParameter("QCALM11SQL", OracleDbType.Clob));
//    cmInserts.Parameters.Add("RS", OracleDbType.RefCursor);
//    cmInserts.Parameters[0].Value = sql;
//    cmInserts.Parameters[1].Direction = ParameterDirection.Output;

//    OracleDataAdapter DA = new OracleDataAdapter(cmInserts);
//    DataTable DT = new DataTable();
//    DA.Fill(DT);

//    return DT;
//}