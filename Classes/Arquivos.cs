using EnterpriseDT.Net.Ftp;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace sgq
{
    public static class Arquivos
    {
        public static void LerArq(string NomeArquivo, ref string Conteudo)
        {
            FileStream objStream = new FileStream(NomeArquivo, FileMode.OpenOrCreate);
            StreamReader Arq = new StreamReader(objStream, System.Text.Encoding.Default);
            Conteudo = Arq.ReadToEnd();
        }

        public static void Create_Excel_from_SQL(String Sql, String File_Path, String worksheet)
        {
            Connection SGQConn = new Connection();
            SqlDataReader oReader = SGQConn.Get_DataReader(Sql);

            Excel oExcel = new Excel();
            oExcel.Create_Worksheet(oReader, worksheet);
            oExcel.Save_Workbook(File_Path);
            oExcel.shutDown();

            oReader.Dispose();
            SGQConn.Dispose();
        }

        public static void Transmitir_To_FTP01(String Folder, String FileName)
        {
            String Servidor = "ftp01";
            String Usuario = "ftp01";
            String Senha = "ftp01";
            //String Pasta = "sgq";

            string File_PathdataSource = Path.Combine(Folder, FileName);
            string File_PathtargetTable = Path.Combine(@"sgq/", FileName);

            //FTPClient ftp = new FTPClient(Servidor);
            FTPClient ftp = new FTPClient();
            ftp.RemoteHost = Servidor;
            ftp.Login(Usuario, Senha);
            ftp.ConnectMode = FTPConnectMode.PASV;
            ftp.TransferType = FTPTransferType.BINARY;
            ftp.Put(File_PathdataSource, File_PathtargetTable);
            ftp.Quit();
        }

        public static Byte[] Get_Bytes(String filePath)
        {
            string filename = Path.GetFileName(filePath);
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Byte[] bytes = br.ReadBytes((Int32)fs.Length);
            br.Close();
            fs.Close();
            return bytes;
        }

        public static void Save_To_BD(String Nome, String Categoria, String Tipo, Byte[] Dados)
        {
            Connection SGQConn = new Connection();
            String Id = SGQConn.Get_String(String.Format("select Id from SGQ_Arquivos where Nome='{0}' and Categoria='{1}' and Tipo='{2}'", Nome, Categoria, Tipo));
            string strQuery = "";

            if (Id != "")
            {
                strQuery = String.Format("update SGQ_Arquivos set Atualizacao = getdate(), Dados = @Dados where Id = {0}", Id);
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.Parameters.Add("@Dados", SqlDbType.Binary).Value = Dados;
                SGQConn.Executar(cmd);
            }
            else
            {
                strQuery = "insert into SGQ_Arquivos (Categoria, Nome, Tipo, Atualizacao, Dados) values (@Categoria, @Nome, @Tipo, getdate(), @Dados)";
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.Parameters.Add("@Categoria", SqlDbType.VarChar).Value = Categoria;
                cmd.Parameters.Add("@Nome", SqlDbType.VarChar).Value = Nome;
                cmd.Parameters.Add("@Tipo", SqlDbType.VarChar).Value = Tipo; 
                cmd.Parameters.Add("@Dados", SqlDbType.Binary).Value = Dados;
                SGQConn.Executar(cmd);
            } 
            SGQConn.Dispose();
        }
    }
}
