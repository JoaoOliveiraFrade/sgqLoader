declare @t table (
 subproject varchar(11),
 delivery varchar(15),
 currentReleaseMonth int,
 currentReleaseYear int,
 clarityReleaseMonth int,
 clarityReleaseYear int,
 
 priorityGlobal int,
 state varchar(40),
 category varchar(40),
 UN varchar(30),
 workFrontState varchar(30),
 topic varchar(60),
 RT varchar(50),
 deliveryState varchar(30),
 statusCategoryORL varchar(1),

 readyStrategyTestingAndContracting varchar(1),
 readyTimeline varchar(1),
 readyTestPlan varchar(1)
) 
insert into @t (
 subproject,
 delivery,
 currentReleaseMonth,
 currentReleaseYear,
 clarityReleaseMonth,
 clarityReleaseYear,

 priorityGlobal,
 state,
 category,
 UN,
 workFrontState,
 topic,
 RT,
 deliveryState,
 statusCategoryORL,

 readyStrategyTestingAndContracting,
 readyTimeline,
 readyTestPlan
)
select 
	subproject,
	delivery,

	currentReleaseMonth,
	currentReleaseYear,

	clarityReleaseMonth,
	clarityReleaseYear,

	priorityGlobal,
	state,
	category,
	UN,
	max(workFrontState) as workFrontState,
	max(topic) as topic,
	max(RT) as RT,
	deliveryState,
	statusCategoryORL,

	max(readyStrategyTestingAndContracting) as readyStrategyTestingAndContracting,
	max(readyTimeline) as readyTimeline,
	max(readyTestPlan) as readyTestPlan
