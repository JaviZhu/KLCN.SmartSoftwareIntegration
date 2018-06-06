

using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Principal;

namespace WebApp.Controllers
{
    public class Basic : Controller
    {
        public Basic()
        {
            CustomUser.UserName = WindowsIdentity.GetCurrent().Name;

            var userproperty = OperateActiveDirectory.SearchForUser(CustomUser.UserName.ToLower().Replace("corpfcsint\\", ""));
            CustomUser.DN = userproperty["DN"];
            CustomUser.Department = userproperty["Department"];
            CustomUser.DisplayName = userproperty["DisplayName"];
            CustomUser.IpPhone = userproperty["IpPhone"];
            CustomUser.Mail = userproperty["Mail"];
            CustomUser.Mobile = userproperty["Mobile"];
            CustomUser.TelephoneNumber = userproperty["TelephoneNumber"];
            CustomUser.ThumbnailPhoto = userproperty["ThumbnailPhoto"];
            CustomUser.Title = userproperty["Title"];
            CustomUser.WwwHomepage = userproperty["WwwHomepage"];

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
        public static string DN { get; set; }
        public static string UserName { get; set; }
        public static string Mail { get; set; }
        public static string DisplayName { get; set; }
        public static string ThumbnailPhoto { get; set; }
        public static string Title { get; set; }
        public static string Mobile { get; set; }
        public static string IpPhone { get; set; }
        public static string TelephoneNumber { get; set; }
        public static string Department { get; set; }
        public static string WwwHomepage { get; set; }
    }
}
