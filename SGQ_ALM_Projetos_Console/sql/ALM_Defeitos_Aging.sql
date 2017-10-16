update
	alm_defeitos
set 
	alm_defeitos.Aging = Aux.Aging
from
	(
	select 
		Subprojeto,
		Entrega,
		Defeito,
        round(
            cast(
				Sum(Tempo_Util)
            as float) / 60
		,2) as Aging
	from 
		ALM_Defeitos_Tempos dt WITH (NOLOCK)
	group by
		Subprojeto,
		Entrega,
		Defeito
	) Aux
where
	alm_defeitos.Subprojeto = Aux.Subprojeto and 
	alm_defeitos.Entrega = Aux.Entrega and 
	alm_defeitos.Defeito = Aux.Defeito and
    (alm_defeitos.Aging is null or alm_defeitos.Aging <> Aux.Aging)