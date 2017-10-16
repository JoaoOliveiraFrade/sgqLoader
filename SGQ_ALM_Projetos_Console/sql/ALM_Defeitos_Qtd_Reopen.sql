update ALM_Defeitos
set Qtd_Reopen = 
    (select count(*)
        from  
        (
			select distinct Dt_Ate from ALM_Defeitos_Tempos t 
			where t.Subprojeto = ALM_Defeitos.Subprojeto and 
					t.Entrega = ALM_Defeitos.Entrega and 
					t.Defeito = ALM_Defeitos.Defeito and 
					t.Status = 'REOPEN'
		) x
    )