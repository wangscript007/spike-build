﻿// grouping.cs
//
// Copyright 2012 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Spike.Build.Minifiers
{
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of parenthetical '(' expr ')' operators
    /// </summary>
    internal class JsGroupingOperator : JsExpression
    {
        private JsAstNode m_operand;

        public JsAstNode Operand
        {
            get { return m_operand; }
            set
            {
                m_operand.IfNotNull(n => n.Parent = (n.Parent == this) ? null : n.Parent);
                m_operand = value;
                m_operand.IfNotNull(n => n.Parent = this);
            }
        }

        public JsGroupingOperator(JsContext context, JsParser parser)
            : base(context, parser)
        {
        }

        public override void Accept(IJsVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }

        public override JsPrimitiveType FindPrimitiveType()
        {
            return Operand != null
                ? Operand.FindPrimitiveType()
                : JsPrimitiveType.Other;
        }

        public override JsOperatorPrecedence Precedence
        {
            get
            {
                return JsOperatorPrecedence.Primary;
            }
        }

        public override IEnumerable<JsAstNode> Children
        {
            get
            {
                return EnumerateNonNullNodes(Operand);
            }
        }

        public override bool ReplaceChild(JsAstNode oldNode, JsAstNode newNode)
        {
            if (Operand == oldNode)
            {
                Operand = newNode;
                return true;
            }

            return false;
        }

        public override bool IsEquivalentTo(JsAstNode otherNode)
        {
            // we be equivalent if the other node is the
            // equivalent of the operand, right? The only difference would be the
            // parentheses, so maybe it'd still be the equivalent, no?
            var otherGroup = otherNode as JsGroupingOperator;
            return (otherGroup != null && Operand.IsEquivalentTo(otherGroup.Operand))
                || Operand.IsEquivalentTo(otherNode);
        }

        public override bool IsConstant
        {
            get
            {
                return Operand.IfNotNull(o => o.IsConstant);
            }
        }

        public override string ToString()
        {
            return '(' + (Operand == null ? "<null>" : Operand.ToString()) + ')';
        }
    }
}
