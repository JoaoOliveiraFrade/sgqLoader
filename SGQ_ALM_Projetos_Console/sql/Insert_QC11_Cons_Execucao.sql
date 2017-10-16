insert into QC11_Cons_Execucao (
	Dt_Carga,
	Release,
	Esteira_Teste,
	Projeto,
	Demanda,
	Status_Demanda,
	Etapa,
	Ciclo,
	Fase,
	Sistema,
	Fornecedor,
	Macrocenario,
	Status,
	Total_CTs,
	Cancelados,
	Ativos,
	Nao_Planejados,
	Planejados,
	Previstos,
	Realizados,
	Nao_Realizados,

	Pedente_Liberacao_Tecnica,
	Lib_Validacao_Tecnica,
	Lib_Revalidacao_Tecnica,
	Rejeitados_Tecnica,
	Validados_Tecnica,

	Pedente_Liberacao_Cliente,
	Lib_Validacao_Cliente,
	Lib_Revalidacao_Cliente,
	Rejeitados_Cliente,
	Validados_Cliente,

	Total_Defeitos,
	Defeitos_Cancelados,
	Defeitos_Fechados,
	Defeitos_Abertos,
	Defeitos_IN_PROGRESS,
	Defeitos_MIGRATE,
	Defeitos_NEW,
	Defeitos_ON_RETEST,
	Defeitos_PENDENT,
	Defeitos_REJECTED,
	Defeitos_REOPEN)
select
    CONVERT(CHAR(10), GETDATE(), 105) as Dt_Carga,
    isnull(Release,0) as Release,
    Esteira_Teste,
    Projeto,
    Demanda,
    isnull(Status_Demanda,0) as Status_Demanda,
    Etapa,
    Ciclo,
    Fase,
    Sistema,
    isnull(Fornecedor,'') as Fornecedor,
    isnull(Macrocenario,'') as Macrocenario,
    isnull(Status,'') as Status,
    sum(Cancelados) + sum(Ativos) as Total_CTs,
    sum(Cancelados) as Cancelados,
    sum(Ativos) as Ativos,
    sum(Nao_Planejados) as Nao_Planejados,
    sum(Planejados) as Planejados,
    sum(Previstos) as Previstos,
    sum(Realizados) as Realizados,
    sum(Nao_Realizados) as Nao_Realizados,
    sum(Realizados) - (sum(Lib_Validacao_Tecnica) + sum(Lib_Revalidacao_Tecnica) + sum(Validados_Tecnica) + sum(Rejeitados_Tecnica)) as Pedente_Liberacao_Tecnica,
    sum(Lib_Validacao_Tecnica) as Lib_Validacao_Tecnica,
    sum(Lib_Revalidacao_Tecnica) as Lib_Revalidacao_Tecnica,
    sum(Rejeitados_Tecnica) as Rejeitados_Tecnica,
    sum(Validados_Tecnica) as Validados_Tecnica,
    sum(Realizados) - (sum(Lib_Validacao_Cliente) + sum(Lib_Revalidacao_Cliente) + sum(Validados_Cliente) + sum(Rejeitados_Cliente)) as Pedente_Liberacao_Cliente,
    sum(Lib_Validacao_Cliente) as Lib_Validacao_Cliente,
    sum(Lib_Revalidacao_Cliente) as Lib_Revalidacao_Cliente,
    sum(Rejeitados_Cliente) as Rejeitados_Cliente,
    sum(Validados_Cliente) as Validados_Cliente,

    sum(Defeitos_Cancelados + Defeitos_Fechados + Defeitos_IN_PROGRESS + Defeitos_MIGRATE + Defeitos_NEW + Defeitos_ON_RETEST + Defeitos_PENDENT + Defeitos_REJECTED + Defeitos_REOPEN) as Total_Defeitos,
    sum(Defeitos_Cancelados) as Defeitos_Cancelados,
    sum(Defeitos_Fechados) as Defeitos_Fechados,
    sum(Defeitos_IN_PROGRESS + Defeitos_MIGRATE + Defeitos_NEW + Defeitos_ON_RETEST + Defeitos_PENDENT + Defeitos_REJECTED + Defeitos_REOPEN) as Defeitos_Abertos,
    sum(Defeitos_IN_PROGRESS) as Defeitos_IN_PROGRESS,
    sum(Defeitos_MIGRATE) as Defeitos_MIGRATE,
    sum(Defeitos_NEW) as Defeitos_NEW,
    sum(Defeitos_ON_RETEST) as Defeitos_ON_RETEST,
    sum(Defeitos_PENDENT) as Defeitos_PENDENT,
    sum(Defeitos_REJECTED) as Defeitos_REJECTED,
    sum(Defeitos_REOPEN) as Defeitos_REOPEN
