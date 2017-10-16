declare @t table (
	statusStrategyTestingAndContracting varchar(10),
	readyStrategyTestingAndContracting varchar(1),
	statusTimeline varchar(10),
	readyTimeline varchar(1),
	statusTestPlan varchar(10),
	readyTestPlan varchar(1)
)
insert into @t
select 
	statusStrategyTestingAndContracting,
	readyStrategyTestingAndContracting,
	statusTimeline,
	readyTimeline,
	statusTestPlan,
	readyTestPlan
from 
	sgq_projects sgq_p
	left join BITI_Subprojetos sub
		on sub.id = sgq_p.subproject
where
	(
		sgq_p.RT in ('CARLOS HENRIQUE', 'SORAIA CASAGRANDE', 'CLAUDIA CARVALHO', '') or
		isnull(sgq_p.dtUpdateStrategyTestingAndContracting,'') <> '' or
		isnull(sgq_p.dtUpdateTimeline,'') <>'' or
		isnull(sgq_p.dtUpdateTestPlan,'') <> ''
	) and
	sub.estado <> 'CANCELADO' and
	sgq_p.subproject in 
		(
			select distinct ft.subprojeto
			from BITI_Frentes_Trabalho ft
			where ft.subprojeto = sgq_p.subproject and
					ft.area in ('TESTES E RELEASE', 'SUPORTE E PROJETOS', 'TRANSFORMACAO DE BSS') and
					ft.estado not in ('CANCELADA', 'CANCELADA SEM DESENHO', 'PARTICIPAÇÃO RECUSADA') and
					ft.sistema_nome = 'NÃO INFORMADO'
		)

delete SGQ_PulledChainHistoric where data = convert(varchar(8), getdate(), 5)

insert into SGQ_PulledChainHistoric 
select * 
from
	(
		select
			convert(varchar(8), getdate(), 5) as data,
			'Estratégia' as Atividade,
			sum(case when 
				statusStrategyTestingAndContracting = 'BACKLOG' and readyStrategyTestingAndContracting = 'N' 
				then 1 
				else 0 
			end) as [Backlog Not Ready],
			sum(case when 
				statusStrategyTestingAndContracting = 'BACKLOG' and readyStrategyTestingAndContracting = 'S' 
				then 1 
				else 0 
			end) as [Backlog Ready],
			sum(case when 
				statusStrategyTestingAndContracting = 'ON GOING'
				then 1 
				else 0 
			end) as [On Going],
			sum(case when 
				statusStrategyTestingAndContracting = 'REALIZADO'
				then 1 
				else 0 
			end) as Finalizado
		from
			@t

		union all

		select
			convert(varchar(8), getdate(), 5) as data,
			'Cronograma' as Atividade,
			sum(case when 
				statusTimeline = 'BACKLOG' and readyTimeline = 'N' 
				then 1 
				else 0 
			end) as [Backlog Not Ready],
			sum(case when 
				statusTimeline = 'BACKLOG' and readyTimeline = 'S' 
				then 1 
				else 0 
			end) as [Backlog Ready],
			sum(case when 
				statusTimeline = 'ON GOING'
				then 1 
				else 0 
			end) as [On Going],
			sum(case when 
				statusTimeline = 'REALIZADO'
				then 1 
				else 0 
			end) as Finalizado
		from
			@t

		union all

		select
			convert(varchar(8), getdate(), 5) as data,
			'Plano' as Atividade,
			sum(case when 
				statusTestPlan = 'BACKLOG' and readyTestPlan = 'N' 
				then 1 
				else 0 
			end) as [Backlog Not Ready],
			sum(case when 
				statusTestPlan = 'BACKLOG' and readyTestPlan = 'S' 
				then 1 
				else 0 
			end) as [Backlog Ready],
			sum(case when 
				statusTestPlan = 'ON GOING'
				then 1 
				else 0 
			end) as [On Going],
			sum(case when 
				statusTestPlan = 'REALIZADO'
				then 1 
				else 0 
			end) as Finalizado
		from
			@t
	) aux1
