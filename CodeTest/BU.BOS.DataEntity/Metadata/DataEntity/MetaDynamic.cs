using BU.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BU.BOS.Orm.Metadata.DataEntity
{
    internal sealed class MetaDynamic : System.Dynamic.DynamicMetaObject
    {
        #region Fields
        private static readonly Expression[] NoArgs = new Expression[0];
        private static readonly MethodInfo TryGetMember;
        private static readonly MethodInfo TrySetMember;
        #endregion

        #region Properties
        // Properties
        private BU.BOS.Orm.DataEntity.DynamicObject OrmDynamicObject
        {
            get
            {
                return (BU.BOS.Orm.DataEntity.DynamicObject)base.Value;
            }
        }

        // Nested Types
        private delegate DynamicMetaObject Fallback(DynamicMetaObject errorSuggestion);

        #endregion
        #region Methods
        static MetaDynamic()
        {
            Type type = typeof(BU.BOS.Orm.DataEntity.DynamicObject);
            TryGetMember = type.GetMethod("TryGetMember", BindingFlags.NonPublic | BindingFlags.Instance);
            TrySetMember = type.GetMethod("TrySetMember", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        internal MetaDynamic(Expression expression, BU.BOS.Orm.DataEntity.DynamicObject value)
            : base(expression, BindingRestrictions.Empty, value)
        {
        }

        internal static T[] AddFirst<T>(IList<T> list, T item)
        {
            T[] array = new T[list.Count + 1];
            array[0] = item;
            list.CopyTo(array, 1);
            return array;
        }

        internal static bool AreEquivalent(Type t1, Type t2)
        {
            if (!(t1 == t2))
            {
                return t1.IsEquivalentTo(t2);
            }
            return true;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            Fallback fallback = e => binder.FallbackGetMember(this, e);
            return this.CallMethodWithResult(TryGetMember, binder, NoArgs, fallback);
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            Fallback fallback = e => binder.FallbackSetMember(this, value, e);
            return this.CallMethodReturnLast(TrySetMember, binder, GetArgs(new DynamicMetaObject[] { value }), fallback);
        }

        private DynamicMetaObject CallMethodReturnLast(MethodInfo method, DynamicMetaObjectBinder binder, Expression[] args, Fallback fallback)
        {
            DynamicMetaObject obj2 = fallback(null);
            ParameterExpression left = Expression.Parameter(typeof(object), null);
            Expression[] arguments = AddFirst<Expression>(args, Constant(binder));
            arguments[args.Length] = Expression.Assign(left, arguments[args.Length]);
            DynamicMetaObject errorSuggestion = new DynamicMetaObject(Expression.Block(new ParameterExpression[] { left }, new Expression[] { Expression.Condition(Expression.Call(this.GetLimitedSelf(), method, arguments), left, obj2.Expression, typeof(object)) }), this.GetRestrictions().Merge(obj2.Restrictions));
            return fallback(errorSuggestion);
        }

        private DynamicMetaObject CallMethodWithResult(MethodInfo methodName, DynamicMetaObjectBinder binder, Expression[] args, Fallback fallback)
        {
            return this.CallMethodWithResult(methodName, binder, args, fallback, null);
        }

        private DynamicMetaObject CallMethodWithResult(MethodInfo method, DynamicMetaObjectBinder binder, Expression[] args, Fallback fallback, Fallback fallbackInvoke)
        {
            DynamicMetaObject obj2 = fallback(null);
            ParameterExpression expression = Expression.Parameter(typeof(object), null);
            Expression[] destinationArray = new Expression[args.Length + 2];
            Array.Copy(args, 0, destinationArray, 1, args.Length);
            destinationArray[0] = Constant(binder);
            destinationArray[destinationArray.Length - 1] = expression;
            DynamicMetaObject errorSuggestion = new DynamicMetaObject(expression, BindingRestrictions.Empty);
            if (binder.ReturnType != typeof(object))
            {
                errorSuggestion = new DynamicMetaObject(Expression.Convert(errorSuggestion.Expression, binder.ReturnType), errorSuggestion.Restrictions);
            }
            if (fallbackInvoke != null)
            {
                errorSuggestion = fallbackInvoke(errorSuggestion);
            }
            DynamicMetaObject obj4 = new DynamicMetaObject(Expression.Block(new ParameterExpression[] { expression }, new Expression[] { Expression.Condition(Expression.Call(this.GetLimitedSelf(), method, destinationArray), errorSuggestion.Expression, obj2.Expression, binder.ReturnType) }), this.GetRestrictions().Merge(errorSuggestion.Restrictions).Merge(obj2.Restrictions));
            return fallback(obj4);
        }

        private static ConstantExpression Constant(DynamicMetaObjectBinder binder)
        {
            Type baseType = binder.GetType();
            while (!baseType.IsVisible)
            {
                baseType = baseType.BaseType;
            }
            return Expression.Constant(binder, baseType);
        }

        private static Expression[] GetArgs(params DynamicMetaObject[] args)
        {
            Expression[] expressions = GetExpressions(args);
            for (int i = 0; i < expressions.Length; i++)
            {
                expressions[i] = Expression.Convert(args[i].Expression, typeof(object));
            }
            return expressions;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            DynamicPropertyCollection properties = this.OrmDynamicObject.DynamicObjectType.Properties;
            string[] strArray = new string[properties.Count];
            for (int i = 0; i < properties.Count; i++)
            {
                strArray[i] = properties[i].Name;
            }
            return strArray;
        }

        private static Expression[] GetExpressions(DynamicMetaObject[] objects)
        {
            Expression[] expressionArray = new Expression[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                DynamicMetaObject obj2 = objects[i];
                expressionArray[i] = obj2.Expression;
            }
            return expressionArray;
        }

        private Expression GetLimitedSelf()
        {
            if (AreEquivalent(base.Expression.Type, base.LimitType))
            {
                return base.Expression;
            }
            return Expression.Convert(base.Expression, base.LimitType);
        }

        private BindingRestrictions GetRestrictions()
        {
            if ((base.Value == null) && base.HasValue)
            {
                return BindingRestrictions.GetInstanceRestriction(base.Expression, null);
            }
            return BindingRestrictions.GetTypeRestriction(base.Expression, base.LimitType);
        }


        #endregion
    }
}
