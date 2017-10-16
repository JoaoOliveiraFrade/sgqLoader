using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using Oracle.DataAccess.Client;

namespace sgq
{
    public class BDOracle
    {
        private string _StringConexao = "";
        private OracleConnection _Conexao = null;

        public BDOracle(string StringConexao)
        {
            _StringConexao = StringConexao;
            Open();
        }

        public void Open()
        {
            if (_Conexao != null)
            {
                if (_Conexao.State == ConnectionState.Open) return;
            }
            else
            {
                _Conexao = new OracleConnection(_StringConexao);
            }

            try
            {
                _Conexao.Open();
            }
            catch (Exception oEX)
            {
                //throw new Exception(oEX.Message);
                Gerais.Enviar_Email_Para_Administradores("Open " + _StringConexao, oEX.Message.ToString());

            }
        }

        public OracleConnection GetConexao()
        {
            return _Conexao;
        } 


        public void Close()
        {
            if (_Conexao.State == ConnectionState.Open)
                _Conexao.Close();
        }

        private void VerificarConexao(IDbCommand cmd)
        {
            if ((_Conexao == null))
            {
                _Conexao = new OracleConnection(_StringConexao);
                _Conexao.Open();
            }
            else if ((_Conexao.State != ConnectionState.Open))
            {
                _Conexao.Open();
            }
            cmd.Connection = _Conexao;
            return;
        }

        public OracleCommand CriarComando(string cmdText, CommandType cmdType, params IDbDataParameter[] parameters)
        {

            OracleCommand cmd = new OracleCommand(cmdText);
            

            cmd.CommandType = cmdType;

            if ((parameters != null))
            {
                foreach (OracleParameter param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            }
            VerificarConexao(cmd);
            return cmd;
        } 


        public int ExecuteNonQuery(string cmdText, CommandType cmdType, params IDbDataParameter[] parameters)
        {
            int Retorno = 0;
            IDbCommand cmd = CriarComando(cmdText, cmdType, parameters);
            try
            {
                Retorno = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Retorno = -1;
                //throw new Exception(ex.ToString());
                Gerais.Enviar_Email_Para_Administradores("ExecuteNonQuery " + cmdText.ToString(), ex.ToString());

            }
            finally
            {
                cmd.Dispose();
                this.Close();
            }
            return Retorno;
        }
        public int ExecuteNonQuery(string cmdText, CommandType cmdType)
        {
            return ExecuteNonQuery(cmdText, cmdType, null);
        }


        public void RetornaDataReader(ref OracleCommand cmd, ref OracleDataReader oOracleDataReader)
        {
            this.Open();
            cmd.Connection = _Conexao;

            try
            {
                oOracleDataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                Gerais.Enviar_Email_Para_Administradores("RetornaDataReader " + cmd.CommandText, ex.ToString());
            }
            //finally
            //{
            //    cmd.Dispose();
            //    this.Close();
            //}
        }
        public void RetornaDataReader(string sql, ref OracleDataReader oOracleDataReader)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;
            RetornaDataReader(ref cmd, ref oOracleDataReader);
        }

