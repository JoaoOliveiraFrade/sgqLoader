using sgq;
using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace sgq.alm {
    public class Fields {
        public List<Field> list { get; set; }

        public Fields() {
            this.list = new List<Field>();
            this.list.Add(new Field() { target = "Id", source = "PROJECT_ID", type = "N", key = true });
            this.list.Add(new Field() { target = "Dominio", source = "upper(rtrim(DOMAIN_NAME))", key = true });
            this.list.Add(new Field() { target = "DominiodataSource", source = "upper(rtrim(CREATE_from_DOMAIN))" });
            this.list.Add(new Field() { target = "Nome", source = "upper(rtrim(PROJECT_NAME))" });
            this.list.Add(new Field() { target = "Subprojeto", source = "upper(rtrim(substr(PROJECT_NAME,1,11)))" });
            this.list.Add(new Field() { target = "Entrega", source = "upper(rtrim('ENTREGA' || substr(PROJECT_NAME,16,8)))" });
            this.list.Add(new Field() { target = "Esquema", source = "DB_NAME" });
            this.list.Add(new Field() { target = "Diretorio", source = "PHYSICAL_DIRECTORY" });
        }
    }
}

