namespace sgq.alm
{
    public class Evento
    {
        public string Status { get; set; }
        public string Encaminhado_Para { get; set; }
        public string Operador { get; set; }
        public string Dt_De { get; set; }
        public string Dt_Ate { get; set; }
        public long Tempo_Decorrido_Min { get; set; }
        public long Tempo_Util_Min { get; set; }

        //public Defeito() { }
        //public Defeito(string DtDe, string DtAte, string Status, string Encaminhado_Para, string Operador)
        //{
        //    this.pDtDe = DtDe;
        //    this.pDtAte = DtAte;
        //    this.pStatus = Status;
        //    this.pEncaminhado_Para = Encaminhado_Para;
        //    this.pOperador = Operador;
        //}
    }
}
