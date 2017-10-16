using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.alm {
    public class Defeitos_Links {
        public Projeto projeto { get; set; }

        public List<Field> fields { get; set; }

        public TypeUpdate typeUpdate { get; set; }

        public alm.Database database { get; set; }

        public SqlMaker2Param sqlMaker2Param { get; set; }

        public Defeitos_Links(Projeto projeto, TypeUpdate typeUpdate, alm.Database database) {
            this.projeto = projeto;
            this.typeUpdate = typeUpdate;
            this.database = database;

            sqlMaker2Param = new SqlMaker2Param();
            sqlMaker2Param.fields = new List<Field>();

            this.sqlMaker2Param.fields.Add(new Field() { target = "Subprojeto", source = "'{Subprojeto}'", key = true });
            this.sqlMaker2Param.fields.Add(new Field() { target = "Entrega", source = "'{Entrega}'", key = true });
            this.sqlMaker2Param.fields.Add(new Field() { target = "Id", source = "LN_LINK_ID", type = "N", key = true });
            this.sqlMaker2Param.fields.Add(new Field() { target = "Defeito", source = "LN_BUG_ID", type = "N" });
            this.sqlMaker2Param.fields.Add(new Field() { target = "Tabela", source = "LN_ENTITY_TYPE" });
            this.sqlMaker2Param.fields.Add(new Field() { target = "Tabela_Id", source = "LN_ENTITY_ID" });
            this.sqlMaker2Param.fields.Add(new Field() { target = "Atualizador", source = "upper(LN_CREATED_BY)" });

            if (database.name == "ALM11") {
                this.sqlMaker2Param.fields.Add(new Field() { target = "Dt_Alteracao", source = "to_char(ln_creation_date,'dd-mm-yy')" });
            } else {
                this.sqlMaker2Param.fields.Add(new Field() { target = "Dt_Alteracao", source = "substr(ln_vts,9,2) || '-' || substr(ln_vts,6,2) || '-' || substr(ln_vts,3,2) || ' ' || substr(ln_vts,12,8)" });
            }

            this.sqlMaker2Param.dataSource = @"{Esquema}.link";
            this.sqlMaker2Param.dataSourceFilterCondition = "";
            this.sqlMaker2Param.targetTable = "alm_defeitos_links";

            this.sqlMaker2Param.targetSqlLastIdInserted = $"select max(id) from alm_defeitos_links where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";
            this.sqlMaker2Param.targetSqlLastDateUpdate = $"select Defeitos_Links_Incremental_Inicio from alm_projetos where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'";

            this.sqlMaker2Param.dataSourceFilterConditionInsert = $" ln_bug_id > {this.sqlMaker2Param.targetLastIdInserted}";
            this.sqlMaker2Param.dataSourceFilterConditionUpdate = $" to_char(ln_creation_date,'yy-mm-dd') > '{this.sqlMaker2Param.targetLastDateUpdate}'";

            this.sqlMaker2Param.typeDB = "ORACLE";
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
                        set Defeitos_Links_Incremental_Inicio='00-00-00 00:00:00',
                            Defeitos_Links_Incremental_Fim='00-00-00 00:00:00',
                            Defeitos_Links_Incremental_Tempo=0
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
                    set Defeitos_Links_Incremental_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Links_Incremental_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Links_Incremental_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");

            } else if (typeUpdate == TypeUpdate.Full) {
                SGQConn.Executar("delete ALM_Defeitos_Links where subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'");

                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", projeto.Esquema).Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }
                DateTime Dt_Fim = DateTime.Now;

                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Defeitos_Links_Completa_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Links_Completa_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Links_Completa_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");
            }

            SGQConn.Dispose();
        }
    }
}

