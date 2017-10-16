update alm_cts 
set alm_cts.Etapa = Aux.Etapa
from
(
	select
        cts1.Subprojeto,
        cts1.Entrega,
        cts1.CT,
        case 
            when cts1.Ciclo like '%TI%' and UAT = 'SIM' then 'TI (UAT)'
	        when cts1.Ciclo like '%TI%' then 'TI'
	        when cts1.Ciclo like '%UAT%' then 'UAT-PRESENCIAL'
	        when cts1.Ciclo like '%TRG%' then 'TRG'
	        when cts1.Ciclo like '%TS%' then 'TS'
	        when cts1.Ciclo like '%TP%' then 'TP'
            when cts1.Plano_Teste like '%TI%' and cts1.UAT = 'SIM' then 'TI (UAT)'
			when cts1.Plano_Teste like '%TI%' then 'TI'
			when cts1.Plano_Teste like '%UAT%' then 'UAT-PRESENCIAL'
			when cts1.Plano_Teste like '%TRG%' then 'TRG'
			when cts1.Plano_Teste like '%TS%' then 'TS'
			when cts1.Plano_Teste like '%TP%' then 'TP'
			else ''
        end as Etapa
	from alm_cts as cts1
) Aux	
where
	alm_cts.Subprojeto = Aux.Subprojeto and
	alm_cts.Entrega = Aux.Entrega and
	alm_cts.CT = Aux.CT and 
	(alm_cts.Etapa is null or alm_cts.Etapa <> Aux.Etapa)