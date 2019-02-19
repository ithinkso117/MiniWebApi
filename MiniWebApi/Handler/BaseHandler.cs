using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using MiniWebApi.Network;

namespace MiniWebApi.Handler
{
    public class BaseHandler
    {
        private readonly Dictionary<string, CallingMethod> _callingMethods = new Dictionary<string, CallingMethod>();

        public BaseHandler()
        {
            RegisterCallingMethods();
        }

        private void RegisterCallingMethods()
        {
            //Load all methods.
            var methods = GetType().GetMethods();
            foreach (var method in methods.Where(x => x.IsVirtual == false && x.IsStatic == false && x.ReturnType == typeof(void)))
            {
                //Get if the method support WebApiAttribute
                var methodWebApiAttrs = method.GetCustomAttributes(typeof(WebApiMethodAttribute), true);
                if (methodWebApiAttrs.Length == 0)
                {
                    //If no WebApiAttribute, do not add it into the method dictionary.
                    continue;
                }

                if (methodWebApiAttrs.Length >1)
                {
                    // Can not define more than 2 attributes
                    throw new InvalidOperationException($"Method {method.Name} defined more than one WebApi method attributes.");
                }

                if (!method.IsPublic)
                {
                    //Web api method should be public
                    throw new InvalidOperationException($"Method {method.Name} should be public for WebApi method.");
                }

                var methodWebApiType = ((WebApiMethodAttribute) methodWebApiAttrs[0]).ToWebApiType();

                //Get all parameters
                var parameters = method.GetParameters();

                //If the first parameter is not WebApiHttpContext throw the exception
                if (parameters[0].ParameterType != typeof(WebApiHttpContext))
                {
                    throw new InvalidDataException($"The first argument of method {method} must be WebApiHttpContext");
                }

                //Generate CallingParameters
                var paramInfos = new List<CallingParameter>();
                foreach (var parameterInfo in parameters)
                {
                    if (parameterInfo.ParameterType == typeof(WebApiHttpContext))
                    {
                        continue;
                    }
                    var fromType = FromType.None;
                    //Get if the parameter support WebApiAttribute
                    var paramWebApiAttrs = parameterInfo.GetCustomAttributes(typeof(WebApiParameterAttribute), true);
                    if (paramWebApiAttrs.Length > 0)
                    {
                        if (paramWebApiAttrs[0] is FromUrlAttribute)
                        {
                            fromType = FromType.FromUrl;
                        }
                        else if (paramWebApiAttrs[0] is FromBodyAttribute)
                        {
                            fromType = FromType.FromBody;
                        }
                    }
                    paramInfos.Add(new CallingParameter(parameterInfo.Name, parameterInfo.ParameterType, fromType));
                }

                //More than one params' FormType is FromUrl
                if (paramInfos.Count(x => x.FromType == FromType.FromUrl) > 1)
                {
                    throw new InvalidOperationException($"Method {method.Name} defined more than one [FromUrl] parameters.");
                }

                //More than one params' FormType is FromBody
                if (paramInfos.Count(x => x.FromType == FromType.FromBody) > 1)
                {
                    throw new InvalidOperationException($"Method {method.Name} defined more than one [FromBody] parameters.");
                }

                //More than one normal params and one of params the FormType is FromUrl
                if (paramInfos.Any(x => x.FromType == FromType.FromUrl) && paramInfos.Any(x=>x.FromType == FromType.None))
                {
                    throw new InvalidOperationException($"Only one [FromUrl] parameter can be define in Method {method.Name} without any other parameters.");
                }

                //Get Method can not contains FromBody
                if (methodWebApiType == WebApiType.Get && paramInfos.Any(x => x.FromType == FromType.FromBody))
                {
                    throw new InvalidOperationException($"Get method {method.Name} can not contains [FromBody] parameter.");
                }

                //Generate the delegate by Emit.
                var dynamicMethod = new DynamicMethod("", null, new[] { typeof(object), typeof(object[]) }, GetType().Module);
                var il = dynamicMethod.GetILGenerator();

                //Put the first arg in stack which is this object..
                il.Emit(OpCodes.Ldarg_S, 0);
                //Cast the object to the real type.
                il.Emit(OpCodes.Castclass, GetType());

                //Put WebApiHttpContext
                il.Emit(OpCodes.Ldarg_S, 1);
                //Put an integer value in stack which is the index in object[]
                il.Emit(OpCodes.Ldc_I4_S, 0);
                //Get the reference of index which is the integer value from the object[].
                il.Emit(OpCodes.Ldelem_Ref);
                //Cast or unbox the reference.
                il.Emit(OpCodes.Castclass, typeof(WebApiHttpContext));

                //Put all args which is object[] in stack.
                for (var i = 0; i < paramInfos.Count; i++)
                {
                    var parameterInfo = paramInfos[i];
                    //Put the arg 1 which is object[] in stack.
                    il.Emit(OpCodes.Ldarg_S, 1);
                    //Put an integer value in stack which is the index in object[]
                    il.Emit(OpCodes.Ldc_I4_S, i+1);
                    //Get the reference of index which is the integer value from the object[].
                    il.Emit(OpCodes.Ldelem_Ref);
                    //Cast or unbox the reference.
                    il.Emit(parameterInfo.ParameterType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, parameterInfo.ParameterType);
                }
                //Call the method.
                il.Emit(OpCodes.Call, method);
                //Exit the method.
                il.Emit(OpCodes.Ret);

                //Create the delegate by dynamic method.
                var action = (Action<object, object[]>)dynamicMethod.CreateDelegate(typeof(Action<object, object[]>));
                var callingMethod = new CallingMethod(method.Name, methodWebApiType, paramInfos, this, new WebApiMethod(action));
                _callingMethods.Add(callingMethod.Name, callingMethod);
            }

        }

        /// <summary>
        /// Get the CallingMethod from the handler by name.
        /// </summary>
        /// <param name="name">The name of the CallingMethod, if is null, will use the parameters to find the method.</param>
        /// <returns>The CallingMethod, null if not exist.</returns>
        public CallingMethod GetCallingMethod(string name)
        {
            return !_callingMethods.ContainsKey(name) ? null : _callingMethods[name];
        }

        /// <summary>
        /// Get all calling method provided by this handler.
        /// </summary>
        /// <returns>All CallingMethods</returns>
        public CallingMethod[] GetCallingMethods()
        {
            return _callingMethods.Values.ToArray();
        }
    }
}
