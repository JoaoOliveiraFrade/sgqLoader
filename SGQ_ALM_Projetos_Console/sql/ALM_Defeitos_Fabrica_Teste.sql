update ALM_Defeitos 
set Fabrica_Teste = 
	case when 
			Detectado_Por <> ''
		then 
			case when 
					(select fornecedor from ALM_Usuarios where Login = ALM_Defeitos.Detectado_Por) <> '' and
					(select fornecedor from ALM_Usuarios where Login = ALM_Defeitos.Detectado_Por) <> 'OI' and
					(select fornecedor from ALM_Usuarios where Login = ALM_Defeitos.Detectado_Por) <> '.'
				then 
					replace(replace((select fornecedor from ALM_Usuarios where Login = ALM_Defeitos.Detectado_Por),char(10),''),char(13),'')
				else 
					(select Fabrica_Teste 
						from ALM_CTs 
						where 
							ALM_CTs.Subprojeto = ALM_Defeitos.Subprojeto and 
							ALM_CTs.Entrega = ALM_Defeitos.Entrega and 
							ALM_CTs.CT = ALM_Defeitos.CT)
			end
		else 
			(select Fabrica_Teste 
				from ALM_CTs 
				where 
					ALM_CTs.Subprojeto = ALM_Defeitos.Subprojeto and 
					ALM_CTs.Entrega = ALM_Defeitos.Entrega and 
					ALM_CTs.CT = ALM_Defeitos.CT)
	end