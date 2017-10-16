delete SGQ_Historico_CTs_Por_Status where Data = convert(varchar, getdate(),5)

insert into SGQ_Historico_CTs_Por_Status
select 
	Data,
	Subprojeto,
	Entrega,
	Iterations,
	[CANCELLED],
	[NO RUN], 
	[BLOCKED], 
	[FAILED], 
	[NOT COMPLETED], 
	[PASSED]
from
	(
		select 
			convert(varchar, getdate(), 5) as Data,
			Subprojeto,
			Entrega,
            Iterations,
			Status_Exec_CT,
			count(*) Qte
		from 
			ALM_CTs
		group by
			Subprojeto,
			Entrega,
            Iterations,
			Status_Exec_CT
	    ) AS Aux
pivot
	(
		sum(Qte)
		for Status_Exec_CT IN (
					[CANCELLED],
					[NO RUN], 
					[BLOCKED], 
					[FAILED], 
					[NOT COMPLETED], 
					[PASSED])
		) as PivotTable