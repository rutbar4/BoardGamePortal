namespace Portal.Utils
{
    public static class GuidUtils
    {
        public static string GenerateGUID(int header = -1)
        {
            return string.Format("{0}_{1:N}", header, Guid.NewGuid());
        }
    }
}
