﻿using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NIntercept
{
    public class Kernel
    {
        // ModuleScope (AssemblyBuilder)
        // ModuleBuilder Main
        // ProxyTypeBuilder current??


        // CreateClassProxy 
        // ... CreateTypeBuilder => Define Type, interfaces, attributes
        // => define fields ... FieldBuilder
        // => define ctor ... ContructorBuilder
        // => define properties ... PropertyBuilder
        // => define methods ... MethodBuilder
        // => define events ... EventBuilder

        // ModuleScope/Context, Proxy TypeBuilder, Definition, Fields
    }

    //public class ClassProxyBuilder
    //{
    //    public TypeBuilder CreateClassProxyType(ClassProxyDefinition typeDefinition)
    //    {

    //    }

    //    protected virtual TypeBuilder DefineType(ModuleBuilder moduleBuilder, TypeDefinition typeDefinition)
    //    {
    //        TypeBuilder typeBuilder = moduleBuilder.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);
    //        typeBuilder.AddCustomAttribute(typeof(SerializableAttribute));
    //        typeBuilder.SetParent(typeDefinition.Type);

    //        return typeBuilder;
    //    }
    //}

    //public class InterfaceProxyBuilder
    //{

    //}

}
