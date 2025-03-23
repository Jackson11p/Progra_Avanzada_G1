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
        public bool EnviarCorreo(Usuario info, string mensaje, string titulo)
        {
                string cuenta = ConfigurationManager.AppSettings["CorreoNotificaciones"].ToString();
                string contrasenna = ConfigurationManager.AppSettings["ContrasennaNotificaciones"].ToString();
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(cuenta),
                    Subject = titulo,
                    Body = mensaje,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(info.Correo));
                message.Priority = MailPriority.Normal;

                SmtpClient client = new SmtpClient("smtp.office365.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential(cuenta, contrasenna),
                    EnableSsl = true
                };

                client.Send(message);
                return true;
         
        }

        public string MensajeRecuperacion(Usuario info, string codigoTemporal)
        {
            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; color: #000000; margin: 0; padding: 0; }}
                    .container {{ max-width: 600px; margin: 20px auto; padding: 20px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }}
                    .header {{ font-size: 24px; color: #000000; margin-bottom: 20px; font-weight: bold; }}
                    .code {{ font-size: 28px; font-weight: bold; color: #ffffff; margin: 20px 0; padding: 10px; background-color: #ED6436; border-radius: 4px; text-align: center; }}
                    .message {{ font-size: 16px; color: #000000; margin: 10px 0; }}
                    .footer {{ margin-top: 20px; font-size: 14px; color: #000000; text-align: center; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>Hola {info.Nombre},</div>
                    <p class='message'>Recibimos una solicitud para recuperar su acceso al sistema. Utilice el siguiente código:</p>
                    <div class='code'>{codigoTemporal}</div>
                    <p class='message'>Esta es una contraseña temporal, por favor cámbiela al ingresar al sistema.</p>
                    <p class='message'>Si no solicitó este código, por favor ignore este mensaje.</p>
                    <div class='footer'>
                        Este es un correo automático, por favor no responda a este mensaje.
                    </div>
                </div>
            </body>
            </html>";
        }

        public string MensajeCambioAcceso(Usuario info)
        {
            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; color: #000000; margin: 0; padding: 0; }}
                    .container {{ max-width: 600px; margin: 20px auto; padding: 20px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }}
                    .header {{ font-size: 24px; color: #000000; margin-bottom: 20px; font-weight: bold; }}
                    .message {{ font-size: 16px; color: #000000; margin: 10px 0; }}
                    .alert {{ font-size: 18px; font-weight: bold; color: #ffffff; margin: 20px 0; padding: 10px; background-color: #ED6436; border-radius: 4px; text-align: center; }}
                    .footer {{ margin-top: 20px; font-size: 14px; color: #000000; text-align: center; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>Hola {info.Nombre},</div>
                    <p class='message'>Queremos informarle que su contraseña ha sido cambiada correctamente.</p>
                    <div class='alert'>Si no realizó este cambio, comuníquese inmediatamente con nuestro equipo de soporte.</div>
                    <p class='message'>Si usted cambió su contraseña, no es necesario realizar ninguna acción adicional.</p>
                    <div class='footer'>
                        Este es un correo automático, por favor no responda a este mensaje.
                    </div>
                </div>
            </body>
            </html>";
        }

    }

}