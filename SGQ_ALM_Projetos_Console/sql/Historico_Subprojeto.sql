delete SGQ_Historico_Subprojeto where data = convert(varchar, getdate(),5);

insert into SGQ_Historico_Subprojeto
select
	Data,
	Release,
	Subprojeto,
	Entrega,
	Nome,
	Macro_Diretoria,
	GP,
	N4,
	N3,
	Classificacao,
	Reportado_Na_Release, 
	Pontos_Atencao,
    Iterations,
	Risco, 
	Risco_Ordem,
	Motivo_Perda_Release,

	Dt_Alteracao as Dt_Alteracao_Pontos_Atencao,
	Dt_Perda_Release,

	sum(Total) as CTs,  
	sum(Cancelado) as CTs_Cancelados,  
	sum(Ativo) as CTs_tivos,  
	sum(Ativo_UAT) as CTs_Ativos_UAT,  
	sum(Planejado) as CTs_Planejados,

	sum(Prev_Dia) as CTs_Prev_Dia,  
	sum(Real_Dia) as CTs_Real_Dia,  
	sum(Real_Dia) - sum(Prev_Dia) as CTs_Saldo_Dia,

	sum(Prev_Acumulado) as CTs_Prev_Acumulado,  
	sum(Real_Acumulado) as CTs_Real_Acumulado,  
	sum(Real_Acumulado) - sum(Prev_Acumulado) as CTs_Saldo_Acumulado,

	sum(Plano_Lib_Valid_Tecnica) as CTs_Plano_Lib_Valid_Tecnica,
	sum(Plano_Lib_Valid_Cliente) as CTs_Plano_Lib_Valid_Cliente,
	sum(Evidencia_Lib_Valid_Tecnica) as CTs_Evidencia_Lib_Valid_Tecnica,
	sum(Evidencia_Lib_Valid_Cliente) as CTs_Evidencia_Lib_Valid_Cliente,

	sum(Plano_TI_Aprovado) as CTs_Plano_TI_Aprovado,
	sum(Plano_UAT_Aprovado) as CTs_Plano_UAT_Aprovado,
	sum(Evidencia_TI_Aprovado) as CTs_Evidencia_TI_Aprovado,
	sum(Evidencia_UAT_Aprovado) as CTs_Evidencia_UAT_Aprovado,

	(select count(*) from ALM_Defeitos d WITH (NOLOCK)
	    where 
	        d.subprojeto = Aux.Subprojeto and 
			d.entrega = Aux.Entrega
	) as Defeitos,

	(select count(*) from ALM_Defeitos d WITH (NOLOCK)
	    where 
	        d.subprojeto = Aux.Subprojeto and 
			d.entrega = Aux.Entrega and 
			d.Status_Atual = 'CANCELLED'
	) as Defeitos_Cancelados,

	(select count(*) from ALM_Defeitos d WITH (NOLOCK)
	    where 
		    d.subprojeto = Aux.Subprojeto and 
			d.entrega = Aux.Entrega and
			d.Status_Atual <> 'CANCELLED'
	) as Defeitos_Ativos,
	
	(select count(*) from ALM_Defeitos d WITH (NOLOCK)
	    where 
	        d.subprojeto = Aux.Subprojeto and 
			d.entrega = Aux.Entrega and 
			d.Status_Atual = 'CLOSED'
	) as Defeitos_Fechados,

	(select count(*) from ALM_Defeitos d WITH (NOLOCK)
	    where 
	        d.subprojeto = Aux.Subprojeto and 
			d.entrega = Aux.Entrega and 
			d.Status_Atual = 'CLOSED' and
			d.Origem like '%CONSTRUÇÃO%' and
			(d.Ciclo like '%TI%' or d.Ciclo like '%UAT%')
	) as Defeitos_Fechados_Construcao,

	(select count(*) from ALM_Defeitos d WITH (NOLOCK)
	    where 
	        d.subprojeto = Aux.Subprojeto and 
			d.entrega = Aux.Entrega and 
			d.Status_Atual not in('CANCELLED', 'CLOSED')
	) as Defeitos_Abertos,

	(select count(*) from ALM_Defeitos d WITH (NOLOCK)
	    where 
	        d.subprojeto = Aux.Subprojeto and 
			d.entrega = Aux.Entrega and 
			d.Status_Atual in ('ON_RETEST','PENDENT (RETEST)','REJECTED')
	) as Defeitos_Na_FT,

	(select count(*) from ALM_Defeitos d WITH (NOLOCK)
	    where 
	        d.subprojeto = Aux.Subprojeto and 
			d.entrega = Aux.Entrega and 
			d.Status_Atual in ('NEW','IN_PROGRESS','PENDENT (PROGRESS)','REOPEN','MIGRATE')
	) as Defeitos_Na_FD,

	round((convert(float, sum(Prev_Dia)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Prev_Dia,
	round((convert(float, sum(Real_Dia)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Real_Dia,

	round((convert(float, sum(Prev_Acumulado)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Prev_Acumulado,
	round((convert(float, sum(Real_Acumulado)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Real_Acumulado,

	round((CONVERT(float, sum(Plano_TI_Aprovado)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Plano_TI_Aprovado,
	round((CONVERT(float, sum(Plano_UAT_Aprovado)) / (case when sum(Ativo_UAT) <> 0 then sum(Ativo_UAT) else 1 end) * 100), 2) as Perc_Plano_UAT_Aprovado,
	round((CONVERT(float, sum(Evidencia_TI_Aprovado)) / (case when sum(Ativo) <> 0 then sum(Ativo) else 1 end) * 100), 2) as Perc_Evidencia_TI_Aprovado,
	round((CONVERT(float, sum(Evidencia_UAT_Aprovado)) / (case when sum(Ativo_UAT) <> 0 then sum(Ativo_UAT) else 1 end) * 100), 2) as Perc_Evidencia_UAT_Aprovado

from   
    (
    select 
	    convert(varchar, getdate(), 5) as Data,
        (select r.nome from sgq_releases r WITH (NOLOCK) where r.id = eXr.Release) as Release,
        eXr.Subprojeto,
        eXr.Entrega,
        sp.Nome as Nome,
        sp.Macro_Diretoria as Macro_Diretoria,
        sp.Gerente_Projeto as GP,
        sp.Gestor_Direto_LT as N4,
        sp.Gestor_Do_Gestor_LT as N3,
		sp.classificacao_nome as Classificacao,

        case when eXr.Exibir_Status_Diario = 1 then 'SIM' else 'NÃO' end as Reportado_Na_Release, 

        eXr.descricao_Risco as Pontos_Atencao,
        alm_cts.Iterations as Iterations,

        case 
            When eXr.Risco in (1,2,3,4,5) then (select r.nome from SGQ_Riscos r WITH (NOLOCK) where r.id = eXr.Risco)
            When eXr.Risco is null  then 'N/A'
        end as Risco, 

        case 
            When eXr.Risco in (1,2,3,4,5) then (select r.Ordem from SGQ_Riscos r WITH (NOLOCK) where r.id = eXr.Risco)
            When eXr.Risco is null  then 5
        end as Risco_Ordem, 
		 
        (select mpr.nome from SGQ_Motivos_Perda_Releases mpr WITH (NOLOCK) where mpr.id = eXr.Motivo_Perda_Release) as Motivo_Perda_Release,
		
		eXr.Dt_Alteracao,
		eXr.Dt_Perda_Release,

        case when (alm_cts.ct is not null) then 1 else 0 end as Total,  

        case when (Status_Exec_CT = 'CANCELLED')  then 1 else 0 end as Cancelado,  
        case when (Status_Exec_CT <> 'CANCELLED')  then 1 else 0 end as Ativo,

        case when (Status_Exec_CT <> 'CANCELLED' and UAT = 'SIM')  then 1 else 0 end as Ativo_UAT,

        case when (Status_Exec_CT <> 'CANCELLED' and Dt_Planejamento <> '') then 1 else 0 end as Planejado,		          

        case when (Status_Exec_CT <> 'CANCELLED' and Dt_Planejamento <> '') and 
		            (substring(Dt_Planejamento,7,2) + substring(Dt_Planejamento,4,2) + substring(Dt_Planejamento,1,2)) = convert(varchar(6), getdate(), 12) then 1 else 0 end as Prev_Dia,

        case when (Status_Exec_CT = 'PASSED' and Dt_Execucao <> '') and 
		            (substring(Dt_Execucao,7,2) + substring(Dt_Execucao,4,2) + substring(Dt_Execucao,1,2)) = convert(varchar(6), getdate(), 12) then 1 else 0 end as Real_Dia,

        case when (Status_Exec_CT <> 'CANCELLED' and Dt_Planejamento <> '') and 
		            (substring(Dt_Planejamento,7,2) + substring(Dt_Planejamento,4,2) + substring(Dt_Planejamento,1,2)) <= convert(varchar(6), getdate(), 12) then 1 else 0 end as Prev_Acumulado,

        case when (Status_Exec_CT = 'PASSED' and Dt_Execucao <> '') and 
		            (substring(Dt_Execucao,7,2) + substring(Dt_Execucao,4,2) + substring(Dt_Execucao,1,2)) <= convert(varchar(6), getdate(), 12) then 1 else 0 end as Real_Acumulado,


        case when (Status_Exec_CT <> 'CANCELLED' and 
		            (Plano_Validacao_Tecnica = 'LIBERADO PARA VALIDAÇÃO' or Plano_Validacao_Tecnica = 'LIBERADO PARA REVALIDAÇÃO')) then 1 else 0 end as Plano_Lib_Valid_Tecnica,

        case when (Status_Exec_CT <> 'CANCELLED' and UAT = 'SIM') and 
		            (Plano_Validacao_Cliente = 'LIBERADO PARA VALIDAÇÃO' or Plano_Validacao_Cliente = 'LIBERADO PARA REVALIDAÇÃO') then 1 else 0 end as Plano_Lib_Valid_Cliente,

        case when (Status_Exec_CT = 'PASSED') and 
		            (Evidencia_Validacao_Tecnica = 'LIBERADO PARA VALIDAÇÃO' or Evidencia_Validacao_Tecnica = 'LIBERADO PARA REVALIDAÇÃO') then 1 else 0 end as Evidencia_Lib_Valid_Tecnica,

        case when (Status_Exec_CT = 'PASSED' and UAT = 'SIM') and 
		            (Evidencia_Validacao_Cliente = 'LIBERADO PARA VALIDAÇÃO' or Evidencia_Validacao_Cliente = 'LIBERADO PARA REVALIDAÇÃO') then 1 else 0 end as Evidencia_Lib_Valid_Cliente,


        case when (Status_Exec_CT <> 'CANCELLED') and 
				    (Plano_Validacao_Tecnica = 'VALIDADO' or Plano_Validacao_Tecnica = 'N/A') then 1 else 0 end as Plano_TI_Aprovado,

        case when (Status_Exec_CT <> 'CANCELLED' and UAT = 'SIM') and 
				    (Plano_Validacao_Cliente = 'VALIDADO' or Plano_Validacao_Cliente = 'N/A') then 1 else 0 end as Plano_UAT_Aprovado,

        case when (Status_Exec_CT = 'PASSED') and 
				    (Evidencia_Validacao_Tecnica = 'VALIDADO' or Evidencia_Validacao_Tecnica = 'N/A') then 1 else 0 end as Evidencia_TI_Aprovado,

        case when (Status_Exec_CT = 'PASSED' and UAT = 'SIM') and 
				    (Evidencia_Validacao_Cliente = 'VALIDADO' or Evidencia_Validacao_Cliente = 'N/A') then 1 else 0 end as Evidencia_UAT_Aprovado

    from 
        SGQ_Releases_Entregas eXr WITH (NOLOCK)
        left join alm_cts WITH (NOLOCK)
                on alm_cts.subprojeto = eXr.subprojeto and alm_cts.entrega = eXr.entrega
        left join BITI_Subprojetos sp WITH (NOLOCK)
                on sp.id = eXr.subprojeto
    ) as Aux
group by 
	Data,
	Release,
	Subprojeto,
	Entrega,
	Nome,
	Macro_Diretoria,
	GP,
	N4,
	N3,
	Classificacao,
	Reportado_Na_Release, 
	Pontos_Atencao,
    Iterations,
	Risco, 
	Risco_Ordem, 
	Motivo_Perda_Release,
	Dt_Alteracao,
	Dt_Perda_Release
order by
	2,3,4