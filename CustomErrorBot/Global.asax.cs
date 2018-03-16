using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using CustomErrorBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;

namespace CustomErrorBot
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Conversation.UpdateContainer(
                builder =>
                {
                    builder.RegisterModule(new DefaultExceptionMessageOverrideModule());
                });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
