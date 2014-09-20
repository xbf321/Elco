using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elco.Common
{
    public class FastPaging
    {
        #region = property =

        #region = PageSize =

        private int _pagesize = 50;

        public int PageSize
        {
            get { return _pagesize; }
            set { _pagesize = value; }
        }

        #endregion

        #region = PageIndex =

        private int _pageindex = 1;

        public int PageIndex
        {
            get { return _pageindex; }
            set { _pageindex = value; }
        }

        #endregion

        #region = RecordCount =

        private int _recordcount;

        public int RecordCount
        {
            get
            {
                if (_recordcount < 1)
                {
                    throw new Exception("RecordCount必须为一个大于0的数字");
                }
                return _recordcount;
            }
            set { _recordcount = value; }
        }

        #endregion

        #region = TableName =

        private string _tablename;

        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(_tablename))
                {
                    throw new Exception("TableName不能为空");
                }
                return _tablename;
            }
            set { _tablename = value; }
        }

        #endregion

        #region = PrimaryKey =

        private string _primarykey;

        public string PrimaryKey
        {
            get
            {
                if (string.IsNullOrEmpty(_primarykey))
                {
                    throw new Exception("PrimaryKey不能为空");
                }
                return _primarykey;
            }
            set { _primarykey = value; }
        }

        #endregion

        #region = QueryFields =

        private string _queryfields;
        public string QueryFields
        {
            get
            {
                if (string.IsNullOrEmpty(_queryfields))
                {
                    throw new Exception("QueryFields不能为空");
                }
                return _queryfields;
            }
            set { _queryfields = value; }
        }

        #endregion

        #region = TableReName =

        private string _tablerename;

        public string TableReName
        {
            get
            {
                if (string.IsNullOrEmpty(_tablerename))
                {
                    _tablerename = "t1";
                }
                return _tablerename;
            }
            set { _tablerename = value; }
        }

        #endregion

        #region = OrderByTableName =

        private string _orderbytablename;

        public string OrderByTableName
        {
            get { return _orderbytablename; }
            set { _orderbytablename = value; }
        }

        #endregion

        #region = OverOrderBy =

        private string _overorderby;

        public string OverOrderBy
        {
            get { return _overorderby; }
            set { _overorderby = value; }
        }

        #endregion

        #region = JoinSQL =

        private string _joinsql;

        public string JoinSQL
        {
            get { return _joinsql; }
            set { _joinsql = value; }
        }

        #endregion

        #region = Condition =

        private string _condition;

        public string Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        #endregion

        #region = GroupBy =

        private string _groupby;

        public string GroupBy
        {
            get { return _groupby; }
            set { _groupby = value; }
        }

        #endregion

        #region = Having =

        private string _having;

        public string Having
        {
            get { return _having; }
            set { _having = value; }
        }

        #endregion

        #region = Ascending =

        private bool _ascending;

        public bool Ascending
        {
            get { return _ascending; }
            set { _ascending = value; }
        }

        #endregion

        #region = WithOptions =

        private string _withoptions;

        public string WithOptions
        {
            get { return _withoptions; }
            set { _withoptions = value; }
        }

        #endregion

        #endregion

        public string BuildCountSQL()
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.AppendFormat(" SELECT COUNT(*) FROM {0} AS {1} {2}", this.TableName, this.TableReName, WithOptions);
            if (string.IsNullOrEmpty(this.JoinSQL) == false)
            {
                sbSQL.AppendFormat(" {0}", this.JoinSQL);
            }
            sbSQL.Append(" WHERE 1=1");
            if (string.IsNullOrEmpty(this.Condition) == false)
            {
                sbSQL.AppendFormat(" AND {0}", this.Condition);
            }
            return sbSQL.ToString();
        }
        public string Build2005()
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendFormat(" SELECT TOP {0} *", this.PageSize);
            strSQL.Append(" FROM (");
            if (string.IsNullOrEmpty(OverOrderBy))
            {
                OverOrderBy = string.Format("{2}.{0} {1}", this.PrimaryKey, GetSortType(this.Ascending), string.IsNullOrEmpty(OrderByTableName) ? this.TableReName : OrderByTableName);
            }
            strSQL.AppendFormat(" SELECT ROW_NUMBER() OVER (ORDER BY {0}) AS [ROW_NUMBER],{1}", OverOrderBy, this.QueryFields);
            strSQL.AppendFormat(" FROM {0} AS {1} {2}", this.TableName, this.TableReName, WithOptions);
            if (string.IsNullOrEmpty(this.JoinSQL) == false)
            {
                strSQL.AppendFormat(" {0}", this.JoinSQL);
            }
            strSQL.Append(" WHERE 1=1");
            if (string.IsNullOrEmpty(this.Condition) == false)
            {
                strSQL.AppendFormat(" AND {0}", this.Condition);
            }
            if (string.IsNullOrEmpty(GroupBy) == false)
            {

                strSQL.AppendFormat(" {0}", GroupBy);
            }
            if (string.IsNullOrEmpty(Having) == false)
            {
                strSQL.AppendFormat(" {0}", Having);
            }
            strSQL.Append(" ) AS TABLE1");
            if (this.PageSize * (this.PageIndex - 1) > 0)
            {
                strSQL.AppendFormat(" WHERE TABLE1.[ROW_NUMBER] > {0}", this.PageSize * (this.PageIndex - 1));
            }
            //strSQL.AppendFormat(" ORDER BY TABLE1.{0} {1}", this.PrimaryKey, GetSortType(this.Ascending));

            return strSQL.ToString();
        }

        #region = 静态方法 =

        #region =重载=

        public static String Paging(int pageSize, int pageIndex, int recordCount, String tableName, String queryFields, String primaryKey, bool ascending, String condition)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, null, null, queryFields, primaryKey, ascending, condition);
        }
        public static String Paging(int pageSize, int pageIndex, int recordCount, String tableName, String JoinSQL, String queryFields, String primaryKey, bool ascending, String condition)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, null, JoinSQL, queryFields, primaryKey, ascending, condition);
        }
        public static String Paging(int pageSize, int pageIndex, int recordCount, String tableName, String ReName, String queryFields, String primaryKey, String condition)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, ReName, null, queryFields, null, primaryKey, IsAscending("DESC"), condition);
        }
        public static String Paging(int pageSize, int pageIndex, int recordCount, String tableName, String ReName, String JoinSQL, String queryFields, String primaryKey, String condition)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, ReName, JoinSQL, queryFields, null, primaryKey, IsAscending("DESC"), condition);
        }
        public static String Paging(int pageSize, int pageIndex, int recordCount, String tableName, String ReName, String JoinSQL, String queryFields, String primaryKey, bool ascending, String condition)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, ReName, JoinSQL, queryFields, null, primaryKey, ascending, condition);
        }
        public static String Paging(int pageSize, int pageIndex, int recordCount, String tableName, String ReName, String JoinSQL, String queryFields, String GroupBy, String primaryKey, String condition)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, ReName, JoinSQL, queryFields, GroupBy, primaryKey, IsAscending("DESC"), condition);
        }
        public static String Paging(int pageSize, int pageIndex, int recordCount, String tableName, String ReName, String JoinSQL, String queryFields, String GroupBy, String primaryKey, bool ascending)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, ReName, JoinSQL, queryFields, GroupBy, null, primaryKey, ascending, null);
        }
        public static String Paging(int pageSize, int pageIndex, int recordCount, String tableName, String ReName, String JoinSQL, String queryFields, String GroupBy, String primaryKey, bool ascending, String condition)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, ReName, JoinSQL, queryFields, GroupBy, null, primaryKey, ascending, condition);
        }

        #endregion
        /// <summary>
        /// 获取根据指定字段排序并分页查询的 SELECT 语句。
        /// </summary>
        /// <param name="pageSize">每页要显示的记录的数目。</param>
        /// <param name="pageIndex">要显示的页的索引。</param>
        /// <param name="recordCount">数据表中的记录总数。</param>
        /// <param name="tableName">要查询的数据表。</param>
        /// <param name="ReName">主表别名</param>
        /// <param name="JoinSQL">联接SQL</param>
        /// <param name="queryFields">要查询的字段。</param>
        /// <param name="GroupBy">分组排序SQL</param>
        /// <param name="primaryKey">主键字段。</param>
        /// <param name="ascending">是否为升序排列。</param>
        /// <param name="condition">查询的筛选条件。</param>
        /// <returns>返回排序并分页查询的 SELECT 语句。</returns>
        public static String Paging(
            int pageSize,
            int pageIndex,
            int recordCount,
            String tableName,
            String ReName,
            String JoinSQL,
            String queryFields,
            String GroupBy,
            String Having,
            String primaryKey,
            bool ascending,
            String condition)
        {
            #region 实现

            StringBuilder sb = new StringBuilder();
            //计算分页数

            int pageCount = GetPageCount(recordCount, pageSize);   //分页的总数
            //计算中间页的索引
            int middleIndex = GetMidPageIndex(pageCount);           //中间页的索引
            //第一页的索引
            int firstIndex = 1;
            //最后一页的索引                  
            int lastIndex = pageCount;
            //简化字段
            //StringBuilder TempSimpleFields = new StringBuilder();
            //Regex reg = new Regex("[^)|^(]+?, ");
            //MatchCollection mc = reg.Matches((queryFields + ",").Replace(",", ", "));
            //foreach (Match m in mc)
            //{
            //    TempSimpleFields.Append(Regex.Replace(m.Groups[0].Value, ".+AS ", string.Empty));
            //}
            //string SimpleFields = Regex.Replace(TempSimpleFields.ToString(), ", *$", string.Empty);

            #region @PageIndex <= @FirstIndex
            if (pageIndex <= firstIndex)
            {
                sb.Append("SELECT TOP ").Append(pageSize).Append(" ").Append(queryFields)
                    .Append(" FROM ").Append(tableName);
                if (string.IsNullOrEmpty(ReName) == false)
                {
                    sb.Append(" AS ").Append(ReName);
                }
                if (string.IsNullOrEmpty(JoinSQL) == false)
                {
                    sb.Append(" ").Append(JoinSQL);
                }
                if (condition != String.Empty)
                    sb.Append(" WHERE ").Append(condition);
                if (string.IsNullOrEmpty(GroupBy) == false)
                {
                    sb.Append(" GROUP BY ");
                    sb.Append(GroupBy);
                }
                if (string.IsNullOrEmpty(Having) == false)
                {
                    sb.Append(" HAVING ");
                    sb.Append(Having);
                }
                sb.Append(" ORDER BY ").Append(string.IsNullOrEmpty(ReName) ? tableName : ReName).Append(".").Append(primaryKey).Append(" ")
                    .Append(GetSortType(ascending));
            }
            #endregion

            #region @FirstIndex < @PageIndex < @LastIndex
            else if (pageIndex > firstIndex && pageIndex < lastIndex)
            {
                //sb.Append(" SELECT TOP ").Append(pageSize).Append(" ").Append(SimpleFields.Replace(string.IsNullOrEmpty(ReName) ? tableName : ReName + ".", "TableB.")).Append(" FROM ");
                sb.Append(" SELECT TOP ").Append(pageSize).Append(" *").Append(" FROM ");
                sb.Append(" (");
                //sb.Append(" SELECT TOP ").Append(pageSize).Append(" ").Append(SimpleFields.Replace(string.IsNullOrEmpty(ReName) ? tableName : ReName + ".", "TableA.")).Append(" FROM");
                sb.Append(" SELECT TOP ").Append(pageSize).Append(" *").Append(" FROM");
                sb.Append(" (");
                sb.Append(" SELECT TOP ").Append(pageSize * pageIndex).Append(" ").Append(queryFields);
                sb.Append(" FROM ").Append(tableName);
                if (string.IsNullOrEmpty(ReName) == false)
                {
                    sb.Append(" AS ").Append(ReName);
                }
                if (string.IsNullOrEmpty(JoinSQL) == false)
                {
                    sb.Append(" ").Append(JoinSQL);
                }
                if (condition != String.Empty)
                    sb.Append(" WHERE ").Append(condition);
                if (string.IsNullOrEmpty(GroupBy) == false)
                {
                    sb.Append(" GROUP BY ");
                    sb.Append(GroupBy);
                }
                if (string.IsNullOrEmpty(Having) == false)
                {
                    sb.Append(" HAVING ");
                    sb.Append(Having);
                }
                sb.Append(" ORDER BY ").Append(string.IsNullOrEmpty(ReName) ? tableName : ReName).Append(".").Append(primaryKey).Append(" ").Append(GetSortType(ascending));
                sb.Append(" ) as TableA");
                sb.Append(" ORDER BY TableA.").Append(primaryKey).Append(" ").Append(GetSortType(!ascending));
                sb.Append(" ) as TableB");
                sb.Append(" ORDER BY TableB.").Append(primaryKey).Append(" ").Append(GetSortType(ascending));
            }
            #endregion

            #region @PageIndex >= @LastIndex
            else if (pageIndex >= lastIndex)
            {
                //sb.Append("SELECT TOP ").Append(pageSize).Append(" ").Append(SimpleFields.Replace(string.IsNullOrEmpty(ReName) ? tableName : ReName + ".", "TableA.")).Append(" FROM ( SELECT TOP ").Append(recordCount - pageSize * (lastIndex - 1))
                sb.Append("SELECT TOP ").Append(pageSize).Append(" *").Append(" FROM ( SELECT TOP ").Append(recordCount - pageSize * (lastIndex - 1))
                    .Append(" ").Append(queryFields)
                    .Append(" FROM ").Append(tableName);
                if (string.IsNullOrEmpty(ReName) == false)
                {
                    sb.Append(" AS ").Append(ReName);
                }
                if (string.IsNullOrEmpty(JoinSQL) == false)
                {
                    sb.Append(" ").Append(JoinSQL);
                }
                if (condition != String.Empty)
                    sb.Append(" WHERE ").Append(condition);
                if (string.IsNullOrEmpty(GroupBy) == false)
                {
                    sb.Append(" GROUP BY ");
                    sb.Append(GroupBy);
                }
                if (string.IsNullOrEmpty(Having) == false)
                {
                    sb.Append(" HAVING ");
                    sb.Append(Having);
                }
                sb.Append(" ORDER BY ").Append(string.IsNullOrEmpty(ReName) ? tableName : ReName).Append(".").Append(primaryKey).Append(" ")
                    .Append(GetSortType(!ascending))
                    .Append(" ) TableA ORDER BY TableA.").Append(primaryKey).Append(" ")
                    .Append(GetSortType(ascending));
            }
            #endregion

            return sb.ToString();
            #endregion
        }

        /// <summary>
        /// 获取根据指定字段排序并分页查询的 SELECT 语句。
        /// </summary>
        /// <param name="pageSize">每页要显示的记录的数目。</param>
        /// <param name="pageIndex">要显示的页的索引。</param>
        /// <param name="recordCount">数据表中的记录总数。</param>
        /// <param name="tableName">要查询的数据表。</param>
        /// <param name="queryFields">要查询的字段。</param>
        /// <param name="primaryKey">主键字段。</param>
        public static String Paging(
            int pageSize,
            int pageIndex,
            int recordCount,
            String tableName,
            String queryFields,
            String primaryKey)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, queryFields, primaryKey,
                true, String.Empty);
        }

        /// <summary>
        /// 获取根据指定字段排序并分页查询的 SELECT 语句。
        /// </summary>
        /// <param name="pageSize">每页要显示的记录的数目。</param>
        /// <param name="pageIndex">要显示的页的索引。</param>
        /// <param name="recordCount">数据表中的记录总数。</param>
        /// <param name="tableName">要查询的数据表。</param>
        /// <param name="queryFields">要查询的字段。</param>
        /// <param name="primaryKey">主键字段。</param>
        /// <param name="ascending">是否为升序排列。</param>
        /// <returns>返回排序并分页查询的 SELECT 语句。</returns>
        public static String Paging(
            int pageSize,
            int pageIndex,
            int recordCount,
            String tableName,
            String queryFields,
            String primaryKey,
            bool ascending)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, queryFields, primaryKey,
                ascending, String.Empty);
        }

        /// <summary>
        /// 获取根据指定字段排序并分页查询的 SELECT 语句。
        /// </summary>
        /// <param name="pageSize">每页要显示的记录的数目。</param>
        /// <param name="pageIndex">要显示的页的索引。</param>
        /// <param name="recordCount">数据表中的记录总数。</param>
        /// <param name="tableName">要查询的数据表。</param>
        /// <param name="queryFields">要查询的字段。</param>
        /// <param name="primaryKey">主键字段。</param>
        /// <param name="condition">查询的筛选条件。</param>
        /// <returns>返回排序并分页查询的 SELECT 语句。</returns>
        public static String Paging(
            int pageSize,
            int pageIndex,
            int recordCount,
            String tableName,
            String queryFields,
            String primaryKey,
            String condition)
        {
            return Paging(pageSize, pageIndex, recordCount, tableName, queryFields, primaryKey,
                true, condition);
        }

        /// <summary>
        /// 计算分页数。
        /// </summary>
        /// <param name="recordCount">表中得记录总数。</param>
        /// <param name="pageSize">每页显示的记录数。</param>
        /// <returns>分页数。</returns>
        public static int GetPageCount(int recordCount, int pageSize)
        {
            return (int)Math.Ceiling((double)recordCount / pageSize);
        }

        /// <summary>
        /// 计算中间页的页索引。
        /// </summary>
        /// <param name="pageCount">分页数。</param>
        /// <returns>中间页的页索引。</returns>
        public static int GetMidPageIndex(int pageCount)
        {
            return (int)Math.Ceiling((double)pageCount / 2) - 1;
        }

        /// <summary>
        /// 获取排序的方式（"ASC" 表示升序，"DESC" 表示降序）。
        /// </summary>
        /// <param name="ascending">是否为升序。</param>
        /// <returns>排序的方式（"ASC" 表示升序，"DESC" 表示降序）。</returns>
        public static String GetSortType(bool ascending)
        {
            return (ascending ? "ASC" : "DESC");
        }

        /// <summary>
        /// 获取一个布尔值，该值指示排序的方式是否为升序。
        /// </summary>
        /// <param name="orderType">排序的方式（"ASC" 表示升序，"DESC" 表示降序）。</param>
        /// <returns>"ASC"则为 true；"DESC"则为 false；其它的为 true。</returns>
        public static bool IsAscending(String orderType)
        {
            return ((orderType.ToUpper() == "DESC") ? false : true);
        }

        #endregion
    }
}
