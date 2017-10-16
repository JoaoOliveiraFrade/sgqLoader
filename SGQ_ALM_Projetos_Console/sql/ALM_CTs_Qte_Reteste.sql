update alm_cts 
set alm_cts.Qte_Reteste = Aux.Reteste
from
	(select
		Subprojeto,
		Entrega,
		CT,
		count(*) as Reteste
	from
		(select
			dt.Subprojeto,
			dt.Entrega,
			df.CT,
			(select top 1 
				dt2.Status 
				from ALM_DEFEITOS_Tempos dt2 WITH (NOLOCK)
				where 
				dt2.Subprojeto = dt.Subprojeto and 
				dt2.Entrega = dt.Entrega and
				dt2.Defeito = dt.Defeito and
				convert(datetime, dt2.dt_de, 5) > convert(datetime, dt.dt_de, 5) and
				dt2.Status <> 'ON_RETEST' --and
				order by 
				convert(datetime, dt2.dt_de, 5)
			) as Status_Posterior
		from 
			ALM_Defeitos df WITH (NOLOCK)
			inner join ALM_Defeitos_Tempos dt WITH (NOLOCK)
				on dt.Subprojeto = df.Subprojeto and 
					dt.Entrega = df.Entrega and
					dt.Defeito = df.Defeito
		where
			df.CT <> '' and
			df.Status_Atual <> 'CANCELLED' and
			dt.Status = 'ON_RETEST'
		) Aux
	where
		Status_Posterior in ('CLOSED','REOPEN')
	group by
		Subprojeto,
		Entrega,
		CT
	) Aux	
where
	alm_cts.Subprojeto = Aux.Subprojeto and
	alm_cts.Entrega = Aux.Entrega and
	alm_cts.CT = Aux.CT;

update alm_cts set alm_cts.Qte_Reteste = 0 where Qte_Reteste is null;