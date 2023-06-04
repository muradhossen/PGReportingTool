using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ConnectingToPG.Models;
using Npgsql.Replication.PgOutput;

namespace ConnectingToPG
{
    public class DynamicClassFactory
    {
        public Type CreateClass(string className, List<PropertyParam> properties)
        {
            // Create a dynamic assembly
            AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            // Create a dynamic module in the assembly
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            // Create a dynamic type (class)
            TypeBuilder typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class);


            for (int i = 0; i < properties.Count; i++)
            {

                var property = properties[i];

                // Define properties in the dynamic type
                DefineProperty(typeBuilder, property.Name, GetPropertyTypeModifier(property.Type));
            }

           
          //  DefineProperty(typeBuilder, "Age", typeof(int));

            // Create the dynamic type
            return typeBuilder.CreateType();

            // Create an instance of the dynamic type
            //dynamic instance = Activator.CreateInstance(dynamicType);


            //return instance;
            //// Set property values
            //PropertyInfo nameProperty = dynamicType.GetProperty("Name");
            //nameProperty.SetValue(instance, "John Doe");

            //PropertyInfo ageProperty = dynamicType.GetProperty("Age");
            //ageProperty.SetValue(instance, 25);

            //// Get property values
            //string name = (string)nameProperty.GetValue(instance);
            //int age = (int)ageProperty.GetValue(instance);

            //// Output the property values
            //Console.WriteLine("Name: " + name);
            //Console.WriteLine("Age: " + age);
        }

        private static void DefineProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            // Define the private field backing the property
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

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
            getterILGenerator.Emit(OpCodes.Ldfld, fieldBuilder); // Load the value of the field onto the stack
            getterILGenerator.Emit(OpCodes.Ret);

            // Define the setter method
            MethodBuilder setterMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                new Type[] { propertyType });

            ILGenerator setterILGenerator = setterMethodBuilder.GetILGenerator();
            setterILGenerator.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
            setterILGenerator.Emit(OpCodes.Ldarg_1); // Load the value to be assigned onto the stack
            setterILGenerator.Emit(OpCodes.Stfld, fieldBuilder); // Assign the value to the field
            setterILGenerator.Emit(OpCodes.Ret);

            // Set the getter and setter methods for the property
            propertyBuilder.SetGetMethod(getterMethodBuilder);
            propertyBuilder.SetSetMethod(setterMethodBuilder);
        }

        private static Type GetPropertyTypeModifier(string propertyType)
        {
            if (string.IsNullOrWhiteSpace(propertyType))
            {
                throw new ArgumentNullException($"{propertyType} type cann't be null or empty!");
            }

            return propertyType switch
            {
                "string" => typeof(string),
                "double" => typeof(double),
                "int" => typeof(int),
                "long" => typeof(long), 
                "float" => typeof(float),
                "object" => typeof(object),                 
                _=> throw new NotSupportedException("Property type not supported."),
            };
        }

    }
}
