
        private string GetCallerMethodName([CallerMemberName] string memberName = "")
        {
            string methodName = memberName;

            try
            {
                StackFrame stackFrame = new StackFrame(3); 
                methodName = stackFrame.GetMethod().Name;
                if(methodName== "MoveNext") //that means it was asyn method thus skip 2 more level
                {
                    stackFrame = new StackFrame(5);
                    methodName = stackFrame.GetMethod().Name;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while finding the caller method/operation name, assigning it to blank.");
                _telemetryClient.TrackTrace("Error while finding the caller method/operation name, assigning it to blank.", SeverityLevel.Error);
                _telemetryClient.TrackTrace(ex.Message, SeverityLevel.Critical);
            }

            return methodName;
        }
