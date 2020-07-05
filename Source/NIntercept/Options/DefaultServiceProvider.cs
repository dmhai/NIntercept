﻿using NIntercept.Builder;

namespace NIntercept
{
    public class DefaultServiceProvider : IProxyServiceProvider
    {
        private static readonly IProxyPropertyBuilder DefaultProxyPropertyBuilder;
        private static readonly IProxyMethodBuilder DefaultProxyMethodBuilder;
        private static readonly IProxyEventBuilder DefaultProxyEventBuilder;
        private static readonly ICallbackMethodBuilder DefaultCallbackMethodBuilder;

        static DefaultServiceProvider()
        {
            DefaultProxyPropertyBuilder = new ProxyPropertyBuilder();
            DefaultProxyMethodBuilder = new ProxyMethodBuilder();
            DefaultProxyEventBuilder = new ProxyEventBuilder();
            DefaultCallbackMethodBuilder = new CallbackMethodBuilder();
        }

        public virtual IProxyPropertyBuilder ProxyPropertyBuilder
        {
            get { return DefaultProxyPropertyBuilder; }
        }

        public virtual IProxyMethodBuilder ProxyMethodBuilder
        {
            get { return DefaultProxyMethodBuilder; }
        }

        public virtual IProxyEventBuilder ProxyEventBuilder
        {
            get { return DefaultProxyEventBuilder; }
        }

        public virtual ICallbackMethodBuilder CallbackMethodBuilder
        {
            get { return DefaultCallbackMethodBuilder; }
        }
    }
}