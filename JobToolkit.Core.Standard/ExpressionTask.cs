using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    [Serializable]
    public class ExpressionTask : JobTask, ISerializable
    {
        public Expression<Action> Expression { get; private set;}
        public ExpressionTask(Expression<Action> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            this.Expression = expression;
        }
        
        public override void Execute()
        {
            Expression.Compile().Invoke();
        }

        public static implicit operator Expression<Action>(ExpressionTask task)
        {
            return task.Expression;
        }
        
        public static implicit operator ExpressionTask(Expression<Action> exp)
        {
            return new ExpressionTask(exp);
        }

        // The special constructor is used to deserialize values.
        protected ExpressionTask(SerializationInfo info, StreamingContext context)
        {
            ////myProperty_value = (string)info.GetValue("props", typeof(string));
            var data = info.GetString("Expression");

            var serializer = new ExpressionSerializer(new JsonSerializer());
            //var bytes = serializer.SerializeBinary(Expression);
            this.Expression = (Expression<Action>)serializer.DeserializeText(data);
        }


        // Implement this method to serialize data. The method is called on serialization.
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            var data = serializer.SerializeText(Expression);
            //var predicateDeserialized = serializer.DeserializeBinary(bytes);
            info.AddValue("Expression", data, typeof(string));

            //info.AddValue("Text", "_Text");
            //    if (info == null)
            //        throw new ArgumentNullException("info");

            //    info.AddValue("Text", _Text);
            //    info.AddValue("props", myProperty_value, typeof(string));

            //    var callExpression = Expression.Body as MethodCallExpression;
            //    if (callExpression == null)
            //    {
            //        throw new ArgumentException("Expression body should be of type `MethodCallExpression`", "methodCall");
            //    }

            //    Type type;

            //    if (callExpression.Object != null)
            //    {
            //        var objectValue = GetExpressionValue(callExpression.Object);
            //        if (objectValue == null)
            //        {
            //            throw new InvalidOperationException("Expression object should be not null.");
            //        }

            //        type = objectValue.GetType();
            //    }
            //    else
            //    {
            //        type = callExpression.Method.DeclaringType;
            //    }

            //    return new Job(
            //        // ReSharper disable once AssignNullToNotNullAttribute
            //        type,
            //        callExpression.Method,
            //        GetExpressionValues(callExpression.Arguments));
            //
        }
    }
}
