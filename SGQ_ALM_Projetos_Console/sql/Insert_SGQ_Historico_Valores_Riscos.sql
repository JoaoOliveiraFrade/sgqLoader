insert into SGQ_Historico_Valores
select 
 getdate() as Data,
 Release_Mes,
 Release_Ano,
 'RISCOS' as Tipo,
 Risco as Item,
 count(*) as Valor
from
 ( 
	select 
		Release_Mes,
		Release_Ano,
		case when Risco is null 
		  then ''
		  else (select nome from SGQ_Riscos where id = Risco) 
		end as Risco
	from 	
		SGQ_Releases_Entregas re
	where
		(select Status from SGQ_Releases where id = re.Release) = 2 and 
		re.Exibir_Status_Diario = 'True'
 ) as Aux
group by
 Release_Mes,
 Release_Ano,
 Risco