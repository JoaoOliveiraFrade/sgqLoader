using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Timers;
using sgq;
using sgq.biti;

namespace BITI_Console
{
    class Program
    {
        static string Dia_Anterior = "00";
        static bool H06 = true;
        static Timer aTimer;

        static void Main(string[] args)
        {

            RunMain();

            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 600000;
            aTimer.Enabled = true;

			if (args.Count<string>() > 0)
                if (args[0].ToUpper() == "-R")
                    RunMain();

            Console.WriteLine("Pressione \'q\' para sair.");
            while (Console.Read() != 'q') ;
        }
        static private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (DateTime.Now.ToString("dd") != Dia_Anterior)
            {
                H06 = true;
                Dia_Anterior = DateTime.Now.ToString("dd");
            }

            if ((DateTime.Now.DayOfWeek != DayOfWeek.Saturday) && (DateTime.Now.DayOfWeek != DayOfWeek.Sunday))
            {
                if (DateTime.Now.ToString("HH").Contains("06") && H06)
                    RunMain(ref H06);
            }
        }

        static private void RunMain(ref bool Hora)
        {
            RunMain();
            Hora = false;
        }
        static private void RunMain()
        {

            DateTime Dt_Inicio_Geral = DateTime.Now;

            String Mensagem = "";
            DateTime Dt_Inicio;
            DateTime Dt_Fim;
            long Tempo = 0;

            Connection SGQConn = new Connection();

            Dt_Inicio = DateTime.Now;
            var projetos = new Projetos(TypeUpdate.IncrementFullUpdate);
            projetos.LoadData();
            Dt_Fim = DateTime.Now;
            Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Projetos.<br/>";

            //// Com Chave, Insert + Update
            //Dt_Inicio = DateTime.Now;
            //         //SGQConn.Executar("truncate table BITI_Sistemas");
            //         Sistemas.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Sistemas.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Ideias");
            //         Ideias.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Ideias.<br/>";

            //         //Dt_Inicio = DateTime.Now;
            //         //SGQConn.Executar("truncate table BITI_Projetos");
            //         //Projetos.Atualizar(TypeUpdate.Full);
            //         //Dt_Fim = DateTime.Now;
            //         //Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         //Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Projetos.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Subprojetos");
            //         Subprojetos.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Subprojetos.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_CRs");
            //         CRs.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: CRs.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Entregas");
            //         Entregas.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Entregas.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Frentes_Trabalho");
            //         Frentes_Trabalho.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Frentes_Trabalho.<br/>";

            //Dt_Inicio = DateTime.Now;
            //SGQConn.Executar("truncate table BITI_Desenhos");
            //Desenhos.Atualizar(TypeUpdate.Full);
            //Dt_Fim = DateTime.Now;
            //Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Desenho.<br/>";

            //Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Execucoes");
            //         Execucoes.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Execucoes.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Aprovacoes_Financeiras");
            //         Aprovacoes_Financeiras.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Aprovacoes_Financeiras.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Aprovacoes_Financeiras_FT");
            //         Aprovacoes_Financeiras_FT.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Aprovacoes_Financeiras_FT.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Propostas");
            //         Propostas.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Propostas.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Metricas");
            //         Metricas.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Metricas.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Fases_Contratadas");
            //         Fases_Contratadas.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Fases_Contratadas.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Macro_Estimativas");
            //         Macro_Estimativas.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Macro_Estimativas.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Documentos_Controlados");
            //         Documentos_Controlados.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Documentos_Controlados.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Usuarios");
            //         Usuarios.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Usuarios.<br/>";


            //         // Sem Chave, truncate + Insert
            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Cronogramas_Subprojetos");
            //         Cronogramas_Subprojetos.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Cronogramas_Subprojetos.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Cronogramas_Entregas");
            //         Cronogramas_Entregas.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Cronogramas_Entregas.<br/>";

            //         Dt_Inicio = DateTime.Now;
            //         SGQConn.Executar("truncate table BITI_Mudancas_Estados");
            //         Mudancas_Estados.Atualizar(TypeUpdate.Full);
            //         Dt_Fim = DateTime.Now;
            //         Tempo = DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio, Dt_Fim);
            //         Mensagem += Tempo.ToString().PadLeft(4, '0') + " min, Tabela: Mudancas_Estados.<br/>";

            //         Subprojetos.Atualizar_Classificacao();
            //         Entregas.Atualizar_Release();


            DateTime Dt_Fim_Geral = DateTime.Now;

            SGQConn.Dispose();

            Mensagem =
            @"<br/>Início: " + Dt_Inicio_Geral.ToString("dd-MM-yyyy HH:mm:ss") +
            @"<br/>Fim:   " + Dt_Fim_Geral.ToString("dd-MM-yyyy HH:mm:ss") +
            @"<br/>Tempo: " + DataEHora.DateDiff(DataEHora.DateInterval.Minute, Dt_Inicio_Geral, Dt_Fim_Geral).ToString() + " min<br/><br/>" +
            Mensagem;

            Gerais.Enviar_Email(Gerais.Get_Lista_To_Aviso_Carga(), Gerais.Get_Lista_CC_Aviso_Carga(), "SGQ-BITI: Atualizou - Tipo Atualização: COMPLETA", Mensagem);
        }

    }
}
