update
	alm_defeitos
set 
	Ping_Pong = 

		(select
			sum(case when A.Encaminhado_Para = B.Encaminhado_Para then 0 else 1 end) 
		from
			(select 
				Row_Number() OVER(ORDER BY Dt_De) as Id,
				Encaminhado_Para, 
				Dt_De
			from ALM_Defeitos_Tempos dt WITH (NOLOCK)
			where dt.Subprojeto = alm_defeitos.Subprojeto and 
				dt.Entrega = alm_defeitos.Entrega and 
				dt.Defeito = alm_defeitos.Defeito 
			) A

			left join 

			(select 
				(Row_Number() OVER(ORDER BY Dt_De)) - 1 as Id,
				Encaminhado_Para, 
				Dt_De
			from ALM_Defeitos_Tempos dt WITH (NOLOCK)
			where dt.Subprojeto = alm_defeitos.Subprojeto and 
				dt.Entrega = alm_defeitos.Entrega and 
				dt.Defeito = alm_defeitos.Defeito 
			) B
			on B.Id = A.Id
		)