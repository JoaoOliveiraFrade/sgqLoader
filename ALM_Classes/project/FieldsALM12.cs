using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.alm {
    public class FieldsALM12 : Fields  {
        public FieldsALM12() {
            this.list.Add(new Field() { target = "Origem", source = "'RESTORED'" });
            this.list.Add(new Field() { target = "Ativo", source = "'N'" });
            this.list.Add(new Field() { target = "Template_Nome", source = "'TD06'" });
            this.list.Add(new Field() { target = "Template", source = "6" });
            this.list.Add(new Field() { target = "Dt_Criacao", source = "dtCreate" });
        }
    }
}

