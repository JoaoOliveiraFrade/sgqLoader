update alm_defeitos 
set 
	alm_defeitos.Fabrica_Teste = a3.testManuf
from
	(
		select
			a2.subproject, a2.delivery, a2.defeito
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
					a1.subproject, a1.delivery, a1.defeito
					,(case when IsNull(a1.testManuf,'') not in ('', 'OI')
						then a1.testManuf
						else 'N/A' 
					end) as testManuf
				from
					(
						select 
							df.subprojeto as subproject
							,df.entrega as delivery
							,df.defeito
							,case when isNull(detectado_por,'') <> ''
								then 
									case when (select isNull(us.fornecedor,'') from ALM_Usuarios us where us.login = df.detectado_por) not in ('', 'OI', '.')
										then 
											replace(replace((select us.fornecedor from ALM_Usuarios us where us.login = df.detectado_por),char(10),''),char(13),'')
										else 
											df.fabrica_teste
									end
								else 
									df.fabrica_teste
							end as testManuf
						from 
							alm_defeitos df
					) a1
			) a2
	) a3
where
	alm_defeitos.subprojeto = a3.subproject
	and alm_defeitos.entrega = a3.delivery
	and alm_defeitos.defeito = a3.defeito
	and (
		alm_defeitos.fabrica_teste is null
		or alm_defeitos.fabrica_teste <> a3.testManuf
	)