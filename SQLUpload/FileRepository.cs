using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace SQLUpload
{
    public class FileData
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
    }

    public class FileRepository
    {
        private readonly string _connectionString;

        public FileRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddFile(string fileName, Dictionary<string, string> metadata, byte[] file)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"declare @id hierarchyid

select @id = MAX(DocumentNode)
from Production.Document
where DocumentNode.GetAncestor(1) = hierarchyid::GetRoot()

insert into Production.Document(
	DocumentNode, 
	Title, 
	Owner, 
	FolderFlag, 
	FileName, 
	FileExtension, 
	Revision, 
	ChangeNumber, 
	Status, 
	ModifiedDate,
    Document) 
values(
	hierarchyid::GetRoot().GetDescendant(@id, null), 
	@filename, 
	217, 
	0,
	@filename,
	@extention,
	1, 
	0,
	2,
	GetDate(),
    @document)";

                connection.Execute(sql, new { document = file, filename = fileName, extention = new FileInfo(fileName).Extension });
            }
        }

        public FileData GetFile(string name, string nodeId, string guid)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var sql = @"select Document as [File], FileName, FileExtension as Extension from Production.Document
where 
	(DocumentNode = @documentNode or @documentNode is null)
and (FileName = @fileName or @fileName is null)
and (rowguid = @guid or @guid is null)";

                return connection.QueryFirstOrDefault<FileData>(sql, new { documentNode = nodeId, fileName = name, guid });
            }
        }
    }
}
