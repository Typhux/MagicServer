using Magic.Models;
using System;
using System.Reflection;

namespace Magic.Engine
{
    public class FightEngine
    {
        public FightEngine() { }

        public Settings CardMechanic(Settings settings, ResponseCard creature, Character character)
        {
            Type thisType = Type.GetType("Magic.Library." + creature.EditionName);
            ConstructorInfo constructorInfo = thisType.GetConstructor(Type.EmptyTypes);
            object instance = constructorInfo.Invoke(new object[] { });
            MethodInfo theMethod = thisType.GetMethod(creature.CodeName, BindingFlags.NonPublic | BindingFlags.Instance);
            var parameters = new object[3];
            parameters[0] = settings;
            parameters[1] = creature;
            parameters[2] = character;
            if (theMethod != null)
            {
                settings = (Settings)theMethod.Invoke(instance, parameters);
            }
            else
            {
                settings.Logs.Add("This mechanic is not yet implemented.");
            }

            return settings;
        }
    }
}
