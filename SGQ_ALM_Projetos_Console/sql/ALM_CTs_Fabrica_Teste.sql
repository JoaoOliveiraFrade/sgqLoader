update alm_cts 
set 
	alm_cts.Fabrica_Teste = a3.testManuf
from 
	(
		select 
			a2.subproject, a2.delivery, a2.ct
			,case when a2.testManuf not like '%LINK%'
				then 
					case when a2.testManuf not like '%SONDA%'
						then 
							case when a2.testManuf not like '%TRIAD%'
								then 
									case when a2.testManuf not like '%ACC%'
										then 
											a2.testManuf
										else 'ACCENTURE'
									end
								else 'TRIAD'
							end
						else 'SONDA'
					end
				else 'LINK'
			end as testManuf
		from
			(
				select 
					a1.subproject, a1.delivery, a1.ct
					,(case when IsNull(a1.testManuf,'') not in ('', 'OI')
						then a1.testManuf
						else 'N/A' 
					end) as testManuf
				from
					(
						select 
							ct.subprojeto as subproject
							,ct.entrega as delivery
							,ct.ct
							,case when isNull(Testador,'') <> ''
								then 
									case when (select isNull(us.fornecedor,'') from ALM_Usuarios us where us.login = ct.Testador) not in ('', 'OI', '.')
										then 
											replace(replace((select us.fornecedor from ALM_Usuarios us where us.login = ct.Testador),char(10),''),char(13),'')
										else 
											ct.fornecedor
									end
								else 
									ct.fornecedor
							end as testManuf
						from 
							alm_cts ct
					) a1
			) a2
	) a3
where
	alm_cts.subprojeto = a3.subproject
	and alm_cts.entrega = a3.delivery
	and alm_cts.ct = a3.ct
	and (
		alm_cts.fabrica_teste is null
		or alm_cts.fabrica_teste <> a3.testManuf
	)