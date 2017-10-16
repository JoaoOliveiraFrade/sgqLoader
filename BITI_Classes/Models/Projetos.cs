using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.biti
{
    public class Projetos
    {
        public List<Field> fields { get; set; }

        public TypeUpdate typeUpdate { get; set; }

        public Sql sql { get; set; }

        public Projetos(TypeUpdate typeUpdate) {
            this.typeUpdate = typeUpdate;

            this.sql = new Sql();

            this.sql.fields = new List<Field>();
            this.sql.fields.Add(new Field() { target = "id", source = "id_projeto", key = true });
            this.sql.fields.Add(new Field() { target = "ideia", source = "id_ideia" });
            this.sql.fields.Add(new Field() { target = "nome", source = "upper(replace(titulo_projeto,'''',''))" });
            this.sql.fields.Add(new Field() { target = "descricao", source = "replace(descricao_executiva,'''','')" });
            this.sql.fields.Add(new Field() { target = "status", source = "upper(replace(status,'''',''))" });
            this.sql.fields.Add(new Field() { target = "estado", source = "upper(replace(estado,'''',''))" });
            this.sql.fields.Add(new Field() { target = "tipo", source = "upper(replace(tipo_projeto,'''',''))" });
            this.sql.fields.Add(new Field() { target = "complexidade", source = "upper(replace(complexidade,'''',''))" });
            this.sql.fields.Add(new Field() { target = "posicao_im", source = "upper(replace(posicao_im,'''',''))" });
            this.sql.fields.Add(new Field() { target = "sistema_principal", source = "upper(replace(sistema_principal,'''',''))" });
            this.sql.fields.Add(new Field() { target = "gerencia_relacionamento", source = "upper(replace(gerencia_relacionamento,'''',''))" });
            this.sql.fields.Add(new Field() { target = "unidade_negocio", source = "upper(replace(unidade_negocio,'''',''))" });
            this.sql.fields.Add(new Field() { target = "diretoria_ti", source = "upper(replace(diretoria_ti,'''',''))" });
            this.sql.fields.Add(new Field() { target = "area_solicitante", source = "upper(replace(area_solicitante,'''',''))" });
            this.sql.fields.Add(new Field() { target = "gestor_solicitante", source = "upper(replace(gestor_solicitante,'''',''))" });
            this.sql.fields.Add(new Field() { target = "usuario_Solicitante", source = "upper(replace(usuario_solicitante_projeto,'''',''))" });
            this.sql.fields.Add(new Field() { target = "analista_negocio", source = "upper(replace(analista_negocio,'''',''))" });
            this.sql.fields.Add(new Field() { target = "gerente_projeto", source = "upper(replace(gerente_projeto,'''',''))" });
            this.sql.fields.Add(new Field() { target = "gestor_direto_lt", source = "upper(replace(gestor_direto_lt,'''',''))" });
            this.sql.fields.Add(new Field() { target = "gestor_do_gestor_lt", source = "upper(replace(gestor_do_gestor_lt,'''',''))" });
            this.sql.fields.Add(new Field() { target = "pmo", source = "upper(replace(pmo,'''',''))" });
            this.sql.fields.Add(new Field() { target = "prioridade_geral", source = "prioridade_global", type = "N" });
            this.sql.fields.Add(new Field() { target = "prioridade_un", source = "prioridade_un", type = "N" });
            this.sql.fields.Add(new Field() { target = "tipificacao", source = "upper(replace(tipificacao,'''',''))" });
            this.sql.fields.Add(new Field() { target = "tipificacao_pos_priorizacao", source = "upper(replace(tipificacao_pos_priorizacao,'''',''))" });
            this.sql.fields.Add(new Field() { target = "custo_total", source = "custo_total", type = "N" });
            this.sql.fields.Add(new Field() { target = "esforco_total", source = "esforco_total", type = "N" });
            this.sql.fields.Add(new Field() { target = "apuravel", source = "apuravel" });
            this.sql.fields.Add(new Field() { target = "macro_estimativa_total", source = "macroestimativa_total_projeto", type = "N" });
            this.sql.fields.Add(new Field() { target = "particao", source = "upper(particao)" });
            this.sql.fields.Add(new Field() { target = "dt_criacao", source = "convert(varchar,data_criacao,5) + ' ' + convert(varchar(8), data_criacao, 8)" });
            this.sql.fields.Add(new Field() { target = "criador", source = "upper(replace(criador,'''',''))" });
            this.sql.fields.Add(new Field() { target = "dt_atualizacao", source = "convert(varchar,data_ultima_atualizacao,5) + ' ' + convert(varchar(8), data_ultima_atualizacao, 8)" });
            this.sql.fields.Add(new Field() { target = "atualizador", source = "upper(replace(ultimo_atualizador,'''',''))" });

            this.sql.dataSource = "tb_ft_projeto";
            this.sql.targetTable = "biti_projetos";
            this.sql.targetSqlLastIdInserted = "select max(id) from biti_projetos";
            this.sql.targetSqlLastDateUpdate = @"
                select 
                    substring(valor,9,2) + '-' + 
                    substring(valor,6,2) + '-' + 
                    substring(valor,3,2) + ' ' + 
                    substring(valor,12,8) 
                from 
                    sgq_parametros 
                where 
                    nome = 'BITI_Projetos_Update'
            ";

            this.sql.dataSourceFilterCondition = "";
            this.sql.dataSourceFilterConditionInsert = $" id_projeto > '{this.sql.targetLastIdInserted}'";
            this.sql.dataSourceFilterConditionUpdate = $" substring(convert(varchar, data_ultima_atualizacao, 120),3,17) > '{this.sql.targetLastDateUpdate}'";
            this.sql.typeDB = "SQL SERVER";
        }

        public void LoadData() {
            DateTime Dt_Inicio = DateTime.Now;

            Connection SGQConn = new Connection();
            Connection BITIConn = new Connection(Bancos.Biti);

            if (typeUpdate == TypeUpdate.Increment || typeUpdate == TypeUpdate.IncrementFullUpdate) {
                if (typeUpdate == TypeUpdate.IncrementFullUpdate) {
                    SGQConn.Executar("update SGQ_Parametros set Valor = '0000-00-00 00:00:00' where Nome='BITI_Projetos_Update'");
                }
                string Sql_Insert = this.sql.Get_Sql_Insert();
                List<Comando> List_Comandos_Insert = BITIConn.Executar<Comando>(Sql_Insert);
                SGQConn.Executar(List_Comandos_Insert, 1);

                string Sql_Update = this.sql.Get_Sql_Update();
                List<Comando> List_Comandos_Update = BITIConn.Executar<Comando>(Sql_Update);
                SGQConn.Executar(List_Comandos_Insert, 1);

            } else if (typeUpdate == TypeUpdate.Full) {
                SGQConn.Executar("truncate table tb_ft_projeto");

                string Sql_Insert = this.sql.Get_Sql_Insert();
                List<Comando> List_Comandos_Insert = BITIConn.Executar<Comando>(Sql_Insert);
                SGQConn.Executar(List_Comandos_Insert, 1);
            }
            DateTime Dt_Fim = DateTime.Now;
            SGQConn.Executar($"update SGQ_Parametros set Valor = '{Dt_Inicio.ToString("yyyy-MM-dd HH:mm:ss")}' where Nome='BITI_Projetos_Update'");
            SGQConn.Dispose();
        }
    }
}
