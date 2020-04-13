namespace Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string hostname = "127.0.0.1";
            int port = 8333;

            using (var nw = new NodeWalker.NodeWalker(hostname, port))
            {
                nw.Run();
            }
        }
    }
}