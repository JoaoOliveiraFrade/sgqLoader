delete from SGQ_Releases_Sistemas_Etapas
where
    SGQ_Releases_Sistemas_Etapas.id in
		(
        select
            rs.id
        from
            SGQ_Releases_Sistemas_Etapas rs
            left join
            (
            select distinct
                re.release_mes as mes,
                re.release_ano as ano,
                cts.sistema
            from
                SGQ_Releases_Entregas re
                inner join alm_cts cts
                    on cts.subprojeto = re.subprojeto and cts.entrega = re.entrega
            where
                re.motivo_perda_release is null and
                cts.status_exec_ct <> 'CANCELLED'
			) aux
                on aux.mes = rs.mes and aux.ano = rs.ano and aux.sistema = rs.sistema
        where
            aux.mes is null
		)
-------------------------------------------------------
insert into SGQ_Releases_Sistemas_Etapas(mes, ano, sistema, etapa)
select
    aux.mes,
    aux.ano,
    aux.sistema,
    aux.etapa
from
    (
    select distinct
        re.release_mes as mes,
        re.release_ano as ano,
        cts.sistema,
        'TI' as Etapa
    from
        SGQ_Releases_Entregas re
        inner join alm_cts cts
            on cts.subprojeto = re.subprojeto and cts.entrega = re.entrega
    where
        re.motivo_perda_release is null and
        cts.status_exec_ct <> 'CANCELLED'
    ) aux
    left join SGQ_Releases_Sistemas_Etapas rs
        on rs.mes = aux.mes and rs.ano = aux.ano and rs.etapa = aux.etapa
where
    rs.mes is null
-------------------------------------------------------
insert into SGQ_Releases_Sistemas_Etapas(mes, ano, sistema, etapa)
select
    aux.mes,
    aux.ano,
    aux.sistema,
    aux.etapa
from
    (
    select distinct
        re.release_mes as mes,
        re.release_ano as ano,
        cts.sistema,
        'UAT' as Etapa
    from
        SGQ_Releases_Entregas re
        inner join alm_cts cts
            on cts.subprojeto = re.subprojeto and cts.entrega = re.entrega
    where
        re.motivo_perda_release is null and
        cts.status_exec_ct <> 'CANCELLED'
    ) aux
    left join SGQ_Releases_Sistemas_Etapas rs
        on rs.mes = aux.mes and rs.ano = aux.ano and rs.etapa = aux.etapa
where
    rs.mes is null
-------------------------------------------------------
insert into SGQ_Releases_Sistemas_Etapas(mes, ano, sistema, etapa)
select
    aux.mes,
    aux.ano,
    aux.sistema,
    aux.etapa
from
    (
    select distinct
        re.release_mes as mes,
        re.release_ano as ano,
        cts.sistema,
        'TRG' as Etapa
    from
        SGQ_Releases_Entregas re
        inner join alm_cts cts
            on cts.subprojeto = re.subprojeto and cts.entrega = re.entrega
    where
        re.motivo_perda_release is null and
        cts.status_exec_ct <> 'CANCELLED'
    ) aux
    left join SGQ_Releases_Sistemas_Etapas rs
        on rs.mes = aux.mes and rs.ano = aux.ano and rs.etapa = aux.etapa
where
    rs.mes is null
-------------------------------------------------------
declare @Temp_SGQ_Releases_Sistemas_Etapas table (
	Mes int, 
	Ano int, 
	Sistema varchar(100),
	Dt_Primeira_Execucao_TI datetime, 
	Dt_Ultima_Execucao_TI datetime, 
	Dt_Primeira_Execucao_UAT datetime, 
	Dt_Ultima_Execucao_UAT datetime, 
	Dt_Primeira_Execucao_TRG datetime, 
	Dt_Ultima_Execucao_TRG datetime
	);

insert into @Temp_SGQ_Releases_Sistemas_Etapas
select
	Mes,
	Ano,
	Sistema,
	Dt_Primeira_Execucao_TI,
	Dt_Ultima_Execucao_TI,
	Dt_Primeira_Execucao_UAT,
	Dt_Ultima_Execucao_UAT,

	(select min(convert(datetime, cts.Dt_Execucao,5))
	from ALM_cts cts WITH (NOLOCK)
	where
		Subprojeto = 'TRG2017'and
		Ciclo like ('%' + (select sigla from SGQ_Meses where id = Aux.Mes) + '/' + Substring(cast(Aux.Ano as varchar),3, 2) + '%') and
		Ciclo like ('%' + Aux.Sistema + '%') and
		cts.Status_Exec_CT = 'PASSED' and
		cts.Dt_Execucao <> ''
	) as Dt_Primeira_Execucao_TRG,

	(select max(convert(datetime, cts.Dt_Execucao,5))
	from ALM_cts cts WITH (NOLOCK)
	where
		Subprojeto = 'TRG2017'and
		Ciclo like ('%' + (select sigla from SGQ_Meses where id = Aux.Mes) + '/' + Substring(cast(Aux.Ano as varchar),3, 2) + '%') and
		Ciclo like ('%' + Aux.Sistema + '%') and
		cts.Status_Exec_CT = 'PASSED' and
		cts.Dt_Execucao <> ''
	) as Dt_Ultima_Execucao_TRG
