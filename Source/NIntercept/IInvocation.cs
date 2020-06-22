﻿using System;
using System.Reflection;

namespace NIntercept
{
    public interface IInvocation
    {
        MemberInfo Member { get; }
        object Proxy { get; }
        object ReturnValue { get; set; }
        object[] Parameters { get; }
        MethodInfo CallerMethod { get; }
        Type InterceptorProviderType { get; }
        object Target { get; }

        IAwaitableInvocation GetAwaitableContext();
        T GetParameter<T>(int index);
        void Proceed();
    }
}
