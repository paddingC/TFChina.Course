using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFChina.Course.Data;

namespace TFChina.Course.Service
{
    public class UserService
    {
        /// <summary>
        /// user Login 
        /// </summary>
        /// <returns></returns>
        public int Login(string account,string password)
        {
            using (var dbContext = new CourseDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(p=>p.Account==account && p.Password==password);
                if (user == null) return -1;
                return user.UserID;
            }
        }


        /// <summary>
        /// user Login 
        /// </summary>
        /// <returns></returns>
        public int Login(string account)
        {
            using (var dbContext = new CourseDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(p => p.Account == account);
                if (user == null) return -1;
                return user.UserID;
            }
        }

        public User GetUserById(int userId)
        {
            using (var dbContext = new CourseDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(p => p.UserID==userId);
                return user;
            }
        }

        public IList<User> GetAll()
        {
            using (var dbContext = new CourseDbContext())
            {
                return dbContext.Users.ToList();
            }
        }

        public Tuple<int,IList<User>> GetUsers(int pageIndex=1,int pageSize=20)
        {
            using (var dbContext = new CourseDbContext())
            {
                var allUsers = dbContext.Users;
                var pagedUsers = allUsers.OrderBy(p=>p.UserID).Skip(pageIndex - 1).Take(pageSize).ToList();
                return new Tuple<int, IList<User>>(allUsers.Count(), pagedUsers);
            }
        }


        public bool ResetPassword(int userId,string password)
        {
            using (var dbContext = new CourseDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(p => p.UserID == userId);
                if (user == null) throw new Exception("未查找到用户：" + userId);
                user.Password = password;
                dbContext.SaveChanges();
                return true;
            }
        }
    }
}
