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

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.Bot.Builder.FormFlow.Advanced
{
    internal enum StepPhase { Ready, Responding, Completed };
    internal enum StepType { Field, Confirm, Navigation, Message };

    internal struct StepResult
    {
        internal StepResult(NextStep next, string feedback, string prompt)
        {
            this.Next = next;
            this.Feedback = feedback;
            this.Prompt = prompt;
        }

        internal NextStep Next { get; set; }
        internal string Feedback { get; set; }
        internal string Prompt { get; set; }
    }

    internal interface IStep<T>
    {
        string Name { get; }

        StepType Type { get; }

        TemplateBaseAttribute Annotation { get; }

        IField<T> Field { get; }

#if LOCALIZE
        void SaveResources();

        void Localize();
#endif

        bool Active(T state);

        Task<bool> DefineAsync(T state);

        string Start(IDialogContext context, T state, FormState form);

        IEnumerable<TermMatch> Match(IDialogContext context, T state, FormState form, string input);

        Task<StepResult> ProcessAsync(IDialogContext context, T state, FormState form, string input, IEnumerable<TermMatch> matches);

        string NotUnderstood(IDialogContext context, T state, FormState form, string input);

        string Help(T state, FormState form, string commandHelp);

        bool Back(IDialogContext context, T state, FormState form);

        IEnumerable<string> Dependencies { get; }
    }

}
