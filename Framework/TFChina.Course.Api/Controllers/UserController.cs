using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Http;
using TFChina.Course.Api.Models;
using TFChina.Course.Data;
using TFChina.Course.Service;

namespace TFChina.Course.Api.Controllers
{
    public class UserController : ApiController
    {
        private readonly UserService _userService;
        //private readonly EmailService _emailService;
        public UserController()
        {
            _userService = new UserService();
            //_emailService = new EmailService();
        }

        [Route("v1/user/{userId}")]
        public DataTransferObject Get(string userId)
        {
            try
            {
                if (!Int32.TryParse(userId, out int nUserId) || nUserId <= 0)
                {
                    return new DataTransferObject { Status = 0, Message = "UserId必须为数字且大于0" };
                }
                var user = _userService.GetUserById(nUserId);
                return new DataTransferObject { Status = 1 , Result = user };
            }
            catch (Exception ex)
            {
                return new DataTransferObject { Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace };
            }
        }

        [Route("v1/users")]
        public DataTransferObject GetAll()
        {
            try
            {
                var userList = _userService.GetAll();
                return new DataTransferObject { Status = 1, Result = userList };
            }
            catch (Exception ex)
            {
                return new DataTransferObject { Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace };
            }
        }

        [Route("v1/users/{pageIndex}/{pageSize}")]
        public DataTransferObject GetUsers(string pageIndex="1",string pageSize="20")
        {
            try
            {
                if (!Int32.TryParse(pageIndex, out int nPageIndex) || nPageIndex <= 0)
                {
                    return new DataTransferObject { Status = 0, Message = "pageIndex必须为数字且大于0" };
                }

                if (!Int32.TryParse(pageSize, out int nPageSize) || nPageSize <= 0)
                {
                    return new DataTransferObject { Status = 0, Message = "pageSize必须为数字且大于0" };
                }

                var tupleResult = _userService.GetUsers(nPageIndex,nPageSize);
                var result = new UsersOutput
                {
                    PageIndex = nPageIndex,
                    PageSize = nPageSize,
                    Count = tupleResult.Item1,
                    Users = tupleResult.Item2
                }; 
                return new DataTransferObject { Status = 1, Result = result };
            }
            catch (Exception ex)
            {
                return new DataTransferObject { Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace };
            }
        }

        // POST api/<controller>
        public DataTransferObject Post([FromBody]LoginModel model)
        {
            try
            {
                if (model==null||string.IsNullOrEmpty(model.Account))
                {
                    return new DataTransferObject { Status = 0, Message = "账号不能为空" };
                }

                if (!Regex.IsMatch(model.Account, "^[A-Za-z0-9]+([-_.][A-Za-z0-9]+)*@([A-Za-zd0-9]+[-.])+[A-Za-zd]{2,5}$"))
                {
                    return new DataTransferObject { Status = 0, Message = "账号必须为正确的邮件格式" };
                }

                if (!model.Account.ToLower().EndsWith("@tfchina.org"))
                {
                    return new DataTransferObject { Status = 0, Message = "请输入美丽中国邮箱" };
                }

                var result = _userService.Login(model.Account);
                if (result > 0)
                {
                    //DO:验证通过后，可以向客户端发送TOKEN
                    return new DataTransferObject { Status = 1, Result = new { UserId = result }, Message = "登录成功" };
                }
                else
                {
                    return new DataTransferObject { Status = 0, Message = "未查找到用户："+ model.Account };
                }
            }
            catch (Exception ex)
            {
                return new DataTransferObject { Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace };
            }
        }

        //[Route("api/reset/password")]
        //[HttpPost]
        //public DataTransferObject ResetPassword([FromBody]ResetPasswordModel model)
        //{
        //    try
        //    {
        //        if (model.UserId<=0)
        //        {
        //            return new DataTransferObject { Status = 0, Message = "UserId必须大于0" };
        //        }

        //        if (string.IsNullOrEmpty(model.NewPassword))
        //        {
        //            return new DataTransferObject { Status = 0, Message = "新密码不能为空" };
        //        }

        //        if (string.IsNullOrEmpty(model.ConfirmedPassword))
        //        {
        //            return new DataTransferObject { Status = 0, Message = "被确认的新密码不能为空" };
        //        }

        //        if (string.CompareOrdinal(model.ConfirmedPassword,model.NewPassword)!=0)
        //        {
        //            return new DataTransferObject { Status = 0, Message = "密码不一致" };
        //        }

        //        var result = _userService.ResetPassword(model.UserId, model.ConfirmedPassword);
        //        if (result)
        //        {
        //            //DO:密码更新成功后，发送邮件通知用户
        //            var user = _userService.GetUserById(model.UserId);
        //            var message = new Message
        //            {
        //                To = user.Account,
        //                Subject = GetMsgSubject(user),
        //                Content = GetMsgContent(user),
        //            };
        //            _emailService.Send(message);
        //            return new DataTransferObject { Status = 1, Message = "密码重置成功" };
        //        }
        //        else
        //        {
        //            return new DataTransferObject { Status = 0, Message = "密码重置失败" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataTransferObject { Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace };
        //    }
        //}

        //private string GetMsgContent(User user)
        //{
        //    var content = new StringBuilder();
        //    content.Append($"尊敬的{user.Name},");
        //    content.Append("<br/><br/>");
        //    content.Append($"您的密码重置为：{user.Password}。如果非本人操作，请联系管理员");
        //    content.Append("<br/><br/>");
        //    content.Append(ConfigurationManager.AppSettings["Smtp:DisplayName"]);

        //    return content.ToString();
        //}

        //private string GetMsgSubject(User user)
        //{
        //    var subject = $"密码重置通知";
        //    return subject.ToString();
        //}
    }
}