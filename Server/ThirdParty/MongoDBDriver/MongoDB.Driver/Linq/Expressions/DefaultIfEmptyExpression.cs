/* Copyright 2015-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* skipations under the License.
*/

using System;
using System.Linq.Expressions;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Driver.Linq.Expressions
{
    internal sealed class DefaultIfEmptyExpression : ExtensionExpression, ISourcedExpression
    {
        private readonly Expression _source;

        public DefaultIfEmptyExpression(Expression source)
        {
            _source = Ensure.IsNotNull(source, nameof(source));
        }

        public override ExtensionExpressionType ExtensionType
        {
            get { return ExtensionExpressionType.DefaultIfEmpty; }
        }

        public Expression Source
        {
            get { return _source; }
        }

        public override Type Type
        {
            get { return _source.Type; }
        }

        public override string ToString()
        {
            return string.Format("{0}.DefaultIfEmpty()", _source.ToString());
        }

        public DefaultIfEmptyExpression Update(Expression source)
        {
            if (source != _source)
            {
                return new DefaultIfEmptyExpression(source);
            }

            return this;
        }

        protected internal override Expression Accept(ExtensionExpressionVisitor visitor)
        {
            return visitor.VisitDefaultIfEmpty(this);
        }
    }
}
