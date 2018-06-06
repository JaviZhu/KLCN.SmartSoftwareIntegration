

using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Web.Models;

namespace Web.Controllers
{
    public class Basic : Controller
    {
        public CustomUser customUser;
        public Paging paging;
        public Basic()
        {
            paging = new Paging();
            paging.CurrentPage = 1;
            paging.DisplayCount = 8;
        }
        public void FindActiveDictionay(string username)
        {
            customUser = new CustomUser();

            customUser.UserName = username;// WindowsIdentity.GetCurrent().Name;


            var userproperty = OperateActiveDirectory.SearchForUser(customUser.UserName.ToLower().Replace("corpfcsint\\", ""));
            customUser.DN = userproperty["DN"];
            customUser.Department = userproperty["Department"];
            customUser.DisplayName = userproperty["DisplayName"];
            customUser.IpPhone = userproperty["IpPhone"];
            customUser.Mail = userproperty["Mail"];
            customUser.Mobile = userproperty["Mobile"];
            customUser.TelephoneNumber = userproperty["TelephoneNumber"];
            customUser.ThumbnailPhoto = userproperty["ThumbnailPhoto"];
            customUser.Title = userproperty["Title"];
            customUser.WwwHomepage = userproperty["WwwHomepage"];

            //CustomUser.DN = userproperty.DN;
            //CustomUser.Department = userproperty.Department;
            //CustomUser.DisplayName = userproperty.DisplayName;
            //CustomUser.IpPhone = userproperty.IpPhone;
            //CustomUser.Mail = userproperty.Mail;
            //CustomUser.Mobile = userproperty.Mobile;
            //CustomUser.TelephoneNumber = userproperty.TelephoneNumber;
            //CustomUser.ThumbnailPhoto = userproperty.ThumbnailPhoto;
            //CustomUser.Title = userproperty.Title;
            //CustomUser.WwwHomepage = userproperty.WwwHomepage;
        }
    }
    public class CustomUser
    {
        public string DN { get; set; }
        public string UserName { get; set; }
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string ThumbnailPhoto { get; set; }
        public string Title { get; set; }
        public string Mobile { get; set; }
        public string IpPhone { get; set; }
        public string TelephoneNumber { get; set; }
        public string Department { get; set; }
        public string WwwHomepage { get; set; }
    }
    public class Paging
    {
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int Redirect { get; set; }
        public int DisplayCount { get; set; }
        public decimal Pages { get
            {
                decimal num = System.Math.Ceiling((decimal)this.TotalPage / (decimal)this.DisplayCount);
                return num;
            } }
    }
}
