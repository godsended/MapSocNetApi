using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using BumberAPI.Models;

namespace BumberAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        #region Info
        [HttpPost(URLs.loadDescription)]
        public async Task<string> LoadDescription([FromBody] object? req_obj)
        {
            try
            {
                UserInfoResponse response = new UserInfoResponse();
                if (req_obj == null)
                {
                    response.Error = 3;
                    return JsonSerializer.Serialize(response);
                }
                UserInfoRequest? req = ((JsonElement)req_obj).Deserialize<UserInfoRequest>();
                if (req == null)
                {
                    response.Error = 3;
                    return JsonSerializer.Serialize(response);
                }
                if (req.MyId == null || req.Id == null)
                {
                    response.Error = 1;
                    return JsonSerializer.Serialize(response);
                }

                User? me = await UserDatabase.GetAsync(req.MyId);
                User? user = await UserDatabase.GetAsync(req.Id);

                if (me == null || user == null)
                {
                    response.Error = 2;
                    return JsonSerializer.Serialize(response);
                }

                List<string> props = new List<string>();
                props.Add("UserId");
                List<string> vals = new List<string>();
                vals.Add(req.Id);
                if (props == null)
                {
                    response.Error = 22;
                    return JsonSerializer.Serialize(response);
                }
                if (vals == null)
                {
                    response.Error = 23;
                    return JsonSerializer.Serialize(response);
                }
                List<Image> images = await ImagesDatabase.Find(props, vals);

                response.Status = user.Status;
                response.LongDescriptuon = user.Description;
                response.ShortDescription = user.ShortDescription;
                response.Name = user.FirstName + ' ' + user.LastName;

                props = new List<string>();
                props.Add("SenderId");
                props.Add("RecieverId");
                props.Add("Info");
                vals = new List<string>();
                vals.Add(req.MyId);
                vals.Add(req.Id);
                vals.Add("Friend");

                List<Request> res1 = await RequestsDatabase.Find(props, vals);
                vals[0] = req.Id;
                vals[1] = req.MyId;
                List<Request> res2 = await RequestsDatabase.Find(props, vals);

                string frtype = "";
                if (res1 != null && res2 != null)
                {
                    if (res1.Count == 1 && res2.Count == 1)
                        frtype = "Y";
                }
                if (res1 != null && frtype == "")
                {
                    if (res1.Count == 1)
                        frtype = "S";
                }
                if (res2 != null && frtype == "")
                {
                    if (res2.Count == 1)
                        frtype = "MS";
                }
                if(frtype == "")
                {
                    frtype = "N";
                }

                response.FriendType = frtype;

                if (images != null)
                {
                    ImageResponse[] imageResponses = new ImageResponse[images.Count];
                    for (int i = 0; i < images.Count; i++)
                    {
                        imageResponses[i] = new ImageResponse();
                        imageResponses[i].ImageId = images[i].Id;
                        imageResponses[i].IsAvatar = images[i].IsAvatar;
                    }
                    response.Images = imageResponses;
                }

                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            //FriendType
        }

        [HttpPost(URLs.updateShortDescription)]
        public async Task<string> UpdateShortDescription([FromBody] object? req_obj)
        {
            UserInfoResponse response = new UserInfoResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            UserInfoRequest? req = ((JsonElement)req_obj).Deserialize<UserInfoRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.MyId == null || req.Token == null || req.Text == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            User? me = await UserDatabase.GetAsync(req.MyId);

            if (me == null)
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }

            if (req.Token != me.ActionToken)
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }

            me.ShortDescription = req.Text;
            await UserDatabase.UpdateAsync(me.Id, me);
            return JsonSerializer.Serialize(response);
            //FriendType
        }

        [HttpPost(URLs.updateDescription)]
        public async Task<string> UpdateDescription([FromBody] object? req_obj)
        {
            UserInfoResponse response = new UserInfoResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            UserInfoRequest? req = ((JsonElement)req_obj).Deserialize<UserInfoRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.MyId == null || req.Token == null || req.Text == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            User? me = await UserDatabase.GetAsync(req.MyId);

            if (me == null)
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }

            if (req.Token != me.ActionToken)
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }

            me.Description = req.Text;
            await UserDatabase.UpdateAsync(me.Id, me);
            return JsonSerializer.Serialize(response);
            //FriendType
        }

        [HttpPost(URLs.updateStatus)]
        public async Task<string> UpdateStatus([FromBody] object? req_obj)
        {
            UserInfoResponse response = new UserInfoResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            UserInfoRequest? req = ((JsonElement)req_obj).Deserialize<UserInfoRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.MyId == null || req.Token == null || req.Status == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            User? me = await UserDatabase.GetAsync(req.MyId);

            if (me == null)
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }

            if (req.Token != me.ActionToken)
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }

            me.Status = (int)req.Status;
            await UserDatabase.UpdateAsync(me.Id, me);
            return JsonSerializer.Serialize(response);
            //FriendType
        }
        #endregion
        #region Galery
        [HttpPost(URLs.uploadImage)]
        public async Task<string> UploadImage([FromForm] string gr, [FromForm] IFormFile file)
        {
            UserInfoResponse response = new UserInfoResponse();

            Stream s = file.OpenReadStream();
            BinaryReader br = new BinaryReader(s);
            byte[]? data = br.ReadBytes((int)s.Length);

            GaleryRequest? req = JsonSerializer.Deserialize<GaleryRequest>(gr);

            if (gr == null || data == null || req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.Id == null || req.Token == null || req.IsAvatar == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            User? me = await UserDatabase.GetAsync(req.Id);

            if (me == null)
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }

            if (req.Token != me.ActionToken)
            {
                response.Error = 2;
                return JsonSerializer.Serialize(response);
            }
            try
            {
                Image? img = new Image();
                img.LikesCount = 0;
                img.UserId = req.Id;
                img.IsAvatar = req.IsAvatar;

                if ((bool)req.IsAvatar)
                {
                    List<string> pars = new List<string>();
                    pars.Add("UserId");
                    List<string> vals = new List<string>();
                    vals.Add(req.Id);

                    List<string> boolProps = new List<string>();
                    List<bool> boolValues = new List<bool>();

                    boolProps.Add("IsAvatar");
                    boolValues.Add(true);

                    List<Image> imgs = await ImagesDatabase.Find(pars, vals, boolProps, boolValues);
                    if (imgs != null)
                    {
                        for (int i = 0; i < imgs.Count; i++)
                        {
                            if (imgs[i].Id == null)
                                continue;
                            imgs[i].IsAvatar = false;
                            await ImagesDatabase.UpdateAsync(imgs[i].Id, imgs[i]);
                        }
                    }
                }

                string? id = await ImagesDatabase.CreateAsync(img);

                string path = Path.Combine("images", id + ".jpg");
                img.Path = path;
                img.Id = id;

                await ImagesDatabase.UpdateAsync(id, img);

                FileStream fs = System.IO.File.Create(path);
                fs.Write(data, 0, data.Length);
                //System.IO.File.WriteAllBytes(path, data);
                fs.Close();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            return JsonSerializer.Serialize(response);
            //FriendType
        }

        [HttpPost(URLs.getImage)]
        public FileResult? GetImage([FromBody] GaleryRequest? req)
        {
            try
            {
                if (req == null)
                {
                    return null;
                }
                if (req.Id == null || req.ImageId == null)
                {
                    return null;
                }

                User? me = UserDatabase.GetAsync(req.Id).Result;
                Image? img = ImagesDatabase.GetAsync(req.ImageId).Result;

                if (me == null || img == null)
                {
                    return null;
                }

                string? path = img.Path;
                byte[] ans = System.IO.File.ReadAllBytes(path);
                return File(ans, "image/jpeg");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        #region Json
        public class UserInfoResponse
        {
            public int? Error { get; set; }
            public string? LongDescriptuon { get; set; }
            public string? ShortDescription { get; set; }
            public int? Status { get; set; }
            public string? Name { get; set; }
            public string? FriendType { get; set; }
            public ImageResponse[]? Images { get; set; }
        }
        public class UserInfoRequest
        {
            public string? Id { get; set; }
            public string? MyId { get; set; }
            public string? Token { get; set; }
            public string? Text { get; set; }
            public int? Status { get; set; }
            public byte[]? Data { get; set; }
        }
        public class ImageResponse
        {
            public string? ImageId { get; set; }
            public bool? IsAvatar { get; set; }
        }

        public class GaleryResponse
        {
            public int? Error { get; set; }
            public string[]? Images { get; set; }
        }
        public class GaleryRequest
        {
            public string? Id { get; set; }
            public string? Token { get; set; }
            public bool? IsAvatar { get; set; }
            public string? ImageId { get; set; }
        }
        public class FriendRequest
        {
            public string? MyId { get; set; }
            public string? Id { get; set; }
            public string? Token { get; set; }
            public string? Code { get; set; }
        }
        public class FriendResponse
        {
            public int? Error { get; set; }
            public FriendInfo[]? Friends { get; set; }
        }
        public class FriendInfo
        {
            public string? Id { get; set; }
            public string? AvatarId { get; set; }
            public string? Name { get; set; }
            public int? Status { get; set; }
            public string? Points { get; set; }
            public int? Error { get; set; }
        }
        #endregion
    }
}
