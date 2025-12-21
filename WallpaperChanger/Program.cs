using System;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WallpaperChanger
{
    class Program
    {
        // Importar la función de Windows para cambiar el fondo de pantalla
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        // URL fija de la imagen
        private const string IMAGE_URL = "https://dyj.vercel.app/DaniYJuni.png";
        private const string FOLDER_NAME = "DaniYJunior";
        private const string FILE_NAME = "DaniYJuni.png";

        // Frases románticas
        private static readonly string[] RomanticMessages = new[]
        {
            "Eres la mejor <333",
            "Eres increíble <333",
            "Eres mi mayor regalo <333",
            "Iluminas mi vida <333",
            "Eres mi persona favorita <333",
            "Contigo todo es mejor <333",
            "Eres única y especial <333",
            "Mi mundo es mejor contigo <333",
            "Eres mi felicidad <333",
            "Gracias por existir <333"
        };

        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║   Cambiador de Fondo de Pantalla v3.0     ║");
            Console.WriteLine("║          DaniYJuni Edition                 ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            // Mostrar mensaje romántico aleatorio
            ShowRandomRomanticMessage();
            Console.WriteLine();

            try
            {
                // Verificar conexión a Internet
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("🌐 Verificando conexión a Internet...");
                Console.ResetColor();

                if (!CheckInternetConnection())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ No hay conexión a Internet (WiFi)");
                    Console.WriteLine("   Por favor, conéctate a WiFi e intenta de nuevo.");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Presiona cualquier tecla para salir...");
                    Console.ReadKey();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Conexión a Internet detectada");
                Console.ResetColor();
                Console.WriteLine();

                // Obtener la carpeta de Imágenes (Pictures)
                string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                
                // Crear carpeta DaniYJunior si no existe
                string targetFolder = Path.Combine(picturesPath, FOLDER_NAME);
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"📁 Carpeta creada: {targetFolder}");
                    Console.ResetColor();
                }

                // Ruta completa del archivo
                string filePath = Path.Combine(targetFolder, FILE_NAME);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"📥 Descargando desde: {IMAGE_URL}");
                Console.ResetColor();
                Console.WriteLine();

                // Descargar la imagen con barra de progreso
                await DownloadImageWithProgress(IMAGE_URL, filePath);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Imagen descargada exitosamente");
                Console.ResetColor();
                Console.WriteLine();

                // Mostrar otro mensaje romántico
                ShowRandomRomanticMessage();
                Console.WriteLine();

                // Establecer como fondo de pantalla
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("🖼️  Estableciendo como fondo de pantalla...");
                Console.ResetColor();

                SetWallpaper(filePath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ ¡Fondo de pantalla cambiado exitosamente!");
                Console.ResetColor();
                Console.WriteLine();
                
                // Mensaje final romántico
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("💝 ¡Disfruta tu nuevo fondo de pantalla! 💝");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($"📁 Guardado en: {filePath}");
            }
            catch (HttpRequestException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error al descargar la imagen: {ex.Message}");
                Console.WriteLine("   Verifica que la URL sea correcta y que tengas conexión a Internet.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        /// <summary>
        /// Muestra un mensaje romántico aleatorio
        /// </summary>
        private static void ShowRandomRomanticMessage()
        {
            Random random = new Random();
            string message = RomanticMessages[random.Next(RomanticMessages.Length)];
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"💕 {message} 💕");
            Console.ResetColor();
        }

        /// <summary>
        /// Descarga una imagen con barra de progreso animada en tiempo real
        /// </summary>
        private static async Task DownloadImageWithProgress(string url, string destinationPath)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                client.Timeout = TimeSpan.FromSeconds(30);

                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    long? totalBytes = response.Content.Headers.ContentLength;

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        byte[] buffer = new byte[8192];
                        long totalRead = 0;
                        int bytesRead;

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("⏳ Descargando: ");
                        int progressBarStartLeft = Console.CursorLeft;
                        int progressBarStartTop = Console.CursorTop;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;

                            if (totalBytes.HasValue)
                            {
                                double percentage = (double)totalRead / totalBytes.Value * 100;
                                DrawProgressBar(percentage, progressBarStartLeft, progressBarStartTop);
                            }
                        }

                        // Asegurar que muestre 100% al final
                        if (totalBytes.HasValue)
                        {
                            DrawProgressBar(100, progressBarStartLeft, progressBarStartTop);
                        }

                        Console.ResetColor();
                        Console.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// Dibuja una barra de progreso animada
        /// </summary>
        private static void DrawProgressBar(double percentage, int startLeft, int startTop)
        {
            Console.SetCursorPosition(startLeft, startTop);
            
            int barWidth = 40;
            int filled = (int)(barWidth * percentage / 100);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[");
            
            // Parte llena
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string('█', filled));
            
            // Parte vacía
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', barWidth - filled));
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("]");
            
            // Porcentaje
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($" {percentage:F1}%");
            
            Console.ResetColor();
        }

        /// <summary>
        /// Verifica si hay conexión a Internet
        /// </summary>
        /// <returns>True si hay conexión, False si no</returns>
        private static bool CheckInternetConnection()
        {
            try
            {
                // Verificar si hay alguna interfaz de red activa
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    return false;
                }

                // Intentar hacer ping a un servidor confiable
                using (var ping = new Ping())
                {
                    try
                    {
                        PingReply reply = ping.Send("8.8.8.8", 3000); // Google DNS
                        return reply.Status == IPStatus.Success;
                    }
                    catch
                    {
                        // Si falla el ping, intentar con otro servidor
                        try
                        {
                            PingReply reply = ping.Send("1.1.1.1", 3000); // Cloudflare DNS
                            return reply.Status == IPStatus.Success;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Establece una imagen como fondo de pantalla de Windows
        /// </summary>
        /// <param name="path">Ruta completa de la imagen</param>
        private static void SetWallpaper(string path)
        {
            // Verificar que el archivo existe
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("La imagen no existe", path);
            }

            // Cambiar el fondo de pantalla
            SystemParametersInfo(
                SPI_SETDESKWALLPAPER,
                0,
                path,
                SPIF_UPDATEINIFILE | SPIF_SENDCHANGE
            );
        }
    }
}
