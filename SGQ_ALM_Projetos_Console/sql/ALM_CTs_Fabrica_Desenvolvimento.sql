update ALM_CTs 
set 
	Fabrica_Desenvolvimento = 
	(case when (select top 1 Fabrica_Desenvolvimento_Nome from SGQ_Sistemas_Arquitetura where Nome like ALM_CTs.Sistema + '%' ) <> '' 
		then (select top 1 Fabrica_Desenvolvimento_Nome from SGQ_Sistemas_Arquitetura where Nome like ALM_CTs.Sistema + '%' )
		else ALM_CTs.Sistema
	end)