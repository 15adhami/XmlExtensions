using System;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    /// <summary>
    /// Class for emitting a Mod subclass with given SettingsMenuDef
    /// </summary>
    internal static class ModTypeEmitter
    {
        internal static Type EmitModType(SettingsMenuDef settingsMenuDef)
        {
            // Get ModContentPack
            ModContentPack content = settingsMenuDef.modContentPack;
            if (content == null)
            {
                Verse.Log.Error("[XML Extensions] SettingsMenuDef " + settingsMenuDef.defName + " ModContentPack is null");
                return null;
            }

            // Define dynamic assembly
            string modName = "XmlExtensions_Mod_" + settingsMenuDef.defName;
            var asmName = new AssemblyName(modName);
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            var moduleBuilder = asmBuilder.DefineDynamicModule(modName + "Module");

            // Define the Mod subclass
            TypeBuilder typeBuilder = moduleBuilder.DefineType(modName, TypeAttributes.Public | TypeAttributes.Class, typeof(Mod));

            // Define constructor
            ConstructorInfo baseCtor = typeof(Mod).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, [typeof(ModContentPack)], null);
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, [typeof(ModContentPack)]);
            ILGenerator ilCtor = ctorBuilder.GetILGenerator();
            ilCtor.Emit(OpCodes.Ldarg_0);        // load this
            ilCtor.Emit(OpCodes.Ldarg_1);        // load content
            ilCtor.Emit(OpCodes.Call, baseCtor); // call base(content)
            ilCtor.Emit(OpCodes.Ret);

            // Define settingsMenuDefField field
            FieldBuilder settingsMenuDefField = typeBuilder.DefineField("settingsMenuDef", typeof(SettingsMenuDef), FieldAttributes.Public | FieldAttributes.Static);

            // Define DoSettingsWindowContents(Rect) method
            MethodBuilder drawMethod = typeBuilder.DefineMethod("DoSettingsWindowContents", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), [typeof(Rect)]);

            // Implement DoSettingsWindowContents(Rect) method
            // Create call to Find.WindowStack.Add(new XmlExtensions_MenuModSettings(settingsMenuDef, false))
            MethodInfo findWindowStack = typeof(Find).GetProperty("WindowStack").GetGetMethod();
            MethodInfo addWindowMethod = typeof(WindowStack).GetMethod("Add", [typeof(Window)]);
            ConstructorInfo settingsWindowCtor = typeof(ModSettingsWindow).GetConstructor([typeof(SettingsMenuDef), typeof(bool)]);

            // Emit IL of DoSettingsWindowContents(Rect) method
            ILGenerator ilDraw = drawMethod.GetILGenerator();
            ilDraw.Emit(OpCodes.Call, findWindowStack);        // call Find.WindowStack
            ilDraw.Emit(OpCodes.Ldsfld, settingsMenuDefField); // load settingsMenuDef field
            ilDraw.Emit(OpCodes.Ldc_I4_0);                     // load 'false'
            ilDraw.Emit(OpCodes.Newobj, settingsWindowCtor);   // create XmlExtensions_MenuModSettings object
            ilDraw.Emit(OpCodes.Callvirt, addWindowMethod);    // call Find.WindowStack.Add(Window)
            ilDraw.Emit(OpCodes.Ret);                          // return

            // Override DoSettingsWindowContents(Rect) method
            MethodInfo baseDrawMethod = typeof(Mod).GetMethod("DoSettingsWindowContents", BindingFlags.Public | BindingFlags.Instance);
            typeBuilder.DefineMethodOverride(drawMethod, baseDrawMethod);

            // Define SettingsCategory() method
            MethodBuilder catMethod = typeBuilder.DefineMethod("SettingsCategory", MethodAttributes.Public | MethodAttributes.Virtual, typeof(string), Type.EmptyTypes);

            // Emit IL of SettingsCategory() method
            ILGenerator ilCat = catMethod.GetILGenerator();
            ilCat.Emit(OpCodes.Ldstr, Helpers.TryTranslate(settingsMenuDef.label, settingsMenuDef.tKey));
            ilCat.Emit(OpCodes.Ret);

            // Override SettingsCategory() method
            MethodInfo baseCategoryMethod = typeof(Mod).GetMethod("SettingsCategory", BindingFlags.Public | BindingFlags.Instance);
            typeBuilder.DefineMethodOverride(catMethod, baseCategoryMethod);

            // Emit Mod Type
            Type emittedType = typeBuilder.CreateType();

            // Set settingsMenuDefField field value
            FieldInfo settingsMenuDefFieldInfo = emittedType.GetField("settingsMenuDef", BindingFlags.Public | BindingFlags.Static);
            settingsMenuDefFieldInfo.SetValue(null, settingsMenuDef);

            return emittedType;
        }
    }
}