insert into SGQ_Historico_Valores
select 
  getdate() as Data,
  Release_Mes,
  Release_Ano,
  'EXECUÇÃO TESTES' as Tipo,
  (case when Ativos > 0 and Ativos = Realizados then 'CONCLUÍDA' else 'PENDENTE' end) as Item,
  count(*) as Valor
from
(
       select
             Release_Mes,
             Release_Ano,
             Subprojeto,
             Entrega,
             SUM(Ativo) as Ativos,
             SUM(Realizado) as Realizados
       from   
          (select 
                    Release_Mes,
                    Release_Ano,
                    re.Subprojeto,
                    re.Entrega,
                    case when (Status_Exec_CT <> 'CANCELLED')  then 1 else 0 end as Ativo,
                    case when 
                                  (Status_Exec_CT = 'PASSED' and Dt_Execucao <> '') and 
                                  (substring(Dt_Execucao,7,2) + substring(Dt_Execucao,4,2) + substring(Dt_Execucao,1,2)) <= right(convert(varchar(30),dateadd(dd, -0, getdate()),112),6)  then 1 else 0 end as Realizado
             from 
                    SGQ_Releases_Entregas re 
                    left join alm_cts 
                       on alm_cts.subprojeto = re.subprojeto and alm_cts.entrega = re.entrega
                    left join BITI_Subprojetos sp 
                       on sp.id = re.subprojeto
             where
                    (select Status from SGQ_Releases where id = re.Release) = 2 and
					re.Exibir_Status_Diario = 'True'
          ) as Aux
       group by 
             Release_Mes,
             Release_Ano,
             Subprojeto,
             Entrega
) as Aux2
group by
       Release_Mes,
       Release_Ano,
       (case when Aux2.Ativos > 0 and Aux2.Ativos = Aux2.Realizados then 'CONCLUÍDA' else 'PENDENTE' end)