from
	(
		select 
			subproject,
			delivery,

			right(currentRelease, 2) as currentReleaseMonth,
			left(currentRelease, 4) as currentReleaseYear,

			right(case when release_clarity is not null then release_clarity else release_clarity_evento end, 2) as clarityReleaseMonth,
			left(case when release_clarity is not null then release_clarity else release_clarity_evento end, 4) as clarityReleaseYear,

			priorityGlobal,
			state,
			category,
			UN,
			workFrontState,
			topic,
			RT,
			deliveryState,
			statusCategoryORL,

			finishedSystemsInDSOL as readyStrategyTestingAndContracting,
			qualityProposalWithGo as readyTimeline,

			case 
				when isnull(dtDeliveryTestPlan,'') <> '' and 
					 '20' + right(dtDeliveryTestPlan,2) + '-' + substring(dtDeliveryTestPlan,4,2) + '-' + left(dtDeliveryTestPlan,2) <= convert(char(10), getdate(),126)
				then 'S'
				else 'N' 
			end as readyTestPlan
		from
			(
				select
					sub.id as subproject,
					ent.id as delivery,

					(select 
						max(
							convert(varchar, re.release_ano) + right('00' + convert(varchar, re.release_mes), 2)
						)
					from 
						SGQ_Releases_Entregas re
					where
						re.subprojeto = sub.id and 
						re.entrega = ent.id
					) as currentRelease,

					(select top 1
						case when isnull(ex.release_nome,'') <> ''
							then right(ex.release_nome,4) + right('00' + convert(varchar, (select m.id from sgq_meses m where m.nome = (select left(ex.release_nome,len(ex.release_nome)-5)))),2)
							else null
						end
					from 
						BITI_Execucoes ex
						inner join BITI_Frentes_Trabalho ft
						  on ft.subprojeto = sub.id and
							 ft.estado not in ('CANCELADA', 'CANCELADA SEM DESENHO', 'PARTICIPAÇÃO RECUSADA')
					where 
						ex.subprojeto = sub.id and
						ex.entrega = ent.id and
						ex.estado <> 'CANCELADA'
					order by 1 desc
					) as release_clarity,

					(select 
						max(
							case when isnull(ex.dt_evento,'') <> ''
								then '20' + substring(ex.dt_evento, 7,2) + substring(ex.dt_evento, 4,2)
								else null
							end
						)
					from 
						BITI_Execucoes ex
						inner join BITI_Frentes_Trabalho ft
						  on ft.subprojeto = sub.id and
							 ft.estado not in ('CANCELADA', 'CANCELADA SEM DESENHO', 'PARTICIPAÇÃO RECUSADA')
					where 
						ex.subprojeto = sub.id and
						ex.entrega = ent.id and
						ex.estado <> 'CANCELADA'
					) as release_clarity_evento,

					case 
						when sub.Categoria <> 'BAU - BUSINESS AS USUAL' then sub.Prioridade_Global
						else 9999
					end as priorityGlobal,

					case 
						when sub.Estado = 'EM CONSOLIDAÇÃO E APROVAÇÃO DO DESENHO DA SOLUÇÃO' then 'CONSOL. E APROV. DSOL'
						when sub.Estado = 'EM CONSOLIDAÇÃO E APROVAÇÃO DO PLANEJAMENTO' then 'CONSOL. E APROV. PLAN'
						when sub.Estado = 'AGUARDANDO APROVAÇÃO FINANCEIRA' then 'AGUAR. APROV. FINANC.'
						when sub.Estado = 'SUBPROJETO EM CRIAÇÃO' then 'SUBPRJ. EM CRIAÇÃO'
						when sub.Estado = 'EM DESENHO DA SOLUÇÃO' then 'EM DSOL'
						when sub.Estado = 'AGUARDANDO VALIDAÇÃO DE REQUISITOS' then 'AGUAR. VAL. REQUISITOS'
						when sub.Estado = 'AGUARDANDO VALIDAÇÃO DA MACRO ESTIMATIVA' then 'AGUAR. VAL. MACRO ESTI.'
						when sub.Estado = 'EM PLANEJAMENTO PRELIMINAR' then 'EM PLAN. PRELIMINAR' 
						else sub.Estado 
					end as state,

					case 
						when sub.Categoria = 'MELHORIA OPERACIONAL' then 'MO - MELHORIA OPER.'
						when sub.Categoria = 'ORL – OBRIGAÇÃO REGULATÓRIA/LEGAL' then 'ORL - OBRIG. REG./LEGAL'
						when sub.Categoria = 'OT – OBRIGAÇÃO TRIBUTÁRIA' then 'OT – OBRIG. TRIBUTÁRIA'
						else sub.Categoria
					end as category,

					case 
						when sub.UN = 'MARKETING E VAREJO MOBILIDADE' then 'MKT E VAREJO MOBIL.'
						when sub.UN = 'MARKETING E VAREJO RESIDENCIAL' then 'MKT E VAREJO RESI.'
						when sub.UN = 'DESENVOLVIMENTO DE SOLUÇÕES' then 'DESENV. DE SOLUÇÕES'
						when sub.UN = 'PLANEJAMENTO E GOVERNANÇA' then 'PLANEJAMENTO E GOV.' 
						else sub.UN 
					end as UN,

					case 
						when ft.estado = 'EM AVALIAÇÃO DE REQUISITOS' then 'EM AVAL. DE REQUISITOS'
						when ft.estado = 'EM DEFINIÇÃO DE RESPONSÁVEL' then 'EM DEF. DE RESPONSÁVEL'
						when ft.estado = 'FRENTE DE TRABALHO EM CRIAÇÃO' then 'FT EM CRIAÇÃO' 
						else ft.estado 
					end as workFrontState,

					ft.area as topic,

					case 
						when ft.Responsavel_Tecnico = 'CARLOS HENRIQUE PIMENTEL DE MELO JUNI' then 'CARLOS HENRIQUE'
						when ft.Responsavel_Tecnico = 'SORAIA ANDREIA CASAGRANDE' then 'SORAIA CASAGRANDE' 
						else ft.Responsavel_Tecnico 
					end as RT,

					ent.estado as deliveryState,

					case 
						when sub.Categoria = 'ORL – OBRIGAÇÃO REGULATÓRIA/LEGAL' then 'S'
						else 'N'
					end as statusCategoryORL,

					case when
						sub.Estado not in (
							'Subprojeto em Criação', 
							'Em Avaliação de Arquitetura',
							'Aguardando Validação de Requisitos', 
							'Em Macro Estimativa', 
							'Aguardando Validação da Macro Estimativa', 
							'Em Planejamento Preliminar'
						) and
						not exists (
							select 1
							from BITI_Frentes_Trabalho ft
								left join BITI_Desenhos ds 
								  on ds.Frente_Trabalho = ft.id
							where 
								ft.Subprojeto = sub.id and
								ft.Sistema_nome <> 'Não Informado' and
								ds.Estado = 'Em desenho da solução'
						)
						then 'S'
						else 'N'
					end as finishedSystemsInDSOL,
			
					case when 
						exists (
							select 1
							from BITI_Propostas pt
							where pt.subprojeto = sub.id and
								  pt.Frente_Trabalho = ft.id and
								  pt.Estado = 'Proposta com GO'
						)
						then 'S'
						else 'N' 
					end as qualityProposalWithGo,

					(select sgq_p.dtDeliveryTestPlan 
					from SGQ_Projects sgq_p
					where sgq_p.subproject = sub.id and
						sgq_p.delivery = ent.id
					) as dtDeliveryTestPlan

				from
					BITI_Subprojetos sub
					left join BITI_Entregas ent
						on ent.subprojeto = sub.id
					left join BITI_Frentes_Trabalho ft
						on ft.subprojeto = sub.id
				where 
					sub.status = 'ATIVO' and
					sub.estado not in ('CANCELADO', 'ENCERRADO') and
					ent.estado not in('ENTREGA CANCELADA', 'ENCERRADA') and
					ft.area in ('TESTES E RELEASE', 'SUPORTE E PROJETOS', 'TRANSFORMACAO DE BSS') and
					ft.estado not in ('CANCELADA', 'CANCELADA SEM DESENHO', 'PARTICIPAÇÃO RECUSADA') and
					ft.sistema_nome = 'NÃO INFORMADO'
			) aux1
	) aux2
