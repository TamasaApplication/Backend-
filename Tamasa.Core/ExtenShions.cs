using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AhmadBase.Core
{
    public enum FilterConnector
    {
        And,
        Or,
        // Not
    }
    public class TermFilter
    {
        public int PgNumber { get; set; }
        public int PgSize { get; set; }
        public string SearchTerm { get; set; }

    }
    public class SearchFilter : TermFilter
    {
        public Filter Filter { get; set; }

        public FilterConnector Connector { get; set; }

        public List<SearchFilter> Filters { get; set; } = new List<SearchFilter>();

        public void AddFilter(SearchFilter filter)
        {
            filter.Connector = FilterConnector.And;
            Filters.Add(filter);
        }

        public void AddFilter(Filter filter)
        {
            Filters.Add(new SearchFilter { Filter = filter, Connector = FilterConnector.And });
        }

        public void OrFilter(SearchFilter filter)
        {
            filter.Connector = FilterConnector.Or;
            Filters.Add(filter);
        }
        public void OrFilter(Filter filter)
        {
            Filters.Add(new SearchFilter { Filter = filter, Connector = FilterConnector.Or });
        }
        //public void NotFilter(Filter filter)
        //{
        //    Filters.Add(new SearchFilter { Filter = filter, Connector = FilterConnector.Not });
        //}
        //public void NotFilter(SearchFilter filter)
        //{
        //    filter.Connector = FilterConnector.Not;
        //    Filters.Add(filter);
        //}

        public Expression<Func<T, bool>> Translate<T>()
        {
            ParameterExpression param = Expression.Parameter(typeof(T), typeof(T).Name.ToLower());
            var retVal = new SearchFilterTranslator().Translate<T>(this, param);
            if (retVal == null)
                return null;
            return Expression.Lambda<Func<T, bool>>(retVal, param);
        }


        public class SearchFilterTranslator
        {
            public Expression Translate<T>(SearchFilter sf, ParameterExpression param)
            {
                if (sf.Filters == null || sf.Filters.Count <= 0)
                {
                    if (sf.Filter == null)
                        return null;
                    sf.AddFilter(sf.Filter);

                }
                Expression leftExp = null;
                leftExp = sf.Filters[0].Filter == null ? Translate<T>(sf.Filters[0], param) : sf.Filters[0].Filter.Translate<T>(param);

                for (int i = 1; i < sf.Filters.Count; i++)
                {
                    var fltr = sf.Filters[i];
                    Expression rightExp = null;
                    rightExp = fltr.Filter == null ? Translate<T>(fltr, param) : fltr.Filter.Translate<T>(param);
                    if (leftExp == null) // some miss match  type or error happend and we skip the expression
                        leftExp = rightExp;
                    else
                        switch (fltr.Connector)
                        {
                            case FilterConnector.And:
                                if (rightExp == null)
                                    return leftExp;
                                leftExp = Expression.AndAlso(leftExp, rightExp);
                                break;
                            case FilterConnector.Or:
                                if (rightExp == null)
                                    return leftExp;
                                leftExp = Expression.OrElse(leftExp, rightExp);
                                break;
                        }
                }

                return leftExp;
            }
        }

        public Expression<Func<TTo, bool>> Convert<TFrom, TTo>()
        {
            Expression<Func<TFrom, bool>> expr = Translate<TFrom>();
            Dictionary<Expression, Expression> substitutues = new Dictionary<Expression, Expression>();
            var oldParam = expr.Parameters[0];
            var newParam = Expression.Parameter(typeof(TTo), oldParam.Name);
            substitutues.Add(oldParam, newParam);
            Expression body = ConvertNode(expr.Body, substitutues);
            return Expression.Lambda<Func<TTo, bool>>(body, newParam);
        }
        public Expression ConvertNode(Expression node, IDictionary<Expression, Expression> subst)
        {
            if (node == null) return null;
            if (subst.ContainsKey(node)) return subst[node];

            switch (node.NodeType)
            {
                case ExpressionType.Constant:
                    return node;
                case ExpressionType.MemberAccess:
                    {
                        var me = (MemberExpression)node;
                        var newNode = ConvertNode(me.Expression, subst);
                        return Expression.MakeMemberAccess(newNode, newNode.Type.GetMember(me.Member.Name).Single());
                    }
                case ExpressionType.Equal:
                    /* will probably work for a range of common binary-expressions */
                    {
                        var be = (BinaryExpression)node;
                        return Expression.MakeBinary(be.NodeType, ConvertNode(be.Left, subst), ConvertNode(be.Right, subst), be.IsLiftedToNull, be.Method);
                    }
                default:
                    throw new NotSupportedException(node.NodeType.ToString());
            }
        }

        public SearchFilter Rename(SearchFilter sf)
        {
            if (sf.Filters.Count == 0)
            {
                if (sf.Filter != null)
                    sf.AddFilter(sf.Filter);

            }

            for (int i = 0; i < sf.Filters.Count; i++)
            {
                var fltr = sf.Filters[i];
                if (fltr.Filter == null)
                    sf.Filters[i] = Rename(fltr);
                else
                    fltr.Filter.PropertyName = fltr.Filter.PropertyName.Replace(".", "_");

            }

            return sf;
        }

    }
}

