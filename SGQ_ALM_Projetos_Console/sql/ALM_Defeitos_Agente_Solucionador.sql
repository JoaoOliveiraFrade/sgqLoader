update ALM_Defeitos
set Agente_Solucionador =
	(
		select top 1 Encaminhado_Para 
		from ALM_Defeitos_Tempos  dt
		where dt.status = 'CLOSED' and
			    dt.subprojeto = ALM_Defeitos.subprojeto and
			    dt.entrega = ALM_Defeitos.entrega and
			    dt.defeito = ALM_Defeitos.defeito 
		order by
			convert(datetime, dt.dt_de, 5) desc
	)
where 
	agente_solucionador is null or agente_solucionador = ''