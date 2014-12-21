using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Markup;
using Castle.DynamicProxy;
using Mono.CSharp;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Helper
{
    [MarkupExtensionReturnType(typeof (object)), PublicAPI]
    public sealed class MockGeneratorExtension : MarkupExtension
    {
        [NotNull]
        public Type Type { get; set; }

        public MockGeneratorExtension()
        {
            
        }

        public MockGeneratorExtension([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            
            Type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {


            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName {Name = "MockGenerator"},
                AssemblyBuilderAccess.Run);

            var type = assembly.DefineDynamicModule("MockGenerator").DefineType(Type.Name + "Mock");


            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName |
                                                MethodAttributes.HideBySig;

            foreach (var propertyInfo in Type.GetProperties())
            {
                if(propertyInfo.GetIndexParameters().Length > 0) continue;

                var pbuilder = type.DefineProperty(propertyInfo.Name, propertyInfo.Attributes, propertyInfo.PropertyType,
                    null);

                MethodBuilder methodBuilder;
                ILGenerator generator;

                if (propertyInfo.CanRead)
                {
                    methodBuilder = type.DefineMethod("get_" + propertyInfo.Name, getSetAttr, propertyInfo.PropertyType,
                        null);

                    generator = methodBuilder.GetILGenerator();
                    generator.Emit(OpCodes.Ldnull);
                    generator.Emit(OpCodes.Ret);

                    pbuilder.SetGetMethod(methodBuilder);
                }
                if (!propertyInfo.CanWrite) continue;
                methodBuilder = type.DefineMethod("set_" + propertyInfo.Name, getSetAttr, null,
                    new[] {propertyInfo.PropertyType});

                generator = methodBuilder.GetILGenerator();
                generator.Emit(OpCodes.Ret);

                pbuilder.SetSetMethod(methodBuilder);
            }
            
            return Activator.CreateInstance(type.CreateType());
        }
    }
}
