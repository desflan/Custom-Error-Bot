using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Web;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Internals.Scorables;
using Microsoft.Bot.Connector;

namespace CustomErrorBot.Dialogs
{
    public class DefaultExceptionMessageOverrideModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder
                .Register(c =>
                {
                    var cc = c.Resolve<IComponentContext>();
                    Func<IPostToBot> makeInner = () =>
                    {
                        IPostToBot post = new ReactiveDialogTask(cc.Resolve<IDialogTask>(),
                            cc.Resolve<Func<IDialog<object>>>());
                        post = new ExceptionTranslationDialogTask(post);
                        post = new LocalizedDialogTask(post);
                        post = new ScoringDialogTask<double>(post, cc.Resolve<TraitsScorable<IActivity, double>>());
                        return post;
                    };

                    IPostToBot outer = new PersistentDialogTask(makeInner, cc.Resolve<IBotData>());
                    outer = new SerializingDialogTask(outer, cc.Resolve<IAddress>(), c.Resolve<IScope<IAddress>>());
                    outer = new PostUnhandledExceptionToUserOverrideTask(outer, cc.Resolve<IBotToUser>(),
                        cc.Resolve<ResourceManager>(), cc.Resolve<TraceListener>());
                    outer = new LogPostToBot(outer, cc.Resolve<IActivityLogger>());
                    return outer;
                })
                .As<IPostToBot>()
                .InstancePerLifetimeScope();

        }
    }
}