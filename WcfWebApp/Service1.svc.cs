using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfWebApp.Models;
using System.IO;

namespace WcfWebApp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        public void DoWork()
        {
        }


        public Models.User[] GetUsers()
        {
            //TODO: To use this properly tie it into your own business logic
            return new User[] {new User{Id = 1, Name = "Test Guy 1",Email = "AwesomeTester@xamarin.com"},
                                new User{ Id = 2, Name="\"Beeta Man\"",Email = "Chrisntr@xamarin.com"}};
        }

        public Models.TaskList GetTasks(int userId)
        {
            //TODO: To use this properly tie it into your own business logic
            return new TaskList { Id = 121, User = new User { Id = 1, Name = "Test Guy 1",
                Email = "AwesomeTester@xamarin.com" }, 
                Items = new List<Item> {
                    new Item{Id = 1, Title = "Get things done!" },
                    new Item {Id = 2, Title = "Catch a leprechaun!"},

                } };
        }

        const string BasePath = "c:\\UploadedImages";
        public bool UploadImage(Models.ImageData imageData)
        {
            try
            {
                if (!Directory.Exists(BasePath))
                    Directory.CreateDirectory(BasePath);
                var fullFilePath = Path.Combine(BasePath, imageData.FileName);
                File.WriteAllBytes(fullFilePath, imageData.ImageContent);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public byte[] ConvertToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }
    }
}
