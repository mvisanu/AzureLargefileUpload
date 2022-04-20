using BlobTransferMiddleware.model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobTransferMiddleware
{
    
    public class FilesDatabase
    {
        private  string _connectionString;

        public FilesDatabase(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<FileUploadModel> GetFilesToUpload()
        {
            var listModel = new List<FileUploadModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SP_GET_FilesToUpload", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        listModel.Add(new FileUploadModel
                        {
                            Id = new Guid(rdr[0].ToString()),
                            FileName = rdr[1].ToString(),
                            PhysicalPath = rdr[2].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
               
               Console.WriteLine(ex.Message);
               return new List<FileUploadModel>();
            }
            return listModel;
        }

        public void UpdateFile(Guid id)
        {
            string UpdateCommand = "SP_Update_AzureBlob";
            using (SqlConnection sqlConnectionCmdString = new SqlConnection(_connectionString))
            using (SqlCommand sqlRenameCommand = new SqlCommand(UpdateCommand, sqlConnectionCmdString))
            {
                DateTime td = DateTime.Now;
                sqlRenameCommand.CommandType = CommandType.StoredProcedure;
                sqlRenameCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
                sqlRenameCommand.Parameters.Add("@date", SqlDbType.DateTime).Value = td;
                sqlConnectionCmdString.Open();
                sqlRenameCommand.ExecuteNonQuery();
            }
        }
    }
    
}
