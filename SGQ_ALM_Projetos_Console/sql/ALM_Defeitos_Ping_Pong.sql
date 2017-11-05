update
	alm_defeitos
set 
	alm_defeitos.Ping_Pong = Aux.Ping_Pong
from
	(
		select
			a.Subprojeto,
			a.Entrega,
			a.Defeito,
			sum(case when a.Encaminhado_Para <> b.Encaminhado_Para then 1 else 0 end) as Ping_Pong
		from
			(
				select 
					Subprojeto,
					Entrega,
					Defeito,
					Dt_De,
				
					Row_Number() OVER(
						PARTITION BY 
							Subprojeto,
							Entrega,
							Defeito
						ORDER BY
							Subprojeto,
							Entrega,
							Defeito,
							substring(Dt_De,7,2) + substring(Dt_De,4,2)+ substring(Dt_De,1,2) + substring(Dt_De,10,8)
					) as Id,

					Encaminhado_Para
				from ALM_Defeitos_Tempos dt WITH (NOLOCK)
			) a

			left join 

			(
				select 
					Subprojeto,
					Entrega,
					Defeito,
					Dt_De,
				
					Row_Number() OVER(
						PARTITION BY 
							Subprojeto,
							Entrega,
							Defeito
						ORDER BY
							Subprojeto,
							Entrega,
							Defeito,
							substring(Dt_De,7,2) + substring(Dt_De,4,2)+ substring(Dt_De,1,2) + substring(Dt_De,10,8)
					) -1 as Id,

					Encaminhado_Para
				from ALM_Defeitos_Tempos dt WITH (NOLOCK)
			) b
			on b.Subprojeto = a.Subprojeto
				and b.Entrega = a.Entrega
				and b.Defeito = a.Defeito
				and b.Id = a.Id
			group by 
				a.Subprojeto,
				a.Entrega,
				a.Defeito
	) Aux
where
	alm_defeitos.Subprojeto = Aux.Subprojeto and 
	alm_defeitos.Entrega = Aux.Entrega and 
	alm_defeitos.Defeito = Aux.Defeito and
    (alm_defeitos.Ping_Pong is null or alm_defeitos.Ping_Pong <> Aux.Ping_Pong)

--update
--	alm_defeitos
--set 
--	Ping_Pong = 

--		(select
--			sum(case when A.Encaminhado_Para = B.Encaminhado_Para then 0 else 1 end) 
--		from
--			(select 
--				Row_Number() OVER(ORDER BY substring(Dt_De,7,2) + substring(Dt_De,4,2)+ substring(Dt_De,1,2) + substring(Dt_De,10,8)) as Id,
--				Encaminhado_Para, 
--				Dt_De
--			from ALM_Defeitos_Tempos dt WITH (NOLOCK)
--			where dt.Subprojeto = alm_defeitos.Subprojeto and 
--				dt.Entrega = alm_defeitos.Entrega and 
--				dt.Defeito = alm_defeitos.Defeito 
--			) A

--			left join 

--			(select 
--				(Row_Number() OVER(ORDER BY substring(Dt_De,7,2) + substring(Dt_De,4,2)+ substring(Dt_De,1,2) + substring(Dt_De,10,8))) - 1 as Id,
--				Encaminhado_Para, 
--				Dt_De
--			from ALM_Defeitos_Tempos dt WITH (NOLOCK)
--			where dt.Subprojeto = alm_defeitos.Subprojeto and 
--				dt.Entrega = alm_defeitos.Entrega and 
--				dt.Defeito = alm_defeitos.Defeito 
--			) B
--			on B.Id = A.Id
--		)