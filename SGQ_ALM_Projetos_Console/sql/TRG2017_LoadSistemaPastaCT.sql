update alm_defeitos
	set alm_defeitos.Sistema_Pasta_CT = Aux.Sistema_Pasta_CT
from
	(
	    select 
		    df.subprojeto,
		    df.defeito,
		    substring(path, CHARINDEX('2017', cts.path) + 11,30) as Sistema_Pasta_CT
	    from 
		    alm_defeitos df WITH (NOLOCK)
		    left join alm_cts cts WITH (NOLOCK)
			    on cts.subprojeto = df.subprojeto and
			        cts.entrega = df.entrega and
			        cts.ct = df.ct
	    where	
		    df.subprojeto = 'TRG2017'
                        
        UNION ALL

	    select 
		    df.subprojeto,
		    df.defeito,
		    substring(path, CHARINDEX('2017', cts.path) + 11,30) as Sistema_Pasta_CT
	    from 
		    alm_defeitos df WITH (NOLOCK)
		    left join alm_cts cts WITH (NOLOCK)
			    on cts.subprojeto = df.subprojeto and
			        cts.entrega = df.entrega and
			        cts.ct = df.ct
	    where	
		    df.subprojeto = 'TRG2017'
	) as Aux
where
	alm_defeitos.subprojeto = Aux.subprojeto and
	alm_defeitos.defeito = Aux.defeito and

	(alm_defeitos.Sistema_Pasta_CT is null or
	alm_defeitos.Sistema_Pasta_CT <> Aux.Sistema_Pasta_CT)