delete SGQ_Projetos_Etapas
where
    SGQ_Projetos_Etapas.id in
	(
        select distinct se.Id
        from
            SGQ_Projetos_Etapas se
            left join ALM_CTs cts
                on cts.subprojeto = se.subprojeto and cts.entrega = se.entrega and CTs.Status_Exec_Teste <> 'CANCELLED'
        where
            cts.subprojeto is null
	)
-------------------------------------------------------
insert into SGQ_Projetos_Etapas(subprojeto, entrega, etapa)
select distinct cts.subprojeto, cts.entrega, 'TI' as etapa
from
    ALM_CTs cts
    left join SGQ_Projetos_Etapas se
        on se.subprojeto = cts.subprojeto and se.entrega = cts.entrega and se.Etapa = 'TI'
where cts.subprojeto <> 'TRG2017' and cts.status_exec_teste <> 'CANCELLED' and se.subprojeto is null
-------------------------------------------------------
insert into SGQ_Projetos_Etapas(subprojeto, entrega, etapa)
select distinct cts.subprojeto, cts.entrega, 'UAT' as etapa
from
    ALM_CTs cts
    left join SGQ_Projetos_Etapas se
        on se.subprojeto = cts.subprojeto and se.entrega = cts.entrega and se.Etapa = 'UAT'
where cts.subprojeto <> 'TRG2017' and cts.status_exec_teste <> 'CANCELLED' and se.subprojeto is null
-------------------------------------------------------
declare @Temp_SGQ_Projetos_Etapas table (
	subprojeto varchar(30), 
	entrega varchar(30), 
	Dt_Primeira_Execucao_TI datetime, 
	Dt_Ultima_Execucao_TI datetime, 
	Dt_Primeira_Execucao_UAT datetime, 
	Dt_Ultima_Execucao_UAT datetime);

insert into @Temp_SGQ_Projetos_Etapas
select 
	subprojeto,
	entrega,
	min(convert(datetime, case when cts.Ciclo like '%TI%' then cts.Dt_Execucao end,5)) as Dt_Primeira_Execucao_TI,
	max(convert(datetime, case when cts.Ciclo like '%TI%' then cts.Dt_Execucao end,5)) as Dt_Ultima_Execucao_TI,
	min(convert(datetime, case when cts.UAT = 'SIM' or cts.Ciclo like '%UAT%' then cts.Dt_Execucao end,5)) as Dt_Primeira_Execucao_UAT,
	max(convert(datetime, case when cts.UAT = 'SIM' or cts.Ciclo like '%UAT%' then cts.Dt_Execucao end,5)) as Dt_Ultima_Execucao_UAT
from 
	alm_cts cts WITH (NOLOCK)
where
	cts.status_exec_ct = 'PASSED' and
    cts.Dt_Execucao <> ''
group by
	subprojeto,
	entrega
-------------------------------------------------------
update SGQ_Projetos_Etapas
set 
	SGQ_Projetos_Etapas.Dt_Inicio_Real = Aux.Dt_Primeira_Execucao_TI,
	SGQ_Projetos_Etapas.Dt_Fim_Real = Aux.Dt_Ultima_Execucao_TI
from
	@Temp_SGQ_Projetos_Etapas Aux
where
	SGQ_Projetos_Etapas.subprojeto = Aux.subprojeto and
	SGQ_Projetos_Etapas.entrega = Aux.entrega and
	SGQ_Projetos_Etapas.Etapa = 'TI' and
	(
	SGQ_Projetos_Etapas.Dt_Inicio_Real <> Aux.Dt_Primeira_Execucao_TI or
	SGQ_Projetos_Etapas.Dt_Fim_Real <> Aux.Dt_Ultima_Execucao_TI
	)
-------------------------------------------------------
update SGQ_Projetos_Etapas
set 
	SGQ_Projetos_Etapas.Dt_Inicio_Real = Aux.Dt_Primeira_Execucao_UAT,
	SGQ_Projetos_Etapas.Dt_Fim_Real = Aux.Dt_Ultima_Execucao_UAT
from
	@Temp_SGQ_Projetos_Etapas Aux
where
	SGQ_Projetos_Etapas.subprojeto = Aux.subprojeto and
	SGQ_Projetos_Etapas.entrega = Aux.entrega and
	SGQ_Projetos_Etapas.Etapa = 'UAT' and
	(
	SGQ_Projetos_Etapas.Dt_Inicio_Real <> Aux.Dt_Primeira_Execucao_UAT or
	SGQ_Projetos_Etapas.Dt_Fim_Real <> Aux.Dt_Ultima_Execucao_UAT
	)