from
	(
	select 
		re.release_mes as Mes,
		re.release_ano as Ano,
		cts.sistema,
		min(convert(datetime, case when cts.Ciclo like '%TI%' then cts.Dt_Execucao end,5)) as Dt_Primeira_Execucao_TI,
		max(convert(datetime, case when cts.Ciclo like '%TI%' then cts.Dt_Execucao end,5)) as Dt_Ultima_Execucao_TI,
		min(convert(datetime, case when (cts.UAT = 'SIM' or cts.Ciclo like '%UAT%') then cts.Dt_Execucao end,5)) as Dt_Primeira_Execucao_UAT,
		max(convert(datetime, case when (cts.UAT = 'SIM' or cts.Ciclo like '%UAT%') then cts.Dt_Execucao end,5)) as Dt_Ultima_Execucao_UAT
	from
		SGQ_Releases_Entregas re
		inner join alm_cts cts
			on cts.subprojeto = re.subprojeto and cts.entrega = re.entrega
	where
		re.motivo_perda_release is null and
		cts.status_exec_ct = 'PASSED' and
		cts.Dt_Execucao <> ''
	group by
		re.release_mes,
		re.release_ano,
		cts.sistema
	) Aux
order by 2,1,3
-------------------------------------------------------
update SGQ_Releases_Sistemas_Etapas
set 
	SGQ_Releases_Sistemas_Etapas.Dt_Inicio_Real = Aux.Dt_Primeira_Execucao_TI,
	SGQ_Releases_Sistemas_Etapas.Dt_Fim_Real = Aux.Dt_Ultima_Execucao_TI
from
	@Temp_SGQ_Releases_Sistemas_Etapas Aux
where
	SGQ_Releases_Sistemas_Etapas.Mes = Aux.Mes and
	SGQ_Releases_Sistemas_Etapas.Ano = Aux.Ano and
	SGQ_Releases_Sistemas_Etapas.Sistema = Aux.Sistema and
	SGQ_Releases_Sistemas_Etapas.Etapa = 'TI' and
    (
	SGQ_Releases_Sistemas_Etapas.Dt_Inicio_Real <> Aux.Dt_Primeira_Execucao_TI or
	SGQ_Releases_Sistemas_Etapas.Dt_Fim_Real <> Aux.Dt_Ultima_Execucao_TI
    )
-------------------------------------------------------
update SGQ_Releases_Sistemas_Etapas
set 
	SGQ_Releases_Sistemas_Etapas.Dt_Inicio_Real = Aux.Dt_Primeira_Execucao_UAT,
	SGQ_Releases_Sistemas_Etapas.Dt_Fim_Real = Aux.Dt_Ultima_Execucao_UAT
from
	@Temp_SGQ_Releases_Sistemas_Etapas Aux
where
	SGQ_Releases_Sistemas_Etapas.Mes = Aux.Mes and
	SGQ_Releases_Sistemas_Etapas.Ano = Aux.Ano and
	SGQ_Releases_Sistemas_Etapas.Sistema = Aux.Sistema and
	SGQ_Releases_Sistemas_Etapas.Etapa = 'UAT' and
    (
	SGQ_Releases_Sistemas_Etapas.Dt_Inicio_Real <> Aux.Dt_Primeira_Execucao_UAT or
	SGQ_Releases_Sistemas_Etapas.Dt_Fim_Real <> Aux.Dt_Ultima_Execucao_UAT
    )
-------------------------------------------------------
update SGQ_Releases_Sistemas_Etapas
set 
	SGQ_Releases_Sistemas_Etapas.Dt_Inicio_Real = Aux.Dt_Primeira_Execucao_TRG,
	SGQ_Releases_Sistemas_Etapas.Dt_Fim_Real = Aux.Dt_Ultima_Execucao_TRG
from
	@Temp_SGQ_Releases_Sistemas_Etapas Aux
where
	SGQ_Releases_Sistemas_Etapas.Mes = Aux.Mes and
	SGQ_Releases_Sistemas_Etapas.Ano = Aux.Ano and
	SGQ_Releases_Sistemas_Etapas.Sistema = Aux.Sistema and
	SGQ_Releases_Sistemas_Etapas.Etapa = 'TRG' and
    (
	SGQ_Releases_Sistemas_Etapas.Dt_Inicio_Real <> Aux.Dt_Primeira_Execucao_TRG or
	SGQ_Releases_Sistemas_Etapas.Dt_Fim_Real <> Aux.Dt_Ultima_Execucao_TRG
    )