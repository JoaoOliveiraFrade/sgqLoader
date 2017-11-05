update alm_cts 
set 
	alm_cts.Fabrica_Desenvolvimento = a1.devManuf
from 
	(
		select
			ct.subprojeto as subproject
			,ct.entrega as delivery
			,ct.ct	
			,(case when IsNull(sa.Fabrica_Desenvolvimento_Nome, '') <> ''
				then sa.Fabrica_Desenvolvimento_Nome 
				else 'N/A' 
			end) as devManuf
		from
			ALM_CTs ct
			left join SGQ_Sistemas_Arquitetura sa
				on sa.Nome = ct.Sistema
	) a1
where
	alm_cts.subprojeto = a1.subproject
	and alm_cts.entrega = a1.delivery
	and alm_cts.ct = a1.ct
	and 
	(
		alm_cts.Fabrica_Desenvolvimento is null
		or alm_cts.Fabrica_Desenvolvimento <> a1.devManuf
	)