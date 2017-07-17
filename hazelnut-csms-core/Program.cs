using System;
using System.Runtime.InteropServices;
using Hazelnut.CLIApp;

namespace hazelnut_csms_core
{
    class Program
    {
        static void Main(string[] args)
        {
            int huserId = 1;
            if (args.Length > 0) {
                try {
                    huserId = Convert.ToInt32(args[0]);
                } catch(Exception ex) {
                    Console.WriteLine(@"Invalid argument. You must pass the user id to sync. Or 
                        leaving blank to sync user 1");
                    Console.WriteLine(ex);
                    return;
                }
            }
            Console.WriteLine("--Starting Hazelnut CSMS CLIAppClient for user {0}--", huserId);
            CLIAppClient cliAppClient = new CLIAppClient(huserId);
            try {
                cliAppClient.ExecuteSync();
                Console.WriteLine("--Hazelnut CSMS CLIAppClient Finished--");
            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine("--ERROR, something went wrong and CSMS CLIAppClient had to exit--");
            }
        }
    }
}
    