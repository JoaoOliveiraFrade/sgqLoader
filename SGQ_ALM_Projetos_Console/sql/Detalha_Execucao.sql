delete SGQ_Detalha_Execucao where dia = convert(varchar, getdate(),5);

insert into SGQ_Detalha_Execucao
(
	subprojeto,
	entrega,
	Dia,
	Massa_Teste,
	OK,
	Em_Exec,
	Erro,
	Bloqueado,
	NA,
	OK_Reteste,
	Em_Exec_Reteste,
	Erro_Reteste,
	Bloqueado_Reteste,
	NA_Reteste
)
select
    Ex2.subprojeto,
    Ex2.entrega,
    right(Data,2) + '-' + substring(Data,3,2) + '-' + left(Data,2) as Dia,
    Upper(CT.Massa_Teste) as Massa_Teste,
    sum(case when Ex2.Reteste = 0 then Ex2.OK else 0 end) as OK,
    sum(case when Ex2.Reteste = 0 then Ex2.Em_Exec else 0 end) as Em_Exec,
    sum(case when Ex2.Reteste = 0 then Ex2.Erro else 0 end) as Erro,
    sum(case when Ex2.Reteste = 0 then Ex2.Bloqueado else 0 end) as Bloqueado,
    sum(case when Ex2.Reteste = 0 then Ex2.NA else 0 end) as NA,

    sum(case when Ex2.Reteste = 1 then Ex2.OK else 0 end) as OK_Reteste,
    sum(case when Ex2.Reteste = 1 then Ex2.Em_Exec else 0 end) as Em_Exec_Reteste,
    sum(case when Ex2.Reteste = 1 then Ex2.Erro else 0 end) as Erro_Reteste,
    sum(case when Ex2.Reteste = 1 then Ex2.Bloqueado else 0 end) as Bloqueado_Reteste,
    sum(case when Ex2.Reteste = 1 then Ex2.NA else 0 end) as NA_Reteste
from
        (
        select
                Ex.subprojeto,
                Ex.entrega,
                substring(Ex.Dt_Execucao,7,2) + substring(Ex.Dt_Execucao,4,2) + substring(Ex.Dt_Execucao,1,2) as Data,
                Ex.CT,
                (case when Ex.status = 'PASSED' then 1 else 0 end) as OK,
                (case when Ex.status = 'NOT COMPLETED' then 1 else 0 end) as Em_Exec,
                (case when Ex.status = 'FAILED' then 1 else 0 end) as Erro,
                (case when Ex.status = 'BLOCKED' then 1 else 0 end) as Bloqueado,
                (case when Ex.status in('N/A','') then 1 else 0 end) as NA,

                (case when exists
                    (   select top 1 1
                                    from ALM_Historico_Alteracoesfields ht WITH (NOLOCK)
                                    where 
                                                ht.tabela = 'TESTCYCL' and 
                                                ht.Campo = 'STATUS' and 
                                                ht.Novo_valor = 'PASSED' and
                                                ht.subprojeto = Ex.subprojeto and 
                                                ht.entrega = Ex.entrega and
                                                ht.tabela_id = Ex.CT and
                                                convert(datetime, ht.dt_alteracao,5) < convert(datetime, Ex.Dt_Execucao,5)
                    )
                    then 1
                    else 0
                end) as Reteste
        from 
                ALM_Execucoes Ex WITH (NOLOCK)
        where 
                substring(Ex.Dt_Execucao,7,2) + substring(Ex.Dt_Execucao,4,2) + substring(Ex.Dt_Execucao,1,2) = right(convert(varchar(30),dateadd(dd, 0, getdate()),112),6)  and
                --substring(Ex.Dt_Execucao,7,2) + substring(Ex.Dt_Execucao,4,2) + substring(Ex.Dt_Execucao,1,2) = '160130' and
                Status <> 'CANCELLED' --and
                --Ex.subprojeto = 'PRJ00004929' and 
                --Ex.entrega = 'ENTREGA00001639'
        ) Ex2
        left join ALM_CTs CT 
            on CT.subprojeto = Ex2.subprojeto and 
            CT.entrega = Ex2.entrega and 
                CT.CT = Ex2.CT
group by
    Ex2.subprojeto,
    Ex2.entrega,
    Ex2.Data,
        CT.Massa_Teste
order by
    Ex2.subprojeto,
    Ex2.entrega,
    Ex2.Data,
    CT.Massa_Teste