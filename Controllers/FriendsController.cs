using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BumberAPI.Models;
using Microsoft.AspNetCore.Mvc;
using static BumberAPI.Controllers.UserInfoController;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BumberAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        [HttpPost(URLs.addFriend)]
        public async Task<string> AddFriend([FromBody] object? req_obj)
        {
            UserInfoResponse response = new UserInfoResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            FriendRequest? req = ((JsonElement)req_obj).Deserialize<FriendRequest>();
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
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }
            if (me.ActionToken != req.Token)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }
            List<string> props = new List<string>();
            props.Add("SenderId");
            props.Add("RecieverId");
            props.Add("Info");
            List<string> vals = new List<string>();
            vals.Add(req.MyId);
            vals.Add(req.Id);
            vals.Add("Friend");

            List<Request> res = await RequestsDatabase.Find(props, vals);

            if (res != null)
                if (res.Count != 0)
                {
                    response.Error = 2;
                    return JsonSerializer.Serialize(response);
                }

            Request frReq = new Request();
            frReq.Info = "Friend";
            frReq.SenderId = req.MyId;
            frReq.RecieverId = req.Id;

            await RequestsDatabase.CreateAsync(frReq);

            return JsonSerializer.Serialize(response);
        }
        [HttpPost(URLs.deleteFriend)]
        public async Task<string> DeleteFriend([FromBody] object? req_obj)
        {
            UserInfoResponse response = new UserInfoResponse();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            FriendRequest? req = ((JsonElement)req_obj).Deserialize<FriendRequest>();
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
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }
            if (me.ActionToken != req.Token)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }
            List<string> props = new List<string>();
            props.Add("SenderId");
            props.Add("RecieverId");
            props.Add("Info");
            List<string> vals = new List<string>();
            vals.Add(req.MyId);
            vals.Add(req.Id);
            vals.Add("Friend");

            List<Request> res = await RequestsDatabase.Find(props, vals);

            if (res != null)
                if (res.Count == 1)
                {
                    await RequestsDatabase.RemoveAsync(res[0].Id);
                    vals[0] = req.Id;
                    vals[1] = req.MyId;
                    res = await RequestsDatabase.Find(props, vals);
                    if (res != null)
                        if (res.Count == 1)
                            await RequestsDatabase.RemoveAsync(res[0].Id);

                    return JsonSerializer.Serialize(response);
                }

            response.Error = 2;
            return JsonSerializer.Serialize(response);
        }
        [HttpPost(URLs.findFriend)]
        public async Task<string> FindFriend([FromBody] object? req_obj)
        {
            FriendInfo response = new FriendInfo();
            if (req_obj == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            FriendRequest? req = ((JsonElement)req_obj).Deserialize<FriendRequest>();
            if (req == null)
            {
                response.Error = 3;
                return JsonSerializer.Serialize(response);
            }
            if (req.MyId == null || req.Code == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            User? me = await UserDatabase.GetAsync(req.MyId);

            if (me == null)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }
            if (me.ActionToken != req.Token)
            {
                response.Error = 1;
                return JsonSerializer.Serialize(response);
            }

            List<string> props = new List<string>();
            props.Add("InviteCode");
            List<string> vals = new List<string>();
            vals.Add(req.Code);

            List<User> res = await UserDatabase.Find(props, vals);

            if (res != null)
                if (res.Count == 1 && res[0].Id!=null)
                {
                    response.Id = res[0].Id;
                    response.Name = res[0].FirstName + ' ' + res[0].LastName;
                    response.Points = res[0].Rate.ToString();
                    response.Status = res[0].Status;
                    props.Clear();
                    vals.Clear();

                    props.Add("UserId");
                    vals.Add(res[0].Id);

                    List<string> boolProps = new List<string>();
                    List<bool> boolValues = new List<bool>();

                    boolProps.Add("IsAvatar");
                    boolValues.Add(true);

                    List<Image> imgs = await ImagesDatabase.Find(props, vals, boolProps, boolValues);
                    if (imgs != null)
                    {
                        if (imgs.Count != 0)
                            response.AvatarId = imgs[0].Id;
                    }
                    return JsonSerializer.Serialize(response);
                }

            response.Error = 2;

            return JsonSerializer.Serialize(response);
        }

        [HttpPost(URLs.loadFriendList)]
        public async Task<string> LoadFriendList([FromBody] object? req_obj)
        {
            try
            {
                FriendResponse response = new FriendResponse();
                if (req_obj == null)
                {
                    response.Error = 3;
                    return JsonSerializer.Serialize(response);
                }
                FriendRequest? req = ((JsonElement)req_obj).Deserialize<FriendRequest>();
                if (req == null)
                {
                    response.Error = 3;
                    return JsonSerializer.Serialize(response);
                }
                if (req.MyId == null)
                {
                    response.Error = 1;
                    return JsonSerializer.Serialize(response);
                }

                User? me = await UserDatabase.GetAsync(req.MyId);

                if (me == null)
                {
                    response.Error = 1;
                    return JsonSerializer.Serialize(response);
                }
                if (me.ActionToken != req.Token)
                {
                    response.Error = 1;
                    return JsonSerializer.Serialize(response);
                }

                List<string> props = new List<string>();
                props.Add("SenderId");
                List<string> vals = new List<string>();
                vals.Add(req.MyId);

                List<Request> res = await RequestsDatabase.Find(props, vals);

                if (res != null)
                {
                    List<FriendInfo> frs = new List<FriendInfo>();
                    for (int i = 0; i < res.Count; i++)
                    {
                        if (res[i].RecieverId == null)
                            continue;
                        List<string> ansProps = new List<string>();
                        ansProps.Add("SenderId");
                        ansProps.Add("RecieverId");
                        List<string> ansVals = new List<string>();
                        ansVals.Add(res[i].RecieverId);
                        ansVals.Add(req.MyId);
                        List<Request> ansRes = await RequestsDatabase.Find(ansProps, ansVals);
                        if (ansRes != null)
                        {
                            if (ansRes.Count == 1)
                            {
                                FriendInfo fr = new FriendInfo();

                                ansProps.Clear();
                                ansVals.Clear();

                                User? user = await UserDatabase.GetAsync(res[i].RecieverId);

                                fr.Id = user.Id;
                                fr.Name = user.FirstName + ' ' + user.LastName;
                                fr.Points = user.Rate.ToString();
                                fr.Status = user.Status;

                                ansProps.Add("UserId");
                                ansVals.Add(fr.Id);

                                List<string> boolProps = new List<string>();
                                List<bool> boolValues = new List<bool>();

                                boolProps.Add("IsAvatar");
                                boolValues.Add(true);

                                var imgs = await ImagesDatabase.Find(ansProps, ansVals, boolProps, boolValues);
                                if (imgs != null)
                                {
                                    if (imgs.Count == 1)
                                        fr.AvatarId = imgs[0].Id;
                                }
                                frs.Add(fr);
                            }
                        }
                    }

                    response.Friends = frs.ToArray();
                }

                return JsonSerializer.Serialize(response);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}

