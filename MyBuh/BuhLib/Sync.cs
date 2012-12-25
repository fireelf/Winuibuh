using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using Windows.Storage;
using System.IO;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BuhLib
{
    public class Sync:Worker
    {
        private string dbname = "data.db";
        private StreamSocket _socket;
        private StreamSocketListener _listener;
        private List<StreamSocket> _connections;
        DateTime DT;
        private bool _connecting;
        private string server_addr;

        public Sync()
            : base()
        {
            _socket = new StreamSocket();
            _listener = new StreamSocketListener();
            _connections = new List<StreamSocket>();
            _connecting = false;
            DT = new DateTime();
        }

        public Sync(String _dbname)
            : base(_dbname)
        {
            _socket = new StreamSocket();
            _listener = new StreamSocketListener();
            _connections = new List<StreamSocket>();
            _connecting = false;
            DT = new DateTime();
        }

        /// <summary>
        /// Выбираем записи для синхронизации
        /// </summary>
        /// <param name="DT">Дата последней синхронизации</param>
        public List<string> GetSyncData(DateTime DT)
        {
            List<string> res = new List<string>();

            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                string[] tm = DT.GetDateTimeFormats();
                var changes = db.Query<ChangesTable>("SELECT _DBString FROM ChangesTable WHERE _ChangesDateTime >'" + tm[46] + "';");
                foreach (ChangesTable ct in changes)
                {
                    res.Add(ct._DBString);
                }
            }

            return res;
        }

        private void InsertData(IList<string> str)
        {
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                foreach (string st in str)
                {
                    var command = db.CreateCommand(st);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void InsertSingleData(string st)
        {
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var command = db.CreateCommand(st);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Отправка данных для синхронзиации
        /// </summary>
        public void sync_send()
        {
            this.Connect();
            this.SendMessage(_socket, this.getLastSync().ToString());
        }

        // Ответ сервера
        private void sync_reply()
        {
            List<string> str = this.GetSyncData(DT);
            foreach (var sock in _connections)
            {
                foreach (string st in str)
                {
                    SendMessage(sock, st);
                }
            }
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        public void sync_rcv(List<string> str)
        {
            this.Listen();
            this.WaitForData(_socket);
        }

        /*public async void CreateSyncFile(List<string> str)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync("sync.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteLinesAsync(file, str);
        }

        public async void ReadSyncFile()
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            var file = await storageFolder.GetFileAsync("sync.txt");
            var text = await FileIO.ReadLinesAsync(file);
            InsertData(text);
        }*/

        /// <summary>
        /// Изменение времени последней синхронизации
        /// <summary>
        public void SetLastSync()
        {
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var command = db.CreateCommand("UPDATE SyncData SET LastSync ='" + DateTime.Now + "';");
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Изменение сервера синхронизации
        /// </summary>
        public void SetServer(string SrvAddr)
        {
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var command = db.CreateCommand("UPDATE SyncData SET ServerAddr ='" + SrvAddr + "';");
                command.ExecuteNonQuery();
            }
        }

        // Перекодировка
        private string Code(string st)
        {
            string str = "UTF8 Encoded string.";
            Encoding srcEncodingFormat = Encoding.GetEncoding("windows-1251");
            Encoding dstEncodingFormat = Encoding.UTF8;
            byte[] originalByteString = srcEncodingFormat.GetBytes(str);
            byte[] convertedByteString = Encoding.Convert(srcEncodingFormat,
            dstEncodingFormat, originalByteString);

            return dstEncodingFormat.GetString(convertedByteString, 0, convertedByteString.Length);
        }

        /// <summary>
        /// Получаем время последней синхроинзации
        /// </summary>
        public DateTime getLastSync()
        {
            DateTime lsync;
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var command = db.CreateCommand("SELECT LastSync FROM SyncData;");
                lsync = command.ExecuteScalar<DateTime>();
            }

            return lsync;
        }

        /// <summary>
        /// Получаем адрес сервера синхронизации
        /// </summary>
        public string getSyncServer()
        {
            string changes;
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var command = db.CreateCommand("SELECT ServerAddr FROM SyncData;");
                changes = command.ExecuteScalar<string>();
            }

            return changes;
        }

        // Слушаем
        async private void Listen()
        {
            _listener.ConnectionReceived += listenerConnectionReceived;
            await _listener.BindServiceNameAsync("3011");
        }

        private void listenerConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            _connections.Add(args.Socket);

            WaitForData(args.Socket);
        }

        // Ожидаем получения данных
        async private void WaitForData(StreamSocket socket)
        {
            var dr = new DataReader(socket.InputStream);
            //dr.InputStreamOptions = InputStreamOptions.Partial;
            var stringHeader = await dr.LoadAsync(4);

            int strLength = dr.ReadInt32();

            uint numStrBytes = await dr.LoadAsync((uint)strLength);
            string msg = dr.ReadString(numStrBytes);

            if (socket.Information.RemoteHostName.DisplayName == this.getSyncServer())
            {
                InsertSingleData(this.Code(msg));
            }
            else
            {
                DT = Convert.ToDateTime(msg);
                this.sync_reply();
            }

            WaitForData(socket);
        }

        // Подключение
        async private void Connect()
        {
            try
            {
                _connecting = true;
                await _socket.ConnectAsync(new HostName(server_addr), "3011");
                _connecting = false;

                WaitForData(_socket);
            }
            catch (Exception ex)
            {
                _connecting = false;
            }
        }

        // Отправка сообщения
        async private void SendMessage(StreamSocket socket, string message)
        {
            var writer = new DataWriter(socket.OutputStream);
            var len = writer.MeasureString(message); // Gets the UTF-8 string length.
            writer.WriteInt32((int)len);
            writer.WriteString(message);
            var ret = await writer.StoreAsync();
            writer.DetachStream();
        }
    }
}
