# 🖼️ Cambiador de Fondo de Pantalla - DaniYJuni Edition

Aplicación de consola .NET que descarga una imagen desde una URL y la establece como fondo de pantalla de Windows 11.

## ✨ Características

- 📥 Descarga imágenes desde cualquier URL
- 💾 Guarda automáticamente en la carpeta Descargas
- 🖼️ Establece la imagen como fondo de pantalla
- 🎨 Interfaz de consola colorida y moderna
- ⚡ Rápido y eficiente
- 🔒 Compatible con Windows 11/10

## 🚀 Uso

### Opción 1: Ejecutar directamente
```bash
dotnet run
```

### Opción 2: Compilar y ejecutar
```bash
dotnet build
dotnet run
```

### Opción 3: Crear ejecutable
```bash
dotnet publish -c Release -r win-x64 --self-contained
```
El ejecutable estará en: `bin\Release\net8.0\win-x64\publish\WallpaperChanger.exe`

## 📋 Requisitos

- .NET 6.0 o superior
- Windows 10/11
- Conexión a Internet (para descargar imágenes)

## 🎯 Cómo funciona

1. Al ejecutar, te pedirá una URL de imagen (o usa la predeterminada)
2. Descarga la imagen desde la URL
3. La guarda en tu carpeta de Descargas con el nombre `DaniYJuni_Wallpaper_[fecha].jpg`
4. La establece automáticamente como fondo de pantalla

## 🔧 Personalización

Puedes cambiar la URL predeterminada editando esta línea en `Program.cs`:

```csharp
string imageUrl = "https://tu-url-aqui.com/imagen.jpg";
```

## 📝 Notas

- Las imágenes se guardan con marca de tiempo para evitar sobrescribir
- Soporta formatos: JPG, PNG, BMP
- Usa la API de Windows para cambiar el fondo (SystemParametersInfo)

---

Hecho con ❤️ para DaniYJuni
