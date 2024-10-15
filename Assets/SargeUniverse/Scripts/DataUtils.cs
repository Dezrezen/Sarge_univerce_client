using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SargeUniverse.Scripts
{
    public static class DataUtils
    {
        public static string Serialize<T>(this T target)
        {
            var xml = new XmlSerializer(typeof(T));
            var writer = new StringWriter();
            xml.Serialize(writer, target);
            return writer.ToString();
        }

        public static T Deserialize<T>(this string target)
        {
            var xml = new XmlSerializer(typeof(T));
            var reader = new StringReader(target);
            return (T)xml.Deserialize(reader);
        }

        public static async Task<string> SerializeAsync<T>(this T target)
        {
            var task = Task.Run(() =>
            {
                var xml = new XmlSerializer(typeof(T));
                var writer = new StringWriter();
                xml.Serialize(writer, target);
                return writer.ToString();
            });
            return await task;
        }

        public static async Task<T> DeserializeAsync<T>(this string target)
        {
            var task = Task.Run(() =>
            {
                var xml = new XmlSerializer(typeof(T));
                var reader = new StringReader(target);
                return (T)xml.Deserialize(reader);
            });
            return await task;
        }

        public static void CopyTo(Stream source, Stream target)
        {
            var bytes = new byte[4096]; int count;
            while ((count = source.Read(bytes, 0, bytes.Length)) != 0)
            {
                target.Write(bytes, 0, count);
            }
        }

        public static async Task<byte[]> CompressAsync(string target)
        {
            var task = Task.Run(() => Compress(target));
            return await task;
        }

        public static byte[] Compress(string target)
        {
            var bytes = Encoding.UTF8.GetBytes(target);
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                CopyTo(msi, gs);
            }
            return mso.ToArray();
        }

        public static async Task<string> DecompressAsync(byte[] bytes)
        {
            var task = Task.Run(() => Decompress(bytes));
            return await task;
        }

        public static string Decompress(byte[] bytes)
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                CopyTo(gs, mso);
            }
            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }
}