using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using System.Globalization;
// using TDAPIOLELib;

namespace sgq.alm {
    public static class Defeitos_Tempos {
        public static void LoadData(Projeto projeto, TypeUpdate typeUpdate, alm.Database database) {
            DateTime Dt_Inicio = DateTime.Now;

            Connection SGQConn = new Connection();

            if (typeUpdate == TypeUpdate.Full) {
                SGQConn.Executar($"delete ALM_Defeitos_Tempos where Subprojeto = '{projeto.Subprojeto}' and Entrega = '{projeto.Entrega}'");
            }

            Defeitos_Tempos_Processar(projeto, database);

            DateTime Dt_Fim = DateTime.Now;

            if (typeUpdate == TypeUpdate.Increment || typeUpdate == TypeUpdate.IncrementFullUpdate) {
                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Defeitos_Tempos_Incremental_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Tempos_Incremental_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Tempos_Incremental_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");

            } else if (typeUpdate == TypeUpdate.Full) {
                SGQConn.Executar($@"
                    update 
                        alm_projetos 
                    set Defeitos_Tempos_Completa_Inicio='{Dt_Inicio.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Tempos_Completa_Fim='{Dt_Fim.ToString("dd-MM-yy HH:mm:ss")}',
                        Defeitos_Tempos_Completa_Tempo={DataEHora.DateDiff(DataEHora.DateInterval.Second, Dt_Inicio, Dt_Fim)}
                    where 
                        subprojeto='{projeto.Subprojeto}' and entrega='{projeto.Entrega}'
                ");
            }

            SGQConn.Dispose();

            //====================================================================
            //Connection SGQConn = new Connection();
            //SGQConn.Executar(
            //    @"update ALM_Defeitos
            //            set Qtd_Reopen = 
            //                (select count(*)
            //                    from  
            //                    (select distinct Dt_Ate from ALM_Defeitos_Tempos t 
            //                        where t.Subprojeto = '{Subprojeto}' and t.Entrega = '{Entrega}' and t.Defeito = ALM_Defeitos.Defeito and t.Status = 'REOPEN') x
            //                ) ".Replace("{Subprojeto}", projeto.Subprojeto).Replace("{Entrega}", projeto.Entrega));
            //SGQConn.Dispose();
            //====================================================================

            //Gerais.Enviar_Email_Atualizacao_Tabela("ALM", "ALM_Defeitos_Tempos - " + projeto.Subprojeto + " - " + projeto.Entrega, Dt_Inicio, Dt_Fim);
        }

        private static void Defeitos_Tempos_Processar(Projeto projeto, alm.Database database) {
            List<Defeito> List_Defeitos = Get_Defeitos_Nao_Carregados_Ou_Carregados_Open(projeto);

            foreach (Defeito defeito in List_Defeitos) {
                List<Logs> List_Logs = Get_Logs_Defeitos(projeto, defeito, database);

                List<Evento> List_Eventos = Get_Eventos(defeito, List_Logs);

                foreach (Evento evento in List_Eventos) {
                    Update_Defeito_Tempos(projeto, defeito.Id.ToString(), evento);
                    Insert_Defeito_Tempos(projeto, defeito.Id.ToString(), evento);
                }
            }

        }

        private static List<Defeito> Get_Defeitos_Nao_Carregados_Ou_Carregados_Open(Projeto projeto) {
            string sql = $@"
                select 
                    Id,
                    Ultimo_Status,
                    Ultimo_Encaminhado_Para,
                    Ultima_Data
                from
                    (select 
                        d.Defeito as Id,
        
                        (select top 1 Status
                         from ALM_Defeitos_Tempos t 
                         where t.subprojeto = d.subprojeto and t.entrega = d.entrega and t.defeito = d.defeito
                         order by (substring(t.Dt_De,7,2) + substring(t.Dt_De,4,2) + substring(t.Dt_De,1,2) + substring(t.Dt_De,10,8)) desc
                        ) as Ultimo_Status,
        
                        (select top 1 Encaminhado_Para
                         from ALM_Defeitos_Tempos t 
                         where t.subprojeto = d.subprojeto and t.entrega = d.entrega and t.defeito = d.defeito
                         order by (substring(t.Dt_De,7,2) + substring(t.Dt_De,4,2) + substring(t.Dt_De,1,2) + substring(t.Dt_De,10,8)) desc
                        ) as Ultimo_Encaminhado_Para,
        
                        (select max('20' + substring(t.Dt_De,7,2) + '-' + substring(t.Dt_De,4,2) + '-' + substring(t.Dt_De,1,2) + ' ' + substring(t.Dt_De,10,8))
                         from ALM_Defeitos_Tempos t 
                         where t.subprojeto = d.subprojeto and t.entrega = d.entrega and t.defeito = d.defeito
                        ) as Ultima_Data
        
                    from 
                        ALM_Defeitos d
                    where 
                        d.subprojeto = '{projeto.Subprojeto}' and d.entrega = '{projeto.Entrega}'
                    ) as Aux
                where 
                    (Ultimo_Status is null) or (Ultimo_Status not in('CANCELLED','CLOSED'))
                order by 
                    Id
            ";

            Connection SGQConn = new Connection();

            List<Defeito> List_Defeitos = new List<Defeito>();
            List_Defeitos = SGQConn.Executar<Defeito>(sql);
            return List_Defeitos;
        }

        private static List<Logs> Get_Logs_Defeitos(Projeto projeto, Defeito defeito, alm.Database database) {
            Connection SGQConn = new Connection();
            string Esquema = SGQConn.Get_String($"select Esquema from ALM_Projetos where subprojeto = '{projeto.Subprojeto}' and entrega = '{projeto.Entrega}'");
            string Encaminhado_Para = "bg_user_template_09".ToUpper();
            SGQConn.Dispose();

            if (defeito.Ultima_Data == null) {
                defeito.Ultima_Data = "1901-01-01 01:01:01";
            }

            string sql_Logs = $@"
            select 
                to_char(au_time,'yyyy-mm-dd hh24:mi:ss') as Data,
                ap_field_name as Campo,
                ap_new_value as Valor,
                au_user as Operador
            from  
                {Esquema}.audit_properties
                inner join {Esquema}.audit_log
                    on ap_action_id = au_action_id 
                inner join {Esquema}.bug
                    on au_entity_id = bg_bug_id 
            where 
                (au_entity_type = 'BUG') and 
                (ap_field_name = 'BG_STATUS' or ap_field_name = '{Encaminhado_Para}') and
                (au_entity_id = {defeito.Id}) 
            order by au_time"; //.Replace("{Ultima_Data}", defeito.Ultima_Data);

            ALMConnection ALMConn = new ALMConnection(database);
            //List<Logs> List_Logs = new List<Logs>();
            //List_Logs = ALMConn.Executar<Logs>(sql_Logs);
            List<Logs> List_Logs = ALMConn.Executar<Logs>(sql_Logs);
            ALMConn.Dispose();

            return List_Logs;
        }

        private static List<Evento> Get_Eventos(Defeito defeito, List<Logs> List_Logs) {
            string Ultimo_Data = "";
            string Ultimo_Status = "";
            string Ultimo_Encaminhado_Para = "";
            var List_Eventos = new List<Evento>();
            var evento = new Evento();
            evento.Dt_De = "";

            foreach (Logs oLogs in List_Logs) {
                if (evento.Dt_De == "") {
                    evento.Dt_De = oLogs.Data;
                }

                if (oLogs.Data != evento.Dt_De) {
                    evento.Dt_Ate = oLogs.Data;

                    if (evento.Status == null && Ultimo_Status != "") {
                        evento.Status = Ultimo_Status;
                    }

                    if (evento.Encaminhado_Para == null && Ultimo_Encaminhado_Para != "") {
                        evento.Encaminhado_Para = Ultimo_Encaminhado_Para;
                    }

                    Ultimo_Data = evento.Dt_Ate;
                    Ultimo_Status = evento.Status;
                    Ultimo_Encaminhado_Para = evento.Encaminhado_Para;

                    evento.Tempo_Util_Min = (long)DataEHora.BusinessTimeDelta(
                        DateTime.ParseExact(evento.Dt_De, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        DateTime.ParseExact(evento.Dt_Ate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    ).TotalMinutes;

                    evento.Tempo_Decorrido_Min = DataEHora.DateDiff(
                        DataEHora.DateInterval.Minute,
                        DateTime.ParseExact(evento.Dt_De, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        DateTime.ParseExact(evento.Dt_Ate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    );

                    List_Eventos.Add(evento);
                    evento = new Evento();
                    evento.Dt_De = oLogs.Data;
                }

                evento.Operador = oLogs.Operador.ToUpper();

                if (oLogs.Valor != null) {
                    if (oLogs.Campo == "BG_STATUS") {
                        evento.Status = oLogs.Valor.ToUpper();
                    } else {
                        evento.Encaminhado_Para = oLogs.Valor.ToUpper();
                    }
                }
            }

            if (evento.Dt_De != null && evento.Dt_De != "") {
                if (evento.Status == null && Ultimo_Status != "") {
                    evento.Status = Ultimo_Status;
                }

                if (evento.Encaminhado_Para == null && Ultimo_Encaminhado_Para != "") {
                    evento.Encaminhado_Para = Ultimo_Encaminhado_Para;
                }

                if (evento.Status == "CLOSED" || evento.Status == "CANCELLED") {
                    evento.Dt_Ate = evento.Dt_De;
                } else {
                    evento.Dt_Ate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                evento.Tempo_Util_Min = (long)DataEHora.BusinessTimeDelta(
                    DateTime.ParseExact(evento.Dt_De, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    DateTime.ParseExact(evento.Dt_Ate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                ).TotalMinutes;

                evento.Tempo_Decorrido_Min = DataEHora.DateDiff(
                    DataEHora.DateInterval.Minute,
                    DateTime.ParseExact(evento.Dt_De, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    DateTime.ParseExact(evento.Dt_Ate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                );

                List_Eventos.Add(evento);
            }

            return List_Eventos;
        }

        private static void Insert_Defeito_Tempos(Projeto projeto, string defeito, Evento evento) {
            Connection SGQConn = new Connection();

            string sql = $@"
                INSERT INTO ALM_Defeitos_Tempos (
                    Subprojeto,
                    Entrega,
                    Defeito,
                    Status,
                    Encaminhado_Para,
                    Operador,
                    Dt_De,
                    Dt_Ate,
                    Tempo_Decorrido,
                    Tempo_Util)
                SELECT
                    '{projeto.Subprojeto}',
                    '{projeto.Entrega}',
                    {defeito},
                    '{evento.Status}',
                    '{evento.Encaminhado_Para}',
                    '{evento.Operador}',
                    '{evento.Dt_De.Substring(8, 2) + "-" + evento.Dt_De.Substring(5, 2) + "-" + evento.Dt_De.Substring(2, 2) + " " + evento.Dt_De.Substring(11, 8)}',
                    '{evento.Dt_Ate.Substring(8, 2) + "-" + evento.Dt_Ate.Substring(5, 2) + "-" + evento.Dt_Ate.Substring(2, 2) + " " + evento.Dt_Ate.Substring(11, 8)}',
                    {evento.Tempo_Decorrido_Min.ToString()},
                    {evento.Tempo_Util_Min.ToString()}
                WHERE 
                    NOT EXISTS 
                        (
                            select 1 
		                    from 
                                ALM_Defeitos_Tempos 
		                    where 
                                Subprojeto='{projeto.Subprojeto}' and  
                                Entrega='{projeto.Entrega}' and  
			                    Defeito={defeito} and
			                    Dt_De='{evento.Dt_De.Substring(8, 2) + "-" + evento.Dt_De.Substring(5, 2) + "-" + evento.Dt_De.Substring(2, 2) + " " + evento.Dt_De.Substring(11, 8)}'
                        )
                ";

            SGQConn.Executar(sql);
            SGQConn.Dispose();
        }

        private static void Update_Defeito_Tempos(Projeto projeto, string defeito, Evento evento) {
            Connection SGQConn = new Connection();

            string sql = $@"
            UPDATE ALM_Defeitos_Tempos
            SET 
                Status = '{evento.Status}',
                Encaminhado_Para = '{evento.Encaminhado_Para}',
                Dt_Ate = '{evento.Dt_Ate.Substring(8, 2) + "-" + evento.Dt_Ate.Substring(5, 2) + "-" + evento.Dt_Ate.Substring(2, 2) + " " + evento.Dt_Ate.Substring(11, 8)}',
                Operador = '{evento.Operador}',
                Tempo_Decorrido = {evento.Tempo_Decorrido_Min},
                Tempo_Util = {evento.Tempo_Util_Min}
            WHERE 
                Subprojeto = '{projeto.Subprojeto}' and  
                Entrega = '{projeto.Entrega}' and  
                Defeito = {defeito} and
                Dt_De = '{evento.Dt_De.Substring(8, 2) + "-" + evento.Dt_De.Substring(5, 2) + "-" + evento.Dt_De.Substring(2, 2) + " " + evento.Dt_De.Substring(11, 8)}' and
                (
                    Status <> '{evento.Status}' or 
                    Encaminhado_Para <> '{evento.Encaminhado_Para}' or 
                    Dt_Ate <> '{evento.Dt_Ate.Substring(8, 2) + "-" + evento.Dt_Ate.Substring(5, 2) + "-" + evento.Dt_Ate.Substring(2, 2) + " " + evento.Dt_Ate.Substring(11, 8)}' or 
                    Operador <> '{evento.Operador}' or 
                    Tempo_Decorrido <> {evento.Tempo_Decorrido_Min} or 
                    Tempo_Util <> {evento.Tempo_Util_Min}
                )
            ";

            SGQConn.Executar(sql);
            SGQConn.Dispose();
        }
    }
}
