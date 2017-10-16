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
    dominio = 'QUALIDADE_TI' and 
    ativo = 'N'
order by
	(select count(*) from alm_cts 
	 where alm_cts.subprojeto = ALM_Projetos.subprojeto and 
	       alm_cts.entrega = ALM_Projetos.entrega)
