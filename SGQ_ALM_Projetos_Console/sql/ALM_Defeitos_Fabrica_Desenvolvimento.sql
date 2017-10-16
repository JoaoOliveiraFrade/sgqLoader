update ALM_Defeitos
set 
	Fabrica_Desenvolvimento = 
		(case when (select top 1 Fabrica_Desenvolvimento_Nome from SGQ_Sistemas_Arquitetura sa WITH (NOLOCK) where sa.Nome like ALM_Defeitos.Sistema_Defeito + '%') <> '' 
				then (select top 1 Fabrica_Desenvolvimento_Nome from SGQ_Sistemas_Arquitetura sa WITH (NOLOCK) where sa.Nome like ALM_Defeitos.Sistema_Defeito + '%') 
				else ALM_Defeitos.Sistema_Defeito 
		end);

update ALM_Defeitos set Fabrica_Desenvolvimento = 'DIST.SIST.EXTERNO' where Fabrica_Desenvolvimento like 'DISTRIBUIDOR - SISTEMA EXTERNO%';
update ALM_Defeitos set Fabrica_Desenvolvimento = 'GER.INTERF.MAINFRAME' where Fabrica_Desenvolvimento like 'GERENCIADOR DE INTERFACE MAINFRAME%';
update ALM_Defeitos set Fabrica_Desenvolvimento = 'SIST.ENGENHARIA' where Fabrica_Desenvolvimento like 'SISTEMAS DA ENGENHARIA%';
