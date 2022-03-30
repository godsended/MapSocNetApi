using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using BumberAPI.Models;
using MimeKit;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BumberAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserDatabase usersDatabase;

        public AuthController(UserDatabase booksService) =>
    usersDatabase = booksService;

        [HttpPost(URLs.registration)]
        public async Task<string?> Register([FromBody] object? req_obj)
        {
            AuthResponse response = new AuthResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            AuthRequest? req = ((JsonElement)req_obj).Deserialize<AuthRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.Mail == null || req.Pass == null || req.FirstName == null || req.SecondName == null || req.Gender == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            List<string> par = new List<string>();
            par.Add("Mail");
            List<string> val = new List<string>();
            val.Add(req.Mail);
            if (UserDatabase.Find(par, val).Result.Count == 0)
            {
                User user = new User();
                user.Mail = req.Mail;
                user.PassHash = req.Pass;
                user.FirstName = req.FirstName;
                user.LastName = req.SecondName;
                user.Gender = req.Gender;
                user.InviteCode = new Random(Environment.TickCount).Next(100000, 999999).ToString();
                user.ActionToken = Security.Encrypt(req.Mail + req.Pass);

                response.Id = await UserDatabase.CreateAsync(user);
                response.Mail = user.Mail;
                response.Token = user.ActionToken;
                response.FindToken = user.InviteCode;

                return JsonSerializer.Serialize(response);
            }
            else
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }
        }

        [HttpPost(URLs.login)]
        public async Task<string?> Login([FromBody] object? req_obj)
        {
            AuthResponse response = new AuthResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            AuthRequest? req = ((JsonElement)req_obj).Deserialize<AuthRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.Mail == null || req.Pass == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }
            List<string> par = new List<string>();
            par.Add("Mail");
            par.Add("PassHash");
            List<string> val = new List<string>();
            val.Add(req.Mail);
            val.Add(req.Pass);

            List<User> res = await UserDatabase.Find(par, val);
            if (res.Count == 1)
            {
                User user = res[0];

                response.Id = user.Id;
                response.FindToken = user.InviteCode;
                response.Token = user.ActionToken;

                return JsonSerializer.Serialize(response);
            }
            else
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }
        }
        [HttpPost(URLs.loginWithToken)]
        public string LoginWithToken([FromBody] object? req_obj)
        {
            AuthResponse response = new AuthResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            AuthRequest? req = ((JsonElement)req_obj).Deserialize<AuthRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.Id == null || req.Token == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            User? user = UserDatabase.GetAsync(req.Id).Result;

            if (user != null)
            {
                if (user.ActionToken == req.Token)
                    return JsonSerializer.Serialize(response);
                else
                {
                    response.Error = 2;
                    return JsonSerializer.Serialize(response);
                }
            }
            else
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }
        }
        //Тут не хватает генерации кода и отправки его на почту
        [HttpPost(URLs.genPrc)]
        public async Task<string> GenPassRestoreCode([FromBody] object? req_obj)
        {
            AuthResponse response = new AuthResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            AuthRequest? req = ((JsonElement)req_obj).Deserialize<AuthRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.Mail == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            List<string> props = new List<string>();
            props.Add("Mail");
            List<string> values = new List<string>();
            values.Add(req.Mail);
            List<User> users = await UserDatabase.Find(props, values);

            if (users.Count == 1)
            {
                try
                {
                    MimeMessage message = new MimeMessage();
                    message.From.Add(new MailboxAddress("noreply@bumber.ru", "noreply@bumber.ru"));
                    message.To.Add(MailboxAddress.Parse(req.Mail)); //адресат сообщения

                    string code = new Random(Environment.TickCount).Next(100000, 999999).ToString();

                    message.Subject = "Password changing"; //тема сообщени

                    users[0].ConfirmToken = code;
                    await UserDatabase.UpdateAsync(users[0].Id, users[0]);

                    message.Body = new BodyBuilder() { HtmlBody = @"<div style=""color: green;"">" + code + "</div>" }.ToMessageBody(); //тело сообщения (так же в формате HTML)

                    using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.Connect("smtp.beget.com", 465, true); //либо использум порт 465
                        if (!client.IsConnected)
                        {
                            response.Error = 4;
                            return JsonSerializer.Serialize(response);
                        }
                        client.Authenticate("noreply@bumber.ru", "lv*2ThoG"); //логин-пароль от аккаунта
                        if (!client.IsAuthenticated)
                        {
                            response.Error = 5;
                            return JsonSerializer.Serialize(response);
                        }
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
                catch (Exception e)
                {
                    response.Error = 2;
                }
                return JsonSerializer.Serialize(response);
            }
            else
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }
        }

        [HttpPost(URLs.aplPrc)]
        public async Task<string> ApplyPassRestoreCode([FromBody] object? req_obj)
        {
            AuthResponse response = new AuthResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            AuthRequest? req = ((JsonElement)req_obj).Deserialize<AuthRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.Mail == null || req.Code == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            List<string> props = new List<string>();
            props.Add("Mail");
            props.Add("ConfirmToken");
            List<string> values = new List<string>();
            values.Add(req.Mail);
            values.Add(req.Code);
            List<User> users = await UserDatabase.Find(props, values);

            if (users.Count == 1)
            {
                return JsonSerializer.Serialize(response);
            }
            else
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }
        }

        [HttpPost(URLs.changePass)]
        public async Task<string> ChangePassword([FromBody] object? req_obj)
        {
            AuthResponse response = new AuthResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            AuthRequest? req = ((JsonElement)req_obj).Deserialize<AuthRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.Id == null || req.Token == null || req.Pass == null || req.NewPass == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            User? user = await UserDatabase.GetAsync(req.Id);

            if (user != null)
            {
                if (user.ActionToken == req.Token && user.PassHash == req.Pass)
                {
                    user.PassHash = req.NewPass;
                    await UserDatabase.UpdateAsync(user.Id, user);
                }
                else
                {
                    response.Error = 2;
                }
                return JsonSerializer.Serialize(response);
            }
            else
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }
        }
        [HttpPost(URLs.changePassByCode)]
        public async Task<string> ChangePasswordByCode([FromBody] object? req_obj)
        {
            AuthResponse response = new AuthResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            AuthRequest? req = ((JsonElement)req_obj).Deserialize<AuthRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.Mail == null || req.NewPass == null || req.Code == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            List<string> props = new List<string>();
            props.Add("Mail");
            props.Add("ConfirmToken");
            List<string> values = new List<string>();
            values.Add(req.Mail);
            values.Add(req.Code);

            List<User> users = await UserDatabase.Find(props, values);

            if (users.Count == 1)
            {
                users[0].PassHash = req.NewPass;
                await UserDatabase.UpdateAsync(users[0].Id, users[0]);
                return JsonSerializer.Serialize(response);
            }
            else
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }
        }
        [HttpGet("TestGet")]
        public string Test()
        {
            return "Hello world\n";
        }
        public class AuthResponse
        {
            public string? Mail { get; set; }
            public string? Pass { get; set; }
            public string? Id { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Code { get; set; }
            public string? NewPass { get; set; }
            public string? Token { get; set; }
            public string? Gender { get; set; }
            public string? FindToken { get; set; }
            public int? Error { get; set; }
        }

        public class AuthRequest
        {
            public string? Id { get; set; }
            public string? Token { get; set; }
            public string? NewPass { get; set; }
            public string? Pass { get; set; }
            public string? Mail { get; set; }
            public string? Code { get; set; }
            public string? FirstName { get; set; }
            public string? SecondName { get; set; }
            public string? Gender { get; set; }
        }

    }
}
