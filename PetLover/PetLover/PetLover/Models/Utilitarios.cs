using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using PetLover.BaseDatos;

namespace PetLover.Models
{
    public class Utilitarios
    {
        public bool EnviarCorreo(Usuario info, int codigo, string titulo)
        {
            string cuenta = ConfigurationManager.AppSettings["CorreoNotificaciones"].ToString();
            string contrasenna = ConfigurationManager.AppSettings["ContrasennaNotificaciones"].ToString();

            MailMessage message = new MailMessage();
            message.From = new MailAddress(cuenta);
            message.To.Add(new MailAddress(info.Correo));
            message.Subject = titulo;

            // Cuerpo del correo en HTML
            string htmlBody = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            color: #333;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            padding: 20px;
                            background-color: #fff;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }}
                        .header {{
                            font-size: 24px;
                            color: #007BFF;
                            margin-bottom: 20px;
                        }}
                        .code {{
                            font-size: 28px;
                            font-weight: bold;
                            color: #007BFF;
                            margin: 20px 0;
                            padding: 10px;
                            background-color: #e9f5ff;
                            border-radius: 4px;
                            text-align: center;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 14px;
                            color: #777;
                            text-align: center;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>Hola {info.Nombre},</div>
                        <p>Por favor, utilice el siguiente código para ingresar al sistema:</p>
                        <div class='code'>{codigo}</div>
                        <p>Esta es una contraseña temporal, por favor al entrar al sistena cambiela.</p>
                        <p>Si no solicitó este código, por favor ignore este mensaje.</p>
                        <div class='footer'>
                            Este es un correo automático, por favor no responda a este mensaje.
                        </div>
                    </div>
                </body>
                </html>";

            message.Body = htmlBody;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.Normal;

            SmtpClient client = new SmtpClient("smtp.office365.com", 587);
            client.Credentials = new System.Net.NetworkCredential(cuenta, contrasenna);
            client.EnableSsl = true;       
            return false;

        }
    }
}