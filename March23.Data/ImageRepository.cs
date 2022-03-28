using System.Data.SqlClient;

namespace March23.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Upload(Image image)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Images(FileName, Password)
                                    VALUES(@name, @password)
                                    SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@name", image.FileName);
            command.Parameters.AddWithValue("@password", image.Password);
            connection.Open();
            return (int)(decimal)command.ExecuteScalar();
        }
        public Image GetImageById(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Images WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new Image
            {
                Id = id,
                FileName = (string)reader["FileName"],
                Password = (string)reader["Password"],
                Views = (int)reader["NumberofViews"]
            };
        }
        public void UpdateCount(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Images SET NumberofViews = NumberofViews + 1 WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

}
