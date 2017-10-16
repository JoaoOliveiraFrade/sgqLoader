using System;
using System.Data;
using System.Configuration;
using System.Windows.Forms;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Configuration;
using System.Net;
using System.Text;
using sgq;

namespace sgq
{
    public static class Pastas
    {
        public static string CriarPasta(string CaminhoPasta)
        {
            string MensagemErro = "";

            DirectoryInfo dir = new DirectoryInfo(CaminhoPasta);
            try
            {
                dir.Create();
            }
            catch (Exception e)
            {
                MensagemErro = "Não foi possível criar a pasta: " + e.ToString();
            }
            finally { }

            return MensagemErro;
        }

        public static string LimparPasta(string CaminhoPasta)
        {
            string MensagemErro = "";

            DirectoryInfo dir = new DirectoryInfo(CaminhoPasta);
            try
            {
                dir.Delete(true);
            }
            catch (Exception e)
            {
                MensagemErro = "Não foi possível excluir a pasta: " + e.ToString();
            }
            finally { }

            return MensagemErro;
        }
    }
}
