open System
open System.IO
open System.Linq;
open Storage.API.Client.Client
open System.Net.Http
open Storage.API.Client

let storageOptions =
    let options = new StorageClientOptions()
    options.Address <- "http://htpc:4682"
    options

let storageClient = new StorageClient(new HttpClient(), storageOptions)

let processAnime path =
    let response = storageClient.ProcessAnime(path) |> Async.AwaitTask |> Async.RunSynchronously
    if response.IsSuccessStatusCode then
        printfn "(ANIME) Succesfully processed: %s" path
    else
        printfn "(ANIME) FAILURE processing: %s %s" path (response.StatusCode.ToString())

let processTvShow path =
    let response = storageClient.ProcessTvShow(path) |> Async.AwaitTask |> Async.RunSynchronously
    if response.IsSuccessStatusCode then
        printfn "(TVSHOW) Succesfully processed: %s" path
    else
        printfn "(TVSHOW) FAILURE processing: %s ~ %s" path (response.StatusCode.ToString())

[<EntryPoint>]
while true do
    printfn "Enter the Directory address"
    printfn "WARNING: The address must be mapped to the storage's configured drives."
    let directoryPath = Console.ReadLine()
    printfn "Processing: %s" directoryPath

    // Get Videos
    let directory = DirectoryInfo(directoryPath)
    let mkvs = directory.EnumerateFiles("*.mkv")
    let mp4s = directory.EnumerateFiles("*.mp4")
    let paths = mkvs.Concat(mp4s).ToArray()
                    |> Array.map(fun(file) -> file.FullName)

    let isAnime = directoryPath.Contains("/anime/", StringComparison.InvariantCultureIgnoreCase)
                || directoryPath.Contains("\\anime\\", StringComparison.InvariantCultureIgnoreCase)

    if isAnime then
        for path in paths do
            processAnime(path);
    else
        for path in paths do
            processTvShow(path);

    printfn "COMPLETE DIRECTORY: %s" directory.FullName