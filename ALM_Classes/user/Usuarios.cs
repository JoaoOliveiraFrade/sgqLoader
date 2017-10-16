using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Text;
// using TDAPIOLELib;
using sgq;
using sgq.alm;

namespace sgq.alm
{
    public class Usuarios
    {
        public Projeto projeto { get; set; }

        public List<Field> fields { get; set; }

        public TypeUpdate typeUpdate { get; set; }

        public alm.Database database { get; set; }

        public SqlMaker2Param sqlMaker2Param { get; set; }

        public Usuarios(alm.Database database, TypeUpdate typeUpdate = TypeUpdate.Increment) {
            this.database = database;
            this.typeUpdate = typeUpdate;

            sqlMaker2Param = new SqlMaker2Param();

            sqlMaker2Param.fields = new List<Field>();

            sqlMaker2Param.fields.Add(new Field() { target = "Dominio", source = $"'{database.dominio}'", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Id", source = "user_id", type = "N", key = true });
            sqlMaker2Param.fields.Add(new Field() { target = "Login", source = "upper(replace(trim(user_name),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Nome", source = "upper(replace(trim(full_name),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Email", source = "lower(replace(trim(email),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Telefone", source = "upper(replace(trim(replace(replace(phone_number,'--','-'),' ','')),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Descricao", source = "upper(replace(trim(description),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Sistema", source = "upper(replace(trim(us_is_system),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Fornecedor", source = "upper(replace(trim(description),'''',''))" });
            sqlMaker2Param.fields.Add(new Field() { target = "Ativo", source = "upper(US_IS_ACTIVE)" });
            sqlMaker2Param.fields.Add(new Field() { target = "Dt_Alteracao", source = "to_char(last_update,'dd-mm-yy hh24:mi:ss')" });

            sqlMaker2Param.dataSource = "{Esquema}.users";

            sqlMaker2Param.dataSourceFilterCondition = "user_id is not null";
            sqlMaker2Param.targetTable = "ALM_Usuarios";

            this.sqlMaker2Param.targetSqlLastIdInserted = "select max(Id) from ALM_Usuarios";
            this.sqlMaker2Param.targetSqlLastDateUpdate = "select Valor from SGQ_Parametros where Nome = 'ALM_Usuarios_Update'";

            this.sqlMaker2Param.dataSourceFilterConditionInsert = $" user_id > {this.sqlMaker2Param.targetLastIdInserted}";
            this.sqlMaker2Param.dataSourceFilterConditionUpdate = $" to_char(last_update,'yy-mm-dd hh24:mi:ss') > '{this.sqlMaker2Param.targetLastDateUpdate}'";

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
                    SGQConn.Executar($"update SGQ_Parametros set Valor = '0000-00-00 00:00:00' where Nome='ALM_Usuarios_Update'");
                }

                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", this.database.scheme);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }

                string Sql_Update = sqlMaker2.Get_Oracle_Update().Replace("{Esquema}", this.database.scheme);
                OracleDataReader DataReader_Update = ALMConn.Get_DataReader(Sql_Update);
                if (DataReader_Update != null && DataReader_Update.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Update, 1);
                }

            } else if (typeUpdate == TypeUpdate.Full) {
                SGQConn.Executar($"truncate table ALM_Usuarios");

                string Sql_Insert = sqlMaker2.Get_Oracle_Insert().Replace("{Esquema}", this.database.scheme);
                OracleDataReader DataReader_Insert = ALMConn.Get_DataReader(Sql_Insert);
                if (DataReader_Insert != null && DataReader_Insert.HasRows == true) {
                    SGQConn.Executar(ref DataReader_Insert, 1);
                }
            }

            SGQConn.Executar($"update SGQ_Parametros set Valor = '{Dt_Inicio.ToString("dddd-MM-yy HH:mm:ss")}' where Nome='ALM_Usuarios_Update'");

            Gerais.Enviar_Email_Atualizacao_Tabela(
                Assunto: string.Format($"[SGQLoader]{database.name} - Usuários - {this.typeUpdate}"),
                Dt_Inicio: Dt_Inicio,
                Dt_Fim: DateTime.Now
            );
            
            SGQConn.Dispose();
        }
    }
}
