using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class MailBuilder
    {
        readonly System.Net.Mail.MailMessage _mailMessage = new System.Net.Mail.MailMessage();


        public class ImageAttachBuilder
        {
            private readonly HtmlMailBuilder _htmlMailBuilder;
            private  string _modelloHtml;
            private readonly string _nomeContentId;

            public ImageAttachBuilder(HtmlMailBuilder htmlMailBuilder, string modelloHtml, string nomeContentId)
            {
                _htmlMailBuilder = htmlMailBuilder;
                _modelloHtml = modelloHtml;
                _nomeContentId = nomeContentId;
            }
            public HtmlMailBuilder RimpiazzaTag(string nomeTag)
            {
                Regex reg = new Regex(@"\{\b("+ nomeTag + @")\}");
                _modelloHtml = reg.Replace(_modelloHtml, "cid:" + _nomeContentId);
                return _htmlMailBuilder.ImpostaModello(_modelloHtml);
            }

            public HtmlMailBuilder RimpiazzaTagWithUrl(string nomeTag, string url)
            {
                Regex reg = new Regex(@"\{\b(" + nomeTag + @")\}");
                _modelloHtml = reg.Replace(_modelloHtml, url);
                return _htmlMailBuilder.ImpostaModello(_modelloHtml);
            }

            public HtmlMailBuilder SenzaRimpiazzareModelloHtml()
            {
                return _htmlMailBuilder;
            }
        }

        public class HtmlMailBuilder
        {
            private readonly MailBuilder _mailBuilder;
            private string _modelloHtml;
            private readonly List<System.Net.Mail.LinkedResource> _linkedResources = new List<LinkedResource>();
           
            public HtmlMailBuilder(MailBuilder mailBuilder,  string modelloHtml)
            {
                _mailBuilder = mailBuilder;

                this._modelloHtml = modelloHtml;

            }

            public HtmlMailBuilder ImpostaModello(string modelloHtml)
            {
                this._modelloHtml = modelloHtml;
                return this;
            }


            public HtmlMailBuilder ReplaceLinkImage(string nomeTag, string url)
            {
                Regex reg = new Regex(@"\{\b(" + nomeTag + @")\}");
                _modelloHtml = reg.Replace(_modelloHtml, url);
                return this;

            }


            public ImageAttachBuilder DownloadAndAttachImage(string urlImage, string imageResourceContentId)
            {

                string localFilename = Path.GetTempFileName();


                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(urlImage, localFilename);

                }

                System.Net.Mail.LinkedResource imageResource = new System.Net.Mail.LinkedResource(localFilename, "image/jpg");
                imageResource.ContentId = imageResourceContentId;

                _linkedResources.Add(imageResource);

                ImageAttachBuilder imageAttachBuilder = new ImageAttachBuilder(this, _modelloHtml, imageResourceContentId);
                return imageAttachBuilder;
            }

            public MailBuilder End()
            {
                
                _mailBuilder._mailMessage.IsBodyHtml = true;
                _mailBuilder._mailMessage.Body = _modelloHtml;
                return _mailBuilder;
            }
        }

        

        public MailBuilder ImpostaMittente(string mittente)
        {
            _mailMessage.From = new System.Net.Mail.MailAddress(mittente);
            return this;
        }

        public MailBuilder ImpostaSoggetto(string soggetto)
        {
            _mailMessage.Subject = soggetto;
            return this;
        }

        public MailBuilder AggiungiDestinatario(string emailDestinatario)
        {
            _mailMessage.To.Add(emailDestinatario.Trim());
            return this;
        }

        public MailBuilder AggiungiCCn(string emailDestinatario)
        {
            if (!string.IsNullOrEmpty(emailDestinatario))
            {
                _mailMessage.Bcc.Add(emailDestinatario.Trim());
            }

            return this;
        }

        public MailBuilder InCopiaDiConoscenza(string emailDestinatario)
        {
            _mailMessage.CC.Add(emailDestinatario.Trim());
            return this;
        }

        public MailBuilder AggiungiAlternativeView(AlternateView view)
        {
            _mailMessage.AlternateViews.Add(view);
            _mailMessage.IsBodyHtml = true;
            
            return this;
        }

        public HtmlMailBuilder CreaEmailDaModelloHtml(string htmlModelloMail)
        {
            return new HtmlMailBuilder(this, htmlModelloMail);
        }

        public MailBuilder CreaEmailDaModelloDiTesto(string txtModelloMail)
        {
            System.Net.Mail.AlternateView plainTextView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(txtModelloMail, null, "text/plain");
            _mailMessage.AlternateViews.Add(plainTextView);
            return this;
        }


        public static MailBuilder CreaNuovaEmail()
        {
            return new MailBuilder();
        }

        public static implicit operator System.Net.Mail.MailMessage(MailBuilder mb)
        {
            return mb._mailMessage;
        }


    }


   
}