from  
	(select
		r.Id as Release,
		Projeto,
		Demanda,
		(select top 1 qXd.Status from QualityGatesXDemandas qXd where qXd.Sti_Demanda = ct.Demanda and qXd.QualityGate = r.Id) as Status_Demanda,
		(select Esteira_Teste from STI_Demandas where Id = ct.Demanda) as Esteira_Teste,
		(case when (ct.Etapa like '%TI%') and (ct.UAT = 'SIM') then 'TI (UAT)'
				when (ct.Etapa like '%TI%') then 'TI'
				when (ct.Etapa like '%UAT%') then 'UAT-PRESENCIAL'
				when (ct.Etapa like '%TRG%') then 'TRG'
				when (ct.Etapa like '%TS%') then 'TS'
				when (ct.Etapa like '%TP%') then 'TP'
				else ''
		end) as Etapa,
		Etapa as Ciclo,
		Fase,
		Sistema,
		case when Fornecedor <> '' 
						then Fornecedor
						else (select (select Nome from Empresas e where e.id = sa.fd) 
											from SistemasArquitetura sa
											where sa.Nome = ct.Sistema) 
		end as Fornecedor,
		Macrocenario,
		ct.Status,
		case when (ct.Status = 'CANCELLED') then 1 else 0 end as Cancelados,
		case when (ct.Status <> 'CANCELLED') then 1 else 0 end as Ativos,
		case when (ct.Status <> 'CANCELLED') and (Dt_Planejamento = '') then 1 else 0 end as Nao_Planejados,
		case when (ct.Status <> 'CANCELLED') and (Dt_Planejamento <> '') then 1 else 0 end as Planejados,
		case when (ct.Status <> 'CANCELLED') and (Dt_Planejamento <> '') and (substring(Dt_Planejamento,7,2) + substring(Dt_Planejamento,4,2) + substring(Dt_Planejamento,1,2)) <= right(convert(varchar(30),getdate(),112),6) then 1 else 0 end as Previstos,
		case when (ct.Status = 'PASSED') then 1 else 0 end as Realizados,
		case when (ct.Status <> 'CANCELLED') and (ct.Status <> 'PASSED') then 1 else 0 end as Nao_Realizados,
		case when (ct.Status = 'PASSED') and (Status_Validacao_Tecnica = 'LIBERADO PARA VALIDAÇÃO') then 1 else 0 end as Lib_Validacao_Tecnica,
		case when (ct.Status = 'PASSED') and (Status_Validacao_Tecnica = 'LIBERADO PARA REVALIDAÇÃO') then 1 else 0 end as Lib_Revalidacao_Tecnica,
		case when (ct.Status = 'PASSED') and (Status_Validacao_Tecnica = 'REJEITADO') then 1 else 0 end as Rejeitados_Tecnica,
		case when (ct.Status = 'PASSED') and (Status_Validacao_Tecnica = 'VALIDADO') then 1 else 0 end as Validados_Tecnica,
		case when (ct.Status = 'PASSED') and (Status_Validacao_Cliente = 'LIBERADO PARA VALIDAÇÃO') then 1 else 0 end as Lib_Validacao_Cliente,
		case when (ct.Status = 'PASSED') and (Status_Validacao_Cliente  = 'LIBERADO PARA REVALIDAÇÃO') then 1 else 0 end as Lib_Revalidacao_Cliente,
		case when (ct.Status = 'PASSED') and (Status_Validacao_Cliente  = 'REJEITADO') then 1 else 0 end as Rejeitados_Cliente,
		case when (ct.Status = 'PASSED') and (Status_Validacao_Cliente  = 'VALIDADO') then 1 else 0 end as Validados_Cliente,
		(select count(*)  from QC11_Defeitos d where d.Projeto = ct.Projeto and d.ct = ct.ct and d.Status_Atual = 'CANCELLED') as Defeitos_Cancelados,
		(select count(*)  from QC11_Defeitos d where d.Projeto = ct.Projeto and d.ct = ct.ct and d.Status_Atual = 'CLOSED') as Defeitos_Fechados,
		(select count(*)  from QC11_Defeitos d where d.Projeto = ct.Projeto and d.ct = ct.ct and d.Status_Atual = 'IN_PROGRESS') as Defeitos_IN_PROGRESS,
		(select count(*)  from QC11_Defeitos d where d.Projeto = ct.Projeto and d.ct = ct.ct and d.Status_Atual = 'MIGRATE') as Defeitos_MIGRATE,
		(select count(*)  from QC11_Defeitos d where d.Projeto = ct.Projeto and d.ct = ct.ct and d.Status_Atual = 'NEW') as Defeitos_NEW,
		(select count(*)  from QC11_Defeitos d where d.Projeto = ct.Projeto and d.ct = ct.ct and d.Status_Atual = 'ON_RETEST') as Defeitos_ON_RETEST,
		(select count(*)  from QC11_Defeitos d where d.Projeto = ct.Projeto and d.ct = ct.ct and d.Status_Atual in('PENDENT','PENDENT (MIGRATE)','PENDENT (PROGRESS)','PENDENT (RETEST)')) as Defeitos_PENDENT,
		(select count(*)  from QC11_Defeitos d where d.Projeto = ct.Projeto and d.ct = ct.ct and d.Status_Atual = 'REJECTED') as Defeitos_REJECTED,
		(select count(*)  from QC11_Defeitos d where d.Projeto = ct.Projeto and d.ct = ct.ct and d.Status_Atual = 'REOPEN') as Defeitos_REOPEN
	from 
		QC11_CTs ct
		left join QualityGates r
			on  r.Mes = convert(int,substring(ct.Dt_Release,4,2)) and  r.Ano = convert(int,'20' + substring(ct.Dt_Release,7,2))       
                
	UNION ALL

	select
		r.Id as Release,
		d.Projeto,
		d.Demanda,
		(select top 1 qXd.Status from QualityGatesXDemandas qXd where qXd.Sti_Demanda = d.Demanda and qXd.QualityGate = r.Id) as Status_Demanda,
		(select Esteira_Teste from STI_Demandas where Id = d.Demanda) as Esteira_Teste,
		(case when (d.Etapa like '%TI%') then 'TI'
				when (d.Etapa like '%UAT%') then 'UAT-PRESENCIAL'
				when (d.Etapa like '%TRG%') then 'TRG'
				when (d.Etapa like '%TS%') then 'TS'
				when (d.Etapa like '%TP%') then 'TP'
				else ''
		end) as Etapa,
		Etapa as Ciclo,
		d.Fase,
		case when d.Sistema_CT <> '' then d.Sistema_CT else d.Sistema_Defeito end as Sistema,
		(select Nome from Empresas e where e.id = (select sa.fd from SistemasArquitetura sa where sa.Nome = (case when d.Sistema_CT <> '' then d.Sistema_CT else d.Sistema_Defeito end))) as Fornecedor,
		'' as Macrocenario,
		'' as Status,
		0 as Cancelados,
		0 as Ativos,
		0 as Nao_Planejados,
		0 as Planejados,
		0 as Previstos,
		0 as Realizados,
		0 as Nao_Realizados,
		0 as Lib_Validacao_Tecnica,
		0 as Lib_Revalidacao_Tecnica,
		0 as Rejeitados_Tecnica,    
		0 as Validados_Tecnica,
		0 as Lib_Validacao_Cliente,
		0 as Lib_Revalidacao_Cliente,
		0 as Rejeitados_Cliente,    
		0 as Validados_Cliente,

		(case when d.Status_Atual = 'CANCELLED' then 1 else 0 end) as Defeitos_Cancelados,
		(case when d.Status_Atual = 'CLOSED' then 1 else 0 end) as Defeitos_Fechados,
		(case when d.Status_Atual = 'IN_PROGRESS' then 1 else 0 end) as Defeitos_IN_PROGRESS,
		(case when d.Status_Atual = 'MIGRATE' then 1 else 0 end) as Defeitos_MIGRATE,
		(case when d.Status_Atual = 'NEW' then 1 else 0 end) as Defeitos_NEW,
		(case when d.Status_Atual = 'ON_RETEST' then 1 else 0 end) as Defeitos_ON_RETEST,
		(case when d.Status_Atual = 'PENDENT' then 1 else 0 end) as Defeitos_PENDENT,
		(case when d.Status_Atual = 'REJECTED' then 1 else 0 end) as Defeitos_REJECTED,
		(case when d.Status_Atual = 'REOPEN' then 1 else 0 end) as Defeitos_REOPEN
	from 
		QC11_Defeitos d
		left join QualityGates r
			on  r.Mes = convert(int,substring(d.Dt_Release,4,2)) and  r.Ano = convert(int,'20' + substring(d.Dt_Release,7,2))       
	where  
		d.CT = 0 or d.CT is null
	) as Aux1
group by
	Release,
	Esteira_Teste,
	Projeto,
	Demanda,
	Status_Demanda,
	Etapa,
	Ciclo,
	Fase,
	Sistema,
	Fornecedor,
	Macrocenario,
	Status
order by
	Release,
	Esteira_Teste,
	Projeto,
	Demanda,
	Status_Demanda,
	Etapa,
	Ciclo,
	Fase,
	Sistema,
	Fornecedor,
	Macrocenario,
	Status