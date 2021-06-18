using System;

namespace DwFramework.Core
{
    public sealed class TypeNotMatchException : ExceptionBase
    {
        public readonly Type SourceType;
        public readonly Type TargetType;

        public TypeNotMatchException(Type sourceType, Type targetType) : base((StatusCode)40002, $"{sourceType}与{targetType}类型不匹配")
        {
            SourceType = sourceType;
            TargetType = targetType;
        }
    }
}