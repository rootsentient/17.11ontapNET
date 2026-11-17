using System;
using System.Data;
using System.Data.SqlClient;

namespace SalesApp.DAL
{
    /// <summary>Tiện ích truy cập CSDL. MỌI câu lệnh đều dùng SqlParameter (chống SQL Injection).</summary>
    public static class DbHelper
    {
        // Đổi Server cho phù hợp máy bạn: ".", ".\SQLEXPRESS", "(localdb)\MSSQLLocalDB"...
        public static string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=SalesDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);

        public static DataTable GetTable(string sql, params SqlParameter[] ps)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] ps)
        {
            try
            {
                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (ps != null) cmd.Parameters.AddRange(ps);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new Exception("Dữ liệu bị trùng (mã hoặc số điện thoại đã tồn tại).");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new Exception("Vi phạm ràng buộc: dữ liệu đang được hóa đơn sử dụng hoặc giá trị không hợp lệ.");
            }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] ps)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static object Nz(string s) => string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : s.Trim();
    }
}
