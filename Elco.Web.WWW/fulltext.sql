
USE [www_elco-holding_com_cn]

--��ȫ������֧�֣�����SQL Server��ȫ����������
execute sp_fulltext_database 'enable'

--����ȫ�ļ���Ŀ¼ft_test
execute sp_fulltext_catalog 'FT_Article', 'create'

--ΪTitle�н���ȫ����������Ԫ��pk_titleΪBook������������������Ψһ��������������Ǳ���ġ�
execute sp_fulltext_table 'Articles','create', 'FT_Article','PK_Articles_1'

--����ȫ����������
execute sp_fulltext_column 'Articles', 'Title', 'add'


--����ȫ������
execute sp_fulltext_table 'Articles', 'activate'

--���ȫ������Ŀ¼
execute sp_fulltext_catalog 'FT_Article', 'start_full' 