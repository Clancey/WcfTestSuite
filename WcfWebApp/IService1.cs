using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfWebApp.Models;

namespace WcfWebApp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        User[] GetUsers();

        [OperationContract]
        TaskList GetTasks(int userId);

        [OperationContract]
        bool UploadImage(ImageData imageData);

        [OperationContract]
        byte[] ConvertToByteArray(string theString);

    }
}
