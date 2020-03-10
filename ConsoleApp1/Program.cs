using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Entity;
using ConsoleApp1.Infrastructure;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {



            string mittente = "alberto.bellemo@outlook.it";
            string destinatario = "alberto.bellemo@outlook.it";
            string ccn = "alberto.bellemo@outlook.it";

            string linkImageTestata = "https://upload.wikimedia.org/wikipedia/it/thumb/0/04/Logo_della_Ferrari_S.p.A..svg/500px-Logo_della_Ferrari_S.p.A..svg.png";

            string modelloHtml = "<h1>Esempio</h1><img src=\"{IMAGE_TESTATA}\"></a>";
            string modelloTxt = "Esempio senza immagine solo testi";

            string resourceTestataId = "imageTestataResource";
            
            MailMessage mailMessage = MailBuilder.CreaNuovaEmail()
                .ImpostaMittente(mittente)
                .AggiungiDestinatario(destinatario)
                .AggiungiCCn(ccn)
                .ImpostaSoggetto("Test invio email")
                .CreaEmailDaModelloHtml(modelloHtml)
                    .DownloadAndAttachImage(linkImageTestata, resourceTestataId)
                        .RimpiazzaTag("IMAGE_TESTATA")
                    
                    .End()
                .CreaEmailDaModelloDiTesto(modelloTxt)
                ;
            InviaMail(mailMessage);


            

            // Oppure 
            var segnalazione = new Segnalazione();
            segnalazione
                .InseritaDa(new Account())
                .Il(DateTime.Now.AddMonths(-1))
                .VisualizzatoDa(new Account())
                .Il(DateTime.Now.AddDays(-5))
                .PresoInCaricoDa(new Account())
                .Il(DateTime.Now.AddHours(-1))
                .LavorataDa(new Account())
                .Ora();





        }


        #region Codice di supporto
        static void InviaMail(MailMessage mailMessage)
        {

            System.Net.Mail.SmtpClient clientSmtp = GetSmtpClient();
            clientSmtp.Send(mailMessage);

        }

        static System.Net.Mail.SmtpClient GetSmtpClient()
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = ConfigurationManager.AppSettings["SMTP_Server"];// "mailsrv.ideeperviaggiare.it";
            client.Port = Int32.Parse(ConfigurationManager.AppSettings["SMTP_Port"]); //25
            NetworkCredential nc = new NetworkCredential(
                ConfigurationManager.AppSettings["SMTP_Username"],
                ConfigurationManager.AppSettings["SMTP_Password"]);
            client.Credentials = nc;
            return client;
        }
        #endregion
    }
}