        public void RetornaHashtable(ref OracleCommand cmd, ref Hashtable oHashtable)
        {
            this.Open();
            cmd.Connection = _Conexao;

            OracleDataReader dr;
            
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
                //throw new Exception(ex.ToString());
                Gerais.Enviar_Email_Para_Administradores("RetornaHashtable " + cmd.CommandText, ex.ToString());
            }
            finally
            {
                cmd.Dispose();
                this.Close();
            }
        }
        public void RetornaHashtable(string sql, ref Hashtable oHashtable)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;
            RetornaHashtable(ref cmd, ref oHashtable);
        }

        public void RetornaArrayList(ref OracleCommand cmd, ref ArrayList Array)
        {
            this.Open();
            cmd.Connection = _Conexao;
            OracleDataReader dr;
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
                //throw new Exception(ex.ToString());
                Gerais.Enviar_Email_Para_Administradores("RetornaArrayList " + cmd.CommandText, ex.ToString());
            }
            finally
            {
                cmd.Dispose();
                this.Close();
            }
        }
        public void RetornaArrayList(string sql, ref ArrayList Array)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;
            RetornaArrayList(ref cmd, ref Array);
        }

        public void RetornaString(ref OracleCommand cmd, ref string returnString)
        {
            this.Open();
            cmd.Connection = _Conexao;
            OracleDataReader dr;
            try
            {
                dr = cmd.ExecuteReader();
                if (dr.Read())
                    returnString = dr[0].ToString();

                dr.Close();
                dr.Dispose();
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                Gerais.Enviar_Email_Para_Administradores("RetornaString " + cmd.CommandText, ex.ToString());
            }
            finally
            {
                cmd.Dispose();
            }
        }
        public void RetornaString(string sql, ref string returnString)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;
            RetornaString(ref cmd, ref returnString);
        }
        public string RetornaString(string sql)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;

            string VarString = "";

            RetornaString(ref cmd, ref VarString);
            if (!VarString.Equals(""))
                return VarString;
            else
                return "";
        }

        public void RetornaLong(ref OracleCommand cmd, ref long VarLong)
        {
            string VarString = "";
            RetornaString(ref cmd, ref VarString);
            if (!VarString.Equals(""))
                VarLong = Convert.ToInt64(VarString);
        }

        public void RetornaLong(string sql, ref long VarLong)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;
            RetornaLong(ref cmd, ref VarLong);
        }

        public long RetornaLong(string sql)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;

            string VarString = "";

            RetornaString(ref cmd, ref VarString);
            if (!VarString.Equals(""))
                return Convert.ToInt64(VarString);
            else
                return 0;
        }

        public void RetornaDataTable(ref OracleCommand cmd, ref DataTable returnDataTable)
        {
            this.Open();
            cmd.Connection = _Conexao;
            OracleDataAdapter da = new OracleDataAdapter(cmd);
        
            try
            {
                da.Fill(returnDataTable);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                Gerais.Enviar_Email_Para_Administradores("RetornaDataTable " + cmd.CommandText, ex.ToString());
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
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;
            RetornaDataTable(ref cmd, ref returnDataTable);
        }

        public void RetornaDataSet(ref OracleCommand cmd, ref DataSet returnDataSet, string nameTableDataSet)
        {
            this.Open();
            cmd.Connection = _Conexao;

            OracleDataAdapter da = new OracleDataAdapter(cmd);

            try
            {
                da.Fill(returnDataSet, nameTableDataSet);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                Gerais.Enviar_Email_Para_Administradores("RetornaDataSet " + cmd.CommandText, ex.ToString());
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
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;
            RetornaDataSet(ref cmd, ref returnDataSet, nameTableDataSet);
        }

        public bool Executa(string sql)
        {
             return ExecuteNonQuery(sql, CommandType.Text) != -1;
        }

        public bool IncluirRegistro(string Tabela, string Campos, string Valores)
        {   
            return this.Executa("INSERT INTO " + Tabela + "(" + Campos + ") VALUES (" + Valores + ")");
        }

        public bool AlterarRegistro(string Tabela, string ID, string Campos, string Valores)
        {
            string[] vCampos = Campos.Split(',');
            string[] vValores = Valores.Split(',');
            string StringtSet = "";
            for(int i = 0; i < vCampos.GetLength(0);i++)
            {
                StringtSet += vCampos[i] + "=" + vValores[i] + ", ";
            }
            int TamanhoStringtSet = StringtSet.Length;
            if (TamanhoStringtSet > 0)
                StringtSet = StringtSet.Substring(0, TamanhoStringtSet - 2);

            return this.Executa("UPDATE " + Tabela + " SET " + StringtSet + " where ID=" + ID);
        }

        public bool ExcluirRegistro(string Tabela, string ID)
        {
            return this.Executa("DELETE " + Tabela + " where ID=" + ID);
        }

        public string LerCampoID(string Tabela, string NomeCampo, string ID)
        {
            return this.RetornaString("select " + NomeCampo + " from " + Tabela + " where ID=" + ID);
        }

        public string LerCampoCondicao(string Tabela, string NomeCampo, string Condicao)
        {
            return this.RetornaString("select " + NomeCampo + " from " + Tabela + " where " + Condicao);
        }

        public void GravarCampo(string Tabela, string Criterio, string NomeCampo, string NovoConteudo)
        {
            this.Executa("UPDATE " + Tabela + " SET " + NomeCampo + " = " + NovoConteudo + " where " + Criterio);
        }

        public string UltimoID(string Tabela)
        {
            return this.RetornaString("select max(ID) from " + Tabela);
        }

        public string ID(string Tabela, string Condicao, string IDRegAtual)
        {
            string Sql = "select ID from " + Tabela + " where " + Condicao;

            if (IDRegAtual != "0")
                Sql += " and ID <> " + IDRegAtual;

            return this.RetornaString(Sql);
        }

        public string ID(string Tabela, string Condicao)
        {
            return ID(Tabela, Condicao, "0");
        }

        public bool Achou(string sql)
        {
            string Retorno = "";
            RetornaString(sql, ref Retorno);
            return Retorno != "" ? true : false;
        }

        public bool JaExiste(string Tabela, string Condicao, string IDRegAtual)
        {
            return ID(Tabela, Condicao, IDRegAtual) != "" ? false : true;
        }

        public bool JaExiste(string Tabela, string Condicao)
        {
            return ID(Tabela, Condicao, "0") != "" ? false : true;
        }
    }
}
