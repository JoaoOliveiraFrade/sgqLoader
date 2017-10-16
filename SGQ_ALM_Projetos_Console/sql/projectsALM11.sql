select 
    ALM_Projetos.Id, 
    Nome, 
    Dominio, 
    Subprojeto, 
    Entrega,
    Esquema
from 
	alm_Projetos
	left join sgq_projects
		on sgq_projects.subproject = ALM_Projetos.subprojeto and
		    sgq_projects.delivery = ALM_Projetos.entrega
where 
    dominio = 'PRJ' and 
    ativo = 'Y' and 
	entrega not in ('ENTREGA_UNIF', 'ENTREGA_UNIF2') and
	(
		subprojeto like 'TRG%'
		or
		right('0000' + convert(varchar(4), isnull(currentReleaseYear,0)),4) + right('00' + convert(varchar(2), isnull(currentReleaseMonth,0)),2) >= right('0000' + convert(varchar(4), datepart(yyyy, dateadd(m, -1, getdate()))),4) + right('00' + convert(varchar(2), datepart(m, dateadd(m, -1, getdate()))),2)
		or
		right('0000' + convert(varchar(4), isnull(clarityReleaseYear,0)),4) + right('00' + convert(varchar(2), isnull(clarityReleaseMonth,0)),2) >= right('0000' + convert(varchar(4), datepart(yyyy, dateadd(m, -1, getdate()))),4) + right('00' + convert(varchar(2), datepart(m, dateadd(m, -1, getdate()))),2)
	)
order by
	(select count(*) from alm_cts 
	 where alm_cts.subprojeto = ALM_Projetos.subprojeto and 
	       alm_cts.entrega = ALM_Projetos.entrega)
