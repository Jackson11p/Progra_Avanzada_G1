using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using PetLover.BaseDatos;
using System.Globalization;


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
            <head>{EstilosCorreo}</head>
            <body>
                <div class='container'>
                    <div class='header'>Hola {info.Nombre},</div>
                    <p class='message'>Recibimos una solicitud para recuperar su acceso al sistema. Utilice el siguiente código:</p>
                    <div class='code'>{codigoTemporal}</div>
                    <p class='message'>Esta es una contraseña temporal, por favor cámbiela al ingresar al sistema.</p>
                    <p class='message'>Si no solicitó este código, por favor ignore este mensaje.</p>
                    <div class='footer'>Este es un correo automático, por favor no responda a este mensaje.</div>
                </div>
            </body>
            </html>";
        }


        public string MensajeCambioAcceso(Usuario info)
        {
            return $@"
            <html>
            <head>{EstilosCorreo}</head>
            <body>
                <div class='container'>
                    <div class='header'>Hola {info.Nombre},</div>
                    <p class='message'>Queremos informarle que su contraseña ha sido cambiada correctamente.</p>
                    <div class='alert'>Si no realizó este cambio, comuníquese inmediatamente con nuestro equipo de soporte.</div>
                    <p class='message'>Si usted cambió su contraseña, no es necesario realizar ninguna acción adicional.</p>
                    <div class='footer'>Este es un correo automático, por favor no responda a este mensaje.</div>
                </div>
            </body>
            </html>";
        }


        public string MensajeCitaCreada(Usuario info, DateTime fechaHora, string nombreMascota, string nombreVeterinario)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-CR");
            return $@"
            <html>
            <head>{EstilosCorreo}</head>
            <body>
                <div class='container'>
                    <div class='header'>Hola {info.Nombre},</div>
                    <p class='message'>Su cita ha sido registrada exitosamente. Aquí están los detalles:</p>
                    <p class='message'><strong>Mascota:</strong> {nombreMascota}<br/><strong>Veterinario:</strong> {nombreVeterinario}</p>
                    <div class='detail'>📅 {fechaHora:dddd, dd MMMM yyyy hh:mm tt}</div>
                    <div class='footer'>Este es un correo automático, por favor no responda a este mensaje.</div>
                </div>
            </body>
            </html>";
        }

        public string MensajeCitaActualizada(Usuario info, DateTime fechaHora, string nombreMascota, string nombreVeterinario)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-CR");
            return $@"
            <html>
            <head>{EstilosCorreo}</head>
            <body>
                <div class='container'>
                    <div class='header'>Hola {info.Nombre},</div>
                    <p class='message'>La información de su cita ha sido actualizada correctamente.</p>
                    <p class='message'><strong>Mascota:</strong> {nombreMascota}<br/><strong>Veterinario:</strong> {nombreVeterinario}</p>
                    <div class='detail'>📅 Nueva fecha y hora: {fechaHora:dddd, dd MMMM yyyy hh:mm tt}</div>
                    <div class='footer'>Este es un correo automático, por favor no responda a este mensaje.</div>
                </div>
            </body>
            </html>";
        }


        private const string EstilosCorreo = @"
            <style>
                body { font-family: Arial, sans-serif; background-color: #f4f4f4; color: #000000; margin: 0; padding: 0; }
                .container { max-width: 600px; margin: 20px auto; padding: 20px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }
                .header { font-size: 24px; color: #000000; margin-bottom: 20px; font-weight: bold; }
                .message { font-size: 16px; color: #000000; margin: 10px 0; }
                .detail, .code { font-size: 18px; font-weight: bold; color: #ffffff; background-color: #ED6436; padding: 10px; border-radius: 4px; text-align: center; }
                .footer { margin-top: 20px; font-size: 14px; color: #000000; text-align: center; }
                .alert { font-size: 16px; font-weight: bold; color: #ffffff; background-color: #D9534F; padding: 10px; border-radius: 4px; text-align: center; margin-top: 10px; }
            </style>
        ";

    }

}