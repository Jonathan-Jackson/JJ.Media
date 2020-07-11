using RabbitMQ.Client;

namespace JJ.Framework.Helpers {

    public static class RabbitMqHelper {

        public static bool TryConnect(string hostName) {
            try {
                new ConnectionFactory {
                    HostName = hostName
                }.CreateConnection().Dispose();
                return true;
            }
            catch {
                return false;
            }
        }
    }
}