using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingToPG
{
    public class DynamicClassFactory
    {
        public void CreateClass()
        {
            // Create a dynamic assembly
            AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            // Create a dynamic module in the assembly
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            // Create a dynamic type (class)
            TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicClass", TypeAttributes.Public | TypeAttributes.Class);

            // Define properties in the dynamic type
            DefineProperty(typeBuilder, "Name", typeof(string));
            DefineProperty(typeBuilder, "Age", typeof(int));

            // Create the dynamic type
            Type dynamicType = typeBuilder.CreateType();

            // Create an instance of the dynamic type
            dynamic instance = Activator.CreateInstance(dynamicType);

            // Set property values
            PropertyInfo nameProperty = dynamicType.GetProperty("Name");
            nameProperty.SetValue(instance, "John Doe");

            PropertyInfo ageProperty = dynamicType.GetProperty("Age");
            ageProperty.SetValue(instance, 25);

            // Get property values
            string name = (string)nameProperty.GetValue(instance);
            int age = (int)ageProperty.GetValue(instance);

            // Output the property values
            Console.WriteLine("Name: " + name);
            Console.WriteLine("Age: " + age);
        }

        private static void DefineProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            // Define the property
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName,
                PropertyAttributes.HasDefault,
                propertyType,
                null);

            // Define the getter method
            MethodBuilder getterMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes);

            ILGenerator getterILGenerator = getterMethodBuilder.GetILGenerator();
            getterILGenerator.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
            getterILGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("ToString")); // Replace this with actual getter logic
            getterILGenerator.Emit(OpCodes.Ret);

            // Set the getter method for the property
            propertyBuilder.SetGetMethod(getterMethodBuilder);
        }
    }
}
