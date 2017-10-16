delete SGQ_Releases_Etapas
where
    SGQ_Releases_Etapas.id in
	(
        select distinct re.id
        from
            SGQ_Releases_Etapas re
            left join SGQ_Releases r
                on r.mes = re.mes and r.ano = re.ano
        where
            r.mes is null
	)
-------------------------------------------------------
insert into SGQ_Releases_Etapas(mes, ano, etapa)
select distinct r.mes, r.ano, 'TI' as etapa
from
    SGQ_Releases r
    left join SGQ_Releases_Etapas re
        on re.mes = r.mes and re.ano = r.ano and re.Etapa = 'TI'
where re.mes is null
-------------------------------------------------------
insert into SGQ_Releases_Etapas(mes, ano, etapa)
select distinct r.mes, r.ano, 'UAT' as etapa
from
    SGQ_Releases r
    left join SGQ_Releases_Etapas re
        on re.mes = r.mes and re.ano = r.ano and re.Etapa = 'UAT'
where re.mes is null
-------------------------------------------------------
insert into SGQ_Releases_Etapas(mes, ano, etapa)
select distinct r.mes, r.ano, 'TRG' as etapa
from
    SGQ_Releases r
    left join SGQ_Releases_Etapas re
        on re.mes = r.mes and re.ano = r.ano and re.Etapa = 'TRG'
where re.mes is null
-------------------------------------------------------
declare @Temp_SGQ_Releases_Etapas table (
	Mes int, 
	Ano int, 
	Dt_Primeira_Execucao_TI datetime, 
	Dt_Ultima_Execucao_TI datetime, 
	Dt_Primeira_Execucao_UAT datetime, 
	Dt_Ultima_Execucao_UAT datetime, 
	Dt_Primeira_Execucao_TRG datetime, 
	Dt_Ultima_Execucao_TRG datetime);

insert into @Temp_SGQ_Releases_Etapas
select
	Mes,
	Ano,
	min(Dt_Primeira_Execucao_TI) as Dt_Primeira_Execucao_TI,
	max(Dt_Ultima_Execucao_TI) as Dt_Ultima_Execucao_TI,
	min(Dt_Primeira_Execucao_UAT) as Dt_Primeira_Execucao_UAT,
	max(Dt_Ultima_Execucao_UAT) as Dt_Ultima_Execucao_UAT,
	min(Dt_Primeira_Execucao_TRG) as Dt_Primeira_Execucao_TRG,
	max(Dt_Ultima_Execucao_TRG) as Dt_Ultima_Execucao_TRG
from
	(select 
		re.Release_Mes as Mes,
		re.Release_Ano as Ano,
		re.Subprojeto,
		re.Entrega,

		(select min(convert(datetime, cts.Dt_Execucao,5))
		from alm_cts cts WITH (NOLOCK)
		where
			cts.subprojeto = re.subprojeto and
			cts.entrega = re.entrega and
			cts.status_exec_ct = 'PASSED' and
			cts.Ciclo like '%TI%' and
            cts.Dt_Execucao <> ''
		) as Dt_Primeira_Execucao_TI,

		(select max(convert(datetime, cts.Dt_Execucao,5))
		from alm_cts cts WITH (NOLOCK)
		where
			cts.subprojeto = re.subprojeto and
			cts.entrega = re.entrega and
			cts.status_exec_ct = 'PASSED' and
			cts.Ciclo like '%TI%' and
            cts.Dt_Execucao <> ''
		) as Dt_Ultima_Execucao_TI,

		(select min(convert(datetime, cts.Dt_Execucao,5))
		from alm_cts cts WITH (NOLOCK)
		where
			cts.subprojeto = re.subprojeto and
			cts.entrega = re.entrega and
			cts.status_exec_ct = 'PASSED' and
			(cts.UAT = 'SIM' or cts.Ciclo like '%UAT%') and
            cts.Dt_Execucao <> ''
		) as Dt_Primeira_Execucao_UAT,

		(select max(convert(datetime, cts.Dt_Execucao,5))
		from alm_cts cts WITH (NOLOCK)
		where
			cts.subprojeto = re.subprojeto and
			cts.entrega = re.entrega and
			cts.status_exec_ct = 'PASSED' and
			(cts.UAT = 'SIM' or cts.Ciclo like '%UAT%') and
            cts.Dt_Execucao <> ''
		) as Dt_Ultima_Execucao_UAT,

		(select min(convert(datetime, cts.Dt_Execucao,5))
		from alm_cts cts WITH (NOLOCK)
		where
			subprojeto = 'TRG2017'and
			Ciclo like ('%' + (select sigla from SGQ_Meses where id = re.Release_Mes) + '/' + Substring(cast(re.Release_Ano as varchar),3, 2) + '%') and
			cts.status_exec_ct = 'PASSED' and
            cts.Dt_Execucao <> ''
		) as Dt_Primeira_Execucao_TRG,

		(select max(convert(datetime, cts.Dt_Execucao,5))
		from alm_cts cts WITH (NOLOCK)
		where
			subprojeto = 'TRG2017'and
			Ciclo like ('%' + (select sigla from SGQ_Meses where id = re.Release_Mes) + '/' + Substring(cast(re.Release_Ano as varchar),3, 2) + '%') and
			cts.status_exec_ct = 'PASSED' and
            cts.Dt_Execucao <> ''
		) as Dt_Ultima_Execucao_TRG
	from
		SGQ_Releases_Entregas re WITH (NOLOCK)
	where
		re.Motivo_Perda_Release is null
		--re.subprojeto = 'PRJ00000174' and 
		--re.entrega = 'ENTREGA00000419'
	) Aux
