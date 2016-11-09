using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace CustomErrorBot.Dialogs
{
    public class PostUnhandledExceptionToUserOverrideTask : IPostToBot
    {
        private readonly ResourceManager resources;
        private readonly IPostToBot inner;
        private readonly IBotToUser botToUser;
        private readonly TraceListener trace;

        public PostUnhandledExceptionToUserOverrideTask(IPostToBot inner, IBotToUser botToUser, ResourceManager resources, TraceListener trace)
        {
            SetField.NotNull(out this.inner, nameof(inner), inner);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
            SetField.NotNull(out this.resources, nameof(resources), resources);
            SetField.NotNull(out this.trace, nameof(trace), trace);
        }

        async Task IPostToBot.PostAsync<T>(T item, CancellationToken token)
        {
            try
            {
                await inner.PostAsync(item, token);
            }
            catch (Exception)
            {
                try
                {
                    await botToUser.PostAsync("An Error Has Occurred.....", cancellationToken: token);
                }
                catch (Exception inner)
                {
                    trace.WriteLine(inner);
                }

                throw;
            }
        }
    }
}