update alm_defeitos 
set 
	alm_defeitos.fabrica_desenvolvimento = a2.devManuf
from 
	(
		select
			a1.subproject, a1.delivery, a1.defect	
			,case when a1.devManuf <> 'APP DIGITAL - SISTEMA EXTERNO'
				then a1.devManuf
				else 'APP DIGITAL-SE'
			end as devManuf
		from
			(
				select
					df.subprojeto as subproject
					,df.entrega as delivery
					,df.defeito	as defect
					,(case when IsNull(sa.fabrica_desenvolvimento_nome, '') <> ''
						then sa.fabrica_desenvolvimento_nome 
						else 'N/A' 
					end) as devManuf
				from
					ALM_Defeitos df
					left join SGQ_Sistemas_Arquitetura sa
						on sa.Nome = df.sistema_defeito
			) a1
	) a2
where
	alm_defeitos.subprojeto = a2.subproject
	and alm_defeitos.entrega = a2.delivery
	and alm_defeitos.defeito = a2.defect
	and 
	(
		alm_defeitos.fabrica_desenvolvimento is null
		or alm_defeitos.fabrica_desenvolvimento <> a2.devManuf
	)