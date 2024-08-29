public static class DBHelper
{
    public static string GetConnectionString()
    {
        string server = "FX-NB-001\\MSSQLSERVER02";
        string database = "EjercicioAPI";
        return $"Data Source={server};Initial Catalog={database};Integrated Security=True;TrustServerCertificate=True";
    }
}