
USE [www_elco-holding_com_cn]

--打开全文索引支持，启动SQL Server的全文搜索服务
execute sp_fulltext_database 'enable'

--建立全文检索目录ft_test
execute sp_fulltext_catalog 'FT_Article', 'create'

--为Title列建立全文索引数据元，pk_title为Book表中由主键所建立的唯一索引，这个参数是必需的。
execute sp_fulltext_table 'Articles','create', 'FT_Article','PK_Articles_1'

--设置全文索引列名
execute sp_fulltext_column 'Articles', 'Title', 'add'


--建立全文索引
execute sp_fulltext_table 'Articles', 'activate'

--填充全文索引目录
execute sp_fulltext_catalog 'FT_Article', 'start_full' 