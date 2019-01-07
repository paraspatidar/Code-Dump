//lets create and extension method wiht class
public static class Extensions
    {
        public static string DeepExceptiopn(this Exception ex)
        {
            string result = string.Empty;
            result = " Exception : LEVEL 1: " + Environment.NewLine + ex.Message+ "::::Trace===>"+ex.StackTrace;
            if (ex.InnerException != null)
                {
                    result = result + Environment.NewLine + "LEVEL 2:" + Environment.NewLine + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        result = result + Environment.NewLine + "LEVEL 3:" + Environment.NewLine + ex.InnerException.InnerException.Message;

                        if (ex.InnerException.InnerException.InnerException != null)
                        {
                            result = result + Environment.NewLine + "LEVEL 4:" + Environment.NewLine + ex.InnerException.InnerException.InnerException.Message;
                            if (ex.InnerException.InnerException.InnerException.InnerException != null)
                            {
                                result = result + Environment.NewLine + "LEVEL 5:" + Environment.NewLine + ex.InnerException.InnerException.InnerException.InnerException.Message;
                            }
                        }
                    }
                }
            return result;
        }
    }