group by
	subproject,
	delivery,

	currentReleaseMonth,
	currentReleaseYear,

	clarityReleaseMonth,
	clarityReleaseYear,

	priorityGlobal,
	state,
	category,
	UN,
	deliveryState,
	statusCategoryORL
order by
	subproject,
	delivery

----------------------------------------------------------

insert into SGQ_Projects 
	(
		subproject,
		delivery,
		currentReleaseMonth,
		currentReleaseYear,
		clarityReleaseMonth,
		clarityReleaseYear,

		priorityGlobal,
		state,
		category,
		UN,
		workFrontState,
		topic,
		RT,
		deliveryState,
		statusCategoryORL,
 
 		readyStrategyTestingAndContracting,
		readyTimeline,
		readyTestPlan,

		trafficLight,
		statusStrategyTestingAndContracting,
		statusTimeline,
		statusTestPlan
	)
select 
	*,
	'VERDE' as trafficLight,
	'BACKLOG' as statusStrategyTestingAndContracting,
	'BACKLOG' as statusTimeline,
	'BACKLOG' as statusTestPlan
from 
	@t t
where 
	t.subproject + t.delivery not in (
		select SGQ_Projects.subproject + SGQ_Projects.delivery 
		from SGQ_Projects
	)

----------------------------------------------------------
-- Update - Start Date (Ready == S)
----------------------------------------------------------
update SGQ_Projects
set dtStartStrategyTestingAndContracting = convert(varchar, getdate(),5) + ' ' + convert(varchar, getdate(), 108) 
from @t t
where
	SGQ_Projects.subproject = t.subproject and
	SGQ_Projects.delivery = t.delivery and
	t.readyStrategyTestingAndContracting = 'S' and 
	isnull(SGQ_Projects.dtStartStrategyTestingAndContracting,'') = ''

update SGQ_Projects
set dtStartTimeLine = convert(varchar, getdate(),5) + ' ' + convert(varchar, getdate(), 108) 
from @t t
where
	SGQ_Projects.subproject = t.subproject and
	SGQ_Projects.delivery = t.delivery and
	t.readyTimeLine = 'S' and
	isnull(SGQ_Projects.dtStartTimeLine,'') = ''

update SGQ_Projects
set dtStartTestPlan = convert(varchar, getdate(),5) + ' ' + convert(varchar, getdate(), 108) 
from @t t
where
	SGQ_Projects.subproject = t.subproject and
	SGQ_Projects.delivery = t.delivery and
	t.readyTestPlan = 'S' and
	isnull(SGQ_Projects.dtStartTestPlan,'') = ''

----------------------------------------------------------
-- Update - Start Date (Ready == N)
----------------------------------------------------------
update SGQ_Projects
set dtStartStrategyTestingAndContracting = ''
from @t t
where
	SGQ_Projects.subproject = t.subproject and
	SGQ_Projects.delivery = t.delivery and
	t.readyStrategyTestingAndContracting = 'N' and 
	isnull(SGQ_Projects.dtStartStrategyTestingAndContracting,'') <> ''

update SGQ_Projects
set dtStartTimeLine = ''
from @t t
where
	SGQ_Projects.subproject = t.subproject and
	SGQ_Projects.delivery = t.delivery and
	t.readyTimeLine = 'N' and
	isnull(SGQ_Projects.dtStartTimeLine,'') <> ''

update SGQ_Projects
set dtStartTestPlan = ''
from @t t
where
	SGQ_Projects.subproject = t.subproject and
	SGQ_Projects.delivery = t.delivery and
	t.readyTestPlan = 'N' and
	isnull(SGQ_Projects.dtStartTestPlan,'') <> ''

