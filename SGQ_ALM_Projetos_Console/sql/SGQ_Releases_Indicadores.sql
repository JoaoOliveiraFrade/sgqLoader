declare @Temp1 table (
	Release_Mes int, 
	Release_Ano int, 
	Iniciou_TI_Sem_DSol_Aprovado int, 
	Continua_Sem_DSol_Aprovado int, 
	Iniciou_TI_Sem_Plano_Aprovado int, 
	Continua_Sem_Plano_Aprovado int, 
	Iniciou_UAT_Sem_TI_Aprovado int, 
	Continua_Sem_TI_Aprovado int);

insert into @Temp1
select
	Release_Mes,
	Release_Ano,

	sum(case when (Existe_CT_TI_Executado_OK = 'SIM' and 
					(
					    (DSol_Sem_Aprovacao = 'SIM') or
					    (DSol_Sem_Aprovacao = 'NÃO' and Dt_Primeira_Execucao_TI_OK < Dt_Aprovavao_DSol)
					)
				    )
			then 1
			else 0
	end) as Iniciou_TI_Sem_DSol_Aprovado,

	sum(case when (Existe_CT_TI_Executado_OK = 'SIM' and DSol_Sem_Aprovacao = 'SIM')
			then 1
			else 0
	end) as Continua_Sem_DSol_Aprovado,

	sum(case when (Existe_CT_TI_Executado_OK = 'SIM' and 
					(
					    (Existe_CT_TI_Sem_Aprovacao_Plano = 'SIM') or
					    (Existe_CT_TI_Sem_Aprovacao_Plano = 'NÃO' and Dt_Primeira_Execucao_TI_OK < Dt_Ultima_Aprovacao_Plano_TI)
					)
				    )
			then 1
			else 0
	end) as Iniciou_TI_Sem_Plano_Aprovado,

	sum(case when (Existe_CT_TI_Executado_OK = 'SIM' and Existe_CT_TI_Sem_Aprovacao_Plano = 'SIM')
			then 1
			else 0
	end) as Continua_Sem_Plano_Aprovado,

	sum(case when (Existe_CT_UAT_Presencial_Execudado_OK = 'SIM' and 
					(
						(Existe_CT_TI_Sem_Aprovacao_Evidencia  = 'SIM') or 
						(Existe_CT_TI_Sem_Aprovacao_Evidencia = 'NÃO' and Dt_Primeira_Execucao_UAT_Presencial_OK < Dt_Ultima_Aprovacao_Evidencia_TI)
					)
				    ) or
				    (Existe_CT_UAT_Por_Evidencia_Aprovado_Cliente = 'SIM' and 
					(
						(Existe_CT_TI_Sem_Aprovacao_Evidencia  = 'SIM') or 
						(Existe_CT_TI_Sem_Aprovacao_Evidencia  = 'NÃO' and Dt_Primeira_Aprovacao_UAT_Evidencia_Cliente < Dt_Ultima_Aprovacao_Evidencia_TI)
					)
				    )
			then 1
			else 0
	end) as Iniciou_UAT_Sem_TI_Aprovado,

	sum(case when (Existe_CT_UAT_Presencial_Execudado_OK = 'SIM' or Existe_CT_UAT_Por_Evidencia_Aprovado_Cliente = 'SIM') and
				    (Existe_CT_TI_Sem_Aprovacao_Evidencia  = 'SIM')
			then 1
			else 0
	end) as Continua_Sem_TI_Aprovado

	from
		(select 
			re.Release_Mes,
			re.Release_Ano,
			re.Subprojeto,
			re.Entrega,

			(case when exists
				(select top 1 cts.ct
				from alm_cts cts WITH (NOLOCK)
				where
					cts.subprojeto = re.subprojeto and
					cts.entrega = re.entrega and
					cts.status_exec_ct = 'PASSED' and
					cts.Ciclo like '%TI%')
				then 'SIM'
				else 'NÃO'
			end) as Existe_CT_TI_Executado_OK,

			(case when exists
				(select top 1 cts.ct
				from alm_cts cts WITH (NOLOCK)
				where
					cts.subprojeto = re.subprojeto and
					cts.entrega = re.entrega and
					cts.status_exec_ct = 'PASSED' and
					cts.ciclo like '%UAT%')
				then 'SIM'
				else 'NÃO'
			end) as Existe_CT_UAT_Presencial_Execudado_OK,

			--(case when exists
			--	(select top 1 h.Tabela_Id
			--	from alm_cts cts WITH (NOLOCK)
			--		inner join alm_historico_alteracoes_campos h WITH (NOLOCK)
			--			on h.subprojeto = cts.subprojeto and
			--				h.entrega = cts.entrega and
			--				h.Tabela = 'TESTCYCL' and 
			--				h.Campo in('(EVIDÊNCIA) VALIDAÇÃO CLIENTE') and
			--				h.Tabela_Id = cts.ct and
			--				h.Novo_Valor in ('VALIDADO')
			--	where
			--		cts.subprojeto = re.subprojeto and
			--		cts.entrega = re.entrega and
			--		cts.status_exec_ct <> 'CANCELLED' and
			--		cts.UAT = 'SIM' and
			--		cts.Ciclo like '%TI%')
			--	then 'SIM'
			--	else 'NÃO'
			--end) as Existe_CT_UAT_Por_Evidencia_Aprovado_Cliente,

			(case when exists
				(select top 1 1
				from alm_cts cts WITH (NOLOCK)
				where
					cts.subprojeto = re.subprojeto and
					cts.entrega = re.entrega and
					cts.status_exec_ct <> 'CANCELLED' and
					cts.evidencia_validacao_cliente = 'VALIDADO' and
					cts.UAT = 'SIM' and
					cts.Ciclo like '%TI%')
				then 'SIM'
				else 'NÃO'
			end) as Existe_CT_UAT_Por_Evidencia_Aprovado_Cliente,


			(case when not exists(
						select 1
						from BITI_Documentos_Controlados WITH (NOLOCK)
						where Objeto = re.Subprojeto and
							    Tipo = 'Desenho da Solução' and
							    Estado = 'Aprovado'
						)
					then 'SIM'
					else 'NÃO'
			end) as DSol_Sem_Aprovacao,

			--(case when exists
			--	(select top 1 h.Tabela_Id
			--	from alm_cts cts WITH (NOLOCK)
			--		inner join alm_historico_alteracoes_campos h WITH (NOLOCK)
			--			on h.subprojeto = cts.subprojeto and
			--				h.entrega = cts.entrega and
			--				h.Tabela = 'TESTCYCL' and 
			--				h.Campo in('(PLANO) VALIDAÇÃO TÉCNICA',(case when cts.UAT = 'NÃO' then '(PLANO) VALIDAÇÃO CLIENTE' end)) and
			--				h.Tabela_Id = cts.ct and
			--				h.Novo_Valor not in ('VALIDADO','N/A','N/A - SOLICITAÇÃO PROJETO')
			--	where
			--		cts.subprojeto = re.subprojeto and
			--		cts.entrega = re.entrega and
			--		cts.status_exec_ct <> 'CANCELLED' and
			--		cts.Ciclo like '%TI%')
			--	then 'SIM'
			--	else 'NÃO'
			--end) as Existe_CT_TI_Sem_Aprovacao_Plano,


			(case when exists
				(select top 1 1
				from alm_cts cts WITH (NOLOCK)
				where
					cts.subprojeto = re.subprojeto and
					cts.entrega = re.entrega and
					cts.plano_validacao_tecnica not in ('VALIDADO','N/A','N/A - SOLICITAÇÃO PROJETO') and
					cts.status_exec_ct <> 'CANCELLED' and
					cts.Ciclo like '%TI%'
				
				union all

				select top 1 1
				from alm_cts cts WITH (NOLOCK)
				where
					cts.subprojeto = re.subprojeto and
					cts.entrega = re.entrega and
					cts.plano_validacao_cliente not in ('VALIDADO','N/A','N/A - SOLICITAÇÃO PROJETO') and
					cts.UAT = 'SIM' and
					cts.status_exec_ct <> 'CANCELLED' and
					cts.Ciclo like '%TI%')
				then 'SIM'
				else 'NÃO'
			end) as Existe_CT_TI_Sem_Aprovacao_Plano,

			--(case when exists
			--	(select top 1 h.Tabela_Id
			--	from alm_cts cts WITH (NOLOCK)
			--		inner join alm_historico_alteracoes_campos h WITH (NOLOCK)
			--			on h.subprojeto = cts.subprojeto and
			--				h.entrega = cts.entrega and
			--				h.Tabela_Id = cts.ct and
			--				h.Tabela = 'TESTCYCL' and 
			--				h.Campo in ('(EVIDÊNCIA) VALIDAÇÃO TÉCNICA', (case when cts.UAT = 'NÃO' then '(EVIDÊNCIA) VALIDAÇÃO CLIENTE' end)) and
			--				h.Novo_Valor not in ('VALIDADO','N/A','N/A - SOLICITAÇÃO PROJETO')
			--	where
			--		cts.subprojeto = re.subprojeto and
			--		cts.entrega = re.entrega and
			--		cts.status_exec_ct <> 'CANCELLED' and
			--		cts.Ciclo like '%TI%')
			--	then 'SIM'
			--	else 'NÃO'
			--end) as Existe_CT_TI_Sem_Aprovacao_Evidencia,


			(case when exists
				(select top 1 1
				from alm_cts cts WITH (NOLOCK)
				where
					cts.subprojeto = re.subprojeto and
					cts.entrega = re.entrega and
					evidencia_validacao_tecnica not in ('VALIDADO','N/A','N/A - SOLICITAÇÃO PROJETO') and
					cts.status_exec_ct <> 'CANCELLED' and
					cts.Ciclo like '%TI%'
				
				union all

				select top 1 1
				from alm_cts cts WITH (NOLOCK)
				where
					cts.subprojeto = re.subprojeto and
					cts.entrega = re.entrega and
					evidencia_validacao_cliente not in ('VALIDADO','N/A','N/A - SOLICITAÇÃO PROJETO') and
					cts.UAT = 'SIM' and
					cts.status_exec_ct <> 'CANCELLED' and
					cts.Ciclo like '%TI%')
				then 'SIM'
				else 'NÃO'
			end) as Existe_CT_TI_Sem_Aprovacao_Evidencia,

			(select min(convert(datetime, cts.Dt_Execucao,5))
			from alm_cts cts WITH (NOLOCK)
			where
				cts.subprojeto = re.subprojeto and
				cts.entrega = re.entrega and
				cts.status_exec_ct = 'PASSED' and
				cts.Ciclo like '%TI%'
			) as Dt_Primeira_Execucao_TI_OK,

			(select top 1 convert(datetime, Dt_Atualizacao, 5)
			from BITI_Documentos_Controlados 
			where Objeto = re.Subprojeto and
				    Tipo = 'Desenho da Solução' and
				    Estado = 'Aprovado'
			) as Dt_Aprovavao_DSol,
			
			(select max(convert(datetime, h.Dt_Alteracao,5))
			from alm_cts cts WITH (NOLOCK)
				inner join alm_historico_alteracoes_campos h WITH (NOLOCK)
					on h.subprojeto = cts.subprojeto and
						h.entrega = cts.entrega and
						h.Tabela = 'TESTCYCL' and 
						h.Campo in('(PLANO) VALIDAÇÃO TÉCNICA',(case when cts.UAT = 'SIM' then '(PLANO) VALIDAÇÃO CLIENTE' end)) and
						h.Tabela_Id = cts.ct and
						h.Novo_Valor in ('VALIDADO')
			where
				cts.subprojeto = re.subprojeto and
				cts.entrega = re.entrega and
				cts.status_exec_ct <> 'CANCELLED' and
				cts.Ciclo like '%TI%'
			) as Dt_Ultima_Aprovacao_Plano_TI,

			(select max(convert(datetime, h.Dt_Alteracao,5))
			from alm_cts cts WITH (NOLOCK)
				inner join alm_historico_alteracoes_campos h WITH (NOLOCK)
					on h.subprojeto = cts.subprojeto and
						h.entrega = cts.entrega and
						h.Tabela_Id = cts.ct and
						h.Tabela = 'TESTCYCL' and 
						h.Campo in ('(EVIDÊNCIA) VALIDAÇÃO TÉCNICA', (case when cts.UAT = 'SIM' then '(EVIDÊNCIA) VALIDAÇÃO CLIENTE' end)) and
						h.Novo_Valor in ('VALIDADO')
			where
				cts.subprojeto = re.subprojeto and
				cts.entrega = re.entrega and
				cts.status_exec_ct <> 'CANCELLED' and
				cts.Ciclo like '%TI%'
			) as Dt_Ultima_Aprovacao_Evidencia_TI,

			(select min(convert(datetime, cts.Dt_Execucao,5))
			from alm_cts cts WITH (NOLOCK)
			where
				cts.subprojeto = re.subprojeto and
				cts.entrega = re.entrega and
				cts.status_exec_ct = 'PASSED' and
				(cts.Ciclo like '%UAT%')
			) as Dt_Primeira_Execucao_UAT_Presencial_OK,

			(select max(convert(datetime, h.Dt_Alteracao,5))
			from alm_cts cts WITH (NOLOCK)
				inner join alm_historico_alteracoes_campos h WITH (NOLOCK)
					on h.subprojeto = cts.subprojeto and
						h.entrega = cts.entrega and
						h.Tabela_Id = cts.ct and
						h.Tabela = 'TESTCYCL' and 
						h.Campo in('(EVIDÊNCIA) VALIDAÇÃO CLIENTE') and
						h.Novo_Valor = 'VALIDADO'
			where
				cts.subprojeto = re.subprojeto and
				cts.entrega = re.entrega and
				cts.status_exec_ct <> 'CANCELLED' and
				cts.UAT = 'SIM' and
				cts.Ciclo like '%TI%'
			) as Dt_Primeira_Aprovacao_UAT_Evidencia_Cliente

		from
			SGQ_Releases_Entregas re WITH (NOLOCK)
		--where
			--re.subprojeto = 'PRJ00000174' and 
			--re.entrega = 'ENTREGA00000419'
	) Aux
group by 
	Release_Mes,
	Release_Ano

update SGQ_Releases
set 
	SGQ_Releases.Iniciou_TI_Sem_DSol_Aprovado = Aux.Iniciou_TI_Sem_DSol_Aprovado,
	SGQ_Releases.Continua_Sem_DSol_Aprovado = Aux.Continua_Sem_DSol_Aprovado,
	SGQ_Releases.Iniciou_TI_Sem_Plano_Aprovado = Aux.Iniciou_TI_Sem_Plano_Aprovado,
	SGQ_Releases.Continua_Sem_Plano_Aprovado = Aux.Continua_Sem_Plano_Aprovado,
	SGQ_Releases.Iniciou_UAT_Sem_TI_Aprovado = Aux.Iniciou_UAT_Sem_TI_Aprovado,
	SGQ_Releases.Continua_Sem_TI_Aprovado = Aux.Continua_Sem_TI_Aprovado
from
	@Temp1 Aux
where
	SGQ_Releases.Mes = Aux.Release_Mes and
	SGQ_Releases.Ano = Aux.Release_Ano