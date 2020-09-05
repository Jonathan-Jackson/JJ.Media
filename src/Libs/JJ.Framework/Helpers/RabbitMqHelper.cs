using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JJ.Framework.Helpers {

    public static class RabbitMqHelper {

        public static bool TryConnect(string hostName, string username, string password, out Exception ex, int attempts = 5) {
            ex = null;

            for (int i = 1; i <= attempts; i++) {
                try {
                    new ConnectionFactory {
                        HostName = hostName,
                        UserName = username,
                        Password = password
                    }.CreateConnection().Dispose();
                    return true;
                }
                catch (Exception thrown) {
                    ex = thrown;
                    Thread.Sleep(1000 * i);
                }
            }

            return false;
        }

        public static async Task<Exception> TryConnectAsync(string hostName, string username, string password, int attempts = 5) {
            Exception ex = null;

            for (int i = 1; i <= attempts; i++) {
                try {
                    new ConnectionFactory {
                        HostName = hostName,
                        UserName = username,
                        Password = password
                    }.CreateConnection().Dispose();
                    return null;
                }
                catch (Exception thrown) {
                    ex = thrown;
                }

                await Task.Delay(1000 * i);
            }

            return ex;
        }
    }
}