----------------------------------------------------------
-- Update - End Date (Status = BACKLOG)
----------------------------------------------------------
update SGQ_Projects
set dtEndStrategyTestingAndContracting = ''
where
	SGQ_Projects.statusStrategyTestingAndContracting = 'BACKLOG' and 
	isnull(SGQ_Projects.dtEndStrategyTestingAndContracting,'') <> ''

update SGQ_Projects
set dtEndTimeLine = ''
where
	SGQ_Projects.statusTimeLine = 'BACKLOG' and 
	isnull(SGQ_Projects.dtEndTimeLine,'') <> ''

update SGQ_Projects
set dtEndTestPlan = ''
where
	SGQ_Projects.statusTestPlan = 'BACKLOG' and 
	isnull(SGQ_Projects.dtEndTestPlan,'') <> ''

----------------------------------------------------------
-- Update - End Date (Status <> BACKLOG)
----------------------------------------------------------
update SGQ_Projects
set dtEndStrategyTestingAndContracting = convert(varchar, getdate(),5) + ' ' + convert(varchar, getdate(), 108) 
where
	SGQ_Projects.statusStrategyTestingAndContracting <> 'BACKLOG' and 
	isnull(SGQ_Projects.dtEndStrategyTestingAndContracting,'') = ''

update SGQ_Projects
set dtEndTimeLine = convert(varchar, getdate(),5) + ' ' + convert(varchar, getdate(), 108) 
where
	SGQ_Projects.statusTimeLine <> 'BACKLOG' and 
	isnull(SGQ_Projects.dtEndTimeLine,'') = ''

update SGQ_Projects
set dtEndTestPlan = convert(varchar, getdate(),5) + ' ' + convert(varchar, getdate(), 108) 
where
	SGQ_Projects.statusTestPlan <> 'BACKLOG' and 
	isnull(SGQ_Projects.dtEndTestPlan,'') = ''

----------------------------------------------------------
-- Update - Fields
----------------------------------------------------------
update SGQ_Projects
set 
	SGQ_Projects.currentReleaseMonth = t.currentReleaseMonth,
	SGQ_Projects.currentReleaseYear = t.currentReleaseYear,
	SGQ_Projects.clarityReleaseMonth = t.clarityReleaseMonth,
	SGQ_Projects.clarityReleaseYear = t.clarityReleaseYear,

	 SGQ_Projects.priorityGlobal = t.priorityGlobal,
	 SGQ_Projects.state = t.state,
	 SGQ_Projects.category = t.category,
	 SGQ_Projects.UN = t.UN,
	 SGQ_Projects.workFrontState = t.workFrontState,
	 SGQ_Projects.topic = t.topic,
	 SGQ_Projects.RT = t.RT,
	 SGQ_Projects.deliveryState = t.deliveryState,
	 SGQ_Projects.statusCategoryORL = t.statusCategoryORL,

	SGQ_Projects.readyStrategyTestingAndContracting = t.readyStrategyTestingAndContracting,
	SGQ_Projects.readyTimeline = t.readyTimeline,
	SGQ_Projects.readyTestPlan = t.readyTestPlan
from @t t
where
	SGQ_Projects.subproject = t.subproject and
	SGQ_Projects.delivery = t.delivery --and 
	--(
	--	SGQ_Projects.currentReleaseMonth <> t.currentReleaseMonth or
	--	SGQ_Projects.currentReleaseYear <> t.currentReleaseYear or
	--	SGQ_Projects.clarityReleaseMonth <> t.clarityReleaseMonth or
	--	SGQ_Projects.clarityReleaseYear <> t.clarityReleaseYear or

	--	SGQ_Projects.priorityGlobal <> t.priorityGlobal or
	--	SGQ_Projects.state <> t.state or
	--	SGQ_Projects.category <> t.category or
	--	SGQ_Projects.UN <> t.UN or
	--	SGQ_Projects.workFrontState <> t.workFrontState or
	--	SGQ_Projects.topic <> t.topic or
	--	SGQ_Projects.RT <> t.RT or
	--	SGQ_Projects.deliveryState <> t.deliveryState or
	--	SGQ_Projects.statusCategoryORL <> t.statusCategoryORL or

	--	SGQ_Projects.readyStrategyTestingAndContracting <> t.readyStrategyTestingAndContracting or
	--	SGQ_Projects.readyTimeline <> t.readyTimeline or
	--	SGQ_Projects.readyTestPlan <> t.readyTestPlan
	--)
