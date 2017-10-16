update ALM_CTs 
set Fabrica_Teste = 
	case when Testador <> ''
			then case when (select fornecedor from ALM_Usuarios where Login = ALM_CTs.Testador) <> '' and
						    (select fornecedor from ALM_Usuarios where Login = ALM_CTs.Testador) <> 'OI' and
						    (select fornecedor from ALM_Usuarios where Login = ALM_CTs.Testador) <> '.'
					then replace(replace((select fornecedor from ALM_Usuarios where Login = ALM_CTs.Testador),char(10),''),char(13),'')
					else ALM_CTs.fornecedor
				end
			else ALM_CTs.fornecedor
	end;

update ALM_CTs set Fabrica_Teste = 'LINK CONSULTING' where Fabrica_Teste like '%LINK%';
update ALM_CTs set Fabrica_Teste = 'SONDA' where Fabrica_Teste like '%SONDA%';
update ALM_CTs set Fabrica_Teste = 'TRIAD SYSTEM' where Fabrica_Teste like '%TRIAD%';
