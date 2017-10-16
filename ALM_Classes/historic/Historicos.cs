using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.alm {
    public class Historicos {
        public Projeto projeto { get; set; }

        public List<Field> fields { get; set; }

        public TypeUpdate typeUpdate { get; set; }

        public alm.Database database { get; set; }

        public SqlMaker2Param sqlMaker2Param { get; set; }

        public Historicos(Projeto projeto, TypeUpdate typeUpdate, alm.Database database) {
            this.projeto = projeto;
            this.typeUpdate = typeUpdate;
            this.database = database;

            sqlMaker2Param = new SqlMaker2Param();

            sqlMaker2Param.fields = new List<Field>();

            sqlMaker2Param.fields.Add(new Field() { target = "Subprojeto", source = "'{Subprojeto}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Entrega", source = "'{Entrega}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Id", source = "ap_property_id", type = "N", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Tabela", source = "upper(AU_ENTITY_TYPE)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Tabela_Id", source = "AU_ENTITY_ID", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Campo", source = "upper(AP_PROPERTY_NAME)" });
            sqlMaker2Param.fields.Add(new Field() {
                target = "Novo_Valor",
                source =
                    @"replace(
                            case 
                                when AP_NEW_VALUE is not null then TO_CLOB(upper(AP_NEW_VALUE))
                                when AP_NEW_LONG_VALUE is not null then 
                                        TO_CLOB(DBMS_LOB.SUBSTR(AP_NEW_LONG_VALUE,4000,1)) ||
                                        TO_CLOB(DBMS_LOB.SUBSTR(AP_NEW_LONG_VALUE,4000,4001)) ||
                                        TO_CLOB(DBMS_LOB.SUBSTR(AP_NEW_LONG_VALUE,4000,8001)) ||
                                        TO_CLOB(DBMS_LOB.SUBSTR(AP_NEW_LONG_VALUE,4000,12001)) ||
                                        TO_CLOB(DBMS_LOB.SUBSTR(AP_NEW_LONG_VALUE,4000,16001)) ||
                                        TO_CLOB(DBMS_LOB.SUBSTR(AP_NEW_LONG_VALUE,4000,20001))
                                when AP_NEW_DATE_VALUE is not null then TO_CLOB(to_char(AP_NEW_DATE_VALUE ,'dd-mm-yy') )
                            end,
                '''','')"
            });
            sqlMaker2Param.fields.Add(new Field() { target = "Operador", source = "upper(au_user)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Alteracao", source = " to_char(au_time,'dd-mm-yy hh24:mi:ss')" });

            sqlMaker2Param.dataSource = @"{Esquema}.audit_properties inner join {Esquema}.audit_log on ap_action_id = au_action_id";
            sqlMaker2Param.dataSourceFilterCondition = @"upper(AU_ENTITY_TYPE) in ('TESTCYCL','BUG')";

            this.sqlMaker2Param.targetSqlLastIdInserted = $"select max(id) from alm_historico_alteracoesfields where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";
            this.sqlMaker2Param.targetSqlLastDateUpdate = $"select Historico_Incremental_Inicio from alm_projetos where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";

            this.sqlMaker2Param.dataSourceFilterConditionInsert = $" ap_property_id > {this.sqlMaker2Param.targetLastIdInserted}";
            this.sqlMaker2Param.dataSourceFilterConditionUpdate = $" to_char(au_time,'yy-mm-dd hh24:mi:ss') > '{this.sqlMaker2Param.targetLastDateUpdate}'";

            sqlMaker2Param.targetTable = "ALM_Historico_Alteracoesfields";

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
                        set Historico_Incremental_Inicio='00-00-00 00:00:00',
                            Historico_Incremental_Fim='00-00-00 00:00:00',
                            Historico_Incremental_Tempo=0
                        where 
                            subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                    ");
                }

                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }

                //string Sql_Update = sqlMaker2.Get_Oracle_Update().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                //OracleDataReader DataReader_Update = ALMConn.Get_DataReader(Sql_Update);
                //if (DataReader_Update != null && DataReader_Update.HasRows == true) {
                //    SGQConn.Executar(ref DataReader_Update, 1);
                //}

                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Historico_Incremental_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Historico_Incremental_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Historico_Incremental_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");

            } else if (typeUpdate == TypeUpdate.Full) {
                SGQConn.Executar($"delete ALM_Historico_Alteracoesfields where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'");

                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }

                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Historico_Completa_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Historico_Completa_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Historico_Completa_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
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
        //            SGQConn.Get_String(string.Format("Select Max(Id) from ALM_Historico_Alteracoesfields where Subprojeto='{0}' and Entrega='{1}'", projeto.Subprojeto, projeto.Entrega));
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.targetLastIdInserted == "" || sqlMaker2Param.targetLastIdInserted == null)
        //            sqlMaker2Param.targetLastIdInserted = "0";

        //        sqlMaker2Param.dataSourceFilterConditionInsert = " ap_property_id > " + sqlMaker2Param.targetLastIdInserted;
        //    }
        //}
        //public void Carregar_Condicoes_Update()
        //{
        //    if (this.typeUpdate == TypeUpdate.Full)
        //        sqlMaker2Param.dataSourceFilterConditionUpdate = "";
        //    else
        //    {
        //        Connection SGQConn = new Connection();
        //        sqlMaker2Param.Ultima_Atualizacao = SGQConn.Get_String_Por_Id("ALM_Projetos", "Historico_Incremental_Inicio", projeto.Id.ToString());
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.Ultima_Atualizacao == "" || sqlMaker2Param.Ultima_Atualizacao == null)
        //            sqlMaker2Param.Ultima_Atualizacao = "00-00-00 00:00:00";
        //        else
        //            sqlMaker2Param.Ultima_Atualizacao = sqlMaker2Param.Ultima_Atualizacao.Substring(6, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(3, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(0, 2) + " " + sqlMaker2Param.Ultima_Atualizacao.Substring(9, 8);

        //        sqlMaker2Param.dataSourceFilterConditionUpdate = " to_char(au_time,'yy-mm-dd hh24:mi:ss') > '" + sqlMaker2Param.Ultima_Atualizacao + "'";
        //    }
        //}
    }
}

