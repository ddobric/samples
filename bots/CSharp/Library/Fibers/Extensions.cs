﻿// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Bot Framework: http://botframework.com
// 
// Bot Builder SDK Github:
// https://github.com/Microsoft/BotBuilder
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.Bot.Builder.Internals.Fibers
{
    public static partial class Extensions
    {
        // TODO: split off R to get better type inference on T
        public static IWait Call<T, R>(this IFiber fiber, Rest<T> invokeHandler, T item, Rest<R> returnHandler)
        {
            fiber.NextWait<R>().Wait(returnHandler);
            return fiber.Call<T>(invokeHandler, item);
        }

        public static IWait Call<T>(this IFiber fiber, Rest<T> invokeHandler, T item)
        {
            fiber.Push();
            var wait = fiber.NextWait<T>();
            wait.Wait(invokeHandler);
            wait.Post(item);
            return wait;
        }

        public static IWait Wait<T>(this IFiber fiber, Rest<T> resumeHandler)
        {
            var wait = fiber.NextWait<T>();
            wait.Wait(resumeHandler);
            return wait;
        }

        public static IWait Done<T>(this IFiber fiber, T item)
        {
            fiber.Done();
            var wait = fiber.Wait;
            wait.Post(item);
            return wait;
        }

        public static void Post<T>(this IFiber fiber, T item)
        {
            fiber.Wait.Post(item);
        }

        public static Task<T> ToTask<T>(this IAwaitable<T> item)
        {
            var source = new TaskCompletionSource<T>();
            try
            {
                var result = item.GetAwaiter().GetResult();
                source.SetResult(result);
            }
            catch (Exception error)
            {
                source.SetException(error);
            }

            return source.Task;
        }
    }
}
