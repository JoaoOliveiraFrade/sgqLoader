using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.alm
{
    public class Tests
    {
        public Projeto projeto { get; set; }

        public List<Field> fields { get; set; }

        public TypeUpdate typeUpdate { get; set; }

        public alm.Database database { get; set; }

        public SqlMaker2Param sqlMaker2Param { get; set; }

        public Tests(Projeto projeto, TypeUpdate typeUpdate, alm.Database database)
        {
            this.projeto = projeto;
            this.typeUpdate = typeUpdate;
            this.database = database;

            sqlMaker2Param = new SqlMaker2Param();
            
            sqlMaker2Param.fields = new List<Field>();

            sqlMaker2Param.fields.Add(new Field() { target = "Subprojeto", source = "'{Subprojeto}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Entrega", source = "'{Entrega}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Teste", source = "ts_test_id", type = "N", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Nome", source = "upper(replace(trim(ts_name),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Descricao", source = "upper(replace(trim(ts_description),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Qt_Steps", source = "ts_steps" });
            sqlMaker2Param.fields.Add(new Field() { target = "Path", source = "replace(substr(PTH,14),'''','')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Status", source = "upper(replace(ts_status,'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Status_Execucao", source = "upper(replace(ts_exec_status,'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Resposavel", source = "upper(ts_responsible)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Criacao", source = "to_char(ts_creation_Date,'dd-mm-yy')" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Alteracao", source = "substr(ts_vts,9,2) || '-' || substr(ts_vts,6,2) || '-' || substr(ts_vts,3,2) || ' ' || substr(ts_vts,12,8)" });

            if (this.database.name == "ALM11") {
                sqlMaker2Param.fields.Add(new Field() { target = "Sistema", source = "upper(replace(ts_user_template_01,'''',''))" });
                sqlMaker2Param.fields.Add(new Field() { target = "Macrocenario", source = "upper(ts_user_template_02)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Cenario_Teste", source = "upper(ts_user_template_03)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Fornecedor", source = "upper(ts_user_template_05)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Tipo_Requisito", source = "upper(ts_user_template_06)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Qt_Sistemas", source = "ts_user_template_08", type = "N" });
                sqlMaker2Param.fields.Add(new Field() { target = "CT_Automatizado", source = "upper(ts_user_template_15)" });
                sqlMaker2Param.fields.Add(new Field() { target = "TRG", source = "upper(ts_user_template_16)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Prioridade_Execucao", source = "upper(ts_user_template_20)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Sistemas_Afetados", source = "upper(ts_user_template_22)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Complexidade_Accenture", source = "upper(ts_user_template_24)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Requisito", source = "upper(ts_user_template_26)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Modulo", source = "upper(ts_user_template_27)" });
                sqlMaker2Param.fields.Add(new Field() { target = "Complexidade_Link", source = "upper(ts_user_template_28)" });
            }

            sqlMaker2Param.dataSource =
                    @"(select
                         al_item_id, sys_connect_by_path(al_description, ' \ ') PTH
                       from
                         {Esquema}.all_lists connect by prior al_item_id = al_father_id
                         start with al_father_id = 0 and al_description = 'Subject'
                      ) x
                      inner join {Esquema}.test t 
                        on t.ts_subject = x.al_item_id";

            sqlMaker2Param.dataSourceFilterCondition =
                    @"exists(select distinct 1 
                        from {Esquema}.testcycl tc 
                        where tc.tc_test_id = t.ts_test_id)";

            sqlMaker2Param.targetTable = "ALM_Testes";

            this.sqlMaker2Param.targetSqlLastIdInserted = $"select max(teste) from alm_Testes where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";
            this.sqlMaker2Param.targetSqlLastDateUpdate = $"select Testes_Incremental_Inicio from alm_projetos where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";

            this.sqlMaker2Param.dataSourceFilterConditionInsert = $" ts_test_id > {this.sqlMaker2Param.targetLastIdInserted}";
            this.sqlMaker2Param.dataSourceFilterConditionUpdate = $" substr(ts_vts,3,17) > '{this.sqlMaker2Param.targetLastDateUpdate}'";

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
                        set Testes_Incremental_Inicio='00-00-00 00:00:00',
                            Testes_Incremental_Fim='00-00-00 00:00:00',
                            Testes_Incremental_Tempo=0
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

                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Testes_Incremental_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Testes_Incremental_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Testes_Incremental_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");

            } else if (typeUpdate == TypeUpdate.Full) {
                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar("delete alm_testes where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'");
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }

                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Testes_Completa_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Testes_Completa_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Testes_Completa_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
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
        //            SGQConn.Get_String(string.Format("select max(Teste) from ALM_Testes where Subprojeto='{0}' and Entrega='{1}'", projeto.Subprojeto, projeto.Entrega));
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.targetLastIdInserted == "" || sqlMaker2Param.targetLastIdInserted == null)
        //            sqlMaker2Param.targetLastIdInserted = "0";

        //        sqlMaker2Param.dataSourceFilterConditionInsert = " t.ts_test_id > " + sqlMaker2Param.targetLastIdInserted;
        //    }
        //}
        //public void Carregar_Condicoes_Update()
        //{
        //    if (this.typeUpdate == TypeUpdate.Full)
        //        sqlMaker2Param.dataSourceFilterConditionUpdate = "";
        //    else
        //    {
        //        Connection SGQConn = new Connection();
        //        sqlMaker2Param.Ultima_Atualizacao = SGQConn.Get_String_Por_Id("ALM_Projetos", "Testes_Incremental_Inicio", projeto.Id.ToString());
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.Ultima_Atualizacao == "" || sqlMaker2Param.Ultima_Atualizacao == null)
        //            sqlMaker2Param.Ultima_Atualizacao = "00-00-00 00:00:00";
        //        else
        //            sqlMaker2Param.Ultima_Atualizacao = sqlMaker2Param.Ultima_Atualizacao.Substring(6, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(3, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(0, 2) + " " + sqlMaker2Param.Ultima_Atualizacao.Substring(9, 8);

        //        sqlMaker2Param.dataSourceFilterConditionUpdate = " substr(t.TS_VTS,3,17) > '" + sqlMaker2Param.Ultima_Atualizacao + "'";
        //    }
        //}

    }
}

