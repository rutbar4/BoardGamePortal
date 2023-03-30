namespace Portal.Utils
{
    public static class GuidUtils
    {
        public static string GenerateGUID(string header = "0")
        {
            return string.Format("{0}_{1:N}", header, Guid.NewGuid());
        }
    }
}
