insert into SGQ_Historico_Valores
select 
 getdate() as Data,
 Release_Mes,
 Release_Ano,
 'STATUS PROJETO NA RELEASE' as Tipo,
 Item,
 count(*) as Valor
from
 ( 
	select 
		Release_Mes,
		Release_Ano,
		(case when Motivo_Perda_Release is null then 'EM ANDAMENTO' else 'PERDEU RELEASE' end) as Item
	from 	
		SGQ_Releases_Entregas re
	where
		(select Status from SGQ_Releases where id = re.Release) = 2 and
		re.Exibir_Status_Diario = 'True'
 ) as Aux
group by
 Release_Mes,
 Release_Ano,
 Item