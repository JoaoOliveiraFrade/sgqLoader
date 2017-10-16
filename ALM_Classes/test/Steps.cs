using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.alm
{
    public class Steps
    {

        public Projeto projeto { get; set; }

        public List<Field> fields { get; set; }

        public TypeUpdate typeUpdate { get; set; }

        public alm.Database database { get; set; }

        public SqlMaker2Param sqlMaker2Param { get; set; }

        public Steps(Projeto projeto, TypeUpdate typeUpdate, alm.Database database) {
            this.projeto = projeto;
            this.typeUpdate = typeUpdate;
            this.database = database;

            sqlMaker2Param = new SqlMaker2Param();
            
            sqlMaker2Param.fields = new List<Field>();

            sqlMaker2Param.fields.Add(new Field() { target = "Subprojeto", source = "'{Subprojeto}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Entrega", source = "'{Entrega}'", type = "A", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Step", source = "ds_id", type = "N", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Teste", source = "ds_test_id", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Ordem", source = "ds_step_order", type = "N" });
            sqlMaker2Param.fields.Add(new Field() { target = "Nome", source = "upper(replace(trim(ds_step_name),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Descricao", source = "upper(replace(trim(ds_description),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Resultado_Esperado", source = "upper(replace(trim(DS_expected),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Tem_Paramentro", source = "upper(ds_has_params)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Alteracao", source = "substr(ds_vts,9,2) || '-' || substr(ds_vts,6,2) || '-' || substr(ds_vts,3,2) || ' ' || substr(ds_vts,12,8)" });

            sqlMaker2Param.dataSource = @"{Esquema}.DesSteps st";

            sqlMaker2Param.dataSourceFilterCondition =
                @"exists(select distinct 1 
                        from {Esquema}.testcycl tc 
                        where tc.tc_test_id = st.ds_test_id)";

            sqlMaker2Param.targetTable = "alm_steps";

            this.sqlMaker2Param.targetSqlLastIdInserted = $"select max(step) from alm_Steps where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";
            this.sqlMaker2Param.targetSqlLastDateUpdate = $"select Steps_Incremental_Inicio from alm_projetos where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";

            this.sqlMaker2Param.dataSourceFilterConditionInsert = $" ds_id > {this.sqlMaker2Param.targetLastIdInserted}";
            this.sqlMaker2Param.dataSourceFilterConditionUpdate = $" substr(st.ds_vts,3,17) > '{this.sqlMaker2Param.targetLastDateUpdate}'";

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
                        set Steps_Incremental_Inicio='00-00-00 00:00:00',
                            Steps_Incremental_Fim='00-00-00 00:00:00',
                            Steps_Incremental_Tempo=0
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
                    set Steps_Incremental_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Steps_Incremental_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Steps_Incremental_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");

            } else if (typeUpdate == TypeUpdate.Full) {
                SGQConn.Executar("delete alm_steps where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'");

                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }

                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Steps_Completa_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Steps_Completa_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Steps_Completa_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
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
        //            SGQConn.Get_String(string.Format("select max(Step) from ALM_Steps where Subprojeto='{0}' and Entrega='{1}'", projeto.Subprojeto, projeto.Entrega));
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.targetLastIdInserted == "" || sqlMaker2Param.targetLastIdInserted == null)
        //            sqlMaker2Param.targetLastIdInserted = "0";

        //        sqlMaker2Param.dataSourceFilterConditionInsert = " st.ds_id > " + sqlMaker2Param.targetLastIdInserted;
        //    }
        //}
        //public void Carregar_Condicoes_Update()
        //{
        //    if (this.typeUpdate == TypeUpdate.Full)
        //        sqlMaker2Param.dataSourceFilterConditionUpdate = "";
        //    else
        //    {
        //        Connection SGQConn = new Connection();
        //        sqlMaker2Param.Ultima_Atualizacao = SGQConn.Get_String_Por_Id("ALM_Projetos", "Steps_Incremental_Inicio", projeto.Id.ToString());
        //        SGQConn.Dispose();

        //        if (sqlMaker2Param.Ultima_Atualizacao == "" || sqlMaker2Param.Ultima_Atualizacao == null)
        //            sqlMaker2Param.Ultima_Atualizacao = "00-00-00 00:00:00";
        //        else
        //            sqlMaker2Param.Ultima_Atualizacao = sqlMaker2Param.Ultima_Atualizacao.Substring(6, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(3, 2) + "-" + sqlMaker2Param.Ultima_Atualizacao.Substring(0, 2) + " " + sqlMaker2Param.Ultima_Atualizacao.Substring(9, 8);

        //        sqlMaker2Param.dataSourceFilterConditionUpdate = " substr(st.ds_vts,3,17) > '" + sqlMaker2Param.Ultima_Atualizacao + "'";
        //    }
        //}

    }
}

