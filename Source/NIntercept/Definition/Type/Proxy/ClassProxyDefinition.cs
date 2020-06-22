﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NIntercept.Definition
{
    public class ClassProxyDefinition : ProxyTypeDefinition
    {
        public ClassProxyDefinition(ModuleDefinition moduleDefinition, Type type, object target, ProxyGeneratorOptions options) 
            : base(moduleDefinition, type, target, options)
        {
        }

        protected override TypeDefinition GetTypeDefinition()
        {
            return this;
        }

        protected IClassProxyMemberSelector MemberSelector
        {
            get { return ModuleDefinition.MemberSelector; }
        }

        public override TypeDefinitionType TypeDefinitionType
        {
            get { return TypeDefinitionType.ClassProxy; }
        }

        protected override PropertyDefinition[] GetProperties()
        {
            BindingFlags flags = GetFlags();
            TypeDefinition typeDefinition = GetTypeDefinition();
            IEnumerable<PropertyInfo> properties = Type.GetProperties(flags);

            properties = properties.Where(MemberSelector.IncludeProperty); // virtual

            int length = properties.Count();
            PropertyDefinition[] propertyDefinitions = new PropertyDefinition[length];
            for (int i = 0; i < length; i++)
                propertyDefinitions[i] = new PropertyDefinition(typeDefinition, properties.ElementAt(i));

            return propertyDefinitions;
        }

        protected override MethodDefinition[] GetMethods()
        {
            BindingFlags flags = GetFlags();
            TypeDefinition typeDefinition = GetTypeDefinition();
            IEnumerable<MethodInfo> methods = Type
                .GetMethods(flags)
                .Where(m => m.DeclaringType != typeof(object) && !m.IsSpecialName);

            methods = methods.Where(MemberSelector.IncludeMethod); // virtual

            int length = methods.Count();
            MethodDefinition[] methodDefinitions = new MethodDefinition[length];
            for (int i = 0; i < length; i++)
                methodDefinitions[i] = new MethodDefinition(typeDefinition, methods.ElementAt(i));

            return methodDefinitions;
        }

        protected override EventDefinition[] GetEvents()
        {
            BindingFlags flags = GetFlags();
            TypeDefinition typeDefinition = GetTypeDefinition();
            IEnumerable<EventInfo> events = Type.GetEvents(flags);

            events = events.Where(MemberSelector.IncludeEvent); // virtual

            int length = events.Count();
            EventDefinition[] eventDefinitions = new EventDefinition[length];
            for (int i = 0; i < length; i++)
                eventDefinitions[i] = new EventDefinition(typeDefinition, events.ElementAt(i));

            return eventDefinitions;
        }
    }

}