group by 
	Mes,
	Ano
-------------------------------------------------------
update SGQ_Releases_Etapas
set 
	SGQ_Releases_Etapas.Dt_Inicio_Real = Aux.Dt_Primeira_Execucao_TI,
	SGQ_Releases_Etapas.Dt_Fim_Real = Aux.Dt_Ultima_Execucao_TI
from
	@Temp_SGQ_Releases_Etapas Aux
where
	SGQ_Releases_Etapas.Mes = Aux.Mes and
	SGQ_Releases_Etapas.Ano = Aux.Ano and
	SGQ_Releases_Etapas.Etapa = 'TI' and
    (
	SGQ_Releases_Etapas.Dt_Inicio_Real <> Aux.Dt_Primeira_Execucao_TI or
	SGQ_Releases_Etapas.Dt_Fim_Real <> Aux.Dt_Ultima_Execucao_TI
    )
-------------------------------------------------------
update SGQ_Releases_Etapas
set 
	SGQ_Releases_Etapas.Dt_Inicio_Real = Aux.Dt_Primeira_Execucao_UAT,
	SGQ_Releases_Etapas.Dt_Fim_Real = Aux.Dt_Ultima_Execucao_UAT
from
	@Temp_SGQ_Releases_Etapas Aux
where
	SGQ_Releases_Etapas.Mes = Aux.Mes and
	SGQ_Releases_Etapas.Ano = Aux.Ano and
	SGQ_Releases_Etapas.Etapa = 'UAT' and
    (
	SGQ_Releases_Etapas.Dt_Inicio_Real <> Aux.Dt_Primeira_Execucao_UAT or
	SGQ_Releases_Etapas.Dt_Fim_Real <> Aux.Dt_Ultima_Execucao_UAT
    )
-------------------------------------------------------
update SGQ_Releases_Etapas
set 
	SGQ_Releases_Etapas.Dt_Inicio_Real = Aux.Dt_Primeira_Execucao_TRG,
	SGQ_Releases_Etapas.Dt_Fim_Real = Aux.Dt_Ultima_Execucao_TRG
from
	@Temp_SGQ_Releases_Etapas Aux
where
	SGQ_Releases_Etapas.Mes = Aux.Mes and
	SGQ_Releases_Etapas.Ano = Aux.Ano and
	SGQ_Releases_Etapas.Etapa = 'TRG' and
    (
	SGQ_Releases_Etapas.Dt_Inicio_Real <> Aux.Dt_Primeira_Execucao_TRG or
	SGQ_Releases_Etapas.Dt_Fim_Real <> Aux.Dt_Ultima_Execucao_TRG
    )