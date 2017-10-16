using System;
using System.Data;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Collections.Generic;
using System.Reflection;
using sgq;

namespace sgq.alm
{
    public class ALMConnection : IDisposable {
        private OracleConnection oracleConnection;
        private alm.Database database;
        private string storedProcedureName = "GET_QCALM11SQL_V2";
        private string inputParameterName = "QCALM11SQL";
        private string outputParameterName = "RS";

        // deve sair
        public ALMConnection(string Schema)
        {
            var ConnectionString = $"Data Source=btdf5377.brasiltelecom.com.br:1530/QC11PRD1;Persist Security Info=True;User ID={Schema};Password=tdtdtd";
            oracleConnection = new OracleConnection(ConnectionString);
            oracleConnection.Open();
        }
        //--------------

        public ALMConnection(alm.Database database) {
            this.database = database;
            oracleConnection = new OracleConnection(this.database.connectionString);
            oracleConnection.Open();
        }

        public List<T> Executar<T>(string sql)
        {
            OracleDataReader DataReader = this.Get_DataReader(sql);
            List<T> Lista = this.DataReaderMapToList<T>(DataReader);

            if (DataReader != null)
                DataReader.Dispose();

            return Lista;
        }

        public void Dispose() {
            if (oracleConnection.State == ConnectionState.Open) {
                oracleConnection.Close();
            }
        }

        public OracleDataReader Get_DataReader(string sql)
        {
            try
            {
                OracleCommand oracleCommand = new OracleCommand(this.database.prefix + this.storedProcedureName, this.oracleConnection);
                oracleCommand.CommandType = CommandType.StoredProcedure;
                oracleCommand.Parameters.Add(new OracleParameter(this.inputParameterName, OracleDbType.Clob));
                oracleCommand.Parameters.Add(this.outputParameterName, OracleDbType.RefCursor);
                oracleCommand.Parameters[0].Value = sql;
                oracleCommand.Parameters[1].Direction = ParameterDirection.Output;
                OracleDataReader DataReader = oracleCommand.ExecuteReader();
                oracleCommand.Dispose();
                return DataReader;
            }
            catch (Exception oEX)
            {
                //throw new Exception(oEX.Message);
                Gerais.Enviar_Email_Para_Administradores(this.database.name + ": " + this.database.prefix + this.storedProcedureName, oEX.Message.ToString() + "<br/><br/>" + sql);
                // Gerais.Enviar_Email_Para_Administradores("Open GET_QCALM11SQL_V2", oEX.Message.ToString() + "<br/><br/>" + sql);
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
                OracleCommand oracleCommand = new OracleCommand(this.database.prefix + this.storedProcedureName, this.oracleConnection);
                oracleCommand.CommandType = CommandType.StoredProcedure;
                oracleCommand.Parameters.Add(new OracleParameter(this.inputParameterName, OracleDbType.Clob));
                oracleCommand.Parameters.Add(this.outputParameterName, OracleDbType.RefCursor);
                oracleCommand.Parameters[0].Value = sql;
                oracleCommand.Parameters[1].Direction = ParameterDirection.Output;
                OracleDataReader DataReader = oracleCommand.ExecuteReader();
                oracleCommand.Dispose();

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
                OracleCommand oracleCommand = new OracleCommand(sql, this.oracleConnection);
                OracleDataReader DataReader = oracleCommand.ExecuteReader();
                oracleCommand.Dispose();

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
