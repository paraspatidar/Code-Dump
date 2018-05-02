catch(Exception ex)
            {
                data.responce = " Exception : LEVEL 1: " + Environment.NewLine + ex.Message;
                if(ex.InnerException !=null)
                {
                    data.responce = data.responce + Environment.NewLine + "LEVEL 2:" + Environment.NewLine+ ex.InnerException.Message;
                    if(ex.InnerException.InnerException!=null)
                    {
                        data.responce = data.responce + Environment.NewLine + "LEVEL 3:" + Environment.NewLine + ex.InnerException.InnerException.Message;

                        if (ex.InnerException.InnerException.InnerException != null)
                        {
                            data.responce = data.responce + Environment.NewLine + "LEVEL 4:" + Environment.NewLine + ex.InnerException.InnerException.InnerException.Message;
                            if (ex.InnerException.InnerException.InnerException.InnerException != null)
                            {
                                data.responce = data.responce + Environment.NewLine + "LEVEL 5:" + Environment.NewLine + ex.InnerException.InnerException.InnerException.InnerException.Message;
                            }
                        }
                    }
                }
                
            }
