using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Web;
using Autofac;
using Microsoft.Bot.Builder.Autofac.Base;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Builder.Internals.Fibers;
//using Microsoft.Bot.Builder.Internals.Scorables;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;

namespace CustomErrorBot.Dialogs
{
    public class DefaultExceptionMessageOverrideModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
               .RegisterKeyedType<PostUnhandledExceptionToUserOverrideTask, IPostToBot>()
               .InstancePerLifetimeScope();

            builder
                .RegisterAdapterChain<IPostToBot>
                (
                    typeof(EventLoopDialogTask),
                    typeof(SetAmbientThreadCulture),
                    typeof(QueueDrainingDialogTask),
                    typeof(PersistentDialogTask),
                    typeof(ExceptionTranslationDialogTask),
                    typeof(SerializeByConversation),
                    typeof(PostUnhandledExceptionToUserOverrideTask),
                    typeof(LogPostToBot)
                )
                .InstancePerLifetimeScope();
        }
    }
}