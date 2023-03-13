// using System;
// using Newtonsoft.Json;
// using WindowsSetupAssistant.Core.Interfaces;
// using WindowsSetupAssistant.Core.Models.IInstallables;
//
// namespace WindowsSetupAssistant.Core.Logic;
//
// public class JsonInstallerConverter : JsonConverter
// {
//     public override bool CanConvert(Type objectType)
//     {
//         return objectType == typeof(IInstallable);
//     }
//
//     public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//     {
//         serializer.Serialize(writer, value);
//     }
//
//     public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
//     {
//         Console.WriteLine(objectType);
//
//         // TODO: This is horrible. Find the right way to do this
//
//         if (objectType == typeof(IInstallable))
//         {
//
//             try
//             {
//                 serializer.Deserialize(reader, typeof(ChocolateyInstaller));
//
//                 return serializer.Deserialize(reader, typeof(ChocolateyInstaller));
//             }
//             catch (JsonSerializationException)
//             {
//             }
//
//             try
//             {
//                 serializer.Deserialize(reader, typeof(ExecutableInstaller));
//
//                 return serializer.Deserialize(reader, typeof(ExecutableInstaller));
//             }
//             catch (JsonSerializationException)
//             {
//             }
//
//             try
//             {
//                 serializer.Deserialize(reader, typeof(ArchiveInstaller));
//
//                 return serializer.Deserialize(reader, typeof(ArchiveInstaller));
//             }
//             catch (JsonSerializationException)
//             {
//             }
//
//             try
//             {
//                 serializer.Deserialize(reader, typeof(PortableApplicationInstaller));
//
//                 return serializer.Deserialize(reader, typeof(PortableApplicationInstaller));
//             }
//             catch (JsonSerializationException)
//             {
//             }
//             
//             Console.WriteLine();
//         }
//
//         
//         throw new JsonSerializationException();
//     }
// }