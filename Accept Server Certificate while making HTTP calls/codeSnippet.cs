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


//another approach based on cert file or .pem data :

 private static bool CheckServerCertificate(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // the lazy version here is:
            // return true;

            // better version - check that the CA thumbprint is in the chain
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
            {
                // check that the untrusted ca is in the chain
                var ca = new X509Certificate2("ca.pem");
		    
		    //Or if physical file is not available
		    
                var codedData= AppSettings["pem-certificate_base64-encoded-text"];
                 var decodedstring = Convert.FromBase64String(codedData);
                 // var bytedata=decodedstring.ToArray<byte>();
                 ca =new X509Certificate2(decodedstring);
		    
                var caFound = chain.ChainElements
                    .Cast<X509ChainElement>()
                    .Any(x => x.Certificate.Thumbprint == ca.Thumbprint);

                // note you could also hard-code the expected CA thumbprint,
                // but pretty easy to load it from the pem file that aiven provide

                return caFound;
            }
            return false;
        }
    }

