using System;
using System.Configuration;
using System.Timers;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Linq;
//using SGQ_EF;
//using sgq.alm;
using sgq;
using System.Net.Mail;
using Oracle.DataAccess.Client;

namespace sgq.alm_Projetos_Console
{
    class Program
    {
        static Timer timer;

        static string lastDay = "00";

        static bool H06 = true; static bool H15 = true;
        static bool H07 = true; static bool H16 = true;
        static bool H08 = true; static bool H17 = true;
        static bool H09 = true; static bool H18 = true;
        static bool H10 = true; static bool H19 = true;
        static bool H11 = true; static bool H20 = true;
        static bool H12 = true; static bool H21 = true;
        static bool H13 = true; static bool H22 = true;
        static bool H14 = true; static bool H23 = true;

        static void Main(string[] args)
        {

            // loadBitiData();

            if (args.Count<string>() > 0) {
                if (args[0].ToUpper() == "-RunLateAtNight") {
                    RunLateAtNight();
                }
            }

            timer = new System.Timers.Timer(30000);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = false;

            RunAtAllTimes();

            timer.Enabled = true;
            timer.Start();

            Console.WriteLine("Pressione \'q\' para sair.");
            while (Console.Read() != 'q') ;
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            RunAtAllTimes();

            timer.Start();
        }

        static private void RunAtAllTimes() {
            if (DateTime.Now.ToString("dd") != lastDay) {
                H06 = true; H15 = true;
                H07 = true; H16 = true;
                H08 = true; H17 = true;
                H09 = true; H18 = true;
                H10 = true; H19 = true;
                H11 = true; H20 = true;
                H12 = true; H21 = true;
                H13 = true; H22 = true;
                H14 = true; H23 = true;
                
                lastDay = DateTime.Now.ToString("dd");
            }

            var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list);
            projetos.loadData();
            //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list);
            //projetos.loadData();

            if (DateTime.Now.ToString("HH").CompareTo("06") == 0) RunAt06(ref H06);
            if (DateTime.Now.ToString("HH").CompareTo("07") == 0) RunAt07(ref H07);
            if (DateTime.Now.ToString("HH").CompareTo("08") == 0) RunAt08(ref H08);
            if (DateTime.Now.ToString("HH").CompareTo("09") == 0) RunAt09(ref H09);
            if (DateTime.Now.ToString("HH").CompareTo("10") == 0) RunAt10(ref H10);
            if (DateTime.Now.ToString("HH").CompareTo("11") == 0) RunAt11(ref H11);
            if (DateTime.Now.ToString("HH").CompareTo("12") == 0) RunAt12(ref H12);
            if (DateTime.Now.ToString("HH").CompareTo("13") == 0) RunAt13(ref H13);
            if (DateTime.Now.ToString("HH").CompareTo("14") == 0) RunAt14(ref H14);
            if (DateTime.Now.ToString("HH").CompareTo("15") == 0) RunAt15(ref H15);
            if (DateTime.Now.ToString("HH").CompareTo("16") == 0) RunAt16(ref H16);
            if (DateTime.Now.ToString("HH").CompareTo("17") == 0) RunAt17(ref H17);
            if (DateTime.Now.ToString("HH").CompareTo("18") == 0) RunAt18(ref H18);
            if (DateTime.Now.ToString("HH").CompareTo("19") == 0) RunAt19(ref H19);
            if (DateTime.Now.ToString("HH").CompareTo("20") == 0) RunAt20(ref H20);
            if (DateTime.Now.ToString("HH").CompareTo("21") == 0) RunAt21(ref H21);
            if (DateTime.Now.ToString("HH").CompareTo("22") == 0) RunAt22(ref H22);
            if (DateTime.Now.ToString("HH").CompareTo("23") == 0) RunAt23(ref H23);

            //Detalha_Execucao();
            //Historico_Subprojeto();

            ALM_Defeitos_Fabrica_Desenvolvimento();
            ALM_Defeitos_Fabrica_Teste();
            ALM_CTs_Fabrica_Desenvolvimento();
            ALM_CTs_Fabrica_Teste();
        }

        static private void RunAtAllHours() {
            var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list);
            projetos.Update();
            //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list);
            //projetos.Update();

            var usuarios = new alm.Usuarios(alm.Databases.ALM11);
            usuarios.LoadData();
            //usuarios = new Usuarios(alm.Databases.ALM12);
            //usuarios.LoadData();

            ALM_Defeitos_Ping_Pong();
            ALM_Defeitos_Aging();

            TRG2017_LoadSistemaPastaCT();
        }

        static private void RunAt06(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                loadBitiData();
                hour = false;
            }
        }

        static private void RunAt07(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                hour = false;
            }
        }

        static private void RunAt08(ref bool hour) {
            if (hour) {
                RunAtAllHours();

                var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list);
                projetos.loadDefectsTimes();
                //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list);
                //projetos.loadDefectsTimes();

                hour = false;
            }
        }

        static private void RunAt09(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                hour = false;
            }
        }

        static private void RunAt10(ref bool hour) {
            if (hour) {
                RunAtAllHours();

                var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list);
                projetos.loadDefectsTimes();
                //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list);
                //projetos.loadDefectsTimes();

                hour = false;
            }
        }

        static private void RunAt11(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                hour = false;
            }
        }

        static private void RunAt12(ref bool hour) {
            if (hour) {
                RunAtAllHours();

                var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list);
                projetos.loadDefectsTimes();
                //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list);
                //projetos.loadDefectsTimes();

                hour = false;
            }
        }

        static private void RunAt13(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                hour = false;
            }
        }

        static private void RunAt14(ref bool hour) {
            if (hour) {
                RunAtAllHours();

                var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list);
                projetos.loadDefectsTimes();

                //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list);
                //projetos.loadDefectsTimes();

                hour = false;
            }
        }

        static private void RunAt15(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                hour = false;
            }
        }

        static private void RunAt16(ref bool hour) {
            if (hour) {
                RunAtAllHours();

                var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list);
                projetos.loadDefectsTimes();

                //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list);
                //projetos.loadDefectsTimes();

                hour = false;
            }
        }

        static private void RunAt17(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                hour = false;
            }
        }

        static private void RunAt18(ref bool hour) {
            if (hour) {
                RunAtAllHours();

                var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list);
                projetos.loadDefectsTimes();

                //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list);
                //projetos.loadDefectsTimes();

                hour = false;
            }
        }

        static private void RunAt19(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                hour = false;
            }
        }

        static private void RunAt20(ref bool hour) {
            if (hour) {
                RunAtAllHours();

                var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list);
                projetos.loadDefectsTimes();

                //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list);
                //projetos.loadDefectsTimes();

                hour = false;
            }
        }

        static private void RunAt21(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                hour = false;
            }
        }

        static private void RunAt22(ref bool hour) {
            if (hour) {
                RunAtAllHours();
                hour = false;
            }
        }

        static private void RunAt23(ref bool hour) {
            if (hour) {
                if (DateTime.Now.DayOfWeek != DayOfWeek.Friday)
                    RunLateAtNight();
                else {
                    RunAtFDS();
                    // Extrair_Dados_Fornecedores();
                }
                hour = false;
            }
        }


        static private void RunLateAtNight()
        {
            var projetos = new alm.Projetos(alm.Databases.ALM11, new alm.FieldsALM11().list, TypeUpdate.IncrementFullUpdate);
            projetos.Update();
            projetos.loadData();
            projetos.loadDefectsTimes();
            //projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list, TypeUpdate.IncrementFullUpdate);
            //projetos.Update();
            //projetos.loadData();
            //projetos.loadDefectsTimes();

            var usuarios = new alm.Usuarios(alm.Databases.ALM11, TypeUpdate.IncrementFullUpdate);
            usuarios.LoadData();
            //usuarios = new Usuarios(alm.Databases.ALM12, TypeUpdate.IncrementFullUpdate);
            //usuarios.LoadData();

            TRG2017_LoadSistemaPastaCT();

            ALM_Defeitos_Qtd_Reopen();
            ALM_Defeitos_Agente_Solucionador();

            ALM_CTs_Qte_Reteste();
            ALM_CTs_Etapa();

            //CALCULO DAS DATAS REALIZADAS
            //-----------------------------------
            SGQ_Releases_Etapas();
            SGQ_Releases_Sistemas_Etapas();
            SGQ_Projetos_Etapas();
            //-----------------------------------

            Update_SGQ_Projects();
            Update_SGQ_PulledChainHistoric();

            SGQ_Releases_Indicadores();

            SGQ_Releases_Entregas_Indicadores();

            Detalha_Execucao();

            Historico_Subprojeto();

            Historico_Valores();

            SGQ_Historico_CTs_Por_Status();

            //Extrair_Dados_Fornecedores();
        }

        static private void RunAtFDS()
        {
            // Extrair_Dados_Fornecedores();

            //Projetos projetos = new alm.Projetos(alm.Databases.ALM12, new FieldsALM12().list) {
            //    typeUpdate = TypeUpdate.Full
            //};

            //projetos.Update();

            //projetos.Limpar_Dados_Projetos_Excluidos();

            // Usuarios.Atualizar_Dados(TypeUpdate.Full);

            //projetos.loadData();
        }


        static void TRG2017_LoadSistemaPastaCT() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\TRG2017_LoadSistemaPastaCT.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }


        public static void ALM_Defeitos_Qtd_Reopen() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_Defeitos_Qtd_Reopen.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void ALM_Defeitos_Ping_Pong() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_Defeitos_Ping_Pong.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void ALM_Defeitos_Aging() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_Defeitos_Aging.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void ALM_Defeitos_Fabrica_Desenvolvimento() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_Defeitos_Fabrica_Desenvolvimento.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void ALM_Defeitos_Fabrica_Teste() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_Defeitos_Fabrica_Teste.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void ALM_Defeitos_Agente_Solucionador() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_Defeitos_Agente_Solucionador.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }


        public static void ALM_CTs_Qte_Reteste() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_CTs_Qte_Reteste.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void ALM_CTs_Fabrica_Desenvolvimento() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_CTs_Fabrica_Desenvolvimento.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void ALM_CTs_Fabrica_Teste() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_CTs_Fabrica_Teste.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void ALM_CTs_Etapa() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\ALM_CTs_Etapa.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }


        public static void SGQ_Releases_Etapas() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\SGQ_Releases_Etapas.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void SGQ_Releases_Sistemas_Etapas() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\SGQ_Releases_Sistemas_Etapas.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void SGQ_Projetos_Etapas() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\SGQ_Projetos_Etapas.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }


        public static void Update_SGQ_Projects() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\Update_SGQ_Projects.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void Update_SGQ_PulledChainHistoric() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\Update_SGQ_PulledChainHistoric.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }


        public static void SGQ_Releases_Indicadores() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\SGQ_Releases_Indicadores.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }


        public static void SGQ_Releases_Entregas_Indicadores() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\SGQ_Releases_Entregas_Indicadores.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void Detalha_Execucao() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\Detalha_Execucao.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void Historico_Subprojeto() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\Historico_Subprojeto.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        public static void Historico_Valores() {
            string sql1 = File.ReadAllText(Gerais.Caminho_App() + @"\sql\Insert_SGQ_Historico_Valores_Execucao_Teste.sql");
            string sql2 = File.ReadAllText(Gerais.Caminho_App() + @"\sql\Insert_SGQ_Historico_Valores_Riscos.sql");
            string sql3 = File.ReadAllText(Gerais.Caminho_App() + @"\sql\Insert_SGQ_Historico_Valores_Status_Projeto_Release.sql");

            Connection Connection = new Connection();
            Connection.Executar(sql1);
            Connection.Executar(sql2);
            Connection.Executar(sql3);
            Connection.Dispose();
        }

        public static void SGQ_Historico_CTs_Por_Status() {
            string sql = File.ReadAllText(Gerais.Caminho_App() + @"\sql\SGQ_Historico_CTs_Por_Status.sql");
            Connection Connection = new Connection();
            Connection.Executar(sql);
            Connection.Dispose();
        }

        static private void loadBitiData() {

            DateTime Dt_Inicio_Geral = DateTime.Now;

            String Mensagem = "";
            DateTime Dt_Inicio;
            DateTime Dt_Fim;
            long Tempo = 0;

            Connection SGQConn = new Connection();

            Dt_Inicio = DateTime.Now;
            var projetos = new sgq.biti.Projetos(TypeUpdate.IncrementFullUpdate);

            projetos.LoadData();
            Dt_Fim = DateTime.Now;
            Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Projetos.<br/>";


            DateTime Dt_Fim_Geral = DateTime.Now;

            SGQConn.Dispose();

            Mensagem =
            @"<br/>Início: " + Dt_Inicio_Geral.ToString("dd-MM-yyyy HH:mm:ss") +
            @"<br/>Fim:   " + Dt_Fim_Geral.ToString("dd-MM-yyyy HH:mm:ss") +
            @"<br/>Tempo: " + DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio_Geral, Dt_Fim_Geral).ToString() + " min<br/><br/>" +
            Mensagem;

            Gerais.Enviar_Email(Gerais.Get_Lista_To_Aviso_Carga(), Gerais.Get_Lista_CC_Aviso_Carga(), "SGQ-BITI: Atualizou - Tipo Atualização: COMPLETA", Mensagem);
        }



        static private void Teste_Email() {
            MailMessage objEmail = new MailMessage();
            //objEmail.From = new MailAddress("sgq@oi.net.br");
            objEmail.From = new MailAddress("josesilva @oi.net.br");
            //objEmail.From = new MailAddress("joao.frade@oi.net.br");
            //objEmail.To.Add("viviane.a.estrela @accenture.com");
            objEmail.To.Add("joao.frade@oi.net.br");
            objEmail.CC.Add("josesilva @oi.net.br");
            objEmail.Subject = "teste 13";
            objEmail.Body = "mensagem do teste...";

            objEmail.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            objEmail.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            objEmail.IsBodyHtml = true;
            objEmail.Priority = MailPriority.Normal;

            //SmtpClient objSmtp = new SmtpClient("Relayexternoaut.telemar", 25);
            SmtpClient objSmtp = new SmtpClient("relayinternoaut.telemar", 25);
            //objSmtp.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            objSmtp.UseDefaultCredentials = false;
            objSmtp.Credentials = new System.Net.NetworkCredential("oi\\OI345302", "Macia016");
            //objSmtp.Credentials = new System.Net.NetworkCredential("oi\\oi12949", "JoaoDAN8");
            //objSmtp.Credentials = new System.Net.NetworkCredential("SGAT6001", "9P5bKg3G");

            try {
                objSmtp.Send(objEmail);
            } catch (SmtpException) { }
            objEmail.Dispose();
            objSmtp.Dispose();
        }

        //static private void Extrair_Dados_Fornecedores()
        //{
        //    DateTime Dt_Inicio_Geral = DateTime.Now;
        //    DayOfWeek DateTime.Now.DayOfWeek = DateTime.Today.DayOfWeek;

        //    String String_Connection = ConfigurationManager.ConnectionStrings["SGQ"].ConnectionString;
        //    String Sql_CTS = ConfigurationManager.AppSettings["Sql_CTS"];
        //    String Sql_Evidencias_Planos = ConfigurationManager.AppSettings["Sql_Evidencias_Planos"];
        //    String Sql_Defeitos = ConfigurationManager.AppSettings["Sql_Defeitos"];
        //    String Sql_Defeitos_Tempos = ConfigurationManager.AppSettings["Sql_Defeitos_Tempos"];

        //    String Folder = ConfigurationManager.AppSettings["Folder"];
        //    String File_Name = "";
        //    String File_Path = "";

        //    String Categoria = "Dados Fornecedores";
        //    String Tipo = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        //    File_Name = String.Format("Dados_Projetos_Ativos_ALM - CTs - {0}.xlsx", DateTime.Now.DayOfWeek);
        //    File_Path = Path.Combine(Folder, File_Name);
        //    Arquivos.Create_Excel_from_SQL(Sql_CTS, File_Path, "CTs");
        //    System.Threading.Thread.Sleep(5000);
        //    //Arquivos.Transmitir_To_FTP01(Folder, File_Name);
        //    Arquivos.Save_To_BD(File_Name, Categoria, Tipo, Arquivos.Get_Bytes(File_Path));

        //    File_Name = String.Format("Dados_Projetos_Ativos_ALM - Evidencias-Planos - {0}.xlsx", DateTime.Now.DayOfWeek);
        //    File_Path = Path.Combine(Folder, File_Name);
        //    Arquivos.Create_Excel_from_SQL(Sql_Evidencias_Planos, File_Path, "Evidencias-Planos");
        //    System.Threading.Thread.Sleep(5000);
        //    //Arquivos.Transmitir_To_FTP01(Folder, File_Name);
        //    Arquivos.Save_To_BD(File_Name, Categoria, Tipo, Arquivos.Get_Bytes(File_Path));

        //    File_Name = String.Format("Dados_Projetos_Ativos_ALM - Defeitos - {0}.xlsx", DateTime.Now.DayOfWeek);
        //    File_Path = Path.Combine(Folder, File_Name);
        //    Arquivos.Create_Excel_from_SQL(Sql_Defeitos, File_Path, "Defeitos");
        //    System.Threading.Thread.Sleep(5000);
        //    //Arquivos.Transmitir_To_FTP01(Folder, File_Name);
        //    Arquivos.Save_To_BD(File_Name, Categoria, Tipo, Arquivos.Get_Bytes(File_Path));

        //    File_Name = String.Format("Dados_Projetos_Ativos_ALM - Defeitos Tempos - {0}.xlsx", DateTime.Now.DayOfWeek);
        //    File_Path = Path.Combine(Folder, File_Name);
        //    Arquivos.Create_Excel_from_SQL(Sql_Defeitos_Tempos, File_Path, "Defeitos Tempos");
        //    System.Threading.Thread.Sleep(5000);
        //    //Arquivos.Transmitir_To_FTP01(Folder, File_Name);
        //    Arquivos.Save_To_BD(File_Name, Categoria, Tipo, Arquivos.Get_Bytes(File_Path));

        //    DateTime Dt_Fim_Geral = DateTime.Now;

        //    // Enviar Email

        //    BD oBD = new BD(Lib.SGQ());
        //    DataTable dt_Nomes_Projetos = new DataTable();
        //    oBD.RetornaDataTable("select distinct p.Subprojeto + ' - ' + p.Entrega + ' - ' + (select Nome from biti_Subprojetos where id = p.Subprojeto) as Nome from ALM_Projetos p WITH (NOLOCK) where p.Ativo = 'Y' order by 1", ref dt_Nomes_Projetos);
        //    List<String> Nomes_Projetos = new List<String>();
        //    foreach (DataRow row in dt_Nomes_Projetos.Rows)
        //    {
        //        Nomes_Projetos.Add(row.Field<String>("Nome")); ;
        //    }
        //    oBD.Close();

        //    //Gerais.Enviar_Email_Atualizacao_Projetos(
        //    //@"SGQ - ALM - Gerou Extração de Dados para Fornecedores",
        //    //Nomes_Projetos,
        //    //Dt_Inicio_Geral,
        //    //Dt_Fim_Geral
        //    //);
        //}


        //public static void Detalha_Execucao()
        //{
        //    BD oBD = new BD(Lib.SGQ());

        //    string sql = @"delete SGQ_Detalha_Execucao where dia = convert(varchar, getdate(),5)";
        //    oBD.Executar(sql);

        //    sql = @"
        //            insert into SGQ_Detalha_Execucao
        //            (
        //             subprojeto,
        //             entrega,
        //             Dia,
        //             Massa_Teste,
        //             OK,
        //             Em_Exec,
        //             Erro,
        //             Bloqueado,
        //             NA,
        //             OK_Reteste,
        //             Em_Exec_Reteste,
        //             Erro_Reteste,
        //             Bloqueado_Reteste,
        //             NA_Reteste
        //            )
        //            select
        //                Ex2.subprojeto,
        //                Ex2.entrega,
        //                right(Data,2) + '-' + substring(Data,3,2) + '-' + left(Data,2) as Dia,
        //                Upper(CT.Massa_Teste) as Massa_Teste,
        //                sum(case when Ex2.Reteste = 0 then Ex2.OK else 0 end) as OK,
        //                sum(case when Ex2.Reteste = 0 then Ex2.Em_Exec else 0 end) as Em_Exec,
        //                sum(case when Ex2.Reteste = 0 then Ex2.Erro else 0 end) as Erro,
        //                sum(case when Ex2.Reteste = 0 then Ex2.Bloqueado else 0 end) as Bloqueado,
        //                sum(case when Ex2.Reteste = 0 then Ex2.NA else 0 end) as NA,

        //                sum(case when Ex2.Reteste = 1 then Ex2.OK else 0 end) as OK_Reteste,
        //                sum(case when Ex2.Reteste = 1 then Ex2.Em_Exec else 0 end) as Em_Exec_Reteste,
        //                sum(case when Ex2.Reteste = 1 then Ex2.Erro else 0 end) as Erro_Reteste,
        //                sum(case when Ex2.Reteste = 1 then Ex2.Bloqueado else 0 end) as Bloqueado_Reteste,
        //                sum(case when Ex2.Reteste = 1 then Ex2.NA else 0 end) as NA_Reteste
        //            from
        //                   (
        //                   select
        //                         Ex.subprojeto,
        //                         Ex.entrega,
        //                         substring(Ex.Dt_Execucao,7,2) + substring(Ex.Dt_Execucao,4,2) + substring(Ex.Dt_Execucao,1,2) as Data,
        //                         Ex.CT,
        //                         (case when Ex.status = 'PASSED' then 1 else 0 end) as OK,
        //                         (case when Ex.status = 'NOT COMPLETED' then 1 else 0 end) as Em_Exec,
        //                         (case when Ex.status = 'FAILED' then 1 else 0 end) as Erro,
        //                         (case when Ex.status = 'BLOCKED' then 1 else 0 end) as Bloqueado,
        //                         (case when Ex.status in('N/A','') then 1 else 0 end) as NA,

        //                         (case when exists
        //                                (   select top 1 1
        //                                              from alm_historico_alteracoes_campos ht
        //                                              where 
        //                                                           ht.tabela = 'TESTCYCL' and 
        //                                                           ht.Campo = 'STATUS' and 
        //                                                           ht.Novo_valor = 'PASSED' and
        //                                                           ht.subprojeto = Ex.subprojeto and 
        //                                                           ht.entrega = Ex.entrega and
        //                                                           ht.tabela_id = Ex.CT and
        //                                                           convert(datetime, ht.dt_alteracao,5) < convert(datetime, Ex.Dt_Execucao,5)
        //                                )
        //                                then 1
        //                                else 0
        //                         end) as Reteste
        //                   from 
        //                         ALM_Execucoes Ex
        //                   where 
        //                         substring(Ex.Dt_Execucao,7,2) + substring(Ex.Dt_Execucao,4,2) + substring(Ex.Dt_Execucao,1,2) = right(convert(varchar(30),dateadd(dd, 0, getdate()),112),6)  and
        //                         Status <> 'CANCELLED' --and
        //                         --Ex.subprojeto = 'PRJ00004929' and 
        //                         --Ex.entrega = 'ENTREGA00001639'
        //                   ) Ex2
        //                   left join ALM_CTs CT 
        //                     on CT.subprojeto = Ex2.subprojeto and 
        //                        CT.entrega = Ex2.entrega and 
        //                          CT.CT = Ex2.CT
        //            group by
        //                Ex2.subprojeto,
        //                Ex2.entrega,
        //                Ex2.Data,
        //                   CT.Massa_Teste
        //            order by
        //                Ex2.subprojeto,
        //                Ex2.entrega,
        //                Ex2.Data,
        //                CT.Massa_Teste
        //            ";
        //    oBD.Executar(sql);

        //    oBD.Close();
        //}

        //public static void Historico_Subprojeto()
        //{
        //    BD oBD = new BD(Lib.SGQ());

        //    string sql = @"delete SGQ_Historico_Subprojeto where data = convert(varchar, getdate(),5)";
        //    oBD.Executar(sql);

        //    sql = @"
        //        insert into SGQ_Historico_Subprojeto
        //        select
        //         Data,
        //         Release,
        //         Subprojeto,
        //         Entrega,
        //         Nome,
        //         Macro_Diretoria,
        //         GP,
        //         N4,
        //         N3,
        //         Classificacao,
        //         Reportado_Na_Release, 
        //         Pontos_Atencao,
        //         Risco, 
        //         Risco_Ordem,
        //         Motivo_Perda_Release,

        //         Dt_Alteracao as Dt_Alteracao_Pontos_Atencao,
        //         Dt_Perda_Release,

        //         sum(Total) as CTs,  
        //         sum(Cancelado) as CTs_Cancelados,  
        //         sum(Ativo) as CTs_tivos,  
        //         sum(Ativo_UAT) as CTs_Ativos_UAT,  
        //         sum(Planejado) as CTs_Planejados,

        //         sum(Prev_Dia) as CTs_Prev_Dia,  
        //         sum(Real_Dia) as CTs_Real_Dia,  
        //         sum(Real_Dia) - sum(Prev_Dia) as CTs_Saldo_Dia,

        //         sum(Prev_Acumulado) as CTs_Prev_Acumulado,  
        //         sum(Real_Acumulado) as CTs_Real_Acumulado,  
        //         sum(Real_Acumulado) - sum(Prev_Acumulado) as CTs_Saldo_Acumulado,

        //         sum(Plano_Lib_Valid_Tecnica) as CTs_Plano_Lib_Valid_Tecnica,
        //         sum(Plano_Lib_Valid_Cliente) as CTs_Plano_Lib_Valid_Cliente,
        //         sum(Evidencia_Lib_Valid_Tecnica) as CTs_Evidencia_Lib_Valid_Tecnica,
        //         sum(Evidencia_Lib_Valid_Cliente) as CTs_Evidencia_Lib_Valid_Cliente,

        //         sum(Plano_TI_Aprovado) as CTs_Plano_TI_Aprovado,
        //         sum(Plano_UAT_Aprovado) as CTs_Plano_UAT_Aprovado,
        //         sum(Evidencia_TI_Aprovado) as CTs_Evidencia_TI_Aprovado,
        //         sum(Evidencia_UAT_Aprovado) as CTs_Evidencia_UAT_Aprovado,

        //         (select count(*) from ALM_Defeitos d 
        //          where 
        //                 d.subprojeto = Aux.Subprojeto and 
        //           d.entrega = Aux.Entrega
        //         ) as Defeitos,

        //         (select count(*) from ALM_Defeitos d 
        //          where 
        //                 d.subprojeto = Aux.Subprojeto and 
        //           d.entrega = Aux.Entrega and 
        //           d.Status_Atual = 'CANCELLED'
        //         ) as Defeitos_Cancelados,

        //         (select count(*) from ALM_Defeitos d 
        //          where 
        //              d.subprojeto = Aux.Subprojeto and 
        //           d.entrega = Aux.Entrega and
        //           d.Status_Atual <> 'CANCELLED'
        //         ) as Defeitos_Ativos,

        //         (select count(*) from ALM_Defeitos d 
        //          where 
        //                 d.subprojeto = Aux.Subprojeto and 
        //           d.entrega = Aux.Entrega and 
        //           d.Status_Atual = 'CLOSED'
        //         ) as Defeitos_Fechados,

        //         (select count(*) from ALM_Defeitos d 
        //          where 
        //                 d.subprojeto = Aux.Subprojeto and 
        //           d.entrega = Aux.Entrega and 
        //           d.Status_Atual = 'CLOSED' and
        //           d.Origem like '%CONSTRUÇÃO%' and
        //           (d.Ciclo like '%TI%' or d.Ciclo like '%UAT%')
        //         ) as Defeitos_Fechados_Construcao,

        //         (select count(*) from ALM_Defeitos d 
        //          where 
        //                 d.subprojeto = Aux.Subprojeto and 
        //           d.entrega = Aux.Entrega and 
        //           d.Status_Atual not in('CANCELLED', 'CLOSED')
        //         ) as Defeitos_Abertos,

        //         (select count(*) from ALM_Defeitos d 
        //          where 
        //                 d.subprojeto = Aux.Subprojeto and 
        //           d.entrega = Aux.Entrega and 
        //           d.Status_Atual in ('ON_RETEST','PENDENT (RETEST)','REJECTED')
        //         ) as Defeitos_Na_FT,

        //         (select count(*) from ALM_Defeitos d 
        //          where 
        //                 d.subprojeto = Aux.Subprojeto and 
        //           d.entrega = Aux.Entrega and 
        //           d.Status_Atual in ('NEW','IN_PROGRESS','PENDENT (PROGRESS)','REOPEN','MIGRATE')
        //         ) as Defeitos_Na_FD,

        //         round((convert(float, sum(Prev_Dia)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Prev_Dia,
        //         round((convert(float, sum(Real_Dia)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Real_Dia,

        //         round((convert(float, sum(Prev_Acumulado)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Prev_Acumulado,
        //         round((convert(float, sum(Real_Acumulado)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Real_Acumulado,

        //         round((CONVERT(float, sum(Plano_TI_Aprovado)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Plano_TI_Aprovado,
        //         round((CONVERT(float, sum(Plano_UAT_Aprovado)) / (case when sum(Ativo_UAT) <> 0 then sum(Ativo_UAT) else 1 end) * 100), 2) as Perc_Plano_UAT_Aprovado,
        //         round((CONVERT(float, sum(Evidencia_TI_Aprovado)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Evidencia_TI_Aprovado,
        //         round((CONVERT(float, sum(Evidencia_UAT_Aprovado)) / (case when sum(Ativo_UAT) <> 0 then sum(Ativo_UAT) else 1 end) * 100), 2) as Perc_Evidencia_UAT_Aprovado

        //        from   
        //            (
        //            select 
        //             convert(varchar, getdate(), 5) as Data,
        //                (select r.nome from sgq_releases r where r.id = eXr.Release) as Release,
        //                eXr.Subprojeto,
        //                eXr.Entrega,
        //                sp.Nome as Nome,
        //                sp.Macro_Diretoria as Macro_Diretoria,
        //                sp.Gerente_Projeto as GP,
        //                sp.Gestor_Direto_LT as N4,
        //                sp.Gestor_Do_Gestor_LT as N3,
        //          sp.classificacao_nome as Classificacao,

        //                case when eXr.Exibir_Status_Diario = 1 then 'SIM' else 'NÃO' end as Reportado_Na_Release, 

        //                eXr.descricao_Risco as Pontos_Atencao,

        //                case 
        //                    When eXr.Risco in (1,2,3,4,5) then (select ltrim(r.nome) from SGQ_Riscos r where r.id = eXr.Risco)
        //                    When eXr.Risco is null  then 'N/A'
        //                end as Risco, 

        //                case 
        //                    When eXr.Risco in (1,2,3,4,5) then (select r.Ordem from SGQ_Riscos r where r.id = eXr.Risco)
        //                    When eXr.Risco is null  then 5
        //                end as Risco_Ordem, 

        //                (select mpr.nome from SGQ_Motivos_Perda_Releases mpr where mpr.id = eXr.Motivo_Perda_Release) as Motivo_Perda_Release,

        //          eXr.Dt_Alteracao,
        //          eXr.Dt_Perda_Release,

        //                case when (alm_cts.ct is not null) then 1 else 0 end as Total,  

        //                case when (Status_Exec_CT = 'CANCELLED')  then 1 else 0 end as Cancelado,  
        //                case when (Status_Exec_CT <> 'CANCELLED')  then 1 else 0 end as Ativo,

        //                case when (Status_Exec_CT <> 'CANCELLED' and UAT = 'SIM')  then 1 else 0 end as Ativo_UAT,

        //                case when (Status_Exec_CT <> 'CANCELLED' and Dt_Planejamento <> '') then 1 else 0 end as Planejado,		          

        //                case when (Status_Exec_CT <> 'CANCELLED' and Dt_Planejamento <> '') and 
        //                    (substring(Dt_Planejamento,7,2) + substring(Dt_Planejamento,4,2) + substring(Dt_Planejamento,1,2)) = convert(varchar(6), getdate(), 12) then 1 else 0 end as Prev_Dia,

        //                case when (Status_Exec_CT = 'PASSED' and Dt_Execucao <> '') and 
        //                    (substring(Dt_Execucao,7,2) + substring(Dt_Execucao,4,2) + substring(Dt_Execucao,1,2)) = convert(varchar(6), getdate(), 12) then 1 else 0 end as Real_Dia,

        //                case when (Status_Exec_CT <> 'CANCELLED' and Dt_Planejamento <> '') and 
        //                    (substring(Dt_Planejamento,7,2) + substring(Dt_Planejamento,4,2) + substring(Dt_Planejamento,1,2)) <= convert(varchar(6), getdate(), 12) then 1 else 0 end as Prev_Acumulado,

        //                case when (Status_Exec_CT = 'PASSED' and Dt_Execucao <> '') and 
        //                    (substring(Dt_Execucao,7,2) + substring(Dt_Execucao,4,2) + substring(Dt_Execucao,1,2)) <= convert(varchar(6), getdate(), 12) then 1 else 0 end as Real_Acumulado,


        //                case when (Status_Exec_CT <> 'CANCELLED' and 
        //                    (Plano_Validacao_Tecnica = 'LIBERADO PARA VALIDAÇÃO' or Plano_Validacao_Tecnica = 'LIBERADO PARA REVALIDAÇÃO')) then 1 else 0 end as Plano_Lib_Valid_Tecnica,

        //                case when (Status_Exec_CT <> 'CANCELLED' and UAT = 'SIM') and 
        //                    (Plano_Validacao_Cliente = 'LIBERADO PARA VALIDAÇÃO' or Plano_Validacao_Cliente = 'LIBERADO PARA REVALIDAÇÃO') then 1 else 0 end as Plano_Lib_Valid_Cliente,

        //                case when (Status_Exec_CT = 'PASSED') and 
        //                    (Evidencia_Validacao_Tecnica = 'LIBERADO PARA VALIDAÇÃO' or Evidencia_Validacao_Tecnica = 'LIBERADO PARA REVALIDAÇÃO') then 1 else 0 end as Evidencia_Lib_Valid_Tecnica,

        //                case when (Status_Exec_CT = 'PASSED' and UAT = 'SIM') and 
        //                    (Evidencia_Validacao_Cliente = 'LIBERADO PARA VALIDAÇÃO' or Evidencia_Validacao_Cliente = 'LIBERADO PARA REVALIDAÇÃO') then 1 else 0 end as Evidencia_Lib_Valid_Cliente,


        //                case when (Status_Exec_CT <> 'CANCELLED') and 
        //              (Plano_Validacao_Tecnica = 'VALIDADO' or Plano_Validacao_Tecnica = 'N/A') then 1 else 0 end as Plano_TI_Aprovado,

        //                case when (Status_Exec_CT <> 'CANCELLED' and UAT = 'SIM') and 
        //              (Plano_Validacao_Cliente = 'VALIDADO' or Plano_Validacao_Cliente = 'N/A') then 1 else 0 end as Plano_UAT_Aprovado,

        //                case when (Status_Exec_CT = 'PASSED') and 
        //              (Evidencia_Validacao_Tecnica = 'VALIDADO' or Evidencia_Validacao_Tecnica = 'N/A') then 1 else 0 end as Evidencia_TI_Aprovado,

        //                case when (Status_Exec_CT = 'PASSED' and UAT = 'SIM') and 
        //              (Evidencia_Validacao_Cliente = 'VALIDADO' or Evidencia_Validacao_Cliente = 'N/A') then 1 else 0 end as Evidencia_UAT_Aprovado

        //            from 
        //                SGQ_Releases_Entregas eXr 
        //                left join alm_cts 
        //                        on alm_cts.subprojeto = eXr.subprojeto and alm_cts.entrega = eXr.entrega
        //                left join BITI_Subprojetos sp 
        //                        on sp.id = eXr.subprojeto
        //            ) as Aux
        //        group by 
        //         Data,
        //         Release,
        //         Subprojeto,
        //         Entrega,
        //         Nome,
        //         Macro_Diretoria,
        //         GP,
        //         N4,
        //         N3,
        //         Classificacao,
        //         Reportado_Na_Release, 
        //         Pontos_Atencao,
        //         Risco, 
        //         Risco_Ordem, 
        //         Motivo_Perda_Release,
        //         Dt_Alteracao,
        //         Dt_Perda_Release
        //        order by
        //         2,3,4
        //            ";
        //    oBD.Executar(sql);

        //    oBD.Close();
        //}
    }
}
