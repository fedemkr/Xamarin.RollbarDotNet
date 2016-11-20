﻿using System.Diagnostics;
using System.Linq;

namespace RollbarDotNet.iOS
{
    public static class Rollbar
    {
        public static void Init(RollbarConfig config = null)
        {
            Data.DefaultPlatform = "iOS";
            FrameFactory.Init(FrameBuilder);
            RollbarDotNet.Rollbar.Current = new RollbarImplementation(config);
        }

        static Frame[] FrameBuilder(System.Exception exception)
        {
            var frames = new StackTrace(exception, true).GetFrames() ?? new StackFrame[0];
            return frames
                .Select(f => new StackFrameWrapper(f))
                .Select(f => new Frame(f))
                .ToArray();
        }
    }
}