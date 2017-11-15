using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessageModule.Client.Models;
using System.Collections.Generic;

namespace MessageModule.Test
{
    [TestClass]
    public class ClientTest
    {
        #region MessageQueue
        [TestMethod]
        public void CreateMessage()
        {
            int oMessageId = MessageModule.Client.Controller.ClientController.CreateMessage
                (new Client.Models.ClientMessageModel()
                {
                    Agent = "AgentConfig_POL_Redeban_Mail",
                    User = "usario@sistema.com",
                    ProgramTime = DateTime.Now,

                    MessageQueueInfo = new System.Collections.Generic.List<Tuple<string, string>>()
                    {
                        new Tuple<string,string>("To","diego.jaramillo@proveedoresonline.co"),
                        new Tuple<string,string>("RememberUrl","https://www.proveedoresonline.co/marketplace/home/TermsAndConditios"),
                    },
                });

            Assert.AreEqual(true, oMessageId > 0);
        } 
        #endregion

        #region Notification
        [TestMethod]
        public void NotificationUpsert()
        {
            NotificationModel oReturn = new NotificationModel()
            {
                //NotificationId = 1, // PARA ACTUALIZAR
                Image = "fa fa_user",
                Label = "test",
                Url = "www.google.com.co",
                User = "sergio.palacios@proveedoresonline.co",
                State = 2013002,
                Enable = true,
            };

            oReturn.NotificationId = MessageModule.Client.Controller.ClientController.NotificationUpsert(oReturn);

            Assert.AreEqual(true, oReturn != null && oReturn.NotificationId > 0);
        }

        [TestMethod]
        public void NotificationInfoUpsert()
        {
            NotificationInfoModel oReturn = new NotificationInfoModel()
            {
                NotificationInfoId = 1, // PARA ACTUALIZAR
                NotificationId = 1,
                NotificationInfoType = 2008002,// Tipo de Notificación
                Value = "2005003", // MK 
                LargeValue = null,
                Enable = true,
            };

            oReturn.NotificationId = MessageModule.Client.Controller.ClientController.NotificationInfoUpsert(oReturn);

            Assert.AreEqual(true, oReturn != null && oReturn.NotificationId > 0);
        }

        [TestMethod]
        public void NotificationGetByUser()//FALTA PROBAR
        {
            List<NotificationModel> oReturn = MessageModule.Client.Controller.ClientController.NotificationGetByUser(
                //"DA5C572E",
                "sergio.palacios@proveedoresonline.co",
                true);

            Assert.AreEqual(true, oReturn != null && oReturn.Count > 0);
        } 
        #endregion

    }
}
