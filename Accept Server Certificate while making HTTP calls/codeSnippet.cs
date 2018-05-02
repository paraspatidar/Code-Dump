HttpClientHandler handler = new HttpClientHandler();

HttpClient client = new HttpClient();
client.BaseAddress =new Uri(destination);

//register a cert validation call back 
ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(ValidateServerCertificate);
				
		  try
			{
			//do make http call to API , here will go you actual function all which calls the API
			
                HttpResponseMessage response = client.GetAsync(TARGETURL).Result;
                HttpContent content = response.Content;
                string result = content.ReadAsStringAsync().Result;
                data.responce = result;
            }
		   catch(Exception ex)
            {
                //log the Exception
            }
			
			
			

			// here begins the validation method 
		private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at values in app setting and see if this is trusted cert.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                var _thumbprint = System.Configuration.ConfigurationManager.AppSettings["thumbprint"].ToLower();
                var _Subject = System.Configuration.ConfigurationManager.AppSettings["Subject"].ToLower();
                var _SerialNumber = System.Configuration.ConfigurationManager.AppSettings["SerialNumber"].ToLower();
                var _Issuer = System.Configuration.ConfigurationManager.AppSettings["Issuer"].ToLower();

                if (certificate.Issuer.ToLower().Contains(_Issuer)
                        && ((X509Certificate2)certificate).Thumbprint.ToLower() == _thumbprint
                        && ((X509Certificate2)certificate).SerialNumber.ToLower() == _SerialNumber
                        && certificate.Subject.ToLower().Contains(_Subject)
                        )
                {
                    return true;
                }
                //for all other cert which are having issue OR which are not trusted as per app setting , discart the validation
                else
                {
                    return false;
                }
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }
