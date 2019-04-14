using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Mocky
{
    public class Mocker
    {
        private readonly Type klass;
        private Dictionary<string, MockMethod> ms;

        public Mocker(Type klass)
        {
            this.klass = klass;
            this.ms = new Dictionary<string, MockMethod>();
        }

        public MockMethod When(string name)
        {
            MockMethod m;
            if (!ms.TryGetValue(name, out m))
            {
                m = new MockMethod(klass, name);
                ms.Add(name, m);
            }
            return m;
        }

        public object Create()
        {
            Type t = BuildType();
            return Activator.CreateInstance(t, new object[1] { ms.Values.ToArray() });
        }

        private Type BuildType()
        {
            string theName = "Mock" + klass.Name;
            string ASM_NAME = theName;
            string MOD_NAME = theName;
            string TYP_NAME = theName;
            string DLL_NAME = theName + ".dll";
            
            AssemblyBuilder asmBuilder =
                AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName(ASM_NAME), AssemblyBuilderAccess.RunAndSave);
            
            ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule(MOD_NAME , DLL_NAME);
            
            TypeBuilder typeBuilder = modBuilder.DefineType(TYP_NAME);
            
            ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[1] { typeof(MockMethod[]) });
            
            ILGenerator ctorIL = ctor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ret);
            
            typeBuilder.AddInterfaceImplementation(klass);

            List<Type> interfaces = GetAllInterfaces(new List<Type>(){klass});

            foreach (Type inter in interfaces)
            {
                foreach (MethodInfo method in inter.GetMethods())
                {
                    MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                        method.Name, 
                        MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot,
                        method.ReturnType, 
                        GetParameterTypes(method.GetParameters()));
                
                    ImplementMethod(methodBuilder);
                }
            }
            
            Type dynamicType = typeBuilder.CreateTypeInfo().AsType();
            
            asmBuilder.Save(DLL_NAME);

            return dynamicType;
        }

        private Type[] GetParameterTypes(ParameterInfo[] getParameters)
        {
            Type[] res = new Type[getParameters.Length];
            int idx = 0;
            foreach (ParameterInfo parameter in getParameters)
            {
                res[idx++] = parameter.ParameterType;
            }

            return res;
        }

        private List<Type> GetAllInterfaces(List<Type> interfaces)
        {
            List<Type> res = new List<Type>();
            res.AddRange(interfaces);
            foreach (Type inter in interfaces)
            {
                res.AddRange(GetAllInterfaces(inter.GetInterfaces().ToList()));
            }
            return res;
        }

//        private Type BuildType()
//        {
//            string theName = "Mock" + klass.Name;
//            string ASM_NAME = theName;
//            string MOD_NAME = theName;
//            string TYP_NAME = theName;
//            string DLL_NAME = theName + ".dll";
//
//            AssemblyBuilder asmBuilder =
//                AssemblyBuilder.DefineDynamicAssembly(
//                    new AssemblyName(ASM_NAME), AssemblyBuilderAccess.RunAndSave);
//
//            ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule(MOD_NAME , DLL_NAME);
//
//            TypeBuilder typeBuilder = modBuilder.DefineType(TYP_NAME);
//            
//            typeBuilder.AddInterfaceImplementation(klass);
//
//            //ConstructorBuilder ctor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
//            ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[1] { typeof(MockMethod[]) });
//            //ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0] { });
//
//            ILGenerator ctorIL = ctor.GetILGenerator();
//            ctorIL.Emit(OpCodes.Ret);
//
//            MethodBuilder addMethodBuilder = typeBuilder.DefineMethod(
//                "Add", 
//                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot, 
//                typeof(int), 
//                new Type[2] {typeof(int), typeof(int)});
//            
//            
//            MethodBuilder subMethodBuilder = typeBuilder.DefineMethod(
//                "Sub", 
//                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot, 
//                typeof(int), 
//                new Type[2] {typeof(int), typeof(int)});
//            
//            MethodBuilder mulMethodBuilder = typeBuilder.DefineMethod(
//                "Mul", 
//                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot, 
//                typeof(int), 
//                new Type[2] {typeof(int), typeof(int)});
//            
//            MethodBuilder divMethodBuilder = typeBuilder.DefineMethod(
//                "Div", 
//                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot, 
//                typeof(int), 
//                new Type[2] {typeof(int), typeof(int)});
//
//            ImplementMethod(addMethodBuilder);
//            ImplementMethod(subMethodBuilder);
//            ImplementMethod(mulMethodBuilder);
//            ImplementMethod(divMethodBuilder);
//
//            Type dynamicType = typeBuilder.CreateTypeInfo().AsType();
//            
//            asmBuilder.Save(DLL_NAME);
//
//            return dynamicType;
//        }

        private static void ImplementMethod(MethodBuilder addMethodBuilder)
        {
            ILGenerator methIL = addMethodBuilder.GetILGenerator();
            methIL.ThrowException(typeof(NotImplementedException));
        }
    }
}