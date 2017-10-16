using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Data.Odbc;

namespace sgq
{
    public class BDOdbc
    {
        private OdbcConnection cnn;
        private string cnnString;

        public BDOdbc(string BD)
        {
            if (BD == "ACC")
                cnnString = "Dsn=Base Histórica - Accenture;uid=oi;pwd=oiprdoi";
            else
                cnnString = "Dsn=Base Histórica - IBM;uid=UsrBDSISRAF;pwd=FabricaSISRAF";

            Open();
        }

        public void Open()
        {
            cnn = new OdbcConnection(cnnString);
            try
            {
                cnn.Open();
            }
            catch (Exception oEX)
            {
                throw new Exception(oEX.Message);
            }
        }

        public void Close()
        {
            if (cnn.State == ConnectionState.Open)
                cnn.Close();
        }

        public void RetornaHashtable(ref OdbcCommand cmd, ref Hashtable oHashtable)
        {
            this.Open();
            cmd.Connection = cnn;
             
            OdbcDataReader dr;
            try
            {
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oHashtable.Add(dr[0].ToString(), dr[1].ToString());
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                cmd.Dispose();
                this.Close();
            }
        }

        public void RetornaHashtable(string sql, ref Hashtable oHashtable)
        {
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandText = sql;
            RetornaHashtable(ref cmd, ref oHashtable);
        }

        public void RetornaArrayList(ref OdbcCommand cmd, ref ArrayList Array)
        {
            this.Open();
            cmd.Connection = cnn;
            OdbcDataReader dr;
            try
            {
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Array.Add(dr[0].ToString());
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                cmd.Dispose();
                this.Close();
            }
        }

        public void RetornaArrayList(string sql, ref ArrayList Array)
        {
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandText = sql;
            RetornaArrayList(ref cmd, ref Array);
        }

        public void RetornaString(ref OdbcCommand cmd, ref string returnString)
        {
            this.Open();
            cmd.Connection = cnn;
            OdbcDataReader dr;
            try
            {
                dr = cmd.ExecuteReader();
                if (dr.Read())
                    returnString = dr[0].ToString();

                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                cmd.Dispose();
                this.Close();
            }
        }

        public void RetornaString(string sql, ref string returnString)
        {
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandText = sql;
            RetornaString(ref cmd, ref returnString);
        }

        public void RetornaLong(ref OdbcCommand cmd, ref long VarLong)
        {
            string VarString = "";
            RetornaString(ref cmd, ref VarString);
            if (!VarString.Equals(""))
                VarLong = Convert.ToInt64(VarString);
        }

        public void RetornaLong(string sql, ref long VarLong)
        {
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandText = sql;
            RetornaLong(ref cmd, ref VarLong);
        }

        public long RetornaLong(string sql)
        {
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandText = sql;

            string VarString = "";

            RetornaString(ref cmd, ref VarString);
            if (!VarString.Equals(""))
                return Convert.ToInt64(VarString);
            else
                return 0;
        }

        public void RetornaDataTable(ref OdbcCommand cmd, ref DataTable returnDataTable)
        {
            this.Open();
            cmd.Connection = cnn;
            OdbcDataAdapter da = new OdbcDataAdapter(cmd);
            
            try
            {
                da.Fill(returnDataTable);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                cmd.Dispose();
                da.Dispose();
                this.Close();
            }
        }

        public void RetornaDataTable(string sql, ref DataTable returnDataTable)
        {
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandText = sql;
            RetornaDataTable(ref cmd, ref returnDataTable);
        }

        public void RetornaDataSet(ref OdbcCommand cmd, ref DataSet returnDataSet, string nameTableDataSet)
        {
            this.Open();
            cmd.Connection = cnn;

            OdbcDataAdapter da = new OdbcDataAdapter(cmd);

            try
            {
                da.Fill(returnDataSet, nameTableDataSet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                cmd.Dispose();
                da.Dispose();
                this.Close();
            }
        }

        public void RetornaDataSet(string sql, ref DataSet returnDataSet, string nameTableDataSet)
        {
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandText = sql;
            RetornaDataSet(ref cmd, ref returnDataSet, nameTableDataSet);
        }

        public void Executa(string sql)
        {
            this.Open();
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                cmd.Connection = cnn;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                cmd.Dispose();
                this.Close();
            }
        }

        public bool Achou(string sql)
        {
            string Retorno = "";
            RetornaString(sql, ref Retorno);
            return Retorno != "" ? true : false;
        }

        public string ID(string Tabela, string Condicao, string IDRegAtual)
        {
            string ID = "";

            if (IDRegAtual != "0")
                this.RetornaString("select ID from " + Tabela + " where " + Condicao + " and ID <> " + IDRegAtual, ref ID);
            else
                this.RetornaString("select ID from " + Tabela + " where " + Condicao, ref ID);

            return ID;
        }
    }
}
